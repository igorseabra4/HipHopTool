using System.Buffers.Binary;

namespace HipHopFile;

public class BigEndianBinaryWriter : BinaryWriter
{
    public BigEndianBinaryWriter() : base(new MemoryStream()) { }
    public BigEndianBinaryWriter(Stream stream) : base(stream) { }

    public byte[] ToArray() => ((MemoryStream)BaseStream).ToArray();

    public override void Write(int f)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(buffer, f);
        BaseStream.Write(buffer);
    }

    public override void Write(uint f)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(buffer, f);
        BaseStream.Write(buffer);
    }

    public override void Write(string text)
    {
        foreach (char i in text)
            base.Write((byte)i);

        base.Write((byte)0);
        if (text.Length % 2 == 0)
            base.Write((byte)0);
    }

    public void WriteMagic(string magic)
    {
        if (magic.Length != 4)
            throw new ArgumentException("Magic word must have 4 characters");
        Write(magic.ToCharArray());
    }
}
