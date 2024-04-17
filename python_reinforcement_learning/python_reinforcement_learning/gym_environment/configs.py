import enum
from typing import Optional

from pydantic import BaseModel


class RendererMode(enum.Enum):
    Human = "human"
    RGBArray = "rgb_array"
    Nothing = "nothing"


class RendererConfig(BaseModel):
    window_size: int
    render_fps: int
    render_mode: RendererMode = RendererMode.RGBArray
    store_dir: str


class MockSimulationConfig(BaseModel):
    number_enemies: int
    field_size: int
    enemy_live_for_steps: int


class HunterEnvironmentConfig(BaseModel):
    size: int
    max_timestep: int
    udp_address: Optional[str]
    simulation_config: Optional[MockSimulationConfig]
    render_config: RendererConfig

    def to_dict(self) -> dict:
        result_dict = dict(self)
        result_dict["simulation_config"] = dict(self.simulation_config)
        result_dict["render_config"] = dict(self.render_config)
        return result_dict

    @classmethod
    def from_dict(cls, config: dict):
        input_dict = config.copy()
        input_dict["simulation_config"] = MockSimulationConfig(**input_dict["simulation_config"])
        input_dict["render_config"] = RendererConfig(**input_dict["render_config"])
        return cls(**config)
