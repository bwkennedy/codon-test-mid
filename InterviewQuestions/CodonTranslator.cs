using InterviewQuestions.Models;
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
        /// <summary>
        /// Private member to store the whole deserialized json data.
        /// </summary>
        private CodonMapper codeMappedData;

        private static int CODE_FOR_AA_LENGTH = 3;
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
            //I have made the implementation for the json file here.
            switch (fileType)
            {
                case ".json":
                    codeMappedData = Newtonsoft.Json.JsonConvert.DeserializeObject<CodonMapper>(fileContent);
                    break;
                //implementation for xml file 
                case ".xml":
                    break;
                case ".csv":
                    break;
                default:
                    throw new System.NotImplementedException(string.Format("The contents of the file with type \"{0}\" have been loaded, please make use of it.\n{1}", fileType, fileContent));

            }
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            if (dna.Length < 3)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            string subStr = string.Empty;
            for (int i = 0; i < dna.Length - 2; i++)
            {
                subStr = dna.Substring(i, CODE_FOR_AA_LENGTH);
                if (codeMappedData.Starts.FirstOrDefault().Equals(subStr))
                {
                    sb.Append('M');
                    i += 2;
                    continue;
                }
                else
                {
                    if (!sb.ToString().Contains('M'))
                    {
                        continue;
                    }
                    if (CheckStop(subStr))
                    {
                        break;
                    }
                    var foundCodon = codeMappedData.CodOnMap.FirstOrDefault(x => x.Codon.Equals(subStr));
                    if (foundCodon != null)
                    {
                        sb.Append(foundCodon.AminoAcid);
                        i += 2;
                    }
                }
            }


            return string.Empty;
        }

        private bool CheckStart(string substr)
        {
            return codeMappedData.Starts.Contains(substr);
        }

        private bool CheckStop(string substr)
        {
            return codeMappedData.Stops.Contains(substr);
        }
    }
}