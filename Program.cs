using System;
using System.IO;
using System.Security.Cryptography;

namespace ComHash
{
    class Program
    {
        const string DLM = "\t";
        MD5 Md5 = MD5.Create();
        string TargetListFilePath = "";
        string OutputFilePath = "";
        StreamWriter SwOutput = null;
        static void Main(string[] args)
        {
            Console.WriteLine($"Compute Hash Start!! {DateTime.Now}");
            new Program().Proc(args);
            Console.WriteLine($"Compute Hash End!! {DateTime.Now}");
        }
        void Proc(string[] args)
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
                            this.Compute(splt[0], splt[1]);
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
                this.Md5.Dispose();
                if (!(this.SwOutput is null))
                    this.SwOutput.Close();
            }
        }
        bool AnalizeArgs(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    Usage();
                    return false;
                case 1:
                    this.TargetListFilePath = args[0];
                    this.OutputFilePath = "comhash.txt";
                    return true;
                case 2:
                    this.TargetListFilePath = args[0];
                    this.OutputFilePath = args[1];
                    return true;
                default:
                    Usage();
                    return false;
            }
        }
        void Usage()
        {
            Console.WriteLine(
                $"Usage:\n" +
                $" arg1: target extension list file path\n" +
                $" arg2: output file path"
            );
        }
        void Compute(string dir, string ext)
        {
            foreach (string filepath in Directory.GetFiles(dir))
            {
                if (Path.GetExtension(filepath).ToLower().Equals(ext))
                {
                    using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        this.SwOutput.WriteLine(
                            String.Format(
                                "{1}{0}{2}",
                                 DLM,
                                 filepath,
                                 BitConverter.ToString(this.Md5.ComputeHash(fs)).Replace("-","")
                            ));
                    }
                }
            }
        }
    }
}
