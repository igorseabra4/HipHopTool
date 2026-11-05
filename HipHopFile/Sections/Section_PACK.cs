namespace HipHopFile;

public class Section_PACK : HipSection
{
    public Section_PVER PVER;
    public Section_PFLG PFLG;
    public Section_PCNT PCNT;
    public Section_PCRT PCRT;
    public Section_PMOD PMOD;
    public Section_PLAT PLAT;

    public Section_PACK() : base(Section.PACK) { }

    public Section_PACK(BigEndianBinaryReader reader, out Game game, out Platform platform) : base(reader, Section.PACK)
    {
        long startSectionPosition = reader.BaseStream.Position;

        PVER = new Section_PVER(reader, out game);
        PFLG = new Section_PFLG(reader, game, out game);
        PCNT = new Section_PCNT(reader);
        PCRT = new Section_PCRT(reader);
        PMOD = new Section_PMOD(reader);

        if (reader.BaseStream.Position == startSectionPosition + sectionSize)
        {
            platform = Platform.Unknown;
            return;
        }

        PLAT = new Section_PLAT(reader, game, out platform);
    }

    public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
    {
        PVER.Write(game, platform, writer);
        PFLG.Write(game, platform, writer);
        PCNT.Write(game, platform, writer);
        PCRT.Write(game, platform, writer);
        PMOD.Write(game, platform, writer);
        PLAT?.Write(game, platform, writer);
    }
}
