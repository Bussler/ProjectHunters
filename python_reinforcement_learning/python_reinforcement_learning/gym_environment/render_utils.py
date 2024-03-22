import numpy as np
import pygame
from gymnasium import spaces


class PyGameRenderer:
    def __init__(self, window_size, render_fps, environment_size, render_mode) -> None:
        self.window_size = window_size
        self.render_fps = render_fps
        self.environment_size = environment_size
        self.render_mode = render_mode

        self.pix_square_size = self.window_size / environment_size  # The size of a single grid square in pixels

        pygame.init()
        pygame.display.init()
        self.window = pygame.display.set_mode((self.window_size, self.window_size))
        self.canvas = pygame.Surface((self.window_size, self.window_size))
        self.clock = pygame.time.Clock()

    def render_frame(self, observation: spaces.Dict) -> None:
        self.canvas.fill((255, 255, 255))

        player_location = observation["player"]
        enemy_locations = observation["enemies"]

        # TODO map the locations correctly

        # draw player
        pygame.draw.circle(
            self.canvas,
            (0, 0, 255),
            (player_location + 0.5) * self.pix_square_size,
            self.pix_square_size / 3,
        )

        # draw enemies
        for enemy_location in enemy_locations:
            pygame.draw.circle(
                self.canvas,
                (255, 0, 0),
                (enemy_location + 0.5) * self.pix_square_size,
                self.pix_square_size / 3,
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
            return np.transpose(np.array(pygame.surfarray.pixels3d(self.canvas)), axes=(1, 0, 2))

    def free_ressources(self) -> None:
        pygame.display.quit()
        pygame.quit()
