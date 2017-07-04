namespace NCZJDDJFZ.DiJitools
{
    partial class CWXX
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ObjectID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Minx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CWSM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DXLX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LW = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.XG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.xz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ObjectID
            // 
            this.ObjectID.HeaderText = "ObjectID";
            this.ObjectID.Name = "ObjectID";
            this.ObjectID.ReadOnly = true;
            this.ObjectID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ObjectID.Visible = false;
            // 
            // MinY
            // 
            this.MinY.HeaderText = "MinY";
            this.MinY.Name = "MinY";
            this.MinY.ReadOnly = true;
            this.MinY.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MinY.Visible = false;
            // 
            // Minx
            // 
            this.Minx.HeaderText = "Minx";
            this.Minx.Name = "Minx";
            this.Minx.ReadOnly = true;
            this.Minx.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Minx.Visible = false;
            // 
            // MaxY
            // 
            this.MaxY.HeaderText = "MaxY";
            this.MaxY.Name = "MaxY";
            this.MaxY.ReadOnly = true;
            this.MaxY.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MaxY.Visible = false;
            // 
            // MaxX
            // 
            this.MaxX.HeaderText = "MaxX";
            this.MaxX.Name = "MaxX";
            this.MaxX.ReadOnly = true;
            this.MaxX.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MaxX.Visible = false;
            // 
            // CWSM
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Red;
            this.CWSM.DefaultCellStyle = dataGridViewCellStyle1;
            this.CWSM.HeaderText = "错误说明";
            this.CWSM.Name = "CWSM";
            this.CWSM.ReadOnly = true;
            this.CWSM.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CWSM.Width = 500;
            // 
            // DXLX
            // 
            this.DXLX.Frozen = true;
            this.DXLX.HeaderText = "对象类型";
            this.DXLX.MinimumWidth = 80;
            this.DXLX.Name = "DXLX";
            this.DXLX.ReadOnly = true;
            this.DXLX.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DXLX.Width = 80;
            // 
            // LW
            // 
            this.LW.Frozen = true;
            this.LW.HeaderText = "例外";
            this.LW.MinimumWidth = 60;
            this.LW.Name = "LW";
            this.LW.ReadOnly = true;
            this.LW.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.LW.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.LW.Visible = false;
            this.LW.Width = 60;
            // 
            // XG
            // 
            this.XG.Frozen = true;
            this.XG.HeaderText = "修改";
            this.XG.MinimumWidth = 60;
            this.XG.Name = "XG";
            this.XG.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.XG.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.XG.Width = 60;
            // 
            // xz
            // 
            this.xz.Frozen = true;
            this.xz.HeaderText = "性质";
            this.xz.MinimumWidth = 60;
            this.xz.Name = "xz";
            this.xz.ReadOnly = true;
            this.xz.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.xz.Width = 60;
            // 
            // XH
            // 
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle2.Format = "N0";
            dataGridViewCellStyle2.NullValue = null;
            this.XH.DefaultCellStyle = dataGridViewCellStyle2;
            this.XH.Frozen = true;
            this.XH.HeaderText = "序号";
            this.XH.MinimumWidth = 60;
            this.XH.Name = "XH";
            this.XH.ReadOnly = true;
            this.XH.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.XH.Width = 60;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.XH,
            this.xz,
            this.XG,
            this.LW,
            this.DXLX,
            this.CWSM,
            this.MaxX,
            this.MaxY,
            this.Minx,
            this.MinY,
            this.ObjectID});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(521, 143);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            // 
            // CWXX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.dataGridView1);
            this.Name = "CWXX";
            this.Size = new System.Drawing.Size(521, 143);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public  System.Windows.Forms.DataGridViewTextBoxColumn ObjectID;
        public System.Windows.Forms.DataGridViewTextBoxColumn MinY;
        public System.Windows.Forms.DataGridViewTextBoxColumn Minx;
        public System.Windows.Forms.DataGridViewTextBoxColumn MaxY;
        public System.Windows.Forms.DataGridViewTextBoxColumn MaxX;
        public System.Windows.Forms.DataGridViewTextBoxColumn CWSM;
        public System.Windows.Forms.DataGridViewTextBoxColumn DXLX;
        public System.Windows.Forms.DataGridViewCheckBoxColumn LW;
        public System.Windows.Forms.DataGridViewCheckBoxColumn XG;
        public System.Windows.Forms.DataGridViewTextBoxColumn xz;
        public System.Windows.Forms.DataGridViewTextBoxColumn XH;
        public System.Windows.Forms.DataGridView dataGridView1;

    }
}
