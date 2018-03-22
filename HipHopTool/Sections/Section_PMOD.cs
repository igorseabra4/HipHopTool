using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_PMOD : HipSection
    {
        public DateTime fileDate;

        public Section_PMOD Read(BinaryReader binaryReader)
        {
            sectionName = Section.PMOD;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            fileDate = new DateTime(Switch(binaryReader.ReadInt32()));
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PMOD;

            listBytes.AddRange(BitConverter.GetBytes((int)fileDate.ToBinary()).Reverse());            
        }
    }
}