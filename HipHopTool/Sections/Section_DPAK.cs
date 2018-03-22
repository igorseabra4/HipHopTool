using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_DPAK : HipSection
    {
        public int firstPadding;
        public byte[] data;

        public int relativeStartOffset;

        public Section_DPAK Read(BinaryReader binaryReader)
        {
            sectionName = Section.DPAK;
            sectionSize = Switch(binaryReader.ReadInt32());

            firstPadding = Switch(binaryReader.ReadInt32());

            relativeStartOffset = (int)binaryReader.BaseStream.Position;

            data = binaryReader.ReadBytes(sectionSize - 4);

            binaryReader.BaseStream.Position = relativeStartOffset + sectionSize - 4;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DPAK;

            listBytes.AddRange(BitConverter.GetBytes(firstPadding).Reverse());
            listBytes.AddRange(data);
        }
    }
}