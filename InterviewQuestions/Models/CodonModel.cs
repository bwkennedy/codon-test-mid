using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterviewQuestions.Models
{
    /// <summary>
    /// Class to represent the single codon,AminoAcid pair
    /// </summary>
    public class CodonModel
    {
        public string Codon { get; set; }
        public string AminoAcid { get; set; }
    }

    /// <summary>
    /// class to represent the Object to read json file
    /// </summary>
    public class CodonMapper
    {
        public List<string> Starts { get; set; }

        public List<string> Stops { get; set; }

        public List<CodonModel> CodOnMap { get; set; }
    }
}
