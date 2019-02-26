using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestartCloverProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            //FileStream fs = new FileStream(@"C:\clover\saleRequest.txt", FileMode.Open);
            //Console.ReadKey();
            string SaleFilePath = "c:/clover/sale.txt";
            string output = "FAILED\tCANCEL\tBOOOM";

            File.WriteAllText(SaleFilePath, output);


            KillProcess();
            string ExePath = "c:/clover/nona_clover_integration.exe";

            StartProcess(ExePath);

            

        }

        public static void StartProcess(string processName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = processName;
            proc.Start();

        }

        public static void KillProcess()
        {
            Process[] processes = null;

            if (Process.GetProcessesByName("nona_clover_integration").Length > 0)
            {
                try
                {
                    processes = Process.GetProcessesByName("nona_clover_integration");

                    Process app = processes[0];

                    if (!app.HasExited)
                    {
                        app.Kill();
                    }
                }
                catch (Exception ex) { }
                finally
                {
                    if (processes != null)
                    {
                        foreach (Process p in processes)
                        {
                            p.Dispose();
                        }
                    }
                }
            }
        }
    }
}
