namespace HipHopFile
{
    public class Section_PVER : HipSection
    {
        public int subVersion;
        public int clientVersion;
        public int compatible;

        public Section_PVER(int subVersion, int clientVersion, int compatible) : base(Section.PVER)
        {
            this.subVersion = subVersion;
            this.clientVersion = clientVersion;
            this.compatible = compatible;
        }

        public Section_PVER(BigEndianBinaryReader reader, out Game game) : base(reader, Section.PVER)
        {
            subVersion = reader.ReadInt32();
            clientVersion = reader.ReadInt32();
            compatible = reader.ReadInt32();

            if (clientVersion == 262150)
                game = Game.Scooby;
            else if (clientVersion == 655375)
                game = Game.Incredibles; // or BFBB, will check at PFLG
            else
                game = Game.Unknown;
        }

        public override void Serialize(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.PVER;

            writer.Write(subVersion);
            writer.Write(clientVersion);
            writer.Write(compatible);
        }
    }
}
