namespace NCZJDDJFZ.Analysing
{
    partial class Overlay
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.RunOverlay = new System.Windows.Forms.Button();
            this.Exp2XLS = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ExpZD2XLS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RunOverlay
            // 
            this.RunOverlay.Location = new System.Drawing.Point(3, 111);
            this.RunOverlay.Name = "RunOverlay";
            this.RunOverlay.Size = new System.Drawing.Size(111, 23);
            this.RunOverlay.TabIndex = 1;
            this.RunOverlay.Text = "分析";
            this.RunOverlay.UseVisualStyleBackColor = true;
            this.RunOverlay.Click += new System.EventHandler(this.RunOverlay_Click);
            // 
            // Exp2XLS
            // 
            this.Exp2XLS.Location = new System.Drawing.Point(0, 236);
            this.Exp2XLS.Name = "Exp2XLS";
            this.Exp2XLS.Size = new System.Drawing.Size(168, 23);
            this.Exp2XLS.TabIndex = 4;
            this.Exp2XLS.Text = "导出公示用Execl报表";
            this.Exp2XLS.UseVisualStyleBackColor = true;
            this.Exp2XLS.Click += new System.EventHandler(this.Exp2XLS_Click);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.MistyRose;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(3, 3);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(254, 102);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = "1：\r\n进行基本农田叠加分析。\r\n此步骤会分析宗地范围是否侵占基本农田等土地类型的图版，并将侵占的相关面积计算出来写入图形属性表中。\r\n分析完以后，进入XXIOB" +
                "模块中，点击数据入库才能将数据保存到数据库中。";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.MistyRose;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Location = new System.Drawing.Point(3, 151);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(254, 79);
            this.textBox3.TabIndex = 9;
            this.textBox3.Text = "2：\r\n将分析计算后的结果导出至Excel文件中。\r\n此处的导出按钮只将没有侵占基本农田的宗地的相关数据导出。\r\n用于公告.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(3, 289);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "附加：导出所有属性数据至Excel";
            // 
            // ExpZD2XLS
            // 
            this.ExpZD2XLS.Location = new System.Drawing.Point(5, 304);
            this.ExpZD2XLS.Name = "ExpZD2XLS";
            this.ExpZD2XLS.Size = new System.Drawing.Size(227, 23);
            this.ExpZD2XLS.TabIndex = 11;
            this.ExpZD2XLS.Text = "导出所有宗地及房屋属性至Excel";
            this.ExpZD2XLS.UseVisualStyleBackColor = true;
            this.ExpZD2XLS.Click += new System.EventHandler(this.ExpZD2XLS_Click);
            // 
            // Overlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ExpZD2XLS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.Exp2XLS);
            this.Controls.Add(this.RunOverlay);
            this.Name = "Overlay";
            this.Size = new System.Drawing.Size(260, 745);
            this.Load += new System.EventHandler(this.Overlay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RunOverlay;
        private System.Windows.Forms.Button Exp2XLS;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ExpZD2XLS;
    }
}
