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
                Sentence.ParseIntoSentence(field1.Text);
                Sentence.ParseIntoSentence(field2.Text);
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
            Corpus.CorpusList.ForEach(delegate (CorpusWord cl) {
                if(cl.Attribute == Attribute.NULL)
                    Console.WriteLine(cl.Word + " | " + cl.Attribute);
            });
        }
    }
}
