using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurkishTextContradictionAnalysis
{
    class Sentence
    {
        private List<SentenceWord> sw = new List<SentenceWord>();
        public List<SentenceWord> SentenceWords { get { return sw; } set { sw = value; } }
        private short polarity;
        public short Polarity { get { return polarity; } set { polarity = value; } }


        public static Sentence ParseIntoSentence(string text, int no, bool report)
        {
            Sentence sentence = new Sentence();
            List<string> words = text.Split(new char[] { ',', ' ', '.', '?', ':', '!' }).ToList();
            words.RemoveAll(e => e.Equals(" "));
            words.RemoveAll(e => e.Equals(""));
            for (int i = 0; i < words.Count; i++)
            {
                words[i] = words[i].ToLower();
            }

            // Detect three-word compounds.
            for (int i = 0; i < words.Count - 2; i++)
            {
                if (Corpus.CorpusList.Find(w => w.Word.Contains(words[i] + " " + words[i + 1] + " " + words[i + 2])) != null)
                {
                    words[i] = words[i] + " " + words[i + 1] + " " + words[i + 2];
                    for (int j = i + 3; j < words.Count; j++)
                        words[j - 2] = words[j];
                    words.RemoveAt(words.Count - 1);
                    words.RemoveAt(words.Count - 1);
                }
            }

            // Detect two-word compounds.
            for (int i = 0; i < words.Count - 1; i++)
            {
                if (Corpus.CorpusList.Find(w => w.Word.Contains(words[i] + " " + words[i + 1])) != null) { 
                    words[i] = words[i] + " " + words[i + 1];
                    for(int j = i + 2; j < words.Count; j++)
                        words[j - 1] = words[j];
                words.RemoveAt(words.Count - 1);
                }
            }

            for(int i = 0; i < words.Count; i++)
            {
                if(report) Console.WriteLine(words[i]);
                SentenceWord sw;
                sw = SentenceWord.MorphologicalParsing(words[i], words[i], report);
                sentence.sw.Add(sw);
            }
            if (report) Console.Write("Sentence " + no + " stems: ");

            int[] resultEachWord = new int[sentence.sw.Count];

            try {
                for (int i = 0; i < sentence.sw.Count; i++)
                {
                    resultEachWord[i] = Suffix.PolaritySummation(sentence.sw[i].Suffixes);
                    if (sentence.sw[i].Stem.Attribute == Attribute.IVBN || sentence.sw[i].Stem.Attribute == Attribute.VBN || sentence.sw[i].Stem.Attribute == Attribute.JJN)
                        resultEachWord[i] = Suffix.PolaritySummation(resultEachWord[i], -1);
                    if (sentence.sw[i].Stem.Attribute == Attribute.IVBP || sentence.sw[i].Stem.Attribute == Attribute.VBP || sentence.sw[i].Stem.Attribute == Attribute.JJP)
                        resultEachWord[i] = Suffix.PolaritySummation(resultEachWord[i], +1);
                }

                for (int i = 0; i < sentence.sw.Count; i++)
                    if (report) Console.Write(sentence.sw[i].Stem.Word + "(" + sentence.sw[i].Stem.Attribute + ") ");
                if (report) Console.WriteLine();
                int c = 0;

                // VE
                while (c < sentence.sw.Count && !sentence.sw[c].Stem.Word.Equals("ve"))
                    c++;
                if(c < sentence.sw.Count)
                {
                    List<int> changer = resultEachWord.ToList();
                    changer.Remove(changer[c - 1]);
                    changer.Remove(changer[c]);
                    changer[c - 1] = Suffix.PolarityAND(resultEachWord[c - 1], resultEachWord[c + 1]);
                    resultEachWord = changer.ToArray();
                }

                c = 0;

                // VEYA
                while (c < sentence.sw.Count && !sentence.sw[c].Stem.Word.Equals("veya"))
                    c++;
                if (c < sentence.sw.Count)
                {
                    List<int> changer = resultEachWord.ToList();
                    changer.Remove(changer[c - 1]);
                    changer.Remove(changer[c]);
                    changer[c - 1] = Suffix.PolarityOR(resultEachWord[c - 1], resultEachWord[c + 1]);
                    resultEachWord = changer.ToArray();
                }

                c = 0;

                // YA DA
                while (c < sentence.sw.Count && !sentence.sw[c].Stem.Word.Equals("ya da"))
                    c++;
                if (c < sentence.sw.Count)
                {
                    List<int> changer = resultEachWord.ToList();
                    changer.Remove(changer[c - 1]);
                    changer.Remove(changer[c]);
                    changer[c - 1] = Suffix.PolarityXOR(resultEachWord[c - 1], resultEachWord[c + 1]);
                    resultEachWord = changer.ToArray();
                }

                // VAR
                if(sentence.sw.IndexOf(sentence.sw.Find(s => s.Stem.Word.Equals("var"))) > 0)
                    resultEachWord[sentence.sw.IndexOf(sentence.sw.Find(s => s.Stem.Word.Equals("var")))] = 1;

                // DEĞİL
                if (sentence.sw.IndexOf(sentence.sw.Find(s => s.Stem.Word.Equals("değil"))) > 0)
                    resultEachWord[sentence.sw.IndexOf(sentence.sw.Find(s => s.Stem.Word.Equals("değil")))] = -1;

                // YOK  
                if (sentence.sw.IndexOf(sentence.sw.Find(s => s.Stem.Word.Equals("yok"))) > 0)
                    resultEachWord[sentence.sw.IndexOf(sentence.sw.Find(s => s.Stem.Word.Equals("yok")))] = -1;

            } catch(NullReferenceException)
            {
                MessageBox.Show("Error parsing the sentence " + no + ".");
                return null;
            } catch(ArgumentOutOfRangeException)
            {
                MessageBox.Show("Error parsing the sentence " + no + ".");
                return null;
            }
            sentence.polarity = (short) Suffix.PolaritySummation(resultEachWord);
            if (report) Console.WriteLine("Sentence " + no + " polarity is " + sentence.polarity + ".");
            return sentence;
        }
    }
}
