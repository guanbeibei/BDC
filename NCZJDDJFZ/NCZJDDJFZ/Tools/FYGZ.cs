using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Windows.Forms;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using ACDBPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace NCZJDDJFZ.Tools
{
    public partial class FYGZ : Form
    {
        public FYGZ()
        {
            InitializeComponent();
        }

        private void FYGZ_Load(object sender, EventArgs e)
        {
            string zdjz = Tools.ReadWriteReg.read_reg("自动纠正为矩形");
            string scyyxt = Tools.ReadWriteReg.read_reg("删除原有线条");

            if (scyyxt == "")
            {
                SCYYXT.Checked = true;
            }
            else
            {
                if (scyyxt == "1")
                {
                    SCYYXT.Checked = true;
                }
                else
                {
                    SCYYXT.Checked = false;
                }
            }
        }

        // “选择”按钮
        private void button1_Click(object sender, EventArgs e)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 条件判断
                if (Information.IsNumeric(CD.Text.Trim()) == false)
                {
                    MessageBox.Show("长度不是由数字组成的", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                if (Information.IsNumeric(KD.Text.Trim()) == false)
                {
                    MessageBox.Show("宽度不是由数字组成的", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                //if (JG.Text.Trim() == "")
                //{
                //    MessageBox.Show("房屋结构不能为空", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                //if (Information.IsNumeric(CS.Text.Trim()) == false)
                //{
                //    MessageBox.Show("层数不是由数字组成的", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    return;
                //}
                
                #endregion
                #region 数据准备
                double cd = Convert.ToDouble(CD.Text.Trim());
                double kd = Convert.ToDouble(KD.Text.Trim());
                if (cd < kd)
                {
                    MessageBox.Show("长度不能小于宽度。", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                bool issc = SCYYXT.Checked;
                //string jg = JG.Text.Trim();
                //int cs = Convert.ToInt32(CS.Text.Trim());
                if (SCYYXT.Checked)
                {
                    Tools.ReadWriteReg.write_reg("删除原有线条", "1");
                }
                else
                {
                    Tools.ReadWriteReg.write_reg("删除原有线条", "0");
                }
                this.Close();

                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[2];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                filList[1] = new TypedValue(8, "*");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res;
                res = ed.GetSelection(filter);
                SelectionSet SS = res.Value;
                if (SS == null)
                {
                    MessageBox.Show("没有选择到线。", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                ObjectId[] objectId = SS.GetObjectIds();//界址线
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    ACDBPolyline fwx = (ACDBPolyline)trans.GetObject(objectId[0], ACDBOpenMode.ForWrite);
                    #region  如果多段线首尾小于1mm则视为封闭多段线
                    int DDS = fwx.NumberOfVertices;
                    Point2d qd = fwx.GetPoint2dAt(0);
                    Point2d md = fwx.GetPoint2dAt(DDS - 1);
                    if (qd.GetDistanceTo(md) < 0.001)
                    {
                        fwx.RemoveVertexAt(DDS - 1);
                        fwx.Closed = true;
                    }
                    DDS = fwx.NumberOfVertices;
                    if (DDS != 4)
                    {
                        MessageBox.Show("被选择的房屋线不是四个点。", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        trans.Commit();
                        trans.Dispose();
                        return;
                    }
                    #endregion
                    #region 使界址线按顺时针方向
                    ArrayList zb = new ArrayList();
                    double JD = 0;
                    for (int j = 0; j < DDS; j++)
                    {
                        Point2d D0 = new Point2d();
                        Point2d D1 = new Point2d();
                        Point2d D2 = new Point2d();
                        zb.Add(fwx.GetPoint2dAt(j));
                        if (j == 0)
                        {
                            D0 = fwx.GetPoint2dAt(DDS - 1);
                            D1 = fwx.GetPoint2dAt(0);
                            D2 = fwx.GetPoint2dAt(1);
                        }
                        if ((j >= 1) && (j < DDS - 1))
                        {
                            D0 = fwx.GetPoint2dAt(j - 1);
                            D1 = fwx.GetPoint2dAt(j);
                            D2 = fwx.GetPoint2dAt(j + 1);
                        }
                        if (j == DDS - 1)
                        {
                            D0 = fwx.GetPoint2dAt(j - 1);
                            D1 = fwx.GetPoint2dAt(j);
                            D2 = fwx.GetPoint2dAt(0);
                        }
                        double JD0, JD1, JD2;
                        JD1 = Tools.CADTools.GetAngle(D1, D0);
                        JD2 = Tools.CADTools.GetAngle(D1, D2);
                        JD0 = JD2 - JD1;
                        if (JD0 < 0) { JD0 = JD0 + Math.PI * 2; }
                        JD = JD + JD0;
                    }
                    if (JD >= (DDS - 2) * Math.PI + 0.0001)
                    {
                        for (int p = 0; p < DDS; p++)
                        {
                            fwx.SetPointAt(p, (Point2d)zb[DDS - p - 1]);
                        }
                    }
                    #endregion
                    #region 自动纠正
                    double zcb = -1.0;
                    int dyzcbxge1 = 0;//第一条最长边的第一个点
                    for (int i = 0; i < DDS; i++)
                    {
                        int nn = i + 1;
                        if (nn == 4)
                        {
                            nn = 0;
                        }
                        double n_zcb = fwx.GetPoint2dAt(i).GetDistanceTo(fwx.GetPoint2dAt(nn));

                        if (n_zcb > zcb)
                        {
                            dyzcbxge1 = i;
                            zcb = n_zcb;
                        }
                    }
                    int dyzcbxge2 = dyzcbxge1 + 1;//第一条最长边的下个点
                    if (dyzcbxge2 == 4)
                    {
                        dyzcbxge2 = 0;
                    }
                    int drtzcb1 = dyzcbxge1 + 2;//对边的第一个点
                    if (drtzcb1 >= 4)
                    {
                        drtzcb1 = drtzcb1 - 4;
                    }
                    int drtzcb2 = drtzcb1 + 1;//对边的第二个点
                    if (drtzcb2 == 4)
                    {
                        drtzcb2 = 0;
                    }
                    double jd1 = Tools.CADTools.GetAngle(fwx.GetPoint2dAt(dyzcbxge1), fwx.GetPoint2dAt(dyzcbxge2));
                    double jd2 = Tools.CADTools.GetAngle(fwx.GetPoint2dAt(drtzcb2), fwx.GetPoint2dAt(drtzcb1));
                    if (Math.Abs(jd1 - jd2) > Math.PI / 3.0)
                    {
                        jd2 = Math.Abs(Math.PI * 2.0 - jd2);
                    }
                    double jd = (jd1 + jd2) / 2.0;
                    double s1 = fwx.GetPoint2dAt(dyzcbxge1).GetDistanceTo(fwx.GetPoint2dAt(dyzcbxge2));
                    double s3 = fwx.GetPoint2dAt(drtzcb1).GetDistanceTo(fwx.GetPoint2dAt(drtzcb2));

                    double X = (fwx.GetPoint2dAt(0).X + fwx.GetPoint2dAt(1).X + fwx.GetPoint2dAt(2).X + fwx.GetPoint2dAt(3).X) / 4.0;
                    double Y = (fwx.GetPoint2dAt(0).Y + fwx.GetPoint2dAt(1).Y + fwx.GetPoint2dAt(2).Y + fwx.GetPoint2dAt(3).Y) / 4.0;
                    Point2d cen = new Point2d(X, Y);
                    Point2d dd = Tools.CADTools.GetNewPoint(cen, cd / 2.0, jd);
                    Point2d d1 = Tools.CADTools.GetNewPoint(dd, kd / 2.0, jd + Math.PI / 2.0);
                    Point2d d2 = Tools.CADTools.GetNewPoint(d1, cd, jd + Math.PI);
                    Point2d d3 = Tools.CADTools.GetNewPoint(d2, kd, jd + Math.PI + Math.PI / 2.0);
                    Point2d d4 = Tools.CADTools.GetNewPoint(d3, cd, jd);
                    ACDBPolyline fwx2 = new ACDBPolyline();
                    if (issc)
                    {
                        fwx.Erase();
                    }
                    fwx2.AddVertexAt(0, d1, 0, 0, 0);
                    fwx2.AddVertexAt(1, d2, 0, 0, 0);
                    fwx2.AddVertexAt(2, d3, 0, 0, 0);
                    fwx2.AddVertexAt(3, d4, 0, 0, 0);
                    btr.AppendEntity(fwx2);
                    trans.AddNewlyCreatedDBObject(fwx2, true);
                    #region 修改房屋线
                    fwx2.ColorIndex = 1;
                    fwx2.Closed = true;
                    #endregion
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                }


                
            }
        }

        // “取消”按钮
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
