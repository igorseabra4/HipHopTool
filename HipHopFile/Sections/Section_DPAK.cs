using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HipHopFile.Functions;

namespace HipHopFile
{
    public class Section_DPAK : HipSection
    {
        public int globalRelativeStartOffset;
        public byte[] data;

        public Section_DPAK() : base(Section.DPAK) { }

        public Section_DPAK(BinaryReader binaryReader) : base(binaryReader, Section.DPAK)
        {
            if (Section_ATOC.noAHDR)
            {
                data = binaryReader.ReadBytes(sectionSize);
            }
            else
            {
                int firstPadding = Switch(binaryReader.ReadInt32());

                binaryReader.BaseStream.Position += firstPadding;

                globalRelativeStartOffset = (int)binaryReader.BaseStream.Position;

                int sizeOfData = sectionSize - 4 - firstPadding;
                data = binaryReader.ReadBytes(sizeOfData);
            }
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionName = Section.DPAK;

            int firstPaddingPosition = listBytes.Count;

            listBytes.Add(0);
            listBytes.Add(0);
            listBytes.Add(0);
            listBytes.Add(0);

            while (listBytes.Count % 0x20 != 0)
                listBytes.Add(0x33);
            
            globalRelativeStartOffset = listBytes.Count();

            int firstPadding = listBytes.Count - firstPaddingPosition;

            byte[] firstPaddingBytes = BitConverter.GetBytes(firstPadding);
            listBytes[firstPaddingPosition + 0] = firstPaddingBytes[3];
            listBytes[firstPaddingPosition + 1] = firstPaddingBytes[2];
            listBytes[firstPaddingPosition + 2] = firstPaddingBytes[1];
            listBytes[firstPaddingPosition + 3] = firstPaddingBytes[0];

            listBytes.AddRange(data);
        }
    }
}