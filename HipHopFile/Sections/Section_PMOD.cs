using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PMOD : HipSection
    {
        public int modDate;

        public Section_PMOD(int date)
        {
            sectionName = Section.PMOD;
            modDate = date;
        }

        public Section_PMOD(BinaryReader binaryReader)
        {
            sectionName = Section.PMOD;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            modDate = Switch(binaryReader.ReadInt32());
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PMOD;

            listBytes.AddRange(BitConverter.GetBytes(modDate).Reverse());            
        }
    }
}