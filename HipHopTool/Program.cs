using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using static HipHopFile.Functions;

namespace HipHopTool
{
    class Program
    {
        public enum Option
        {
            ExtractHIP = 0,
            CreateHIP = 1,
            Close = 2,
            None = 3
        }

        [STAThread]
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Console.WriteLine("HipHopTool v0.4.2 by igorseabra4");

            Option option = Option.None;

            if (args.Length == 0)
            {
                while (option != Option.Close)
                {
                    Console.WriteLine();
                    Console.WriteLine("What do you wanna do?");
                    Console.WriteLine("Type 0 to extract a HIP/HOP file.");
                    Console.WriteLine("Type 1 to create a HIP/HOP file.");
                    Console.WriteLine("Type 2 to close.");

                    try
                    {
                        option = (Option)Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        option = Option.None;
                    }

                    if (option == Option.ExtractHIP)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog()
                        {
                            Title = "Select a file to unpack",
                            Filter = "HIP/HOP files|*.hip;*.hop"
                        };

                        if (openFileDialog.ShowDialog(new Form() { TopMost = true, TopLevel = true }) == DialogResult.OK)
                        {
                            Console.WriteLine("File: " + openFileDialog.FileName);
                            HipArrayToIni(HipFileToHipArray(openFileDialog.FileName), openFileDialog.FileName + ".d", true);
                        }
                    }
                    else if (option == Option.CreateHIP)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog()
                        {
                            Title = "Select an INI file",
                            Filter = "INI files|*.ini"
                        };
                        if (openFileDialog.ShowDialog(new Form() { TopMost = true, TopLevel = true }) == DialogResult.OK)
                        {
                            SaveFileDialog saveFileDialog = new SaveFileDialog()
                            {
                                Title = "Save as...",
                                Filter = "HIP/HOP files|*.hip;*.hop"
                            };
                            if (saveFileDialog.ShowDialog(new Form() { TopMost = true, TopLevel = true }) == DialogResult.OK)
                            {
                                File.WriteAllBytes(saveFileDialog.FileName, HipArrayToFile(IniToHipArray(openFileDialog.FileName)));
                            }
                        }
                    }
                }
            }
            else
            {
                string hipToUnpack = "null";
                string outputExtractPath = "null";
                string iniToCreate = "null";
                bool multiFolder = true;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].ToLower() == "-extract" | args[i].ToLower() == "-e" | args[i].ToLower() == "-unpack" | args[i].ToLower() == "-u")
                    {
                        hipToUnpack = args[i + 1];
                        option = Option.ExtractHIP;
                    }
                    else if (args[i].ToLower() == "-dest" | args[i].ToLower() == "-d")
                    {
                        outputExtractPath = args[i + 1];
                    }
                    else if (args[i].ToLower() == "-mode" | args[i].ToLower() == "-m")
                    {
                        if (args[i + 1].ToLower().Contains("single"))
                        {
                            multiFolder = false;
                        }
                        else if (args[i + 1].ToLower().Contains("multi"))
                        {
                            multiFolder = true;
                        }
                    }
                    else if (args[i].ToLower() == "-create" | args[i].ToLower() == "-c")
                    {
                        iniToCreate = args[i + 1];
                        option = Option.CreateHIP;
                    }
                }

                if (option == Option.ExtractHIP)
                {
                    Console.WriteLine("File: " + hipToUnpack);

                    if (outputExtractPath == "null")
                        outputExtractPath = hipToUnpack + ".d";

                    Console.WriteLine("Destination: " + outputExtractPath);

                    HipArrayToIni(HipFileToHipArray(hipToUnpack), outputExtractPath, multiFolder);

                    Console.WriteLine("Success");
                }
                else if (option == Option.CreateHIP)
                {
                    Console.WriteLine("File: " + iniToCreate);

                    if (outputExtractPath == "null")
                        outputExtractPath = Path.ChangeExtension(iniToCreate, ".HIP");

                    Console.WriteLine("Destination: " + outputExtractPath);

                    File.WriteAllBytes(outputExtractPath, HipArrayToFile(IniToHipArray(iniToCreate)));

                    Console.WriteLine("Success");
                }
                else if (option == Option.None)
                {
                    foreach (string s in args)
                    {
                        if (Path.GetExtension(s).ToLower() == ".hip" | Path.GetExtension(s).ToLower() == ".hop")
                        {
                            Console.WriteLine("File: " + s);
                            Console.WriteLine("Destination: " + s + ".d");                            
                            HipArrayToIni(HipFileToHipArray(s), s + ".d", true);
                            Console.WriteLine("Success");
                        }
                    }
                }
            }
        }        
    }
}