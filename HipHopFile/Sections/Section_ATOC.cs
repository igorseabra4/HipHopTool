namespace HipHopFile;

public class Section_ATOC : HipSection
{
    public Section_AINF AINF;
    public List<Section_AHDR> AHDRList;

    public Section_ATOC() : base(Section.ATOC)
    {
        AINF = new Section_AINF(0);
        AHDRList = new List<Section_AHDR>();
    }

    public Section_ATOC(BigEndianBinaryReader reader, Platform platform) : base(reader, Section.ATOC)
    {
        long startSectionPosition = reader.BaseStream.Position;

        AINF = new Section_AINF(reader);

        AHDRList = new List<Section_AHDR>();
        while (reader.BaseStream.Position < startSectionPosition + sectionSize)
            AHDRList.Add(new Section_AHDR(reader, platform));
    }

    public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        AINF.Write(game, platform, writer);

        foreach (Section_AHDR i in AHDRList)
            i.Write(game, platform, writer);
    }

    public void SortAHDRList()
    {
        AHDRList = AHDRList.OrderBy(ahdr => ahdr.assetID).ToList();
    }
}