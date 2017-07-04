using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.ObjectData;
using JiHeJiSuanTool;
using Microsoft.VisualBasic;
using NCZJDDJFZ.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Topology.Geometries;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using ACDBPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
namespace NCZJDDJFZ.DiJitools
{
    

    public partial class Dijicltools : UserControl
    {
        public struct zdhANDpl
        {
            public string zdh;
            public ACDBPolyline pline;
        }
        /// <summary>
        /// 错误信息列表结构
        /// </summary>
        public struct err 
        {
            public int xh;
            public string xz;
            public Boolean xg;
            public Boolean lw;
            public string dxlx;
            public string cwsm;
            public double maxX, maxy, minx, miny;
            public string jb;
        }
        public ArrayList arrerr = new ArrayList();
        public ArrayList arrtext = new ArrayList();

        Database db = HostApplicationServices.WorkingDatabase;
        Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
        public Dijicltools()
        {
            InitializeComponent();
        }
        
        /// <summary>删除重点、微短线和回头线
        /// 删除重点、微短线和回头线
        /// </summary>
        /// <param name="IsXSDHK">是否弹出对话框</param>
        public void DEL_WDX(bool IsXSDHK)
        {
           
            int zxs = 0;
            int wdx = 0;
            int cd = 0;
            int htx = 0;
            int xh = 0;
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.Regen();
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                // #region 创建选择集获取多段线数据
                TypedValue[] filList = new TypedValue[1];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null) { return; }
                ObjectId[] objectId = SS.GetObjectIds();
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForWrite);
                    //System.Windows.Forms.Application.DoEvents();

