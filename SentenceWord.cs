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
        private CorpusWord stem;
        private bool isDropped = false;
        private bool isSoftened = false;
        private Suffix[] suffixes;
        private SentenceRole sr;


        public static SentenceWord MorphologicalParsing(string word)
        {
            SentenceWord sw = new SentenceWord();
            if(Corpus.CorpusList.Find(w => w.Word.Equals(word)) == null)
            {
                string stem = word;
                while((Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null) && stem.Length > 0)
                {
                    stem = stem.Substring(0, stem.Length - 1);
                    if(stem.Length > 0) { 
                        if (stem[stem.Length - 1] == 'ğ') { 
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'k';
                            stem = stemArray.ToString();
                            sw.isSoftened = true;
                        }
                        if (stem[stem.Length - 1] == 'd')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 't';
                            stem = stemArray.ToString();
                            sw.isSoftened = true;
                        }
                        if (stem[stem.Length - 1] == 'b')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'p';
                            stem = stemArray.ToString();
                            sw.isSoftened = true;
                        }
                        if (stem[stem.Length - 1] == 'c')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'ç';
                            stem = stemArray.ToString();
                            sw.isSoftened = true;
                        }
                    }
                }

                if (Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null)
                {
                    word = FoundOrNot(word, 0);
                    if (word != null)
                        Console.WriteLine(word);
                    else
                        Console.WriteLine("Unparseable string error.");
                }
                else
                {
                    Console.WriteLine(stem);
                }
            }
            else
            {
                Console.WriteLine(word);
            }
            return null;
        }

        public static string FoundOrNot(string word, int trial_number)
        {
            if (trial_number > 3)
                return null;
            string stem = word;
            int iter = 0;
            switch(trial_number)
            {
                case 0:
                    iter = stem.IndexOf('ğ');
                    break;
                case 1:
                    iter = stem.IndexOf('d');
                    break;
                case 2:
                    iter = stem.IndexOf('b');
                    break;
                case 3:
                    iter = stem.IndexOf('c');
                    break;
            }
            char[] stem_char = stem.ToCharArray();
            if (iter != -1)
            {
                switch (trial_number)
                {
                    case 0:
                        stem_char[iter] = 'k';
                        break;
                    case 1:
                        stem_char[iter] = 't';
                        break;
                    case 2:
                        stem_char[iter] = 'p';
                        break;
                    case 3:
                        stem_char[iter] = 'ç';
                        break;
                }  
                stem = stem_char.ToString();
                while ((Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null) && stem.Length > 0)
                {
                    stem = stem.Substring(0, stem.Length - 1);
                }
                if (Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null)
                {
                    FoundOrNot(word, trial_number + 1);
                }
                else
                    return stem;
            }
            else
            {
                FoundOrNot(word, trial_number + 1);
            }
            return null;
        }
    }
}
