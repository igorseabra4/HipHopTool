using System;
using System.Collections.Generic;
using System.IO;

namespace HipHopFile
{
    public class Section_DICT : HipSection
    {
        public Section_ATOC ATOC;
        public Section_LTOC LTOC;

        public Section_DICT() : base(Section.DICT) { }

        public Section_DICT(BinaryReader binaryReader) : base(binaryReader, Section.DICT)
        {
            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.ATOC.ToString()) throw new Exception();
            ATOC = new Section_ATOC(binaryReader);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LTOC.ToString()) throw new Exception();
            LTOC = new Section_LTOC(binaryReader);
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DICT;

            ATOC.SetBytes(ref listBytes);
            LTOC.SetBytes(ref listBytes);
        }
    }
}
