using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        List<KeyValuePair<string, string>> codonMap = new List<KeyValuePair<string, string>>();
        List<KeyValuePair<string, string>> startSequence = new List<KeyValuePair<string, string>>();
        List<KeyValuePair<string, string>> endSequence = new List<KeyValuePair<string, string>>();

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
            StringReader strReader = new StringReader(fileContent);
            string line;
            if (fileType.Equals(".csv"))
            {
                while ((line = strReader.ReadLine()) != null)
                {
                    string[] splitString = line.Split(',');
                    if (splitString[1].Equals("START"))
                    {
                        startSequence.Add(new KeyValuePair<string, string>(splitString[0], splitString[1]));
                    }
                    else if (splitString[1].Equals("STOP"))
                    {
                        endSequence.Add(new KeyValuePair<string, string>(splitString[0], splitString[1]));
                    }
                    else
                    {
                        codonMap.Add(new KeyValuePair<string, string>(splitString[0], splitString[1]));
                    }
                }
            }
            else if (fileType.Equals(".json"))
            {

            }
            else if (fileType.Equals(".xml"))
            {

            }
            //throw new System.NotImplementedException(string.Format("The contents of the file with type \"{0}\" have been loaded, please make use of it.\n{1}",fileType,fileContent));
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            StringBuilder aaSequence = new StringBuilder();
            int startIndex =-1;

            //find start index
            for (int i = 0; i < dna.Length-3; i++)
            {
                string subThree = dna.Substring(i, 3);

                //check if the three chars are a match in startSequence list<keyvaluepair>
                KeyValuePair<string, string> startValue = startSequence.Where(kvp => kvp.Key.Equals(subThree)).FirstOrDefault();
                if (startValue.Key != null)
                {
                    startIndex = i;
                    aaSequence.Append("M");
                    break;
                }
            }

            //if start index found then process
            if(startIndex != -1)
            {
                //process string in chunks of 3 starting after start
                for (int i = startIndex+3; i < dna.Length - 3; i+=3 )
                {
                    string subThree = dna.Substring(i, 3);
                    //check if the three chars are a match in endSequence 
                    KeyValuePair<string, string> endValue =  endSequence.Where(kvp => kvp.Key.Equals(subThree)).FirstOrDefault();
                    if (endValue.Key != null)
                    {
                        break;
                    }
                    else
                    {
                        KeyValuePair<string, string> aaValue = codonMap.Where(kvp => kvp.Key.Equals(subThree)).First();
                        aaSequence.Append(aaValue.Value);
                    }
                }
            }

            return aaSequence.ToString();
        }
    }
}