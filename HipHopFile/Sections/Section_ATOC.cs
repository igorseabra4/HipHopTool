using System;
using System.Collections.Generic;
using System.IO;

namespace HipHopFile
{
    public class Section_ATOC : HipSection
    {
        public Section_AINF AINF;
        public List<Section_AHDR> AHDRList;

        public static bool noAHDR;

        public Section_ATOC() : base(Section.ATOC)
        {
            AHDRList = new List<Section_AHDR>();
        }

        public Section_ATOC(BinaryReader binaryReader) : base(binaryReader, Section.ATOC)
        {
            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.AINF.ToString()) throw new Exception();
            AINF = new Section_AINF(binaryReader);

            AHDRList = new List<Section_AHDR>();
            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(binaryReader.ReadChars(4));
                if (currentSectionName != Section.AHDR.ToString()) throw new Exception();
                AHDRList.Add(new Section_AHDR(binaryReader));
            }

            noAHDR = AHDRList.Count == 0;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.ATOC;

            AINF.SetBytes(ref listBytes);

            foreach (Section_AHDR i in AHDRList)
                i.SetBytes(ref listBytes);

            noAHDR = AHDRList.Count == 0;
        }
    }
}