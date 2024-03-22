import gymnasium as gym
import numpy as np
from gymnasium import spaces
from ray.rllib.env.env_context import EnvContext

from python_reinforcement_learning.gym_environment.mock_symulation import MockSimulation


class HunterEnvironment(gym.Env):
    action_space = spaces.Discrete(9)
    observation_space = spaces.Dict(
        {
            "agent": spaces.Box(0, 255, shape=(2,), dtype=int),
            "enemies": spaces.Box(0, 255, shape=(2,), dtype=int),  # TODO extend to multiple targets
        }
    )

    mock_environment: MockSimulation = None

    def __init__(
        self,
        env_config: EnvContext = {
            "size": 50,
            "max_timestep": 1000,
            "udp_address": "localhost:1337",
            "mock_environment": None,
        },
    ):
        self.size = env_config["size"]
        self.step_size = 1
        self.current_timestep = 0
        self.max_timestep = env_config["max_timestep"]
        self.udp_address = env_config["udp_address"]
        self.agent_location = np.array([0, 0])
        self.target_locations = np.array([0, 0])  # TODO extend to multiple targets

        # Mock environment for training, to not query the real unity environment all the time
        if env_config["mock_environment"] is not None:
            self.mock_environment = env_config["mock_environment"]

        observation_space = spaces.Dict(
            {
                "agent": spaces.Box(0, self.size - 1, shape=(2,), dtype=int),
                "enemies": spaces.Box(0, self.size - 1, shape=(2,), dtype=int),
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

        self.agent_location = np.array([0, 0])
        self.target_locations = np.array([0, 0])

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
        reward = -1000 if terminated else 1
        observation = self._get_obs()
        info = self._get_info()

        return observation, reward, terminated, False, info

    def _test_terminated(self):
        if self.current_timestep >= self.max_timestep:
            return True
        if self.mock_environment is not None:
            return not self.mock_environment.is_alive()
        return False  # TODO

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
            self.target_locations = observation["enemies"]
        else:
            pass  # TODO send action to unity environment
