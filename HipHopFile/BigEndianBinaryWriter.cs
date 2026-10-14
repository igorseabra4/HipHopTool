using System;
using System.IO;

namespace HipHopFile
{
    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter() : base(new MemoryStream()) { }
        public BigEndianBinaryWriter(Stream stream) : base(stream) { }

        public byte[] ToArray() => ((MemoryStream)BaseStream).ToArray();

        public override void Write(int f)
        {
            WriteReverse4(BitConverter.GetBytes(f));
        }

        public override void Write(uint f)
        {
            WriteReverse4(BitConverter.GetBytes(f));
        }

        private void WriteReverse4(byte[] bytes)
        {
            base.Write(bytes[3]);
            base.Write(bytes[2]);
            base.Write(bytes[1]);
            base.Write(bytes[0]);
        }

        public override void Write(string text)
        {
            foreach (char i in text)
                base.Write((byte)i);

            if (text.Length % 2 == 0)
                base.Write(new byte[] { 0, 0 });
            if (text.Length % 2 == 1)
                base.Write(new byte[] { 0 });
        }

        public void WriteMagic(string magic)
        {
            if (magic.Length != 4)
                throw new ArgumentException("Magic word must have 4 characters");
            Write(magic.ToCharArray());
        }
    }
}
