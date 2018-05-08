using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PCNT : HipSection
    {
        public int fileAHDRCount;
        public int layerLHDRCount;
        public int sizeOfLargestSourceFileAsset;
        public int sizeOfLargestLayer;
        public int sizeOfLargestSourceVirtualAsset;

        public Section_PCNT(int AHDRCount, int LHDRCount, int LargestSourceFileAsset, int LargestLayer, int LargestSourceVirtualAsset)
        {
            sectionName = Section.PCNT;
            fileAHDRCount = AHDRCount;
            layerLHDRCount = LHDRCount;
            sizeOfLargestSourceFileAsset = LargestSourceFileAsset;
            sizeOfLargestLayer = LargestLayer;
            sizeOfLargestSourceVirtualAsset = LargestSourceVirtualAsset;
        }

        public Section_PCNT(BinaryReader binaryReader)
        {
            sectionName = Section.PCNT;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            fileAHDRCount = Switch(binaryReader.ReadInt32());
            layerLHDRCount = Switch(binaryReader.ReadInt32());
            sizeOfLargestSourceFileAsset = Switch(binaryReader.ReadInt32());
            sizeOfLargestLayer = Switch(binaryReader.ReadInt32());
            sizeOfLargestSourceVirtualAsset = Switch(binaryReader.ReadInt32());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PCNT;

            listBytes.AddRange(BitConverter.GetBytes(fileAHDRCount).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(layerLHDRCount).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(sizeOfLargestSourceFileAsset).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(sizeOfLargestLayer).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(sizeOfLargestSourceVirtualAsset).Reverse());
        }
    }
}