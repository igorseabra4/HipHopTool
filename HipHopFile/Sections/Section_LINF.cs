namespace HipHopFile
{
    public class Section_LINF : HipSection
    {
        public int value;

        public Section_LINF(int value) : base(Section.LINF)
        {
            this.value = value;
        }

        public Section_LINF(BigEndianBinaryReader reader) : base(reader, Section.LINF)
        {
            value = reader.ReadInt32();
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.LINF;

            writer.Write(value);
        }
    }
}