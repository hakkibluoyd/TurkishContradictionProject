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
        public CorpusWord Stem { get { return stem; } set { stem = value; } }
        private bool isDropped = false;
        private bool isSoftened = false;
        private bool isImperative = false;
        private Suffix[] suffixes;
        public Suffix[] Suffixes { get { return suffixes; } set { suffixes = value; } }
        private SentenceRole sr;


        public static SentenceWord MorphologicalParsingOfAdverbs(string word, string originalWord, bool report)
        {
            

            bool success;
            List<Suffix> s = new List<Suffix>();
            SentenceWord sw = new SentenceWord();
            string stem = word;
            while (Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null || Corpus.CorpusList.Find(w => w.Word.Equals(stem) && w.Attribute == Attribute.RB) != null && stem.Length > 0)
            {
                stem = stem.Substring(0, stem.Length - 1);
            }

            if (stem.Length == 0) {
                if (report) Console.WriteLine("Unparseable string error.");
                return null;
            }


            sw.stem.Word = stem;
            sw.stem.Attribute = Corpus.CorpusList.Find(w => w.Word.Equals(stem)).Attribute;

            if (sw.stem.Attribute == Attribute.NN || (sw.stem.Attribute == Attribute.PN))
            {
                s = ParseNameSuffixes(sw.stem.Word, originalWord, out success, false, new List<Suffix>());
                sw.suffixes = s.ToArray();
            }
            if(sw.stem.Attribute == Attribute.JJ || sw.stem.Attribute == Attribute.JJP || sw.stem.Attribute == Attribute.JJN)
            {
                s = ParseNameSuffixes(sw.stem.Word, originalWord, out success, true, new List<Suffix>());
                sw.suffixes = s.ToArray();
            }
            if (sw.stem.Attribute == Attribute.VB || sw.stem.Attribute == Attribute.VBN || sw.stem.Attribute == Attribute.VBP ||
            sw.stem.Attribute == Attribute.IVB || sw.stem.Attribute == Attribute.IVBN || sw.stem.Attribute == Attribute.IVBP)
            {
                s = ParseVerbSuffixes(sw.stem.Word, originalWord, out success);
                sw.suffixes = s.ToArray();
            }
            for (int j = 0; j < sw.suffixes.Length; j++)
                if (report) Console.WriteLine(sw.suffixes[j].ToString());
            if (report) Console.WriteLine("Final result: " + sw.stem.Word + " " + sw.stem.Attribute);
            return sw;

        }

        public static SentenceWord MorphologicalParsing(string word, string originalWord, bool report)
        {
            if (word.Length == 0 || word == null)
            {
                if (report) Console.WriteLine("Unparseable string error.");
                return null;
            }
            bool pv = false;
            string stem = word;
            bool success = false;
            SentenceWord sw = new SentenceWord();
            List<Suffix> s = null;
            if (Corpus.CorpusList.Find(w => w.Word.Equals(word)) == null)
            {
                stem = word;
                while ((Corpus.CorpusList.Find(w => w.Word.Equals(stem)) == null) && stem.Length > 0)
                {      
                    if(!sw.isSoftened) { 
                        if (stem[stem.Length - 1] == 'ğ') { 
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'k';
                            stem = new string(stemArray);
                            sw.isSoftened = TestForAvailability('ğ', stem, originalWord, out pv, out s);
                            if (sw.isSoftened) break;
                        }
                        if (stem[stem.Length - 1] == 'd')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 't';
                            stem = new string(stemArray);
                            sw.isSoftened = TestForAvailability('d', stem, originalWord, out pv, out s);
                            if (sw.isSoftened) break;
                        }
                        if (stem[stem.Length - 1] == 'b')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'p';
                            stem = new string(stemArray);
                            sw.isSoftened = TestForAvailability('b', stem, originalWord, out pv, out s);
                            if (sw.isSoftened) break;
                        }
                        if (stem[stem.Length - 1] == 'c')
                        {
                            char[] stemArray = stem.ToCharArray();
                            stemArray[stem.Length - 1] = 'ç';
                            stem = new string(stemArray);
                            sw.isSoftened = TestForAvailability('c', stem, originalWord, out pv, out s);
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
                        if (report) Console.WriteLine("Unparseable string error.");
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
            if (!stem.Equals(word) || !originalWord.Equals(word)) {
                if (sw.stem.Attribute == Attribute.NN || (sw.stem.Attribute == Attribute.PN || sw.stem.Attribute == Attribute.RP || sw.stem.Attribute == Attribute.JJ || sw.stem.Attribute == Attribute.JJN || sw.stem.Attribute == Attribute.JJP)) {
                    if(pv)
                    {
                        try { 
                            sw.stem.Attribute = Corpus.CorpusList.Find(w => w.Word.Equals(sw.stem.Word) && (w.Attribute == Attribute.VB || w.Attribute == Attribute.IVB || w.Attribute == Attribute.IVBN || w.Attribute == Attribute.IVBP || w.Attribute == Attribute.VBN || w.Attribute == Attribute.VBP)).Attribute;
                        } catch
                        {
                            return null;
                        }
                    }
                    if (!sw.isSoftened) {
                        if (sw.stem.Attribute == Attribute.JJN || sw.stem.Attribute == Attribute.JJP || sw.stem.Attribute == Attribute.JJ)
                        {
                            s = ParseNameSuffixes(sw.stem.Word, originalWord, out success, true, new List<Suffix>());
                        }
                        else
                        {
                            s = ParseNameSuffixes(sw.stem.Word, originalWord, out success, false, new List<Suffix>());
                        }
                    }
                    else
                        success = true;
                }

                if (sw.stem.Attribute == Attribute.VB || sw.stem.Attribute == Attribute.VBN || sw.stem.Attribute == Attribute.VBP ||
                sw.stem.Attribute == Attribute.IVB || sw.stem.Attribute == Attribute.IVBN || sw.stem.Attribute == Attribute.IVBP) {
                    if (!sw.isSoftened)
                        s = ParseVerbSuffixes(sw.stem.Word, originalWord, out success);
                    else
                        success = true;
                }
                if (sw.stem.Attribute == Attribute.RB)
                    return MorphologicalParsingOfAdverbs(word, originalWord, report);
                if (!success) {
                    CorpusWord c = Corpus.CorpusList.Find(w => w.Word.Equals(word) && (w.Attribute == Attribute.VB || w.Attribute == Attribute.IVB || w.Attribute == Attribute.IVBN || w.Attribute == Attribute.IVBP || w.Attribute == Attribute.VBN || w.Attribute == Attribute.VBP));
                    if ((sw.stem.Attribute == Attribute.NN || sw.stem.Attribute == Attribute.JJ || sw.stem.Attribute == Attribute.JJN || sw.stem.Attribute == Attribute.JJP) && c != null)
                    {
                        sw.stem.Attribute = c.Attribute;
                        s = ParseVerbSuffixes(sw.stem.Word, originalWord, out success);
                    }
                    else
                    {
                        return MorphologicalParsing(word.Substring(0, word.Length - 1), originalWord, report);
                    } 
                }
                sw.suffixes = s.ToArray();
            }
            else
            {
                sw.suffixes = new Suffix[0];
            }
            if (sw.suffixes.Length == 0 && (sw.stem.Attribute == Attribute.VB || sw.stem.Attribute == Attribute.VBN || sw.stem.Attribute == Attribute.VBP ||
                sw.stem.Attribute == Attribute.IVB || sw.stem.Attribute == Attribute.IVBN || sw.stem.Attribute == Attribute.IVBP)) {
                sw.isImperative = true;
                if (report) Console.WriteLine("Direct Imperative Verb");
            }
                for (int j = 0; j < sw.suffixes.Length; j++)
                    if (report) Console.WriteLine(sw.suffixes[j].ToString());
            if (report) Console.WriteLine("Final result: " + sw.stem.Word + " " + sw.stem.Attribute);
            return sw;
        }

        private static bool TestForAvailability(char change, string stem, string word, out bool parsedVerb, out List<Suffix> suffixes)
        {
            parsedVerb = false;
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
            suffixes = ParseNameSuffixes(stem, word, out success, false, new List<Suffix>());
            if(!success)
            {
                parsedVerb = true;
                suffixes = ParseVerbSuffixes(stem, word, out success);
            }
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
            if (!EndsWithConsonant(word))
                return false;
            if (LastConsonant(word) == 'p' || LastConsonant(word) == 'ç' || LastConsonant(word) == 't' || LastConsonant(word) == 'k' || LastConsonant(word) == 'f' || LastConsonant(word) == 'h' || LastConsonant(word) == 's' || LastConsonant(word) == 'ş')
                return true;
            else
                return false;
        }
        private static List<Suffix> ParseVerbSuffixes(string stem, string word, out bool success)
        {
            List<Suffix> suffixes = new List<Suffix>();
            int iteration = 0;
            bool foundNeg = false;
            int i = stem.Length - 1;
            success = true;
            while (!word.Equals(stem) && iteration < 64)
            {
                try
                {
                    if (word.Length > (i + 10))
                    {
                        // FPS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 10).Equals("yacaksınız"))
                        {
                            suffixes.Add(new Suffix("yacaksınız", Semantics.FPS, 1));
                            stem = stem + word.Substring(i + 1, 10);
                            i = i + 10;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 10).Equals("yeceksiniz"))
                        {
                            suffixes.Add(new Suffix("yeceksiniz", Semantics.FPS, 1));
                            stem = stem + word.Substring(i + 1, 10);
                            i = i + 10;
                        }
                        // NGPS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 10).Equals("yamazsınız"))
                        {
                            suffixes.Add(new Suffix("yamazsınız", Semantics.NGPS, -1));
                            stem = stem + word.Substring(i + 1, 10);
                            i = i + 10;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 10).Equals("yemezsiniz"))
                        {
                            suffixes.Add(new Suffix("yemezsiniz", Semantics.NGPS, -1));
                            stem = stem + word.Substring(i + 1, 10);
                            i = i + 10;
                        }
                    }
                    if (word.Length > (i + 9))
                    {
                        // COPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 9).Equals("ıyorsunuz"))
                        {
                            suffixes.Add(new Suffix("ıyorsunuz", Semantics.COPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 9).Equals("iyorsunuz"))
                        {
                            suffixes.Add(new Suffix("iyorsunuz", Semantics.COPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 9).Equals("uyorsunuz"))
                        {
                            suffixes.Add(new Suffix("uyorsunuz", Semantics.COPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 9).Equals("üyorsunuz"))
                        {
                            suffixes.Add(new Suffix("üyorsunuz", Semantics.COPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        // FPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 9).Equals("acaksınız"))
                        {
                            suffixes.Add(new Suffix("acaksınız", Semantics.FPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 9).Equals("eceksiniz"))
                        {
                            suffixes.Add(new Suffix("eceksiniz", Semantics.FPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        // NGPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 9).Equals("amazsınız"))
                        {
                            suffixes.Add(new Suffix("amazsınız", Semantics.NGPS, -1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 9).Equals("emezsiniz"))
                        {
                            suffixes.Add(new Suffix("emezsiniz", Semantics.NGPS, -1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        // NEPS
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 9).Equals("malısınız"))
                        {
                            suffixes.Add(new Suffix("malısınız", Semantics.NEPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 9).Equals("melisiniz"))
                        {
                            suffixes.Add(new Suffix("melisiniz", Semantics.NEPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                    }
                    if (word.Length > (i + 8))
                    {
                        // HPSV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 8).Equals("mışsınız"))
                        {
                            suffixes.Add(new Suffix("mışsınız", Semantics.HPSV, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 8).Equals("mişsiniz"))
                        {
                            suffixes.Add(new Suffix("mişsiniz", Semantics.HPSV, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 8).Equals("muşsunuz"))
                        {
                            suffixes.Add(new Suffix("muşsunuz", Semantics.HPSV, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 8).Equals("müşsünüz"))
                        {
                            suffixes.Add(new Suffix("müşsünüz", Semantics.HPSV, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        // COPS
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 8).Equals("yorsunuz"))
                        {
                            suffixes.Add(new Suffix("yorsunuz", Semantics.COPS, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        // FSS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 8).Equals("yacaksın"))
                        {
                            suffixes.Add(new Suffix("yacaksın", Semantics.FSS, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 8).Equals("yeceksin"))
                        {
                            suffixes.Add(new Suffix("yeceksin", Semantics.FSS, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        // FPT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 8).Equals("yacaklar"))
                        {
                            suffixes.Add(new Suffix("yacaklar", Semantics.FPT, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 8).Equals("yecekler"))
                        {
                            suffixes.Add(new Suffix("yecekler", Semantics.FPT, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        // NGVS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 8).Equals("yamazsın"))
                        {
                            suffixes.Add(new Suffix("yamazsın", Semantics.NGVS, -1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 8).Equals("yemezsin"))
                        {
                            suffixes.Add(new Suffix("yemezsin", Semantics.NGVS, -1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        // NGPT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 8).Equals("yamazlar"))
                        {
                            suffixes.Add(new Suffix("yamazlar", Semantics.NGPT, -1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 8).Equals("yemezler"))
                        {
                            suffixes.Add(new Suffix("yemezler", Semantics.NGPT, -1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        // NPPS
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 8).Equals("mazsınız"))
                        {
                            suffixes.Add(new Suffix("mazsınız", Semantics.NPPS, -1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 8).Equals("mezsiniz"))
                        {
                            suffixes.Add(new Suffix("mezsiniz", Semantics.NPPS, -1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                    }
                    if (word.Length > (i + 7))
                    {
                        // COSS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 7).Equals("ıyorsun"))
                        {
                            suffixes.Add(new Suffix("ıyorsun", Semantics.COSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 7).Equals("iyorsun"))
                        {
                            suffixes.Add(new Suffix("iyorsun", Semantics.COSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("uyorsun"))
                        {
                            suffixes.Add(new Suffix("uyorsun", Semantics.COSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("üyorsun"))
                        {
                            suffixes.Add(new Suffix("üyorsun", Semantics.COSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // COPT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 7).Equals("ıyorlar"))
                        {
                            suffixes.Add(new Suffix("ıyorlar", Semantics.COPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 7).Equals("iyorlar"))
                        {
                            suffixes.Add(new Suffix("iyorlar", Semantics.COPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("uyorlar"))
                        {
                            suffixes.Add(new Suffix("uyorlar", Semantics.COPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("üyorlar"))
                        {
                            suffixes.Add(new Suffix("üyorlar", Semantics.COPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // FSF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("yacağım"))
                        {
                            suffixes.Add(new Suffix("yacağım", Semantics.FSF, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("yeceğim"))
                        {
                            suffixes.Add(new Suffix("yeceğim", Semantics.FSF, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // FSS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("acaksın"))
                        {
                            suffixes.Add(new Suffix("acaksın", Semantics.FSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("eceksin"))
                        {
                            suffixes.Add(new Suffix("eceksin", Semantics.FSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // FPF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("yacağız"))
                        {
                            suffixes.Add(new Suffix("yacağız", Semantics.FPF, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("yeceğiz"))
                        {
                            suffixes.Add(new Suffix("yeceğiz", Semantics.FPF, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // FPT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("acaklar"))
                        {
                            suffixes.Add(new Suffix("acaklar", Semantics.FPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("ecekler"))
                        {
                            suffixes.Add(new Suffix("ecekler", Semantics.FPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // PRPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 7).Equals("arsınız"))
                        {
                            suffixes.Add(new Suffix("arsın", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 7).Equals("ersiniz"))
                        {
                            suffixes.Add(new Suffix("ersin", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 7).Equals("ırsınız"))
                        {
                            suffixes.Add(new Suffix("ırsın", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 7).Equals("irsiniz"))
                        {
                            suffixes.Add(new Suffix("irsin", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("ursunuz"))
                        {
                            suffixes.Add(new Suffix("ursun", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("ürsünüz"))
                        {
                            suffixes.Add(new Suffix("ürsün", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // NGVS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("amazsın"))
                        {
                            suffixes.Add(new Suffix("amazsın", Semantics.NGVS, -1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("emezsin"))
                        {
                            suffixes.Add(new Suffix("emezsin", Semantics.NGVS, -1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // NGPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("yamayız"))
                        {
                            suffixes.Add(new Suffix("yamayız", Semantics.NGPF, -1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("yemeyiz"))
                        {
                            suffixes.Add(new Suffix("yemeyiz", Semantics.NGPF, -1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // NGPT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("amazlar"))
                        {
                            suffixes.Add(new Suffix("amazlar", Semantics.NGPT, -1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("emezler"))
                        {
                            suffixes.Add(new Suffix("emezler", Semantics.NGPT, -1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // REPS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("yasınız"))
                        {
                            suffixes.Add(new Suffix("yasınız", Semantics.REPS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("yesiniz"))
                        {
                            suffixes.Add(new Suffix("yesiniz", Semantics.REPS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // NESF
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("malıyım"))
                        {
                            suffixes.Add(new Suffix("malıyım", Semantics.NESF, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("meliyim"))
                        {
                            suffixes.Add(new Suffix("meliyim", Semantics.NESF, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // NESS
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("malısın"))
                        {
                            suffixes.Add(new Suffix("malısın", Semantics.NESS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("melisin"))
                        {
                            suffixes.Add(new Suffix("melisin", Semantics.NESS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // NEPF
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("malıyız"))
                        {
                            suffixes.Add(new Suffix("malıyız", Semantics.NEPF, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("meliyiz"))
                        {
                            suffixes.Add(new Suffix("meliyiz", Semantics.NEPF, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // NEPT
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("malılar"))
                        {
                            suffixes.Add(new Suffix("malılar", Semantics.NEPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("meliler"))
                        {
                            suffixes.Add(new Suffix("meliler", Semantics.NEPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                    }
                    if (word.Length > (i + 6))
                    {
                        // HSSV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("mışsın"))
                        {
                            suffixes.Add(new Suffix("mışsın", Semantics.HSSV, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("mişsin"))
                        {
                            suffixes.Add(new Suffix("mişsin", Semantics.HSSV, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("muşsun"))
                        {
                            suffixes.Add(new Suffix("muşsun", Semantics.HSSV, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("müşsün"))
                        {
                            suffixes.Add(new Suffix("müşsün", Semantics.HSSV, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // HPTV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("mışlar"))
                        {
                            suffixes.Add(new Suffix("mışlar", Semantics.HPTV, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("mişler"))
                        {
                            suffixes.Add(new Suffix("mişler", Semantics.HPTV, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("muşlar"))
                        {
                            suffixes.Add(new Suffix("muşlar", Semantics.HPTV, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("müşler"))
                        {
                            suffixes.Add(new Suffix("müşler", Semantics.HPTV, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // COSF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("ıyorum"))
                        {
                            suffixes.Add(new Suffix("ıyorum", Semantics.COSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("iyorum"))
                        {
                            suffixes.Add(new Suffix("iyorum", Semantics.COSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("uyorum"))
                        {
                            suffixes.Add(new Suffix("uyorum", Semantics.COSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("üyorum"))
                        {
                            suffixes.Add(new Suffix("üyorum", Semantics.COSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // COSS
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 6).Equals("yorsun"))
                        {
                            suffixes.Add(new Suffix("yorsun", Semantics.COSS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // COPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("ıyoruz"))
                        {
                            suffixes.Add(new Suffix("ıyoruz", Semantics.COPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("iyoruz"))
                        {
                            suffixes.Add(new Suffix("iyoruz", Semantics.COPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("uyoruz"))
                        {
                            suffixes.Add(new Suffix("uyoruz", Semantics.COPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("üyoruz"))
                        {
                            suffixes.Add(new Suffix("üyoruz", Semantics.COPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // COPT
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 6).Equals("yorlar"))
                        {
                            suffixes.Add(new Suffix("yorlar", Semantics.COPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // FSF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("acağım"))
                        {
                            suffixes.Add(new Suffix("acağım", Semantics.FSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("eceğim"))
                        {
                            suffixes.Add(new Suffix("eceğim", Semantics.FSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // FPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("acağız"))
                        {
                            suffixes.Add(new Suffix("acağız", Semantics.FPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("eceğiz"))
                        {
                            suffixes.Add(new Suffix("eceğiz", Semantics.FPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // PRPS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("rsınız"))
                        {
                            suffixes.Add(new Suffix("rsınız", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("rsiniz"))
                        {
                            suffixes.Add(new Suffix("rsiniz", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("rsunuz"))
                        {
                            suffixes.Add(new Suffix("rsunuz", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("rsünüz"))
                        {
                            suffixes.Add(new Suffix("rsünüz", Semantics.PRPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // NGPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("amayız"))
                        {
                            suffixes.Add(new Suffix("amayız", Semantics.NGPF, -1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("emeyiz"))
                        {
                            suffixes.Add(new Suffix("emeyiz", Semantics.NGPF, -1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // NPSS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("mazsın"))
                        {
                            suffixes.Add(new Suffix("mazsın", Semantics.NPSS, -1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("mezsin"))
                        {
                            suffixes.Add(new Suffix("mezsin", Semantics.NPSS, -1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // NPPT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("mazlar"))
                        {
                            suffixes.Add(new Suffix("mazlar", Semantics.NPPT, -1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("mezler"))
                        {
                            suffixes.Add(new Suffix("mezler", Semantics.NPPT, -1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // REPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("asınız"))
                        {
                            suffixes.Add(new Suffix("asınız", Semantics.REPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("esiniz"))
                        {
                            suffixes.Add(new Suffix("esiniz", Semantics.REPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // MPPT
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("sınlar"))
                        {
                            suffixes.Add(new Suffix("sınlar", Semantics.MPPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("sinler"))
                        {
                            suffixes.Add(new Suffix("sinler", Semantics.MPPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("sunlar"))
                        {
                            suffixes.Add(new Suffix("sunlar", Semantics.MPPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("sünler"))
                        {
                            suffixes.Add(new Suffix("sünler", Semantics.MPPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                    }
                    if (word.Length > (i + 5))
                    {
                        // KPSV
                        if (!IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("dınız"))
                        {
                            suffixes.Add(new Suffix("dınız", Semantics.KPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("diniz"))
                        {
                            suffixes.Add(new Suffix("diniz", Semantics.KPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("dunuz"))
                        {
                            suffixes.Add(new Suffix("dunuz", Semantics.KPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("dünüz"))
                        {
                            suffixes.Add(new Suffix("dünüz", Semantics.KPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("tınız"))
                        {
                            suffixes.Add(new Suffix("tınız", Semantics.KPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("tiniz"))
                        {
                            suffixes.Add(new Suffix("tiniz", Semantics.KPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("tunuz"))
                        {
                            suffixes.Add(new Suffix("tunuz", Semantics.KPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("tünüz"))
                        {
                            suffixes.Add(new Suffix("tünüz", Semantics.KPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // KPTV
                        if (!IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("dılar"))
                        {
                            suffixes.Add(new Suffix("dılar", Semantics.KPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("diler"))
                        {
                            suffixes.Add(new Suffix("diler", Semantics.KPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("dular"))
                        {
                            suffixes.Add(new Suffix("dular", Semantics.KPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("düler"))
                        {
                            suffixes.Add(new Suffix("düler", Semantics.KPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("tılar"))
                        {
                            suffixes.Add(new Suffix("tılar", Semantics.KPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("tiler"))
                        {
                            suffixes.Add(new Suffix("tiler", Semantics.KPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("tular"))
                        {
                            suffixes.Add(new Suffix("tular", Semantics.KPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("tüler"))
                        {
                            suffixes.Add(new Suffix("tüler", Semantics.KPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // HSFV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("mışım"))
                        {
                            suffixes.Add(new Suffix("mışım", Semantics.HSFV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("mişim"))
                        {
                            suffixes.Add(new Suffix("mişim", Semantics.HSFV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("muşum"))
                        {
                            suffixes.Add(new Suffix("muşum", Semantics.HSFV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("müşüm"))
                        {
                            suffixes.Add(new Suffix("müşüm", Semantics.HSFV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // HPFV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("mışız"))
                        {
                            suffixes.Add(new Suffix("mışız", Semantics.HPFV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("mişiz"))
                        {
                            suffixes.Add(new Suffix("mişiz", Semantics.HPFV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("muşuz"))
                        {
                            suffixes.Add(new Suffix("muşuz", Semantics.HPFV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("müşüz"))
                        {
                            suffixes.Add(new Suffix("müşüz", Semantics.HPFV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // COSF
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 5).Equals("yorum"))
                        {
                            suffixes.Add(new Suffix("yorum", Semantics.COSF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // COPF
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 5).Equals("yoruz"))
                        {
                            suffixes.Add(new Suffix("yoruz", Semantics.COPF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // PRSS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("arsın"))
                        {
                            suffixes.Add(new Suffix("arsın", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("ersin"))
                        {
                            suffixes.Add(new Suffix("ersin", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("ırsın"))
                        {
                            suffixes.Add(new Suffix("ırsın", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("irsin"))
                        {
                            suffixes.Add(new Suffix("irsin", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("ursun"))
                        {
                            suffixes.Add(new Suffix("ursun", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("ürsün"))
                        {
                            suffixes.Add(new Suffix("ürsün", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // PRPT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("arlar"))
                        {
                            suffixes.Add(new Suffix("arlar", Semantics.PRPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("erler"))
                        {
                            suffixes.Add(new Suffix("erler", Semantics.PRPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("ırlar"))
                        {
                            suffixes.Add(new Suffix("ırlar", Semantics.PRPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("irler"))
                        {
                            suffixes.Add(new Suffix("irler", Semantics.PRPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("urlar"))
                        {
                            suffixes.Add(new Suffix("urlar", Semantics.PRPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("ürler"))
                        {
                            suffixes.Add(new Suffix("ürler", Semantics.PRPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // NGVF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("yamam"))
                        {
                            suffixes.Add(new Suffix("yamam", Semantics.NGVF, -1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("yemem"))
                        {
                            suffixes.Add(new Suffix("yemem", Semantics.NGVF, -1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // NGVT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("yamaz"))
                        {
                            suffixes.Add(new Suffix("yamaz", Semantics.NGVT, -1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("yemez"))
                        {
                            suffixes.Add(new Suffix("yemez", Semantics.NGVT, -1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // NPPF
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("mayız"))
                        {
                            suffixes.Add(new Suffix("mayız", Semantics.NPPF, -1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("meyiz"))
                        {
                            suffixes.Add(new Suffix("meyiz", Semantics.NPPF, -1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // CPSV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("sanız"))
                        {
                            suffixes.Add(new Suffix("sanız", Semantics.CPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("seniz"))
                        {
                            suffixes.Add(new Suffix("seniz", Semantics.CPSV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // CPTV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("larsa"))
                        {
                            suffixes.Add(new Suffix("larsa", Semantics.CPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("lerse"))
                        {
                            suffixes.Add(new Suffix("lerse", Semantics.CPTV, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // RESF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("yayım"))
                        {
                            suffixes.Add(new Suffix("yayım", Semantics.RESF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("yeyim"))
                        {
                            suffixes.Add(new Suffix("yeyim", Semantics.RESF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // RESS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("yasın"))
                        {
                            suffixes.Add(new Suffix("yasın", Semantics.RESS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("yesin"))
                        {
                            suffixes.Add(new Suffix("yesin", Semantics.RESS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // REPF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("yalım"))
                        {
                            suffixes.Add(new Suffix("yalım", Semantics.REPF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("yelim"))
                        {
                            suffixes.Add(new Suffix("yelim", Semantics.REPF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        // REPT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("yalar"))
                        {
                            suffixes.Add(new Suffix("yalar", Semantics.REPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("yeler"))
                        {
                            suffixes.Add(new Suffix("yeler", Semantics.REPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                    }
                    if (word.Length > (i + 4))
                    {
                        // FST
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("acak"))
                        {
                            suffixes.Add(new Suffix("acak", Semantics.FST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ecek"))
                        {
                            suffixes.Add(new Suffix("ecek", Semantics.FST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // COST
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ıyor"))
                        {
                            suffixes.Add(new Suffix("ıyor", Semantics.COST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("iyor"))
                        {
                            suffixes.Add(new Suffix("iyor", Semantics.COST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("uyor"))
                        {
                            suffixes.Add(new Suffix("uyor", Semantics.COST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("üyor"))
                        {
                            suffixes.Add(new Suffix("üyor", Semantics.COST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // PRSF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("arım"))
                        {
                            suffixes.Add(new Suffix("arım", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("erim"))
                        {
                            suffixes.Add(new Suffix("erim", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ırım"))
                        {
                            suffixes.Add(new Suffix("ırım", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("irim"))
                        {
                            suffixes.Add(new Suffix("irim", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("urum"))
                        {
                            suffixes.Add(new Suffix("urum", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ürüm"))
                        {
                            suffixes.Add(new Suffix("ürüm", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // PRSS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("rsın"))
                        {
                            suffixes.Add(new Suffix("rsın", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("rsin"))
                        {
                            suffixes.Add(new Suffix("rsin", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("rsun"))
                        {
                            suffixes.Add(new Suffix("rsun", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("rsün"))
                        {
                            suffixes.Add(new Suffix("rsün", Semantics.PRSS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // PRPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("arız"))
                        {
                            suffixes.Add(new Suffix("arız", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("eriz"))
                        {
                            suffixes.Add(new Suffix("eriz", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ırız"))
                        {
                            suffixes.Add(new Suffix("ırız", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("iriz"))
                        {
                            suffixes.Add(new Suffix("iriz", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("uruz"))
                        {
                            suffixes.Add(new Suffix("uruz", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ürüz"))
                        {
                            suffixes.Add(new Suffix("ürüz", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // PRPT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("rlar"))
                        {
                            suffixes.Add(new Suffix("rlar", Semantics.PRPT, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("rler"))
                        {
                            suffixes.Add(new Suffix("rler", Semantics.PRPT, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // REST
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ayım"))
                        {
                            suffixes.Add(new Suffix("ayım", Semantics.REST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("eyim"))
                        {
                            suffixes.Add(new Suffix("eyim", Semantics.REST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // RESS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("asın"))
                        {
                            suffixes.Add(new Suffix("asın", Semantics.RESS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("esin"))
                        {
                            suffixes.Add(new Suffix("esin", Semantics.RESS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // REPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("alım"))
                        {
                            suffixes.Add(new Suffix("alım", Semantics.REPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("elim"))
                        {
                            suffixes.Add(new Suffix("elim", Semantics.REPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // REPT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("alar"))
                        {
                            suffixes.Add(new Suffix("alar", Semantics.REPT, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("eler"))
                        {
                            suffixes.Add(new Suffix("eler", Semantics.REPT, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // NEST
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("malı"))
                        {
                            suffixes.Add(new Suffix("malı", Semantics.NEST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("meli"))
                        {
                            suffixes.Add(new Suffix("meli", Semantics.NEST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // MPPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ınız"))
                        {
                            suffixes.Add(new Suffix("ınız", Semantics.MPPS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("iniz"))
                        {
                            suffixes.Add(new Suffix("iniz", Semantics.MPPS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("unuz"))
                        {
                            suffixes.Add(new Suffix("unuz", Semantics.MPPS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ünüz"))
                        {
                            suffixes.Add(new Suffix("ünüz", Semantics.MPPS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // NGVF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("amam"))
                        {
                            suffixes.Add(new Suffix("amam", Semantics.NGVF, -1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("emem"))
                        {
                            suffixes.Add(new Suffix("emem", Semantics.NGVF, -1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        // NGVT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("amaz"))
                        {
                            suffixes.Add(new Suffix("amaz", Semantics.NGVT, -1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("emez"))
                        {
                            suffixes.Add(new Suffix("emez", Semantics.NGVT, -1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                    }
                    if (word.Length > (i + 3))
                    {
                        // LEX
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("mak"))
                        {
                            suffixes.Add(new Suffix("mak", Semantics.LEX, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                            return ParseNameSuffixes(stem, word, out success, false, suffixes);
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("mek"))
                        {
                            suffixes.Add(new Suffix("mek", Semantics.LEX, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                            return ParseNameSuffixes(stem, word, out success, false, suffixes);
                        }
                        // KSFV
                        if (!IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("dım"))
                        {
                            suffixes.Add(new Suffix("dım", Semantics.KSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("dim"))
                        {
                            suffixes.Add(new Suffix("dim", Semantics.KSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("dum"))
                        {
                            suffixes.Add(new Suffix("dum", Semantics.KSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("düm"))
                        {
                            suffixes.Add(new Suffix("düm", Semantics.KSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("tım"))
                        {
                            suffixes.Add(new Suffix("tım", Semantics.KSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("tim"))
                        {
                            suffixes.Add(new Suffix("tim", Semantics.KSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("tum"))
                        {
                            suffixes.Add(new Suffix("tum", Semantics.KSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("tüm"))
                        {
                            suffixes.Add(new Suffix("tüm", Semantics.KSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // KSSV
                        if (!IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("dın"))
                        {
                            suffixes.Add(new Suffix("dın", Semantics.KSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("din"))
                        {
                            suffixes.Add(new Suffix("din", Semantics.KSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("dun"))
                        {
                            suffixes.Add(new Suffix("dun", Semantics.KSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("dün"))
                        {
                            suffixes.Add(new Suffix("dün", Semantics.KSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("tın"))
                        {
                            suffixes.Add(new Suffix("tın", Semantics.KSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("tin"))
                        {
                            suffixes.Add(new Suffix("tin", Semantics.KSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("tun"))
                        {
                            suffixes.Add(new Suffix("tun", Semantics.KSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("tün"))
                        {
                            suffixes.Add(new Suffix("tün", Semantics.KSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // KPFV
                        if (!IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("dık"))
                        {
                            suffixes.Add(new Suffix("dık", Semantics.KPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("dik"))
                        {
                            suffixes.Add(new Suffix("dik", Semantics.KPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("duk"))
                        {
                            suffixes.Add(new Suffix("duk", Semantics.KPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("dük"))
                        {
                            suffixes.Add(new Suffix("dük", Semantics.KPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("tık"))
                        {
                            suffixes.Add(new Suffix("tık", Semantics.KPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("tik"))
                        {
                            suffixes.Add(new Suffix("tik", Semantics.KPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("tuk"))
                        {
                            suffixes.Add(new Suffix("tuk", Semantics.KPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("tük"))
                        {
                            suffixes.Add(new Suffix("tük", Semantics.KPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // HSTV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("mış"))
                        {
                            suffixes.Add(new Suffix("mış", Semantics.HSTV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("miş"))
                        {
                            suffixes.Add(new Suffix("miş", Semantics.HSTV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("muş"))
                        {
                            suffixes.Add(new Suffix("muş", Semantics.HSTV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("müş"))
                        {
                            suffixes.Add(new Suffix("müş", Semantics.HSTV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // COST
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 3).Equals("yor"))
                        {
                            suffixes.Add(new Suffix("yor", Semantics.COST, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // PRSF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("rız"))
                        {
                            suffixes.Add(new Suffix("rız", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("riz"))
                        {
                            suffixes.Add(new Suffix("riz", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("ruz"))
                        {
                            suffixes.Add(new Suffix("ruz", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("rüz"))
                        {
                            suffixes.Add(new Suffix("rüz", Semantics.PRSF, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // PRPF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("rım"))
                        {
                            suffixes.Add(new Suffix("rım", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("rim"))
                        {
                            suffixes.Add(new Suffix("rim", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("rum"))
                        {
                            suffixes.Add(new Suffix("rum", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("rüm"))
                        {
                            suffixes.Add(new Suffix("rüm", Semantics.PRPF, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // NPSF
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("mam"))
                        {
                            suffixes.Add(new Suffix("mam", Semantics.NPSF, -1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("mem"))
                        {
                            suffixes.Add(new Suffix("mem", Semantics.NPSF, -1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // NPST
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("maz"))
                        {
                            suffixes.Add(new Suffix("maz", Semantics.NPSF, -1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("mez"))
                        {
                            suffixes.Add(new Suffix("mez", Semantics.NPSF, -1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // CSFV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("sam"))
                        {
                            suffixes.Add(new Suffix("sam", Semantics.CSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("sem"))
                        {
                            suffixes.Add(new Suffix("sem", Semantics.CSFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // CSSV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("san"))
                        {
                            suffixes.Add(new Suffix("san", Semantics.CSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("sen"))
                        {
                            suffixes.Add(new Suffix("sen", Semantics.CSSV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // CPFV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("sak"))
                        {
                            suffixes.Add(new Suffix("sak", Semantics.CPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("sek"))
                        {
                            suffixes.Add(new Suffix("sek", Semantics.CPFV, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        // MPST
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("sın"))
                        {
                            suffixes.Add(new Suffix("sın", Semantics.MPST, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("sin"))
                        {
                            suffixes.Add(new Suffix("sin", Semantics.MPST, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("sun"))
                        {
                            suffixes.Add(new Suffix("sun", Semantics.MPST, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("sün"))
                        {
                            suffixes.Add(new Suffix("sün", Semantics.MPST, 1));
                            stem = stem + word.Substring(i + 1, 3);
                            i = i + 3;
                        }
                    }
                    if (word.Length > (i + 2))
                    {
                        // PRST
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ar"))
                        {
                            suffixes.Add(new Suffix("ar", Semantics.PRST, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("er"))
                        {
                            suffixes.Add(new Suffix("er", Semantics.PRST, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ır"))
                        {
                            suffixes.Add(new Suffix("ır", Semantics.PRST, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("ir"))
                        {
                            suffixes.Add(new Suffix("ir", Semantics.PRST, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("ur"))
                        {
                            suffixes.Add(new Suffix("ur", Semantics.PRST, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("ür"))
                        {
                            suffixes.Add(new Suffix("ür", Semantics.PRST, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        // KSTV
                        if (!IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("dı"))
                        {
                            suffixes.Add(new Suffix("dı", Semantics.KSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("di"))
                        {
                            suffixes.Add(new Suffix("di", Semantics.KSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("du"))
                        {
                            suffixes.Add(new Suffix("du", Semantics.KSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (!IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("dü"))
                        {
                            suffixes.Add(new Suffix("dü", Semantics.KSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("tı"))
                        {
                            suffixes.Add(new Suffix("tı", Semantics.KSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("ti"))
                        {
                            suffixes.Add(new Suffix("ti", Semantics.KSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("tu"))
                        {
                            suffixes.Add(new Suffix("tu", Semantics.KSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("tü"))
                        {
                            suffixes.Add(new Suffix("tü", Semantics.KSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        // NEG
                        if (!foundNeg)
                        {
                            if (!foundNeg && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("ma"))
                            {
                                suffixes.Add(new Suffix("ma", Semantics.NEG, -1));
                                stem = stem + word.Substring(i + 1, 2);
                                i = i + 2;
                                foundNeg = true;
                            }
                            if (!foundNeg && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("me"))
                            {
                                suffixes.Add(new Suffix("me", Semantics.NEG, -1));
                                stem = stem + word.Substring(i + 1, 2);
                                i = i + 2;
                                foundNeg = true;
                            }
                            if (!foundNeg && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("mı"))
                            {
                                suffixes.Add(new Suffix("mı", Semantics.NEG, -1));
                                stem = stem + word.Substring(i + 1, 2);
                                i = i + 2;
                                foundNeg = true;
                            }
                            if (!foundNeg && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("mi"))
                            {
                                suffixes.Add(new Suffix("mi", Semantics.NEG, -1));
                                stem = stem + word.Substring(i + 1, 2);
                                i = i + 2;
                                foundNeg = true;
                            }
                            if (!foundNeg && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("mu"))
                            {
                                suffixes.Add(new Suffix("mu", Semantics.NEG, -1));
                                stem = stem + word.Substring(i + 1, 2);
                                i = i + 2;
                                foundNeg = true;
                            }
                            if (!foundNeg && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("mü"))
                            {
                                suffixes.Add(new Suffix("mü", Semantics.NEG, -1));
                                stem = stem + word.Substring(i + 1, 2);
                                i = i + 2;
                                foundNeg = true;
                            }
                        }
                        // CSTV
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("sa"))
                        {
                            suffixes.Add(new Suffix("sa", Semantics.CSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("se"))
                        {
                            suffixes.Add(new Suffix("se", Semantics.CSTV, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        // REST
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("ya"))
                        {
                            suffixes.Add(new Suffix("ya", Semantics.REST, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("ye"))
                        {
                            suffixes.Add(new Suffix("ye", Semantics.REST, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        // MPPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ın"))
                        {
                            suffixes.Add(new Suffix("ın", Semantics.MPPS, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("in"))
                        {
                            suffixes.Add(new Suffix("in", Semantics.MPPS, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("un"))
                        {
                            suffixes.Add(new Suffix("un", Semantics.MPPS, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("ün"))
                        {
                            suffixes.Add(new Suffix("ün", Semantics.MPPS, 1));
                            stem = stem + word.Substring(i + 1, 2);
                            i = i + 2;
                        }
                    }
                    if (word.Length > (i + 1))
                    {
                        // REST
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 1).Equals("a"))
                        {
                            suffixes.Add(new Suffix("a", Semantics.REST, 1));
                            stem = stem + word.Substring(i + 1, 1);
                            i++;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 1).Equals("e"))
                        {
                            suffixes.Add(new Suffix("e", Semantics.REST, 1));
                            stem = stem + word.Substring(i + 1, 1);
                            i++;
                        }
                        // PRST
                        if (EndsWithVowel(stem) && word.Substring(i + 1, 1).Equals("r"))
                        {
                            suffixes.Add(new Suffix("r", Semantics.PRST, 1));
                            stem = stem + word.Substring(i + 1, 1);
                            i++;
                        }
                    }
                    iteration++;
                }
                catch (ArgumentOutOfRangeException)
                {
                    iteration++;
                }
            }
            if (suffixes.Count == 0)
                success = false;
            if (iteration >= 64)
                success = false;
            return suffixes;
        }



        private static List<Suffix> ParseNameSuffixes(string stem, string word, out bool success, bool foundPossessive, List<Suffix> suffixes)
        {
            int iteration = 0;
            int i = stem.Length - 1;
            success = true;
            while (!word.Equals(stem) && iteration < 64)
            {
                try {
                    if (word.Length > (i + 9))
                    {
                        // HPS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 9).Equals("ymışsınız"))
                        {
                            suffixes.Add(new Suffix("ymışsınız", Semantics.HPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 9).Equals("ymişsiniz"))
                        {
                            suffixes.Add(new Suffix("ymişsiniz", Semantics.HPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 9).Equals("ymuşsunuz"))
                        {
                            suffixes.Add(new Suffix("ymuşsunuz", Semantics.HPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 9).Equals("ymüşsünüz"))
                        {
                            suffixes.Add(new Suffix("ymüşsünüz", Semantics.HPS, 1));
                            stem = stem + word.Substring(i + 1, 9);
                            i = i + 9;
                        }
                    }
                    if (word.Length > (i + 8))
                    {
                        // HPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 8).Equals("mışsınız"))
                        {
                            suffixes.Add(new Suffix("mışsınız", Semantics.HPS, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 8).Equals("mişsiniz"))
                        {
                            suffixes.Add(new Suffix("mişsiniz", Semantics.HPS, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 8).Equals("muşsunuz"))
                        {
                            suffixes.Add(new Suffix("muşsunuz", Semantics.HPS, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 8).Equals("müşsünüz"))
                        {
                            suffixes.Add(new Suffix("müşsünüz", Semantics.HPS, 1));
                            stem = stem + word.Substring(i + 1, 8);
                            i = i + 8;
                        }
                    }
                    if (word.Length > (i + 7))
                    {
                        // HSS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 7).Equals("ymışsın"))
                        {
                            suffixes.Add(new Suffix("ymışsın", Semantics.HSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 7).Equals("ymişsin"))
                        {
                            suffixes.Add(new Suffix("ydiniz", Semantics.HSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("ymuşsun"))
                        {
                            suffixes.Add(new Suffix("ydunuz", Semantics.HSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("ymüşsün"))
                        {
                            suffixes.Add(new Suffix("ymüşsün", Semantics.HSS, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        // HPT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 7).Equals("ymışlar"))
                        {
                            suffixes.Add(new Suffix("ymışlar", Semantics.HPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 7).Equals("ymişler"))
                        {
                            suffixes.Add(new Suffix("ymişler", Semantics.HPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 7).Equals("ymuşlar"))
                        {
                            suffixes.Add(new Suffix("ymuşlar", Semantics.HPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 7).Equals("ymüşler"))
                        {
                            suffixes.Add(new Suffix("ymüşler", Semantics.HPT, 1));
                            stem = stem + word.Substring(i + 1, 7);
                            i = i + 7;
                        }
                    }
                    if (word.Length > (i + 6))
                    {
                        // KPS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("ydınız"))
                        {
                            suffixes.Add(new Suffix("ydınız", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("ydiniz"))
                        {
                            suffixes.Add(new Suffix("ydiniz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("ydunuz"))
                        {
                            suffixes.Add(new Suffix("ydunuz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("ydünüz"))
                        {
                            suffixes.Add(new Suffix("ydünüz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // KPT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("ydılar"))
                        {
                            suffixes.Add(new Suffix("ydılar", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("ydiler"))
                        {
                            suffixes.Add(new Suffix("ydiler", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("ydular"))
                        {
                            suffixes.Add(new Suffix("ydular", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("ydüler"))
                        {
                            suffixes.Add(new Suffix("ydüler", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // HSF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("ymışım"))
                        {
                            suffixes.Add(new Suffix("ymışım", Semantics.HSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("ymişim"))
                        {
                            suffixes.Add(new Suffix("ymişim", Semantics.HSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("ymuşum"))
                        {
                            suffixes.Add(new Suffix("ymuşum", Semantics.HSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("ymüşüm"))
                        {
                            suffixes.Add(new Suffix("ymüşüm", Semantics.HSF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // HPF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("ymışız"))
                        {
                            suffixes.Add(new Suffix("ymışız", Semantics.HPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("ymişiz"))
                        {
                            suffixes.Add(new Suffix("ymişiz", Semantics.HPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("ymuşuz"))
                        {
                            suffixes.Add(new Suffix("ymuşuz", Semantics.HPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("ymüşüz"))
                        {
                            suffixes.Add(new Suffix("ymüşüz", Semantics.HPF, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // HPT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("mışlar"))
                        {
                            suffixes.Add(new Suffix("mışlar", Semantics.HPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("mişler"))
                        {
                            suffixes.Add(new Suffix("mişler", Semantics.HPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("muşlar"))
                        {
                            suffixes.Add(new Suffix("muşlar", Semantics.HPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("müşler"))
                        {
                            suffixes.Add(new Suffix("müşler", Semantics.HPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // CPS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("ysanız"))
                        {
                            suffixes.Add(new Suffix("ysanız", Semantics.CPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("yseniz"))
                        {
                            suffixes.Add(new Suffix("yseniz", Semantics.CPS, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // CPT
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("ysalar"))
                        {
                            suffixes.Add(new Suffix("ysalar", Semantics.CPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("yseler"))
                        {
                            suffixes.Add(new Suffix("yseler", Semantics.CPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        // IPT
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("dırlar"))
                        {
                            suffixes.Add(new Suffix("dırlar", Semantics.IPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("dirler"))
                        {
                            suffixes.Add(new Suffix("dirler", Semantics.IPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("durlar"))
                        {
                            suffixes.Add(new Suffix("durlar", Semantics.IPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("dürler"))
                        {
                            suffixes.Add(new Suffix("dürler", Semantics.IPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 6).Equals("tırlar"))
                        {
                            suffixes.Add(new Suffix("tırlar", Semantics.IPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 6).Equals("tirler"))
                        {
                            suffixes.Add(new Suffix("tirler", Semantics.IPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 6).Equals("turlar"))
                        {
                            suffixes.Add(new Suffix("turlar", Semantics.IPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 6).Equals("türler"))
                        {
                            suffixes.Add(new Suffix("türler", Semantics.IPT, 1));
                            stem = stem + word.Substring(i + 1, 6);
                            i = i + 6;
                        }
                    }
                    if (word.Length > (i + 5))
                    {
                        // IPS
                        if ((LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("sınız"))
                        {
                            suffixes.Add(new Suffix("sınız", Semantics.IPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("siniz"))
                        {
                            suffixes.Add(new Suffix("siniz", Semantics.IPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("sunuz"))
                        {
                            suffixes.Add(new Suffix("sunuz", Semantics.IPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if ((LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("sünüz"))
                        {
                            suffixes.Add(new Suffix("sünüz", Semantics.IPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }

                        // KPS
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("dınız"))
                        {
                            suffixes.Add(new Suffix("dınız", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("diniz"))
                        {
                            suffixes.Add(new Suffix("diniz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("dunuz"))
                        {
                            suffixes.Add(new Suffix("dunuz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("dünüz"))
                        {
                            suffixes.Add(new Suffix("dünüz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("tınız"))
                        {
                            suffixes.Add(new Suffix("tınız", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("tiniz"))
                        {
                            suffixes.Add(new Suffix("tiniz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("tunuz"))
                        {
                            suffixes.Add(new Suffix("tunuz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("tünüz"))
                        {
                            suffixes.Add(new Suffix("tünüz", Semantics.KPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }

                        // KPT
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("dılar"))
                        {
                            suffixes.Add(new Suffix("dılar", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("diler"))
                        {
                            suffixes.Add(new Suffix("dilar", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("dular"))
                        {
                            suffixes.Add(new Suffix("dular", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && !IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("düler"))
                        {
                            suffixes.Add(new Suffix("düler", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("tılar"))
                        {
                            suffixes.Add(new Suffix("tılar", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("tiler"))
                        {
                            suffixes.Add(new Suffix("tiler", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("tular"))
                        {
                            suffixes.Add(new Suffix("tular", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && IsRough(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("tüler"))
                        {
                            suffixes.Add(new Suffix("tüler", Semantics.KPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }

                        // HSF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("mışım"))
                        {
                            suffixes.Add(new Suffix("mışım", Semantics.HSF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("mişim"))
                        {
                            suffixes.Add(new Suffix("mişim", Semantics.HSF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("muşum"))
                        {
                            suffixes.Add(new Suffix("muşum", Semantics.HSF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("müşüm"))
                        {
                            suffixes.Add(new Suffix("müşüm", Semantics.HSF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }

                        // HPF
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 5).Equals("mışız"))
                        {
                            suffixes.Add(new Suffix("mışız", Semantics.HPF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 5).Equals("mişiz"))
                        {
                            suffixes.Add(new Suffix("mişiz", Semantics.HPF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("muşuz"))
                        {
                            suffixes.Add(new Suffix("muşuz", Semantics.HPF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("müşüz"))
                        {
                            suffixes.Add(new Suffix("müşüz", Semantics.HPF, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }

                        // CPS
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("sanız"))
                        {
                            suffixes.Add(new Suffix("sanız", Semantics.CPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("seniz"))
                        {
                            suffixes.Add(new Suffix("seniz", Semantics.CPS, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }

                        // CPT
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 5).Equals("salar"))
                        {
                            suffixes.Add(new Suffix("salar", Semantics.CPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                        if (EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 5).Equals("seler"))
                        {
                            suffixes.Add(new Suffix("seler", Semantics.CPT, 1));
                            stem = stem + word.Substring(i + 1, 5);
                            i = i + 5;
                        }
                    }
                    if (word.Length > (i + 4))
                    {
                        // ABL
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ndan"))
                        {
                            suffixes.Add(new Suffix("ndan", Semantics.ABL, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("nden"))
                        {
                            suffixes.Add(new Suffix("nden", Semantics.ABL, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }

                        // PPF
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ımız"))
                        {
                            suffixes.Add(new Suffix("ımız", Semantics.PPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("imiz"))
                        {
                            suffixes.Add(new Suffix("imiz", Semantics.PPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("umuz"))
                        {
                            suffixes.Add(new Suffix("umuz", Semantics.PPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ümüz"))
                        {
                            suffixes.Add(new Suffix("ümüz", Semantics.PPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }

                        // PPS
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ınız"))
                        {
                            suffixes.Add(new Suffix("ınız", Semantics.PPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("iniz"))
                        {
                            suffixes.Add(new Suffix("iniz", Semantics.PPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("unuz"))
                        {
                            suffixes.Add(new Suffix("unuz", Semantics.PPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ünüz"))
                        {
                            suffixes.Add(new Suffix("ünüz", Semantics.PPF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }

                        // PPT
                        if (!foundPossessive && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ları"))
                        {
                            suffixes.Add(new Suffix("ları", Semantics.PPT, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }
                        if ((!foundPossessive && LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("leri"))
                        {
                            suffixes.Add(new Suffix("leri", Semantics.PPT, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                            foundPossessive = true;
                        }

                        // KSF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ydım"))
                        {
                            suffixes.Add(new Suffix("ydım", Semantics.KSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("ydim"))
                        {
                            suffixes.Add(new Suffix("ydim", Semantics.KSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ydum"))
                        {
                            suffixes.Add(new Suffix("ydum", Semantics.KSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ydüm"))
                        {
                            suffixes.Add(new Suffix("ydüm", Semantics.KSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }

                        // KSS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ydın"))
                        {
                            suffixes.Add(new Suffix("ydın", Semantics.KSS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("ydin"))
                        {
                            suffixes.Add(new Suffix("ydin", Semantics.KSS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ydun"))
                        {
                            suffixes.Add(new Suffix("ydun", Semantics.KSS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ydün"))
                        {
                            suffixes.Add(new Suffix("ydün", Semantics.KSS, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }

                        // KPF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ydık"))
                        {
                            suffixes.Add(new Suffix("ydık", Semantics.KSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("ydik"))
                        {
                            suffixes.Add(new Suffix("ydik", Semantics.KSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("yduk"))
                        {
                            suffixes.Add(new Suffix("yduk", Semantics.KSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ydük"))
                        {
                            suffixes.Add(new Suffix("ydük", Semantics.KSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }

                        // HST
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 4).Equals("ymış"))
                        {
                            suffixes.Add(new Suffix("ymış", Semantics.HST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 4).Equals("ymiş"))
                        {
                            suffixes.Add(new Suffix("ymiş", Semantics.HST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ymuş"))
                        {
                            suffixes.Add(new Suffix("ymuş", Semantics.HST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ymüş"))
                        {
                            suffixes.Add(new Suffix("ymüş", Semantics.HST, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }

                        // CSF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ysam"))
                        {
                            suffixes.Add(new Suffix("ysam", Semantics.CSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ysem"))
                        {
                            suffixes.Add(new Suffix("ysem", Semantics.CSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }

                        // CSS
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ysan"))
                        {
                            suffixes.Add(new Suffix("ysan", Semantics.CSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ysen"))
                        {
                            suffixes.Add(new Suffix("ysen", Semantics.CSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }

                        // CPF
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı' || LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 4).Equals("ysak"))
                        {
                            suffixes.Add(new Suffix("ysak", Semantics.CSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                        if (EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i' || LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 4).Equals("ysek"))
                        {
                            suffixes.Add(new Suffix("ysek", Semantics.CSF, 1));
                            stem = stem + word.Substring(i + 1, 4);
                            i = i + 4;
                        }
                    }
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
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("mız"))
                        {
                            suffixes.Add(new Suffix("mız", Semantics.PPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("miz"))
                        {
                            suffixes.Add(new Suffix("miz", Semantics.PPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("muz"))
                        {
                            suffixes.Add(new Suffix("muz", Semantics.PPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("müz"))
                        {
                            suffixes.Add(new Suffix("müz", Semantics.PPF, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                            foundPossessive = true;
                        }

                        // PPS
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 3).Equals("nız"))
                        {
                            suffixes.Add(new Suffix("nız", Semantics.PPS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 3).Equals("niz"))
                        {
                            suffixes.Add(new Suffix("niz", Semantics.PPS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 3).Equals("nuz"))
                        {
                            suffixes.Add(new Suffix("nuz", Semantics.PPS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 3).Equals("nüz"))
                        {
                            suffixes.Add(new Suffix("nüz", Semantics.PPS, 1));
                            stem = stem + (word.Substring(i + 1, 3));
                            i = i + 3;
                            foundPossessive = true;
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
                        // PST
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("sı"))
                        {
                            suffixes.Add(new Suffix("sı", Semantics.PST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("si"))
                        {
                            suffixes.Add(new Suffix("si", Semantics.PST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("su"))
                        {
                            suffixes.Add(new Suffix("su", Semantics.PST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithVowel(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("sü"))
                        {
                            suffixes.Add(new Suffix("sü", Semantics.PST, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
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


                        // PSF
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ım"))
                        {
                            suffixes.Add(new Suffix("ım", Semantics.PSF, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("im"))
                        {
                            suffixes.Add(new Suffix("im", Semantics.PSF, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("um"))
                        {
                            suffixes.Add(new Suffix("um", Semantics.PSF, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("üm"))
                        {
                            suffixes.Add(new Suffix("üm", Semantics.PSF, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }

                        // PSS
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 2).Equals("ın"))
                        {
                            suffixes.Add(new Suffix("ın", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 2).Equals("in"))
                        {
                            suffixes.Add(new Suffix("in", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 2).Equals("un"))
                        {
                            suffixes.Add(new Suffix("un", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 2).Equals("ün"))
                        {
                            suffixes.Add(new Suffix("ün", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 2));
                            i = i + 2;
                            foundPossessive = true;
                        }
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
                    if (word.Length > (i + 1)) { 
                        // ACC
                        if (foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 1).Equals("ı")) { 
                            suffixes.Add(new Suffix("ı", Semantics.ACC, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                        }
                        if (foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 1).Equals("i")) {
                            suffixes.Add(new Suffix("i", Semantics.ACC, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                        }
                        if (foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 1).Equals("u")) {
                            suffixes.Add(new Suffix("u", Semantics.ACC, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                        }
                        if (foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 1).Equals("ü")) {
                            suffixes.Add(new Suffix("ü", Semantics.ACC, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                        }

                        // PST
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'a' || LastVowel(stem) == 'ı') && word.Substring(i + 1, 1).Equals("ı"))
                        {
                            suffixes.Add(new Suffix("ı", Semantics.PST, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'e' || LastVowel(stem) == 'i') && word.Substring(i + 1, 1).Equals("i"))
                        {
                            suffixes.Add(new Suffix("i", Semantics.PST, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'o' || LastVowel(stem) == 'u') && word.Substring(i + 1, 1).Equals("u"))
                        {
                            suffixes.Add(new Suffix("u", Semantics.PST, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                            foundPossessive = true;
                        }
                        if (!foundPossessive && EndsWithConsonant(stem) && (LastVowel(stem) == 'ö' || LastVowel(stem) == 'ü') && word.Substring(i + 1, 1).Equals("ü"))
                        {
                            suffixes.Add(new Suffix("ü", Semantics.PST, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                            foundPossessive = true;
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
                        if (!foundPossessive && EndsWithVowel(stem) && word.Substring(i + 1, 1).Equals("m"))
                        {
                            suffixes.Add(new Suffix("m", Semantics.PSF, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                            foundPossessive = true;
                        }

                        // PSS
                        if (!foundPossessive && EndsWithVowel(stem) && word.Substring(i + 1, 1).Equals("n"))
                        {
                            suffixes.Add(new Suffix("n", Semantics.PSS, 1));
                            stem = stem + (word.Substring(i + 1, 1));
                            i++;
                            foundPossessive = true;
                        }
                    }
                    iteration++;
                }
                catch(ArgumentOutOfRangeException)
                {
                    iteration++;
                }
            }
            if (suffixes.Count == 0)
                success = false;
            if (iteration >= 64)
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
