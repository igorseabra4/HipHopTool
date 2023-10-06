using System;
using System.Collections.Generic;
using System.IO;

namespace HipHopFile
{
    public class Section_LTOC : HipSection
    {
        public Section_LINF LINF;
        public List<Section_LHDR> LHDRList;

        public Section_LTOC() : base(Section.LTOC)
        {
            LINF = new Section_LINF(0);
            LHDRList = new List<Section_LHDR>();
        }

        public Section_LTOC(BinaryReader binaryReader) : base(binaryReader, Section.LTOC)
        {
            long startSectionPosition = binaryReader.BaseStream.Position;

            string currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LINF.ToString())
                throw new Exception();
            LINF = new Section_LINF(binaryReader);

            LHDRList = new List<Section_LHDR>();

            while (binaryReader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(binaryReader.ReadChars(4));
                if (currentSectionName != Section.LHDR.ToString())
                    throw new Exception();
                LHDRList.Add(new Section_LHDR(binaryReader));
            }

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.LTOC;

            LINF.SetBytes(game, platform, ref listBytes);
            foreach (Section_LHDR i in LHDRList)
                i.SetBytes(game, platform, ref listBytes);
        }
    }
}