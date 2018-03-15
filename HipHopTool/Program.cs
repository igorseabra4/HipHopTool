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
    class Program
    {        
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            string[] Arguments = Environment.GetCommandLineArgs();

            if (Arguments.Length > 1)
            {
                foreach (string i in Arguments)
                    if (Path.GetExtension(i).ToLower() == ".hip" | Path.GetExtension(i).ToLower() == ".hop")
                        Run(i);
            }
            else
            {
                string[] FilesInFolder = Directory.GetFiles(Directory.GetCurrentDirectory());

                foreach (string i in FilesInFolder)
                    if (Path.GetExtension(i).ToLower() == ".hip" | Path.GetExtension(i).ToLower() == ".hop")
                        Run(i);
            }
            Console.ReadKey();
        }

        static void Run(string i)
        {
            LoadHip(i);
            string hipFilePath = Path.GetDirectoryName(i);
            string unpackFolder = Path.Combine(hipFilePath, Path.GetFileName(i) + ".d");
            Directory.CreateDirectory(unpackFolder);
            ExtractAllFilesTo(unpackFolder);
        }

        public static List<HipSection> hipFile;        
        public static BinaryReader masterFileReader;

        static void LoadHip(string fileName)
        {
            hipFile = new List<HipSection>();

            masterFileReader = new BinaryReader(new FileStream(fileName, FileMode.Open));

            while (masterFileReader.BaseStream.Position < masterFileReader.BaseStream.Length)
                hipFile.Add(ReadSection(masterFileReader));

            masterFileReader.Close();
        }

        static void ExtractAllFilesTo(string folderName)
        {
            foreach (HipSection i in hipFile)
            {
                if (i.sectionName == "DICT")
                    foreach (HipSection k in i.ChildSections[0].ChildSections)
                    {
                        if (k.sectionName != "AHDR") continue;
                        Section_AHDR j = (Section_AHDR)k;

                        string directoryToUnpack = Path.Combine(folderName, j.fileType);

                        if (!Directory.Exists(directoryToUnpack))
                            Directory.CreateDirectory(directoryToUnpack);

                        string filePath = Path.Combine(directoryToUnpack, ((Section_ADBG)j.ChildSections[0]).fileName + "." + j.fileType);
                        File.WriteAllBytes(filePath, j.containedFile);
                    }
            }
        }
    }
}
