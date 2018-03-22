using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_AINF : HipSection
    {
        public int unknown;

        public Section_AINF Read(BinaryReader binaryReader)
        {
            sectionName = Section.AINF;
            sectionSize = Switch(binaryReader.ReadInt32());

            unknown = Switch(binaryReader.ReadInt32());

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.AINF;

            listBytes.AddRange(BitConverter.GetBytes(unknown).Reverse());
        }
    }
}