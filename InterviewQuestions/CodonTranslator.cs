using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
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


    public class CodonTranslator
    {
        private Dictionary<string, string> codonMap;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="codonTableFileName">Filename of the DNA codon table.</param>
        public CodonTranslator(string codonTableFileName)
        {
            codonMap = new Dictionary<string, string>();
            var file = new StreamReader(codonTableFileName);
            var fileContent = file.ReadToEnd();
            BuildTranslationMapFromFileContent(fileContent, Path.GetExtension(codonTableFileName));
        }

        private void BuildTranslationMapFromFileContent(string fileContent, string fileType)
        {
            switch (fileType)
            {
                case ".xml":
                    BuildMapFromXml(fileContent);
                    break;
                case ".json":
                    BuildMapFromJson(fileContent);
                    break;
                case ".csv":
                    BuildMapFromCsv(fileContent);
                    break;
                default: throw new System.NotImplementedException(string.Format("The contents of the file with type \"{0}\" have been loaded, please make use of it.\n{1}", fileType, fileContent));
            }
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            StringBuilder sb = new StringBuilder();

            char[] dnaChars = dna.ToCharArray();

            var pointer = 0;
            bool isStartFound = false;

            while (pointer < dnaChars.Length)
            {
                var nucleotidesSb = new StringBuilder();
                int i = 0;

                while (i < 3)
                {
                    if (pointer < dnaChars.Length)
                    {
                        nucleotidesSb.Append(dnaChars[pointer++]);
                    }

                    i++;
                }

                var nucleotides = nucleotidesSb.ToString();

                if (codonMap.ContainsKey(nucleotides))
                {
                    var codon = codonMap[nucleotides];

                    if (codon.Contains("#"))
                    {
                        var codonValArr = codon.Split('#');
                        if (codonValArr[1].Equals("START", StringComparison.InvariantCultureIgnoreCase))
                        {
                            isStartFound = true;
                            sb.Append(codonValArr[0]);
                        }
                    }
                    else if (codon.Equals("STOP", StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }
                    else if (isStartFound)
                    {
                        sb.Append(codon);
                    }
                }
            }

            return sb.ToString();
        }

        #region Helpers

        public class Codon
        {
            public List<string> Starts { get; set; }
            public List<string> Stops { get; set; }

            public List<CodonMap> CodonMap { get; set; }
        }
        public class CodonMap
        {
            public string Codon { get; set; }
            public string AminoAcid { get; set; }

        }

        private void BuildMapFromXml(string fileContent)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(fileContent);

            var codonsXpath = "Data/CodonMap/CodonPair";
            var codonNodes = xmlDoc.SelectNodes(codonsXpath);

            foreach (XmlNode codonNode in codonNodes)
            {
                var codon = codonNode.FirstChild.FirstChild.Value;
                var aminoAcid = codonNode.LastChild.LastChild.Value;

                codonMap.Add(codon, aminoAcid);
            }

            var startsXpath = "Data/Starts";
            var startsNodes = xmlDoc.SelectNodes(startsXpath);
            foreach (XmlNode startNode in startsNodes)
            {
                var codon = startNode.FirstChild.FirstChild.Value;
                if (codonMap.ContainsKey(codon))
                {
                    var codonVal = codonMap[codon];
                    var valueToReplace = codonVal + "#START";
                    codonMap[codon] = valueToReplace;
                }
            }

            var stopsXpath = "Data/Stops";
            var stopsNodes = xmlDoc.SelectNodes(stopsXpath);
            foreach (XmlNode stopNode in stopsNodes)
            {
                var codon = stopNode.FirstChild.FirstChild.Value;
                codonMap.Add(codon, "STOP");
            }
        }

        private void BuildMapFromJson(string fileContent)
        {
            var cObj = JsonConvert.DeserializeObject<Codon>(fileContent);

            foreach (var codon in cObj.CodonMap)
            {
                codonMap.Add(codon.Codon, codon.AminoAcid);
            }

            foreach (var codon in cObj.Stops)
            {
                codonMap.Add(codon, "STOP");
            }

            foreach (var codon in cObj.Starts)
            {
                if (codonMap.ContainsKey(codon))
                {
                    var codonVal = codonMap[codon];
                    var valueToReplace = codonVal + "#START";
                    codonMap[codon] = valueToReplace;
                }
            }
        }

        private void BuildMapFromCsv(string fileContent)
        {
            string[] file = fileContent.Split(new[] { "\n" }, StringSplitOptions.None);

            foreach (var line in file)
            {
                string[] current = line.Split(',');

                if (codonMap.ContainsKey(current[0]))
                {
                    var currentVal = codonMap[current[0]];
                    var valueToReplace = currentVal + "#" + current[1];
                    codonMap[current[0]] = valueToReplace;
                }
                else
                {
                    codonMap.Add(current[0], current[1]);
                }
            }
        }
        #endregion
    }
}