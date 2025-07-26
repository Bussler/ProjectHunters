# ProjectHunters

[**Project**](https://bussler.github.io/#/project-hunters)

## Summary

This project explores the intersection of deep reinforcement learning and game development through training a PPO (Proximal Policy Optimization) agent on a custom top-down VampireSurvivors clone.  

The implementation features a Unity-based game environment that serves as a training ground for the reinforcement learning agent, with a custom OpenAI Gym environment facilitating communication between the Python-based RL training pipeline and Unity via UDP sockets.  
To optimize training efficiency, the project includes a script-based abstraction layer that mocks the Unity environment during training, while reserving the full Unity environment for final agent inference and evaluation.  
Training is conducted using [RLLIB](https://docs.ray.io/en/latest/rllib/index.html), leveraging open-source reinforcement learning capabilities to achieve robust agent performance in complex game scenarios.

## Results

The PPO agent is able to learn basic evasive behaviours of incoming enemies relatively quickly after about 1000 distributed training iterations (1h of training).  
The following gif shows the learning progress of the agent after 2000 training iterations: The agent is able to successfully evade enemies, until they die automatically.  
![Results](images_inference/0/inference.gif)

## Setup

### Unity

- Install UnityHub
- Install Unity. Unity Version for development: 2022.3.17f1 (lts).
    - Install Visual Studio with unity
    - If Visual Studio already installed: install Visual Studio extension `Unity Game Development`
- Required Unity Packages:
    - Input System (1.7.0)
    - [LootLocker (2.1.3)](https://github.com/lootlocker/unity-sdk) (requires [git](https://git-scm.com/) on your system)
    - TextMeshPro (3.0.6)

### Python Reinforcement Learning

- Install [python](https://www.python.org/downloads/), [poetry](https://python-poetry.org/docs/)
- Navigate to `python_reinforcement_learning`
- Execute `poetry install`. The python environment with all needed packages should be installed.
