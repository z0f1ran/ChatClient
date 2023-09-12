using ChatServer.Model;
using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;

namespace ChatServer
{
    internal class Program
    {
        static List<Client> users;
        static TcpListener listener;
        static void Main(string[] args)
        {
            users= new List<Client>();
            listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 8000);
            listener.Start();

            while (true)
            {
                var client = new Client(listener.AcceptTcpClient());
                users.Add(client);

                BroadcastConnection();
            }
        }

        static void BroadcastConnection()
        {
            foreach (var user in users)
            {
                foreach (var usr in users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(MessageModel message)
        {
            foreach(var user in users)
            {   
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                var jsonMessage = JsonSerializer.Serialize(message);
                msgPacket.WriteMessage(jsonMessage);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            users.Remove(disconnectedUser);
            foreach (var user in users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
            var jsonMessage = new MessageModel
            {
                Username = disconnectedUser.Username,
                Message = "is disconnected!",
                Time= DateTime.Now,
            };
            BroadcastMessage(jsonMessage);
        }
    }
}
