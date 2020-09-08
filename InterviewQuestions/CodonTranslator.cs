using System;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="codonTableFileName">Filename of the DNA codon table.</param>
        private string startCodon;
        private List<string> stopCodon;
        private Dictionary<string, string> codonMap;

        public CodonTranslator(string codonTableFileName)
        {
            stopCodon = new List<string>(){};
            codonMap = new Dictionary<string, string>(){};
            var file = new StreamReader(codonTableFileName);
            var fileContent = file.ReadToEnd();
            BuildTranslationMapFromFileContent(fileContent, Path.GetExtension(codonTableFileName));
        }

        private void BuildTranslationMapFromFileContent(string fileContent, string fileType)
        {
            //Console.WriteLine(fileType);
            if(fileType == ".csv")
                        {
                // reader is no a thing. FIX THIS
                //Fix THIS
                using (StringReader reader = new StringReader(fileContent))
                    {
                        string line;
                        //Console.WriteLine(fileType);
                        while ((line = reader.ReadLine()) != null)
                        {
                            var values = line.Split(',');
                            //Console.WriteLine("HI");
                            if(values[1] == "START"){
                                startCodon = values[0];
                                //Console.WriteLine(values[0], values[1]);
                            }
                            else if(values[1] == "STOP"){
                                stopCodon.Add(values[0]);
                            }
                            else{
                                codonMap.Add(values[0], values[1]);
                            }  
                        }
                    }
                    // while (!reader.EndOfStream)
                    // {
                    //     var values = line.Split(',');
                    //     if(values[1] == "START"){
                    //         startCodon = values[0];
                    //     }
                    //     else if(values[1] == "STOP"){
                    //         stopCodon.Add(values[0]);
                    //     }
                    //     else{
                    //         codonMap.Add(values[0], values[1]);
                    //     }                        
                    // }
                }
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            string aminoSequence = "";
            bool inSequence = false;
            int step = 1;
            int i = 0;
            dna = dna.Replace("\n", "");
            //Console.WriteLine(dna);
            while(i < dna.Length-2){
                //Console.WriteLine(dna.Substring(i,3));
                if(dna.Substring(i,3) == startCodon){
                    inSequence = true;
                    step = 3;
                }
                if(inSequence && stopCodon.Contains(dna.Substring(i, 3))){
                    //Console.WriteLine("Broke");
                    break;
                }
                if(inSequence)
                {
                    //if (i == 34)
                    //{
                    //    Console.WriteLine(dna.Substring(i, 3));
                    //    Console.WriteLine(dna.Substring(i, 10));
                    //    Console.WriteLine(codonMap[dna.Substring(i, 3)]);
                    //}

                    aminoSequence += codonMap[dna.Substring(i, 3)];
                }
                //Console.WriteLine(i);
                i += step;
            }
            //Console.WriteLine("How did I get here");

            //Console.WriteLine(aminoSequence);
            return aminoSequence;
        }
    }
}