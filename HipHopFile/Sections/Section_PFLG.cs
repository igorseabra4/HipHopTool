namespace HipHopFile
{
    public class Section_PFLG : HipSection
    {
        public int flags;

        public Section_PFLG(int flags) : base(Section.PFLG)
        {
            sectionType = Section.PFLG;
            this.flags = flags;
        }

        public Section_PFLG(BigEndianBinaryReader reader, Game game, out Game outGame) : base(reader, Section.PFLG)
        {
            flags = reader.ReadInt32();

            if (flags != 0x2E & game >= Game.Incredibles)
                outGame = Game.BFBB;
            else
                outGame = game;
        }

        public override void Setwriter(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.PFLG;
            writer.Write(flags);
        }
    }
}
