using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PMOD : HipSection
    {
        public int modDate;

        public Section_PMOD(int modDate) : base(Section.PMOD)
        {
            this.modDate = modDate;
        }

        public Section_PMOD(BinaryReader binaryReader) : base(binaryReader, Section.PMOD)
        {            
            modDate = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PMOD;

            listBytes.AddBigEndian(modDate);            
        }
    }
}