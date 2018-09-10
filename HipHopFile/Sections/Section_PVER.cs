using System.Collections.Generic;
using System.IO;
using static HipHopFile.Functions;

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

        public Section_PVER(BinaryReader binaryReader) : base(binaryReader, Section.PVER)
        {
            subVersion = Switch(binaryReader.ReadInt32());
            clientVersion = Switch(binaryReader.ReadInt32());
            compatible = Switch(binaryReader.ReadInt32());
            
            if (clientVersion == 262150)
                currentGame = Game.Scooby;
            else if (clientVersion == 655375)
                currentGame = Game.Incredibles; // or BFBB, will check at PFLG
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PVER;

            listBytes.AddBigEndian(subVersion);
            listBytes.AddBigEndian(clientVersion);
            listBytes.AddBigEndian(compatible);
        }
    }
}
