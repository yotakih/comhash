using System;
using System.Collections.Generic;
using System.IO;

namespace ComHash
{
    class Program
    {
        const string DLM = "\t";
        Dictionary<string, string> DicContext = new Dictionary<string, string>();
        string RootDir = "";
        string TargetListFilePath = "";
        string OutputFilePath = "";
        StreamWriter SwOutput = null;
        static void Main(string[] args)
        {
            Console.WriteLine($"Compute Hash Start!! {DateTime.Now}");
            new Program().Proc(args, ComHashClass.ComHashFunc);
            Console.WriteLine($"Compute Hash End!! {DateTime.Now}");
        }
        void Proc(string[] args, Func<string, Dictionary<string, string>, string> func)
        {
            try
            {
                if (!this.AnalizeArgs(args))
                    return;
                this.SwOutput = new StreamWriter(this.OutputFilePath, false);
                using (var sr = new StreamReader(this.TargetListFilePath))
                {
                    string ln = sr.ReadLine();
                    while (!(ln is null))
                    {
                        string[] splt = ln.Split(DLM);
                        if (splt.Length == 2){
                            var interdir = splt[0];
                            var fulldir = Path.Join(this.RootDir, interdir);
                            Console.WriteLine(fulldir);
                            var ext = splt[1];
                            if (!Directory.Exists(fulldir)) return;
                            foreach (string filepath in Directory.GetFiles(fulldir))
                            {
                                if (Path.GetExtension(filepath).ToLower().Equals(ext))
                                {
                                    this.SwOutput.WriteLine(
                                        String.Format(
                                            "{1}{0}{2}{0}{3}",
                                             DLM,
                                             filepath.Substring(this.RootDir.Length),
                                             filepath,
                                             func(filepath, this.DicContext)
                                        ));
                                }
                            }
                        }
                        ln = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"error: {e.Message}");
            }
            finally
            {
                if (!(this.SwOutput is null))
                    this.SwOutput.Close();
            }
        }
        bool AnalizeArgs(string[] args)
        {
            bool ret = true;
            switch (args.Length)
            {
                case 0:
                case 1:
                    Usage();
                    ret = false;
                    break;
                case 2:
                    this.RootDir = args[0];
                    this.TargetListFilePath = args[1];
                    this.OutputFilePath = "comhash.txt";
                    ret = true;
                    break;
                case 3:
                    this.RootDir = args[0];
                    this.TargetListFilePath = args[1];
                    this.OutputFilePath = args[2];
                    ret = true;
                    break;
                default:
                    Usage();
                    ret = false;
                    break;
            }
            this.DicContext["RootDir"] = this.RootDir;
            this.DicContext["TargetListFilePath"] = this.TargetListFilePath;
            this.DicContext["OutputFilePath"] = this.OutputFilePath;
            return ret;
        }
        void Usage()
        {
            Console.WriteLine(
                $"Usage:\n" +
                $" arg1: select Root Dir\n" +
                $" arg2: target extension list file path\n" +
                $" arg3: output file path"
            );
        }
    }
}
