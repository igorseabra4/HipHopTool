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

        public Section_DICT(BinaryReader binaryReader, Platform platform) : base(binaryReader, Section.DICT)
        {
            string currentSectionName;

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.ATOC.ToString()) throw new Exception();
            ATOC = new Section_ATOC(binaryReader, platform);

            currentSectionName = new string(binaryReader.ReadChars(4));
            if (currentSectionName != Section.LTOC.ToString()) throw new Exception();
            LTOC = new Section_LTOC(binaryReader);

            foreach (var AHDR in ATOC.AHDRList)
                if (AHDR.assetType == AssetType.JSP)
                    if (LTOC.LHDRList.Where(LHDR => LHDR.assetIDlist.Contains(AHDR.assetID)).FirstOrDefault().layerType > 9)
                        AHDR.assetType = AssetType.JSPInfo;
        }

        public override void SetListBytes(Game game, Platform platform, ref List<byte> listBytes)
        {
            sectionType = Section.DICT;

            ATOC.SetBytes(game, platform, ref listBytes);
            LTOC.SetBytes(game, platform, ref listBytes);
        }
    }
}
