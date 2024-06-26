import ray
from ray.rllib.algorithms.algorithm import Algorithm
from ray.rllib.algorithms.ppo import PPOConfig
from ray.rllib.evaluation.metrics import summarize_episodes
from ray.rllib.evaluation.worker_set import WorkerSet
from ray.rllib.utils.typing import ResultDict
from ray.tune.logger import pretty_print

from python_reinforcement_learning.gym_environment.configs import (
    HunterEnvironmentConfig,
    MockSimulationConfig,
    RendererConfig,
    RendererMode,
)
from python_reinforcement_learning.gym_environment.mock_symulation import MockSimulation
from python_reinforcement_learning.gym_environment.project_hunter_environment import (
    HunterEnvironment,
)


def render_evaluation(algorithm: Algorithm, eval_workers: WorkerSet) -> ResultDict:
    # Play and render one episode on an evaluation worker environment
    local_env = eval_workers.local_worker().env
    if local_env is not None:
        obs, info = local_env.reset()
        terminated = truncated = False
        while not terminated and not truncated:
            action = algorithm.compute_single_action(obs)
            obs, reward, terminated, truncated, info = local_env.step(action)
            local_env.render()

    metrics = []
    for i in range(algorithm.evaluation_config.evaluation_duration):
        print(f"Running evaluation iteration {i+1}/{algorithm.evaluation_config.evaluation_duration}")
        metrics_all_workers = eval_workers.foreach_worker(
            func=lambda worker: (worker.sample(), worker.get_metrics[1]), local_worker=False
        )
        for metric in metrics_all_workers:
            metrics.extend(metric)

    results = summarize_episodes(metrics)
    return results


def train_rllib():
    # Optionally: Use Ray Tune to search for the best hyperparameters
    ray.init()

    sim_config = MockSimulationConfig(number_enemies=4, field_size=20, enemy_live_for_steps=40)
    render_config = RendererConfig(
        window_size=512, render_fps=4, render_mode=RendererMode.RGBArray, store_dir="rllib_train_images"
    )
    env_config = HunterEnvironmentConfig(
        size=20, max_timestep=1000, udp_address=None, simulation_config=sim_config, render_config=render_config
    )

    config = (
        PPOConfig()
        .environment(
            HunterEnvironment,
            env_config=env_config.to_dict(),
        )
        .framework("torch")
        .rollouts(num_rollout_workers=0)
        .resources(num_gpus=1)
        .evaluation(
            custom_evaluation_function=render_evaluation,
            evaluation_interval=1000,
            evaluation_num_workers=0,
            evaluation_duration=10,
        )
    )

    stop = {
        "training_iterations": 10000,
        "timesteps_total": 10000000,
        "episode_reward_mean": 500,
        "save_iterations": 1000,
        "print_iterations": 1000,
    }

    # use fixed learning rate instead of grid search (needs tune)
    config.lr = 1e-3
    algo = config.build()

    # run manual training loop and print results after each iteration
    for i in range(stop["training_iterations"]):
        result = algo.train()
        if (
            result["timesteps_total"] >= stop["timesteps_total"]
            or result["episode_reward_mean"] >= stop["episode_reward_mean"]
        ):
            print("Reached stopping criteria, stopping training.")
            break

        if i % stop["save_iterations"] == 0:
            checkpoint_dir = algo.save("rllib_checkpoints").checkpoint.path
            print(f"Checkpoint for episode {i} saved in directory {checkpoint_dir}")

        if i % stop["print_iterations"] == 0:
            print(pretty_print(result))

    checkpoint_dir = algo.save("rllib_checkpoints").checkpoint.path
    print(f"Checkpoint saved in directory {checkpoint_dir}")

    algo.evaluate()

    algo.stop()
    ray.shutdown()


if __name__ == "__main__":
    train_rllib()
