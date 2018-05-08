using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_DHDR : HipSection
    {
        public int value;

        public Section_DHDR(int a)
        {
            sectionName = Section.DHDR;
            value = a;
        }

        public Section_DHDR(BinaryReader binaryReader)
        {
            sectionName = Section.DHDR;
            sectionSize = Switch(binaryReader.ReadInt32());

            value = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DHDR;

            listBytes.AddRange(BitConverter.GetBytes(value).Reverse());
        }
    }
}