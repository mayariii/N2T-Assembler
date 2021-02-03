using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace N2T_Assembler
{
    internal class Assembler
    {
        private static void Main(string[] args)

        {
            //file input & naming
            string sourceFilePath = "C:\\test\\PongL.asm";
            string fileNameIn = Path.GetFileName(sourceFilePath);
            string fileNameOut = Path.GetFileNameWithoutExtension(sourceFilePath) + ".hack";
            string destFilePath = Path.GetDirectoryName(sourceFilePath) + "\\" + fileNameOut;

            //read file
            IEnumerable<string> lines = File.ReadLines(sourceFilePath);

            // comp bits dictionary
            Dictionary<string, string> compDictionary =
                new Dictionary<string, string>();

            compDictionary.Add("0", "101010");
            compDictionary.Add("1", "111111");
            compDictionary.Add("-1", "111010");
            compDictionary.Add("D", "001100");
            compDictionary.Add("A", "110000");
            compDictionary.Add("M", "110000");
            compDictionary.Add("!D", "001101");
            compDictionary.Add("!A", "110001");
            compDictionary.Add("!M", "110001");
            compDictionary.Add("-D", "001111");
            compDictionary.Add("-A", "110011");
            compDictionary.Add("-M", "110011");
            compDictionary.Add("D+1", "011111");
            compDictionary.Add("A+1", "110111");
            compDictionary.Add("M+1", "110111");
            compDictionary.Add("D-1", "001110");
            compDictionary.Add("A-1", "110010");
            compDictionary.Add("M-1", "110010");
            compDictionary.Add("D+A", "000010");
            compDictionary.Add("D+M", "000010");
            compDictionary.Add("D-A", "010011");
            compDictionary.Add("D-M", "010011");
            compDictionary.Add("A-D", "000111");
            compDictionary.Add("M-D", "000111");
            compDictionary.Add("D&A", "000000");
            compDictionary.Add("D&M", "000000");
            compDictionary.Add("D|A", "010101");
            compDictionary.Add("D|M", "010101");

            // destination bits dictionary
            Dictionary<string, string> destDictionary =
                new Dictionary<string, string>();

            destDictionary.Add("null", "000");
            destDictionary.Add("M", "001");
            destDictionary.Add("D", "010");
            destDictionary.Add("MD", "011");
            destDictionary.Add("A", "100");
            destDictionary.Add("AM", "101");
            destDictionary.Add("AD", "110");
            destDictionary.Add("AMD", "111");

            // jump bits dictionary
            Dictionary<string, string> jumpDictionary =
                new Dictionary<string, string>();

            jumpDictionary.Add("null","000");
            jumpDictionary.Add("JGT", "001");
            jumpDictionary.Add("JEQ", "010");
            jumpDictionary.Add("JGE", "011");
            jumpDictionary.Add("JLT", "100");
            jumpDictionary.Add("JNE", "101");
            jumpDictionary.Add("JLE", "110");
            jumpDictionary.Add("JMP", "111");

            // dictionary for pre-defined symbols
            Dictionary<string, Int32> symbolDictionary =
                new Dictionary<string, Int32>();

            compDictionary.Add("R0", "0");
            compDictionary.Add("R1", "1");
            compDictionary.Add("R2", "2");
            compDictionary.Add("R3", "3");
            compDictionary.Add("R4", "4");
            compDictionary.Add("R5", "5");
            compDictionary.Add("R6", "6");
            compDictionary.Add("R7", "7");
            compDictionary.Add("R8", "8");
            compDictionary.Add("R9", "9");
            compDictionary.Add("R10", "10");
            compDictionary.Add("R11", "11");
            compDictionary.Add("R12", "12");
            compDictionary.Add("R13", "13");
            compDictionary.Add("R14", "14");
            compDictionary.Add("R15", "15");
            compDictionary.Add("SCREEN", "16384");
            compDictionary.Add("KBD", "24576");

            // write to file
            string[] dataToWrite = lines.ToArray();
            string convertedString = "";
            string comp = "";

            using (StreamWriter outputFile = new StreamWriter(destFilePath))
            {
                foreach (string line in dataToWrite)
                {
                    if (line.StartsWith("//") || string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    else  
                    {
                        
                        // break it down now y'all
                        if (line.StartsWith("@"))
                        {
                            //process A instruction
                            string address = line.Split('@')[1];
                            var convertedAddress = Convert.ToString(Int32.Parse(address), 2).PadLeft(16,'0');
                            outputFile.WriteLine(convertedAddress);
                        }

                        if (line.Contains("=") || line.Contains(";"))
                        {
                            
                            //process C instruction

                            if (line.Contains("=") && !line.Contains(";")) //e.g. MD=M+1
                            {
                                
                                var dest = line.Split('=')[0];
                                string destConverted = destDictionary[dest];

                                comp = line.Split('=', ';')[1];
                                string compConverted = compDictionary[comp];
                                var aBit = comp.Contains("M") ? "1" : "0";

                                convertedString = compConverted + destConverted + "000";
                                outputFile.WriteLine("111" + aBit + convertedString);
                            }

                            if (!line.Contains("=") && line.Contains(";")) //e.g. 0;JMP
                            {
                                comp = line.Split('=', ';')[0];
                                string compConverted = compDictionary[comp];
                                var aBit = comp.Contains("M") ? "1" : "0";

                                var jump = line.Split('=', ';')[1];
                                string jumpConverted = jumpDictionary[jump];

                                convertedString = compConverted + "000" + jumpConverted;
                                outputFile.WriteLine("111" + aBit + convertedString);

                            }
                            else
                            {
                                continue;
                            }
                        }

                    }
                }
            }

            Console.WriteLine("Translation complete.");
        }
    }
}
