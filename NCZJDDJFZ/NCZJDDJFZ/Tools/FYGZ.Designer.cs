namespace NCZJDDJFZ.Tools
{
    partial class FYGZ
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.CD = new System.Windows.Forms.TextBox();
            this.KD = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SCYYXT = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "长度:";
            // 
            // CD
            // 
            this.CD.Location = new System.Drawing.Point(112, 39);
            this.CD.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CD.Name = "CD";
            this.CD.Size = new System.Drawing.Size(105, 25);
            this.CD.TabIndex = 1;
            // 
            // KD
            // 
            this.KD.Location = new System.Drawing.Point(112, 91);
            this.KD.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.KD.Name = "KD";
            this.KD.Size = new System.Drawing.Size(105, 25);
            this.KD.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 95);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "宽度:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(293, 39);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 42);
            this.button1.TabIndex = 9;
            this.button1.Text = "选  择";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(293, 165);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 42);
            this.button2.TabIndex = 10;
            this.button2.Text = "取  消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(57, 192);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(195, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "注意:此功能只能纠正四点房";
            // 
            // SCYYXT
            // 
            this.SCYYXT.AutoSize = true;
            this.SCYYXT.Checked = true;
            this.SCYYXT.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SCYYXT.Location = new System.Drawing.Point(60, 141);
            this.SCYYXT.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.SCYYXT.Name = "SCYYXT";
            this.SCYYXT.Size = new System.Drawing.Size(119, 19);
            this.SCYYXT.TabIndex = 11;
            this.SCYYXT.Text = "删除原有线条";
            this.SCYYXT.UseVisualStyleBackColor = true;
            // 
            // FYGZ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 250);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SCYYXT);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.KD);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CD);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FYGZ";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "四点房纠正";
            this.Load += new System.EventHandler(this.FYGZ_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CD;
        private System.Windows.Forms.TextBox KD;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox SCYYXT;
    }
}