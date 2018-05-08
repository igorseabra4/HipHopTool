using System;
using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_DICT : HipSection
    {
        public Section_ATOC ATOC;
        public Section_LTOC LTOC;

        public Section_DICT()
        {
            sectionName = Section.DICT;
        }

        public Section_DICT(BinaryReader binaryReader)
        {
            sectionName = Section.DICT;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.ATOC.ToString()) throw new Exception();
            ATOC = new Section_ATOC(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LTOC.ToString()) throw new Exception();
            LTOC = new Section_LTOC(binaryReader);
            
            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DICT;

            ATOC.SetBytes(ref listBytes);
            LTOC.SetBytes(ref listBytes);
        }
    }
}
