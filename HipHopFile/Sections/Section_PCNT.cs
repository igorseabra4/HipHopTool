namespace HipHopFile
{
    public class Section_PCNT : HipSection
    {
        public int AHDRCount;
        public int LHDRCount;
        public int sizeOfLargestSourceFileAsset;
        public int sizeOfLargestLayer;
        public int sizeOfLargestSourceVirtualAsset;

        public Section_PCNT() : base(Section.PCNT)
        {
            AHDRCount = 0;
            LHDRCount = 0;
            sizeOfLargestSourceFileAsset = 0;
            sizeOfLargestLayer = 0;
            sizeOfLargestSourceVirtualAsset = 0;
        }

        public Section_PCNT(int AHDRCount, int LHDRCount) : this(AHDRCount, LHDRCount, 0, 0, 0)
        {
        }

        public Section_PCNT(int AHDRCount, int LHDRCount, int sizeOfLargestSourceFileAsset, int sizeOfLargestLayer, int sizeOfLargestSourceVirtualAsset) : base(Section.PCNT)
        {
            this.AHDRCount = AHDRCount;
            this.LHDRCount = LHDRCount;
            this.sizeOfLargestSourceFileAsset = sizeOfLargestSourceFileAsset;
            this.sizeOfLargestLayer = sizeOfLargestLayer;
            this.sizeOfLargestSourceVirtualAsset = sizeOfLargestSourceVirtualAsset;
        }

        public Section_PCNT(BigEndianBinaryReader reader) : base(reader, Section.PCNT)
        {
            AHDRCount = reader.ReadInt32();
            LHDRCount = reader.ReadInt32();
            sizeOfLargestSourceFileAsset = reader.ReadInt32();
            sizeOfLargestLayer = reader.ReadInt32();
            sizeOfLargestSourceVirtualAsset = reader.ReadInt32();
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.PCNT;

            writer.Write(AHDRCount);
            writer.Write(LHDRCount);
            writer.Write(sizeOfLargestSourceFileAsset);
            writer.Write(sizeOfLargestLayer);
            writer.Write(sizeOfLargestSourceVirtualAsset);
        }
    }
}