using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PCRT : HipSection
    {
        public int fileDate;
        public string dateString;

        public Section_PCRT(int fileDate, string dateString) : base(Section.PCRT)
        {
            this.fileDate = fileDate;
            this.dateString = dateString;
        }
        
        public Section_PCRT(BinaryReader binaryReader) : base(binaryReader, Section.PCRT)
        {
            fileDate = Switch(binaryReader.ReadInt32());            
            dateString = ReadString(binaryReader);
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PCRT;

            listBytes.AddBigEndian(fileDate);
            listBytes.AddString(dateString);
        }
    }
}