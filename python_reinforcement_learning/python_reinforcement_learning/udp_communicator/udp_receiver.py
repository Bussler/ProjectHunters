import asyncio
import logging

from .messages import InfoMessage

HOST = "0.0.0.0"
PORT = 1337

loop = asyncio.get_event_loop()

# Set up logging
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)


class InfoLogRecvProtocol(asyncio.DatagramProtocol):
    def __init__(self):
        super().__init__()

    # used by asyncio
    def connection_made(self, transport) -> None:
        self.transport = transport

    # Main entrypoint for processing message
    def datagram_received(self, data, addr) -> None:
        message = InfoMessage.parse(data)
        if message is not None:
            logger.debug(f"[x] Received InfoMessage message: {message.info}")
        else:
            logger.debug(f"[x] Received corrupt message, expected InfoMessage: {data}")


def main():
    logger.info(f"[x] Listening on {HOST}:{PORT}")
    t = loop.create_datagram_endpoint(InfoLogRecvProtocol, local_addr=(HOST, PORT))
    loop.run_until_complete(t)  # Server starts listening
    loop.run_forever()


if __name__ == "__main__":
    main()
