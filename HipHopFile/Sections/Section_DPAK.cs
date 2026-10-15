namespace HipHopFile
{
    public class Section_DPAK : HipSection
    {
        public int globalRelativeStartOffset;
        public byte[] data;

        public Section_DPAK() : base(Section.DPAK) { }

        public Section_DPAK(BigEndianBinaryReader reader) : base(reader, Section.DPAK)
        {
            if (Section_ATOC.noAHDR)
            {
                data = reader.ReadBytes(sectionSize);
            }
            else
            {
                int firstPadding = reader.ReadInt32();

                reader.BaseStream.Position += firstPadding;

                globalRelativeStartOffset = (int)reader.BaseStream.Position;

                int sizeOfData = sectionSize - 4 - firstPadding;
                data = reader.ReadBytes(sizeOfData);
            }
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.DPAK;

            int firstPaddingPosition = (int)writer.BaseStream.Position;

            writer.Write((int)0);

            int alignment = platform == Platform.GameCube ? 0x20 : 0x800;

            while (writer.BaseStream.Position % alignment != 0)
                writer.Write((byte)0x33);

            globalRelativeStartOffset = (int)writer.BaseStream.Position;

            int firstPadding = globalRelativeStartOffset - (firstPaddingPosition + 4);

            writer.BaseStream.Position = firstPaddingPosition;
            writer.Write(firstPadding);

            writer.BaseStream.Position = globalRelativeStartOffset;
            writer.Write(data);
        }
    }
}