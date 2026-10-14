using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HipHopFile
{
    public class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(byte[] data) : base(new MemoryStream(data)) { }
        public BigEndianBinaryReader(Stream stream) : base(stream) { }

        public override int ReadInt32()
        {
            return BitConverter.ToInt32(ReadReverse4(), 0);
        }

        public override uint ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadReverse4(), 0);
        }

        private byte[] ReadReverse4()
        {
            var b0 = base.ReadByte();
            var b1 = base.ReadByte();
            var b2 = base.ReadByte();
            var b3 = base.ReadByte();
            return new byte[4] { b3, b2, b1, b0 };
        }

        public override string ReadString()
        {
            List<char> charList = new List<char>();
            do
                charList.Add((char)ReadByte());
            while (charList.Last() != '\0');
            charList.Remove('\0');

            if (charList.Count % 2 == 0)
                BaseStream.Position += 1;

            return new string(charList.ToArray());
        }
    }
}
