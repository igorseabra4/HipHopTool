using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_PCNT : HipSection
    {
        public int fileADBGCount;
        public int layerLDBGCount;
        public int sizeOfLargestFile;
        public int loadedSum;
        public int sizeOfOneOfTheFiles;

        public Section_PCNT Read(BinaryReader binaryReader)
        {
            sectionName = Section.PCNT;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            fileADBGCount = Switch(binaryReader.ReadInt32());
            layerLDBGCount = Switch(binaryReader.ReadInt32());
            sizeOfLargestFile = Switch(binaryReader.ReadInt32());
            loadedSum = Switch(binaryReader.ReadInt32());
            sizeOfOneOfTheFiles = Switch(binaryReader.ReadInt32());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PCNT;

            listBytes.AddRange(BitConverter.GetBytes(fileADBGCount).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(layerLDBGCount).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(sizeOfLargestFile).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(loadedSum).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(sizeOfOneOfTheFiles).Reverse());
        }
    }
}