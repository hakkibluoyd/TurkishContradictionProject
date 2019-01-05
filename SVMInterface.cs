using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibSVMsharp;

namespace TurkishTextContradictionAnalysis
{
    public static class SVMInterface
    {
        public static SVMModel model;
        static SVMParameter parameter = new SVMParameter();
        static SVMInterface()
        {
            parameter.Type = SVMType.C_SVC;
            parameter.Kernel = SVMKernelType.LINEAR;
            parameter.C = 1;
            parameter.Gamma = 1; 
        }

        public static void Train()
        {
            SVMProblem svmproblem = new SVMProblem();
            List<Tuple<Sentence,string>> sentences = new List<Tuple<Sentence, string>>();
            StreamReader sr = new StreamReader("training_data.txt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] lines = line.Split(',');
                lines[0] = lines[0].Substring(1, lines[0].Length - 2);
                string sentence = lines[0];
                sentences.Add(Tuple.Create(Sentence.ParseIntoSentence(sentence, 0, false), lines[1]));
            }

            List<SVMNode[]> modelNodes = new List<SVMNode[]>();

            for(int i = 0; i < sentences.Count; i++)
            {
                List<SVMNode> nodes = new List<SVMNode>();
                for(int j = 0; j < Corpus.count; j++)
                {
                    SVMNode sentenceNode = new SVMNode(j, 0);
                    nodes.Add(sentenceNode);
                }
                for(int j = 0; j < sentences[i].Item1.SentenceWords.Count; j++)
                {
                    CorpusWord cw = Corpus.CorpusList.Find(c => c.Word.Equals(sentences[i].Item1.SentenceWords[j].Stem.Word) && c.Attribute.Equals(sentences[i].Item1.SentenceWords[j].Stem.Attribute));
                    SVMNode sentenceNode = new SVMNode(Corpus.CorpusList.IndexOf(cw), 1);
                    SVMNode sentenceNodeToRemove = new SVMNode(Corpus.CorpusList.IndexOf(cw), 0);
                    nodes.Remove(sentenceNodeToRemove);
                    nodes.Add(sentenceNode);
                }
                SVMNode[] sentenceNodes = nodes.ToArray();
                sentenceNodes = sentenceNodes.OrderBy(x => x.Index).ToArray();
                
                svmproblem.Add(sentenceNodes, Corpus.CorpusList.IndexOf(Corpus.CorpusList.Find(c => c.Word.Equals(sentences[i].Item2))));
            }
            
            model = SVM.Train(svmproblem, parameter);

            sr.Close();
        }
    }
}

