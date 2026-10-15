using System;
using System.Collections.Generic;
using System.Linq;

namespace HipHopFile
{
    public class Section_ATOC : HipSection
    {
        public Section_AINF AINF;
        public List<Section_AHDR> AHDRList;

        public static bool noAHDR;

        public Section_ATOC() : base(Section.ATOC)
        {
            AINF = new Section_AINF(0);
            AHDRList = new List<Section_AHDR>();
        }

        public Section_ATOC(BigEndianBinaryReader reader, Platform platform) : base(reader, Section.ATOC)
        {
            long startSectionPosition = reader.BaseStream.Position;

            string currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.AINF.ToString())
                throw new Exception();
            AINF = new Section_AINF(reader);

            AHDRList = new List<Section_AHDR>();
            while (reader.BaseStream.Position < startSectionPosition + sectionSize)
            {
                currentSectionName = new string(reader.ReadChars(4));
                if (currentSectionName != Section.AHDR.ToString())
                    throw new Exception();
                AHDRList.Add(new Section_AHDR(reader, platform));
            }

            noAHDR = AHDRList.Count == 0;
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.ATOC;

            AINF.SetBytes(game, platform, writer);

            foreach (Section_AHDR i in AHDRList)
                i.SetBytes(game, platform, writer);

            noAHDR = AHDRList.Count == 0;
        }

        public Section_AHDR GetFromAssetID(uint assetID)
        {
            foreach (var AHDR in AHDRList)
                if (AHDR.assetID == assetID)
                    return AHDR;
            return null;
        }

        public void SortAHDRList()
        {
            AHDRList = AHDRList.OrderBy(ahdr => ahdr.assetID).ToList();
        }
    }
}