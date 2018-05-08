using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_ADBG : HipSection
    {
        public int alignment;
        public string assetName;
        public string assetFileName;
        public int checksum;

        public Section_ADBG(int align, string name, string fileName, int check)
        {
            alignment = align;
            assetName = name;
            assetFileName = fileName;
            checksum = check;
        }

        public Section_ADBG(BinaryReader binaryReader)
        {
            sectionName = Section.ADBG;
            sectionSize = Switch(binaryReader.ReadInt32());
            
            long startSectionPosition = binaryReader.BaseStream.Position;

            alignment = Switch(binaryReader.ReadInt32());
            assetName = ReadString(binaryReader);

            assetFileName = ReadString(binaryReader);
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize - 4;
            checksum = Switch(binaryReader.ReadInt32());
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.ADBG;

            listBytes.AddRange(BitConverter.GetBytes(alignment).Reverse());			
            WriteString(ref listBytes, assetName);            
            WriteString(ref listBytes, assetFileName);            
            listBytes.AddRange(BitConverter.GetBytes(checksum).Reverse());
        }
    }
}
