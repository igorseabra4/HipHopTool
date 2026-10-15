using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HipHopFile
{
    public class Section_DICT : HipSection
    {
        public Section_ATOC ATOC;
        public Section_LTOC LTOC;

        public Section_DICT() : base(Section.DICT)
        {
            ATOC = new Section_ATOC();
            LTOC = new Section_LTOC();
        }

        public Section_DICT(BigEndianBinaryReader reader, Platform platform) : base(reader, Section.DICT)
        {
            string currentSectionName;

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.ATOC.ToString())
                throw new Exception();
            ATOC = new Section_ATOC(reader, platform);

            currentSectionName = new string(reader.ReadChars(4));
            if (currentSectionName != Section.LTOC.ToString())
                throw new Exception();
            LTOC = new Section_LTOC(reader);

            foreach (var AHDR in ATOC.AHDRList)
                if (AHDR.assetType == AssetType.JSP)
                    if (LTOC.LHDRList.First(LHDR => LHDR.assetIDlist.Contains(AHDR.assetID)).layerType > 9)
                        AHDR.assetType = AssetType.JSPInfo;
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.DICT;

            ATOC.SetBytes(game, platform, writer);
            LTOC.SetBytes(game, platform, writer);
        }
    }
}
