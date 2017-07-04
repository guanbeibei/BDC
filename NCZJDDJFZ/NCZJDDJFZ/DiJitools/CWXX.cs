using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Drawing;
using System.Windows.Forms;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;

namespace NCZJDDJFZ.DiJitools
{
    public partial class CWXX : UserControl
    {
        public CWXX()
        {
            InitializeComponent();
        }
        Entity StarEntity = null;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.EndEdit();
            if (e.RowIndex >= 0)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[e.RowIndex].Cells["XG"].Value) == true)
                {
                    dataGridView1.Rows[e.RowIndex].Cells["LW"].Value = false;
                    DataGridViewCellStyle highlightCellStyle = new DataGridViewCellStyle();
                    highlightCellStyle.Font = new System.Drawing.Font("宋体", 9, FontStyle.Strikeout);
                    highlightCellStyle.ForeColor = System.Drawing.Color.Black;
                    dataGridView1.Rows[e.RowIndex].Cells["CWSM"].Style = highlightCellStyle;
                }
                else
                {
                    DataGridViewCellStyle highlightCellStyle = new DataGridViewCellStyle();
                    highlightCellStyle.Font = new System.Drawing.Font("宋体", 9);
                    highlightCellStyle.ForeColor = System.Drawing.Color.Red;
                    dataGridView1.Rows[e.RowIndex].Cells["CWSM"].Style = highlightCellStyle;
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (StarEntity != null)
            {
                try
                {
                    StarEntity.Unhighlight();
                }
                catch
                {
                }
            }
            double minx = (double)dataGridView1.Rows[e.RowIndex].Cells["Minx"].Value;
            double miny = (double)dataGridView1.Rows[e.RowIndex].Cells["MinY"].Value;
            double maxx = (double)dataGridView1.Rows[e.RowIndex].Cells["MaxX"].Value;
            double maxy = (double)dataGridView1.Rows[e.RowIndex].Cells["MaxY"].Value;
            string jb = dataGridView1.Rows[e.RowIndex].Cells["ObjectID"].Value.ToString().Trim();
            string zxjzb = minx.ToString("0.00") + "," + miny.ToString("0.00");
            string ysjzb = maxx.ToString("0.00") + "," + maxy.ToString("0.00");
            AcadApplication.DocumentManager.MdiActiveDocument.SendStringToExecute("zoom\n" + "w\n"
                   + zxjzb + "\n" + ysjzb + "\n", false, false, false);
            Entity cadENT;
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite, true);
                    try
                    {
                        cadENT = (Entity)trans.GetObject(Tools.CADTools.HandletoObjectID(jb), AcadOpenMode.ForWrite);
                        cadENT.Highlight();
                        StarEntity = cadENT;
                    }
                    catch
                    {
                        MessageBox.Show("对象已删除！", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        
    }
}
