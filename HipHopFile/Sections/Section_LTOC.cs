using System;
using System.Collections.Generic;

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

        public Section_LTOC(BigEndianBinaryReader reader) : base(reader, Section.LTOC)
        {
            long startSectionPosition = reader.BaseStream.Position;

            string currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.LINF.ToString())
                throw new Exception();
            LINF = new Section_LINF(reader);

            LHDRList = new List<Section_LHDR>();

            while (reader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(reader.ReadChars(4));
                if (currentSectionName != Section.LHDR.ToString())
                    throw new Exception();
                LHDRList.Add(new Section_LHDR(reader));
            }

            reader.BaseStream.Position = startSectionPosition + sectionSize;
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.LTOC;

            LINF.SetBytes(game, platform, writer);
            foreach (Section_LHDR i in LHDRList)
                i.SetBytes(game, platform, writer);
        }
    }
}