using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace InterviewQuestions
{

    public class CodonTranslator
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="CodonTable.csv>Filename of the DNA codon table.</param>
        public CodonTranslator(string codonTableFileName)
        {
            var file = new StreamReader("CodonTable.csv");
            var fileContent = file.ReadToEnd();
            BuildTranslationMapFromFileContent(fileContent, Path.GetExtension(codonTableFileName));
        }
        private void BuildTranslationMapFromFileContent(string fileContent, string fileType)
        {
            List<string> filecont = fileContent.Split(',').ToList<string>();
            Dictionary<string, string> Codon_Dict = new Dictionary<string, string>();

            for (int line = 0; line < fileContent.Length; line++)
            {
                //Codon_Dict.Add(line[0], line[1]);
            }

                throw new System.NotImplementedException(string.Format("The contents of the file with type \"{0}\" have been loaded, please make use of it.\n{1}", fileType, fileContent));
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            return "";
        }



        static void Main(string[] args)
        {
            string dna = "ATGCATTAA";

            Dictionary<string, string> Codon_Dict = Read_Csv_Return_Dict();

            int start_index = dna.IndexOf("ATG");
            int end_index = FindEndingIndex(dna);

            string dna_key = "";
            for (int dna_index = start_index; dna_index < end_index; dna_index = dna_index + 3)
            {
                string codon = dna.Substring(dna_index, 3);
                string codon_key = Codon_Dict[codon];

                if (codon_key == "Start")
                {
                    codon_key = "M";
                }

                dna_key = dna_key + codon_key;
            }

            Console.WriteLine("result: " + dna_key);

        }

        public static int FindEndingIndex(string dna)
        {
            int taa_index = dna.IndexOf("TAA");
            int tag_index = dna.IndexOf("TAG");
            int tga_index = dna.IndexOf("TGA");

            if (taa_index < 0)
            {
                taa_index = 1000;
            }
            if (tag_index < 0)
            {
                tag_index = 1000;
            }
            if (tga_index < 0)
            {
                tga_index = 1000;
            }

            int[] array1 = new int[] { taa_index, tag_index, tga_index };

            int end_index = array1.Min();

            return end_index;
        }


        public static Dictionary<string, string> Read_Csv_Return_Dict()
        {
            Dictionary<string, string> Codon_Dict = new Dictionary<string, string>();
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader("CodonTable.csv"))
                {
                    string line;
                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        string[] col = line.Split(',');

                        Codon_Dict.Add(col[0], col[1]);
                    }

                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return Codon_Dict;
        }
    }
}