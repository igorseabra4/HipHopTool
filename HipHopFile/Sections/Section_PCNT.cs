using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

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

        public Section_PCNT(int AHDRCount, int LHDRCount, int sizeOfLargestSourceFileAsset, int sizeOfLargestLayer, int sizeOfLargestSourceVirtualAsset) : base(Section.PCNT)
        {
            this.AHDRCount = AHDRCount;
            this.LHDRCount = LHDRCount;
            this.sizeOfLargestSourceFileAsset = sizeOfLargestSourceFileAsset;
            this.sizeOfLargestLayer = sizeOfLargestLayer;
            this.sizeOfLargestSourceVirtualAsset = sizeOfLargestSourceVirtualAsset;
        }

        public Section_PCNT(BinaryReader binaryReader) : base(binaryReader, Section.PCNT)
        {
            AHDRCount = Switch(binaryReader.ReadInt32());
            LHDRCount = Switch(binaryReader.ReadInt32());
            sizeOfLargestSourceFileAsset = Switch(binaryReader.ReadInt32());
            sizeOfLargestLayer = Switch(binaryReader.ReadInt32());
            sizeOfLargestSourceVirtualAsset = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PCNT;

            listBytes.AddBigEndian(AHDRCount);
            listBytes.AddBigEndian(LHDRCount);
            listBytes.AddBigEndian(sizeOfLargestSourceFileAsset);
            listBytes.AddBigEndian(sizeOfLargestLayer);
            listBytes.AddBigEndian(sizeOfLargestSourceVirtualAsset);
        }
    }
}