using System.Collections.Generic;

namespace HipHopFile
{
    public class Section_HIPB : HipSection
    {
        public static readonly int CurrentVersion = 3;
        public bool VersionMismatch = false;
        public int Version = CurrentVersion;
        public int HasNoLayers;
        public Game IncrediblesGame;
        public Platform ScoobyPlatform;
        public Dictionary<int, string> LayerNames = new Dictionary<int, string>();

        public Section_HIPB() : base(Section.HIPA)
        {
            HasNoLayers = 0;
            ScoobyPlatform = Platform.Unknown;
            LayerNames = new Dictionary<int, string>();
        }

        public Section_HIPB(BigEndianBinaryReader reader) : base(reader, Section.HIPB)
        {
            Version = reader.ReadInt32();

            HasNoLayers = 0;
            if (Version >= 1)
                HasNoLayers = reader.ReadInt32();

            ScoobyPlatform = Platform.Unknown;

            LayerNames = new Dictionary<int, string>();

            if (Version >= 2)
            {
                ScoobyPlatform = (Platform)reader.ReadInt32();
                int customLayerNameCount = reader.ReadInt32();
                for (int i = 0; i < customLayerNameCount; i++)
                {
                    int index = reader.ReadInt32();
                    string layerName = reader.ReadString();
                    LayerNames[index] = layerName;
                }
            }

            IncrediblesGame = Game.Unknown;

            if (Version >= 3)
                IncrediblesGame = (Game)reader.ReadInt32();

            if (Version > CurrentVersion)
                VersionMismatch = true;
        }

        public override void Setwriter(Game game, Platform platform, BigEndianBinaryWriter writer)
        {
            sectionType = Section.HIPB;

            writer.Write(CurrentVersion);
            if (CurrentVersion >= 1)
                writer.Write(HasNoLayers);
            if (CurrentVersion >= 2)
            {
                writer.Write((int)ScoobyPlatform);
                writer.Write(LayerNames.Count);
                foreach (int index in LayerNames.Keys)
                {
                    writer.Write(index);
                    writer.Write(LayerNames[index]);
                }
            }
            if (CurrentVersion >= 3)
                writer.Write((int)IncrediblesGame);
        }

        public string GetLayerName(int index)
        {
            if (LayerNames.ContainsKey(index))
                return LayerNames[index];
            return null;
        }
    }
}
