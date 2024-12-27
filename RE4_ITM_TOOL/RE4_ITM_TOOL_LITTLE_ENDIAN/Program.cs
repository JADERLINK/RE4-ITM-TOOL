﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_ITM_TOOL_LITTLE_ENDIAN
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("# RE4 ITM TOOL (LITTLE ENDIAN)");
            Console.WriteLine("# By JADERLINK");
            Console.WriteLine("# youtube.com/@JADERLINK");
            Console.WriteLine("# github.com/JADERLINK");
            Console.WriteLine("# VERSION 1.1.0 (2024-12-27)");

            RE4_ITM_TOOL.MainAction.Continue(args, SimpleEndianBinaryIO.Endianness.LittleEndian);
        }
    }
}
