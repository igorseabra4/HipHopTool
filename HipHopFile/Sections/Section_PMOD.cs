namespace HipHopFile
{
    public class Section_PMOD : HipSection
    {
        public int modDate;

        public Section_PMOD(int modDate) : base(Section.PMOD)
        {
            this.modDate = modDate;
        }

        public Section_PMOD(BigEndianBinaryReader reader) : base(reader, Section.PMOD)
        {
            modDate = reader.ReadInt32();
        }

        public override void Setwriter(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.PMOD;

            writer.Write(modDate);
        }
    }
}