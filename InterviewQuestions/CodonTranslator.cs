using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Xml;
using NUnit.Framework;

namespace InterviewQuestions
{
    /*  
     *  The genetic code is a set of rules by which DNA or mRNA is translated into proteins (amino acid sequences).
     *  
     *  1) Three nucleotides (or tri-nucleotide), called codons, specify which amino acid will be used.
     *  2) Since codons are defined by three nucleotides, every sequence can be read in three reading frames, depending on the starting point.
     *     The actual reading frame used for translation is determined by a start codon. 
     *     In our case, we will define the start codon to be the most commonly used ATG (in some organisms there may be other start codons).
     *  3) Translation begins with the start codon, which is translated as Methionine (abbreviated as 'M').
     *  4) Translation continues until a stop codon is encountered. There are three stop codons (TAG, TGA, TAA)
     *  
     * 
     *  Included in this project is a comma seperated value (CSV) text file with the codon translations.
     *  Each line of the file has the codon, followed by a space, then the amino acid (or start or stop)
     *  For example, the first line:
     *  CTA,L
     *  should be interpreted as: "the codon CTA is translated to the amino acid L"
     *  
     *  
     *  You should not assume that the input sequence begins with the start codon. Any nucleotides before the start codon should be ignored.
     *  You should not assume that the input sequence ends with the stop codon. Any nucleotides after the stop codon should be ignored.
     * 
     *  For example, if the input DNA sequence is GAACAAATGCATTAATACAAAAA, the output amino acid sequence is MH.
     *  GAACAA ATG CAT TAA TACAAAAA
     *         \ / \ /
     *          M   H
     *          
     *  ATG -> START -> M
     *  CAT -> H
     *  TAA -> STOP
     *  
     */


    public class CodonTranslator
    {
        private List<string> startCodons;
        private List<string> endCodons;
        private Dictionary<string, string> codonsToTranslations;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="codonTableFileName">Filename of the DNA codon table.</param>
        public CodonTranslator(string codonTableFileName)
        {
            var file = new StreamReader(codonTableFileName);
            var fileContent = file.ReadToEnd();
            BuildTranslationMapFromFileContent(fileContent, Path.GetExtension(codonTableFileName));
        }

        private void BuildTranslationMapFromFileContent(string fileContent, string fileType)
        {
            string cleanFileContent = Regex.Replace(fileContent, @"\s+", "");

            string rawStartsData = SuperSubstring(cleanFileContent, "<Starts>", "</Starts>");
            startCodons = FilterStrings(rawStartsData);


            string rawEndsData = SuperSubstring(cleanFileContent, "<Stops>", "</Stops>");
            endCodons = FilterStrings(rawEndsData);

            string codonMap = SuperSubstring(cleanFileContent, "<CodonMap>", "</CodonMap>");
            codonsToTranslations = FilterPairs(codonMap);


            //throw new System.NotImplementedException(string.Format("The contents of the file with type \"{0}\" have been loaded, please make use of it.\n{1}",fileType,fileContent));
        }

        private string SuperSubstring(string content, string firstString, string secondString)
        {
            int pFrom = content.IndexOf(firstString) + firstString.Length;
            int pTo = content.LastIndexOf(secondString);

            string result = content.Substring(pFrom, pTo - pFrom);
            return result;
        }

        private Dictionary<string, string> FilterPairs(string input)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            string currentCodon = "";
            string currentAcid = "";
            while (input.Length > 0)
            {
                input = input.Substring(18);
                currentCodon = input.Substring(0, 3);
                input = input.Substring(22);
                currentAcid = input.Substring(0, 1);
                output.Add(currentCodon, currentAcid);
                input = input.Substring(25);

            }

            return output;
        }

        private List<string> FilterStrings(string input)
        {
            List<string> output = new List<string>();
            while (input.Length > 0)
            {
                input = input.Substring(8);
                output.Add(input.Substring(0, 3));
                input = input.Substring(12);
            }

            return output;
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            string output = "";

            int indexOfFirstStartCodon = Int32.MaxValue;
            int currentIndex = 0;
            foreach(string startCodon in startCodons)
            {
                currentIndex = dna.IndexOf(startCodon);
                if (currentIndex < indexOfFirstStartCodon)
                    indexOfFirstStartCodon = currentIndex;
            }

            dna = dna.Substring(indexOfFirstStartCodon);
            while (dna.Length > 2)
            {
                string currentCodon = dna.Substring(0, 3);
                if (endCodons.Contains((currentCodon)))
                {

                    return output;
                }
                else
                {

                    output = output + codonsToTranslations[currentCodon];
                    dna = dna.Substring(3);
                }
            }
            return output;
        }
    }
}