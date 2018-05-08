using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_LINF : HipSection
    {
        public int value;

        public Section_LINF(int a)
        {
            sectionName = Section.LINF;
            value = a;
        }

        public Section_LINF(BinaryReader binaryReader)
        {
            sectionName = Section.LINF;
            sectionSize = Switch(binaryReader.ReadInt32());

            value = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.LINF;

            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }
    }
}