                    #region 显示进度条
                    ProgressMeter progressmeter = new ProgressMeter();
                    progressmeter.Start("正在删除重点、微短线和回头线...");
                    progressmeter.SetLimit(objectId.Length);
                    zxs = objectId.Length;
                    #endregion
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        progressmeter.MeterProgress();
                        ACDBPolyline JZX;
                        JZX = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        //Extents3d box = (Extents3d)JZX.Bounds;
                        //MessageBox.Show(box.MaxPoint.X.ToString());
                        double jd = Math.PI / 2.0;
                        int DDS = JZX.NumberOfVertices;
                        #region 如果多段线长度小于5mm则删除
                        if (JZX.Length < 0.005 || DDS <= 1)
                        {
                            wdx++;
                            JZX.Erase();
                            continue;
                        }
                        #endregion 如果多段线长度小于5mm则删除
                        #region  如果多段线首尾小于1mm则视为封闭多段线
                        Point2d qd = JZX.GetPoint2dAt(0);
                        Point2d md = JZX.GetPoint2dAt(DDS - 1);
                        if (qd.GetDistanceTo(md) < 0.001) { JZX.Closed = true; }
                        for (int j = 0; j < DDS; j++)
                        {
                            JZX.SetPointAt(j, new Point2d((long)(10000.0 * JZX.GetPoint2dAt(j).X + 0.5) / 10000.0, (long)(10000.0 * JZX.GetPoint2dAt(j).Y + 0.5) / 10000.0));
                        }
                        #endregion
                        #region 删除重复点和回头线
                        try
                        {
                            for (int j = 0; j < DDS; j++)
                            {
                                Point2d Dxy1 = JZX.GetPoint2dAt(j);
                                Point2d Dxy2 = new Point2d();
                                if (JZX.Closed == false && j == DDS - 1)
                                {
                                    break;
                                }
                                if (JZX.Closed == true && j == DDS - 1)
                                {
                                    Dxy2 = JZX.GetPoint2dAt(0);
                                }
                                if (j < DDS - 1)
                                {
                                    Dxy2 = JZX.GetPoint2dAt(j + 1);
                                }
                                if (j > 0)
                                {
                                    Point2d Dxy0 = JZX.GetPoint2dAt(j - 1);
                                    JiheTool jh = new JiheTool();
                                    jd = jh.JiSuanJiaoDu(new MyPoint(Dxy1.X, Dxy1.Y), new MyPoint(Dxy0.X, Dxy0.Y), new MyPoint(Dxy2.X, Dxy2.Y));
                                }
                                double s = Dxy1.GetDistanceTo(Dxy2);
                                if (s < 0.005)
                                {
                                    if (DDS <= 2)
                                    {
                                        JZX.Erase();
                                        break;
                                    }
                                    JZX.RemoveVertexAt(j);
                                    DDS = JZX.NumberOfVertices;
                                    j = j - 1;
                                    cd++;
                                }
                                if (s >= 0.005 && Math.Abs(jd) < 0.0001)
                                {
                                    if (DDS <= 2)
                                    {
                                        JZX.Erase();
                                        break;
                                    }
                                    JZX.RemoveVertexAt(j + 1);
                                    DDS = JZX.NumberOfVertices;
                                    j = j - 1;
                                    htx++;
                                }
                            }
                        }
                        catch (System.Exception)
                        {
                            Extents3d JZXBounds = (Extents3d)JZX.Bounds;
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "多段线";
                            er.cwsm = "多段线可能存在弧线段，请手工处理。";
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = JZX.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        #endregion
                    }
                    trans.Commit();
                    trans.Dispose();
                    Class1.WritErr(arrerr);
                    progressmeter.Stop();
                }
            }
            if (IsXSDHK)
            {
                MessageBox.Show("总共处理了 " + zxs.ToString() + " 条多段线" + "\n" +
                               "共删除微短线 " + wdx.ToString() + " 条。" + "\n" +
                               "共删除重点 " + cd.ToString() + " 个。" + "\n" +
                               "共清理回头线 " + htx.ToString() + " 处。",
                               "地籍处理工具", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // “删除重点、微短线”按钮
        /// <summary>
        /// 线处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CLX_Click(object sender, EventArgs e)
        {
            arrerr.Clear();
            arrtext.Clear();
            DEL_WDX(true);
           
        }
        /// <summary>
        /// 宗地属性和界址线的检查和处理
        /// </summary>
        public void  zdjc(double rc)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择数据字典节点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return  ;
            }
            if (treeView1.SelectedNode.Level != 4)
            {
                MessageBox.Show("必须选择到行政村节点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return  ;
            }
            arrerr.Clear();
            arrtext.Clear();
            ArrayList ZJFH = new ArrayList();
            int xh = 0;
            string DJH = treeView1.SelectedNode.Tag.ToString();
            string JDH = DJH.Substring(6, 3);
            string JFH = DJH.Substring(9, 3);
            ArrayList pologylist = new ArrayList();
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 创建选择集获取界址线数据
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[2];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null) { return; }
                ObjectId[] objectId = SS.GetObjectIds();
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    ProgressMeter progressmeter = new ProgressMeter();
                    progressmeter.Start("正在检查宗地...");
                    progressmeter.SetLimit(objectId.Length);
                    arrerr.Clear();
                    arrtext.Clear();
                    for (int i = 0; i < objectId.Length; i++)
                    {

                        progressmeter.MeterProgress();
                        ACDBPolyline JZX;
                        JZX = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        int DDS = JZX.NumberOfVertices;
                        Extents3d JZXBounds = (Extents3d)JZX.Bounds;
                        ResultBuffer resultBuffer = JZX.XData;
                        try
                        {
                            #region 无属性
                            if (resultBuffer == null)//无属性
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "界址线无属性。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            //TypedValue[] tv3 = resultBuffer.AsArray();
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = GetApp(tv2, "SOUTH");
                            if (tv3[1].Value.ToString().Trim() != "300000") { continue; }//如果是街道界
                            #region 界址线属性丢失
                            if (tv3.Length < 5)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "界址线属性丢失。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 检查是否封闭、宗地面积过小、界址线节点数太少
                            if (JZX.Closed != true)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "界址线不封闭。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            if (JZX.Area < 4.0)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "宗地面积过小，不合常理。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            if (DDS < 3)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "界址线节点数太少。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
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
                                zb.Add(JZX.GetPoint2dAt(j));
                                if (j == 0)
                                {
                                    D0 = JZX.GetPoint2dAt(DDS - 1);
                                    D1 = JZX.GetPoint2dAt(0);
                                    D2 = JZX.GetPoint2dAt(1);
                                }
                                if ((j >= 1) && (j < DDS - 1))
                                {
                                    D0 = JZX.GetPoint2dAt(j - 1);
                                    D1 = JZX.GetPoint2dAt(j);
                                    D2 = JZX.GetPoint2dAt(j + 1);
                                }
                                if (j == DDS - 1)
                                {
                                    D0 = JZX.GetPoint2dAt(j - 1);
                                    D1 = JZX.GetPoint2dAt(j);
                                    D2 = JZX.GetPoint2dAt(0);
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
                                    JZX.SetPointAt(p, (Point2d)zb[DDS - p - 1]);
                                }
                            }
                            #endregion
                            #region 获取宗地信息
                            string jfh;
                            string jdh;
                            string zdh;
                            string djh;
                            string tdyt;
                            djh = tv3[2].Value.ToString();
                            tdyt = tv3[4].Value.ToString().Trim();
                            jdh = djh.Substring(0, 3);
                            jfh = djh.Substring(3, 3);
                            #endregion
                            #region 地类代码不能为空
                            if (tdyt == "")//地类代码不能为空
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地类代码不能为空。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 地籍号不是由数字组成的
                            if (Information.IsNumeric(djh) == false)//地籍号不是由数字组成的
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地籍号\"" + jdh + "\"不是由数字组成的。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 地类代码不是由数字组成的
                            if (Information.IsNumeric(tdyt) == false)//地类代码不是由数字组成的
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地类代码\"" + tdyt + "\"不是由数字组成的。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 二调中不存在的地类代码
                            string[] dldmlist ={"011", "012", "013", "021", "022", "023", "031", "032", "033", "041", "042", "043",
                               "051","052","053","054","061","062","063","071","072","081","082","083","084","085","086","087","088",
                               "091","092","093","094","095",
                               "101", "102","103", "104", "105", "106", "107", "111", "112", "113", "114", "115", "116", 
                               "117", "118", "119","121", "122", "123", "124", "125", "126", "127"};
                            bool Iszhq = false;
                            for (int j = 0; j < dldmlist.Length; j++)
                            {
                                char a = '0';
                                tdyt = tdyt.PadLeft(3, a);
                                if (tdyt == dldmlist[j]) { Iszhq = true; break; }
                            }

                            if (Iszhq == false)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "二次调查规程中不存在" + tdyt + "这样的地类代码。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 街道号错误
                            if (JDH != jdh)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地籍区号不正确(即街道号)。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 街坊号错误
                            if (JFH != jfh)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地籍子区号不正确(即街坊号)。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 地籍号长度错误
                            if (djh.Length < 7 || djh.Length > 11)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地籍号位数太多或太少。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 宗地号重号
                            zdh = djh.Substring(6, djh.Length - 6);
                            zdh = zdh.PadLeft(5, '0');//将宗地号变为标准宗地号
                            string BZDJH = DJH.Substring(6, 6) + zdh;
                            bool Isfind = false;
                            for (int j = 0; j < ZJFH.Count; j++)//如果街坊号有重号
                            {
                                if (BZDJH == ZJFH[j].ToString())
                                {
                                    xh++;
                                    err er = new err();
                                    er.xh = xh;
                                    er.xz = "必须修改";
                                    er.dxlx = "界址线";
                                    er.cwsm = "宗地号重号。";
                                    er.minx = JZXBounds.MinPoint.X;
                                    er.miny = JZXBounds.MinPoint.Y;
                                    er.maxX = JZXBounds.MaxPoint.X;
                                    er.maxy = JZXBounds.MaxPoint.Y;
                                    er.jb = JZX.Handle.Value.ToString();
                                    arrerr.Add(er);
                                    Isfind = true;
                                    break;
                                }
                            }
                            if (Isfind == false)
                            {
                                ZJFH.Add(BZDJH);
                            }
                            #endregion
                            Topology.Geometries.Polygon polygon = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(JZX);
                            polygon.UserData = JZX.Handle.Value.ToString().Trim();
                            pologylist.Add(polygon);
                        }
                        #region 未知错误
                        catch (System.Exception cw)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "界址线";
                            er.cwsm = cw.Message;
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = JZX.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        #endregion
                    }
                    progressmeter.Stop();
                    ProgressMeter progressmeter2 = new ProgressMeter();
                    progressmeter2.Start("正在检查悬挂线...");
                    progressmeter2.SetLimit(objectId.Length);
                    #region 检查悬挂点和界址点间距过小
                    Tools.CADTools.AddNewLayer("错误标记", "Continuous", 1);
                    for (int i = 0; i < pologylist.Count; i++)
                    {
                        progressmeter2.MeterProgress();
                        Topology.Geometries.Polygon Polygon1 = (Topology.Geometries.Polygon)pologylist[i];
                        for (int j = i + 1; j < pologylist.Count; j++)
                        {
                            Topology.Geometries.Polygon Polygon2 = (Topology.Geometries.Polygon)pologylist[j];
                            if (!Polygon2.Intersects(Polygon1)) { continue; }//如果连个多边形没有交点结束本次循环
                            ArrayList pointarr = Tools.Uitl.JZXFX(Polygon1, Polygon2, rc);//分析在两个宗地缓冲区内的且不在节点上的点
                            for (int p = 0; p < pointarr.Count; p++)
                            {
                                Topology.Geometries.Point point = (Topology.Geometries.Point)pointarr[p];
                                Circle circle = new Circle();
                                circle.Center = new Point3d(point.X, point.Y, 0);
                                circle.Radius = 2;
                                circle.ColorIndex = 5;
                                circle.Layer = "错误标记";
                                circle.LineWeight = LineWeight.LineWeight030;
                                btr.AppendEntity(circle);
                                trans.AddNewlyCreatedDBObject(circle, true);
                                Topology.Geometries.Polygon polygon = null;
                                if (point.UserData.ToString() == "1")
                                {
                                    polygon = Polygon1;
                                }
                                else
                                {
                                    polygon = Polygon2;
                                }
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "圈圈处节点被相邻宗地包含或到相邻宗地距离太近。";
                                er.minx = polygon.EnvelopeInternal.MinX;
                                er.miny = polygon.EnvelopeInternal.MinY;
                                er.maxX = polygon.EnvelopeInternal.MaxX;
                                er.maxy = polygon.EnvelopeInternal.MaxY;
                                er.jb = polygon.UserData.ToString();
                                arrerr.Add(er);
                            }
                        }
                    }
                    progressmeter2.Stop();
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }

        // “界址线检查修改”按钮
        /// <summary>
        /// 宗地检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            double rc=0.18;
            if(!Information.IsNumeric(this.textBox2.Text.Trim()))
            {
                MessageBox.Show("本宗地与相邻宗地界址点最小间距不是由数字", "地籍工具", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                return;
            }
            rc=Convert.ToDouble(this.textBox2.Text.Trim());
            DelCWBJ();
            zdjc(rc);
            Class1.WritErr(arrerr);
        }
        private void Dijicltools_Load(object sender, EventArgs e)
        {
            
            string connectionString = Uitl.LJstring();//写连接字符串
            string selstring = "SELECT * FROM DataDic";

            if (Tools.DataBasic.Create_Dic_table(connectionString))
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlDataAdapter Adapter = DataBasic.Dic_Initadapter(selstring, connectionString);
                DataSet dataset = new DataSet();
                Adapter.Fill(dataset, "DataDic");//填充Dataset
                System.Data.DataTable TB = dataset.Tables["DataDic"];

                Tools.Uitl uitl = new Uitl();
                uitl.DrowTree(this.treeView1, TB);//画节点树
                connection.Close();
            }
        }
        
        /// <summary>
        /// 计算方位角
        /// </summary>
        /// <param name="DY"></param>
        /// <param name="DX"></param>
        /// <returns></returns>
        private double FWJ(double DY, double DX)
        {
            double t = 0;
            if (DX == 0.0)
            {
                t = Math.Sign(DX) * Math.PI / 2.0;
            }
            else
            {
                t = Math.Atan(Math.Abs(DY) / Math.Abs(DX));
                if (DX < 0 && DY >= 0) { t = Math.PI - t; }
                if (DX < 0 && DY < 0) { t = Math.PI + t; }
                if (DX > 0 && DY < 0) { t = Math.PI * 2 - t; }
            }
            return t;
        }

        // “界址线起点纠正”按钮
        /// <summary>
        /// 修改界址线起点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 创建选择集获取界址线数据
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[2];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null) { return; }
                ObjectId[] objectId = SS.GetObjectIds();
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    for (int p = 0; p < objectId.Length; p++)
                    {
                        ACDBPolyline JZX = (ACDBPolyline)trans.GetObject(objectId[p], ACDBOpenMode.ForWrite);
                        int DDS = JZX.NumberOfVertices;

                        int RowLeng = DDS;
                        int[] JZDH = new int[RowLeng + 1];
                        Double[] JZDX = new Double[RowLeng + 1];
                        Double[] JZDY = new Double[RowLeng + 1];
                        Double[] JZDJL = new Double[RowLeng + 1];
                        for (int i = 0; i < RowLeng; i++)
                        {
                            JZDH[i] = i + 1;
                            JZDX[i] = JZX.GetPoint2dAt(i).X;
                            JZDY[i] = JZX.GetPoint2dAt(i).Y;
                        }

                        ////////////////////分析东南西北四个方向/////////////////////////////
                        JZDX[RowLeng] = JZDX[0];
                        JZDY[RowLeng] = JZDY[0];
                        for (int i = 0; i < RowLeng; i++)
                        {
                            JZDJL[i] = Math.Sqrt((JZDX[i] - JZDX[i + 1]) * (JZDX[i] - JZDX[i + 1]) +
                                                (JZDY[i] - JZDY[i + 1]) * (JZDY[i] - JZDY[i + 1]));
                        }
                        double Maxs = -1;
                        int Maxjzdh = 1;
                        for (int i = 0; i < RowLeng; i++)
                        {
                            if (Maxs < JZDJL[i])
                            {
                                Maxs = JZDJL[i];
                                Maxjzdh = JZDH[i];
                            }
                        }
                        double Maxjzdx1 = JZDX[Maxjzdh - 1];
                        double Maxjzdy1 = JZDY[Maxjzdh - 1];
                        double Maxjzdx2 = JZDX[Maxjzdh];
                        double Maxjzdy2 = JZDY[Maxjzdh];

                        double DX0 = Math.Atan(Math.Abs(Maxjzdx2 - Maxjzdx1) / Math.Abs(Maxjzdy2 - Maxjzdy1 + 0.0000000001));
                        if (Maxjzdy1 < Maxjzdy2 && Maxjzdx1 > Maxjzdx2) { DX0 = Math.PI / 2.0 - DX0; }
                        if (Maxjzdy1 > Maxjzdy2 && Maxjzdx1 < Maxjzdx2) { DX0 = Math.PI / 2.0 - DX0; }
                        double[] XX1 = new double[RowLeng + 1];
                        double[] YY1 = new double[RowLeng + 1];
                        double[] XX2 = new double[RowLeng + 1];
                        double[] YY2 = new double[RowLeng + 1];

                        for (int i = 0; i < RowLeng; i++)
                        {
                            XX1[i] = Math.Sqrt(JZDX[i] * JZDX[i] + JZDY[i] * JZDY[i])
                                   * Math.Cos(FWJ(JZDX[i], JZDY[i]) - DX0);
                            YY1[i] = Math.Sqrt(JZDX[i] * JZDX[i] + JZDY[i] * JZDY[i])
                                  * Math.Sin(FWJ(JZDX[i], JZDY[i]) - DX0);
                        }


                        double dx2 = -1 * (Math.PI / 2.0 - DX0);
                        for (int i = 0; i < RowLeng; i++)
                        {
                            XX2[i] = Math.Sqrt(JZDX[i] * JZDX[i] + JZDY[i] * JZDY[i])
                                  * Math.Cos(FWJ(JZDX[i], JZDY[i]) - dx2);
                            YY2[i] = Math.Sqrt(JZDX[i] * JZDX[i] + JZDY[i] * JZDY[i])
                                  * Math.Sin(FWJ(JZDX[i], JZDY[i]) - dx2);
                        }
                        int[] JZDn = new int[4];
                        double maxXY = 999999999999999.0;
                        int N = 0;
                        for (int i = 0; i <= RowLeng - 1; i++)
                        {
                            if (maxXY > JZDX[i] - JZDY[i])
                            {
                                maxXY = JZDX[i] - JZDY[i];
                                JZDn[0] = JZDH[i];
                                N = i;
                            }
                        }
                        #region 修改起点
                        if (N > 0)
                        {
                            ArrayList arr = new ArrayList();
                            for (int i = 0; i < JZX.NumberOfVertices; i++)
                            {
                                arr.Add(JZX.GetPoint2dAt(i));
                            }
                            for (int i = 0; i < JZX.NumberOfVertices - N; i++)
                            {
                                JZX.SetPointAt(i, (Point2d)arr[N + i]);
                            }

                            for (int i = JZX.NumberOfVertices - N; i < JZX.NumberOfVertices; i++)
                            {
                                JZX.SetPointAt(i, (Point2d)arr[i - JZX.NumberOfVertices + N]);
                            }
                        }
                        #endregion

                    }
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            BM.Text = "行政代码：" + e.Node.Tag.ToString();
        }

        // “生成界址点”按钮
        /// <summary>
        /// 重新生成界址点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            #region 条件
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择数据字典节点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (treeView1.SelectedNode.Level != 4)
            {
                MessageBox.Show("必须选择到行政村节点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            #endregion
            int zdjzdh = 0;//最大界址点号
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Tools.CADTools.AddNewLayer("JZP", "Continuous", 1);
                #region 创建选择集获取界址线、界址点数据
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[2];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null) { return; }
                ObjectId[] objectId = SS.GetObjectIds();
                TypedValue[] filList_jzd = new TypedValue[2];
                filList_jzd[0] = new TypedValue((int)DxfCode.Start, "CIRCLE");
                filList_jzd[1] = new TypedValue(8, "JZP");
                SelectionFilter filter_jzd = new SelectionFilter(filList_jzd);
                PromptSelectionResult res_jzd = ed.SelectAll(filter_jzd);
                SelectionSet SS_jzd = res_jzd.Value;
                ObjectId[] objectId_jzd = new ObjectId[0];
                if (SS_jzd != null)
                {
                    objectId_jzd = SS_jzd.GetObjectIds();
                }
                #endregion
                List<Point2d> JZDZB = new List<Point2d>();//界址点集合
                #region 获取最大界址点号同时删除不是本软件生成的界址点
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    for (int i = 0; i < objectId_jzd.Length; i++)
                    {
                        Circle circle = (Circle)trans.GetObject(objectId_jzd[i], ACDBOpenMode.ForWrite);
                        ResultBuffer resultBuffer = circle.XData;
                        TypedValue[] tv2 = resultBuffer.AsArray();
                        TypedValue[] tv3 = GetApp(tv2, "SOUTH");
                        if (tv3.Length == 5 && tv3[4].Value.ToString() == "BDC")
                        {
                            int jzdh = (int)tv3[3].Value;
                            if (jzdh > zdjzdh)
                            {
                                zdjzdh = jzdh;
                            }
                            JZDZB.Add(new Point2d(circle.Center.X, circle.Center.Y));
                        }
                        else
                        {
                            circle.Erase();
                        }
                    }
                    trans.Commit();
                    trans.Dispose();
                }
                #endregion
                #region 增加界址点
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    #region 显示进度条
                    ProgressMeter progressmeter = new ProgressMeter();
                    progressmeter.Start("正在生成界址点...");
                    progressmeter.SetLimit(objectId.Length + 2);
                    #endregion
                    ArrayList ZJFH = new ArrayList();
                    progressmeter.MeterProgress();
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        ACDBPolyline JZX;
                        JZX = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        int DDS = JZX.NumberOfVertices;
                        Extents3d JZXBounds = (Extents3d)JZX.Bounds;
                        ResultBuffer resultBuffer = JZX.XData;
                        try
                        {
                            //TypedValue[] tv3 = resultBuffer.AsArray();
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = GetApp(tv2, "SOUTH");
                            if (tv3[1].Value.ToString().Trim() != "300000") { continue; }//如果是街坊界
                            string djh = tv3[2].Value.ToString();
                            string jdh = djh.Substring(0, 3);
                            string jfh = djh.Substring(3, 3);
                            string zdh = djh.Substring(6, djh.Length - 6);
                            zdh = zdh.PadLeft(5, '0');
                            zdhANDpl zdhandpl = new zdhANDpl();
                            zdhandpl.zdh = jdh + jfh + zdh;
                            zdhandpl.pline = JZX;
                            ZJFH.Add(zdhandpl);
                        }
                        catch (System.Exception)
                        {
                            MessageBox.Show("界址线属性有错误，请在第二步修改无问题后在重新生成界址点。", "地籍处理工具", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }
                    Sort_Comparer sort = new Sort_Comparer();
                    ZJFH.Sort(sort);
                    
                    int jzdh =zdjzdh+1 ;
                    for (int m = 0; m < ZJFH.Count; m++)//写界址点号
                    {
                        progressmeter.MeterProgress();
                        zdhANDpl zdhandpl = (zdhANDpl)ZJFH[m];
                        ACDBPolyline JZX = zdhandpl.pline;
                        int DDS = JZX.NumberOfVertices;
                        for (int J = 0; J < DDS; J++)
                        {
                            Point2d point = JZX.GetPoint2dAt(J);
                            bool isY = false;
                            for (int P = 0; P < JZDZB.Count; P++)
                            {
                                if (JZDZB[P].GetDistanceTo(point) < 0.0001)
                                {
                                    isY = true;//有界址点圆圈
                                    break;
                                }
                            }
                            if (isY == false)//没有界址点圆圈
                            {
                                JZDZB.Add(point);
                                Circle circle = new Circle();
                                circle.Center = new Point3d(point.X, point.Y, 0);
                                circle.Layer = "JZP";
                                circle.ColorIndex = 1;
                                circle.Radius = 0.5;
                                btr.AppendEntity(circle);
                                trans.AddNewlyCreatedDBObject(circle, true);
                                TypedValue[] tv = new TypedValue[5];
                                tv[0] = new TypedValue(1001, "SOUTH");
                                tv[1] = new TypedValue(1000, "301000");
                                tv[2] = new TypedValue(1000, "");
                                tv[3] = new TypedValue(1071, jzdh);
                                tv[4] = new TypedValue(1000, "BDC");
                                jzdh = jzdh + 1;
                                circle.XData = new ResultBuffer(tv);
                            }
                        }
                    }
                    trans.Commit();
                    trans.Dispose();
                    progressmeter.Stop();
                }
                #endregion
            }
        }

        // “界址点属性检查”按钮
        /// <summary>
        /// 界址点属性检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            arrerr.Clear();
            arrtext.Clear();
           // ArrayList arrJzdh = new ArrayList();
            List<int> arrJzdh = new List<int>();
            int xh = 0;
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Tools.CADTools.AddNewLayer("错误标记", "Continuous", 1);
                DelCWBJ();//删除错误标记图层
                #region 创建选择集获取界址点数据
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList_jzd = new TypedValue[2];
                filList_jzd[0] = new TypedValue((int)DxfCode.Start, "CIRCLE");
                filList_jzd[1] = new TypedValue(8, "JZP");
                SelectionFilter filter_jzd = new SelectionFilter(filList_jzd);
                PromptSelectionResult res_jzd = ed.SelectAll(filter_jzd);
                SelectionSet SS_jzd = res_jzd.Value;
                ObjectId[] objectId_jzd = new ObjectId[0];
                if (SS_jzd != null)
                {
                    objectId_jzd = SS_jzd.GetObjectIds();
                }
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    #region 显示进度条
                    ProgressMeter progressmeter = new ProgressMeter();
                    progressmeter.Start("正在生成界址点...");
                    progressmeter.SetLimit(objectId_jzd.Length);
                    #endregion
                    for (int i = 0; i < objectId_jzd.Length; i++)
                    {
                        progressmeter.MeterProgress();
                        Circle circle = (Circle)trans.GetObject(objectId_jzd[i], ACDBOpenMode.ForWrite);
                        Extents3d circleBounds = (Extents3d)circle.Bounds;
                        ResultBuffer resultBuffer = circle.XData;
                        try
                        {
                            #region 无属性
                            if (resultBuffer == null)//无属性
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址点";
                                er.cwsm = "界址点无属性。";
                                er.minx = circleBounds.MinPoint.X;
                                er.miny = circleBounds.MinPoint.Y;
                                er.maxX = circleBounds.MaxPoint.X;
                                er.maxy = circleBounds.MaxPoint.Y;
                                er.jb = circle.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 界址点属性丢失
                            //TypedValue[] tv3 = resultBuffer.AsArray();
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = GetApp(tv2, "SOUTH");
                            if (tv3.Length < 4)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址点";
                                er.cwsm = "界址点属性丢失。";
                                er.minx = circleBounds.MinPoint.X;
                                er.miny = circleBounds.MinPoint.Y;
                                er.maxX = circleBounds.MaxPoint.X;
                                er.maxy = circleBounds.MaxPoint.Y;
                                er.jb = circle.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 删除非本软件生成的界址点

                            if (tv3.Length == 5 && tv3[4].Value.ToString() != "BDC")
                            {
                                circle.Erase();
                                continue;
                            }
                            if (tv3.Length == 4  )
                            {
                                circle.Erase();
                                continue;
                            }
                            #endregion
                            #region 界址点重号
                            int Jzdh = (int)tv3[3].Value;
                            bool Ischh = false;
                            for (int j = 0; j < arrJzdh.Count; j++)
                            {
                                if ( arrJzdh[j] == Jzdh)
                                {
                                    xh++;
                                    err er = new err();
                                    er.xh = xh;
                                    er.xz = "必须修改";
                                    er.dxlx = "界址点";
                                    er.cwsm = "界址点重号。";
                                    er.minx = circleBounds.MinPoint.X;
                                    er.miny = circleBounds.MinPoint.Y;
                                    er.maxX = circleBounds.MaxPoint.X;
                                    er.maxy = circleBounds.MaxPoint.Y;
                                    er.jb = circle.Handle.Value.ToString();
                                    arrerr.Add(er);
                                    Ischh = true;
                                    break;
                                }
                            }
                            if (Ischh == false)
                            {
                                arrJzdh.Add((int)tv3[3].Value);
                            }
                            #endregion
                        }
                        #region 未知错误
                        catch (System.Exception)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "界址点";
                            er.cwsm = "未知错误";
                            er.minx = circleBounds.MinPoint.X;
                            er.miny = circleBounds.MinPoint.Y;
                            er.maxX = circleBounds.MaxPoint.X;
                            er.maxy = circleBounds.MaxPoint.Y;
                            er.jb = circle.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        #endregion
                    }
                    trans.Commit();
                    trans.Dispose();
                    Class1.WritErr(arrerr);
                    progressmeter.Stop();
                }
            }
        }
        /// <summary>
        /// 删除错误标记图层
        /// </summary>
        public void DelCWBJ()
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[1];
                filList[0] = new TypedValue(8, "错误标记");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null) { return; }
                ObjectId[] objectId = SS.GetObjectIds();
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                 
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        Entity entity = (Entity)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        entity.Erase();
                    }
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }
        
        // “房屋拓扑关系处理”按钮
        /// <summary>
        /// 房屋结构注记检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click_1(object sender, EventArgs e)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                bool isch = true;
                arrerr.Clear();
                arrtext.Clear();
                int xh = 0;
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                #region 创建房屋选择集
                TypedValue[] filList_FW = new TypedValue[1];
                filList_FW.SetValue(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "JZWSX"), 0);
                SelectionFilter filter_FW = new SelectionFilter(filList_FW);
                PromptSelectionResult res_FW = ed.SelectAll(filter_FW);
                SelectionSet SS_FW = res_FW.Value;
                if (SS_FW == null)
                {
                    MessageBox.Show("没有房屋数据。", "地籍工具", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                ObjectId[] objectId_FW = SS_FW.GetObjectIds();
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                        ProgressMeter progressmeter = new ProgressMeter();
                        arrerr.Clear();
                        arrtext.Clear();
                        #region  将房屋信息写入集合
                        progressmeter.SetLimit(objectId_FW.Length);
                        progressmeter.Start("正在检查房屋数据...");
                        Topology.Geometries.Polygon[] FW_area = new Topology.Geometries.Polygon[objectId_FW.Length];//房屋面对象数组
                        ACDBPolyline[] FWX_CAD = new ACDBPolyline[objectId_FW.Length];//CAD线数组，目的是提高速度
                        for (int i = 0; i < objectId_FW.Length; i++)
                        {
                            progressmeter.MeterProgress();
                            ACDBPolyline FWX = (ACDBPolyline)trans.GetObject(objectId_FW[i], ACDBOpenMode.ForWrite);
                            Extents3d JZXBounds = (Extents3d)FWX.Bounds;
                            #region 不闭合
                            if (!FWX.Closed)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "建筑物线";
                                er.cwsm = "不是闭合线。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = FWX.Handle.Value.ToString();
                                arrerr.Add(er);
                                isch = false;
                                continue;
                            }
                            #endregion
                            #region 面积过小
                            if (FWX.Area < 1.0)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "建筑物线";
                                er.cwsm = "面积过小。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = FWX.Handle.Value.ToString();
                                arrerr.Add(er);
                                isch = false;
                                continue;
                            }
                            #endregion
                            Topology.Geometries.Polygon FW_pline = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(FWX);
                            FW_area[i] = FW_pline;
                            FWX_CAD[i] = FWX;
                        }
                        progressmeter.Stop();
                        #endregion
                        #region 分析、处理房屋、阳台等面状地物之间的拓扑关系
                        double rc = 0.06;
                        if (isch == false)
                        {
                            MessageBox.Show("请把不合理的房屋、阳台等问题处理完成后再进行！", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        progressmeter.SetLimit(objectId_FW.Length);
                        progressmeter.Start("正在进行房屋拓扑关系处理...");
                        for (int i = 0; i < FW_area.Length; i++)//循环所有房屋边线
                        {
                            progressmeter.MeterProgress();
                            Topology.Geometries.Geometry FW_area_buff = (Topology.Geometries.Geometry)FW_area[i].Buffer(rc);
                            for (int j = 0; j < FW_area.Length; j++)//循环所有房屋边线
                            {
                                if (j == i)
                                {
                                    continue;
                                }
                                if (FW_area_buff.Intersects(FW_area[j]))//如果有交集
                                {
                                    for (int k = 0; k < FWX_CAD[j].NumberOfVertices; k++)//循环有交集的房屋边线个顶点
                                    {

                                        Point2d point1 = FWX_CAD[j].GetPoint2dAt(k);
                                        NCZJDDJFZ.Tools.PointFX_FW pointfx = new PointFX_FW(FWX_CAD[i], point1,0.05);//分析
                                        if (pointfx.PointTOMinDist < rc)//点到多边形的距离小于容差
                                        {
                                            if (pointfx.IsCH)
                                            {
                                                continue;
                                            }
                                            if (pointfx.point_DDCH)
                                            {
                                                FWX_CAD[j].SetPointAt(k, pointfx.NewPoint);//移动其他房屋顶点
                                                continue;
                                            }
                                            if (pointfx.OnBianJie)//在边界线上
                                            {
                                                FWX_CAD[i].AddVertexAt(pointfx.XH + 1, pointfx.NewPoint, 0, 0, 0);//在本房屋边线上加点
                                            }
                                            else
                                            {
                                                if (pointfx.point_CZ_OnBianJie)//如果此的顶点到多段线的垂足在线上
                                                {
                                                    if (pointfx.point_CZ_OnBianJie_DD)
                                                    {
                                                        FWX_CAD[j].SetPointAt(k, pointfx.NewPoint);//移动其他房屋顶点
                                                    }
                                                    else
                                                    {
                                                        FWX_CAD[j].SetPointAt(k, pointfx.NewPoint);//移动其他房屋顶点
                                                        FWX_CAD[i].AddVertexAt(pointfx.XH + 1, pointfx.NewPoint, 0, 0, 0);//在本房屋边线上加点
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //for (int k = 0; k < FWX_CAD.Length; k++)//循环有交集的房屋边线个顶点
                        //{
                        //    for (int i = 0; i < FWX_CAD[k].NumberOfVertices; i++)
                        //    {
                        //        FWX_CAD[k].SetPointAt(i, new Point2d((long)(10000.0 * FWX_CAD[k].GetPoint2dAt(i).X + 0.5) / 10000.0, (long)(10000.0 * FWX_CAD[k].GetPoint2dAt(i).Y + 0.5) / 10000.0));
                        //    }
                        //}
                        progressmeter.Stop();
                        #endregion
                        trans.Commit();
                        trans.Dispose();
                    }
                    catch (System.Exception ee)
                    {
                        MessageBox.Show(ee.Message, "地籍工具", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                Class1.WritErr(arrerr);
            }
        }

        // “界址线吸附”按钮
        private void button9_Click(object sender, EventArgs e)
        {
            double rc = 0.18;
            if (!Information.IsNumeric(this.textBox1.Text.Trim()))
            {
                MessageBox.Show("吸附容差不是数字", "地籍工具", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                return;
            }
            rc = Convert.ToDouble(this.textBox1.Text.Trim());
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                arrerr.Clear();
                arrtext.Clear();
                int xh = 0;
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                Tools.CADTools.AddNewLayer("错误标记", "Continuous", 1);
                DelCWBJ();//删除错误标记图层
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);

                    TypedValue[] filList_dw = new TypedValue[2];
                    filList_dw[0]  = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                    filList_dw[1]  = new TypedValue(8, "*");
                    SelectionFilter filter_dw = new SelectionFilter(filList_dw);
                    PromptSelectionResult res_dw = ed.SelectAll(filter_dw);
                    SelectionSet SS_dw = res_dw.Value;
                    if (SS_dw == null) { return; }
                    ObjectId[] objectId_dw = SS_dw.GetObjectIds();
                    ArrayList arrDWX = new ArrayList();
                    for (int i = 0; i < objectId_dw.Length; i++)
                    {
                        ACDBPolyline pline = (ACDBPolyline)trans.GetObject(objectId_dw[i], ACDBOpenMode.ForWrite);
                        Topology.Geometries.LineString lineString =(LineString) Tools.CADTools.ConvertToLineString(pline);
                        if (pline.Layer.ToLower() == "jzd")
                        {
                            continue;
                        }
                        if (pline.NumberOfVertices <= 1)
                        {
                            pline.Erase();
                            continue;
                        }
                        lineString.UserData = pline;
                        arrDWX.Add(lineString);
                    }

                    TypedValue[] filList = new TypedValue[2];
                    filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                    filList[1] = new TypedValue(8, "JZD");
                    SelectionFilter filter = new SelectionFilter(filList);
                    PromptSelectionResult res = ed.SelectAll(filter);
                    SelectionSet SS = res.Value;
                    if (SS == null) { return; }
                    ObjectId[] objectId = SS.GetObjectIds();
                    ProgressMeter progressmeter = new ProgressMeter();
                    progressmeter.Start("正在进行界址线吸附...");
                    progressmeter.SetLimit(objectId.Length);
                    int xgts = 0;//修改条数
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        progressmeter.MeterProgress();
                        ACDBPolyline pline_JZX = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        Extents3d JZXBounds = (Extents3d)pline_JZX.Bounds;
                        try
                        {
                            Topology.Geometries.LineString LineString_JZX = (LineString)Tools.CADTools.ConvertToLineString(pline_JZX);
                            Topology.Geometries.Geometry Buffer = (Topology.Geometries.Geometry)LineString_JZX.Buffer(rc, 30);
                            ArrayList arrXJDWX = new ArrayList();//与界址线相交的地物线集合
                            for (int j = 0; j < arrDWX.Count; j++)
                            {
                                if (Buffer.Intersects((LineString)arrDWX[j]))
                                {
                                    arrXJDWX.Add(arrDWX[j]);
                                }
                            }
                            for (int j = 0; j < arrXJDWX.Count; j++)
                            {
                                Topology.Geometries.LineString DWX = (Topology.Geometries.LineString)arrXJDWX[j];
                                ACDBPolyline pline_DWX = (ACDBPolyline)DWX.UserData;
                                for (int P = 0; P < pline_JZX.NumberOfVertices; P++)
                                {
                                    Point2d point = pline_JZX.GetPoint2dAt(P);
                                    Tools.PointFX_FW pointFX = new PointFX_FW(pline_DWX, point, rc);
                                    if (pointFX.PointTOMinDist < rc)
                                    {
                                        if (pointFX.IsCH)
                                        {
                                            continue;
                                        }
                                        if (pointFX.OnBianJie)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            if (pointFX.NewPoint.GetDistanceTo(pline_DWX.GetPoint2dAt(pointFX.XH)) < 0.0001)
                                            {
                                                pline_DWX.SetPointAt(pointFX.XH, point);
                                               // xgts++;
                                                continue;
                                            }
                                            if (pointFX.XH + 1 <= pline_DWX.NumberOfVertices - 1)
                                            {
                                                if (pointFX.NewPoint.GetDistanceTo(pline_DWX.GetPoint2dAt(pointFX.XH + 1)) < 0.0001)
                                                {
                                                    pline_DWX.SetPointAt(pointFX.XH + 1, point);
                                                    //xgts++;
                                                    continue;
                                                }
                                            }
                                            pline_DWX.AddVertexAt(pointFX.XH + 1, point, 0, 0, 0);
                                            xgts++;
                                        }
                                    }
                                }
                                for (int P = 0; P < pline_DWX.NumberOfVertices; P++)
                                {
                                    Point2d point = pline_DWX.GetPoint2dAt(P);
                                    Tools.PointFX_FW pointFX = new PointFX_FW(pline_JZX, point,rc);
                                    if (pointFX.PointTOMinDist < rc)
                                    {
                                        if (pointFX.IsCH)
                                        {
                                            continue;
                                        }
                                        if (pointFX.OnBianJie)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            
                                            pline_DWX.SetPointAt(P, pointFX.NewPoint);
                                            if (pointFX.PointTOMinDist > 0.0001)
                                            {
                                                xgts++;
                                            }
                                            
                                        }
                                    }
                                }
                            }
                        }
                        catch (System.Exception ee)
                        {
                             
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "酌情修改";
                            er.dxlx = "界址线";
                            er.cwsm = ee.Message;
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = pline_JZX.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                    }
                    trans.Commit();
                    trans.Dispose();
                    MessageBox.Show("共修改了 " + xgts.ToString()+" 个点", "地籍处理工具", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progressmeter.Stop();
                }
            }
            DEL_WDX(false);
        }
       /// <summary>
       /// 求指定应用程序名称的扩展属性集合
       /// </summary>
       /// <param name="res">从对象中获取的所有扩展属性集合</param>
       /// <param name="appName">应用程序名称</param>
       /// <returns>仅包含该扩展属性名称的数据集合</returns>
        public TypedValue[] GetApp(TypedValue[] res,string appName)
        {
            if (res == null) { return null; }
            int  qs=0;//从哪一行起
            int nn = 0;//总共多少行
            for (int i = 0; i < res.Length; i++)
            {
               if(res[i].TypeCode==1001 && res[i].Value.ToString().ToUpper()==appName.ToUpper())
                {
                    qs=i;
                    break;
                }
            }
            for (int i = qs; i < res.Length; i++)
            {
                if (res[i].TypeCode == 1001 && res[i].Value.ToString().ToUpper() != appName.ToUpper())
                {
                    break;
                }
                nn++;
            }
            if (nn > 0)
            {
                int n = 0;
                TypedValue[] resr = new TypedValue[nn];
                for (int i = qs; i < qs + nn; i++)
                {
                    resr[n] = res[i];
                    n++;
                }
                return resr;
            }
            else
            {
                return null;
            }
        }

        // “无属性实体检查”按钮
        private void button6_Click(object sender, EventArgs e)
        {
            arrerr.Clear();
            arrtext.Clear();
            int xh = 0;
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Tools.CADTools.OnAllLayer();
                #region 创建选择集 
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[1];
                filList[0] = new TypedValue((int)DxfCode.Start, "*");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null) { return; }
                ObjectId[] objectId = SS.GetObjectIds();
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    #region 显示进度条
                    ProgressMeter progressmeter = new ProgressMeter();
                    progressmeter.Start("正在检查无属性实体...");
                    progressmeter.SetLimit(objectId.Length);
                    #endregion
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        progressmeter.MeterProgress();
                        Entity entity = (Entity)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        Extents3d point = (Extents3d)entity.Bounds;
                        ResultBuffer resultBuffer = entity.XData;
                        #region 无属性
                        if (resultBuffer == null)//无属性
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "CAD实体对象";
                            er.cwsm = "对象无属性。";
                            er.minx = point.MinPoint.X-0.1;
                            er.miny = point.MinPoint.Y-0.1;
                            er.maxX = point.MaxPoint.X+0.1;
                            er.maxy = point.MaxPoint.Y+0.1;
                            er.jb = entity.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        #endregion
                        #region 属性丢失
                        TypedValue[] tv2 = resultBuffer.AsArray();
                        TypedValue[] tv3 = GetApp(tv2, "SOUTH");
                        if (tv3==null)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "CAD实体对象";
                            er.cwsm = "对象无属性。";
                            er.minx = point.MinPoint.X;
                            er.miny = point.MinPoint.Y;
                            er.maxX = point.MaxPoint.X;
                            er.maxy = point.MaxPoint.Y;
                            er.jb = entity.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        if (tv3.Length <=1)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "CAD实体对象";
                            er.cwsm = "对象属性丢失。";
                            er.minx = point.MinPoint.X;
                            er.miny = point.MinPoint.Y;
                            er.maxX = point.MaxPoint.X;
                            er.maxy = point.MaxPoint.Y;
                            er.jb = entity.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        #endregion
                    }
                    Class1.WritErr(arrerr);
                    trans.Commit();
                    trans.Dispose();
                    progressmeter.Stop();
                }
            }
        }

        // “统改街道街坊号”按钮
        private void button8_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择数据字典节点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (treeView1.SelectedNode.Level != 4)
            {
                MessageBox.Show("必须选择到行政村节点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            string XZmc = treeView1.SelectedNode.Text;
            string tsnr = "本功能将图形中所有的地籍区号和地籍子区号" + "\r\n";
            tsnr = tsnr + "统一改为“" + XZmc + "”所在的地籍区号和地籍子区号！";
            if (DialogResult.No == MessageBox.Show(tsnr, "重要提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return;
            }
            string XZDM = treeView1.SelectedNode.Tag.ToString();
            string JDH = XZDM.Substring(6, 3);
            string JFH = XZDM.Substring(9, 3);
            string XZQH = XZDM.Substring(0, 6);
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    #region 获取界址线
                    TypedValue[] filList = new TypedValue[2];
                    filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                    filList[1] = new TypedValue(8, "JZD");
                    SelectionFilter filter = new SelectionFilter(filList);
                    Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                    PromptSelectionResult res = ed.SelectAll(filter);
                    SelectionSet SS = res.Value;
                    if (SS == null)
                    {
                        MessageBox.Show("没有界址线", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    ObjectId[] objectId = SS.GetObjectIds();
                    #endregion
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        ACDBPolyline pline_jzx = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        try
                        {
                            ResultBuffer resultBuffer = pline_jzx.XData;
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                            string txdjh = tv3[2].Value.ToString().Trim();
                            txdjh = JDH + JFH + txdjh.Substring(6);


                            tv3[2] = new TypedValue(1000, txdjh);
                            RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                            regAppTableRecord.Name = "SOUTH";
                            if (!symbolTable.Has("SOUTH"))
                            {
                                symbolTable.Add(regAppTableRecord);
                                trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                            }
                            if (!pline_jzx.IsWriteEnabled)
                            {
                                pline_jzx.UpgradeOpen();
                            }
                            pline_jzx.XData = new ResultBuffer(tv3);
                        }
                        catch
                        {
                            MessageBox.Show("您的图形有错误，请利用检查功能检查后再修改。", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                    trans.Commit();
                    trans.Dispose();
                    MessageBox.Show("修改完成。", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        /// <summary>
        /// 房屋属性结构
        /// </summary>
        public struct FWSXJG
        {

            public string 建筑物类型0;
            public string 地籍号1;
            public string 房屋性质2;
            public string 房屋产别3;
            public string 房屋用途4;
            public string 规划用途5;
            public string 权利人类型6;
            public string 产权来源7;
            public string 共有情况8;
            public string 房屋幢号9;
            public string 户号10;
            public string 总层数11;
            public string 所在层数12;
            public string 房屋结构13;
            public string 竣工时间14;
            public string 专有建筑面积15;
            public string 分摊建筑面积16;
            public string 房屋功能17;
            public string 附加说明18;
            public string 调查日期19;
            public string 调查意见20;
            public string 调查员21;
            public string 面积折扣系数22;
            public string 北23;
            public string 东24;
            public string 南25;
            public string 西26;
            public string 自动分析地籍号27;
            public string 备注28;
            public double 本层建筑面积29;
        }
        /// <summary>修改对象属性
        /// 修改对象属性
        /// </summary>
        /// <param name="plineid"></param>
        /// <param name="sx"></param>
        public void XG_SX(ObjectId plineid, string djh, double mj)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite);
                ACDBPolyline pl = (ACDBPolyline)trans.GetObject(plineid, ACDBOpenMode.ForWrite);
                SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                 
                MapApplication mapApp = HostMapApplicationServices.Application;
                Tables tables = mapApp.ActiveProject.ODTables;
                Autodesk.Gis.Map.ObjectData.Table table;

                table = tables["房屋属性"];

                Autodesk.Gis.Map.ObjectData.Record record = Autodesk.Gis.Map.ObjectData.Record.Create();
                table.InitRecord(record);

                Records records = table.GetObjectTableRecords(0, pl, Autodesk.Gis.Map.Constants.OpenMode.OpenForWrite, true);
                if (records.Count > 0)
                {
                    foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                    {
                        Autodesk.Gis.Map.Utilities.MapValue val;
                        val = record2[1];
                        val.Assign(djh);
                        val = record2[29];
                        val.Assign(mj);
                        records.UpdateRecord(record2);
                        trans.Commit();
                        trans.Dispose();
                    }
                }
                records.Dispose();
            }
        }

        // “房屋检查与维护”按钮
        private void button10_Click_1(object sender, EventArgs e)
        {
            bool isch = true;
            arrerr.Clear();
            arrtext.Clear();
            ArrayList ZJFH = new ArrayList();
            int xh = 0;
            ArrayList pologylist = new ArrayList();
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 创建选择集获取界址线数据
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[2];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null) { return; }
                ObjectId[] objectId = SS.GetObjectIds();
                #endregion
                #region 创建房屋选择集
                TypedValue[] filList_FW = new TypedValue[1];
                filList_FW.SetValue(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "JZWSX"), 0);
                SelectionFilter filter_FW = new SelectionFilter(filList_FW);
                PromptSelectionResult res_FW = ed.SelectAll(filter_FW);
                SelectionSet SS_FW = res_FW.Value;
                if (SS_FW == null) { return; }
                ObjectId[] objectId_FW = SS_FW.GetObjectIds();
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    #region 检查是否存在"房屋属性"表
                    MapApplication mapApp = HostMapApplicationServices.Application;
                    Tables tables = mapApp.ActiveProject.ODTables;
                    Autodesk.Gis.Map.ObjectData.Table table;

                    bool IsxxB = true;
                    if (tables.TablesCount == 0) { IsxxB = true; }
                    if (tables.TablesCount > 0)
                    {
                        StringCollection tbablename = tables.GetTableNames();
                        for (int j = 0; j < tables.TablesCount; j++)
                        {
                            if ("房屋属性" == tbablename[j])
                            {
                                IsxxB = false;
                                break;
                            }
                        }
                    }
                    if (IsxxB == true)
                    {
                        MessageBox.Show("没有属性表！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    #endregion
                    ProgressMeter progressmeter = new ProgressMeter();
                    arrerr.Clear();
                    arrtext.Clear();
                    #region  将房屋信息写入集合
                    progressmeter.SetLimit(objectId_FW.Length);
                    progressmeter.Start("正在检查房屋数据...");
                    Topology.Geometries.Polygon[]  FW_area = new Topology.Geometries.Polygon[objectId_FW.Length];//房屋面对象数组
                    FWSXJG[] fwsxjg = new FWSXJG[objectId_FW.Length];//房屋属性结构
                    ACDBPolyline[] FWX_CAD = new ACDBPolyline[objectId_FW.Length];//CAD线数组，目的是提高速度
                    for (int i = 0; i < objectId_FW.Length; i++)
                    {
                        progressmeter.MeterProgress();
                        ACDBPolyline FWX = (ACDBPolyline)trans.GetObject(objectId_FW[i], ACDBOpenMode.ForWrite);
                        Extents3d JZXBounds = (Extents3d)FWX.Bounds;
                        FWX_CAD[i] = FWX;
                        #region 不闭合
                        if (!FWX.Closed)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "建筑物线";
                            er.cwsm = "不是闭合线。";
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = FWX.Handle.Value.ToString();
                            arrerr.Add(er);
                            isch = false;
                            continue;
                        }
                        #endregion
                        #region 面积过小
                        if (FWX.Area<1.0)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "建筑物线";
                            er.cwsm = "面积过小。";
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = FWX.Handle.Value.ToString();
                            arrerr.Add(er);
                            isch = false;
                            continue;
                        }
                        #endregion
                        #region 将建筑物线转换为面、属性数据写入数组
                        table = tables["房屋属性"];
                        Records records = table.GetObjectTableRecords(0, FWX, Autodesk.Gis.Map.Constants.OpenMode.OpenForRead, true);
                        if (records.Count > 0)
                        {
                            Topology.Geometries.Polygon FW_pline = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(FWX);
                            FW_pline.UserData = FWX.Handle.Value.ToString();
                            FW_area[i] = FW_pline;
                            FWSXJG sx = new FWSXJG();//房屋属性
                            #region 读取房屋属性
                            foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                            {
                                Autodesk.Gis.Map.Utilities.MapValue val;
                                val = record2[0];
                                sx.建筑物类型0 = val.StrValue;
                                val = record2[1];
                                sx.地籍号1= val.StrValue;
                                val = record2[2];
                                sx.房屋性质2= val.StrValue;
                                val = record2[3];
                                sx.房屋产别3= val.StrValue;
                                val = record2[4];
                                sx.房屋用途4= val.StrValue;
                                val = record2[5];
                                sx.规划用途5= val.StrValue;
                                val = record2[6];
                                sx.权利人类型6= val.StrValue;
                                val = record2[7];
                                sx.产权来源7= val.StrValue;
                                val = record2[8];
                                sx.共有情况8= val.StrValue;
                                val = record2[9];
                                sx.房屋幢号9= val.StrValue;
                                val = record2[10];
                                sx.户号10= val.StrValue;
                                val = record2[11];
                                sx.总层数11= val.StrValue;
                                val = record2[12];
                                sx.所在层数12= val.StrValue;
                                val = record2[13];
                                sx.房屋结构13= val.StrValue;
                                val = record2[14];
                                sx.竣工时间14= val.StrValue;
                                val = record2[15];
                                sx.专有建筑面积15= val.StrValue;
                                val = record2[16];
                                sx.分摊建筑面积16= val.StrValue;
                                val = record2[17];
                                sx.房屋功能17= val.StrValue;
                                val = record2[18];
                                sx.附加说明18= val.StrValue;
                                val = record2[19];
                                sx.调查日期19= val.StrValue;
                                val = record2[20];
                                sx.调查意见20= val.StrValue;
                                val = record2[21];
                                sx.调查员21= val.StrValue;
                                val = record2[22];
                                sx.面积折扣系数22= val.StrValue;
                                val = record2[23];
                                sx.北23= val.StrValue;
                                val = record2[24];
                                sx.东24= val.StrValue;
                                val = record2[25];
                                sx.南25= val.StrValue;
                                val = record2[26];
                                sx.西26= val.StrValue;
                                val = record2[27];
                                sx.自动分析地籍号27= val.StrValue;
                                val = record2[28];
                                sx.备注28= val.StrValue;
                                val = record2[29];
                                sx.本层建筑面积29 = val.DoubleValue;
                            }
                            #endregion
                            fwsxjg[i] = sx;
                        }
                        else
                        {
                            #region 写错
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "建筑物线";
                            er.cwsm = "无属性。";
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = FWX.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                            #endregion
                        }
                        records.Dispose();
                        #endregion
                    }
                    progressmeter.Stop();
                    #endregion
                    #region 分析、处理房屋、阳台等面状地物之间的拓扑关系
                    if (isch == false)
                    {
                        Class1.WritErr(arrerr);
                        MessageBox.Show("请把不合理的房屋、阳台等问题处理完成后再进行！", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    progressmeter.SetLimit(objectId_FW.Length);
                    progressmeter.Start("正在进行房屋几何关系检查...");
                    for (int i = 0; i < FW_area.Length; i++)//循环所有房屋、阳台边线
                    {
                        progressmeter.MeterProgress();
                        for (int j = i + 1; j < FW_area.Length; j++)//循环所有房屋、阳台边线
                        {
                           if( fwsxjg[i].所在层数12!=fwsxjg[j].所在层数12)//不在同一层不检查
                           {
                                continue;
                           }
                            if (FW_area[i].Intersects(FW_area[j]))//如果有交集
                            {
                                if (FW_area[i].Touches(FW_area[j]))//如果是接触则退出循环
                                {
                                    continue;
                                }
                                else
                                {
                                    //double mj1 = FW_area[i].Area;
                                    //double mj2 = FW_area[j].Area;
                                    //double mj = 0;//较小的房子的面积
                                    //if (mj1 >= mj2) { mj = mj2; }
                                    //else
                                    //{
                                    //    mj = mj1;
                                    //}
                                    //double xjmj = FW_area[i].Intersection(FW_area[j]).Area;//两个房子叠加后的面积
                                    //if (xjmj < 0.000001)
                                    //{
                                    //    continue;
                                    //}
                                    #region 写入两个房子重叠，但所在层数相同的错误
                                    //if (Math.Abs(mj1 - mj2) < 0.001 && Math.Abs(mj - xjmj) < 0.001 && fwsxjg[i].所在层数12 == fwsxjg[j].所在层数12)//两个房子重叠，但所在层数相同
                                    //{
                                    //    Topology.Geometries.IEnvelope envelope = FW_area[i].EnvelopeInternal;
                                    //    xh++;
                                    //    err er = new err();
                                    //    er.xh = xh;
                                    //    er.xz = "必须修改";
                                    //    er.dxlx = "CAD实体对象";
                                    //    er.cwsm = "两个房子重叠、但所在楼层相同。";
                                    //    er.minx = envelope.MinX;
                                    //    er.miny = envelope.MinY;
                                    //    er.maxX = envelope.MaxX;
                                    //    er.maxy = envelope.MaxY;
                                    //    er.jb = FW_area[i].UserData.ToString();
                                    //    arrerr.Add(er);
                                    //    continue;
                                    //}
                                   
                                    //if (fwsxjg[i].所在层数12 == fwsxjg[j].所在层数12)//两个房子重叠，但所在层数相同
                                    //{
                                    //    bool IS_BH = false;
                                    //    if (mj1 >= mj2)
                                    //    {
                                    //        IS_BH = FW_area[j].CoveredBy(FW_area[i]);
                                    //    }
                                    //    else
                                    //    {
                                    //        IS_BH = FW_area[i].CoveredBy(FW_area[j]);
                                    //    }
                                    //    if (IS_BH)
                                    //    {
                                    //        Topology.Geometries.IEnvelope envelope = FW_area[i].EnvelopeInternal;
                                    //        xh++;
                                    //        err er = new err();
                                    //        er.xh = xh;
                                    //        er.xz = "必须修改";
                                    //        er.dxlx = "CAD实体对象";
                                    //        er.cwsm = "两个房子重叠、但所在楼层相同。";
                                    //        er.minx = envelope.MinX;
                                    //        er.miny = envelope.MinY;
                                    //        er.maxX = envelope.MaxX;
                                    //        er.maxy = envelope.MaxY;
                                    //        er.jb = FW_area[i].UserData.ToString();
                                    //        arrerr.Add(er);
                                    //        continue;
                                    //    }
                                    //}
                                    #endregion
                                    #region 写入两个房子交叉了的错误（相同层的)
                                    ////if (fwsxjg[i].建筑物类型0 == "房屋" || fwsxjg[i].建筑物类型0 == "门廊" || fwsxjg[i].建筑物类型0 == "车棚")//找出是房屋、门廊等建筑物
                                    ////{
                                    ////    if (fwsxjg[j].建筑物类型0 == "房屋" || fwsxjg[j].建筑物类型0 == "门廊" || fwsxjg[j].建筑物类型0 == "车棚")//找出是房屋、门廊等建筑物
                                    ////    {

                                    //if (xjmj < mj * 0.5)//两个房子叠加后不合理，可能是另一栋房子多画了
                                    //{
                                    Topology.Geometries.IEnvelope envelope = FW_area[i].EnvelopeInternal;
                                    xh++;
                                    err er = new err();
                                    er.xh = xh;
                                    er.xz = "必须修改";
                                    er.dxlx = "CAD实体对象";
                                    er.cwsm = "两个面交叉。";
                                    er.minx = envelope.MinX;
                                    er.miny = envelope.MinY;
                                    er.maxX = envelope.MaxX;
                                    er.maxy = envelope.MaxY;
                                    er.jb = FW_area[i].UserData.ToString();
                                    arrerr.Add(er);
                                    //    continue;
                                    //    ////    }
                                    //    ////}
                                    //}
                                    #endregion
                                }
                            }
                        }
                    }
                    progressmeter.Stop();
                    #endregion
                    progressmeter.SetLimit(objectId.Length);
                    progressmeter.Start("正在进行房屋逻辑关系检查...");
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        progressmeter.MeterProgress();
                        ACDBPolyline JZX;
                        JZX = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        int DDS = JZX.NumberOfVertices;
                        Extents3d JZXBounds = (Extents3d)JZX.Bounds;
                        ResultBuffer resultBuffer = JZX.GetXDataForApplication("SOUTH");
                        try
                        {
                            #region 获取宗地信息
                            TypedValue[] tv3 = resultBuffer.AsArray();
                           // if (tv3[1].Value.ToString().Trim() != "300000") { continue; }//如果是街道界
                            string DM = tv3[2].Value.ToString().Trim();
                            string ZDH = DM.Substring(6);
                            ZDH = ZDH.PadLeft(5, '0');
                            string DJH = DM.Substring(0, 6) + ZDH;
                            #endregion
                            #region 找出宗地范围内的房屋、阳台等
                            Topology.Geometries.Polygon JZX_area = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(JZX);//界址线面
                            Topology.Geometries.Polygon JZX_area_buffer = (Topology.Geometries.Polygon)JZX_area.Buffer(0.1);//界址线缓冲区
                            List<Topology.Geometries.Polygon> ZD_inside_FW_ML_CP = new List<Topology.Geometries.Polygon>();//与界址线缓冲区有交集的房屋、门廊、车棚的面的集合
                            List<Topology.Geometries.Polygon> ZD_inside_YT_YL_LT = new List<Topology.Geometries.Polygon>();//与界址线缓冲区有交集的阳台、檐廊、楼梯的面的集合
                            List<FWSXJG> ZD_inside_fwsxjg_FW_ML_CP = new List<FWSXJG>();//与界址线缓冲区有交集的房屋、门廊、车棚属性集合
                            List<FWSXJG> ZD_inside_fwsxjg_YT_YL_LT = new List<FWSXJG>();//与界址线缓冲区有交集的阳台、檐廊、楼梯属性集合
                            List<int> ZD_inside_fwsxjg_FW_ML_CP_XH = new List<int>();//房屋的序号
                            List<Topology.Geometries.Polygon> ZD_inside_DX_Z = new List<Topology.Geometries.Polygon>();//与界址线缓冲区有交集的全部面的集合
                            List<FWSXJG> ZD_inside_fwsxjg_Z = new List<FWSXJG>();//与界址线缓冲区有交集的全部面属性集合

                            for (int j = 0; j < FW_area.Length; j++)
                            {
                                if (JZX_area_buffer.Intersects(FW_area[j]))//房屋与界址线缓冲区有交集
                                {
                                    if (FW_area[j].Intersection(JZX_area).Area > 0.5 * FW_area[j].Area)//如果房屋有一半以上在宗地内就认为是本宗地的房屋、门廊等建筑物
                                    {
                                        ZD_inside_DX_Z.Add(FW_area[j]);
                                        ZD_inside_fwsxjg_Z.Add(fwsxjg[j]);


                                        if (fwsxjg[j].建筑物类型0 == "房屋" || fwsxjg[j].建筑物类型0 == "门廊" || fwsxjg[j].建筑物类型0 == "车棚")//找出是房屋、门廊等建筑物
                                        {

                                            ZD_inside_FW_ML_CP.Add(FW_area[j]);
                                            ZD_inside_fwsxjg_FW_ML_CP.Add(fwsxjg[j]);
                                            ZD_inside_fwsxjg_FW_ML_CP_XH.Add(j);
                                        }
                                        else
                                        {
                                           
                                            ZD_inside_YT_YL_LT.Add(FW_area[j]);
                                            ZD_inside_fwsxjg_YT_YL_LT.Add(fwsxjg[j]);
                                        }
                                    }
                                }
                            }
                            #endregion
                            #region 将房屋按幢分类
                            List<List<FWSXJG>> DS_SX = new List<List<FWSXJG>>();//以栋为单位的 房屋的数组  属性  一个宗地内的多栋房屋
                            List<List<Topology.Geometries.Polygon>> DS_DX = new List<List<Topology.Geometries.Polygon>>();//以栋为单位的 房屋的数组  面对象
                            #region MyRegion
                            List<List<FWSXJG>> Z_SX = new List<List<FWSXJG>>();//以栋为单位的 房屋的数组  属性  一个宗地内的多栋房屋 包含阳台
                            List<List<Topology.Geometries.Polygon>> Z_DX = new List<List<Topology.Geometries.Polygon>>();//以栋为单位的 房屋的数组  面对象
                            if (ZD_inside_DX_Z.Count == 0)
                            {
                                continue;
                            }
                            while (ZD_inside_DX_Z.Count > 0)
                            {
                                List<FWSXJG> TD_SX2 = new List<FWSXJG>();//同栋房各层数组 属性
                                List<Topology.Geometries.Polygon> TD_DX2 = new List<Topology.Geometries.Polygon>();//同栋房各层数组 面对象


                                string FirstDH2 = ZD_inside_fwsxjg_Z[0].房屋幢号9;
                                for (int j = 0; j < ZD_inside_fwsxjg_Z.Count; j++)
                                {
                                    if (ZD_inside_fwsxjg_Z[j].房屋幢号9 == FirstDH2)
                                    {
                                        TD_SX2.Add(ZD_inside_fwsxjg_Z[j]);
                                        ZD_inside_fwsxjg_Z.RemoveAt(j);

                                        TD_DX2.Add(ZD_inside_DX_Z[j]);
                                        ZD_inside_DX_Z.RemoveAt(j);
                                        j--;
                                    }
                                }
                                Z_SX.Add(TD_SX2);
                                Z_DX.Add(TD_DX2);

                            }
                            
                            #endregion
                            #endregion
                            #region 房屋、阳台关系检查
                            string dcsj = Z_SX[0][0].调查日期19;
                            string dcy = Z_SX[0][0].调查员21;
                            string dcyj = Z_SX[0][0].调查意见20;
                            //if (sx.建筑物类型0 == "阳台" || sx.建筑物类型0 == "楼梯" || sx.建筑物类型0 == "檐廊")
                            //{
                            //    break;
                            //}
                            FWSXJG bjz = new FWSXJG();//用来比较的一个基本值    本宗
                            
                            for (int j = 0; j < Z_SX.Count; j++)//DS_DX   一个宗地内的多栋房屋
                            {
                                List<FWSXJG> TD_SX = Z_SX[j];//获取某一栋房屋等信息  属性
                                for (int k = 0; k < TD_SX.Count; k++)//先找出第一层 将同栋的一层进行融合 如果融合后的面的个数不等于1则
                                {
                                    if (Z_SX[j][k].建筑物类型0 == "房屋" || Z_SX[j][k].建筑物类型0 == "门廊" || Z_SX[j][k].建筑物类型0 == "车棚")
                                    {
                                        bjz = Z_SX[j][k];
                                        break;
                                    }
                                }
                            }
                            for (int j = 0; j < Z_DX.Count; j++)//DS_DX   一个宗地内的多栋房屋
                            {
                                FWSXJG bjz_bd = new FWSXJG();//用来比较的一个基本值   本幢
                                List<Topology.Geometries.Polygon> TD_DX = Z_DX[j];//获取某一栋房屋等信息  对象
                                List<FWSXJG> TD_SX = Z_SX[j];//获取某一栋房屋等信息  属性
                                Topology.Geometries.Geometry dyc = null;//将所有第一层全部融合，融合不到一起的就是另一栋房屋的 
                                int nn = 0;
                                List<int> list_Cen = new List<int>();//同栋的所在层编号（剔除相同的）
                                for (int k = 0; k < TD_DX.Count; k++)//先找出第一层 将同栋的一层进行融合 如果融合后的面的个数不等于1则
                                {
                                    if (TD_SX[k].建筑物类型0 == "房屋" || TD_SX[k].建筑物类型0 == "门廊" || TD_SX[k].建筑物类型0 == "车棚")
                                    {
                                        bjz_bd = TD_SX[k];//找出本幢比较的一个基本值
                                    }
                                    #region 将所有不同号“所在楼层”加到一个数组，同时检查有没有等于零的所在楼层
                                    string Cen = TD_SX[k].所在层数12;
                                    int nCen = Convert.ToInt32(Cen);
                                    if (list_Cen.IndexOf(nCen) < 0)
                                    {
                                        list_Cen.Add(nCen);
                                    }
                                    if (nCen == 0)
                                    {
                                        Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                        xh++;
                                        err er = new err();
                                        er.xh = xh;
                                        er.xz = "必须修改";
                                        er.dxlx = "CAD实体对象";
                                        er.cwsm = "所在层数不可为零。";
                                        er.minx = envelope.MinX;
                                        er.miny = envelope.MinY;
                                        er.maxX = envelope.MaxX;
                                        er.maxy = envelope.MaxY;
                                        er.jb = TD_DX[k].UserData.ToString();
                                        arrerr.Add(er);
                                        //continue;
                                        //MessageBox.Show("所在层数不可为零", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                    #endregion
                                    #region 融合所有第一层的"房屋" 、"门廊"、"车棚"
                                    string JZWLX = TD_SX[k].建筑物类型0;
                                    if (TD_SX[k].所在层数12 == "1")//////
                                    {
                                        if (JZWLX == "房屋" || JZWLX == "门廊" || JZWLX == "车棚")
                                        {
                                            if (nn == 0)
                                            {
                                                dyc = TD_DX[k];
                                                dyc.UserData = TD_DX[k].UserData;
                                            }
                                            else
                                            {
                                                dyc = (Topology.Geometries.Geometry)dyc.Union(TD_DX[k]);
                                                dyc.UserData = TD_DX[k].UserData;
                                            }
                                            nn++;
                                        }
                                    }
                                    #endregion
                                }
                                #region 将"没有第一层！"和"第一层栋号重号！错误写入列表
                                if (dyc == null)
                                {
                                    Topology.Geometries.IEnvelope envelope = TD_DX[0].EnvelopeInternal;
                                    xh++;
                                    err er = new err();
                                    er.xh = xh;
                                    er.xz = "必须修改";
                                    er.dxlx = "CAD实体对象";
                                    er.cwsm = "此处房屋没有一层。";
                                    er.minx = envelope.MinX;
                                    er.miny = envelope.MinY;
                                    er.maxX = envelope.MaxX;
                                    er.maxy = envelope.MaxY;
                                    er.jb = TD_DX[0].UserData.ToString();
                                    arrerr.Add(er);
                                    // MessageBox.Show("没有一层！", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                                else
                                {
                                    if (dyc.NumGeometries != 1)
                                    {
                                        Topology.Geometries.IEnvelope envelope = dyc.EnvelopeInternal;
                                        xh++;
                                        err er = new err();
                                        er.xh = xh;
                                        er.xz = "必须修改";
                                        er.dxlx = "CAD实体对象";
                                        er.cwsm = "第一层栋号重号！";
                                        er.minx = envelope.MinX;
                                        er.miny = envelope.MinY;
                                        er.maxX = envelope.MaxX;
                                        er.maxy = envelope.MaxY;
                                        er.jb = dyc.UserData.ToString();
                                        arrerr.Add(er);
                                        // MessageBox.Show("第一层栋号重号！", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                }
                                #endregion
                                #region 检查有没有楼层跳号的情况，即丢了楼层号
                                if (list_Cen.IndexOf(0) < 0)
                                {
                                    list_Cen.Add(0);
                                }
                                list_Cen.Sort();
                                int zdyc = list_Cen[0];//最底一层

                                for (int k = 1; k < list_Cen.Count; k++)
                                {
                                    if ((zdyc + 1) == 0)
                                    {
                                        continue;
                                    }
                                    if (zdyc + 1 != list_Cen[k])
                                    {
                                        int cs = zdyc + 1;
                                        Topology.Geometries.IEnvelope envelope = TD_DX[0].EnvelopeInternal;
                                        xh++;
                                        err er = new err();
                                        er.xh = xh;
                                        er.xz = "必须修改";
                                        er.dxlx = "CAD实体对象";
                                        er.cwsm = "此栋房屋没有" + cs.ToString() + "层";
                                        er.minx = envelope.MinX;
                                        er.miny = envelope.MinY;
                                        er.maxX = envelope.MaxX;
                                        er.maxy = envelope.MaxY;
                                        er.jb = TD_DX[0].UserData.ToString();
                                        arrerr.Add(er);
                                        zdyc++;
                                        //MessageBox.Show("总层数不正确", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                    zdyc++;
                                }
                                #endregion
                                for (int k = 0; k < TD_DX.Count; k++)//同栋楼与第一层楼没有交集，则说明是其他栋的编号，栋号编写错误
                                {
                                    #region 检查楼层总数是否与所画层数吻合
                                    if (Convert.ToInt32(TD_SX[k].总层数11) != list_Cen.Count - 1)
                                    {
                                        Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                        xh++;
                                        err er = new err();
                                        er.xh = xh;
                                        er.xz = "必须修改";
                                        er.dxlx = "CAD实体对象";
                                        er.cwsm = "楼层总数与CAD中所绘制的层数数不吻合！";
                                        er.minx = envelope.MinX;
                                        er.miny = envelope.MinY;
                                        er.maxX = envelope.MaxX;
                                        er.maxy = envelope.MaxY;
                                        er.jb = TD_DX[k].UserData.ToString();
                                        arrerr.Add(er);
                                        //MessageBox.Show("总层数不正确", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                    #endregion
                                    #region 检查所在层数是否大于总层数
                                    if (Convert.ToInt32(TD_SX[k].总层数11) < Convert.ToInt32(TD_SX[k].所在层数12))
                                    {
                                        Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                        xh++;
                                        err er = new err();
                                        er.xh = xh;
                                        er.xz = "必须修改";
                                        er.dxlx = "CAD实体对象";
                                        er.cwsm = "所在层数大于总层数！";
                                        er.minx = envelope.MinX;
                                        er.miny = envelope.MinY;
                                        er.maxX = envelope.MaxX;
                                        er.maxy = envelope.MaxY;
                                        er.jb = TD_DX[k].UserData.ToString();
                                        arrerr.Add(er);
                                        //MessageBox.Show("所在层数不可大于总层数！", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    }
                                    #endregion
                                    #region  检查各层是否与第一层的（融合数据）相交，也就是检查有没有其他地方的栋号错编到本栋里了或者栋号重复了
                                    if (TD_SX[k].所在层数12 != "1")//////
                                    {
                                        if (!dyc.Intersects(TD_DX[k]))
                                        {
                                            string jzwlx0 = TD_SX[k].建筑物类型0;
                                            if (jzwlx0 == "阳台" || jzwlx0 == "楼梯" || jzwlx0 == "檐廊")
                                            {

                                            }
                                            else
                                            {
                                                Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                                xh++;
                                                err er = new err();
                                                er.xh = xh;
                                                er.xz = "必须修改";
                                                er.dxlx = "CAD实体对象";
                                                er.cwsm = "此处房屋栋号重号！";
                                                er.minx = envelope.MinX;
                                                er.miny = envelope.MinY;
                                                er.maxX = envelope.MaxX;
                                                er.maxy = envelope.MaxY;
                                                er.jb = TD_DX[k].UserData.ToString();
                                                arrerr.Add(er);
                                                //MessageBox.Show("栋号编写错误", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                            }
                                        }
                                    }
                                    #endregion
                                    #region 检查调查时间、调查员、调查意见、房屋性质、房屋产别、权利人类型、房屋用途等在同一个宗地是否一致
                                    if (TD_SX[k].建筑物类型0 == "房屋" || TD_SX[k].建筑物类型0 == "门廊" || TD_SX[k].建筑物类型0 == "车棚")
                                    {
                                        #region 检查调查时间
                                        if (TD_SX[k].调查日期19 != bjz.调查日期19)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地调查时间应相同！";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 检查调查员
                                        if (TD_SX[k].调查员21 != bjz.调查员21)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地调查员应相同！";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 检查调查意见
                                        if (TD_SX[k].调查意见20 != bjz.调查意见20)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地调查意见应相同！";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 检查房屋性质
                                        if (TD_SX[k].房屋性质2 != bjz.房屋性质2)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地房屋性质应相同！";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 检查房屋产别
                                        if (TD_SX[k].房屋产别3 != bjz.房屋产别3)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地房屋产别应相同！";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 权利人类型
                                        if (TD_SX[k].权利人类型6 != bjz.权利人类型6)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地权利人类型应相同！";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 检查房屋用途
                                        if (TD_SX[k].房屋用途4 != bjz.房屋用途4)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地房屋用途应相同,若不同应分宗。";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 检查规划用途
                                        if (TD_SX[k].规划用途5 != bjz.规划用途5)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地规划用途应相同,若不同应分宗。";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 检查附加说明
                                        if (TD_SX[k].附加说明18 != bjz.附加说明18)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一宗地附加说明应相同。";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                        #region 检查竣工时间
                                        if (TD_SX[k].竣工时间14 != bjz_bd.竣工时间14)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一幢房屋竣工时间应相同。";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }

                                        #endregion
                                        #region 检查房屋结构
                                        if (TD_SX[k].房屋结构13 != bjz_bd.房屋结构13)
                                        {
                                            Topology.Geometries.IEnvelope envelope = TD_DX[k].EnvelopeInternal;
                                            xh++;
                                            err er = new err();
                                            er.xh = xh;
                                            er.xz = "必须修改";
                                            er.dxlx = "CAD实体对象";
                                            er.cwsm = "同一幢房屋，房屋结构应相同。";
                                            er.minx = envelope.MinX;
                                            er.miny = envelope.MinY;
                                            er.maxX = envelope.MaxX;
                                            er.maxy = envelope.MaxY;
                                            er.jb = TD_DX[k].UserData.ToString();
                                            arrerr.Add(er);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                            #region 分析依附于某一层的阳台、檐廊、楼梯,将相关属性写入
                            double Z_bzdjzmj = 0;//本宗地建筑面积
                            double Z_bzdjzzdmj = 0;//本宗地建筑占地面积
                            for (int j = 0; j < ZD_inside_FW_ML_CP.Count; j++)//宗地中的所有栋房屋
                            {
                                ACDBPolyline mycfw = FWX_CAD[ZD_inside_fwsxjg_FW_ML_CP_XH[j]];//某一层房屋实体
                                double mj = 0;
                                for (int p = 0; p < ZD_inside_YT_YL_LT.Count; p++)//宗地中的所有栋的阳台等
                                {
                                    if (ZD_inside_fwsxjg_FW_ML_CP[j].房屋幢号9 == ZD_inside_fwsxjg_YT_YL_LT[p].房屋幢号9)//栋号相同
                                    {
                                        if (ZD_inside_fwsxjg_FW_ML_CP[j].所在层数12 == ZD_inside_fwsxjg_YT_YL_LT[p].所在层数12)//层数相同
                                        {
                                            if (ZD_inside_YT_YL_LT[p].Touches(ZD_inside_FW_ML_CP[j]))//有接触
                                            {
                                                double MJ2 = ZD_inside_YT_YL_LT[p].Area * (Convert.ToDouble(ZD_inside_fwsxjg_YT_YL_LT[p].面积折扣系数22));
                                                MJ2 = Math.Round(MJ2, 2);
                                                mj = mj + MJ2;
                                                XG_SX(CADTools.HandletoObjectID(ZD_inside_YT_YL_LT[p].UserData.ToString()), DJH, MJ2);
                                                ZD_inside_YT_YL_LT.RemoveAt(p);
                                                ZD_inside_fwsxjg_YT_YL_LT.RemoveAt(p);
                                                p--;
                                            }
                                        }
                                    }
                                }

                                mj = mj + Math.Round(mycfw.Area * (Convert.ToDouble(ZD_inside_fwsxjg_FW_ML_CP[j].面积折扣系数22)), 2);
                                Z_bzdjzmj = Z_bzdjzmj + Math.Round( mj,2);
                                //if(ZD_inside_fwsxjg_FW_ML_CP[j].自动分析地籍号27!="否")
                                //{
                                //    DJH = ZD_inside_fwsxjg_FW_ML_CP[j].地籍号1;
                                //    Z_bzdjzmj = Z_bzdjzmj + mj;
                                //}
                                if (ZD_inside_fwsxjg_FW_ML_CP[j].所在层数12 == "1")
                                {
                                    //Z_bzdjzzdmj = Z_bzdjzzdmj + mycfw.Area * (Convert.ToDouble(ZD_inside_fwsxjg_FW_ML_CP[j].面积折扣系数22));
                                    Z_bzdjzzdmj = Z_bzdjzzdmj + Math.Round(mj, 2);
                                }
                                XG_SX(mycfw.ObjectId, DJH, mj);
                            }
                            Tools.CADTools.Add_JZX_SX(JZX.ObjectId, Z_bzdjzmj, Z_bzdjzzdmj);
                            for (int j = 0; j < ZD_inside_YT_YL_LT.Count; j++)
                            {
                                Topology.Geometries.IEnvelope envelope = ZD_inside_YT_YL_LT[j].EnvelopeInternal;
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "CAD实体对象";
                                er.cwsm = "不依附与房屋的阳台、檐廊、楼梯！";
                                er.minx = envelope.MinX;
                                er.miny = envelope.MinY;
                                er.maxX = envelope.MaxX;
                                er.maxy = envelope.MaxY;
                                er.jb = ZD_inside_YT_YL_LT[j].UserData.ToString();
                                arrerr.Add(er);
                            }
                            #endregion
                        }
                        #region 未知错误
                        catch (System.Exception cw)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "界址线";
                            er.cwsm = cw.Message;
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = JZX.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        #endregion
                    }
                    progressmeter.Stop();
                    Class1.WritErr(arrerr);
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }

        // “查找界址点最大号”按钮
        private void button11_Click_1(object sender, EventArgs e)
        {
            //arrerr.Clear();
            //arrtext.Clear();
            int ZD_jzdh = -1;
            ArrayList arrJzdh = new ArrayList();
            int xh = 0;
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Tools.CADTools.AddNewLayer("错误标记", "Continuous", 1);
                DelCWBJ();//删除错误标记图层
                #region 创建选择集获取界址点数据
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList_jzd = new TypedValue[2];
                filList_jzd[0] = new TypedValue((int)DxfCode.Start, "CIRCLE");
                filList_jzd[1] = new TypedValue(8, "JZP");
                SelectionFilter filter_jzd = new SelectionFilter(filList_jzd);
                PromptSelectionResult res_jzd = ed.SelectAll(filter_jzd);
                SelectionSet SS_jzd = res_jzd.Value;
                ObjectId[] objectId_jzd = new ObjectId[0];
                if (SS_jzd != null)
                {
                    objectId_jzd = SS_jzd.GetObjectIds();
                }
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    #region 显示进度条
                    //ProgressMeter progressmeter = new ProgressMeter();
                    //progressmeter.Start("正在生成界址点...");
                    //progressmeter.SetLimit(objectId_jzd.Length);
                    #endregion
                    for (int i = 0; i < objectId_jzd.Length; i++)
                    {
                        //progressmeter.MeterProgress();
                        Circle circle = (Circle)trans.GetObject(objectId_jzd[i], ACDBOpenMode.ForWrite);
                        Extents3d circleBounds = (Extents3d)circle.Bounds;
                        ResultBuffer resultBuffer = circle.XData;
                        try
                        {
                            #region 无属性
                            if (resultBuffer == null)//无属性
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址点";
                                er.cwsm = "界址点无属性。";
                                er.minx = circleBounds.MinPoint.X;
                                er.miny = circleBounds.MinPoint.Y;
                                er.maxX = circleBounds.MaxPoint.X;
                                er.maxy = circleBounds.MaxPoint.Y;
                                er.jb = circle.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 界址点属性丢失
                            //TypedValue[] tv3 = resultBuffer.AsArray();
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = GetApp(tv2, "SOUTH");
                            if (tv3.Length < 4)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址点";
                                er.cwsm = "界址点属性丢失。";
                                er.minx = circleBounds.MinPoint.X;
                                er.miny = circleBounds.MinPoint.Y;
                                er.maxX = circleBounds.MaxPoint.X;
                                er.maxy = circleBounds.MaxPoint.Y;
                                er.jb = circle.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 查找最大号
                            if (tv3.Length == 5 && tv3[4].Value.ToString() == "BDC")
                            {
                                int Jzdh = (int)tv3[3].Value;
                                if (ZD_jzdh < Jzdh)
                                {
                                    ZD_jzdh = Jzdh;
                                }
                            }
                            #endregion
                        }
                        #region 未知错误
                        catch (System.Exception)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "界址点";
                            er.cwsm = "未知错误";
                            er.minx = circleBounds.MinPoint.X;
                            er.miny = circleBounds.MinPoint.Y;
                            er.maxX = circleBounds.MaxPoint.X;
                            er.maxy = circleBounds.MaxPoint.Y;
                            er.jb = circle.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        #endregion
                    }
                    trans.Commit();
                    trans.Dispose();
                    Class1.WritErr(arrerr);
                    MessageBox.Show("最大界址点号是 " + ZD_jzdh.ToString() + " 号。", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // “查找宗地最大号”按钮
        private void button12_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                MessageBox.Show("请选择数据字典节点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return  ;
            }
            if (treeView1.SelectedNode.Level != 4)
            {
                MessageBox.Show("必须选择到行政村节点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return  ;
            }
            arrerr.Clear();
            arrtext.Clear();
            int ZD_ZDH = -1;
            ArrayList ZJFH = new ArrayList();
            int xh = 0;
            string DJH = treeView1.SelectedNode.Tag.ToString();
            string JDH = DJH.Substring(6, 3);
            string JFH = DJH.Substring(9, 3);
            ArrayList pologylist = new ArrayList();
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 创建选择集获取界址线数据
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[2];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null) { return; }
                ObjectId[] objectId = SS.GetObjectIds();
                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    ProgressMeter progressmeter = new ProgressMeter();
                    progressmeter.Start("正在检查宗地...");
                    progressmeter.SetLimit(objectId.Length);
                    arrerr.Clear();
                    arrtext.Clear();
                    for (int i = 0; i < objectId.Length; i++)
                    {

                        progressmeter.MeterProgress();
                        ACDBPolyline JZX;
                        JZX = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        int DDS = JZX.NumberOfVertices;
                        Extents3d JZXBounds = (Extents3d)JZX.Bounds;
                        ResultBuffer resultBuffer = JZX.XData;
                        try
                        {
                            #region 无属性
                            if (resultBuffer == null)//无属性
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "界址线无属性。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            //TypedValue[] tv3 = resultBuffer.AsArray();
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = GetApp(tv2, "SOUTH");
                            if (tv3[1].Value.ToString().Trim() != "300000") { continue; }//如果是街道界
                            #region 界址线属性丢失
                            if (tv3.Length < 5)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "界址线属性丢失。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #region 获取宗地信息
                            string jfh;
                            string jdh;
                            string zdh;
                            string djh;
                            string tdyt;
                            djh = tv3[2].Value.ToString();
                            tdyt = tv3[4].Value.ToString().Trim();
                            jdh = djh.Substring(0, 3);
                            jfh = djh.Substring(3, 3);
                            #endregion
                            #region 街道号错误
                            if (JDH != jdh)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地籍区号不正确(即街道号)。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            #region 街坊号错误
                            if (JFH != jfh)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地籍子区号不正确(即街坊号)。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            //#region 地籍号长度错误
                            if (djh.Length < 7 || djh.Length > 11)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "界址线";
                                er.cwsm = "地籍号位数太多或太少。";
                                er.minx = JZXBounds.MinPoint.X;
                                er.miny = JZXBounds.MinPoint.Y;
                                er.maxX = JZXBounds.MaxPoint.X;
                                er.maxy = JZXBounds.MaxPoint.Y;
                                er.jb = JZX.Handle.Value.ToString();
                                arrerr.Add(er);
                                continue;
                            }
                            #endregion
                            zdh = djh.Substring(6, djh.Length - 6);
                            int nzdh = Convert.ToInt32(zdh);
                            if (ZD_ZDH < nzdh)
                            {
                                ZD_ZDH = nzdh;
                            }
                        }
                        
                        catch (System.Exception cw)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "界址线";
                            er.cwsm = cw.Message;
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = JZX.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                            throw;
                        }
                    }
                }
            }
            MessageBox.Show("最大宗地点号是 " + ZD_ZDH.ToString() + " 号。", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // “注记房屋结构”按钮
        private void button13_Click(object sender, EventArgs e)
        {
            ArrayList arrerr = new ArrayList();
            ArrayList arrtext = new ArrayList();
            arrerr.Clear();
            arrtext.Clear();
            ArrayList ZJFH = new ArrayList();
            int xh = 0;
            ArrayList pologylist = new ArrayList();
            
            double zg= 2.0* Convert.ToDouble(Tools.ReadWriteReg.read_reg("比例尺分母"))/1000.0; 
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 创建房屋选择集
                Editor ed = ACDBApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList_FW = new TypedValue[1];
                filList_FW.SetValue(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "JZWSX"), 0);
                SelectionFilter filter_FW = new SelectionFilter(filList_FW);
                PromptSelectionResult res_FW = ed.SelectAll(filter_FW);
                SelectionSet SS_FW = res_FW.Value;
                if (SS_FW == null) { return; }
                ObjectId[] objectId_FW = SS_FW.GetObjectIds();
                #endregion
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    ObjectId zt2 = Tools.CADTools.AddNewTextStyle("宋体", "宋体", 1, 0, false);
                    Tools.CADTools.AddNewLayer("房屋结构注记", "Continuous", 2);
                    #region 检查是否存在"房屋属性"表
                    MapApplication mapApp = HostMapApplicationServices.Application;
                    Tables tables = mapApp.ActiveProject.ODTables;
                    Autodesk.Gis.Map.ObjectData.Table table;

                    bool IsxxB = true;
                    if (tables.TablesCount == 0) { IsxxB = true; }
                    if (tables.TablesCount > 0)
                    {
                        StringCollection tbablename = tables.GetTableNames();
                        for (int j = 0; j < tables.TablesCount; j++)
                        {
                            if ("房屋属性" == tbablename[j])
                            {
                                IsxxB = false;
                                break;
                            }
                        }
                    }
                    if (IsxxB == true)
                    {
                        MessageBox.Show("没有属性表！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    #endregion
                    ProgressMeter progressmeter = new ProgressMeter();
                    arrerr.Clear();
                    arrtext.Clear();
                    #region   房屋结构注记
                    progressmeter.SetLimit(objectId_FW.Length);
                    progressmeter.Start("正在检查房屋数据...");
                    Topology.Geometries.Polygon[] FW_area = new Topology.Geometries.Polygon[objectId_FW.Length];//房屋面对象数组
                    for (int i = 0; i < objectId_FW.Length; i++)
                    {
                        progressmeter.MeterProgress();
                        ACDBPolyline FWX = (ACDBPolyline)trans.GetObject(objectId_FW[i], ACDBOpenMode.ForWrite);
                        Extents3d JZXBounds = (Extents3d)FWX.Bounds;
                       // FWX_CAD[i] = FWX;
                        #region 不闭合
                        if (!FWX.Closed)
                        {
                            xh++;
                            err er = new  err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "建筑物线";
                            er.cwsm = "不是闭合线。";
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = FWX.Handle.Value.ToString();
                            arrerr.Add(er);
                             
                            continue;
                        }
                        #endregion
                        #region 面积过小
                        if (FWX.Area < 1.0)
                        {
                            xh++;
                            NCZJDDJFZ.DiJitools.Dijicltools.err er = new NCZJDDJFZ.DiJitools.Dijicltools.err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "建筑物线";
                            er.cwsm = "面积过小。";
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = FWX.Handle.Value.ToString();
                            arrerr.Add(er);

                            continue;
                        }
                        #endregion
                        #region 将建筑物线转换为面、属性数据写入数组
                        table = tables["房屋属性"];
                        Records records = table.GetObjectTableRecords(0, FWX, Autodesk.Gis.Map.Constants.OpenMode.OpenForRead, true);
                        if (records.Count > 0)
                        {
                            FWSXJG sx = new FWSXJG();//房屋属性
                            #region 读取房屋属性
                            foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                            {
                                Autodesk.Gis.Map.Utilities.MapValue val;
                                val = record2[0];
                                sx.建筑物类型0 = val.StrValue;
                                val = record2[1];
                               
                                if (sx.建筑物类型0 == "阳台" || sx.建筑物类型0 == "楼梯" || sx.建筑物类型0 == "檐廊")
                                {
                                    break;
                                }
                                sx.地籍号1 = val.StrValue;
                                val = record2[2];
                                if (sx.地籍号1 == "")
                                {
                                    xh++;
                                    NCZJDDJFZ.DiJitools.Dijicltools.err er = new NCZJDDJFZ.DiJitools.Dijicltools.err();
                                    er.xh = xh;
                                    er.xz = "必须修改";
                                    er.dxlx = "建筑物线";
                                    er.cwsm = "地籍号不可为空！必须重新对房屋进行检查与维护！";
                                    er.minx = JZXBounds.MinPoint.X-0.1;
                                    er.miny = JZXBounds.MinPoint.Y-0.1;
                                    er.maxX = JZXBounds.MaxPoint.X+0.1;
                                    er.maxy = JZXBounds.MaxPoint.Y+0.1;
                                    er.jb = FWX.Handle.Value.ToString();
                                    arrerr.Add(er);
                                    //isch = false;
                                    break;
                                }
                                sx.房屋性质2 = val.StrValue;
                                val = record2[3];
                                sx.房屋产别3 = val.StrValue;
                                val = record2[4];
                                sx.房屋用途4 = val.StrValue;
                                val = record2[5];
                                sx.规划用途5 = val.StrValue;
                                val = record2[6];
                                sx.权利人类型6 = val.StrValue;
                                val = record2[7];
                                sx.产权来源7 = val.StrValue;
                                val = record2[8];
                                sx.共有情况8 = val.StrValue;
                                val = record2[9];
                                sx.房屋幢号9 = val.StrValue;
                                val = record2[10];
                                sx.户号10 = val.StrValue;
                                val = record2[11];
                                sx.总层数11 = val.StrValue;
                                val = record2[12];
                                sx.所在层数12 = val.StrValue;
                                val = record2[13];
                                sx.房屋结构13 = val.StrValue;
                                val = record2[14];
                                sx.竣工时间14 = val.StrValue;
                                val = record2[15];
                                sx.专有建筑面积15 = val.StrValue;
                                val = record2[16];
                                sx.分摊建筑面积16 = val.StrValue;
                                val = record2[17];
                                sx.房屋功能17 = val.StrValue;
                                val = record2[18];
                                sx.附加说明18 = val.StrValue;
                                val = record2[19];
                                sx.调查日期19 = val.StrValue;
                                val = record2[20];
                                sx.调查意见20 = val.StrValue;
                                val = record2[21];
                                sx.调查员21 = val.StrValue;
                                val = record2[22];
                                sx.面积折扣系数22 = val.StrValue;
                                val = record2[23];
                                sx.北23 = val.StrValue;
                                val = record2[24];
                                sx.东24 = val.StrValue;
                                val = record2[25];
                                sx.南25 = val.StrValue;
                                val = record2[26];
                                sx.西26 = val.StrValue;
                                val = record2[27];
                                sx.自动分析地籍号27 = val.StrValue;
                                val = record2[28];
                                sx.备注28 = val.StrValue;
                                val = record2[29];
                                sx.本层建筑面积29 = val.DoubleValue;
                            }
                            #endregion
                            #region 房屋结构注记
                            if(sx.建筑物类型0=="房屋"|| sx.建筑物类型0=="门廊"||sx.建筑物类型0=="车棚")
                            {
                                if (sx.所在层数12 == "1")
                                {
                                    Point3d zjd = new Point3d((JZXBounds.MinPoint.X + JZXBounds.MaxPoint.X) / 2.0, (JZXBounds.MinPoint.Y + JZXBounds.MaxPoint.Y) / 2.0, 0);
                                    DBText text = new DBText();
                                    string fwjg = sx.房屋结构13;
                                    string jhjg = "";
                                    if (fwjg == "钢结构")
                                    {
                                        jhjg = "钢";
                                    }
                                    if (fwjg == "钢、钢筋混凝土结构")
                                    {
                                        jhjg = "钢砼";
                                    }
                                    if (fwjg == "钢筋混凝土结构")
                                    {
                                        jhjg = "砼";
                                    }
                                    if (fwjg == "混合结构")
                                    {
                                        jhjg = "混";
                                    }
                                    if (fwjg == "砖木结构")
                                    {
                                        jhjg = "砖";
                                    }
                                    if (fwjg == "其他结构")
                                    {
                                        jhjg = "其他";
                                    }
                                    jhjg = jhjg + sx.总层数11;
                                    text.TextString = jhjg;
                                    text.Oblique = 0;
                                    text.Rotation = 0;
                                    //text.Position = point;
                                    text.Height = zg;
                                    text.WidthFactor = 1;
                                    text.ColorIndex = 7;
                                    text.HorizontalMode = TextHorizontalMode.TextCenter;//水平对齐方式
                                    text.VerticalMode = TextVerticalMode.TextVerticalMid;
                                    text.AlignmentPoint = zjd;
                                    text.Layer = "房屋结构注记";
                                    text.TextStyleId = zt2;
                                    TypedValue[] tv = new TypedValue[2];
                                    tv[0] = new TypedValue(1001, "SOUTH");
                                    tv[1] = new TypedValue(1000, "FWSXJGZJ");
                                    RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                                    regAppTableRecord.Name = "SOUTH";
                                    if (!symbolTable.Has("SOUTH"))
                                    {
                                        symbolTable.Add(regAppTableRecord);
                                        trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                                    }
                                    if (!text.IsWriteEnabled)
                                    {
                                        text.UpgradeOpen();
                                    }
                                    text.XData = new ResultBuffer(tv);

                                    btr.AppendEntity(text);
                                    trans.AddNewlyCreatedDBObject(text, true);
                                }
                            #endregion
                            }
                        }
                        else
                        {
                            #region 写错
                            xh++;
                            NCZJDDJFZ.DiJitools.Dijicltools.err er = new NCZJDDJFZ.DiJitools.Dijicltools.err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "建筑物线";
                            er.cwsm = "无属性。";
                            er.minx = JZXBounds.MinPoint.X;
                            er.miny = JZXBounds.MinPoint.Y;
                            er.maxX = JZXBounds.MaxPoint.X;
                            er.maxy = JZXBounds.MaxPoint.Y;
                            er.jb = FWX.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                            #endregion
                        }
                        records.Dispose();
                        #endregion
                    }
                    trans.Commit();
                    trans.Dispose();
                    progressmeter.Stop();
                    #endregion
                }
            }
            Class1.WritErr(arrerr);
        }

       
    }
}
