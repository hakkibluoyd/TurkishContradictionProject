using LibSVMsharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurkishTextContradictionAnalysis
{
    public partial class MainForm : Form
    {
        Sentence sentence1, sentence2;
        public MainForm()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (field1.Text != "" && field2.Text != "")
            {
                sentence1 = Sentence.ParseIntoSentence(field1.Text, 1, true);
                sentence2 = Sentence.ParseIntoSentence(field2.Text, 2, true);
                if(sentence1 != null && sentence2 != null) { 
                    List<SVMNode> nodes = new List<SVMNode>();
                    for (int j = 0; j < Corpus.count; j++)
                    {
                        SVMNode sentenceNode = new SVMNode(j, 0);
                        nodes.Add(sentenceNode);
                    }
                    for (int j = 0; j < sentence1.SentenceWords.Count; j++)
                    {
                        CorpusWord cw = Corpus.CorpusList.Find(c => c.Word.Equals(sentence1.SentenceWords[j].Stem.Word) && c.Attribute.Equals(sentence1.SentenceWords[j].Stem.Attribute));
                        SVMNode sentenceNode = new SVMNode(Corpus.CorpusList.IndexOf(cw), 1);
                        SVMNode sentenceNodeToRemove = new SVMNode(Corpus.CorpusList.IndexOf(cw), 0);
                        nodes.Remove(sentenceNodeToRemove);
                        nodes.Add(sentenceNode);
                    }
                    SVMNode[] sentenceNodes = nodes.ToArray();
                    sentenceNodes = sentenceNodes.OrderBy(x => x.Index).ToArray();

                    double s1_prediction = SVM.Predict(SVMInterface.model, sentenceNodes);

                    nodes = new List<SVMNode>();
                    for (int j = 0; j < Corpus.count; j++)
                    {
                        SVMNode sentenceNode = new SVMNode(j, 0);
                        nodes.Add(sentenceNode);
                    }
                    for (int j = 0; j < sentence2.SentenceWords.Count; j++)
                    {
                        CorpusWord cw = Corpus.CorpusList.Find(c => c.Word.Equals(sentence2.SentenceWords[j].Stem.Word) && c.Attribute.Equals(sentence2.SentenceWords[j].Stem.Attribute));
                        SVMNode sentenceNode = new SVMNode(Corpus.CorpusList.IndexOf(cw), 1);
                        SVMNode sentenceNodeToRemove = new SVMNode(Corpus.CorpusList.IndexOf(cw), 0);
                        nodes.Remove(sentenceNodeToRemove);
                        nodes.Add(sentenceNode);
                    }
                    sentenceNodes = nodes.ToArray();
                    sentenceNodes = sentenceNodes.OrderBy(x => x.Index).ToArray();

                    double s2_prediction = SVM.Predict(SVMInterface.model, sentenceNodes);

                    Console.WriteLine(s1_prediction + " " + s2_prediction);

                    if(s1_prediction != s2_prediction)
                    {
                        result.Text = "UNRELATED";
                    }
                    else if (sentence1 != null && sentence2 != null) { 
                        if (sentence1.Polarity == sentence2.Polarity)
                            result.Text = "NO CONTRADICTION";
                        if (sentence1.Polarity != sentence2.Polarity)
                            result.Text = "CONTRADICTION";
                    }
                }
            }
        }

        private void field1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SVMInterface.Train();
            /**Corpus.CorpusList.ForEach(delegate (CorpusWord cl) {
                if(cl.Attribute == Attribute.NULL)
                    Console.WriteLine(cl.Word + " | " + cl.Attribute);
            });*/
        }
    }
}
