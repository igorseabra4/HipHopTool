using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_LINF : HipSection
    {
        public int unknown;

        public Section_LINF Read(BinaryReader binaryReader)
        {
            sectionName = Section.LINF;
            sectionSize = Switch(binaryReader.ReadInt32());

            unknown = Switch(binaryReader.ReadInt32());

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.LINF;

            listBytes.AddRange(BitConverter.GetBytes(unknown).Reverse());
        }
    }
}