namespace HipHopFile;

public class Section_STRM : HipSection
{
    public Section_DHDR DHDR;
    public Section_DPAK DPAK;

    public Section_STRM() : base(Section.STRM)
    {
        DHDR = new Section_DHDR();
        DPAK = new Section_DPAK();
    }

    public Section_STRM(BigEndianBinaryReader reader) : base(reader, Section.STRM)
    {
        DHDR = new Section_DHDR(reader);
        DPAK = new Section_DPAK(reader);
    }

    public int SectionSize
    {
        get => sectionSize;
        set => sectionSize = value;
    }

    public override void Write(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        writer.WriteMagic(sectionType.ToString());
        writer.Write(sectionSize);
        Serialize(game, platform, writer);
    }

    public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        DHDR.Write(game, platform, writer);
        DPAK.Write(game, platform, writer);
    }
}
