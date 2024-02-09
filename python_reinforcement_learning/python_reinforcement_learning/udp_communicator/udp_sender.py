import socket
import asyncio
import random

HOST = 'localhost'
PORT = 1337

loop = asyncio.get_event_loop()

def send_message(message: str) -> None:
    sock = socket.socket(socket.AF_INET,  # Internet
                         socket.SOCK_DGRAM)  # UDP
    sock.sendto(message.encode(), (HOST, PORT))
    print(f"[x] Sent message: {message}")


async def write_test_messages(log_text: str = "C://unityProjects//ProjectHunters//python_reinforcement_learning//tests//testlogs.txt") -> None:
    fp = open(log_text)
    print("[x] Writing messages...")
    for line in fp.readlines():
        await asyncio.sleep(random.uniform(0.1, 1.0))
        send_message(line)

def main():
    loop.run_until_complete(write_test_messages()) # Start writing messages (or running tests)

if __name__ == '__main__':
    main()