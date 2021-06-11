using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

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

    public class CodonTable
    {
        public HashSet<string> Starts;
        public HashSet<string> Stops;
        // Wanted to use Dictionary here, but struggled getting it to work with json.
        public HashSet<CodonPair> CodonMap;
    }
    public class CodonPair
    {
        public string Codon;
        public string AminoAcid;
    }

    public class CodonTranslator
    {
        public CodonTable codonTable = new CodonTable();
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="codonTableFileName">Filename of the DNA codon table.</param>
        public CodonTranslator(string codonTableFileName)
        {
            // Initialize HashSets.
            codonTable.Starts = new HashSet<string>();
            codonTable.Stops = new HashSet<string>();
            codonTable.CodonMap = new HashSet<CodonPair>();
            var file = new StreamReader(codonTableFileName);
            var fileContent = file.ReadToEnd();
            BuildTranslationMapFromFileContent(fileContent, Path.GetExtension(codonTableFileName));
        }

        private void BuildTranslationMapFromFileContent(string fileContent, string fileType)
        {
            if (fileType == ".xml")
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(fileContent);
                XmlNodeList startNodes = xmlDoc.SelectNodes("Data/Starts/string");
                foreach (XmlNode childNode in startNodes)
                {
                    codonTable.Starts.Add(childNode.InnerText);
                }
                var endNodes = xmlDoc.SelectNodes("Data/Stops/string");
                foreach (XmlNode childNode in endNodes)
                {
                    codonTable.Stops.Add(childNode.InnerText);
                }
                XmlNodeList codonPairs = xmlDoc.SelectNodes("Data/CodonMap/CodonPair");
                foreach (XmlNode childNode in codonPairs)
                {
                    CodonPair codonPair = new CodonPair();
                    codonPair.Codon = childNode["Codon"].InnerText;
                    codonPair.AminoAcid = childNode["AminoAcid"].InnerText;
                    codonTable.CodonMap.Add(codonPair);
                }
            }
            else if (fileType == ".json")
            {
                codonTable = JsonConvert.DeserializeObject<CodonTable>(fileContent);
            }
            else if (fileType == ".csv")
            {
                string[] codons = fileContent.Split(
                   new[] { "\r\n", "\r", "\n" },
                   StringSplitOptions.None
               );
                foreach (string codonMap in codons)
                {
                    string[] temp = codonMap.Split(',');
                    string codon = temp[0];
                    string aminoAcid = temp[1];
                    if (aminoAcid == "START")
                    {
                        codonTable.Starts.Add(codon);
                    }
                    else if (aminoAcid == "STOP")
                    {
                        codonTable.Stops.Add(codon);
                    }
                    else
                    {
                        CodonPair codonPair = new CodonPair();
                        codonPair.Codon = codon;
                        codonPair.AminoAcid = aminoAcid;
                        codonTable.CodonMap.Add(codonPair);
                    }
                }
            }
            else
            {
                throw new System.NotImplementedException(string.Format("File of type {0} is not supported", fileType));

            }
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            StringBuilder aminoAcid = new StringBuilder();
            for (int i = 0; i < dna.Length; i++)
            {
                string codon = dna.Substring(i, 3);
                if (codonTable.Starts.Contains(codon))
                {
                    foreach (CodonPair pair in codonTable.CodonMap)
                    {
                        if (pair.Codon == codon)
                        {
                            aminoAcid.Append(pair.AminoAcid);
                            break;
                        }
                    }
                    for (int j = i + 3; j < dna.Length; j += 3)
                    {
                        codon = dna.Substring(j, 3);
                        foreach (CodonPair pair in codonTable.CodonMap)
                        {
                            if (pair.Codon == codon)
                            {
                                aminoAcid.Append(pair.AminoAcid);
                                break;
                            }
                        }
                        if (codonTable.Stops.Contains(codon))
                        {
                            return aminoAcid.ToString();
                        }

                    }
                }
            }
            return aminoAcid.ToString();
        }
    }
}
