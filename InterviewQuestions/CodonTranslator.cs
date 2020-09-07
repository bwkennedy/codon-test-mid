using System;
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
        public CodonTranslator(string codonTableFileName)
        {
            var file = new StreamReader(codonTableFileName);
            var fileContent = file.ReadToEnd();
            string outputAmino;
            BuildTranslationMapFromFileContent(fileContent, Path.GetExtension(codonTableFileName));
            string inputDna = "";
            outputAmino = Translate(inputDna);

        }

        private void BuildTranslationMapFromFileContent(string fileContent, string fileType)
        {
            throw new System.NotImplementedException(string.Format("The contents of the file with type \"{0}\" have been loaded, please make use of it.\n{1}",fileType,fileContent));
            /* build a 2D array from the csv
             * the first column will be the codon to match to
             * the second will be the mapping
             */
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            /*each nucleotide is a string of three characters
             * read in three characters, then compare to the mapping provided in the CSV
             * if there is a match, then read the next word
             * if the next word is start then begin building the output amino acid
             * continue reading codons adding the translation to the output amino acid
             * when a stop codon is found stop reading the dna string
             */
            string outputAmino = "";
            string tempAmino;
            bool startFound = false;
            bool stopNotFound = true;

            do
            {
                int dnaIndex = 0;
                const int aminoLength = 3;
            //    CopyTo(dnaIndex, dna, tempAmino, aminoLength);

                if(startFound)
                {
                    /* concatenate outputAmino with the mapping
                     * if tempAmino matches one of the stop sequences
                     * stopNotFound = false
                     */
                }

                else
                {
                    /*if tempAmino matches start
                     * startFound = true
                     */
                }

            }
            while (stopNotFound);


            return outputAmino;
        }
    }
}