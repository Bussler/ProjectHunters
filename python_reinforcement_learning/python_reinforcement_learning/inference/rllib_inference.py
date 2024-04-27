import ray
from ray.rllib.algorithms.algorithm import Algorithm

from python_reinforcement_learning.gym_environment.configs import (
    HunterEnvironmentConfig,
    MockSimulationConfig,
    RendererConfig,
    RendererMode,
)
from python_reinforcement_learning.gym_environment.project_hunter_environment import (
    HunterEnvironment,
)


def inference_rllib(env: HunterEnvironment, model_path: str, num_episodes=10):
    ray.init()
    # Load the model
    algo = Algorithm.from_checkpoint(model_path)

    episode_reward = 0
    terminated = truncated = False

    obs, info = env.reset()
    # Run the episodes

    while not terminated and not truncated:
        action = algo.compute_single_action(obs)
        obs, reward, terminated, truncated, info = env.step(action)
        env.render()
        episode_reward += reward

    env.close()
    algo.stop()
    ray.shutdown()


if __name__ == "__main__":
    sim_config = MockSimulationConfig(number_enemies=4, field_size=20, enemy_live_for_steps=40)
    render_config = RendererConfig(
        window_size=512, render_fps=4, render_mode=RendererMode.RGBArray, store_dir="images_inference"
    )
    env_config = HunterEnvironmentConfig(
        size=20, max_timestep=1000, udp_address=None, simulation_config=sim_config, render_config=render_config
    )
    env = HunterEnvironment(env_config)
    inference_rllib(env, "C:/unityProjects/ProjectHunters/python_reinforcement_learning/rllib_checkpoints")
