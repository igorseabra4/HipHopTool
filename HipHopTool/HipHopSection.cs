using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HipHopTool.Functions;

namespace HipHopTool
{
    public abstract class HipSection
    {
        public string sectionName;
        public uint sectionSize;
        public byte[] data;
        public HipSection[] ChildSections;        
    }

    public class Section_Node_Generic : HipSection
    {
        public Section_Node_Generic() { }

        public Section_Node_Generic(HipSection section)
        {
            sectionName = section.sectionName;
            sectionSize = section.sectionSize;
            data = section.data;

            BinaryReader fileReader = new BinaryReader(new MemoryStream(data));
            List<HipSection> sections = new List<HipSection>();

            while (fileReader.BaseStream.Position < fileReader.BaseStream.Length)
                sections.Add(ReadSection(fileReader));

            ChildSections = sections.ToArray();
        }
    }

    public class Section_AHDR : HipSection
    {
        public int unknown1;
        public string fileType;
        public int fileOffset;
        public int fileSize;
        public int unknown2;
        public int unknown3;

        public byte[] containedFile;

        public Section_AHDR(HipSection section)
        {
            sectionName = section.sectionName;
            sectionSize = section.sectionSize;
            data = section.data;

            BinaryReader fileReader = new BinaryReader(new MemoryStream(data));

            unknown1 = Switch(fileReader.ReadInt32());
            fileType = new string(fileReader.ReadChars(4));
            fileOffset = Switch(fileReader.ReadInt32());
            fileSize = Switch(fileReader.ReadInt32());
            unknown2 = Switch(fileReader.ReadInt32());
            unknown3 = Switch(fileReader.ReadInt32());

            containedFile = ReadContainedFile(fileOffset, fileSize);

            List<HipSection> sections = new List<HipSection>();

            while (fileReader.BaseStream.Position < fileReader.BaseStream.Length)
                sections.Add(ReadSection(fileReader));

            ChildSections = sections.ToArray();
        }
    }

    public class Section_ADBG : HipSection
    {
        public int unknown1;
        public string fileName;

        public Section_ADBG(HipSection section)
        {
            sectionName = section.sectionName;
            sectionSize = section.sectionSize;
            data = section.data;

            BinaryReader fileReader = new BinaryReader(new MemoryStream(data));

            unknown1 = Switch(fileReader.ReadInt32());
            List<char> charList = new List<char>();

            do
            {
                charList.Add(fileReader.ReadChar());
            }
            while (charList.Last() != 0);
            charList.RemoveAt(charList.Count - 1);

            fileName = new string(charList.ToArray());
        }
    }

    public class Section_LHDR : HipSection
    {
        public int unknownFlags;
        public int amountOfUnknown;
        public int[] unknowns;

        public Section_LHDR(HipSection section)
        {
            sectionName = section.sectionName;
            sectionSize = section.sectionSize;
            data = section.data;

            BinaryReader fileReader = new BinaryReader(new MemoryStream(data));

            unknownFlags = Switch(fileReader.ReadInt32());
            amountOfUnknown = Switch(fileReader.ReadInt32());

            List<int> unknownList = new List<int>(amountOfUnknown);
            for (int i = 0; i < amountOfUnknown; i++)
            {
                unknownList.Add(Switch(fileReader.ReadInt32()));
            }

            List<HipSection> sections = new List<HipSection>();

            while (fileReader.BaseStream.Position < fileReader.BaseStream.Length)
                sections.Add(ReadSection(fileReader));

            ChildSections = sections.ToArray();
        }
    }
}
