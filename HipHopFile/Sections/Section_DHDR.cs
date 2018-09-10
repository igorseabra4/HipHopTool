using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_DHDR : HipSection
    {
        public int value;

        public Section_DHDR(int value) : base(Section.DHDR)
        {
            this.value = value;
        }

        public Section_DHDR(BinaryReader binaryReader) : base(binaryReader, Section.DHDR)
        {
            value = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DHDR;

            listBytes.AddBigEndian(value);
        }
    }
}