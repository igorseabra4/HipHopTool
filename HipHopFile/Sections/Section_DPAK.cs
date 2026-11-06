namespace HipHopFile;

public class Section_DPAK : HipSection
{
    public Action<BigEndianBinaryWriter> writePayload;

    public Section_DPAK(Action<BigEndianBinaryWriter> writePayload) : base(Section.DPAK)
    {
        this.writePayload = writePayload;
    }

    public Section_DPAK(BigEndianBinaryReader reader) : base(reader, Section.DPAK)
    {
        reader.BaseStream.Position += sectionSize;
    }

    public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        int firstPaddingPosition = (int)writer.BaseStream.Position;
        int alignment = platform == Platform.GameCube ? 0x20 : 0x800;

        int firstPadding = (alignment - (firstPaddingPosition + 4) % alignment) % alignment;

        writer.Write(firstPadding);

        HipFile.WritePadding(writer, firstPadding);
        writePayload?.Invoke(writer);
    }
}