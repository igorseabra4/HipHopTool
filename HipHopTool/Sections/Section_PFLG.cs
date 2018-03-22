using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_PFLG : HipSection
    {
        public int flags;

        public Section_PFLG Read(BinaryReader binaryReader)
        {
            sectionName = Section.PFLG;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            flags = Switch(binaryReader.ReadInt32());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PFLG;
            listBytes.AddRange(BitConverter.GetBytes(flags).Reverse());
        }
    }
}
