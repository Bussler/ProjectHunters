import gymnasium as gym
import numpy as np
from gymnasium import spaces
from ray.rllib.env.env_context import EnvContext
from ray.rllib.utils.spaces.repeated import Repeated

from python_reinforcement_learning.gym_environment.mock_symulation import MockSimulation
from python_reinforcement_learning.gym_environment.render_utils import PyGameRenderer


class HunterEnvironment(gym.Env):
    mock_environment: MockSimulation = None

    def __init__(
        self,
        env_config: EnvContext = {
            "size": 20,
            "max_timestep": 1000,
            "udp_address": "localhost:1337",
            "mock_environment": MockSimulation(4, 20, 15),
            "render_mode": "rgb_array",
        },
    ):
        self.size = env_config["size"]
        self.step_size = 1
        self.current_timestep = 0
        self.max_timestep = env_config["max_timestep"]
        self.udp_address = env_config["udp_address"]
        self.render_mode = env_config["render_mode"]

        self.player_location: np.array = np.array([0, 0])
        self.enemy_locations: list[np.array] = []

        # Mock environment for training, to not query the real unity environment all the time
        if env_config["mock_environment"] is not None:
            self.mock_environment = env_config["mock_environment"]

        if self.render_mode == "human" or self.render_mode == "rgb_array":
            self.renderer = PyGameRenderer(
                window_size=512, render_fps=4, environment_size=self.size, render_mode=self.render_mode
            )

        self.action_space = spaces.Discrete(9)
        self.observation_space = spaces.Dict(
            {
                "player": spaces.Box(-self.size - 1, self.size - 1, shape=(2,), dtype=int),
                "enemies": Repeated(spaces.Box(-self.size - 1, self.size - 1, shape=(2,), dtype=int), max_len=100),
            }
        )

        print("HunterEnvironment initialized")

    # translate environment state to observation
    def _get_obs(self) -> spaces.Dict:
        return {"player": self.player_location, "enemies": self.enemy_locations}

    # auxiliary information returned by the environment
    def _get_info(self) -> dict:
        return {"current_timestep": self.current_timestep}

    def reset(self, *, seed=None, options=None) -> tuple[spaces.Dict, dict]:
        super().reset(seed=seed)

        self.current_timestep = 0

        self.player_location = np.array([0, 0])
        self.enemy_locations = np.array([0, 0])

        if self.mock_environment is not None:
            self.mock_environment.reset()

        observation = self._get_obs()
        info = self._get_info()

        return observation, info

    def step(self, action):
        direction = self._handle_action(action)
        self._communicate_action(direction)

        self.current_timestep += self.step_size

        terminated = self._test_terminated()
        reward = self._calculate_reward()
        observation = self._get_obs()
        info = self._get_info()

        if self.render_mode == "human":
            self.renderer.render_frame(self._get_obs())

        return observation, reward, terminated, False, info

    def render(self):
        if self.render_mode == "rgb_array" and self.renderer is not None:
            return self.renderer.render_frame(self._get_obs())
        else:
            return None

    def close(self):
        if self.renderer is not None:
            self.renderer.free_ressources()
        # TODO close unity environment

    def _test_terminated(self):
        if self.current_timestep >= self.max_timestep:
            return True
        if self.mock_environment is not None:
            return not self.mock_environment.is_alive() or self.mock_environment.is_won()
        return False  # TODO

    def _calculate_reward(self):
        if self.mock_environment is not None:
            if not self.mock_environment.is_alive():
                return -1000
            if self.mock_environment.is_won():
                return 1
        return 1

    def _handle_action(self, action) -> np.array:
        if action == 0:
            return np.array([0, 0])
        if action == 1:
            return np.array([0, 1])
        if action == 2:
            return np.array([1, 0])
        if action == 3:
            return np.array([0, -1])
        if action == 4:
            return np.array([-1, 0])
        if action == 5:
            return np.array([1, 1])
        if action == 6:
            return np.array([1, -1])
        if action == 7:
            return np.array([-1, 1])
        if action == 8:
            return np.array([-1, -1])

    def _communicate_action(self, direction: np.array) -> None:
        if self.mock_environment is not None:
            observation, info, done = self.mock_environment.perform_action(direction)
            self.player_location = observation["player"]
            self.enemy_locations = observation["enemies"]
        else:
            pass  # TODO send action to unity environment


if __name__ == "__main__":
    env = HunterEnvironment()
    env.reset()
    for i in range(15):
        observation, reward, terminated, truncated, info = env.step(1)
        if terminated:
            break
        env.render()
    env.close()
