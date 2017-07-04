using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using AcaDPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
 
namespace NCZJDDJFZ.Tools
{
    /// <summary>
    /// 宗地图最后处理
    /// </summary>
    class ZDTZHCL
    {
        /// <summary>
        /// 打印图纸
        /// </summary>
        /// <param name="doc">需要打印的文档</param>
        /// <param name="exArea">打印区域</param>
        /// <param name="strPrinter">打印机名称</param>
        /// <param name="iSize">什么纸张</param>
        /// <param name="hs">横行打印还是竖向打印</param>
        /// <param name="bl">打印比例</param>
        private void ploct(Document doc, Extents2d exArea, string strPrinter, string iSize, string hs, double bl)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction tr = db.TransactionManager.StartTransaction())           //开启事务
            {
                //设置布局管理器为当前布局管理器
                LayoutManager layoutMan = LayoutManager.Current;
                //获取当前布局，用GetObject的方式
                Layout currentLayout = (Layout)tr.GetObject(layoutMan.GetLayoutId(layoutMan.CurrentLayout), AcadOpenMode.ForRead);

                //创建一个PlotInfo类，从布局获取信息
                PlotInfo pi = new PlotInfo();
                pi.Layout = currentLayout.ObjectId;
                //从布局获取一个PlotSettings对象的附本
                PlotSettings ps = new PlotSettings(currentLayout.ModelType);
                ps.CopyFrom(currentLayout);
                //创建PlotSettingsValidator对象，通过它来改变PlotSettings.
                PlotSettingsValidator psv = PlotSettingsValidator.Current;
                //设置打印窗口范围
                psv.SetPlotWindowArea(ps, exArea);
                psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);
                //设置自定义比例
                psv.SetUseStandardScale(ps, false);
                psv.SetCustomPrintScale(ps, new CustomScale(bl, 1));

                //单位为毫米
               // psv.SetPlotPaperUnits(ps, PlotPaperUnit.Millimeters);

                //设置居中打印
                psv.SetPlotCentered(ps, true);
                //设置横向打印还是纵向打印

                if (hs == "H")
                {
                    psv.SetPlotRotation(ps, PlotRotation.Degrees090);
                }

                //if ((exArea.MaxPoint.X - exArea.MinPoint.X) > (exArea.MaxPoint.Y - exArea.MinPoint.Y))
                //{
                //    psv.SetPlotRotation(ps, PlotRotation.Degrees090);                   //横向
                //}
                else
                {
                    psv.SetPlotRotation(ps, PlotRotation.Degrees000);                   //纵向  
                }
                //设置样式表1
                //StringCollection styleSheet = psv.GetPlotStyleSheetList();//获取样式表集合
                try
                {
                    psv.SetCurrentStyleSheet(ps, "acad.ctb");
                }
                catch  
                {
                    MessageBox.Show("请指定打印样式表", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                //设置打印设备
                psv.SetPlotConfigurationName(ps, strPrinter, null);                 //打印A3
                StringCollection sMediasA3 = psv.GetCanonicalMediaNameList(ps);
                //bool isA3 = false;
                int i = 0;
                for (i = 0; i < sMediasA3.Count; i++)
                {
                    if (sMediasA3[i] == iSize)
                    {
                        psv.SetPlotConfigurationName(ps, strPrinter, sMediasA3[i]);
                        break;
                    }
                }

                //重写打印信息
                pi.OverrideSettings = ps;
                //确认打印信息
                PlotInfoValidator piv = new PlotInfoValidator();
                piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                piv.Validate(pi);
                Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", 0);
                try
                {
                    //检查打印状态
                    if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
                    {
                        //开户引擎
                        using (PlotEngine pe = PlotFactory.CreatePublishEngine())
                        {
                            PlotProgressDialog ppDialog = new PlotProgressDialog(false, 1, true);
                            using (ppDialog)
                            {
                                //显示进程对话框
                                ppDialog.OnBeginPlot();
                                ppDialog.IsVisible = true;
                                //开始打印布局
                                pe.BeginPlot(ppDialog, null);
                                //sw.WriteLine(DateTime.Now + "       pe.BeginPlot()开始执行.");
                                //显示当前打印相关信息
                                pe.BeginDocument(pi, doc.Name, null, 1, false, null);
                                //打印第一个视口
                                PlotPageInfo ppInfo = new PlotPageInfo();
                                //sw.WriteLine(DateTime.Now + "       创建一个PlotPageInfo类");
                                pe.BeginPage(ppInfo, pi, true, null);    //此处第三个参数至为关键，如果设为false则会不能执行.
                                pe.BeginGenerateGraphics(null);
                                pe.EndGenerateGraphics(null);
                                //完成打印视口
                                pe.EndPage(null);
                                ppDialog.OnEndSheet();
                                //完成文档打印
                                pe.EndDocument(null);
                                //完成打印操作
                                ppDialog.OnEndPlot();
                                pe.EndPlot(null);
                            }
                        }
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("BACKGROUNDPLOT", 2);
            }
        }


        /// <summary>
        /// 宗地图打印
        /// </summary>
        /// <param name="dyjmc">打印机名称</param>
        /// <param name="A3">A3打印纸名称</param>
        /// <param name="A4">A4打印纸名称</param>
        public void DY(string dyjmc, string A3, string A4)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, AcadOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], AcadOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, AcadOpenMode.ForWrite);
                    TypedValue[] filList = new TypedValue[2];
                    filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                    filList[1] = new TypedValue(8, "本宗界址线");
                    SelectionFilter filter = new SelectionFilter(filList);
                    Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                    PromptSelectionResult res = ed.SelectAll(filter);
                    SelectionSet SS = res.Value;
                    if (SS == null)
                    {
                        MessageBox.Show("此没有界址线！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    ObjectId[] objectId = SS.GetObjectIds();
                    AcaDPolyline Pline = (AcaDPolyline)trans.GetObject(objectId[0], AcadOpenMode.ForRead);
                    ResultBuffer resultBuffer1;
                    string zz = "";
                    string hs = "";
                    int blc = 1;
                    double Xmin = 0, Ymin = 0, Xmax = 0, Ymax = 0;
                    try
                    {
                        resultBuffer1 = Pline.XData;

                        TypedValue[] tv2 = resultBuffer1.AsArray();
                        TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "ZDTDY");
                        zz = tv3[1].Value.ToString();
                        hs = tv3[2].Value.ToString();
                        blc = Convert.ToInt32(tv3[3].Value);
                        Xmin = (double)tv3[4].Value;
                        Ymin = (double)tv3[5].Value;
                        Xmax = (double)tv3[6].Value ;
                        Ymax = (double)tv3[7].Value;
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("此界址线没有没有扩展属性了", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    Extents2d daqy = new Extents2d(new Point2d(Xmin, Ymin), new Point2d(Xmax, Ymax));
                    double SCALE = 1000.0 / blc;
                    string ZZMC = "";
                    if (zz == "A3")
                    { ZZMC = A3; }
                    if (zz == "A4")
                    {
                        ZZMC = A4;
                    }
                    ploct(AcadApplication.DocumentManager.MdiActiveDocument, daqy, dyjmc, ZZMC, hs, SCALE);
                    trans.Commit();
                }
            }
        }
       
       
    }
}
