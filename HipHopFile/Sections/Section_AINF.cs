using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_AINF : HipSection
    {
        public int value;

        public Section_AINF(int a)
        {
            sectionName = Section.AINF;
            value = a;
        }

        public Section_AINF(BinaryReader binaryReader)
        {
            sectionName = Section.AINF;
            sectionSize = Switch(binaryReader.ReadInt32());

            value = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.AINF;

            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }
    }
}