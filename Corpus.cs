using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurkishTextContradictionAnalysis
{
    public class Corpus
    {
        private List<CorpusWord> corpusList = new List<CorpusWord>();
        public List<CorpusWord> CorpusList {
            get
            {
                return corpusList;
            }
            set
            {
                corpusList = value;
            }
        }
        public Corpus()
        {
            CreateList();
        }

        private void CreateList()
        {
            StreamReader sr = new StreamReader("corpus.txt"); /** Change textfile name from here. **/
            int c = 0;
            string line;
            while((line = sr.ReadLine()) != null && c++ < 3000) {
                CorpusWord cw = new CorpusWord();
                List<string> tokens = line.Split(' ').ToList();
                while(tokens.Count > 2)
                {
                    tokens[0] = tokens[0] + " " + tokens[1];
                    tokens[1] = tokens[2];
                    tokens.RemoveAt(tokens.Count - 1);
                }
                Console.WriteLine("Read as: " + tokens[0] + " " + tokens[1]);
                cw.Word = tokens[0];
                cw.Attribute = CorpusWord.ToAttribute(tokens[1]);
                corpusList.Add(cw);
            }
        }
    }
}
