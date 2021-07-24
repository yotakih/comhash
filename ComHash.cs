using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ComHash
{
    class ComHashClass
    {
        public static string ComHashFunc(string filepath, Dictionary<string, string> dicContext)
        {
            var bufferSize = 4096;
            var waitTasks = new Task[2];
            var read = 0;
            byte[] bint;
            var bin1 = new byte[bufferSize];
            var bin2 = new byte[bufferSize];

            using(var md5 = MD5.Create())
            {
                using(var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    using(var cs = new CryptoStream(Stream.Null, md5, CryptoStreamMode.Write))
                    {
                        do
                        {
                            waitTasks[0] = cs.WriteAsync(bin2, 0, read);
                            Task<Int32> readTask = fs.ReadAsync(bin1, 0, bufferSize);
                            waitTasks[1] = readTask;
                            Task.WaitAll(waitTasks);
                            read = readTask.Result;
                            bint = bin1;
                            bin1 = bin2;
                            bin2 = bint;
                        }while(read > 0);
                        cs.FlushFinalBlock();
                    }
                }
                return String.Format(
                    @"{0}",
                    BitConverter.ToString(
                        md5.Hash
                    ).Replace("-","")
                );
            }
        }
    }
}
