namespace HipHopFile
{
    public class Section_HIPA : HipSection
    {
        public Section_HIPA() : base(Section.HIPA) { }

        public Section_HIPA(BigEndianBinaryReader reader) : base(reader, Section.HIPA) { }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.HIPA;
        }
    }
}
