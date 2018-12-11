using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurkishTextContradictionAnalysis
{
    public enum Semantics
    {
        SFP, // -m, -ım, -im Singular First Pronoun
        SSP, // -n, -sın, -sin Singular Second Pronoun
        STP, // -ı, -i Singular Third Pronoun
        PFP, // Plural First Pronoun
        PSP, // Plural Second Pronoun
        PTP, // Plural Third Pronoun
        FTR, // Future Tense
        PLU, // Plural Suffix
        IMPSS, // Imperative Singular Second
        IMPST, // Imperative Singular Third
        IMPPS, // Imperative Plural Second
        IMPPT, // Imperative Plural Third
        NEG, // Negation
        NEGPST // Negation of Present Singular Third Pronoun
    }

    public class Suffix
    {
        private string syntax;
        private Semantics semantics;
        private short polarity;
    }
}