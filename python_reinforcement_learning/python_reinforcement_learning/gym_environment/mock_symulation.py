import numpy as np

from python_reinforcement_learning.gym_environment.configs import MockSimulationConfig

DELTA_TIME = 0.02  # simulation time step, similar to UNITY's Time.deltaTime
BOUNDING_BOX_SIZE = 1  # size of collision bounding box for player and enemies


class Player:
    postion: np.array = np.array([0.0, 0.0])
    speed: float = 20
    lifes: int = 1

    def __init__(self, field_size: int) -> None:
        self.field_size = field_size
        self.reset()

    def move(self, direction: np.array) -> None:
        direction_norm = np.linalg.norm(direction, 1)
        if direction_norm > 0:
            direction_normed = direction / direction_norm
            new_position = self.postion + direction_normed * self.speed * DELTA_TIME
            self.postion = np.clip(new_position, -self.field_size // 2, self.field_size // 2)

    def reset(self) -> None:
        self.postion = np.array([0.0, 0.0])
        self.lifes = 1

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
    speed: float = 16
    steps_to_stay_alive: int = 20
    lives = 1

    def __init__(self, field_size: int, live_for_steps: int) -> None:
        self.field_size = field_size
        self.live_for_steps = live_for_steps
        self.reset()

    def move(self, player_position: np.array) -> None:
        direction = player_position - self.postion

        direction_norm = np.linalg.norm(direction, 1)
        if direction_norm > 0:
            direction_normed = direction / direction_norm
            self.postion += direction_normed * self.speed * DELTA_TIME

    def reset(self) -> None:
        self.reset_position()
        self.steps_to_stay_alive = self.live_for_steps
        self.lives = 1

    def reset_position(self) -> None:
        self.postion = np.random.uniform(low=-self.field_size // 2, high=self.field_size // 2, size=(2,))

    def get_observation(self) -> np.array:
        return self.postion

    def take_step_damage(self) -> bool:
        """Reduces time to live. If time to live is 0, the enemy is reset and takes damage.

        Returns true if the enemy is still alive, false otherwise.

        Returns:
            bool: _description_
        """
        self.steps_to_stay_alive -= 1
        if self.steps_to_stay_alive <= 0:
            self.reset_position()
            return self.take_damage()
        return True

    def take_damage(self) -> bool:
        self.lives -= 1
        if self.lives <= 0:
            return False
        return True


class MockSimulation:
    player: Player = None
    enemies: list[Enemy] = None

    def __init__(self, config: MockSimulationConfig) -> None:
        self.field_size = config.field_size
        self.number_enemies = config.number_enemies
        self.enemy_live_for_steps = config.enemy_live_for_steps
        self.player = Player(config.field_size)
        self.reset()

    def reset(self) -> None:
        self.player.reset()

        self.enemies = [Enemy(self.field_size, self.enemy_live_for_steps) for _ in range(self.number_enemies)]
        for enemy in self.enemies:
            enemy.reset()

    def perform_action(self, move_direction: np.array) -> tuple[np.array, dict, bool]:
        """Perform a step in the simulation.

        The player moves in the given direction.
        The enemies move towards the player.
        If an enemy is close enough to the player, the player takes damage.
        If an enemy has no more time to live, it is reset and takes damage.

        Args:
            move_direction (np.array): direction in which the player should move

        Returns:
            tuple[np.array, dict, bool]: observation, info, done
        """
        self.player.move(move_direction)

        enemies_to_remove = []
        for enemy in self.enemies:
            enemy.move(self.player.postion)

            if not enemy.take_step_damage():
                enemies_to_remove.append(enemy)
                continue

            if np.linalg.norm(self.player.postion - enemy.postion, 1) < BOUNDING_BOX_SIZE:
                self.player.take_damage()
                enemy.reset_position()

        for i in enemies_to_remove:
            self.enemies.remove(i)

        observation = self.create_observation()
        info = self.player.get_info()
        done = not self.player.is_alive()

        return observation, info, done

    def create_observation(self) -> np.array:
        return {
            "player": self.player.get_observation(),
            "enemies": [enemy.get_observation() for enemy in self.enemies],
        }

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
