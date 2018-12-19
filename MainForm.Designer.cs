namespace TurkishTextContradictionAnalysis
{
    partial class MainForm
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.field1 = new System.Windows.Forms.TextBox();
            this.field2 = new System.Windows.Forms.TextBox();
            this.startProcess = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Input 2";
            // 
            // field1
            // 
            this.field1.Location = new System.Drawing.Point(58, 29);
            this.field1.Name = "field1";
            this.field1.Size = new System.Drawing.Size(263, 20);
            this.field1.TabIndex = 2;
            this.field1.TextChanged += new System.EventHandler(this.field1_TextChanged);
            // 
            // field2
            // 
            this.field2.Location = new System.Drawing.Point(58, 60);
            this.field2.Name = "field2";
            this.field2.Size = new System.Drawing.Size(263, 20);
            this.field2.TabIndex = 3;
            // 
            // startProcess
            // 
            this.startProcess.Location = new System.Drawing.Point(133, 86);
            this.startProcess.Name = "startProcess";
            this.startProcess.Size = new System.Drawing.Size(75, 23);
            this.startProcess.TabIndex = 4;
            this.startProcess.Text = "Analyze";
            this.startProcess.UseVisualStyleBackColor = true;
            this.startProcess.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.Tomato;
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(121, 133);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 5;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 174);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.startProcess);
            this.Controls.Add(this.field2);
            this.Controls.Add(this.field1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "Turkish Contradiction Analysis";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox field1;
        private System.Windows.Forms.TextBox field2;
        private System.Windows.Forms.Button startProcess;
        private System.Windows.Forms.TextBox textBox3;
    }
}

