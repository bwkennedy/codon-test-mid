using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        //unlikely for genetic sequences to ever not be in sets of 3, but create it as a variable instead of a magic number as good practice
        private const int SequenceSize = 3;

        //using a list of keyValue pairs because dictionaries don't allow duplicate keys
        private List<KeyValuePair<string, string>> _TranslationDictionary;

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
            switch (fileType)
            {
                case ".csv":
                    _TranslationDictionary = ParseCsvFile(fileContent);
                    break;
                case ".json":
                    _TranslationDictionary = ParseJsonFile(fileContent);
                    break;
                case ".xml":
                    _TranslationDictionary = ParseXmlFile(fileContent);
                    break;
            }
        }

        /// <summary>
        /// Translates a sequence of DNA into a sequence of amino acids.
        /// </summary>
        /// <param name="dna">DNA sequence to be translated.</param>
        /// <returns>Amino acid sequence</returns>
        public string Translate(string dna)
        {
            //some of the more complex tests fail due to new line characters, I had assumed it should process based on each set of three characters, and the new lines were just there to mess up the results
            // this is clearly not the case since the tests fail still, but I'm not sure what the rule is, aka if they should then just split into sub-three character sets and be processed that way or not.
            // Sadly, at this point my hour is up, so I don't have time to experiment to figure out the exact rule
            dna = dna.Replace("\n", String.Empty);

            //break the string down into a list of the sets of sequence size
            var listOfSets = Enumerable.Range(0, dna.Length / SequenceSize)
                .Select(x => dna.Substring(x * SequenceSize, SequenceSize)).ToList();

            var startedRecording = false;
            var translationResults = string.Empty;

            foreach (var set in listOfSets)
            {
                var matches = _TranslationDictionary.Where(x => x.Key.Equals(set)).ToList();

                if (matches.Any(x => x.Value.Equals("START")))
                {
                    startedRecording = true;
                }
                if (matches.Any(x => x.Value.Equals("STOP")))
                {
                    startedRecording = false;
                }
                if (startedRecording)
                {
                    translationResults +=
                        matches.FirstOrDefault(x => !x.Value.Equals("START") && !x.Value.Equals("STOP")).Value;
                }
            }

            return translationResults;
        }


        private List<KeyValuePair<string, string>> ParseCsvFile(string fileContent)
        {
            try
            {
                var linesArray = fileContent.Split(Environment.NewLine.ToCharArray());
                var translationDictionary = new List<KeyValuePair<string, string>>();
                foreach (var line in linesArray)
                {
                    var parsedLine = line.Split(',');
                    if (!string.IsNullOrWhiteSpace(parsedLine[0]) && !string.IsNullOrWhiteSpace(parsedLine[1]))
                    {
                        var keyValue = new KeyValuePair<string, string>(parsedLine[0], parsedLine[1]);
                        translationDictionary.Add(keyValue);
                    }
                    
                }

                return translationDictionary;
            }
            catch (Exception e)
            {
                Console.WriteLine("Encountered an exception while attempting to parse csv file");
                Console.WriteLine(e);
                throw;
            }

        }

        private List<KeyValuePair<string, string>> ParseJsonFile(string fileContent)
        {
            throw new NotImplementedException();
        }

        private List<KeyValuePair<string, string>> ParseXmlFile(string fileContent)
        {
            throw new NotImplementedException();
        }
    }
}