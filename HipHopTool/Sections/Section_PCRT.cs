using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_PCRT : HipSection
    {
        public DateTime fileDate;
        public string dateString;

        public Section_PCRT Read(BinaryReader binaryReader)
        {
            sectionName = Section.PCRT;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            fileDate = new DateTime (Switch(binaryReader.ReadInt32()));
            
            List<char> charList = new List<char>(sectionSize);
            char c = binaryReader.ReadChar();
            while (c != 0)
            {
                charList.Add(c);
                c = binaryReader.ReadChar();
            }

            dateString = new string(charList.ToArray());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PCRT;

            listBytes.AddRange(BitConverter.GetBytes((int)fileDate.ToBinary()).Reverse());

            foreach (char i in dateString)
                listBytes.Add((byte)i);

            if (dateString.Length % 2 == 0) listBytes.AddRange(new byte[] { 0, 0 });
            if (dateString.Length % 2 == 1) listBytes.Add(0);
        }
    }
}