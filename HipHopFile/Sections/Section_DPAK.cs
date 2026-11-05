namespace HipHopFile;

public class Section_DPAK : HipSection
{
    public Section_DPAK() : base(Section.DPAK) { }

    public Section_DPAK(BigEndianBinaryReader reader) : base(reader, Section.DPAK)
    {
        reader.BaseStream.Position += sectionSize;
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
        int firstPaddingPosition = (int)writer.BaseStream.Position;
        int alignment = platform == Platform.GameCube ? 0x20 : 0x800;

        int firstPadding = (alignment - (firstPaddingPosition + 4) % alignment) % alignment;

        writer.Write(firstPadding);

        HipFile.WritePadding(writer, firstPadding);
    }
}