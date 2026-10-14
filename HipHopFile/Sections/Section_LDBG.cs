namespace HipHopFile
{
    public class Section_LDBG : HipSection
    {
        public int value;

        public Section_LDBG(int value) : base(Section.LDBG)
        {
            this.value = value;
        }

        public Section_LDBG(BigEndianBinaryReader reader) : base(reader, Section.LDBG)
        {
            value = reader.ReadInt32();
        }

        public override void Setwriter(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.LDBG;

            writer.Write(value);
        }
    }
}