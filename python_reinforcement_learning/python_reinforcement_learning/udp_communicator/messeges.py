import enum
import json
from dataclasses import dataclass


class StopType(enum.Enum):
    STOP = "STOP"
    RESUME = "RESUME"


@dataclass
class Message:
    classType: str
    
    def as_json(self):
        return json.dumps(
            self.__dict__, default=lambda x: x.value if isinstance(x, enum.Enum) else x
        )  # Convert enum to string (json doesn't support enum


@dataclass
class ControlMessage(Message):
    sendMessage: StopType
    
    def __init__(self, sendMessage: StopType):
        super().__init__(classType="ControlMessage")
        self.sendMessage = sendMessage


@dataclass
class InfoMessage(Message):
    content: str
    
    def __init__(self, info: str):
        super().__init__(classType="InfoMessage")
        self.info = info


if __name__ == "__main__":
    message = ControlMessage(StopType.STOP)
    print(message.as_json())

    message = InfoMessage("This is a test message.")
    print(message.as_json())
