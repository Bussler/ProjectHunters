import gymnasium as gym
import numpy as np
from gymnasium import spaces
from ray.rllib.env.env_context import EnvContext


class HunterEnvironment(gym.Env):
    action_space = spaces.Discrete(4)  # TODO
    observation_space = spaces.Dict(
        {
            "agent": spaces.Box(0, 255, shape=(3,), dtype=int),
            "enemies": spaces.Box(0, 255, shape=(3,), dtype=int),  # TODO extend to multiple targets
        }
    )

    def __init__(self, env_config: EnvContext = {"size": 100, "max_timestep": 1000, "udp_address": "localhost:1337"}):
        # TODO
        self.size = env_config["size"]
        self.step_size = 1
        self.current_timestep = 0
        self.max_timestep = env_config["max_timestep"]
        self.udp_address = env_config["udp_address"]
        self.agent_location = np.array([0, 0, 0])
        self.target_locations = np.array([0, 0, 0])  # TODO extend to multiple targets

        observation_space = spaces.Dict(
            {
                "agent": spaces.Box(0, self.size - 1, shape=(3,), dtype=int),
                "enemies": spaces.Box(0, self.size - 1, shape=(3,), dtype=int),
            }
        )

        print("HunterEnvironment initialized")

    # translate environment state to observation
    def _get_obs(self) -> spaces.Dict:
        return {"agent": self.agent_location, "enemies": self.target_locations}

    # auxiliary information returned by the environment
    def _get_info(self) -> dict:
        return {"current_timestep": self.current_timestep}

    def reset(self, *, seed=None, options=None) -> tuple[spaces.Dict, dict]:
        super().reset(seed=seed)

        self.current_timestep = 0

        self.agent_location = np.array([0, 0, 0])
        self.target_locations = np.array([0, 0, 0])

        observation = self._get_obs()
        info = self._get_info()

        return observation, info

    def step(self, action):
        direction = self._handle_action(action)
        self._communicate_action(direction)

        self.current_timestep += 1

        terminated = self._test_terminated()
        reward = 1 if terminated else 0  # Binary sparse rewards
        observation = self._get_obs()
        info = self._get_info()

        return observation, reward, terminated, False, info

    def _test_terminated(self):
        if self.current_timestep >= self.max_timestep:
            return True
        return True  # TODO

    def _handle_action(self, action) -> np.array:
        return np.array([0, 0, 0])  # TODO

    def _communicate_action(self, direction: np.array) -> None:
        # use `np.clip` to make sure we don't leave the grid
        self.agent_location = np.clip(self.agent_location + direction, 0, self.size - 1)

        # TODO
        pass
