using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public class Section_DICT : HipSection
    {
        public Section_ATOC ATOC;
        public Section_LTOC LTOC;

        public Section_DICT Read(BinaryReader binaryReader)
        {
            sectionName = Section.DICT;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.ATOC.ToString()) throw new Exception();
            ATOC = new Section_ATOC().Read(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LTOC.ToString()) throw new Exception();
            LTOC = new Section_LTOC().Read(binaryReader);
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            return this;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DICT;

            listBytes.AddRange(ATOC.GetBytes());
            listBytes.AddRange(LTOC.GetBytes());
        }
    }
}
