namespace HipHopFile;

public class Section_DHDR : HipSection
{
    public int value;

    public Section_DHDR(int value = -1) : base(Section.DHDR)
    {
        this.value = value;
    }

    public Section_DHDR(BigEndianBinaryReader reader) : base(reader, Section.DHDR)
    {
        value = reader.ReadInt32();
    }

    public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        writer.Write(value);
    }
}