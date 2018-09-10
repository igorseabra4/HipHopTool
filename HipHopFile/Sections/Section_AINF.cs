using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_AINF : HipSection
    {
        public int value;

        public Section_AINF(int value) : base(Section.AINF)
        {
            this.value = value;
        }

        public Section_AINF(BinaryReader binaryReader) : base(binaryReader, Section.AINF)
        {
            value = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.AINF;

            listBytes.AddBigEndian(value);
        }
    }
}