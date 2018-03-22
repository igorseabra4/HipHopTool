using System.Collections.Generic;
using System.IO;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_HIPA : HipSection
    {
        public Section_HIPA Read(BinaryReader binaryReader)
        {
            sectionName = Section.HIPA;
            sectionSize = Switch(binaryReader.ReadInt32());

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.HIPA;
        }
    }
}
