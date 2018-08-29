using System;
using System.IO;
using System.Windows.Forms;
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

            SendMessage("HipHopTool v0.4.3 by igorseabra4");

            Option option = Option.None;

            if (args.Length == 0)
            {
                while (option != Option.Close)
                {
                    SendMessage();
                    SendMessage("What do you wanna do?");
                    SendMessage("Type 0 to extract a HIP/HOP file.");
                    SendMessage("Type 1 to create a HIP/HOP file.");
                    SendMessage("Type 2 to close.");

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
                            SendMessage("File: " + openFileDialog.FileName);
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
                    SendMessage("File: " + hipToUnpack);

                    if (outputExtractPath == "null")
                        outputExtractPath = hipToUnpack + ".d";

                    SendMessage("Destination: " + outputExtractPath);

                    HipArrayToIni(HipFileToHipArray(hipToUnpack), outputExtractPath, multiFolder);

                    SendMessage("Success");
                }
                else if (option == Option.CreateHIP)
                {
                    SendMessage("File: " + iniToCreate);

                    if (outputExtractPath == "null")
                        outputExtractPath = Path.ChangeExtension(iniToCreate, ".HIP");

                    SendMessage("Destination: " + outputExtractPath);

                    File.WriteAllBytes(outputExtractPath, HipArrayToFile(IniToHipArray(iniToCreate)));

                    SendMessage("Success");
                }
                else if (option == Option.None)
                {
                    foreach (string s in args)
                    {
                        if (Path.GetExtension(s).ToLower() == ".hip" | Path.GetExtension(s).ToLower() == ".hop")
                        {
                            SendMessage("File: " + s);
                            SendMessage("Destination: " + s + ".d");                            
                            HipArrayToIni(HipFileToHipArray(s), s + ".d", true);
                            SendMessage("Success");
                        }
                    }
                }
            }
        }        
    }
}