from typing import Union

import gymnasium as gym
import numpy as np
from gymnasium import spaces
from ray.rllib.utils.spaces.repeated import Repeated

from python_reinforcement_learning.gym_environment.configs import (
    HunterEnvironmentConfig,
    MockSimulationConfig,
    RendererConfig,
    RendererMode,
)
from python_reinforcement_learning.gym_environment.mock_symulation import MockSimulation
from python_reinforcement_learning.gym_environment.render_utils import PyGameRenderer


class HunterEnvironment(gym.Env):
    mock_environment: MockSimulation = None

    def __init__(
        self,
        env_config: Union[HunterEnvironmentConfig, dict],
    ):
        if isinstance(env_config, dict):
            self.config = HunterEnvironmentConfig.from_dict(env_config)
        else:
            self.config = env_config

        self.size = self.config.size
        self.step_size = 1
        self.current_timestep = 0
        self.max_timestep = self.config.max_timestep

        # Mock environment for training, to not query the real unity environment all the time
        if self.config is not None:
            self.mock_environment = MockSimulation(self.config.simulation_config)
        if self.config.udp_address is not None:
            self.udp_address = self.config.udp_address

        self.renderer = PyGameRenderer(self.config.render_config, self.config.size)
        self.render_mode = self.config.render_config.render_mode

        self.player_location: np.array = np.array([0, 0])
        self.enemy_locations: list[np.array] = []

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

        if self.render_mode == RendererMode.Human:
            self.renderer.render_frame(self._get_obs())

        return observation, reward, terminated, False, info

    def render(self):
        if self.render_mode == RendererMode.RGBArray and self.renderer is not None:
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
    sim_config = MockSimulationConfig(number_enemies=4, field_size=20, enemy_live_for_steps=15)
    render_config = RendererConfig(window_size=512, render_fps=4, render_mode=RendererMode.RGBArray, store_dir="images")
    env_config = HunterEnvironmentConfig(
        size=20, max_timestep=1000, udp_address=None, simulation_config=sim_config, render_config=render_config
    )
    env = HunterEnvironment(env_config)
    env.reset()
    for i in range(20):
        observation, reward, terminated, truncated, info = env.step(1)
        if terminated:
            break
        env.render()
    env.close()
