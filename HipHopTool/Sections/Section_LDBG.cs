using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_LDBG : HipSection
    {
        public int unknown1;
        
        public Section_LDBG Read(BinaryReader binaryReader)
        {
            sectionName = Section.LDBG;
            sectionSize = Switch(binaryReader.ReadInt32());
            
            unknown1 = Switch(binaryReader.ReadInt32());

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.LDBG;

            listBytes.AddRange(BitConverter.GetBytes(unknown1).Reverse());
        }
    }
}