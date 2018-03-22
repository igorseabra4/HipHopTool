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
        [STAThread]
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Console.WriteLine("HipHopTool v0.1 by igorseabra4");

            bool willExit = false;

            while (!willExit)
            {
                Console.WriteLine("What do you wanna do?");
                Console.WriteLine("Type 0 to extract a HIP/HOP file.");
                Console.WriteLine("Type 1 to create a HIP/HOP file.");

                byte option = 2;
                while (option != 0 & option != 1)
                {
                    option = Convert.ToByte(Console.ReadLine());
                }

                if (option == 0)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Title = "Select a file to unpack",
                        Filter = "HIP/HOP files|*.hip;*.hop"
                    };

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Console.WriteLine("File: " + openFileDialog.FileName);
                        UnpackHIP(openFileDialog.FileName);
                        willExit = true;
                    }
                }
                else if (option == 1)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Title = "Select a file to insert to",
                        Filter = "HIP/HOP files|*.hip;*.hop"
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FolderBrowserDialog folderDialog = new FolderBrowserDialog()
                        {
                            Description = "Select a .d folder to get files from",
                            SelectedPath = Path.GetDirectoryName(openFileDialog.FileName)
                        };

                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            SaveFileDialog saveFileDialog = new SaveFileDialog()
                            {
                                Title = "Save as...",
                                Filter = "HIP/HOP files|*.hip;*.hop"
                            };
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                File.WriteAllBytes(saveFileDialog.FileName,
                                    CreateHIPFile(
                                        ReplaceHipFiles(
                                            LoadHip(openFileDialog.FileName),
                                            folderDialog.SelectedPath
                                            )
                                            )
                                            )
                                            ;
                                willExit = true;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Done. Press any key to close this window.");
            Console.ReadKey();
        }

        private static void UnpackHIP(string fileName)
        {
            string unpackFolder = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileName(fileName) + ".d");
            Directory.CreateDirectory(unpackFolder);
            foreach (HipSection i in LoadHip(fileName))
            {
                if (i is Section_DICT DICT)
                    foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                    {
                        string directoryToUnpack = Path.Combine(unpackFolder, AHDR.fileType);

                        if (!Directory.Exists(directoryToUnpack))
                            Directory.CreateDirectory(directoryToUnpack);

                        string filePath;
                        if (AHDR.ADBG.firstString != null)
                            filePath = Path.Combine(directoryToUnpack, AHDR.ADBG.firstString);
                        else
                            filePath = Path.Combine(directoryToUnpack, AHDR.ADBG.secondString);

                        File.WriteAllBytes(filePath, AHDR.containedFile);
                    }
            }
        }
        
        public static BinaryReader masterFileReader;

        private static HipSection[] LoadHip(string fileName)
        {
            List<HipSection> hipFile = new List<HipSection>();

            masterFileReader = new BinaryReader(new FileStream(fileName, FileMode.Open));

            while (masterFileReader.BaseStream.Position < masterFileReader.BaseStream.Length)
            {
                string currentSection = new string(masterFileReader.ReadChars(4));
                if (currentSection == Section.HIPA.ToString()) hipFile.Add(new Section_HIPA().Read(masterFileReader));
                else if (currentSection == Section.PACK.ToString()) hipFile.Add(new Section_PACK().Read(masterFileReader));
                else if (currentSection == Section.DICT.ToString()) hipFile.Add(new Section_DICT().Read(masterFileReader));
                else if (currentSection == Section.STRM.ToString()) hipFile.Add(new Section_STRM().Read(masterFileReader));
                else throw new Exception(currentSection);
            }

            masterFileReader.Close();

            return hipFile.ToArray();
        }

        private static HipSection[] ReplaceHipFiles(HipSection[] list, string selectedPath)
        {
            string[] Folders = Directory.GetDirectories(selectedPath);

            Dictionary<string, Dictionary<string, byte[]>> FileDictionary = new Dictionary<string, Dictionary<string, byte[]>>();

            foreach (string f in Folders)
            {
                FileDictionary.Add(Path.GetFileName(f), new Dictionary<string, byte[]>());

                foreach (string i in Directory.GetFiles(f))
                {
                    byte[] file = File.ReadAllBytes(i);
                    FileDictionary[Path.GetFileName(f)].Add(Path.GetFileName(i), file);
                }
            }

            List<byte> newStream = new List<byte>();
            int relativeStartOffset = 0;

            foreach (HipSection i in list)
            {
                if (i is Section_STRM STRM)
                {
                    relativeStartOffset = STRM.DPAK.relativeStartOffset;
                    //STRM.DPAK.firstPadding = 0;
                    newStream = STRM.DPAK.data.ToList();
                }
            }

            foreach (HipSection i in list)
            {
                if (i is Section_DICT DICT)
                    foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                    {
                        if (AHDR.ADBG.firstString != null)
                            AHDR.containedFile = FileDictionary[AHDR.fileType.Replace(" ", "")][AHDR.ADBG.firstString];
                        else
                            AHDR.containedFile = FileDictionary[AHDR.fileType.Replace(" ", "")][AHDR.ADBG.secondString];

                        newStream.RemoveRange(AHDR.fileOffset - relativeStartOffset, AHDR.fileSize);
                        newStream.InsertRange(AHDR.fileOffset - relativeStartOffset, AHDR.containedFile);
                        //AHDR.fileOffset = relativeStartOffset + newStream.Count();
                        //AHDR.fileSize = AHDR.containedFile.Length;

                        //newStream.AddRange(AHDR.containedFile);
                        //AHDR.plusValue = 0;
                        //for (int j = 0; j < AHDR.plusValue; j++)
                        //newStream.Add(0x33);
                    }
            }

            foreach (HipSection i in list)
            {
                if (i is Section_STRM STRM)
                {
                    STRM.DPAK.data = newStream.ToArray();
                }
            }

            return list;
        }

        private static byte[] CreateHIPFile(HipSection[] hipFile)
        {
            List<byte> list = new List<byte>();
            foreach (HipSection i in hipFile)
                list.AddRange(i.GetBytes());

            return list.ToArray();
        }
    }
}
