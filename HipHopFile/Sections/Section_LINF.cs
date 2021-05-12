using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_LINF : HipSection
    {
        public int value;

        public Section_LINF(int value) : base(Section.LINF)
        {
            this.value = value;
        }

        public Section_LINF(BinaryReader binaryReader) : base(binaryReader, Section.LINF)
        {
            value = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.LINF;

            listBytes.AddBigEndian(value);
        }
    }
}