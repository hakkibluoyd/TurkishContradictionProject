using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurkishTextContradictionAnalysis
{
    class Sentence
    {
        private List<SentenceWord> sw = new List<SentenceWord>();
        private short polarity;

        public Sentence(params string[] args)
        {
            foreach(string arg in args)
            {
                sw.Add(new SentenceWord(arg));
            }
        }
    }
}
