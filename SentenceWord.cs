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
        private CorpusWord stem = new CorpusWord();
        private bool isDropped = false;
        private bool isSoftened = false;
        private Suffix[] suffixes;
        private SentenceRole sr;

        public static SentenceWord MorphologicalParsingOfAdverbs(string word)
        {
            bool success;
            List<Suffix> s = new List<Suffix>();
            SentenceWord sw = new SentenceWord();
            string stem = word;
            while (Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null || Corpus.CorpusList.Find(w => w.Word.Equals(stem) && w.Attribute == Attribute.RB) != null && stem.Length > 0)
            {
                Console.WriteLine(stem + " " + word);
                stem = stem.Substring(0, stem.Length - 1);
            }

            if (stem.Length == 0) {
                Console.WriteLine("Unparseable string error.");
                return null;
            }


            sw.stem.Word = stem;
            sw.stem.Attribute = Corpus.CorpusList.Find(w => w.Word.Equals(stem)).Attribute;

            if (sw.stem.Attribute == Attribute.NN || sw.stem.Attribute == Attribute.JJ || /* Temporary */ sw.stem.Attribute == Attribute.NULL /* Temporary */ || sw.stem.Attribute == Attribute.JJN || sw.stem.Attribute == Attribute.JJP)
            {
                s = ParseNameSuffixes(sw.stem.Word, word, out success, false);
                sw.suffixes = s.ToArray();
            }
            for (int j = 0; j < sw.suffixes.Length; j++)
                Console.WriteLine(sw.suffixes[j].ToString());
            Console.WriteLine("Final result: " + sw.stem.Word + " " + sw.stem.Attribute);
            return sw;

        }

        public static SentenceWord MorphologicalParsing(string word)
        {
            string stem = word;
            bool success = false;
            SentenceWord sw = new SentenceWord();
            List<Suffix> s = null;
            if (Corpus.CorpusList.Find(w => w.Word.Equals(word)) == null)
            {
                stem = word;
                while ((Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null) && stem.Length > 0)
                {
                    Console.WriteLine("Word: " + stem);             
                    if(!sw.isSoftened) { 
                        if (stem[stem.Length - 1] == 'ğ') { 
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'k';
                            stem = new string(stemArray);
                            sw.isSoftened = TestForAvailability('ğ', stem, word, out s);
                            if (sw.isSoftened) break;
                        }
                        if (stem[stem.Length - 1] == 'd')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 't';
                            stem = new string(stemArray);
                            sw.isSoftened = TestForAvailability('d', stem, word, out s);
                            if (sw.isSoftened) break;
                        }
                        if (stem[stem.Length - 1] == 'b')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'p';
                            stem = new string(stemArray);
                            sw.isSoftened = TestForAvailability('b', stem, word, out s);
                            if (sw.isSoftened) break;
                        }
                        if (stem[stem.Length - 1] == 'c')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'ç';
                            stem = new string(stemArray);
                            sw.isSoftened = TestForAvailability('c', stem, word, out s);
                            if (sw.isSoftened) break;
                        }
                    }
                    stem = stem.Substring(0, stem.Length - 1);
                }


                if (Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null)
                {
                    word = FoundOrNot(word, 0);
                    if (word != null)
                    { 
                        sw.stem.Word = word;
                        sw.stem.Attribute = Corpus.CorpusList.Find(w => w.Word.Equals(sw.stem.Word)).Attribute;
                    }
                    else
                    { 
                        Console.WriteLine("Unparseable string error.");
                        return null;
                    }
                }
                else
                {
                    
                    sw.stem.Word = stem;
                    sw.stem.Attribute = Corpus.CorpusList.Find(w => w.Word.Equals(sw.stem.Word)).Attribute;
                }
            }
            else
            {
                sw.stem.Word = word;
                sw.stem.Attribute = Corpus.CorpusList.Find(w => w.Word.Equals(sw.stem.Word)).Attribute;
            }
            if (!stem.Equals(word)) {
                if (sw.stem.Attribute == Attribute.NN || sw.stem.Attribute == Attribute.JJ || /* Temporary */ sw.stem.Attribute == Attribute.NULL /* Temporary */ || sw.stem.Attribute == Attribute.JJN || sw.stem.Attribute == Attribute.JJP) {
                    if (!sw.isSoftened)
                        s = ParseNameSuffixes(sw.stem.Word, word, out success, false);
                    else
                        success = true;
                }
                if (sw.stem.Attribute == Attribute.RB)
                    return MorphologicalParsingOfAdverbs(word);


                if (!success) { 
                    Console.WriteLine("Unparseable string error.");
                    return null;
                }
                sw.suffixes = s.ToArray();
            }
            else
            {
                sw.suffixes = new Suffix[0];
            }
            for (int j = 0; j < sw.suffixes.Length; j++)
                Console.WriteLine(sw.suffixes[j].ToString());
            Console.WriteLine("Final result: " + sw.stem.Word + " " + sw.stem.Attribute);
            return sw;
        }

        private static bool TestForAvailability(char change, string stem, string word, out List<Suffix> suffixes)
        {
            suffixes = null;
            if (Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null)
            {
                return false;
            }
            bool success;
            int i = 0;
            char[] stem_array = stem.ToCharArray();
            char[] word_array = word.ToCharArray();
            while (stem_array[i] == word_array[i])
                i++;
            stem_array[i] = change;
            stem = new string(stem_array);
            
            suffixes = ParseNameSuffixes(stem, word, out success, false);
            return success;
        }

        private static bool EndsWithVowel(string word)
        {
            if (word.Length > 0 && (word[word.Length - 1] == 'a' || word[word.Length - 1] == 'e' || word[word.Length - 1] == 'ı' || word[word.Length - 1] == 'i' ||
            word[word.Length - 1] == 'o' || word[word.Length - 1] == 'ö' || word[word.Length - 1] == 'u' || word[word.Length - 1] == 'ü'))
                return true;
            else
                return false;
        }

        private static bool EndsWithConsonant(string word)
        {
            if (word[word.Length - 1] != 'a' && word[word.Length - 1] != 'e' && word[word.Length - 1] != 'ı' && word[word.Length - 1] != 'i' &&
            word[word.Length - 1] != 'o' && word[word.Length - 1] != 'ö' && word[word.Length - 1] != 'u' && word[word.Length - 1] != 'ü' && word.Length > 1)
                return true;
            else
                return false;
        }

        private static char LastVowel(string word)
        {
            char v = ' ';
            while (word[word.Length - 1] != 'a' && word[word.Length - 1] != 'e' && word[word.Length - 1] != 'ı' && word[word.Length - 1] != 'i' &&
            word[word.Length - 1] != 'o' && word[word.Length - 1] != 'ö' && word[word.Length - 1] != 'u' && word[word.Length - 1] != 'ü' && word.Length > 1)
                word = word.Substring(0, word.Length - 1);
            v = word[word.Length - 1];
            return v;
        }

        private static char LastConsonant(string word)
        {
            char v = ' ';
            while (word.Length > 0 && (word[word.Length - 1] == 'a' || word[word.Length - 1] == 'e' || word[word.Length - 1] == 'ı' || word[word.Length - 1] == 'i' ||
            word[word.Length - 1] == 'o' || word[word.Length - 1] == 'ö' || word[word.Length - 1] == 'u' || word[word.Length - 1] == 'ü'))
                word = word.Substring(0, word.Length - 1);
            if(word.Length > 0)
                v = word[word.Length - 1];
            return v;
        }

        private static bool IsRough(string word)
        {
            if (LastConsonant(word) == 'p' || LastConsonant(word) == 'ç' || LastConsonant(word) == 't' || LastConsonant(word) == 'k')
                return true;
            else
                return false;
        }

        private static List<Suffix> ParseNameSuffixes(string stem, string word, out bool success, bool foundPossessive)
        {
            Console.WriteLine(stem + " with " + word + ".");
            int iteration = 0;
            int i = stem.Length - 1;
            List<Suffix> suffixes = new List<Suffix>();
            success = true;
            while (!word.Equals(stem) && iteration < 256)
            {
                try {
                    if (word.Length > (i + 3))
                    {
                        // PLU
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("lar"))
                        {
                            suffixes.Add(new Suffix("lar", Semantics.PLU, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("ler"))
                        {
                            suffixes.Add(new Suffix("ler", Semantics.PLU, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // ABL
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' ||
                            LastVowel(stem) == 'u') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tan"))
                        {
                            suffixes.Add(new Suffix("tan", Semantics.ABL, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' ||
                            LastVowel(stem) == 'ü') && IsRough(stem) && word.Substring(i + 1, 3).Equals("ten"))
                        {
                            suffixes.Add(new Suffix("ten", Semantics.ABL, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' ||
                            LastVowel(stem) == 'u') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("dan"))
                        {
                            suffixes.Add(new Suffix("dan", Semantics.ABL, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' ||
                            LastVowel(stem) == 'ü') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("den"))
                        {
                            suffixes.Add(new Suffix("den", Semantics.ABL, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // PPF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("mız"))
                        {
                            suffixes.Add(new Suffix("mız", Semantics.PPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("miz"))
                        {
                            suffixes.Add(new Suffix("miz", Semantics.PPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("muz"))
                        {
                            suffixes.Add(new Suffix("muz", Semantics.PPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("müz"))
                        {
                            suffixes.Add(new Suffix("müz", Semantics.PPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // PPS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("nız"))
                        {
                            suffixes.Add(new Suffix("nız", Semantics.PPS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("niz"))
                        {
                            suffixes.Add(new Suffix("niz", Semantics.PPS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("nuz"))
                        {
                            suffixes.Add(new Suffix("nuz", Semantics.PPS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("nüz"))
                        {
                            suffixes.Add(new Suffix("nüz", Semantics.PPS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // ISF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("yım"))
                        {
                            suffixes.Add(new Suffix("yım", Semantics.ISF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("yim"))
                        {
                            suffixes.Add(new Suffix("yim", Semantics.ISF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("yum"))
                        {
                            suffixes.Add(new Suffix("yum", Semantics.ISF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("yüm"))
                        {
                            suffixes.Add(new Suffix("yüm", Semantics.ISF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // ISS
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("sın"))
                        {
                            suffixes.Add(new Suffix("sın", Semantics.ISS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("sin"))
                        {
                            suffixes.Add(new Suffix("sin", Semantics.ISS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("sun"))
                        {
                            suffixes.Add(new Suffix("sun", Semantics.ISS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("sün"))
                        {
                            suffixes.Add(new Suffix("sün", Semantics.ISS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // IST
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("dır"))
                        {
                            suffixes.Add(new Suffix("dır", Semantics.IST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("dir"))
                        {
                            suffixes.Add(new Suffix("dir", Semantics.IST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("dur"))
                        {
                            suffixes.Add(new Suffix("dur", Semantics.IST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("dür"))
                        {
                            suffixes.Add(new Suffix("dür", Semantics.IST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tır"))
                        {
                            suffixes.Add(new Suffix("tır", Semantics.IST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tir"))
                        {
                            suffixes.Add(new Suffix("tir", Semantics.IST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tur"))
                        {
                            suffixes.Add(new Suffix("tur", Semantics.IST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tür"))
                        {
                            suffixes.Add(new Suffix("tür", Semantics.IST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // IPF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("yız"))
                        {
                            suffixes.Add(new Suffix("yız", Semantics.IPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("yiz"))
                        {
                            suffixes.Add(new Suffix("yiz", Semantics.IPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("yuz"))
                        {
                            suffixes.Add(new Suffix("yuz", Semantics.IPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("yüz"))
                        {
                            suffixes.Add(new Suffix("yüz", Semantics.IPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // KSF
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("dım"))
                        {
                            suffixes.Add(new Suffix("dım", Semantics.KSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("dim"))
                        {
                            suffixes.Add(new Suffix("dim", Semantics.KSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("dum"))
                        {
                            suffixes.Add(new Suffix("dum", Semantics.KSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("düm"))
                        {
                            suffixes.Add(new Suffix("düm", Semantics.KSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("tım"))
                        {
                            suffixes.Add(new Suffix("tım", Semantics.KSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("tim"))
                        {
                            suffixes.Add(new Suffix("tim", Semantics.KSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("tum"))
                        {
                            suffixes.Add(new Suffix("tum", Semantics.KSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("tüm"))
                        {
                            suffixes.Add(new Suffix("tüm", Semantics.KSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // KSS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("dın"))
                        {
                            suffixes.Add(new Suffix("dın", Semantics.KSS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("din"))
                        {
                            suffixes.Add(new Suffix("din", Semantics.KSS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("dun"))
                        {
                            suffixes.Add(new Suffix("dun", Semantics.KSS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("dün"))
                        {
                            suffixes.Add(new Suffix("dün", Semantics.KSS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // KST
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("ydı"))
                        {
                            suffixes.Add(new Suffix("ydı", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("ydi"))
                        {
                            suffixes.Add(new Suffix("ydi", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("ydu"))
                        {
                            suffixes.Add(new Suffix("ydu", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("ydü"))
                        {
                            suffixes.Add(new Suffix("ydü", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // KPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("dık"))
                        {
                            suffixes.Add(new Suffix("dık", Semantics.KPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("dik"))
                        {
                            suffixes.Add(new Suffix("dik", Semantics.KPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("duk"))
                        {
                            suffixes.Add(new Suffix("duk", Semantics.KPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && !IsRough(stem) && word.Substring(i + 1, 3).Equals("dük"))
                        {
                            suffixes.Add(new Suffix("dük", Semantics.KPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tık"))
                        {
                            suffixes.Add(new Suffix("tık", Semantics.KPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tik"))
                        {
                            suffixes.Add(new Suffix("tik", Semantics.KPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tuk"))
                        {
                            suffixes.Add(new Suffix("tuk", Semantics.KPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && IsRough(stem) && word.Substring(i + 1, 3).Equals("tük"))
                        {
                            suffixes.Add(new Suffix("tük", Semantics.KPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // HST
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("mış"))
                        {
                            suffixes.Add(new Suffix("mış", Semantics.HST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("miş"))
                        {
                            suffixes.Add(new Suffix("miş", Semantics.HST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("muş"))
                        {
                            suffixes.Add(new Suffix("muş", Semantics.HST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("müş"))
                        {
                            suffixes.Add(new Suffix("müş", Semantics.HST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // CSF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("sam"))
                        {
                            suffixes.Add(new Suffix("sam", Semantics.CSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("sem"))
                        {
                            suffixes.Add(new Suffix("sem", Semantics.CSF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // CSS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("san"))
                        {
                            suffixes.Add(new Suffix("san", Semantics.CSS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("sen"))
                        {
                            suffixes.Add(new Suffix("sen", Semantics.CSS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // CPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("sak"))
                        {
                            suffixes.Add(new Suffix("sak", Semantics.CPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("sek"))
                        {
                            suffixes.Add(new Suffix("sek", Semantics.CPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // CST
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("ysa"))
                        {
                            suffixes.Add(new Suffix("ysa", Semantics.CST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("yse"))
                        {
                            suffixes.Add(new Suffix("yse", Semantics.CST, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }

                        // VIA
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("yla"))
                        {
                            suffixes.Add(new Suffix("yla", Semantics.VIA, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("yle"))
                        {
                            suffixes.Add(new Suffix("yle", Semantics.VIA, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                        }
                    }
                    if (word.Length > (i + 2))
                    {
                        if (!foundPossessive)
                        {
                            // ACC
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("yı"))
                            {
                                suffixes.Add(new Suffix("yı", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("yi"))
                            {
                                suffixes.Add(new Suffix("yi", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("yu"))
                            {
                                suffixes.Add(new Suffix("yu", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("yü"))
                            {
                                suffixes.Add(new Suffix("yü", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                        }
                        else
                        {
                            // PST
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("sı"))
                            {
                                suffixes.Add(new Suffix("sı", Semantics.PST, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("si"))
                            {
                                suffixes.Add(new Suffix("si", Semantics.PST, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("su"))
                            {
                                suffixes.Add(new Suffix("su", Semantics.PST, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("sü"))
                            {
                                suffixes.Add(new Suffix("sü", Semantics.PST, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            // ACC
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("nı"))
                            {
                                suffixes.Add(new Suffix("nı", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("ni"))
                            {
                                suffixes.Add(new Suffix("ni", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("nu"))
                            {
                                suffixes.Add(new Suffix("nu", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("nü"))
                            {
                                suffixes.Add(new Suffix("nü", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                        }

                        // LOC                      
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' ||
                            LastVowel(stem) == 'u') && IsRough(stem) && word.Substring(i + 1, 2).Equals("ta"))
                        {
                            suffixes.Add(new Suffix("ta", Semantics.LOC, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' ||
                        LastVowel(stem) == 'ü') && IsRough(stem) && word.Substring(i + 1, 2).Equals("te"))
                        {
                            suffixes.Add(new Suffix("te", Semantics.LOC, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' ||
                        LastVowel(stem) == 'u') && !IsRough(stem) && word.Substring(i + 1, 2).Equals("da"))
                        {
                            suffixes.Add(new Suffix("da", Semantics.LOC, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' ||
                            LastVowel(stem) == 'ü') && !IsRough(stem) && word.Substring(i + 1, 2).Equals("de"))
                        {
                            suffixes.Add(new Suffix("de", Semantics.LOC, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }

                        if(!foundPossessive)
                        {
                            // ISF
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ım"))
                            {
                                suffixes.Add(new Suffix("ım", Semantics.ISF, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("im"))
                            {
                                suffixes.Add(new Suffix("im", Semantics.ISF, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("um"))
                            {
                                suffixes.Add(new Suffix("um", Semantics.ISF, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("üm"))
                            {
                                suffixes.Add(new Suffix("üm", Semantics.ISF, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                        }
                        else
                        {
                            // PSF
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ım"))
                            {
                                suffixes.Add(new Suffix("ım", Semantics.PSF, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("im"))
                            {
                                suffixes.Add(new Suffix("im", Semantics.PSF, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("um"))
                            {
                                suffixes.Add(new Suffix("um", Semantics.PSF, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("üm"))
                            {
                                suffixes.Add(new Suffix("üm", Semantics.PSF, 1));
                                stem = stem + (word.Substring(i + 1, 2));
                                i = i + 2;
                            }
                        }

                        // PSS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ın"))
                        {
                            suffixes.Add(new Suffix("ın", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("in"))
                        {
                            suffixes.Add(new Suffix("in", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("un"))
                        {
                            suffixes.Add(new Suffix("un", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("ün"))
                        {
                            suffixes.Add(new Suffix("ün", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }

                        // IPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ız"))
                        {
                            suffixes.Add(new Suffix("ız", Semantics.IPF, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("iz"))
                        {
                            suffixes.Add(new Suffix("iz", Semantics.IPF, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("uz"))
                        {
                            suffixes.Add(new Suffix("uz", Semantics.IPF, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("üz"))
                        {
                            suffixes.Add(new Suffix("üz", Semantics.IPF, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }

                        // KST
                        //Console.WriteLine(stem + " and its values: " + "\n Ends with vowel: " + EndsWithVowel(stem) + "\n Ends with consonant: " + EndsWithConsonant(stem) + "\n Last vowel: " + LastVowel(stem) + "\n Last consonant: " + LastConsonant(stem));
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && !IsRough(stem) && word.Substring(i + 1, 2).Equals("dı"))
                        {
                            suffixes.Add(new Suffix("dı", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && !IsRough(stem) && word.Substring(i + 1, 2).Equals("di"))
                        {
                            suffixes.Add(new Suffix("di", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && !IsRough(stem) && word.Substring(i + 1, 2).Equals("du"))
                        {
                            suffixes.Add(new Suffix("du", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && !IsRough(stem) && word.Substring(i + 1, 2).Equals("dü"))
                        {
                            suffixes.Add(new Suffix("dü", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && IsRough(stem) && word.Substring(i + 1, 2).Equals("tı"))
                        {
                            suffixes.Add(new Suffix("tı", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && IsRough(stem) && word.Substring(i + 1, 2).Equals("ti"))
                        {
                            suffixes.Add(new Suffix("ti", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && IsRough(stem) && word.Substring(i + 1, 2).Equals("tu"))
                        {
                            suffixes.Add(new Suffix("tu", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && IsRough(stem) && word.Substring(i + 1, 2).Equals("tü"))
                        {
                            suffixes.Add(new Suffix("tü", Semantics.KST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }

                        // CST
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("sa"))
                        {
                            suffixes.Add(new Suffix("sa", Semantics.CST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("se"))
                        {
                            suffixes.Add(new Suffix("se", Semantics.CST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }

                        // VIA
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("la"))
                        {
                            suffixes.Add(new Suffix("sa", Semantics.VIA, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("le"))
                        {
                            suffixes.Add(new Suffix("se", Semantics.VIA, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }

                        // RPN
                        if (word.Substring(i + 1, 2).Equals("ki"))
                        {
                            suffixes.Add(new Suffix("ki", Semantics.RPN, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }

                        // EQU
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && !IsRough(stem) && word.Substring(i + 1, 2).Equals("ca"))
                        {
                            suffixes.Add(new Suffix("ca", Semantics.EQU, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && !IsRough(stem) && word.Substring(i + 1, 2).Equals("ce"))
                        {
                            suffixes.Add(new Suffix("ce", Semantics.EQU, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && IsRough(stem) && word.Substring(i + 1, 2).Equals("ça"))
                        {
                            suffixes.Add(new Suffix("ce", Semantics.EQU, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && IsRough(stem) && word.Substring(i + 1, 2).Equals("çe"))
                        {
                            suffixes.Add(new Suffix("ce", Semantics.EQU, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                        }
                    }

                    if (word.Length > i + 1) { 
                        if(!foundPossessive) { 
                            // ACC
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 1).Equals("ı")) { 
                                suffixes.Add(new Suffix("ı", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 1));
                                i++;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 1).Equals("i")) {
                                suffixes.Add(new Suffix("i", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 1));
                                i++;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 1).Equals("u")) {
                                suffixes.Add(new Suffix("u", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 1));
                                i++;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 1).Equals("ü")) {
                                suffixes.Add(new Suffix("ü", Semantics.ACC, 1));
                                stem = stem + (word.Substring(i + 1, 1));
                                i++;
                            }
                        }
                        else
                        { 

                            // PST
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 1).Equals("ı"))
                            {
                                suffixes.Add(new Suffix("ı", Semantics.PST, 1));
                                stem = stem + (word.Substring(i + 1, 1));
                                i++;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 1).Equals("i"))
                            {
                                suffixes.Add(new Suffix("i", Semantics.PST, 1));
                                stem = stem + (word.Substring(i + 1, 1));
                                i++;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 1).Equals("u"))
                            {
                                suffixes.Add(new Suffix("u", Semantics.PST, 1));
                                stem = stem + (word.Substring(i + 1, 1));
                                i++;
                            }
                            if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 1).Equals("ü"))
                            {
                                suffixes.Add(new Suffix("ü", Semantics.PST, 1));
                                stem = stem + (word.Substring(i + 1, 1));
                                i++;
                            }
                        }
                        // DAT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 1).Equals("a"))
                        {
                            suffixes.Add(new Suffix("a", Semantics.DAT, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 1).Equals("e"))
                        {
                            suffixes.Add(new Suffix("e", Semantics.DAT, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                        }

                        // PSF
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 1).Equals("m"))
                        {
                            suffixes.Add(new Suffix("m", Semantics.PSF, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                        }

                        // PSS
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 1).Equals("n"))
                        {
                            suffixes.Add(new Suffix("n", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                        }
                    }
                    iteration++;
                }
                catch(ArgumentOutOfRangeException)
                {

                }
            }
            if (suffixes.Count == 0)
                success = false;
            if (iteration >= 256)
                success = false;
            return suffixes;
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
                stem = new string(stem_char);
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
