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


        public static Sentence ParseIntoSentence(string text)
        {
            List<string> words = text.Split(new char[] { ',', ' ', '.', '?', ':', '!' }).ToList();
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
                SentenceWord.MorphologicalParsing(words[i]);
            }
            return null;
        }
    }
}
