using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.ObjectData;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using ACDBPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using NtsGeometry = NetTopologySuite.Geometries.Geometry;
using NtsPolygon = NetTopologySuite.Geometries.Polygon;
using NPOI.HSSF.UserModel;
using System.Data.SqlClient;

namespace NCZJDDJFZ.Tools
{

    /// <summary>读树所有节点和属性
    /// </summary>
    public class GetAllnodes
    {
        ArrayList mArrayNodeName = new ArrayList();
        ArrayList mArrayNodeTag = new ArrayList();
        ArrayList mArrayNodeText = new ArrayList();
        ArrayList mArrayNodeFullPath = new ArrayList();
        public ArrayList ArrayNodeName
        {
            get { return mArrayNodeName; }
        }
        public ArrayList ArrayNodeTag
        {
            get { return mArrayNodeTag; }
        }
        public ArrayList ArrayNodeText
        {
            get { return mArrayNodeText; }
        }
        public ArrayList ArrayNodeFullPath
        {
            get { return mArrayNodeFullPath; }
        }

        public void GetAllnode(TreeNodeCollection mnodes)
        {
            foreach (TreeNode node in mnodes)
            {
                if (node.Nodes.Count != 0)
                {
                    GetAllnode(node.Nodes);
                }
                mArrayNodeName.Add(node.Name);
                mArrayNodeTag.Add(node.Tag.ToString());
                mArrayNodeText.Add(node.Text);
                mArrayNodeFullPath.Add(node.FullPath);
            }
        }
    }
    public static class ReadWriteReg
    {
        /// <summary>
        /// 在HKEY_LOCAL_MACHINE\SOFTWARE下创建“NCZJDDJFZAPPSET”子项并设置键值
        /// </summary>
        /// <param name="zx1">键名称</param>
        /// <param name="value">键值</param>
        /// <returns>是否成功</returns>
        public static bool write_reg(string zx1, string value)
        {
            Microsoft.Win32.RegistryKey rsg = null;
            bool Iscz = false;
            try
            {
                rsg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
                foreach (string names in rsg.GetSubKeyNames())
                {
                    if (names == "NCZJDDJFZAPPSET")
                    {
                        Iscz = true;
                        break;
                    }
                }
                if (Iscz)
                {
                    rsg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\NCZJDDJFZAPPSET", true);
                    rsg.SetValue(zx1, value);
                    Iscz = true;
                    return Iscz;
                }
                else
                {
                    rsg.CreateSubKey("NCZJDDJFZAPPSET");
                    rsg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\NCZJDDJFZAPPSET", true);
                    rsg.SetValue(zx1, value);
                    Iscz = true;
                    return Iscz;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                rsg.Close();

            }
            return Iscz;
        }
        /// <summary>
        /// 读HKEY_LOCAL_MACHINE\SOFTWARE\\BYLAPPSET项下的键值
        /// </summary>
        /// <param name="zx1">键名称</param>
        /// <returns>键值</returns>
        public static string read_reg(string zx1)
        {
            string value = "";
            Microsoft.Win32.RegistryKey rsg = null;
            rsg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\NCZJDDJFZAPPSET\\");
            if (rsg == null)
            {
                return value;
            }
            else
            {
                value = rsg.GetValue(zx1, "").ToString();
            }
            rsg.Close();
            return value;
        }
    }

