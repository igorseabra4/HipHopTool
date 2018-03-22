using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_DHDR : HipSection
    {
        public int unknown;

        public Section_DHDR Read(BinaryReader binaryReader)
        {
            sectionName = Section.DHDR;
            sectionSize = Switch(binaryReader.ReadInt32());

            unknown = Switch(binaryReader.ReadInt32());

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DHDR;

            listBytes.AddRange(BitConverter.GetBytes(unknown).Reverse());
        }
    }
}