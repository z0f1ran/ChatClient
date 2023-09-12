﻿using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        PacketReader packetReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = packetReader.ReadByte();

            Username = packetReader.ReadMessage();
            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received! {msg}");
                            Program.BroadcastMessage(new Model.MessageModel
                            {
                                Username = this.Username,
                                Message = msg, 
                                Time= DateTime.Now,
                            }) ;
                            break;
                        default: break;
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine($"[{UID}]: Disconnected!");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
