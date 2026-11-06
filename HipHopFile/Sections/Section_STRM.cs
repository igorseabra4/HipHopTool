namespace HipHopFile;

public class Section_STRM : HipSection
{
    public Section_DHDR DHDR;
    public Section_DPAK DPAK;

    public Section_STRM(Action<BigEndianBinaryWriter> writePayload) : base(Section.STRM)
    {
        DHDR = new Section_DHDR();
        DPAK = new Section_DPAK(writePayload);
    }

    public Section_STRM(BigEndianBinaryReader reader) : base(reader, Section.STRM)
    {
        DHDR = new Section_DHDR(reader);
        DPAK = new Section_DPAK(reader);
    }

    public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        DHDR.Write(game, platform, writer);
        DPAK.Write(game, platform, writer);
    }
}
