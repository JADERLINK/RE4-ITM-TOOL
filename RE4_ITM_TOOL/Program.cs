using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_ITM_TOOL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("# RE4 ITM TOOL");
            Console.WriteLine("# By JADERLINK");
            Console.WriteLine("# youtube.com/@JADERLINK");
            Console.WriteLine("# VERSION 1.0.2 (2024-08-17)");

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-ITM-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                FileInfo fileInfo = null;

                try
                {
                    fileInfo = new FileInfo(args[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in the path: " + Environment.NewLine + ex);
                }
                if (fileInfo != null)
                {
                    Console.WriteLine("File: " + fileInfo.Name);

                    if (fileInfo.Extension.ToUpperInvariant() == ".ITM")
                    {
                        try
                        {
                            Console.WriteLine("Extract Mode:");
                            Extract.ExtractFile(fileInfo.FullName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + Environment.NewLine + ex);
                        }

                    }
                    else if (fileInfo.Extension.ToUpperInvariant() == ".IDXITM")
                    {
                        try
                        {
                            Console.WriteLine("Repack Mode:");
                            Repack.RepackFile(fileInfo.FullName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + Environment.NewLine + ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("The extension is not valid: " + fileInfo.Extension);
                    }
                }

            }
            else
            {
                Console.WriteLine("File specified does not exist.");
            }

            Console.WriteLine("Finished!!!");

        }
    }
}