    public static class AnalysingTool
    {
        public static bool Overlay()
        {
            ProgressMeter progressmeter = new ProgressMeter();
            progressmeter.Start("1/3 正在读取分析数据...");

            string SHPFILE_JBNT = ReadWriteReg.read_reg("基本农田SHP文件");
            // 检查Shape文件是否存在
            string DBFFILE_JBNT = SHPFILE_JBNT.Substring(0, SHPFILE_JBNT.Length - 3) + "dbf";
            if (!FileExisting(SHPFILE_JBNT) || !FileExisting(DBFFILE_JBNT))
                return false;
            // 读取SHP文件并重建索引
            ShapeFile SHP_JBNT = new ShapeFile(SHPFILE_JBNT, true);
            SHP_JBNT.RebuildSpatialIndex();
            progressmeter.Stop();

            using (DocumentLock documentLock = AcadApp.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {

                Database db = HostApplicationServices.WorkingDatabase;

                #region 创建选择集获取宗地数据
                Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[2];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res = ed.SelectAll(filter);
                SelectionSet SS = res.Value;
                if (SS == null)
                {
                    MessageBox.Show("没有界址线。", "分析失败", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
                ObjectId[] objectIdZD = SS.GetObjectIds();//界址线
                #endregion

                #region 创建房屋选择集
                //Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList_FW = new TypedValue[1];
                filList_FW.SetValue(new TypedValue((int)DxfCode.ExtendedDataRegAppName, "JZWSX"), 0);
                SelectionFilter filter_FW = new SelectionFilter(filList_FW);
                PromptSelectionResult res_FW = ed.SelectAll(filter_FW);
                SelectionSet SS_FW = res_FW.Value;
                if (SS_FW == null) { return false; }
                ObjectId[] objectId_FW = SS_FW.GetObjectIds();
                #endregion

                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForWrite, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);

                    MapApplication mapApp = HostMapApplicationServices.Application;
                    Tables tables = mapApp.ActiveProject.ODTables;
                    Autodesk.Gis.Map.ObjectData.Table tableZD;
                    Autodesk.Gis.Map.ObjectData.Table tableFW;
                    tableZD = tables["建筑面积表"];
                    tableFW = tables["房屋属性"];

                    progressmeter.Start("2/3 正在分析宗地...");
                    progressmeter.SetLimit(objectIdZD.Length);
                    #region 循环每一个宗地
                    for (int i = 0; i < objectIdZD.Length; i++)
                    {
                        ACDBPolyline JZX = (ACDBPolyline)trans.GetObject(objectIdZD[i], ACDBOpenMode.ForWrite);
                        // 宅基地宗地面
                        NtsPolygon ZjdPolygon = (NtsPolygon)Tools.CADTools.ConvertToSharpMapIPolygon(JZX);
                        // 宅基地宗地面外接矩形
                        GeoAPI.Geometries.Envelope ZjdEnv = new GeoAPI.Geometries.Envelope(
                            ZjdPolygon.EnvelopeInternal.MinX,
                            ZjdPolygon.EnvelopeInternal.MaxX,
                            ZjdPolygon.EnvelopeInternal.MinY,
                            ZjdPolygon.EnvelopeInternal.MaxY);
                        System.Collections.ObjectModel.Collection<uint> ObjIdsJBNT = SHP_JBNT.GetObjectIDsInView(ZjdEnv);
                        // 循环与该宗地面相交的每一个基本农田面并进行分析计算
                        // 1、获取图斑类型
                        // 2、获取宗地所占图斑面积
                        // 3、分类统计
                        // 4、将分析结果保存至图形属性中
                        Dictionary<string, double> PolygonAreaDic = GetDefaultAreaDic();
                        foreach (uint fId in ObjIdsJBNT)
                        {
                            FeatureDataRow fdr = SHP_JBNT.GetFeature(fId);
                            string dlbm = fdr["DLBM"].ToString();

                            NtsGeometry leftGeo = null;
                            if (ZjdPolygon.Intersects(fdr.Geometry))
                            {
                                leftGeo = ZjdPolygon.Intersection(fdr.Geometry) as NtsGeometry;

                                if (leftGeo != null)
                                {
                                    double area = leftGeo.Area;

                                    PolygonAreaDic[dlbm] += Math.Abs(area);
                                }
                            }
                        }
                        if (ObjIdsJBNT.Count > 0)
                        {
                            string FLMJ = "";
                            double sum = 0.0;
                            foreach (KeyValuePair<string, double> kv in PolygonAreaDic)
                            {
                                FLMJ += kv.Key + "," + kv.Value.ToString("0.0000") + "|";
                                sum += kv.Value;
                            }
                            FLMJ = FLMJ.Substring(0, FLMJ.Length - 1);

                            Records records = tableZD.GetObjectTableRecords(0, objectIdZD[i], Autodesk.Gis.Map.Constants.OpenMode.OpenForWrite, true);
                            if (records.Count > 0)
                            {
                                foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                                {
                                    Autodesk.Gis.Map.Utilities.MapValue val;
                                    if (sum > 0)
                                    {
                                        //是否侵占基本农田
                                        val = record2[3];
                                        val.Assign("是");
                                        //疑似侵占
                                        if (sum < 1.0)
                                        {
                                            val = record2[4];
                                            val.Assign("是");
                                        }
                                        else
                                        {
                                            val = record2[4];
                                            val.Assign("否");
                                        }
                                        //侵占分类面积
                                        val = record2[5];
                                        val.Assign(FLMJ);
                                    }
                                    else
                                    {
                                        //是否侵占基本农田
                                        val = record2[3];
                                        val.Assign("否");
                                        //疑似侵占                                        
                                        val = record2[4];
                                        val.Assign("否");
                                    }
                                    records.UpdateRecord(record2);
                                }
                            }

                        }
                        progressmeter.MeterProgress();
                    }
                    progressmeter.Stop();
                    #endregion

                    progressmeter.Start("3/3 正在分析房屋...");
                    progressmeter.SetLimit(objectId_FW.Length);
                    #region 循环每一个房屋面
                    for (int i = 0; i < objectId_FW.Length; i++)
                    {
                        ACDBPolyline FWX = (ACDBPolyline)trans.GetObject(objectId_FW[i], ACDBOpenMode.ForWrite);
                        // 房屋面
                        NtsPolygon FwPolygon = (NtsPolygon)Tools.CADTools.ConvertToSharpMapIPolygon(FWX);
                        // 房屋面外接矩形
                        GeoAPI.Geometries.Envelope FwEnv = new GeoAPI.Geometries.Envelope(
                            FwPolygon.EnvelopeInternal.MinX,
                            FwPolygon.EnvelopeInternal.MaxX,
                            FwPolygon.EnvelopeInternal.MinY,
                            FwPolygon.EnvelopeInternal.MaxY);
                        System.Collections.ObjectModel.Collection<uint> ObjIdsJBNT = SHP_JBNT.GetObjectIDsInView(FwEnv);
                        // 循环与该宗地面相交的每一个基本农田面并进行分析计算
                        // 1、获取图斑类型
                        // 2、获取宗地所占图斑面积
                        // 3、分类统计
                        // 4、将分析结果保存至图形属性中
                        Dictionary<string, double> PolygonAreaDic = GetDefaultAreaDic();
                        foreach (uint fId in ObjIdsJBNT)
                        {
                            FeatureDataRow fdr = SHP_JBNT.GetFeature(fId);
                            string dlbm = fdr["DLBM"].ToString();

                            NtsGeometry leftGeo = null;
                            if (FwPolygon.Intersects(fdr.Geometry))
                            {
                                leftGeo = FwPolygon.Intersection(fdr.Geometry) as NtsGeometry;

                                if (leftGeo != null)
                                {
                                    double area = leftGeo.Area;

                                    PolygonAreaDic[dlbm] += Math.Abs(area);
                                }
                            }
                        }
                        if (ObjIdsJBNT.Count > 0)
                        {
                            string FLMJ = "";
                            double sum = 0.0;
                            foreach (KeyValuePair<string, double> kv in PolygonAreaDic)
                            {
                                FLMJ += kv.Key + "," + kv.Value.ToString() + "|";
                                sum += kv.Value;
                            }
                            FLMJ = FLMJ.Substring(0, FLMJ.Length - 1);

                            Records records = tableFW.GetObjectTableRecords(0, objectId_FW[i], Autodesk.Gis.Map.Constants.OpenMode.OpenForWrite, true);
                            if (records.Count > 0)
                            {
                                foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                                {
                                    Autodesk.Gis.Map.Utilities.MapValue val;
                                    if (sum > 0.0)
                                    {
                                        //是否侵占基本农田
                                        val = record2[30];
                                        val.Assign("是");
                                        //疑似侵占
                                        if (sum < 1.0)
                                        {
                                            val = record2[31];
                                            val.Assign("是");
                                        }
                                        //侵占分类面积
                                        val = record2[32];
                                        val.Assign(FLMJ);
                                    }
                                    else
                                    {
                                        //是否侵占基本农田
                                        val = record2[30];
                                        val.Assign("否");
                                        //疑似侵占
                                        val = record2[31];
                                        val.Assign("否");
                                    }
                                    records.UpdateRecord(record2);
                                }
                            }

                        }




                        progressmeter.MeterProgress();
                    }
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                }


            }
            progressmeter.Stop();
            MessageBox.Show("基本农田叠加分析完成！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        public static bool FileExisting(string FilePath)
        {
            FileInfo info = new FileInfo(FilePath);
            if (!info.Exists)
            {
                MessageBox.Show(FilePath + " 文件不存在！", "不动产软件", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
                return true;
        }

        public static Dictionary<string, double> GetDefaultAreaDic()
        {
            System.Collections.Generic.Dictionary<string, double> dic = new Dictionary<string, double>();
            dic.Add("011", 0.0);//水田
            dic.Add("012", 0.0);//水浇地
            dic.Add("013", 0.0);//旱地

            dic.Add("021", 0.0);//果园
            dic.Add("022", 0.0);//茶园
            dic.Add("023", 0.0);//其他园地

            dic.Add("031", 0.0);//有林地
            dic.Add("032", 0.0);//灌木林地
            dic.Add("033", 0.0);//其他林地

            dic.Add("041", 0.0);
            dic.Add("042", 0.0);
            dic.Add("043", 0.0);

            dic.Add("051", 0.0);
            dic.Add("052", 0.0);
            dic.Add("053", 0.0);
            dic.Add("054", 0.0);

            dic.Add("061", 0.0);
            dic.Add("062", 0.0);
            dic.Add("063", 0.0);

            dic.Add("071", 0.0);
            dic.Add("072", 0.0);

            dic.Add("081", 0.0);
            dic.Add("082", 0.0);
            dic.Add("083", 0.0);
            dic.Add("084", 0.0);
            dic.Add("085", 0.0);
            dic.Add("086", 0.0);
            dic.Add("087", 0.0);
            dic.Add("088", 0.0);

            dic.Add("091", 0.0);
            dic.Add("092", 0.0);
            dic.Add("093", 0.0);
            dic.Add("094", 0.0);
            dic.Add("095", 0.0);

            dic.Add("101", 0.0);
            dic.Add("102", 0.0);
            dic.Add("103", 0.0);
            dic.Add("104", 0.0);//农村道路
            dic.Add("105", 0.0);
            dic.Add("106", 0.0);
            dic.Add("107", 0.0);

            dic.Add("111", 0.0);
            dic.Add("112", 0.0);
            dic.Add("113", 0.0);
            dic.Add("114", 0.0);//坑塘水面
            dic.Add("115", 0.0);
            dic.Add("116", 0.0);
            dic.Add("117", 0.0);//沟渠
            dic.Add("118", 0.0);
            dic.Add("119", 0.0);

            dic.Add("121", 0.0);
            dic.Add("122", 0.0);//设施农用地
            dic.Add("123", 0.0);//田坎
            dic.Add("124", 0.0);
            dic.Add("125", 0.0);
            dic.Add("126", 0.0);
            dic.Add("127", 0.0);

            return dic;
        }

        // 根据地类编码查找地类名称
        /// <summary>
        /// 根据地类编码查找地类名称
        /// </summary>
        /// <param name="dlbm">地类编码</param>
        /// <returns>地类名称</returns>
        public static string GetDLMC(string dlbm)
        {
            string dlmc = "";
            string[] dlbm_mc ={ "水田","水浇地","旱地","果园","茶园","其它园地",
                             "有林地","灌木林地","其它林地","天然牧草地","人工牧草地",
                             "其它草地","批发零售用","住宿餐饮用","商务金融用","其它商服用",
                             "工业用地","采矿用地","仓储用地","城镇住宅用","农村宅基地","机关团体用",
                             "新闻出版用","科教用地","医卫慈善用","文体娱乐用","公共设施用",
                             "公园与绿地","风景名胜设施用地","军事设施用地","使领馆用地",
                             "监教场所用地","宗教用地","殡葬用地","铁路用地","公路用地","街巷用地",
                             "农村道路","机场用地","港口码头用地","管道运输用地","河流水面","湖泊水面",
                             "水库水面","坑塘水面","沿海滩涂","内陆滩涂","沟渠","水工建筑用地","冰川及永久积",
                             "空闲地","设施农用地","田坎","盐碱地","沼泽地","沙地","裸地"};
            string[] dlbm_dm ={"011","012","013","021","022","023","031","032","033",
                               "041","042","043","051","052","053","054","061","062",
                               "063","071","072","081","082","083","084","085","086",
                               "087","088","091","092","093","094","095","101","102", 
                               "103","104","105","106","107","111","112","113","114",
                               "115","116","117","118","119","121","122","123","124",
                               "125","126","127"};
            for (int i = 0; i < dlbm_dm.Length; i++)
            {
                if (dlbm == dlbm_dm[i])
                {
                    dlmc = dlbm_mc[i];
                }
            }
            return dlmc;
        }

        // 根据地类名称查找地类编码
        /// <summary>
        /// 根据地类名称查找地类编码
        /// </summary>
        /// <param name="dlmc">地类名称</param>
        /// <returns>地类编码</returns>
        public static string GetDLBM(string dlmc)
        {
            string dlbm = "";
            string[] dlbm_mc ={ "水田","水浇地","旱地","果园","茶园","其它园地",
                             "有林地","灌木林地","其它林地","天然牧草地","人工牧草地",
                             "其它草地","批发零售用","住宿餐饮用","商务金融用","其它商服用",
                             "工业用地","采矿用地","仓储用地","城镇住宅用","农村宅基地","机关团体用",
                             "新闻出版用","科教用地","医卫慈善用","文体娱乐用","公共设施用",
                             "公园与绿地","风景名胜设施用地","军事设施用地","使领馆用地",
                             "监教场所用地","宗教用地","殡葬用地","铁路用地","公路用地","街巷用地",
                             "农村道路","机场用地","港口码头用地","管道运输用地","河流水面","湖泊水面",
                             "水库水面","坑塘水面","沿海滩涂","内陆滩涂","沟渠","水工建筑用地","冰川及永久积",
                             "空闲地","设施农用地","田坎","盐碱地","沼泽地","沙地","裸地"};
            string[] dlbm_dm ={"011","012","013","021","022","023","031","032","033",
                               "041","042","043","051","052","053","054","061","062",
                               "063","071","072","081","082","083","084","085","086",
                               "087","088","091","092","093","094","095","101","102", 
                               "103","104","105","106","107","111","112","113","114",
                               "115","116","117","118","119","121","122","123","124",
                               "125","126","127"};
            for (int i = 0; i < dlbm_dm.Length; i++)
            {
                if (dlmc == dlbm_mc[i])
                {
                    dlbm = dlbm_dm[i];
                }
            }
            return dlbm;
        }

        public static void ExpReportXLS()
        {

            try
            {

                using (DocumentLock documentLock = AcadApp.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
                {
                    Database db = HostApplicationServices.WorkingDatabase;
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);

                        MapApplication mapApp = HostMapApplicationServices.Application;
                        Tables tables = mapApp.ActiveProject.ODTables;
                        Autodesk.Gis.Map.ObjectData.Table tableZD;
                        tableZD = tables["建筑面积表"];
                        SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);

                        #region 创建选择集获取宗地信息
                        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
                        TypedValue[] filList = new TypedValue[2];
                        filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                        filList[1] = new TypedValue(8, "JZD");
                        SelectionFilter filter = new SelectionFilter(filList);
                        PromptSelectionResult res = ed.SelectAll(filter);
                        SelectionSet SS = res.Value;
                        if (SS == null)
                        {
                            MessageBox.Show("没有界址线。", "分析失败", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        ObjectId[] objectId = SS.GetObjectIds();
                        #endregion

                        #region 文件另存对话框设置
                        string Filename = "";
                        SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog();
                        diag.Filter = "Excel文件 (*.xls)|*.xls|所有文件 (*.*)|*.*";
                        diag.DefaultExt = "xls";
                        #endregion

                        if (diag.ShowDialog() == DialogResult.OK)
                        {
                            #region




                            #endregion
                            ProgressMeter progressmeter = new ProgressMeter();
                            progressmeter.Start("正在将分类面积信息导出至Excel中...");
                            progressmeter.SetLimit(objectId.Length);

                            Filename = diag.FileName.Trim();
                            if (Filename != "")
                            {
                                HSSFWorkbook hSSFWorkbook = new HSSFWorkbook();
                                HSSFSheet sheet1 = hSSFWorkbook.CreateSheet("Sheet1");
                                HSSFRow row1 = sheet1.CreateRow(0);
                                #region 添加表头
                                row1.CreateCell(0).SetCellValue("序号");
                                row1.CreateCell(1).SetCellValue("权利人");
                                row1.CreateCell(2).SetCellValue("身份证号");
                                row1.CreateCell(3).SetCellValue("不动产权利类型");// +                                
                                row1.CreateCell(4).SetCellValue("土地坐落");
                                row1.CreateCell(5).SetCellValue("不动产单元号");// +
                                row1.CreateCell(6).SetCellValue("宗地面积/房屋面积");
                                row1.CreateCell(7).SetCellValue("宗地用途/房屋用途");// +
                                row1.CreateCell(8).SetCellValue("备注");//留空
                                #endregion
                                string connectionString = Tools.Uitl.GetConnectionString();
                                using (SqlConnection connection = new SqlConnection(connectionString))
                                {
                                    connection.Open();
                                    int count = 1;
                                    for (int i = 0; i < objectId.Length; i++)
                                    {
                                        progressmeter.MeterProgress();

                                        string QZ = "否";
                                        string YSQZ = "否";
                                        Records records = tableZD.GetObjectTableRecords(0, objectId[i], Autodesk.Gis.Map.Constants.OpenMode.OpenForRead, true);
                                        if (records.Count > 0)
                                        {
                                            foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                                            {
                                                QZ = record2[3].StrValue.Trim();
                                                YSQZ = record2[4].StrValue.Trim();
                                            }
                                        }
                                        records.Dispose();

                                        if (QZ == "是") continue;

                                        #region 从数据库中查询图形相关数据
                                        ACDBPolyline pline_jzx = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);

                                        ResultBuffer resultBuffer = pline_jzx.XData;
                                        TypedValue[] tv2 = resultBuffer.AsArray();

                                        TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                                        string txdjh = tv3[2].Value.ToString().Trim();

                                        TypedValue[] tv4 = Tools.CADTools.GetApp(tv2, "QHDM");
                                        string XZQH = tv4[1].Value.ToString().Trim();

                                        txdjh = XZQH + txdjh.Substring(0, 6) + txdjh.Substring(6).PadLeft(5, '0');
                                        string selconne = "SELECT * FROM DCB WHERE (DJH = '" + txdjh + "')";

                                        string qlr = "";
                                        string sfzh = "";
                                        string tdzl = "";
                                        string zdmj = "";
                                        string jzmj = "";
                                        string bdcdyh = "";

                                        bool iszd = false;//是否找到了

                                        SqlCommand command = new SqlCommand(selconne, connection);//连接数据库
                                        SqlDataReader reader = command.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            bdcdyh = reader["WZDJH"].ToString().Trim() + "F00010001";
                                            qlr = reader["QLR"].ToString().Trim();
                                            tdzl = reader["TDZL"].ToString().Trim();
                                            sfzh = reader["KCR"].ToString().Trim();
                                            zdmj = reader["ZDMJ"].ToString().Trim();
                                            jzmj = reader["JZMJ"].ToString().Trim();

                                            iszd = true;
                                        }
                                        reader.Close();
                                        #endregion

                                        if (!iszd) continue;

                                        #region Excel文件操作


                                        HSSFRow row2 = sheet1.CreateRow(count);
                                        // 序号
                                        row2.CreateCell(0).SetCellValue(count.ToString());
                                        // 权利人
                                        //if (iszd)
                                            row2.CreateCell(1).SetCellValue(qlr);
                                        //else
                                            //row2.CreateCell(1).SetCellValue("错误：图形中地籍号为【" + txdjh + "】的宗地相关信息在数据库中未找到，请检查处理！");
                                        // 身份证号码
                                        row2.CreateCell(2).SetCellValue(sfzh);
                                        // 不动产权利类型
                                        row2.CreateCell(3).SetCellValue("宅基地使用权及房屋所有权");
                                        // 土地坐落
                                        row2.CreateCell(4).SetCellValue(tdzl);
                                        // 不动产单元号
                                        row2.CreateCell(5).SetCellValue(bdcdyh);
                                        // 面积
                                        // 宗地面积/房屋面积
                                        row2.CreateCell(6).SetCellValue(zdmj + "/" + jzmj);
                                        // 用途
                                        // 宗地用途/房屋用途
                                        row2.CreateCell(7).SetCellValue("农村宅基地/住宅");
                                        // 核实结果
                                        row2.CreateCell(8).SetCellValue("");

                                        #endregion
                                        count++;
                                    }
                                }
                                FileStream FileStreamfile = new FileStream(Filename, FileMode.Create);
                                hSSFWorkbook.Write(FileStreamfile);
                                FileStreamfile.Close();

                                progressmeter.Stop();
                                MessageBox.Show("保存成功！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                progressmeter.Stop();
                                MessageBox.Show("文件名不能为空！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }

        public static void ExpAll2XLS()
        {

            try
            {

                using (DocumentLock documentLock = AcadApp.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
                {
                    Database db = HostApplicationServices.WorkingDatabase;
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);

                        MapApplication mapApp = HostMapApplicationServices.Application;
                        Tables tables = mapApp.ActiveProject.ODTables;
                        Autodesk.Gis.Map.ObjectData.Table tableZD;
                        tableZD = tables["建筑面积表"];
                        SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);

                        #region 创建选择集获取宗地信息
                        Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
                        TypedValue[] filList = new TypedValue[2];
                        filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                        filList[1] = new TypedValue(8, "JZD");
                        SelectionFilter filter = new SelectionFilter(filList);
                        PromptSelectionResult res = ed.SelectAll(filter);
                        SelectionSet SS = res.Value;
                        if (SS == null)
                        {
                            MessageBox.Show("没有界址线。", "分析失败", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        ObjectId[] objectId = SS.GetObjectIds();
                        #endregion

                        #region 文件另存对话框设置
                        string Filename = "";
                        SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog();
                        diag.Filter = "Excel文件 (*.xls)|*.xls|所有文件 (*.*)|*.*";
                        diag.DefaultExt = "xls";
                        #endregion

                        if (diag.ShowDialog() == DialogResult.OK)
                        {
                            ProgressMeter progressmeter = new ProgressMeter();
                            progressmeter.Start("正在将所有宗地及房屋信息导出至Excel中...");
                            progressmeter.SetLimit(objectId.Length);

                            Filename = diag.FileName.Trim();
                            if (Filename != "")
                            {
                                HSSFWorkbook hSSFWorkbook = new HSSFWorkbook();
                                HSSFSheet sheet1 = hSSFWorkbook.CreateSheet("Sheet1");
                                HSSFRow row1 = sheet1.CreateRow(0);
                                #region 添加表头
                                row1.CreateCell(0).SetCellValue("序号");
                                row1.CreateCell(1).SetCellValue("权利人");
                                row1.CreateCell(2).SetCellValue("身份证号");
                                row1.CreateCell(3).SetCellValue("土地坐落");
                                row1.CreateCell(4).SetCellValue("宗地面积");
                                row1.CreateCell(5).SetCellValue("房屋面积");
                                row1.CreateCell(6).SetCellValue("是否侵占基本农田");
                                row1.CreateCell(7).SetCellValue("疑似侵占");

                                row1.CreateCell(8).SetCellValue("水田");
                                row1.CreateCell(9).SetCellValue("水浇地");
                                row1.CreateCell(10).SetCellValue("旱地");
                                row1.CreateCell(11).SetCellValue("果园");
                                row1.CreateCell(12).SetCellValue("茶园");
                                row1.CreateCell(13).SetCellValue("其他园地");
                                row1.CreateCell(14).SetCellValue("有林地");
                                row1.CreateCell(15).SetCellValue("灌木林地");
                                row1.CreateCell(16).SetCellValue("其他林地");
                                row1.CreateCell(17).SetCellValue("农村道路");
                                row1.CreateCell(18).SetCellValue("坑塘水泊");
                                row1.CreateCell(19).SetCellValue("沟渠");
                                row1.CreateCell(20).SetCellValue("设施农业用地");
                                row1.CreateCell(21).SetCellValue("田坎");
                                #endregion
                                string connectionString = Tools.Uitl.GetConnectionString();
                                using (SqlConnection connection = new SqlConnection(connectionString))
                                {
                                    connection.Open();
                                    int count = 1;
                                    for (int i = 0; i < objectId.Length; i++)
                                    {
                                        progressmeter.MeterProgress();

                                        string QZ = "否";
                                        string YSQZ = "否";
                                        string QZMJ = "";
                                        Records records = tableZD.GetObjectTableRecords(0, objectId[i], Autodesk.Gis.Map.Constants.OpenMode.OpenForRead, true);
                                        if (records.Count > 0)
                                        {
                                            foreach (Autodesk.Gis.Map.ObjectData.Record record2 in records)
                                            {
                                                QZ = record2[3].StrValue.Trim();
                                                YSQZ = record2[4].StrValue.Trim();
                                                QZMJ = record2[5].StrValue.Trim();
                                            }
                                        }
                                        records.Dispose();

                                        #region 从数据库中查询图形相关数据
                                        ACDBPolyline pline_jzx = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);

                                        ResultBuffer resultBuffer = pline_jzx.XData;
                                        TypedValue[] tv2 = resultBuffer.AsArray();

                                        TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                                        string txdjh = tv3[2].Value.ToString().Trim();

                                        TypedValue[] tv4 = Tools.CADTools.GetApp(tv2, "QHDM");
                                        string XZQH = tv4[1].Value.ToString().Trim();

                                        txdjh = XZQH + txdjh.Substring(0, 6) + txdjh.Substring(6).PadLeft(5, '0');
                                        string selconne = "SELECT * FROM DCB WHERE (DJH = '" + txdjh + "')";

                                        string qlr = "";
                                        string sfzh = "";
                                        string tdzl = "";
                                        string zdmj = "";
                                        string jzmj = "";

                                        bool iszd = false;//是否找到了

                                        SqlCommand command = new SqlCommand(selconne, connection);//连接数据库
                                        SqlDataReader reader = command.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            qlr = reader["QLR"].ToString().Trim();
                                            tdzl = reader["TDZL"].ToString().Trim();
                                            sfzh = reader["KCR"].ToString().Trim();
                                            zdmj = reader["ZDMJ"].ToString().Trim();
                                            jzmj = reader["JZMJ"].ToString().Trim();

                                            iszd = true;
                                        }
                                        reader.Close();
                                        #endregion

                                        if (!iszd) continue;

                                        #region Excel文件操作

                                        HSSFRow row2 = sheet1.CreateRow(count);
                                        // 序号
                                        row2.CreateCell(0).SetCellValue(count.ToString());
                                        // 权利人
                                        //if (iszd)
                                        row2.CreateCell(1).SetCellValue(qlr);
                                        //else
                                        //row2.CreateCell(1).SetCellValue("错误：图形中地籍号为【" + txdjh + "】的宗地相关信息在数据库中未找到，请检查处理！");
                                        // 身份证号码
                                        row2.CreateCell(2).SetCellValue(sfzh);
                                        // 土地坐落
                                        row2.CreateCell(3).SetCellValue(tdzl);
                                        // 宗地面积
                                        row2.CreateCell(4).SetCellValue(zdmj);
                                        // 房屋面积
                                        row2.CreateCell(5).SetCellValue(jzmj);
                                        // 侵占基本农田
                                        row2.CreateCell(6).SetCellValue(QZ);
                                        // 疑似侵占
                                        row2.CreateCell(7).SetCellValue(YSQZ);

                                        if (QZ == "是" && !string.IsNullOrEmpty(QZMJ))
                                        {
                                            Dictionary<string, double> PolygonAreaDic = GetDefaultAreaDic();
                                            string[] flmjsz = QZMJ.Split('|');

                                            foreach (string s in flmjsz)
                                            {
                                                string[] flmj = s.Split(',');
                                                PolygonAreaDic[flmj[0]] = Convert.ToDouble(flmj[1]);
                                            }

                                            row2.CreateCell(8).SetCellValue(PolygonAreaDic["011"].ToString("0.00"));
                                            row2.CreateCell(9).SetCellValue(PolygonAreaDic["012"].ToString("0.00"));
                                            row2.CreateCell(10).SetCellValue(PolygonAreaDic["013"].ToString("0.00"));

                                            row2.CreateCell(11).SetCellValue(PolygonAreaDic["021"].ToString("0.00"));
                                            row2.CreateCell(12).SetCellValue(PolygonAreaDic["022"].ToString("0.00"));
                                            row2.CreateCell(13).SetCellValue(PolygonAreaDic["023"].ToString("0.00"));

                                            row2.CreateCell(14).SetCellValue(PolygonAreaDic["031"].ToString("0.00"));
                                            row2.CreateCell(15).SetCellValue(PolygonAreaDic["032"].ToString("0.00"));
                                            row2.CreateCell(16).SetCellValue(PolygonAreaDic["033"].ToString("0.00"));

                                            row2.CreateCell(17).SetCellValue(PolygonAreaDic["104"].ToString("0.00"));

                                            row2.CreateCell(18).SetCellValue(PolygonAreaDic["114"].ToString("0.00"));

                                            row2.CreateCell(19).SetCellValue(PolygonAreaDic["117"].ToString("0.00"));

                                            row2.CreateCell(20).SetCellValue(PolygonAreaDic["122"].ToString("0.00"));

                                            row2.CreateCell(21).SetCellValue(PolygonAreaDic["123"].ToString("0.00"));
                                        }
                                        else
                                        {
                                            row2.CreateCell(8).SetCellValue("");
                                            row2.CreateCell(9).SetCellValue("");
                                            row2.CreateCell(10).SetCellValue("");

                                            row2.CreateCell(11).SetCellValue("");
                                            row2.CreateCell(12).SetCellValue("");
                                            row2.CreateCell(13).SetCellValue("");

                                            row2.CreateCell(14).SetCellValue("");
                                            row2.CreateCell(15).SetCellValue("");
                                            row2.CreateCell(16).SetCellValue("");

                                            row2.CreateCell(17).SetCellValue("");

                                            row2.CreateCell(18).SetCellValue("");

                                            row2.CreateCell(19).SetCellValue("");

                                            row2.CreateCell(20).SetCellValue("");

                                            row2.CreateCell(21).SetCellValue("");
                                        }
                                        #endregion
                                        count++;
                                    }
                                }
                                FileStream FileStreamfile = new FileStream(Filename, FileMode.Create);
                                hSSFWorkbook.Write(FileStreamfile);
                                FileStreamfile.Close();

                                progressmeter.Stop();
                                MessageBox.Show("保存成功！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                progressmeter.Stop();
                                MessageBox.Show("文件名不能为空！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }
    }


}
