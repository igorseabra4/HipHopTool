using System;

namespace HipHopFile
{
    public abstract class HipSection
    {
        public Section sectionType;
        public int sectionSize;

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
            writer.Write((int)0);
            writer.Write((int)0);

            Serialize(game, platform, writer);

            sectionSize = (int)writer.BaseStream.Position - position - 0x8;

            var sectionNameString = sectionType.ToString();
            var sectionSizeBytes = BitConverter.GetBytes(sectionSize);

            var currentPosition = writer.BaseStream.Position;
            writer.BaseStream.Position = position;
            writer.WriteMagic(sectionNameString);
            writer.Write(sectionSizeBytes);
            writer.BaseStream.Position = currentPosition;
        }

        public abstract void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer);
    }
}
