namespace HipHopFile
{
    public class Section_ADBG : HipSection
    {
        public int alignment;
        public string assetName;
        public string assetFileName;
        public int checksum;

        public Section_ADBG() : base(Section.ADBG)
        {
        }

        public Section_ADBG(int alignment, string assetName, string assetFileName, int checksum) : base(Section.ADBG)
        {
            this.alignment = alignment;
            this.assetName = assetName;
            this.assetFileName = assetFileName;
            this.checksum = checksum;
        }

        public Section_ADBG(BigEndianBinaryReader reader) : base(reader, Section.ADBG)
        {
            alignment = reader.ReadInt32();
            assetName = reader.ReadString();
            assetFileName = reader.ReadString();
            checksum = reader.ReadInt32();
        }

        public override void Setwriter(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.ADBG;

            writer.Write(alignment);
            writer.Write(assetName);
            writer.Write(assetFileName);
            writer.Write(checksum);
        }
    }
}
