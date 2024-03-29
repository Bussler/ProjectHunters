import asyncio
import logging
import random
import socket

from .messages import ControlMessage, InfoMessage, ControlType

HOST = "localhost"
PORT = 1337

# asyncio event loop
loop = asyncio.get_event_loop()

# Set up logging
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)


def send_message(message: str) -> None:
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # Internet  # UDP
    sock.sendto(message.encode(), (HOST, PORT))
    logger.debug(f"[x] Sent message: {message}")


async def write_test_messages(
    log_text: str = "C://unityProjects//ProjectHunters//python_reinforcement_learning//tests//testlogs.txt",
) -> None:
    fp = open(log_text)
    logger.info("[x] Writing messages...")
    for line in fp.readlines():
        await asyncio.sleep(random.uniform(0.1, 1.0))
        message = InfoMessage(line)
        send_message(message.as_json())


async def write_stop_message() -> None:
    message = ControlMessage(ControlType.STOP)
    send_message(message.as_json())


async def write_resume_message() -> None:
    message = ControlMessage(ControlType.RESUME)
    send_message(message.as_json())
    

async def write_restart_message() -> None:
    message = ControlMessage(ControlType.RESTART)
    send_message(message.as_json())


def stop() -> None:
    loop.run_until_complete(write_stop_message())


def resume() -> None:
    loop.run_until_complete(write_resume_message())
    

def restart() -> None:
    loop.run_until_complete(write_restart_message())


def main():
    loop.run_until_complete(write_test_messages())  # Start writing messages (or running tests)


if __name__ == "__main__":
    main()
