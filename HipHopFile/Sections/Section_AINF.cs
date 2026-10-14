namespace HipHopFile
{
    public class Section_AINF : HipSection
    {
        public int value;

        public Section_AINF(int value) : base(Section.AINF)
        {
            this.value = value;
        }

        public Section_AINF(BigEndianBinaryReader reader) : base(reader, Section.AINF)
        {
            value = reader.ReadInt32();
        }

        public override void Setwriter(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.AINF;

            writer.Write(value);
        }
    }
}