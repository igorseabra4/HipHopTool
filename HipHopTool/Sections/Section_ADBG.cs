using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_ADBG : HipSection
    {
        public int offsetToSecondString;
        public string firstString;
        public string secondString;
        public int check;

        public Section_ADBG Read(BinaryReader binaryReader)
        {
            sectionName = Section.ADBG;
            sectionSize = Switch(binaryReader.ReadInt32());
            
            long startSectionPosition = binaryReader.BaseStream.Position;

            offsetToSecondString = Switch(binaryReader.ReadInt32());

            List<char> charList;

            if (offsetToSecondString > 0)
            {
                charList = new List<char>(sectionSize);
                while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
                {
                    charList.Add((char)binaryReader.ReadByte());
                    if (charList.Last() == '\0')
                    {
                        charList.Remove('\0');
                        break;
                    }
                }

                firstString = new string(charList.ToArray());
            }
            else firstString = null;

            if (offsetToSecondString > 0)
                binaryReader.BaseStream.Position = startSectionPosition + offsetToSecondString + 4;

            charList = new List<char>(sectionSize);
            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                charList.Add((char)binaryReader.ReadByte());
                if (charList.Last() == '\0')
                {
                    charList.Remove('\0');
                    break;
                }
            }

            secondString = new string(charList.ToArray());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize - 4;
            check = Switch(binaryReader.ReadInt32());

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.ADBG;

            listBytes.AddRange(BitConverter.GetBytes(offsetToSecondString).Reverse());

            if (firstString != null)
            {
                foreach (char i in firstString)
                    listBytes.Add((byte)i);

                if (firstString.Length % 4 == 0) listBytes.AddRange(new byte[] { 0, 0 });
                if (firstString.Length % 4 == 1) listBytes.AddRange(new byte[] { 0 });
                if (firstString.Length % 4 == 2) listBytes.AddRange(new byte[] { 0, 0 });
                if (firstString.Length % 4 == 3) listBytes.AddRange(new byte[] { 0 });
            }
            
            foreach (char i in secondString)
                listBytes.Add((byte)i);

            if (firstString != null)
            {
                if (listBytes.Count % 4 == 0) listBytes.AddRange(new byte[] { 0, 0 });
                if (listBytes.Count % 4 == 1) listBytes.AddRange(new byte[] { 0 });
                if (listBytes.Count % 4 == 2) listBytes.AddRange(new byte[] { 0, 0 });
                if (listBytes.Count % 4 == 3) listBytes.AddRange(new byte[] { 0, });
            }
            else
            {
                if (listBytes.Count % 4 == 0) listBytes.AddRange(new byte[] { 0, 0, 0, 0 });
                if (listBytes.Count % 4 == 1) listBytes.AddRange(new byte[] { 0, 0, 0 });
                if (listBytes.Count % 4 == 2) listBytes.AddRange(new byte[] { 0, 0, 0, 0 });
                if (listBytes.Count % 4 == 3) listBytes.AddRange(new byte[] { 0, 0, 0 });
            }

            listBytes.AddRange(BitConverter.GetBytes(check).Reverse());
        }
    }
}
