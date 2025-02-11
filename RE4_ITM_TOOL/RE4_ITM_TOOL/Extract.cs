﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_ITM_TOOL
{
    internal static class Extract
    {
        public static void ExtractFile(string file, Endianness endianness)
        {
            FileInfo fileInfo = new FileInfo(file);

            string baseDirectory = Path.GetDirectoryName(fileInfo.FullName);
            string baseFileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);

            string baseFilePath = Path.Combine(baseDirectory, baseFileName);

            string pattern = "^(00)([0-9]{2})$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.CultureInvariant);

            if (regex.IsMatch(baseFileName))
            {
                baseFilePath = Path.Combine(baseDirectory, baseFileName + "_ITM");
            }

            var br = new EndianBinaryReader(fileInfo.OpenRead(), endianness);
            uint Magic = br.ReadUInt32();

            if (Magic != 0x00000003)
            {
                Console.WriteLine("Invalid ITM file!");
                br.Close();
                return;
            }

            var idxitm = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".idxitm")).CreateText();
            idxitm.WriteLine("# github.com/JADERLINK/RE4-ITM-TOOL");
            idxitm.WriteLine("# youtube.com/@JADERLINK");
            idxitm.WriteLine("# RE4 ITM TOOL By JADERLINK");
            idxitm.WriteLine();

            uint item_numbers_offset = br.ReadUInt32();
            uint models_offset = br.ReadUInt32();
            uint tpls_offset = br.ReadUInt32();


            br.BaseStream.Position = item_numbers_offset;
            uint Amount = br.ReadUInt32();

            uint[] ItmIDs = new uint[Amount];

            for (int i = 0; i < Amount; i++)
            {
                uint ID = br.ReadUInt16();
                br.ReadUInt16(); // padding
                br.ReadUInt32(); // padding
                ItmIDs[i] = ID;
            }
           

            if (Amount != 0)
            {
                (uint BinOffset, int BinLength, uint TplOffset, int TplLength)[] Arr = new (uint BinOffset, int BinLength, uint TplOffset, int TplLength)[Amount];

                if (models_offset != 0 && tpls_offset != 0)
                {
                    //bin
                    br.BaseStream.Position = models_offset;
                    uint modelsAmount = br.ReadUInt32();

                    uint[] models = new uint[modelsAmount];

                    for (int i = 0; i < models.Length; i++)
                    {
                        models[i] = br.ReadUInt32();
                    }

                    //tpl
                    br.BaseStream.Position = tpls_offset;
                    uint tplsAmount = br.ReadUInt32();

                    uint[] tpls = new uint[tplsAmount];

                    for (int i = 0; i < tpls.Length; i++)
                    {
                        tpls[i] = br.ReadUInt32();
                    }

                    //parses
                    for (int i = 0; i < Arr.Length; i++)
                    {
                        if (models.Length > i && models[i] != 0)
                        {
                            Arr[i].BinOffset = models[i] + models_offset;
                        }

                        if (tpls.Length > i && tpls[i] != 0)
                        {
                            Arr[i].TplOffset = tpls[i] + tpls_offset;
                        }
                    }
                }

                // tamanhos
                for (int i = 0; i < Arr.Length - 1; i++)
                {
                    { //bin
                        int length = (int)Arr[i + 1].BinOffset - (int)Arr[i].BinOffset;
                        length = (length < 0) ? 0 : length;
                        length = (length == Arr[i + 1].BinOffset) ? 0 : length;
                        Arr[i].BinLength = length;
                    }

                    { //tpl
                        int length = (int)Arr[i + 1].TplOffset - (int)Arr[i].TplOffset;
                        length = (length < 0) ? 0 : length;
                        length = (length == Arr[i + 1].TplOffset) ? 0 : length;
                        Arr[i].TplLength = length;
                    }

                }

                //ultimo
                {  //tpl
                    int length = (int)br.BaseStream.Length - (int)Arr.Last().TplOffset;
                    length = (length == br.BaseStream.Length) ? 0 : length;
                    Arr[Arr.Length - 1].TplLength = length;
                }

                { //bin

                    int length1 = (int)Arr[0].TplOffset - (int)Arr.Last().BinOffset;
                    int length2 = (int)tpls_offset - (int)Arr.Last().BinOffset;
                    int length = (length2 < 0) ? length1 : length2;
                    Arr[Arr.Length - 1].BinLength = length;
                }

                // imprime info
                (uint ID, uint File)[] idxSource = new (uint ID, uint File)[Amount];

                for (int i = 0; i < Arr.Length; i++)
                {
                    idxSource[i].ID = ItmIDs[i];
                    idxSource[i].File = ItmIDs[i];

                    if (Arr[i].BinLength <= 0)
                    {
                        for (int j = i; j < Arr.Length; j++)
                        {
                            if (Arr[j].BinLength > 0) 
                            {
                                idxSource[i].File = ItmIDs[j];
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < idxSource.Length; i++)
                {
                    string res = "ID_0x" + idxSource[i].ID.ToString("X2") + ":itm" + idxSource[i].File.ToString("x3");
                    idxitm.WriteLine(res);
                    Console.WriteLine(res);
                }
                idxitm.Close();

                //extrai os arquivos
                try
                {
                    Directory.CreateDirectory(baseFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error creating directory: " + baseFilePath);
                    Console.WriteLine(ex);
                }

                for (int i = 0; i < Arr.Length; i++)
                {
                    if (Arr[i].BinLength > 0)
                    {
                        br.BaseStream.Position = Arr[i].BinOffset;
                        byte[] arqBIN = br.ReadBytes(Arr[i].BinLength);

                        try
                        {
                            File.WriteAllBytes(Path.Combine(baseFilePath, "itm" + ItmIDs[i].ToString("x3") + ".BIN"), arqBIN);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error saving file: " + "itm" + ItmIDs[i].ToString("x3") + ".BIN");
                            Console.WriteLine(ex);
                        }
                    }

                    if (Arr[i].TplLength > 0)
                    {
                        br.BaseStream.Position = Arr[i].TplOffset;
                        byte[] arqTPL = br.ReadBytes(Arr[i].TplLength);

                        try
                        {
                            File.WriteAllBytes(Path.Combine(baseFilePath, "itm" + ItmIDs[i].ToString("x3") + ".TPL"), arqTPL);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error saving file: " + "itm" + ItmIDs[i].ToString("x3") + ".TPL");
                            Console.WriteLine(ex);
                        }
                    }

                }

            }

            br.Close();
        }
    }
}
