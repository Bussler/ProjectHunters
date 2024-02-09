import asyncio

HOST = '0.0.0.0'
PORT = 1337

loop = asyncio.get_event_loop()

class LogRecvProtocol(asyncio.DatagramProtocol):
    def __init__(self):
        super().__init__()

    # used by asyncio
    def connection_made(self, transport) -> None:
        self.transport = transport

    # Main entrypoint for processing message
    def datagram_received(self, data, addr) -> None:
        print(f"[x] Received Syslog message: {data}")


def main():
    print(f"[x] Listening on {HOST}:{PORT}")
    t = loop.create_datagram_endpoint(LogRecvProtocol, local_addr=(HOST, PORT))
    loop.run_until_complete(t) # Server starts listening
    loop.run_forever()


if __name__ == '__main__':
    main()