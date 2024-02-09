import enum
import json
from dataclasses import dataclass


class StopType(enum.Enum):
    STOP = "STOP"
    RESUME = "RESUME"


@dataclass
class Message:
    """Dataclass to hold information about a message."""

    ClassType: str

    def as_json(self):
        """Return the Message dataclass as a json string.

        Returns:
            _type_: _description_
        """
        return json.dumps(
            self.__dict__, default=lambda x: x.value if isinstance(x, enum.Enum) else x
        )  # Convert enum to string (json doesn't support enum

    @classmethod
    def parse(cls, data: str):
        """Generate a Message class from a json string. Returns None, if the json is corrupt.

        Args:
            data (str): json string
        """
        raise NotImplementedError()


@dataclass
class ControlMessage(Message):
    SendMessage: StopType

    def __init__(self, sendMessage: StopType):
        super().__init__(ClassType="ControlMessage")
        self.SendMessage = sendMessage

    @classmethod
    def parse(cls, data: str):
        json_data = json.loads(data)
        if "classType" not in json_data or json_data["ClassType"] != "ControlMessage":
            return None
        return cls(sendMessage=StopType(json_data["SendMessage"]))


@dataclass
class InfoMessage(Message):
    Info: str

    def __init__(self, info: str):
        super().__init__(ClassType="InfoMessage")
        self.Info = info

    @classmethod
    def parse(cls, data: str):
        json_data = json.loads(data)
        if "classType" not in json_data or json_data["ClassType"] != "InfoMessage":
            return None
        return cls(info=json_data["Info"])
