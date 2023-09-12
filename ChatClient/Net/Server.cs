using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    internal class Server
    {
        TcpClient client;
        public PacketReader packetReader;

        public event Action connectedEvent;
        public event Action messageReceiveEvent;
        public event Action userDisconnectEvent;

        public Server()
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!client.Connected)
            {
                client.Connect("127.0.0.1", 8000);
                packetReader = new PacketReader(client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(1);
                    connectPacket.WriteMessage(username);
                    client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            messageReceiveEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("Mmm?");
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
