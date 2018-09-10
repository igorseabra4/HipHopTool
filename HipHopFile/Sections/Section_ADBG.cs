using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_ADBG : HipSection
    {
        public int alignment;
        public string assetName;
        public string assetFileName;
        public int checksum;

        public Section_ADBG(int alignment, string assetName, string assetFileName, int checksum) : base(Section.ADBG)
        {
            this.alignment = alignment;
            this.assetName = assetName;
            this.assetFileName = assetFileName;
            this.checksum = checksum;
        }

        public Section_ADBG(BinaryReader binaryReader) : base(binaryReader, Section.ADBG)
        {
            alignment = Switch(binaryReader.ReadInt32());
            assetName = ReadString(binaryReader);
            assetFileName = ReadString(binaryReader);
            checksum = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.ADBG;

            listBytes.AddBigEndian(alignment);
            listBytes.AddString(assetName);
            listBytes.AddString(assetFileName);       
            listBytes.AddBigEndian(checksum);
        }
    }
}
