using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace RE4_ITM_TOOL
{
    internal static class Repack
    {
        public static void RepackFile(string file)
        {
            StreamReader idx = null;
            BinaryWriter itm = null;
            FileInfo fileInfo = new FileInfo(file);
            string baseName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            string baseDiretory = fileInfo.DirectoryName;

            try
            {
                idx = new FileInfo(file).OpenText();
                itm = new BinaryWriter(new FileInfo(baseDiretory + "\\" + baseName + ".ITM").Create(), Encoding.GetEncoding(1252));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + Environment.NewLine + ex);
            }

            if (idx != null)
            {
                Dictionary<uint, string> BaseFileDic = new Dictionary<uint, string>();
                string endLine = "";
                while (endLine != null)
                {
                    endLine = idx.ReadLine();

                    if (endLine != null)
                    {
                        endLine = endLine.Trim();

                        if (!(endLine.Length == 0
                            || endLine.StartsWith("#")
                            || endLine.StartsWith("\\")
                            || endLine.StartsWith("/")
                            || endLine.StartsWith(":")
                            ))
                        {
                            var split = endLine.Split(':');

                            if (split.Length >=2)
                            {
                                int Id = -1;
                                string key = split[0].Trim().ToUpperInvariant().Replace("ID_0X", "");
                                string value = split[1].Trim();
                                if (int.TryParse(key, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out Id))
                                {
                                    if (!BaseFileDic.ContainsKey((uint)Id))
                                    {
                                        BaseFileDic.Add((uint)Id, value);
                                    }

                                } 

                            }

                        }

                    }

                }
                idx.Close();

                //ordena pelo nome do arquivo
                var Content = BaseFileDic.OrderBy(x => x.Key).OrderBy(x => x.Value).ToArray();
                int Amount = Content.Length;

                itm.Write((uint)0x03); // magic
                itm.Write((uint)0x20); // offset dos ids

                itm.BaseStream.Position = 0x20;
                itm.Write((uint)Amount); //quantidade

                for (int i = 0; i < Amount; i++)
                {
                    itm.Write((uint)Content[i].Key);
                    itm.Write((uint)0x00);

                    Console.WriteLine("The ID 0x"+ Content[i].Key .ToString("x2") + " was defined in the file!");
                }

                // calculo offset tabela de bin
                int Id_table_lenght = 0;
                {
                    int div = (0x4 + (Content.Length * 8)) / 32;
                    int rest = (0x4 + (Content.Length * 8)) % 32;
                    div += rest > 0 ? 1 : 0;
                    Id_table_lenght = div * 32;
                }

                uint BIN_OFFSET = 0x20 + (uint)Id_table_lenght;

                itm.BaseStream.Position = 0x08;
                itm.Write((uint)BIN_OFFSET);

                //inserção dos bins
                itm.BaseStream.Position = BIN_OFFSET;
                itm.Write((uint)Amount); //quantidade

                int BinTpl_table_lenght = 0;
                {
                    int div = (0x4 + (Content.Length * 4)) / 32;
                    int rest = (0x4 + (Content.Length * 4)) % 32;
                    div += rest > 0 ? 1 : 0;
                    BinTpl_table_lenght = div * 32;
                }

                uint offsetToNextBin = (uint)BinTpl_table_lenght + BIN_OFFSET;
                uint offsetToSetOffset = (uint)BIN_OFFSET + 0x4;

                uint offsetLastBin = offsetToNextBin;
                string lastFile = "";

                for (int i = 0; i < Amount; i++)
                {
                    string binFilePath = Path.Combine(baseDiretory, baseName, Content[i].Value + ".BIN");

                    itm.BaseStream.Position = offsetToNextBin;
                    FileInfo binInfo = new FileInfo(binFilePath);
                    if (binInfo.Exists && binFilePath != lastFile)
                    {
                        var fileStream = binInfo.OpenRead();
                        fileStream.CopyTo(itm.BaseStream);
                        fileStream.Close();

                        long currentPosition = itm.BaseStream.Position;
                        long cDiv = currentPosition / 16;
                        long cRest = currentPosition % 16;
                        cDiv += cRest > 0 ? 1 : 0;
                        currentPosition = cDiv * 16;
                        offsetLastBin = offsetToNextBin;
                        offsetToNextBin = (uint)currentPosition;
                        lastFile = binFilePath;

                        Console.WriteLine("The file " + binInfo.Name + " was inserted into ITM!");
                    }
                    else if (!binInfo.Exists)
                    {
                        Console.WriteLine("The file "+ binInfo.Name + " does not exist!");
                    }

                    //seta offset
                    itm.BaseStream.Position = offsetToSetOffset;
                    itm.Write((uint)offsetLastBin - BIN_OFFSET);

                    offsetToSetOffset += 0x4;

                }

                //tpl
                uint TPL_OFFSET = offsetToNextBin;

                itm.BaseStream.Position = 0x0C;
                itm.Write((uint)TPL_OFFSET);

                itm.BaseStream.Position = TPL_OFFSET;
                itm.Write((uint)Amount); //quantidade

                uint offsetToNextTPL = (uint)BinTpl_table_lenght + TPL_OFFSET;
                offsetToSetOffset = (uint)TPL_OFFSET + 0x4;

                uint offsetLastTPL = offsetToNextTPL;
                lastFile = "";

                for (int i = 0; i < Amount; i++)
                {
                    string TPLFilePath = Path.Combine(baseDiretory, baseName, Content[i].Value + ".TPL");

                    itm.BaseStream.Position = offsetToNextTPL;
                    FileInfo TPLInfo = new FileInfo(TPLFilePath);
                    if (TPLInfo.Exists && TPLFilePath != lastFile)
                    {
                        var fileStream = TPLInfo.OpenRead();
                        fileStream.CopyTo(itm.BaseStream);
                        fileStream.Close();

                        long currentPosition = itm.BaseStream.Position;
                        long cDiv = currentPosition / 16;
                        long cRest = currentPosition % 16;
                        cDiv += cRest > 0 ? 1 : 0;
                        currentPosition = cDiv * 16;
                        offsetLastTPL = offsetToNextTPL;
                        offsetToNextTPL = (uint)currentPosition;
                        lastFile = TPLFilePath;

                        Console.WriteLine("The file " + TPLInfo.Name + " was inserted into ITM!");
                    }
                    else if (!TPLInfo.Exists)
                    {
                        Console.WriteLine("The file " + TPLInfo.Name + " does not exist!");
                    }

                    //seta offset
                    itm.BaseStream.Position = offsetToSetOffset;
                    itm.Write((uint)offsetLastTPL - TPL_OFFSET);

                    offsetToSetOffset += 0x4;
                }

                //alinhamento
                itm.BaseStream.Position = itm.BaseStream.Length;
                long oldFullSize = itm.BaseStream.Length;
                long oDiv = oldFullSize / 16;
                long oRest = oldFullSize % 16;
                oDiv += oRest > 0 ? 1 : 0;
                long Dif = (oDiv * 16) - oldFullSize;
                itm.Write(new byte[Dif]);

                itm.Close();
            }

        }
    }
}
