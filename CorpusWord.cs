using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurkishTextContradictionAnalysis
{
    public enum Attribute
    {
        NN, // Noun
        IJ, // Interjection
        IVB, // Intransitive Verb
        IVBN, // Intransitive Verb (Negative)
        IVBP, // Intransitive Verb (Positive)
        VB, // Transitive Verb
        VBN, // Transitive Verb (Negative)
        VBP, // Transitive Verb (Positive)
        RP, // Particle
        RB, // Adverb
        JJ, // Adjective
        JJN, // Adjective (Negative)
        JJP, // Adjective (Positive)
        EX, // Exclamation
        PN, // Pronoun
        DT, // Determiner
        NULL // ERROR
    }
    public class CorpusWord
    {
        private string word;
        public string Word { get { return word; } set { word = value; } }
        private Attribute attribute;
        public Attribute Attribute { get { return attribute; } set { attribute = value; } }

        public static Attribute ToAttribute(string s)
        {
            if (s.Equals("NN")) return Attribute.NN;
            if (s.Equals("IVB")) return Attribute.IVB;
            if (s.Equals("IVBN")) return Attribute.IVBN;
            if (s.Equals("IVBP")) return Attribute.IVBP;
            if (s.Equals("VB")) return Attribute.VB;
            if (s.Equals("VBN")) return Attribute.VBN;
            if (s.Equals("VBP")) return Attribute.VBP;
            if (s.Equals("RP")) return Attribute.RP;
            if (s.Equals("RB")) return Attribute.RB;
            if (s.Equals("JJ")) return Attribute.JJ;
            if (s.Equals("JJN")) return Attribute.JJN;
            if (s.Equals("JJP")) return Attribute.JJP;
            if (s.Equals("EX")) return Attribute.EX;
            if (s.Equals("PN")) return Attribute.PN;
            if (s.Equals("DT")) return Attribute.DT;
            if (s.Equals("IJ")) return Attribute.IJ;
            return Attribute.NULL;
        }
    }
}
