import numpy as np

DELTA_TIME = 0.02  # simulation time step, similar to UNITY's Time.deltaTime
BOUNDING_BOX_SIZE = 1  # size of collision bounding box for player and enemies


class Player:
    postion: np.array = np.array([0.0, 0.0])
    speed: float = 15
    lifes: int = 3

    def __init__(self, field_size: int) -> None:
        self.field_size = field_size
        self.reset()

    def move(self, direction: np.array) -> None:
        direction_normed = direction / np.linalg.norm(direction, 1)
        new_position = self.postion + direction_normed * self.speed * DELTA_TIME
        self.postion = np.clip(new_position, -self.field_size // 2, self.field_size // 2)

    def reset(self) -> None:
        self.postion = np.array([0.0, 0.0])
        self.lifes = 3

    def get_observation(self) -> np.array:
        return self.postion

    def get_info(self) -> dict:
        return {"lifes": self.lifes}

    def take_damage(self) -> None:
        self.lifes -= 1

    def is_alive(self) -> bool:
        return self.lifes > 0


class Enemy:
    postion: np.array = np.array([0.0, 0.0])
    speed: float = 13
    steps_to_stay_alive: int = 15

    def __init__(self, field_size: int, live_for_steps: int) -> None:
        self.field_size = field_size
        self.live_for_steps = live_for_steps

    def move(self, player_position: np.array) -> None:
        direction = player_position - self.postion
        direction_normed = direction / np.linalg.norm(direction, 1)
        self.postion += direction_normed * self.speed * DELTA_TIME

    def reset(self) -> None:
        self.postion = np.random.uniform(low=-self.field_size // 2, high=self.field_size // 2, size=(2,))
        self.steps_to_stay_alive = self.live_for_steps

    def get_observation(self) -> np.array:
        return self.postion

    def take_damage(self) -> None:
        self.steps_to_stay_alive -= 1
        if self.steps_to_stay_alive <= 0:
            return False
        return True


class MockSimulation:
    player: Player = None
    enemies: list[Enemy] = None

    def __init__(self, number_enemies: int = 4, field_size: int = 50, enemy_live_for_steps: int = 15) -> None:
        self.field_size = field_size
        self.player = Player(field_size)
        self.enemies = [Enemy(self.field_size, enemy_live_for_steps) for _ in range(number_enemies)]
        self.reset()

    def reset(self) -> None:
        self.player.reset()
        for enemy in self.enemies:
            enemy.reset()

    def perform_action(self, move_direction: np.array) -> tuple[np.array, dict, bool]:
        self.player.move(move_direction)

        enemies_to_remove = []
        for enemy in self.enemies:
            enemy.move(self.player.postion)

            if not enemy.take_damage():
                enemies_to_remove.append(enemy)
                continue

            if np.linalg.norm(self.player.postion - enemy.postion, 1) < BOUNDING_BOX_SIZE:
                self.player.take_damage()
                enemy.reset()

        for i in enemies_to_remove:
            self.enemies.remove(i)

        observation = {
            "player": self.player.get_observation(),
            "enemies": [enemy.get_observation() for enemy in self.enemies],
        }
        info = self.player.get_info()
        done = not self.player.is_alive()

        return observation, info, done

    def is_alive(self) -> bool:
        return self.player.is_alive()

    def is_won(self) -> bool:
        return len(self.enemies) == 0


if __name__ == "__main__":
    mock_simulation = MockSimulation()
    for _ in range(100):
        print(f"ITER: {_}", mock_simulation.perform_action(np.array([1, 1])))
        if not mock_simulation.is_alive() or mock_simulation.is_won():
            break
