using System;
using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_LTOC : HipSection
    {
        public Section_LINF LINF;
        public List<Section_LHDR> LHDRList;

        public Section_LTOC() : base(Section.LTOC)
        {
            LHDRList = new List<Section_LHDR>();
        }

        public Section_LTOC(BinaryReader binaryReader) : base(binaryReader, Section.LTOC)
        {
            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LINF.ToString()) throw new Exception();
            LINF = new Section_LINF(binaryReader);

            LHDRList = new List<Section_LHDR>();

            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(binaryReader.ReadChars(4));
                if (currentSectionName != Section.LHDR.ToString()) throw new Exception();
                LHDRList.Add(new Section_LHDR(binaryReader));
            }

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.LTOC;

            LINF.SetBytes(ref listBytes);
            foreach (Section_LHDR i in LHDRList)
                i.SetBytes(ref listBytes);
        }
    }
}