namespace HipHopFile;

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
        ATOC = new Section_ATOC(reader, platform);
        LTOC = new Section_LTOC(reader);

        foreach (var AHDR in ATOC.AHDRList)
            if (AHDR.assetType == AssetType.JSP)
                if (LTOC.LHDRList.First(LHDR => LHDR.assetIDlist.Contains(AHDR.assetID)).layerType > 9)
                    AHDR.assetType = AssetType.JSPInfo;
    }

    public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        ATOC.Write(game, platform, writer);
        LTOC.Write(game, platform, writer);
    }
}
