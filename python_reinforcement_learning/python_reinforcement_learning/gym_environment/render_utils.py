import os

import numpy as np
import pygame
from gymnasium import spaces

from python_reinforcement_learning.gym_environment.configs import (
    MockSimulationConfig,
    RendererConfig,
    RendererMode,
)


class PyGameRenderer:
    def __init__(
        self,
        config: RendererConfig,
        field_size: int,
    ) -> None:
        self.window_size = config.window_size
        self.render_fps = config.render_fps
        self.environment_size = field_size
        self.render_mode: RendererMode = config.render_mode

        self.pix_square_size = self.window_size / field_size  # The size of a single grid square in pixels

        self.image_cache: list[np.array] = []
        self.store_dir = config.store_dir

        self._init_pygame()

    def _init_pygame(self) -> None:
        pygame.init()
        pygame.display.init()
        self.window = pygame.display.set_mode((self.window_size, self.window_size))
        self.canvas = pygame.Surface((self.window_size, self.window_size))
        self.clock = pygame.time.Clock()

    def render_frame(self, observation: spaces.Dict) -> None:
        if self.render_mode == RendererMode.Nothing:
            return

        self.canvas.fill((255, 255, 255))

        player_location = observation["player"]
        enemy_locations = observation["enemies"]

        # TODO map the locations correctly

        # draw player
        pygame.draw.circle(
            self.canvas,
            (0, 0, 255),
            (player_location + (self.environment_size // 2)) * self.pix_square_size,
            self.pix_square_size / 2,
        )

        # draw enemies
        for enemy_location in enemy_locations:
            pygame.draw.circle(
                self.canvas,
                (255, 0, 0),
                (enemy_location + (self.environment_size // 2)) * self.pix_square_size,
                self.pix_square_size / 2,
            )

        # Finally, add some gridlines
        for x in range(self.environment_size + 1):
            pygame.draw.line(
                self.canvas,
                0,
                (0, self.pix_square_size * x),
                (self.window_size, self.pix_square_size * x),
                width=1,
            )
            pygame.draw.line(
                self.canvas,
                0,
                (self.pix_square_size * x, 0),
                (self.pix_square_size * x, self.window_size),
                width=1,
            )

        # copy our drawings from `canvas` to the visible window
        if self.render_mode == "human":
            # The following line copies our drawings from `canvas` to the visible window
            self.window.blit(self.canvas, self.canvas.get_rect())
            pygame.event.pump()
            pygame.display.update()

            # We need to ensure that human-rendering occurs at the predefined framerate.
            # The following line will automatically add a delay to keep the framerate stable.
            self.clock.tick(self.render_fps)
        else:  # rgb_array
            image = np.transpose(np.array(pygame.surfarray.pixels3d(self.canvas)), axes=(1, 0, 2))
            self.image_cache.append(image)
            return image

    def free_ressources(self) -> None:
        self.store_images()
        pygame.display.quit()
        pygame.quit()

    def store_images(self) -> None:
        if len(self.image_cache) <= 0:
            return

        directory = os.path.join(self.store_dir)
        if not os.path.exists(directory):
            os.makedirs(directory)

        for i, image in enumerate(self.image_cache):
            pygame.image.save(pygame.surfarray.make_surface(image), os.path.join(directory, f"frame_{i}.png"))
        self.image_cache = []
