using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net.IO
{
    internal class PacketBuilder
    {
        MemoryStream ms;
        BinaryWriter bw;
        public PacketBuilder()
        {
            ms = new MemoryStream();
            bw = new BinaryWriter(ms);
        }

        public void WriteOpCode(byte opcode)
        {
            bw.Write(opcode);
        }

        public void WriteMessage(string msg)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
            int messageLength = messageBytes.Length;

            bw.Write(messageLength);
            bw.Write(messageBytes);
        } 

        public byte[] GetPacketBytes()
        {
            return ms.ToArray();
        }
    }
}
