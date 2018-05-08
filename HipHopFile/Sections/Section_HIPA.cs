using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_HIPA : HipSection
    {
        public Section_HIPA()
        {
            sectionName = Section.HIPA;
        }

        public Section_HIPA(BinaryReader binaryReader)
        {
            sectionName = Section.HIPA;
            sectionSize = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.HIPA;
        }
    }
}
