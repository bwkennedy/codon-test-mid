using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

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
        public List<string> Starts = new List<string>();
        public List<string> Stops = new List<string>();
        public Dictionary<string, string> CodonMap = new Dictionary<string, string>();
    }

    public class CodonTranslator
    {
        CodonTable codonTable = new CodonTable();

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
            
            if (fileType == ".json")
            {
                dynamic json = JsonConvert.DeserializeObject(fileContent);
                foreach (var element in json.Starts)
                {
                    codonTable.Starts.Add(element.ToString());
                }
                foreach (var element in json.Stops)
                {
                    codonTable.Stops.Add(element.ToString());
                }
                foreach (dynamic element in json.CodonMap)
                {
                    codonTable.CodonMap.Add(element.Codon.ToString(), element.AminoAcid.ToString());
                }
            }
            else if (fileType == ".csv")
            {
                using(TextReader sr = new StringReader(fileContent))
                {
                    var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
                    {
                        HasHeaderRecord = false
                    };
                    var csvReader = new CsvReader(sr, csvConfig);
                    
                    string codon;
                    string aminoAcid;
                    
                    while (csvReader.Read())
                    {
                        csvReader.TryGetField<string>(0, out codon);
                        csvReader.TryGetField<string>(1, out aminoAcid);
                        
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
                            codonTable.CodonMap.Add(codon, aminoAcid);
                        }
                    }
                }
            }
            else if (fileType == ".xml")
            {
                using(TextReader sr = new StringReader(fileContent))
                {
                    using (XmlReader reader = XmlReader.Create(sr))
                    {
                        while (reader.Read())
                        {
                            
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "Starts":
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Starts")
                                            {
                                                break;
                                            }
                                            else if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                codonTable.Starts.Add(reader.Value);
                                            }
                                        }
                                        break;
                                    case "Stops":
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Stops")
                                            {
                                                break;
                                            }
                                            else if (reader.NodeType == XmlNodeType.Text)
                                            {
                                                codonTable.Stops.Add(reader.Value);
                                            }
                                        }
                                        break;
                                    case "CodonMap":
                                        
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "CodonMap")
                                            {
                                                break;
                                            }
                                            else if (reader.Name == "CodonPair")
                                            {
                                                string lastName = "";
                                                string codon = "";
                                                string aminoAcid = "";
                                                
                                                while (reader.Read())
                                                {
                                                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "CodonPair")
                                                    {
                                                        break;
                                                    }
                                                    else if (reader.NodeType == XmlNodeType.Text)
                                                    {
                                                        if (lastName == "Codon")
                                                        {
                                                            codon = reader.Value;
                                                        } else if (lastName == "AminoAcid")
                                                        {
                                                            aminoAcid = reader.Value;
                                                        }
                                                    }

                                                    lastName = reader.Name;
                                                }
                                                codonTable.CodonMap.Add(codon, aminoAcid);
                                            }
                                        }
                                        break;
                                }
                            }
                            
                        }
                    }
                }
            }
            else
            {
                throw new System.NotImplementedException(string.Format("The contents of the file with type \"{0}\" have been loaded, please make use of it.\n{1}",fileType,fileContent));
            }
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            string translation = "";
            
            var startIndex = -1;
            
            // find the index of the start codon
            for (int i = 0; i < dna.Length - 2; i++)
            {
                var codon = dna.Substring(i, 3);
                if (codonTable.Starts.Contains(codon))
                {
                    startIndex = i;
                    break;
                }
            }

            // build the translation and check for stop codons
            for (int i = startIndex; i < dna.Length; i += 3)
            {
                var codon = dna.Substring(i, 3);
                if (codonTable.Stops.Contains(codon))
                {
                    break;
                }
                translation += codonTable.CodonMap[codon];
            }
            
            return translation;
        }
    }
}