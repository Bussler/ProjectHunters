import gymnasium as gym
import numpy as np
from gymnasium import spaces


class GridWorldEnv(gym.Env):
    action_space = spaces.Discrete(4)  # TODO
    observation_space = spaces.Box(low=0, high=255, shape=(64, 64, 3), dtype=np.uint8)  # TODO

    def __init__(self, size=100, udp_address="localhost:1337"):
        # TODO
        self.size = size
        self.step_size = 0.02
        self.udp_address = udp_address
        self.agent_location = np.array([0, 0, 0])
        self.target_locations = [np.array([0, 0, 0])]
        pass

    # translate environment state to observation
    def _get_obs(self) -> dict:
        return {"agent": self.agent_location, "target": self.target_locations}

    # auxiliary information returned by the environment
    def _get_info(self) -> dict:
        return {"info": None}  # TODO

    def reset(self, seed=None) -> tuple[dict, dict]:
        super().reset(seed=seed)

        self.agent_location = np.array([0, 0, 0])
        self.target_locations = [np.array([0, 0, 0])]

        observation = self._get_obs()
        info = self._get_info()

        return observation, info

    def step(self, action):
        direction = self._handle_action[action]
        self._communicate_action(direction)

        terminated = self._test_terminated()
        reward = 1 if terminated else 0  # Binary sparse rewards
        observation = self._get_obs()
        info = self._get_info()

        return observation, reward, terminated, False, info

    def _test_terminated(self):
        return False  # TODO

    def _handle_action(self, action) -> np.array:
        return np.array([0, 0, 0])  # TODO

    def _communicate_action(self, direction: np.array) -> None:
        # use `np.clip` to make sure we don't leave the grid
        self.agent_location = np.clip(self.agent_location + direction, 0, self.size - 1)

        # TODO
        pass
