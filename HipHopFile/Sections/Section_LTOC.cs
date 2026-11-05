namespace HipHopFile;

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

        LINF = new Section_LINF(reader);

        LHDRList = new List<Section_LHDR>();

        while (reader.BaseStream.Position < startSectionPosition + sectionSize)
            LHDRList.Add(new Section_LHDR(reader));

        reader.BaseStream.Position = startSectionPosition + sectionSize;
    }

    public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        LINF.Write(game, platform, writer);
        foreach (Section_LHDR i in LHDRList)
            i.Write(game, platform, writer);
    }
}