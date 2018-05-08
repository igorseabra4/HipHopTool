using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_LDBG : HipSection
    {
        public int value;

        public Section_LDBG(int a)
        {
            sectionName = Section.LDBG;
            value = a;
        }

        public Section_LDBG(BinaryReader binaryReader)
        {
            sectionName = Section.LDBG;
            sectionSize = Switch(binaryReader.ReadInt32());
            
            value = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.LDBG;

            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }
    }
}