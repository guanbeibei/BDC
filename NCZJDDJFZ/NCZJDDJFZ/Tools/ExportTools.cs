using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Gis.Map;
using GeoAPI.Geometries;
using NCZJDDJFZ.Data;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using NtsPoint = NetTopologySuite.Geometries.Point;


namespace NCZJDDJFZ.Tools
{
    public static class ExportTools
    {

        public static void ExpJzd2Shp()
        {
            ProgressMeter progressmeter = new ProgressMeter();
            progressmeter.Start("正在将界址点数据导出至ESRI shp文件...");

            try
            {
                #region 文件另存对话框设置
                string outShpFilenameNoExt = "";
                SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog();
                diag.Filter = "ESRI shp文件 (*.shp)|*.shp|所有文件 (*.*)|*.*";
                diag.DefaultExt = "shp";
                #endregion

                using (DocumentLock documentLock = AcadApp.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
                {
                    Database db = HostApplicationServices.WorkingDatabase;
                    MapApplication mapApp = HostMapApplicationServices.Application;

                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {

                        #region 创建选择集获取界址点圆
                        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
                        TypedValue[] filList = new TypedValue[2];
                        filList[0] = new TypedValue((int)DxfCode.Start, "CIRCLE");
                        filList[1] = new TypedValue((int)DxfCode.LayerName, "JZP");
                        SelectionFilter filter = new SelectionFilter(filList);
                        PromptSelectionResult res = ed.SelectAll(filter);
                        SelectionSet SS = res.Value;
                        if (SS == null) { return; }
                        ObjectId[] objectIds = SS.GetObjectIds();
                        #endregion
                        progressmeter.SetLimit(objectIds.Length);

                        if (diag.ShowDialog() == DialogResult.OK)
                        {
                            outShpFilenameNoExt = diag.FileName.Trim();
                            if (!string.IsNullOrEmpty(outShpFilenameNoExt))
                            {
                                #region 导出shp文件操作

                                IGeometryFactory gf = new GeometryFactory();

                                // 如果存在同名文件（不包括扩展名），需要提示是否覆盖已存在文件

                                ShapefileDataWriter sfdw = new ShapefileDataWriter(outShpFilenameNoExt, gf, Encoding.Default);

                                // ======================================================
                                // --------------------- DbaseType ----------------------
                                // 字段类型
                                //      C - 字符型  
                                //      Y - 货币型  
                                //      N - 数值型  
                                //      F - 浮点型  
                                //      D - 日期型  
                                //      T - 日期时间型  
                                //      B - 双精度型  
                                //      I - 整型  
                                //      L - 逻辑型 
                                //      M - 备注型  
                                //      G - 通用型  
                                //      C - 字符型（二进制） 
                                //      M - 备注型（二进制） 
                                //      P - 图片型  
                                // -------------------------------------------------------
                                DbaseFieldDescriptor[] dbfileds = new JZD().GetDbaseFieldsDescriptor();

                                // =======================================================
                                IList<IFeature> features = new List<IFeature>();
                                for (int i = 0; i < objectIds.Length; i++)
                                {
                                    progressmeter.MeterProgress();

                                    // 生成图元
                                    Circle circle = (Circle)trans.GetObject(objectIds[i], ACDBOpenMode.ForRead);
                                    ResultBuffer resultBuffer = circle.GetXDataForApplication("SOUTH");
                                    if (resultBuffer == null) continue; // 无属性

                                    NtsPoint pointJzd = new NtsPoint(circle.Center.X, circle.Center.Y);
                                    TypedValue[] XValuesBuffer = resultBuffer.AsArray();
                                    string SXDM = XValuesBuffer[1].Value.ToString().Trim();
                                    string JZDH = XValuesBuffer[3].Value.ToString().Trim(); 

                                    #region ---------------------------生成属性表 ---------------------------
                                    AttributesTable att = new AttributesTable();
                                    //att.AddAttribute("Colunm1", "字符型数据");
                                    //att.AddAttribute("Colunm2", Convert.ToInt64(864));
                                    //att.AddAttribute("Colunm3", Convert.ToDateTime("2017-03-05"));

                                    // 0 界址点号 - 字符串 6
                                    att.AddAttribute("属性代码", SXDM);
                                    // 1 界址点号 - 字符串 8
                                    att.AddAttribute("界址点号", JZDH);
                                    
                                    #endregion -------------------------------------------------------------------

                                    // 根据图元和属性生成要素
                                    Feature f = new Feature(pointJzd, att);

                                    features.Add(f);
                                }

                                // 写入shp和dbf文件
                                if (features.Count > 0)
                                {
                                    DbaseFileHeader outDFH = ShapefileDataWriter.GetHeader(dbfileds, features.Count);
                                    if (outDFH != null)
                                    {
                                        sfdw.Header = outDFH;
                                        sfdw.Write(features);
                                    }
                                }
                                #endregion

                                progressmeter.Stop();
                                MessageBox.Show("输出shp文件成功！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                progressmeter.Stop();
                                MessageBox.Show("输出shp文件失败\r\n文件名不能为空！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                progressmeter.Stop();
                MessageBox.Show("输出shp文件失败\r\n" + ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

    }
}
