using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_PVER : HipSection
    {
        public int subVersion;
        public int clientVersion;
        public int compatible;

        public Section_PVER(int newSubVersion, int newClientVersion, int newCompatible)
        {
            sectionName = Section.PVER;
            subVersion = newSubVersion;
            clientVersion = newClientVersion;
            compatible = newCompatible;
        }

        public Section_PVER(BinaryReader binaryReader)
        {
            sectionName = Section.PVER;
            sectionSize = Switch(binaryReader.ReadInt32());

            long startSectionPosition = binaryReader.BaseStream.Position;

            subVersion = Switch(binaryReader.ReadInt32());
            clientVersion = Switch(binaryReader.ReadInt32());
            compatible = Switch(binaryReader.ReadInt32());

            binaryReader.BaseStream.Position = startSectionPosition + sectionSize;

            if (clientVersion == 262150)
            {
                currentGame = Game.Scooby;
            }
            else if (clientVersion == 655375)
            {
                currentGame = Game.Incredibles; // or BFBB, will check at flag
            }
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.PVER;

            listBytes.AddRange(BitConverter.GetBytes(subVersion).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(clientVersion).Reverse());
            listBytes.AddRange(BitConverter.GetBytes(compatible).Reverse());
        }
    }
}
