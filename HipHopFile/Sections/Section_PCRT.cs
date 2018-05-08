using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PCRT : HipSection
    {
        public int fileDate;
        public string dateString;

        public Section_PCRT(int date1, string date2)
        {
            sectionName = Section.PCRT;
            fileDate = date1;
            dateString = date2;
        }
        
        public Section_PCRT(BinaryReader binaryReader)
        {
            sectionName = Section.PCRT;
            sectionSize = Switch(binaryReader.ReadInt32());

            fileDate = Switch(binaryReader.ReadInt32());            
            dateString = ReadString(binaryReader);
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PCRT;

            listBytes.AddRange(BitConverter.GetBytes(fileDate).Reverse());
            WriteString(ref listBytes, dateString);
        }
    }
}