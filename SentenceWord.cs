using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurkishTextContradictionAnalysis
{
    enum SentenceRole
    {
        SUBJ, // Subject
        OBJ, // Object
        INDOBJ, // Indirect Object
        ADVC, // Adverbial Clause
        PRED // Predicate
    }
    public class SentenceWord
    {
        CorpusWord stem;
        bool isDropped;
        bool isSoftened;
        Suffix[] suffixes;
        SentenceRole sr;

        public SentenceWord(string arg)
        {

        }
    }
}
