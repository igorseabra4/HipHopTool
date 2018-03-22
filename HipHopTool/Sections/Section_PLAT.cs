using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_PLAT : HipSection
    {
        public string platformString;

        public Section_PLAT Read(BinaryReader binaryReader)
        {
            sectionName = Section.PLAT;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;
            
            List<char> charList = new List<char>(sectionSize);
            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                char c = binaryReader.ReadChar();
                charList.Add(c);
            }

            platformString = new string(charList.ToArray());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PLAT;

            foreach (char i in platformString)
                listBytes.Add((byte)i);
        }
    }
}
