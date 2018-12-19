using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurkishTextContradictionAnalysis
{
    public static class Corpus
    {
        private static List<CorpusWord> corpusList = new List<CorpusWord>();
        public static List<CorpusWord> CorpusList {
            get
            {
                return corpusList;
            }
            set
            {
                corpusList = value;
            }
        }
        static Corpus()
        {
            CreateList();
        }

        private static void CreateList()
        {
            StreamReader sr = new StreamReader("corpus.txt"); /** Change textfile name from here. **/
            string line;
            while((line = sr.ReadLine()) != null) {
                CorpusWord cw = new CorpusWord();
                List<string> tokens = line.Split(' ').ToList();
                if(tokens.Count > 1)
                { 
                    while(tokens.Count > 2)
                    {
                        tokens[0] = tokens[0] + " " + tokens[1];
                        tokens[1] = tokens[2];
                        if (tokens.Count > 3) tokens[2] = tokens[3]; 
                        tokens.RemoveAt(tokens.Count - 1);
                    }
                    cw.Attribute = CorpusWord.ToAttribute(tokens[1]);
                }
                else
                {
                    cw.Attribute = Attribute.NULL;
                }

                cw.Word = tokens[0];    
                corpusList.Add(cw);
            }
        }
    }
}
