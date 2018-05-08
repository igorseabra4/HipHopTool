using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_DPAK : HipSection
    {
        public int firstPadding;
        public byte[] data;
        
        public Section_DPAK(int a)
        {
            sectionName = Section.DPAK;
            firstPadding = a;
        }

        public Section_DPAK(BinaryReader binaryReader)
        {
            sectionName = Section.DPAK;
            sectionSize = Switch(binaryReader.ReadInt32());

            firstPadding = Switch(binaryReader.ReadInt32());
            globalRelativeStartOffset = (int)binaryReader.BaseStream.Position;

            data = binaryReader.ReadBytes(sectionSize - 4);
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DPAK;

            listBytes.AddRange(BitConverter.GetBytes(firstPadding).Reverse());

            globalRelativeStartOffset = listBytes.Count();

            listBytes.AddRange(data);
        }
    }
}