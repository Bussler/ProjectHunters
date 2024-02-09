import enum
import json
from dataclasses import dataclass


class StopType(enum.Enum):
    STOP = "STOP"
    RESUME = "RESUME"


@dataclass
class Message:
    def as_json(self):
        return json.dumps(
            self.__dict__, default=lambda x: x.value if isinstance(x, enum.Enum) else x
        )  # Convert enum to string (json doesn't support enum


@dataclass
class ControlMessage(Message):
    sendMessage: StopType
    classType: str = "ControlMessage"


@dataclass
class InfoMessage(Message):
    content: str
    classType: str = "InfoMessage"


if __name__ == "__main__":
    message = ControlMessage(StopType.STOP)
    print(message.as_json())

    message = InfoMessage("This is a test message.")
    print(message.as_json())
