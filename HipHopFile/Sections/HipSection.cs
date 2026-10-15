namespace HipHopFile
{
    public abstract class HipSection
    {
        protected Section sectionType;
        protected int sectionSize;

        public HipSection() { }

        public HipSection(Section sectionName)
        {
            this.sectionType = sectionName;
        }

        public HipSection(BigEndianBinaryReader reader, Section sectionType)
        {
            this.sectionType = sectionType;
            sectionSize = reader.ReadInt32();
        }

        public void SetBytes(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            int position = (int)writer.BaseStream.Position;
            writer.Write(new byte[8]);

            Serialize(game, platform, writer);

            sectionSize = (int)writer.BaseStream.Position - position - 0x8;

            var sectionNameString = sectionType.ToString();

            var currentPosition = writer.BaseStream.Position;
            writer.BaseStream.Position = position;
            writer.WriteMagic(sectionNameString);
            writer.Write(sectionSize);
            writer.BaseStream.Position = currentPosition;
        }

        public abstract void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer);
    }
}
