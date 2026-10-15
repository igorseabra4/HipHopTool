namespace HipHopFile
{
    public class Section_PCRT : HipSection
    {
        public int fileDate;
        public string dateString;

        public Section_PCRT(int fileDate, string dateString) : base(Section.PCRT)
        {
            this.fileDate = fileDate;
            this.dateString = dateString;
        }

        public Section_PCRT(BigEndianBinaryReader reader) : base(reader, Section.PCRT)
        {
            fileDate = reader.ReadInt32();
            dateString = reader.ReadString();
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.PCRT;

            writer.Write(fileDate);
            writer.Write(dateString);
        }
    }
}