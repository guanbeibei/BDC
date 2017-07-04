using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.ObjectData;
using NCZJDDJFZ.Tools;
using ReadOrWriteXML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using ACDBPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using err = NCZJDDJFZ.DiJitools.Dijicltools.err;
using FWSXJG = NCZJDDJFZ.DiJitools.Dijicltools.FWSXJG;
using QRCoder;
using System.Drawing;

namespace NCZJDDJFZ.DCB
{
    public partial class DCB_JBB : UserControl
    {
        ArrayList arrerr = new ArrayList();
        public ArrayList arrtext = new ArrayList();
        public DataSet da_ZD = new DataSet();//定义DataSet 
        public SqlDataAdapter DP = new SqlDataAdapter();//初始化适配器
        public DataSet da_ZDD = new DataSet();//定义DataSet 
        public SqlDataAdapter DPD = new SqlDataAdapter();//初始化适配器
        public DataSet da_ZDX = new DataSet();//定义DataSet 
        public SqlDataAdapter DPX = new SqlDataAdapter();//初始化适配器
        System.Data.DataTable TB_DCB = null;

        string JzdOutputType = "子区和宗内编号";
        string CQ = "默认";
        double FDMJ = 0.0;

        public DCB_JBB()
        {
            InitializeComponent();
        }

        private void DCB_JBB_Load(object sender, EventArgs e)
        {
            string Sfzmj = Tools.ReadWriteReg.read_reg("法定面积");
            try
            {
                FDMJ = Convert.ToDouble(Sfzmj);
            }
            catch
            {
                MessageBox.Show("法定面积填写错误！请检查系统设置！", "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            CQ = Tools.ReadWriteReg.read_reg("测区名称");
            #region 根据测区名称显示不同测区所需要输出表格的名称
            switch (CQ)
            {
                case "默认":
                    this.BGLB.Items.AddRange(new object[]
                    {
                        "申请书和审批表",
                        "调查表",
                        "房屋调查表",
                        "宗地草图",
                        "宗地图",
                        "分层分户图",
                        "界址点坐标册"
                    });
                    break;
                case "阜阳":
                    this.BGLB.Items.AddRange(new object[]
                    {
                        "申请审批表",
                        "调查表",
                        "房屋调查表",
                        "宗地草图",
                        "宗地图",
                        "分层分户图",
                        "界址点坐标册",
                        "权属来源证明",
                        "具结承诺证明书",
                        "建房申请审批表",
                        "档案封面"
                    });
                    break;
                case "金寨":
                    this.BGLB.Items.AddRange(new object[] 
                    {
                        "申请审批表",
                        "调查表",
                        "房屋调查表",
                        "宗地草图",
                        "宗地图",
                        "分层分户图",
                        "界址点坐标册",
                        "权属来源证明",
                        "具结承诺证明书",
                        "建房申请审批表"
                        //"档案封面"
                    });
                    break;
                case "芜湖":
                    this.BGLB.Items.AddRange(new object[] 
                    {
                        "申请书和审批表",
                        "调查表",
                        "房屋调查表",
                        "宗地草图",
                        "宗地图",
                        "分层分户图",
                        "界址点坐标册",
                        "权属来源证明",
                        //"具结承诺证明书",
                        "申请书与证明书",
                        "建房申请审批表"
                        //"档案封面"
                    });
                    break;
                case "宣城":
                    this.BGLB.Items.AddRange(new object[] 
                    {
                        "申请书和审批表",
                        "调查表",
                        "房屋调查表",
                        "宗地草图",
                        "宗地图",
                        "分层分户图",
                        "界址点坐标册",
                        //"权属来源证明",
                        //"具结承诺证明书",
                        //"建房申请审批表",
                        "确认申请审核表"
                        //"档案封面"
                    });
                    break;
                default:
                    this.BGLB.Items.AddRange(new object[]
                    {
                        "申请书和审批表",
                        "调查表",
                        "房屋调查表",
                        "宗地草图",
                        "宗地图",
                        "分层分户图",
                        "界址点坐标册"
                    });
                    break;
            }
            #endregion

            JzdOutputType = Tools.ReadWriteReg.read_reg("界址点编号输出方式");

            ReadOrWritexml readOrWritexml = new ReadOrWritexml();

            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            this.ZDY1.Text = readOrWritexml.ReadXml("//界址签章表自定义1");
            this.ZDY2.Text = readOrWritexml.ReadXml("//界址签章表自定义2");
            this.ZDY3.Text = readOrWritexml.ReadXml("//界址签章表自定义3");
            DrawDataTree();// 画数据字典节点树
            if (readOrWritexml.ReadXml("//上下偏离-档案") != "")
            {
                this.SXPL.Value = Convert.ToDecimal(readOrWritexml.ReadXml("//上下偏离-档案"));
            }
            else
            {
                this.SXPL.Value = 72;
            }
            if (readOrWritexml.ReadXml("//左右偏离-档案") != "")
            {
                this.ZYPL.Value = Convert.ToDecimal(readOrWritexml.ReadXml("//左右偏离-档案"));
            }
            else
            {
                this.ZYPL.Value = 30;
            }

            #region 装入打印机名称
            Database db = HostApplicationServices.WorkingDatabase;
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    LayoutManager layoutManager = LayoutManager.Current;
                    Autodesk.AutoCAD.DatabaseServices.Layout layout = (Autodesk.AutoCAD.DatabaseServices.Layout)trans.GetObject(layoutManager.GetLayoutId(layoutManager.CurrentLayout), ACDBOpenMode.ForRead);
                    Autodesk.AutoCAD.PlottingServices.PlotInfo plotInfo = new Autodesk.AutoCAD.PlottingServices.PlotInfo();
                    plotInfo.Layout = layout.ObjectId;
                    PlotSettings plotSettings = new PlotSettings(layout.ModelType);
                    plotSettings.CopyFrom(layout);
                    PlotSettingsValidator plotSettingsValidator = PlotSettingsValidator.Current;
                    System.Collections.Specialized.StringCollection PlotDeviceList = plotSettingsValidator.GetPlotDeviceList();
                    DYJMC.Items.Clear();
                    for (int i = 0; i < PlotDeviceList.Count; i++)
                    {
                        DYJMC.Items.Add(PlotDeviceList[i].ToString());
                    }
                    ////plotSettingsValidator.SetPlotWindowArea(plotSettings, new Extents2d(1, 1, 5, 5));
                    //plotSettingsValidator.SetPlotType(plotSettings, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);

                    //plotSettingsValidator.SetPlotConfigurationName(plotSettings, "打印机名称", null);

                    //System.Collections.Specialized.StringCollection  mgStringCollection2 = plotSettingsValidator.GetCanonicalMediaNameList(plotSettings);//纸张
                    //plotInfo.OverrideSettings = plotSettings;
                    trans.Commit();
                }
            }
            #endregion
        }

        // 根据地类编码查找地类名称
        /// <summary>
        /// 根据地类编码查找地类名称
        /// </summary>
        /// <param name="dlbm">地类编码</param>
        /// <returns>地类名称</returns>
        public string find_dlmc(string dlbm)
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
        public string find_dlbm(string dlmc)
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

        // 获取房屋属性
        /// <summary>
        /// 获取房屋属性
        /// </summary>
        /// <param name="fw_pline">房屋对象</param>
        /// <returns>属性集合</returns>
        public FWSXJG GetFWsx(ACDBPolyline FWX)
        {
            FWSXJG sx = new FWSXJG();//房屋属性
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForRead, true);
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
                    MessageBox.Show("没有对象属性表！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return sx;
                }
                #endregion
                #region 将属性写入数组


                table = tables["房屋属性"];
                Records records = table.GetObjectTableRecords(0, FWX, Autodesk.Gis.Map.Constants.OpenMode.OpenForRead, true);
                if (records.Count > 0)
                {
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
                }

                return sx;
                #endregion
            }
        }

        public List<FWSXJG> FCHE_fwsxjg = new List<FWSXJG>();//房屋属性结构
        public List<Topology.Geometries.Polygon> FCHE_Polygon = new List<Topology.Geometries.Polygon>();//房屋 

        // 分层分户将房屋属性写入内存
        public void FCFH_Write_FW_SX_to_LC()
        {
            for (int i = 0; i < FCHE_fwsxjg.Count; i++)
            {
                FCHE_fwsxjg.RemoveAt(i);
                FCHE_Polygon.RemoveAt(i);
            }
            ArrayList arrerr = new ArrayList();
            ArrayList arrtext = new ArrayList();
            arrerr.Clear();
            arrtext.Clear();
            ArrayList ZJFH = new ArrayList();
            int xh = 0;
            ArrayList pologylist = new ArrayList();
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 创建房屋选择集
                Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
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
                                    er.minx = JZXBounds.MinPoint.X;
                                    er.miny = JZXBounds.MinPoint.Y;
                                    er.maxX = JZXBounds.MaxPoint.X;
                                    er.maxy = JZXBounds.MaxPoint.Y;
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
                            Topology.Geometries.Polygon FW_pline = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(FWX);
                            FCHE_Polygon.Add(FW_pline);
                            FCHE_fwsxjg.Add(sx);
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
                    progressmeter.Stop();
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                }
            }
            Class1.WritErr(arrerr);
        }

        // 房屋属性结构集合
        /// <summary>
        /// 房屋属性结构集合
        /// </summary>
        public List<FWSXJG> fwsxjg = new List<FWSXJG>();//房屋属性结构

        // 将房屋属性写入集合
        /// <summary>
        /// 将房屋属性写入集合
        /// </summary>
        public void Write_FW_SX_to_LC()
        {
            for (int i = 0; i < fwsxjg.Count; i++)
            {
                fwsxjg.RemoveAt(i);
            }
            ArrayList arrerr = new ArrayList();
            ArrayList arrtext = new ArrayList();
            arrerr.Clear();
            arrtext.Clear();
            ArrayList ZJFH = new ArrayList();
            int xh = 0;
            ArrayList pologylist = new ArrayList();
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 创建房屋选择集
                Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
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
                            //Topology.Geometries.Polygon FW_pline = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(FWX);
                            //FW_pline.UserData = FWX.Handle.Value.ToString();
                            //FW_area[i] = FW_pline;
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
                                    er.minx = JZXBounds.MinPoint.X;
                                    er.miny = JZXBounds.MinPoint.Y;
                                    er.maxX = JZXBounds.MaxPoint.X;
                                    er.maxy = JZXBounds.MaxPoint.Y;
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
                            fwsxjg.Add(sx);
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
                    progressmeter.Stop();
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                }
            }
            Class1.WritErr(arrerr);
        }

        // 画数据字典节点树
        /// <summary>
        /// 画数据字典节点树
        /// </summary>
        public void DrawDataTree()
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

        //  自定义的ListBox属性表
        /// <summary>
        ///  构建ListBox属性表
        /// </summary>
        public class ListBoxsx
        {
            private string nN0_Name;
            private string nNHBH;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="N0_Name">宗地号+名称</param>
            /// <param name="NHBH">宗地号</param>
            public ListBoxsx(string N0_Name, string NHBH)
            {
                this.nN0_Name = N0_Name;
                this.nNHBH = NHBH;
            }
            /// <summary>
            /// 宗地号+名称
            /// </summary>
            public string N0_Name
            {
                set
                {
                    nN0_Name = value;
                }
                get
                {
                    return nN0_Name;
                }
            }
            /// <summary>
            /// 宗地号
            /// </summary>
            public string NHBH
            {
                set
                {
                    nNHBH = value;
                }
                get
                {
                    return nNHBH;
                }
            }
        }

        // 增加宗地列表框
        /// <summary>
        ///  增加宗地列表框
        /// </summary>
        public void AddHuzhulistbox(DataSet dataset_DCB)
        {
            this.HZLB.DataSource = null;
            this.HZLB.Items.Clear();
            TB_DCB = dataset_DCB.Tables["DCB"];
            ArrayList listboxDataSource = new ArrayList();
            for (int i = 0; i < TB_DCB.Rows.Count; i++)
            {
                ListBoxsx list = new ListBoxsx(TB_DCB.Rows[i]["DJH"].ToString().Trim().Substring(12, 5) + "-"
                    + TB_DCB.Rows[i]["QLR"].ToString().Trim(), TB_DCB.Rows[i]["DJH"].ToString().Trim());
                listboxDataSource.Add(list);
            }
            // IComparer sort = new Sort_LH_Comparer();

            if (TB_DCB.Rows.Count <= 0)
            {
                // MessageBox.Show("此组没有数据", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            // listboxDataSource.Sort(sort);
            this.HZLB.DataSource = listboxDataSource;//绑定数据源
            this.HZLB.ValueMember = "NHBH";//属性值
            this.HZLB.DisplayMember = "N0_Name";//显示名称
            this.HZLB.EndUpdate();
            this.HZLB.ClearSelected();//清除选择
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Level == 4)
            {
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                string JDH = XZDM.Substring(6, 3);
                string JFH = XZDM.Substring(9, 3);
                string XZQH = XZDM.Substring(0, 6);
                label147.Text = JDH;
                label150.Text = JFH;
                string connectionString = Tools.Uitl.LJstring();//写连接字符串
                string selstring = "SELECT * FROM DCB where DJH like '" + XZDM + "%%%%%'";
                if (Tools.DataBasic.Create_DCB_table(connectionString, "DCB", Tools.DataBasicString.Create_DCB))//如果”DCB“不存在则创建DCB
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlDataAdapter Adapter_TB = Tools.DataBasic.DCB_Initadapter(selstring, connectionString);
                        DataSet dataset_DCB = new DataSet();
                        Adapter_TB.Fill(dataset_DCB, "DCB");//填充Dataset
                        AddHuzhulistbox(dataset_DCB);//刷新宗地列表框
                        connection.Close();
                    }
                }
            }
        }

        private void HZLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //save();
            //FillData();// 将数据库中的数据填充到窗体控件
        }

        private void HZLB_DoubleClick(object sender, EventArgs e)
        {
            System.Drawing.Size size = new System.Drawing.Size(658, 650);
            DCB_JBB dd = new DCB_JBB();
            dd.Size = size;
            save();
            FillData();// 将数据库中的数据填充到窗体控件

            this.FCFHTLB.DataSource = null;
        }


        // 缩放CAD窗口
        /// <summary>
        /// 缩放CAD窗口
        /// </summary>
        /// <param name="centerPoint">中心点坐标</param>
        /// <param name="h">窗口高度</param>
        /// <param name="w">宽度</param>
        public void Zoom(Point2d centerPoint, double h, double w)
        {
            Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
            ViewTableRecord viewTableRecord = ed.GetCurrentView();
            viewTableRecord.CenterPoint = new Point2d(centerPoint.X, centerPoint.Y);
            viewTableRecord.Target = new Point3d(0, 0, 0);
            viewTableRecord.Height = h;
            viewTableRecord.Width = w;
            ed.SetCurrentView(viewTableRecord);
            // Autodesk.AutoCAD.ApplicationServices.Application.UpdateScreen();
        }

        // 将数据库中的数据填充到窗体控件
        /// <summary>
        /// 将数据库中的数据填充到窗体控件
        /// </summary>
        public void FillData()
        {
            //if (!Tools.Uitl.JCDOG())
            //{
            //    return;
            //}
            #region 条件检查与准备
            if (this.HZLB.SelectedIndex == -1)
            {
                return;
            }
            if (ATT.Document_ZDT_SC != null && !ATT.Document_ZDT_SC.IsDisposed)
            {
                ATT.Document_ZDT_SC.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
            }
            if (ATT.Document_ZDT_XG != null && !ATT.Document_ZDT_XG.IsDisposed)
            {
                ATT.Document_ZDT_XG.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
            }
            if (ATT.Document_ZDT_LL != null && !ATT.Document_ZDT_LL.IsDisposed)
            {
                ATT.Document_ZDT_LL.CloseAndDiscard();//不保存
            }
            if (ATT.Document_ZDT_DY != null && !ATT.Document_ZDT_DY.IsDisposed)
            {
                ATT.Document_ZDT_DY.CloseAndDiscard();//不保存
            }
            ListBoxsx listBoxsx = (ListBoxsx)HZLB.Items[HZLB.SelectedIndex];
            string ndjh = listBoxsx.NHBH;
            ATT.DJH = ndjh;
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            string selstring = "SELECT * FROM DCB where DJH = '" + ndjh + "'";
            string selstring_ZDD = "SELECT  *  FROM D_ZDD where DJH = '" + ndjh + "'";
            string selstring_ZDX = "SELECT  *  FROM D_ZDX where DJH = '" + ndjh + "'";
            da_ZD.Clear();
            da_ZDD.Clear();
            da_ZDX.Clear();
            #endregion
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                #region 读数据库中“DCB”表数据
                connection.Open();
                DP = Tools.DataBasic.DCB_Initadapter(selstring, connectionString);
                DP.Fill(da_ZD, "DCB");//填充Dataset
                TB_DCB = da_ZD.Tables["DCB"];
                DataRow row = TB_DCB.Rows[0];
                #endregion
                #region 缩放图形
                if (checkBox1.Checked)
                {
                    double minx = (double)row["Xmin"];
                    double miny = (double)row["Ymin"];
                    double maxx = (double)row["Xmax"];
                    double maxy = (double)row["Ymax"];
                    double minx2 = minx - (maxx - minx) / 2.0;
                    double maxx2 = maxx + (maxx - minx) / 2.0;
                    double miny2 = miny - (maxy - miny) / 2.0;
                    double maxy2 = maxy + (maxy - miny) / 2.0;
                    Point2d cir = new Point2d((minx2 + maxx2) / 2.0, (miny2 + maxy2) / 2.0);
                    Zoom(cir, maxy2 - miny2, maxx2 - minx2);
                }
                #endregion
                //SFTXSBB.Checked = (bool)row["SFTXSBB"];
                #region 申请书1
                ZZXM.Text = row["ZZXM"].ToString().Trim();
                ZRXM.Text = row["ZRXM"].ToString().Trim();
                KCR.Text = row["KCR"].ToString().Trim();
                CMZYJ.Text = row["CMZYJ"].ToString().Trim();
                XZGHBMFZR.Text = row["XZGHBMFZR"].ToString().Trim();


                QLR.Text = row["QLR"].ToString().Trim();
                SQRLX.SelectedIndex = -1;
                SQRLX.SelectedItem = row["SQRLX"].ToString().Trim();
                //FRDBZJZL.Text = row["QLR_ZJZL"].ToString().Trim();
                //FRDBZJBH.Text = row["QLR_ZJBH"].ToString().Trim();
                DWXZ.SelectedIndex = -1;
                DWXZ.SelectedItem = row["DWXZ"].ToString().Trim();
                FRDBXM.Text = row["FRDBXM"].ToString().Trim();
                TXDZ.Text = row["TXDZ"].ToString().Trim();
                YZBM.Text = row["YZBM"].ToString().Trim();
                DZYJ.Text = row["DZYJ"].ToString().Trim();
                LXR.Text = row["LXR"].ToString().Trim();
                // FRDBDH.Text = row["LXDH"].ToString().Trim();
                //SQS_DLRXM.Text = row["SQS_DLRXM"].ToString().Trim();
                ZYZGZSH.Text = row["ZYZGZSH"].ToString().Trim();
                DLJGMC.Text = row["DLJGMC"].ToString().Trim();
                DLJGDH.SelectedIndex = -1;
                DLJGDH.SelectedItem = row["DLJGDH"].ToString().Trim();
                QLR2.Text = row["QLR2"].ToString().Trim();
                SQRLX2.SelectedIndex = -1;
                SQRLX2.SelectedItem = row["SQRLX2"].ToString().Trim();
                //QLR_ZJZL2.Text = row["QLR_ZJZL2"].ToString().Trim();
                QLR_ZJZL2.SelectedIndex = -1;
                QLR_ZJZL2.SelectedItem = row["QLR_ZJZL2"].ToString().Trim();
                QLR_ZJBH2.Text = row["QLR_ZJBH2"].ToString().Trim();
                DWXZ2.SelectedIndex = -1;
                DWXZ2.SelectedItem = row["DWXZ2"].ToString().Trim();
                FRDBXM2.Text = row["FRDBXM2"].ToString().Trim();
                TXDZ2.Text = row["TXDZ2"].ToString().Trim();
                YZBM2.Text = row["YZBM2"].ToString().Trim();
                DZYJ2.Text = row["DZYJ2"].ToString().Trim();
                LXR2.Text = row["LXR2"].ToString().Trim();
                LXDH2.Text = row["LXDH2"].ToString().Trim();
                SQS_DLRXM2.Text = row["SQS_DLRXM2"].ToString().Trim();
                //ZYZGZSH2.Text = row["ZYZGZSH2"].ToString().Trim();
                DLJGMC2.Text = row["DLJGMC2"].ToString().Trim();
                DLJGDH2.Text = row["DLJGDH2"].ToString().Trim();
                //if (row["SQRQ"].ToString().Trim() == "")
                //{
                //    SQRQ.Checked = false;
                //}
                //else
                //{
                //    SQRQ.Checked = true;
                SQRQ.Text = row["SQRQ"].ToString().Trim();
                //}
                #endregion
                #region 申请书2
                TEXTBZ1.Text = row["TEXTBZ1"].ToString().Trim();



                TDZL.Text = row["TDZL"].ToString().Trim();
                double zdmj = (double)row["ZDMJ"];
                ZDMJ.Text = zdmj.ToString("0.00");
                SJYT.Text = row["SJYT"].ToString().Trim();
                QSXZ.SelectedIndex = -1;
                QSXZ.SelectedItem = row["QSXZ"].ToString().Trim();
                DLBM.SelectedIndex = -1;
                DLBM.SelectedItem = row["DLBM"].ToString().Trim();
                SYQLX.Text = row["SYQLX"].ToString().Trim();
                QLSLQJ.SelectedIndex = -1;
                QLSLQJ.SelectedItem = row["QLSLQJ"].ToString().Trim();
                XYDZL.Text = row["XYDZL"].ToString().Trim();
                TDJG.Text = row["TDJG"].ToString().Trim();
                TDDYMJ.Text = row["TDDYMJ"].ToString().Trim();
                DYSX.Text = row["DYSX"].ToString().Trim();

                TDDYJE.Text = row["TDDYJE"].ToString().Trim();
                SQDJLR.Text = row["SQDJLR"].ToString().Trim();
                SQSBZ.Text = row["SQSBZ"].ToString().Trim();
                TEXTBZ2.Text = row["TEXTBZ2"].ToString();//提交的土地证书编号
                //QDJG.Text = row["FBZ2"].ToString().Trim();//取得价格DYQX_Q
                if (row["DYQX_Q"].ToString().Trim() == "单一版")
                {
                    DYB.Checked = true;
                    JCB.Checked = false;
                }
                else
                {
                    JCB.Checked = true;
                    DYB.Checked = false;
                }

                if (row["DYQX_Z"].ToString().Trim() == "是")
                {
                    FBCZ_YSE.Checked = true;
                    FBCZ_NO.Checked = false;
                }
                else
                {
                    FBCZ_YSE.Checked = false;
                    FBCZ_NO.Checked = true;
                }
                //if (row["DYSQRJZRQ"].ToString().Trim() == "")
                //{
                //    DYSQRJZRQ.Checked = false;
                //}
                //else
                //{
                //    DYSQRJZRQ.Checked = true;
                DYSQRJZRQ.Text = row["DYSQRJZRQ"].ToString().Trim();
                //}
                //if (row["DESQRJZRQ"].ToString().Trim() == "")
                //{
                //    DESQRJZRQ.Checked = false;
                //}
                //else
                //{
                //    DESQRJZRQ.Checked = true;
                DESQRJZRQ.Text = row["DESQRJZRQ"].ToString().Trim();
                //}
                #endregion
                #region 基本表
                FRDBZJBH.Text = row["FRDBZJBH"].ToString().Trim();
                FRDBZJZL.SelectedIndex = -1;
                FRDBZJZL.SelectedItem = row["FRDBZJZL"].ToString().Trim();
                FRDBDH.Text = row["FRDBDH"].ToString().Trim();
                JBB_DLRXM.Text = row["JBB_DLRXM"].ToString().Trim();
                JBB_DLRZJBH.Text = row["JBB_DLRZJBH"].ToString().Trim();
                JBB_DLRZJZL.SelectedIndex = -1;
                JBB_DLRZJZL.SelectedItem = row["JBB_DLRZJZL"].ToString().Trim();
                JBB_DLRDH.Text = row["JBB_DLRDH"].ToString().Trim();
                GMJJHYDM.Text = row["GMJJHYDM"].ToString().Trim();
                YBZDDM.Text = row["YBZDDM"].ToString().Trim();
                TFH.Text = row["TFH"].ToString().Trim();
                PZYT.Text = row["PZYT"].ToString().Trim();
                PZYT_DLDM.SelectedIndex = -1;
                PZYT_DLDM.SelectedItem = row["PZYT_DLDM"].ToString().Trim();
                JZZDMJ.Text = row["JZZDMJ"].ToString().Trim();
                JZMJ.Text = row["JZMJ"].ToString().Trim();
                GYQLRQK.Text = row["GYQLRQK"].ToString().Trim();
                SM.Text = row["SM"].ToString().Trim();
                BZ.Text = row["BZ"].ToString().Trim();
                DZ.Text = row["DZ"].ToString().Trim();
                NZ.Text = row["NZ"].ToString().Trim();
                XZ.Text = row["XZ"].ToString().Trim();

                //JBB_ZDMJ.Text = zdmj.ToString("0.00");
                // TDDYMJ.Text = row["FZMJ"].ToString().Trim();
                //if (row["DCBRQ"].ToString().Trim() == "")
                //{
                //    DCBRQ.Checked = false;
                //}
                //else
                //{
                //    DCBRQ.Checked = true;
                DCBRQ.Text = row["DCBRQ"].ToString().Trim();
                //}
                //if (row["SYQX_Q"].ToString().Trim() == "")
                //{
                //    SYQX_Q.Checked = false;
                //}
                //else
                //{
                //    SYQX_Q.Checked = true;
                SYQX_Q.Text = row["SYQX_Q"].ToString().Trim();
                //}
                //if (row["SYQX_Z"].ToString().Trim() == "")
                //{
                //    SYQX_Z.Checked = false;
                //}
                //else
                //{
                //    SYQX_Z.Checked = true;
                SYQX_Z.Text = row["SYQX_Z"].ToString().Trim();
                //}
                #endregion
                #region 审核表
                QSDCJS.Text = row["QSDCJS"].ToString();
                DCYXM.Text = row["DCYXM"].ToString().Trim();
                DJKZJS.Text = row["DJKZJS"].ToString();
                CLYXM.Text = row["CLYXM"].ToString().Trim();
                DJDCJGSHYJ.Text = row["DJDCJGSHYJ"].ToString();
                SHRXM.Text = row["SHRXM"].ToString().Trim();

                SHHG.Checked = (bool)row["SHHG"];

                //if (row["DCRQ"].ToString().Trim() == "")
                //{
                //    DCRQ.Checked = false;
                //}
                //else
                //{
                //DCRQ.Checked = true;
                DCRQ.Text = row["DCRQ"].ToString().Trim();
                //}
                //if (row["CLRQ"].ToString().Trim() == "")
                //{
                //    CLRQ.Checked = false;
                //}
                //else
                //{
                //    CLRQ.Checked = true;
                CLRQ.Text = row["CLRQ"].ToString().Trim();
                //}
                //if (row["SHRQ"].ToString().Trim() == "")
                //{
                //    SHRQ.Checked = false;
                //}
                //else
                //{
                //    SHRQ.Checked = true;
                SHRQ.Text = row["SHRQ"].ToString().Trim();
                //}

                #endregion
                #region 审批表
                FZMJ.Text = row["FZMJ"].ToString().Trim();
                CSYJ.Text = row["CSYJ"].ToString();
                CSSCR.Text = row["CSSCR"].ToString().Trim();
                //CSRTDDJSGZH.Text = row["CSRTDDJSGZH"].ToString().Trim();
                SHYJ.Text = row["SHYJ"].ToString();
                SHFZR.Text = row["SHFZR"].ToString().Trim();
                //SHRTDDJSGZH.Text = row["SHRTDDJSGZH"].ToString().Trim();
                PZYJ.Text = row["PZYJ"].ToString();
                PZR.Text = row["PZR"].ToString().Trim();
                SPBBZ.Text = row["SPBBZ"].ToString().Trim();
                ZDMJ_SHB.Text = zdmj.ToString("0.00");
                FZJS.Text = row["FZJS"].ToString();//
                //if (row["CSRJ"].ToString().Trim() == "")
                //{
                //    CSRJ.Checked = false;
                //}
                //else
                //{
                //    CSRJ.Checked = true;
                CSRJ.Text = row["CSRJ"].ToString().Trim();
                //}
                //if (row["SHRQ_SPB"].ToString().Trim() == "")
                //{
                //    SHRQ_SPB.Checked = false;
                //}
                //else
                //{
                //    SHRQ_SPB.Checked = true;
                SHRQ_SPB.Text = row["SHRQ_SPB"].ToString().Trim();
                //}
                //if (row["PZRQ"].ToString().Trim() == "")
                //{
                //    PZRQ.Checked = false;
                //}
                //else
                //{
                //    PZRQ.Checked = true;
                PZRQ.Text = row["PZRQ"].ToString().Trim();
                //}
                PZTG.Checked = (bool)row["PZTG"];
                SHTG.Checked = (bool)row["SHTG"];
                CSHG.Checked = (bool)row["CSHG"];
                #endregion
                #region 申报表
                //SBB_BH.Text = row["SBB_BH"].ToString().Trim();
                //JTRK.Text = row["JTRK"].ToString().Trim();
                //RKJG.Text = row["RKJG"].ToString().Trim();
                //YYLFJS.Text = row["YYLFJS"].ToString().Trim();
                //YYLFMJ.Text = row["YYLFMJ"].ToString().Trim();
                //YYPFJS.Text = row["YYPFJS"].ToString().Trim();
                //YYPFMJ.Text = row["YYPFMJ"].ToString().Trim();
                //LJLFJS.Text = row["LJLFJS"].ToString().Trim();
                //LJLFMJ.Text = row["LJLFMJ"].ToString().Trim();
                //LJPFJS.Text = row["LJPFJS"].ToString().Trim();
                //LJPFMJ.Text = row["LJPFMJ"].ToString().Trim();
                //YJZJDMJ.Text = zdmj.ToString("0.00");
                //STMJ.Text = row["STMJ"].ToString().Trim();
                //HDMJ.Text = row["HDMJ"].ToString().Trim();
                //CDMJ.Text = row["CDMJ"].ToString().Trim();
                //YZJDMJ.Text = row["YZJDMJ"].ToString().Trim();
                //KXDMJ.Text = row["KXDMJ"].ToString().Trim();
                //HSMJ.Text = row["HSMJ"].ToString().Trim();
                //CMZYJ.Text = row["CMZYJ"].ToString();
                //ZZXM.Text = row["ZZXM"].ToString().Trim();
                //CWHYJ.Text = row["CWHYJ"].ToString();
                //ZRXM.Text = row["ZRXM"].ToString().Trim();
                //XCKCYJ.Text = row["XCKCYJ"].ToString();
                //KCR.Text = row["KCR"].ToString().Trim();
                //XZGHBMYJ.Text = row["XZGHBMYJ"].ToString();
                //XZGHBMFZR.Text = row["XZGHBMFZR"].ToString().Trim();
                //XZJWGHYJ.Text = row["XZJWGHYJ"].ToString();
                //XZJWFZR.Text = row["XZJWFZR"].ToString().Trim();
                //XZZFSPYJ.Text = row["XZZFSPYJ"].ToString();
                //XGTBMSCYJ.Text = row["XGTBMSCYJ"].ToString();
                //XGTBMSCR.Text = row["XGTBMSCR"].ToString().Trim();
                //XZFSCYJ.Text = row["XZFSCYJ"].ToString();
                //SQLY.Text = row["TEXTBZ3"].ToString();
                //double mj = (double)row["FBZ1"];
                //YJZJDMJ.Text = mj.ToString("0.00");
                //if (row["SBB_TBRQ"].ToString().Trim() == "")
                //{
                //    SBB_TBRQ.Checked = false;
                //}
                //else
                //{
                //    SBB_TBRQ.Checked = true;
                //    SBB_TBRQ.Text = row["SBB_TBRQ"].ToString().Trim();
                //}
                //if (row["CMZQZRQ"].ToString().Trim() == "")
                //{
                //    CMZQZRQ.Checked = false;
                //}
                //else
                //{
                //    CMZQZRQ.Checked = true;
                //    CMZQZRQ.Text = row["CMZQZRQ"].ToString().Trim();
                //}
                //if (row["CWHQZRQ"].ToString().Trim() == "")
                //{
                //    CWHQZRQ.Checked = false;
                //}
                //else
                //{
                //    CWHQZRQ.Checked = true;
                //    CWHQZRQ.Text = row["CWHQZRQ"].ToString().Trim();
                //}
                //if (row["KCRQ"].ToString().Trim() == "")
                //{
                //    KCRQ.Checked = false;
                //}
                //else
                //{
                //    KCRQ.Checked = true;
                //    KCRQ.Text = row["KCRQ"].ToString().Trim();
                //}
                //if (row["XZGHBMQZRQ"].ToString().Trim() == "")
                //{
                //    XZGHBMQZRQ.Checked = false;
                //}
                //else
                //{
                //    XZGHBMQZRQ.Checked = true;
                //    XZGHBMQZRQ.Text = row["XZGHBMQZRQ"].ToString().Trim();
                //}
                //if (row["XZJWQZRQ"].ToString().Trim() == "")
                //{
                //    XZJWQZRQ.Checked = false;
                //}
                //else
                //{
                //    XZJWQZRQ.Checked = true;
                //    XZJWQZRQ.Text = row["XZJWQZRQ"].ToString().Trim();
                //}
                //if (row["XZZFSPRQ"].ToString().Trim() == "")
                //{
                //    XZZFSPRQ.Checked = false;
                //}
                //else
                //{
                //    XZZFSPRQ.Checked = true;
                //    XZZFSPRQ.Text = row["XZZFSPRQ"].ToString().Trim();
                //}
                //if (row["XGTBMSCRQ"].ToString().Trim() == "")
                //{
                //    XGTBMSCRQ.Checked = false;
                //}
                //else
                //{
                //    XGTBMSCRQ.Checked = true;
                //    XGTBMSCRQ.Text = row["XGTBMSCRQ"].ToString().Trim();
                //}
                //if (row["XZFSCRQ"].ToString().Trim() == "")
                //{
                //    XZFSCRQ.Checked = false;
                //}
                //else
                //{
                //    XZFSCRQ.Checked = true;
                //    XZFSCRQ.Text = row["XZFSCRQ"].ToString().Trim();
                //}

                #endregion
                #region 写入界址表示表
                DPD = Tools.DataBasic.D_ZDD_Initadapter(selstring_ZDD, connectionString);
                DPD.Fill(da_ZDD, "D_ZDD");//填充Dataset
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.DefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = da_ZDD;
                dataGridView1.DataMember = "D_ZDD";

                this.dataGridView1.Columns["DJH"].Visible = false;
                this.dataGridView1.Columns["JZJJ"].Visible = false;
                this.dataGridView1.Columns["X"].Visible = false;
                this.dataGridView1.Columns["Y"].Visible = false;
                this.dataGridView1.Columns["BZ"].Visible = false;


                dataGridView1.Columns["MBBSM"].HeaderText = "序号";
                dataGridView1.Columns["JZDH"].HeaderText = "界址点号";
                dataGridView1.Columns["JBZL"].HeaderText = "界标种类";
                dataGridView1.Columns["KZBC"].HeaderText = "勘丈距离";
                dataGridView1.Columns["JZXLB"].HeaderText = "界址类别";
                dataGridView1.Columns["JZXWZ"].HeaderText = "界线位置";
                dataGridView1.Columns["SM"].HeaderText = "说明";
                dataGridView1.Columns["MBBSM"].ReadOnly = true;
                dataGridView1.Columns["JZDH"].ReadOnly = true;

                dataGridView1.Columns["MBBSM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["JZDH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["JBZL"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["KZBC"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["JZXLB"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["JZXWZ"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["SM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["MBBSM"].Width = 36;
                dataGridView1.Columns["JZDH"].Width = 60;
                dataGridView1.Columns["JBZL"].Width = 60;
                dataGridView1.Columns["KZBC"].Width = 60;
                dataGridView1.Columns["JZXLB"].Width = 60;
                dataGridView1.Columns["JZXWZ"].Width = 36;
                dataGridView1.Columns["SM"].Width = 36;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1["JBZL", i].Value = dataGridView1["JBZL", i].Value.ToString().Trim();
                    dataGridView1["JZXLB", i].Value = dataGridView1["JZXLB", i].Value.ToString().Trim();
                }
                this.dataGridView1.Columns["DJH"].Visible = false;
                this.dataGridView1.Columns["JZJJ"].Visible = false;
                this.dataGridView1.Columns["X"].Visible = false;
                this.dataGridView1.Columns["Y"].Visible = false;
                this.dataGridView1.Columns["BZ"].Visible = false;
                #endregion
                #region 写入界址签章表
                BZ.Text = row["BZ"].ToString().Trim();
                DZ.Text = row["DZ"].ToString().Trim();
                NZ.Text = row["NZ"].ToString().Trim();
                XZ.Text = row["XZ"].ToString().Trim();

                DPX = Tools.DataBasic.D_ZDX_Initadapter(selstring_ZDX, connectionString);
                DPX.Fill(da_ZDX, "D_ZDX");//填充Dataset
                dataGridView2.AllowUserToAddRows = false;
                dataGridView2.DefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
                dataGridView2.AutoGenerateColumns = true;
                dataGridView2.DataSource = da_ZDX;
                dataGridView2.Refresh();
                dataGridView2.DataMember = "D_ZDX";

                this.dataGridView2.Columns["DJH"].Visible = false;
                this.dataGridView2.Columns["MBBSM"].Visible = false;

                dataGridView2.Columns["QDH"].HeaderText = "起点号";
                dataGridView2.Columns["ZJDH"].HeaderText = "中间点号";
                dataGridView2.Columns["ZDH"].HeaderText = "终点号";
                dataGridView2.Columns["BZ"].HeaderText = "邻宗地权利人";
                dataGridView2.Columns["LZDZJRXM"].HeaderText = "邻宗地指界人";
                dataGridView2.Columns["BZDZJRXM"].HeaderText = "本宗地指界人";
                dataGridView2.Columns["ZJRQ"].HeaderText = "指界日期";
                dataGridView2.Columns["LZDDJH"].HeaderText = "邻宗地地籍号";

                dataGridView2.Columns["QDH"].DisplayIndex = 1;
                dataGridView2.Columns["ZJDH"].DisplayIndex = 2;
                dataGridView2.Columns["ZDH"].DisplayIndex = 3;
                dataGridView2.Columns["BZ"].DisplayIndex = 4;
                dataGridView2.Columns["LZDZJRXM"].DisplayIndex = 5;
                dataGridView2.Columns["BZDZJRXM"].DisplayIndex = 6;
                dataGridView2.Columns["ZJRQ"].DisplayIndex = 7;
                dataGridView2.Columns["LZDDJH"].DisplayIndex = 8;

                dataGridView2.Columns["QDH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["ZJDH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["ZDH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["LZDDJH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["LZDZJRXM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["BZDZJRXM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["ZJRQ"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["BZ"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns["QDH"].Width = 36;
                dataGridView2.Columns["ZJDH"].Width = 36;
                dataGridView2.Columns["ZDH"].Width = 36;
                dataGridView2.Columns["LZDDJH"].Width = 100;
                dataGridView2.Columns["LZDZJRXM"].Width = 52;
                dataGridView2.Columns["BZDZJRXM"].Width = 52;
                dataGridView2.Columns["ZJRQ"].Width = 85;
                dataGridView2.Columns["BZ"].Width = 70;

                this.dataGridView2.Columns["DJH"].Visible = false;
                this.dataGridView2.Columns["MBBSM"].Visible = false;
                #endregion
                connection.Close();
            }
        }

        // 添加列表
        /// <summary>
        /// 添加列表
        /// </summary>
        /// <param name="CB">ComboBox控件名称</param>
        /// <param name="S">需要添加的项</param>
        private void addlist(ComboBox CB, string S) // 添加列表
        {
            int count = CB.Items.Count;
            bool Istrue = false;
            for (int i = 0; i < count; i++)
            {
                if (S == CB.Items[i].ToString().Trim()) { Istrue = true; }
            }
            if (count < 10 && Istrue == false && S != "")
            {
                CB.Items.Add(S);
            }
            if (count >= 10 && Istrue == false && S != "")
            {
                CB.Items.Add(S);
                CB.Items.RemoveAt(0);
            }
        }

        // 保存窗体数据到数据库
        /// <summary>
        /// 保存窗体数据到数据库
        /// </summary>
        public void save()
        {
            try
            {
                if (da_ZD.Tables.Count <= 0)
                {
                    return;
                }
                string connectionString = Tools.Uitl.LJstring();//写连接字符串
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataRow row = TB_DCB.Rows[0];
                    #region 基本表
                    //row["SFTXSBB"] = SFTXSBB.Checked;
                    string LX = "J";
                    if (SQRLX.Text.Trim().IndexOf("集体") >= 0)
                    {
                        LX = "J";
                    }
                    else
                    {
                        LX = "G";
                    }
                    string tzm = "";
                    if (SQRLX.Text.Trim().IndexOf("宅基地") >= 0)
                    {
                        tzm = "C";
                    }
                    if (SQRLX.Text.Trim().IndexOf("建设") >= 0)
                    {
                        tzm = "B";
                    }

                    //if (QSXZ.Text.Trim() == "集体土地所有权")
                    //{
                    //    tzm = "A";
                    //}

                    row["WZDJH"] = row["DJH"].ToString().Trim().Substring(0, 12) + LX + tzm + row["DJH"].ToString().Trim().Substring(12, 5);
                    #endregion
                    #region 申请书一
                    row["ZZXM"] = ZZXM.Text.Trim();
                    row["ZRXM"] = ZRXM.Text.Trim();
                    row["KCR"] = KCR.Text.Trim();
                    row["CMZYJ"] = CMZYJ.Text.Trim();

                    row["SQRQ"] = SQRQ.Text.Trim();

                    row["QLR"] = QLR.Text.Trim();
                    row["TEXTBZ1"] = TEXTBZ1.Text.Trim();

                    row["SQRLX"] = SQRLX.Text.Trim();
                    // row["QLR_ZJZL"] = FRDBZJZL.Text.Trim();
                    //row["QLR_ZJBH"] = FRDBZJBH.Text.Trim();
                    row["DWXZ"] = DWXZ.Text.Trim();
                    row["FRDBXM"] = FRDBXM.Text.Trim();
                    row["TXDZ"] = TXDZ.Text.Trim();
                    row["YZBM"] = YZBM.Text.Trim();
                    row["DZYJ"] = DZYJ.Text.Trim();
                    row["LXR"] = LXR.Text.Trim();
                    //row["LXDH"] = FRDBDH.Text.Trim();
                    //row["SQS_DLRXM"] = SQS_DLRXM.Text.Trim();
                    row["ZYZGZSH"] = ZYZGZSH.Text.Trim();
                    row["DLJGMC"] = DLJGMC.Text.Trim();
                    row["DLJGDH"] = DLJGDH.Text.Trim();
                    row["QLR2"] = QLR2.Text.Trim();
                    row["SQRLX2"] = SQRLX2.Text.Trim();
                    row["QLR_ZJZL2"] = QLR_ZJZL2.Text.Trim();
                    row["QLR_ZJBH2"] = QLR_ZJBH2.Text.Trim();
                    row["DWXZ2"] = DWXZ2.Text.Trim();
                    row["FRDBXM2"] = FRDBXM2.Text.Trim();
                    row["TXDZ2"] = TXDZ2.Text.Trim();
                    row["YZBM2"] = YZBM2.Text.Trim();
                    row["DZYJ2"] = DZYJ2.Text.Trim();
                    row["LXR2"] = LXR2.Text.Trim();
                    row["LXDH2"] = LXDH2.Text.Trim();
                    row["SQS_DLRXM2"] = SQS_DLRXM2.Text.Trim();
                    row["DLJGMC2"] = DLJGMC2.Text.Trim();
                    row["DLJGDH2"] = DLJGDH2.Text.Trim();
                    row["DLJGDH2"] = DLJGDH2.Text.Trim();
                    row["XZGHBMFZR"] = XZGHBMFZR.Text.Trim();//村民组名称
                    //}
                    #endregion
                    #region 申请书二
                    row["TDZL"] = TDZL.Text.Trim();
                    row["SJYT"] = SJYT.Text.Trim();
                    row["DLBM"] = DLBM.Text.Trim();
                    row["QSXZ"] = QSXZ.Text.Trim();
                    row["SYQLX"] = SYQLX.Text.Trim();
                    row["QLSLQJ"] = QLSLQJ.Text.Trim();
                    row["XYDZL"] = XYDZL.Text.Trim();
                    if (TDJG.Text.Trim() == "")
                    {
                        row["TDJG"] = 0;
                    }
                    else
                    {
                        row["TDJG"] = Convert.ToDouble(TDJG.Text.Trim());
                    }
                    if (TDDYMJ.Text.Trim() == "")
                    {
                        row["TDDYMJ"] = 0;
                    }
                    else
                    {
                        row["TDDYMJ"] = Convert.ToDouble(TDDYMJ.Text.Trim());
                    }
                    row["DYSX"] = DYSX.Text.Trim();
                    row["TDDYJE"] = Convert.ToDouble(TDDYJE.Text.Trim());
                    row["SQDJLR"] = SQDJLR.Text.Trim();
                    row["SQSBZ"] = SQSBZ.Text.Trim();
                    row["TEXTBZ2"] = TEXTBZ2.Text;//提交的土地证书编号
                    //row["FBZ2"] = Convert.ToDouble(QDJG.Text.ToString().Trim());//取得价格
                    if (DYB.Checked == true)
                    {
                        row["DYQX_Q"] = "单一版";
                    }
                    else
                    {
                        row["DYQX_Q"] = "集成版";
                    }
                    if (FBCZ_YSE.Checked == true)
                    {
                        row["DYQX_Z"] = "是";
                    }
                    else
                    {
                        row["DYQX_Z"] = "否";
                    }
                    //if (DYSQRJZRQ.Checked == false)
                    //{
                    //    row["DYSQRJZRQ"] = "";
                    //}
                    //else
                    //{
                    row["DYSQRJZRQ"] = DYSQRJZRQ.Text.Trim();
                    //}
                    //if (DESQRJZRQ.Checked == false)
                    //{
                    //    row["DESQRJZRQ"] = "";
                    //}
                    //else
                    //{
                    row["DESQRJZRQ"] = DESQRJZRQ.Text.Trim();
                    //}
                    #endregion
                    #region 调查表
                    row["BZ"] = BZ.Text.Trim();
                    row["DZ"] = DZ.Text.Trim();
                    row["NZ"] = NZ.Text.Trim();
                    row["XZ"] = XZ.Text.Trim();
                    row["FRDBZJBH"] = FRDBZJBH.Text.Trim();
                    row["FRDBZJZL"] = FRDBZJZL.Text.Trim();
                    row["FRDBDH"] = FRDBDH.Text.Trim();
                    row["JBB_DLRXM"] = JBB_DLRXM.Text.Trim();
                    row["JBB_DLRZJBH"] = JBB_DLRZJBH.Text.Trim();
                    row["JBB_DLRZJZL"] = JBB_DLRZJZL.Text.Trim();
                    row["JBB_DLRDH"] = JBB_DLRDH.Text.Trim();
                    row["GMJJHYDM"] = GMJJHYDM.Text.Trim();
                    row["YBZDDM"] = YBZDDM.Text.Trim();
                    row["TFH"] = TFH.Text.Trim();
                    row["PZYT"] = PZYT.Text.Trim();
                    row["PZYT_DLDM"] = PZYT_DLDM.Text.Trim();
                    // row["FZMJ"] =Convert.ToDouble( JBB_PZMJ.Text.Trim());
                    row["JZZDMJ"] = Convert.ToDouble(JZZDMJ.Text.Trim());
                    row["JZMJ"] = Convert.ToDouble(JZMJ.Text.Trim());
                    row["GYQLRQK"] = GYQLRQK.Text.Trim();
                    row["SM"] = SM.Text.Trim();
                    //if (DCBRQ.Checked == false)
                    //{
                    //    row["DCBRQ"] = "";
                    //}
                    //else
                    //{
                    row["DCBRQ"] = DCBRQ.Text.Trim();
                    //}
                    //if (SYQX_Q.Checked == false)
                    //{
                    //    row["SYQX_Q"] = "";
                    //}
                    //else
                    //{
                    row["SYQX_Q"] = SYQX_Q.Text.Trim();
                    //}
                    //if (SYQX_Z.Checked == false)
                    //{
                    //    row["SYQX_Z"] = "";
                    //}
                    //else
                    //{
                    row["SYQX_Z"] = SYQX_Z.Text.Trim();
                    //}
                    #endregion
                    #region 审核表
                    row["QSDCJS"] = QSDCJS.Text;
                    row["DCYXM"] = DCYXM.Text.Trim();
                    row["DJKZJS"] = DJKZJS.Text;
                    row["CLYXM"] = CLYXM.Text.Trim();
                    row["DJDCJGSHYJ"] = DJDCJGSHYJ.Text;
                    row["SHRXM"] = SHRXM.Text.Trim();
                    //if (DCRQ.Checked == false)
                    //{
                    //    row["DCRQ"] = "";
                    //}
                    //else
                    //{
                    row["DCRQ"] = DCRQ.Text.Trim();
                    //}
                    //if (CLRQ.Checked == false)
                    //{
                    //    row["CLRQ"] = "";
                    //}
                    //else
                    //{
                    row["CLRQ"] = CLRQ.Text.Trim();
                    //}
                    //if (SHRQ.Checked == false)
                    //{
                    //    row["SHRQ"] = "";
                    //}
                    //else
                    //{
                    row["SHRQ"] = SHRQ.Text.Trim();
                    //}
                    //if (SHHG.Checked == true)
                    //{
                    //    row["SHHG"] = true;
                    //}
                    //else
                    //{
                    row["SHHG"] = false;
                    //}
                    #endregion
                    #region 审批表
                    row["FZMJ"] = Convert.ToDouble(FZMJ.Text.Trim());
                    row["CSYJ"] = CSYJ.Text;
                    row["CSSCR"] = CSSCR.Text.Trim();
                    //row["CSRTDDJSGZH"] = CSRTDDJSGZH.Text.Trim();
                    row["SHYJ"] = SHYJ.Text;
                    row["SHFZR"] = SHFZR.Text.Trim();
                    //row["SHRTDDJSGZH"] = SHRTDDJSGZH.Text.Trim();
                    row["PZYJ"] = PZYJ.Text;
                    row["PZR"] = PZR.Text.Trim();
                    row["SPBBZ"] = SPBBZ.Text.Trim();
                    row["FZJS"] = FZJS.Text;
                    // row["QSDCJS"] = QSDCJS.Text.Trim();
                    //if (CSRJ.Checked == false)
                    //{
                    //    row["CSRJ"] = "";
                    //}
                    //else
                    //{
                    row["CSRJ"] = CSRJ.Text.Trim();
                    //}
                    //if (SHRQ_SPB.Checked == false)
                    //{
                    //    row["SHRQ_SPB"] = "";
                    //}
                    //else
                    //{
                    row["SHRQ_SPB"] = SHRQ_SPB.Text.Trim();
                    //}
                    //if (PZRQ.Checked == false)
                    //{
                    //    row["PZRQ"] = "";
                    //}
                    //else
                    //{
                    row["PZRQ"] = PZRQ.Text.Trim();
                    //}
                    row["CSHG"] = CSHG.Checked;
                    row["SHTG"] = SHTG.Checked;
                    row["PZTG"] = PZTG.Checked;
                    #endregion
                    #region 更新数据库
                    try
                    {
                        DP.Update(da_ZD, "DCB");
                    }
                    catch (System.Exception ee)
                    {
                        MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    try
                    {
                        this.dataGridView2.EndEdit();
                        this.BindingContext[da_ZDX, "D_ZDX"].EndCurrentEdit();
                        DPX.Update(da_ZDX, "D_ZDX");
                    }
                    catch (System.Exception ee)
                    {
                        MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    try
                    {
                        this.dataGridView1.EndEdit();
                        this.BindingContext[da_ZDD, "D_ZDD"].EndCurrentEdit();
                        DPD.Update(da_ZDD, "D_ZDD");
                    }
                    catch (System.Exception ee)
                    {
                        MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                    connection.Close();


                    #endregion

                }
                #region 保存默认值
                ATT.SYQLX_m = SYQLX.Text;
                ATT.LXR2_m = LXR2.Text;
                ATT.DYB_m = DYB.Checked;
                ATT.FBCZ_YSE_m = FBCZ_YSE.Checked;
                ATT.SQRLX2_m = SQRLX2.Text;
                ATT.LXR_m = LXR.Text.Trim();
                ATT.SQRLX_m = SQRLX.Text.Trim();//申请类型
                ATT.FRDBZJZL_m = FRDBZJZL.Text.Trim();//证件种类
                ATT.DWXZ_m = DWXZ.Text.Trim();//单位性质
                ATT.TXDZ_m = TXDZ.Text.Trim();//通讯地址
                ATT.YZBM_m = YZBM.Text.Trim();
                ATT.TEXTBZ1_m = TEXTBZ1.Text.Trim();
                ATT.DWXZ2_m = DWXZ2.Text.Trim();
                ATT.DLJGDH_m = DLJGDH.Text.Trim();
                ATT.DLJGDH2_m = DLJGDH2.Text.Trim();
                ATT.XZGHBMFZR_m = XZGHBMFZR.Text.Trim();
                ATT.SQRQ_m = SQRQ.Text.Trim();//申请日期
                ATT.TDZL_m = TDZL.Text.Trim();//坐    落
                ATT.SJYT_m = SJYT.Text.Trim();//实际用途
                //ATT.DLBM_m = DLBM.Text.Trim();//地类编码
                ATT.QSXZ_m = QSXZ.Text.Trim();//权属性质
                ATT.QLSLQJ_m = QLSLQJ.Text.Trim();//权利设立情况
                ATT.SQDJLR_m = SQDJLR.Text.Trim();//申请登记的内容
                ATT.DYSQRJZRQ_m = DYSQRJZRQ.Text.Trim();//第一申请人签章日期
                ATT.DESQRJZRQ_m = DESQRJZRQ.Text.Trim();//第二申请人签章日期
                ATT.GMJJHYDM_m = GMJJHYDM.Text.Trim();//国民经济行业代码
                ATT.PZYT_m = PZYT.Text.Trim();//批准用途
                ATT.PZYT_DLDM_m = PZYT_DLDM.Text.Trim();//批准用途地类编码
                ATT.DCBRQ_m = DCBRQ.Text.Trim();//调查表日期
                ATT.SYQX_Q_m = SYQX_Q.Text.Trim();//使用期限  自
                ATT.QSDCJS = QSDCJS.Text;//权属调查记事
                ATT.DCYXM = DCYXM.Text.Trim();//调查员姓名

                ATT.DJKZJS = DJKZJS.Text.Trim();//地籍测量记事
                ATT.CLYXM = CLYXM.Text.Trim();//测量员姓名

                ATT.SHHG = SHHG.Checked;//审核合格
                ATT.DJDCJGSHYJ = DJDCJGSHYJ.Text.Trim();//地籍调查结果审核意见
                ATT.SHRXM = SHRXM.Text.Trim();//审核人姓名
                ATT.DCRQ = DCRQ.Text.Trim();//调查日期
                ATT.CLRQ = CLRQ.Text.Trim();//测量日期
                ATT.SHRQ = SHRQ.Text.Trim();//审核日期
                ATT.CSYJ = CSYJ.Text.Trim();//国土资源行政主管部门初审意见
                ATT.CSSCR = CSSCR.Text.Trim();//审查人

                ATT.SHYJ = SHYJ.Text.Trim();//国土资源行政主管部门审核意见
                ATT.SHFZR = SHFZR.Text.Trim();//负责人
                ATT.PZYJ = PZYJ.Text.Trim();//人民政府批准意见
                ATT.PZR = PZR.Text.Trim();//人民政府负责人
                ATT.SPBBZ = SPBBZ.Text.Trim();//备注
                ATT.FZJS = FZJS.Text.Trim();//土地证书记事
                ATT.CSRJ = CSRJ.Text.Trim();//审查日  期
                ATT.SHRQ_SPB = SHRQ_SPB.Text.Trim();//初审日  期
                ATT.PZRQ = PZRQ.Text.Trim();//人民政府批准日  期
                if (CSHG.Checked)
                {
                    ATT.CSHG = true;
                }
                else
                {
                    ATT.CSHG = false;//初审合格
                }
                if (SHTG.Checked)
                {
                    ATT.SHTG = true;
                }
                else
                {
                    ATT.SHTG = false;//审核通过
                }
                if (PZTG.Checked)
                {
                    ATT.PZTG = true;
                }
                else
                {
                    ATT.PZTG = false;//通过批准
                }

                ATT.ZZXM = ZZXM.Text.Trim();// 
                ATT.CMZYJ = CMZYJ.Text;// 
                ATT.ZRXM = ZRXM.Text.Trim();// 
                ATT.KCR = KCR.Text.Trim();// 

                #endregion
                addlist(TXDZ, ATT.TXDZ_m);
                addlist(TDZL, ATT.TDZL_m);
                addlist(TEXTBZ1, ATT.TEXTBZ1_m);
            }
            catch (System.Exception ee)
            {
                MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        // 申请书（二） - “使用上次默认值”按钮
        private void button19_Click(object sender, EventArgs e)
        {
            DYB.Checked = ATT.DYB_m;
            JCB.Checked = !ATT.DYB_m;
            FBCZ_YSE.Checked = ATT.FBCZ_YSE_m;
            FBCZ_NO.Checked = !ATT.FBCZ_YSE_m;
            LXR2.Text = ATT.LXR2_m;
            SQDJLR.Text = ATT.SQDJLR_m;//申请登记的内容
            DYSQRJZRQ.Text = ATT.DYSQRJZRQ_m;//第一申请人签章日期
            DESQRJZRQ.Text = ATT.DESQRJZRQ_m;//第二申请人签章日期

        }


        private void QLR_ZJBH_Click(object sender, EventArgs e)
        {
            if (FRDBZJBH.Text == "" && FRDBZJZL.Text == "居民身份证")
            {
                FRDBZJBH.Text = treeView1.SelectedNode.Tag.ToString().Substring(0, 6) + "19";
                FRDBZJBH.SelectionStart = 8;
            }
        }

        // 基本表 - “使用上次默认值”按钮
        private void SYSCMRZ_JBB_Click(object sender, EventArgs e)
        {
            SYQLX.Text = ATT.SYQLX_m;
            QLSLQJ.Text = ATT.QLSLQJ_m;
            DWXZ.Text = ATT.DWXZ_m;
            QSXZ.Text = ATT.QSXZ_m;
            DLJGDH2.Text = ATT.DLJGDH2_m;
            DCBRQ.Text = ATT.DCBRQ_m;//调查表日期
            GMJJHYDM.Text = ATT.GMJJHYDM_m;//国民经济行业代码
            PZYT.Text = ATT.PZYT_m;//批准用途
            PZYT_DLDM.Text = ATT.PZYT_DLDM_m;//批准用途地类编码
            SYQX_Q.Text = ATT.SYQX_Q_m; //使用期限  自
            SYQX_Z.Text = ATT.SYQX_Z_m; //使用期限  至
            //if (ATT.DCBRQ_m != "")
            //{
            //    DCBRQ.Checked = true;
            //}
            //else
            //{
            //    DCBRQ.Checked = false;
            //}
            //if (ATT.SYQX_Q_m != "")
            //{
            //    SYQX_Q.Checked = true;
            //}
            //else
            //{
            //    SYQX_Q.Checked = false;
            //}
            //if (ATT.SYQX_Z_m != "")
            //{
            //    SYQX_Z.Checked = true;
            //}
            //else
            //{
            //    SYQX_Z.Checked = false;
            //}
        }

        private void PZYT_DLDM_SelectedIndexChanged(object sender, EventArgs e)
        {
            PZYT.Text = find_dlmc(PZYT_DLDM.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            SHBPZ shpz = new SHBPZ();
            shpz.ShowDialog();
        }

        // 审核表 粘贴自定义值1
        private void button14_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            QSDCJS.Text = readOrWritexml.ReadXml("//权属调查记事1");
            DCYXM.Text = readOrWritexml.ReadXml("//调查员姓名1");
            DCRQ.Text = readOrWritexml.ReadXml("//调查日期1");
            DJKZJS.Text = readOrWritexml.ReadXml("//地籍测量记事1");
            CLYXM.Text = readOrWritexml.ReadXml("//测量员姓名1");
            CLRQ.Text = readOrWritexml.ReadXml("//测量日期1");
            DJDCJGSHYJ.Text = readOrWritexml.ReadXml("//地籍调查结果审核意见1");
            SHRXM.Text = readOrWritexml.ReadXml("//审核人姓名1");
            SHRQ.Text = readOrWritexml.ReadXml("//审核日期1");
            if (readOrWritexml.ReadXml("//审核合格1") == "1")
            {
                SHHG.Checked = true;
            }
            else
            {
                SHHG.Checked = false;
            }

            DataRow row = TB_DCB.Rows[0];
            QSDCJS.Text = QSDCJS.Text.Replace("【土地坐落】", row["TDZL"].ToString().Trim());
            QSDCJS.Text = QSDCJS.Text.Replace("【权利性质】", row["QSXZ"].ToString().Trim());
            QSDCJS.Text = QSDCJS.Text.Replace("【权利类型】", row["SYQLX"].ToString().Trim());
            double zdmj = (double)row["ZDMJ"];
            QSDCJS.Text = QSDCJS.Text.Replace("【宗地面积】", zdmj.ToString("0.00"));
            QSDCJS.Text = QSDCJS.Text.Replace("【实际用途】", row["SJYT"].ToString().Trim());

            DJKZJS.Text = DJKZJS.Text.Replace("【土地坐落】", row["TDZL"].ToString().Trim());
            DJKZJS.Text = DJKZJS.Text.Replace("【宗地面积】", zdmj.ToString("0.00"));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            QSDCJS.Text = readOrWritexml.ReadXml("//权属调查记事2");
            DCYXM.Text = readOrWritexml.ReadXml("//调查员姓名2");

            DCRQ.Text = readOrWritexml.ReadXml("//调查日期2");

            DJKZJS.Text = readOrWritexml.ReadXml("//地籍测量记事2");
            CLYXM.Text = readOrWritexml.ReadXml("//测量员姓名2");

            CLRQ.Text = readOrWritexml.ReadXml("//测量日期2");

            DJDCJGSHYJ.Text = readOrWritexml.ReadXml("//地籍调查结果审核意见2");
            SHRXM.Text = readOrWritexml.ReadXml("//审核人姓名2");

            SHRQ.Text = readOrWritexml.ReadXml("//审核日期2");

            if (readOrWritexml.ReadXml("//审核合格2") == "1")
            {
                SHHG.Checked = true;
            }
            else
            {
                SHHG.Checked = false;
            }
            int jzdgs = da_ZDD.Tables["D_ZDD"].Rows.Count - 1;
            QSDCJS.Text = QSDCJS.Text.Replace("#", jzdgs.ToString());//替换界址点个数
        }

        private void ZDYMRZ_SPB_Click(object sender, EventArgs e)
        {
            SPB spb = new SPB();
            spb.ShowDialog();
        }

        // 审批表 使用自定义值粘贴
        private void SYZDYZZT_SPB_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            // 初审
            CSYJ.Text = readOrWritexml.ReadXml("//国土资源行政主管部门初审意见");
            CSSCR.Text = readOrWritexml.ReadXml("//审查人");
            CSRJ.Text = readOrWritexml.ReadXml("//审查日期");

            if (readOrWritexml.ReadXml("//初审合格") != "1")
            {
                CSHG.Checked = false;
            }
            else
            {
                CSHG.Checked = true;
            }
            // 审核
            SHYJ.Text = readOrWritexml.ReadXml("//国土资源行政主管部门审核意见");
            SHFZR.Text = readOrWritexml.ReadXml("//审核负责人");
            SHRQ_SPB.Text = readOrWritexml.ReadXml("//审核日期");

            if (readOrWritexml.ReadXml("//审核通过") != "1")
            {
                SHTG.Checked = false;
            }
            else
            {
                SHTG.Checked = true;
            }
            // 批准
            PZYJ.Text = readOrWritexml.ReadXml("//人民政府批准意见");
            PZR.Text = readOrWritexml.ReadXml("//批准人");
            PZRQ.Text = readOrWritexml.ReadXml("//政府批准日期");
            if (readOrWritexml.ReadXml("//通过批准") != "1")
            {
                PZTG.Checked = false;
            }
            else
            {
                PZTG.Checked = true;
            }
            SPBBZ.Text = readOrWritexml.ReadXml("//备注");
            FZJS.Text = readOrWritexml.ReadXml("//土地证书记事");

            //DataRow row = TB_DCB.Rows[0];
            //// 初审
            //CSYJ.Text = CSYJ.Text.Replace("【权利人】", row["QLR"].ToString().Trim());
            //double zdmj = (double)row["ZDMJ"];
            //CSYJ.Text = CSYJ.Text.Replace("【宗地面积】", zdmj.ToString("0.00"));
            //if (Convert.ToDouble(FZMJ.Text.Trim()) < 0.001)
            //{
            //    MessageBox.Show("发证面积不可为0。", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //}
            //else
            //{
            //    CSYJ.Text = CSYJ.Text.Replace("【批准面积】", FZMJ.Text.Trim());
            //}
            //double jzzmj = (double)row["JZMJ"];
            //CSYJ.Text = CSYJ.Text.Replace("【建筑总面积】", jzzmj.ToString("0.00"));
            //double jzzdmj = (double)row["JZZDMJ"];
            //CSYJ.Text = CSYJ.Text.Replace("【建筑占地面积】", jzzdmj.ToString("0.00"));
        }


        private void FZMJ_DoubleClick(object sender, EventArgs e)
        {
            FZMJ.Text = ZDMJ_SHB.Text;
        }

        private void FRDBXM_DoubleClick(object sender, EventArgs e)
        {
            FRDBXM.Text = QLR.Text.Trim();
        }

        private void QLR_TextChanged(object sender, EventArgs e)
        {
            if (QLR.Text.Trim().Length <= 4)
            {
                FRDBXM.Text = QLR.Text.Trim();
            }
        }


        // 申请书（二） - “自定义登记默认值”按钮
        private void button2_Click(object sender, EventArgs e)
        {
            SQDJLR sqdjlr = new SQDJLR();
            sqdjlr.ShowDialog();
        }

        // 申请书（二） - “粘贴登记默认值”按钮
        private void button1_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            SQDJLR.Text = readOrWritexml.ReadXml("//自定义申请登记的内容");
            LXR2.Text = readOrWritexml.ReadXml("//登记原因");

        }

        private void FenXiFangxiang()//分析东南西北四个方向界址点号
        {
            double x, y;
            int RowLeng = da_ZDD.Tables["D_ZDD"].Rows.Count - 1;
            int[] JZDH = new int[RowLeng + 1];
            Double[] JZDX = new Double[RowLeng + 1];
            Double[] JZDY = new Double[RowLeng + 1];
            Double[] JZDJL = new Double[RowLeng + 1];
            double X = 0, Y = 0;
            for (int i = 0; i < RowLeng; i++)
            {
                DataRow row = da_ZDD.Tables["D_ZDD"].Rows[i];
                x = Convert.ToDouble(row["X"].ToString());
                y = Convert.ToDouble(row["Y"].ToString());

                JZDH[i] = i + 1;
                JZDX[i] = y;
                JZDY[i] = x;
                X = x + X;
                Y = y + Y;
            }

            /////////////////////////////////////////////////////////////////////
            ATT.JZZN = RowLeng.ToString();//界址点个数
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
                       * Math.Cos(Tools.CADTools.FWJ(JZDX[i], JZDY[i]) - DX0);
                YY1[i] = Math.Sqrt(JZDX[i] * JZDX[i] + JZDY[i] * JZDY[i])
                      * Math.Sin(Tools.CADTools.FWJ(JZDX[i], JZDY[i]) - DX0);
            }


            double dx2 = -1 * (Math.PI / 2.0 - DX0);
            for (int i = 0; i < RowLeng; i++)
            {
                XX2[i] = Math.Sqrt(JZDX[i] * JZDX[i] + JZDY[i] * JZDY[i])
                      * Math.Cos(Tools.CADTools.FWJ(JZDX[i], JZDY[i]) - dx2);
                YY2[i] = Math.Sqrt(JZDX[i] * JZDX[i] + JZDY[i] * JZDY[i])
                      * Math.Sin(Tools.CADTools.FWJ(JZDX[i], JZDY[i]) - dx2);
            }

            int[] JZDn = new int[4];
            JZDn[0] = JZDH[0];
            double maxXY = -999999999999999.0;
            for (int i = 1; i <= RowLeng - 3; i++)
            {
                if (maxXY < JZDX[i] + JZDY[i])
                {
                    maxXY = JZDX[i] + JZDY[i];
                    JZDn[1] = JZDH[i];
                }
            }
            maxXY = -999999999999999.0;
            for (int i = JZDn[1]; i <= RowLeng - 2; i++)
            {
                if (maxXY < JZDY[i] - JZDX[i])
                {
                    maxXY = JZDY[i] - JZDX[i];
                    JZDn[2] = JZDH[i];
                }
            }

            maxXY = 999999999999999.0;
            for (int i = JZDn[2]; i <= RowLeng - 1; i++)
            {
                if (maxXY > JZDX[i] + JZDY[i])
                {
                    maxXY = JZDX[i] + JZDY[i];
                    JZDn[3] = JZDH[i];
                }
            }
            ATT.DongBai = JZDn[1];
            ATT.DongNan = JZDn[2];
            ATT.XiNan = JZDn[3];

        }

        public struct ZDD
        {
            public string ZDH;
            public string XH;
            public string JZDH;
        }

        // 界址签章 - “填写分析”按钮
        private void TXFX_Click(object sender, EventArgs e)
        {
            #region  打开条件与准备工作
            if (this.HZLB.SelectedIndex == -1)
            {
                return;
            }
            ListBoxsx listBoxsx = (ListBoxsx)HZLB.Items[HZLB.SelectedIndex];
            string ndjh = listBoxsx.NHBH;
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            string cxh = ndjh.Substring(0, 12);//不含宗地号的值
            #endregion
            #region 查找此街坊的所有界址点表值
            ArrayList ArrZDD = new ArrayList();//此街坊的所有界址点表值
            using (SqlConnection Sqlcon = new SqlConnection())//定义连接
            {
                Sqlcon.ConnectionString = connectionString;
                string queryString = "SELECT * " + "FROM D_ZDD " + "WHERE (DJH LIKE " + "'" + cxh + "%" + "')";//查询字符串
                SqlCommand command = new SqlCommand(queryString, Sqlcon);//连接数据库
                Sqlcon.Open();
                if (Sqlcon.State == ConnectionState.Broken)
                {
                    MessageBox.Show("数据库连接失败！");
                    return;
                }
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ZDD zdd = new ZDD();
                    zdd.ZDH = reader["DJH"].ToString().Trim();
                    zdd.XH = reader["MBBSM"].ToString().Trim();
                    zdd.JZDH = reader["JZDH"].ToString().Trim();
                    ArrZDD.Add(zdd);
                }
                reader.Close();
                Sqlcon.Close();
            }
            #endregion
            #region 查找邻宗地
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                string sjzdh = this.dataGridView2["QDH", i].Value.ToString().Trim();//指界表中起界址点号
                string ejzdh = this.dataGridView2["ZDH", i].Value.ToString().Trim();
                for (int j = 0; j < ArrZDD.Count - 1; j++)
                {
                    ZDD Szdd = new ZDD();
                    ZDD Ezdd = new ZDD();
                    Szdd = (ZDD)ArrZDD[j];
                    Ezdd = (ZDD)ArrZDD[j + 1];
                    if (Szdd.ZDH == Ezdd.ZDH)
                    {
                        if (Szdd.JZDH == ejzdh && Ezdd.JZDH == sjzdh)
                        {
                            this.dataGridView2["LZDDJH", i].Value = Szdd.ZDH;
                            break;
                        }
                    }
                }
            }
            #endregion
            #region 查找邻宗指界人
            using (SqlConnection Lzconn = new SqlConnection())
            {
                Lzconn.ConnectionString = connectionString;
                Lzconn.Open();
                if (Lzconn.State == ConnectionState.Broken)
                {
                    MessageBox.Show("数据库连接失败！");
                    return;
                }
                for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
                {
                    string Lzdjh = this.dataGridView2["LZDDJH", i].Value.ToString().Trim();
                    if (Lzdjh != "")
                    {
                        string selconne = "SELECT * FROM DCB WHERE (DJH = '" + Lzdjh + "')";
                        SqlCommand command = new SqlCommand(selconne, Lzconn);//连接数据库
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string bzdzjr = reader["FRDBXM"].ToString().Trim();
                            string dlr = reader["JBB_DLRXM"].ToString().Trim();
                            if (dlr != "")
                            {
                                dataGridView2.Rows[i].Cells["LZDZJRXM"].Value = dlr;
                            }
                            else
                            {
                                dataGridView2.Rows[i].Cells["LZDZJRXM"].Value = bzdzjr;
                            }
                            dataGridView2.Rows[i].Cells["BZ"].Value = reader["QLR"].ToString().Trim();
                        }
                        reader.Close();
                    }
                }
                Lzconn.Close();
            }
            #endregion
            #region 简化界址签章表
            FenXiFangxiang();//分析东南西北四个方向界址点号
            string jzdh1, jzdh2, sz1, sz2;
            sz1 = this.dataGridView2["BZ", 0].Value.ToString().Trim();
            jzdh1 = this.dataGridView2["MBBSM", 0].Value.ToString().Trim();
            int dongbai = ATT.DongBai;
            int dongnan = ATT.DongNan;
            int xinan = ATT.XiNan;
            for (int i = 0; i < this.dataGridView2.Rows.Count - 1; i++)
            {
                sz2 = this.dataGridView2["BZ", i + 1].Value.ToString().Trim();
                jzdh2 = this.dataGridView2["MBBSM", i + 1].Value.ToString().Trim();

                if (sz1 == sz2)
                {
                    if (jzdh2 != dongbai.ToString()
                           && jzdh2 != dongnan.ToString() && jzdh2 != xinan.ToString())
                    {
                        this.dataGridView2["ZDH", i].Value = this.dataGridView2["ZDH", i + 1].Value;
                        this.dataGridView2["ZJDH", i].Value = this.dataGridView2["QDH", i + 1].Value;
                        this.dataGridView2.Rows.RemoveAt(i + 1);
                        i--;
                    }
                    if (jzdh2 == dongbai.ToString()
                           || jzdh2 == dongnan.ToString() || jzdh2 == xinan.ToString())
                    {
                        sz1 = sz2;
                        jzdh1 = jzdh2;
                    }
                }
                else
                {
                    sz1 = sz2;
                    jzdh1 = jzdh2;
                }
            }
            this.dataGridView2.Update();
            #endregion
            tcsz();
        }

        // 界址签章 - “打开初始表”按钮
        private void button7_Click(object sender, EventArgs e)
        {
            if (this.HZLB.SelectedIndex == -1)
            {
                return;
            }
            dataGridView2.Update();
            while (dataGridView2.BindingContext[da_ZDX, "D_ZDX"].Count > 0)
            {
                dataGridView2.BindingContext[da_ZDX, "D_ZDX"].RemoveAt(0);
            }

            System.Data.DataTable Table_zdd = da_ZDD.Tables["D_ZDD"];
            string bzdzjr = TB_DCB.Rows[0]["FRDBXM"].ToString().Trim();//本宗指界人
            string dlr = TB_DCB.Rows[0]["JBB_DLRXM"].ToString().Trim();
            if (dlr != "")
            {
                bzdzjr = dlr;
            }
            string tc = MRTC.Text.Trim();
            if (tc == "")
            {
                tc = "空地";
            }
            for (int i = 0; i < Table_zdd.Rows.Count - 1; i++)
            {
                dataGridView2.BindingContext[da_ZDX, "D_ZDX"].AddNew();
                dataGridView2.Rows[i].Cells["DJH"].Value = Table_zdd.Rows[i]["DJH"];//地籍号
                dataGridView2.Rows[i].Cells["MBBSM"].Value = i + 1;//排序号
                dataGridView2.Rows[i].Cells["QDH"].Value = Table_zdd.Rows[i]["JZDH"];//起点号
                dataGridView2.Rows[i].Cells["ZDH"].Value = Table_zdd.Rows[i + 1]["JZDH"];//终点号
                dataGridView2.Rows[i].Cells["BZ"].Value = tc;//邻宗地权利人
                dataGridView2.Rows[i].Cells["BZDZJRXM"].Value = bzdzjr;//本宗地指界人姓名
                dataGridView2.Rows[i].Cells["ZJRQ"].Value = this.ZJRQ.Text;//指界日期
                dataGridView2.Rows[i].Cells["LZDDJH"].Value = "";//邻宗地地籍号
            }
        }

        // 界址签章 - “删除选中行”按钮
        private void SCXZH_Click(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                this.BindingContext[da_ZDX, "D_ZDX"].RemoveAt(this.BindingContext[da_ZDX, "D_ZDX"].Position);
            }
        }

        private void HT_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = HT.Text;
                    tcsz();
                }
            }
            HT.Checked = false;
        }

        private void DL_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = DL.Text;
                    tcsz();
                }
            }
            DL.Checked = false;
        }

        private void GGDL_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = GGDL.Text;
                    tcsz();
                }
            }
            GGDL.Checked = false;
        }

        private void KD_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = KD.Text;
                    tcsz();
                }
            }
            KD.Checked = false;
        }

        private void NT_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = NT.Text;
                    tcsz();
                }
            }
            NT.Checked = false;
        }

        private void XD_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = XD.Text;
                    tcsz();
                }
            }
            XD.Checked = false;
        }

        private void JX_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = JX.Text;
                    tcsz();
                }
            }
            JX.Checked = false;
        }

        private void HD_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = HD.Text;
                    tcsz();
                }
            }
            HD.Checked = false;
        }

        private void CD_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = CD.Text;
                    tcsz();
                }
            }
            CD.Checked = false;
        }

        private void TANG_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = TANG.Text;
                    tcsz();
                }
            }
            TANG.Checked = false;
        }

        private void ZDY1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = ZDY1.Text;
                    tcsz();
                }
            }
            ZDY1.Checked = false;
        }

        private void ZDY2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = ZDY2.Text;
                    tcsz();
                }
            }
            ZDY2.Checked = false;
        }

        private void ZDY3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BindingContext[da_ZDX, "D_ZDX"].Position > -1)
            {
                if (dataGridView2.CurrentCell.ColumnIndex == 9)
                {
                    dataGridView2.CurrentCell.Value = ZDY3.Text;
                    tcsz();
                }
            }
            ZDY3.Checked = false;
        }

        // 界址签章 - “自定义设置”按钮
        private void ZDYSZ_QZB_Click(object sender, EventArgs e)
        {
            ZDY zdy = new ZDY();
            zdy.ShowDialog();
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            this.ZDY1.Text = readOrWritexml.ReadXml("//界址签章表自定义1");
            this.ZDY2.Text = readOrWritexml.ReadXml("//界址签章表自定义2");
            this.ZDY3.Text = readOrWritexml.ReadXml("//界址签章表自定义3");
        }

        // 填充四至
        /// <summary>
        /// 填充四至
        /// </summary>
        public void tcsz()
        {
            string bj = "1";
            this.BZ.Text = "";
            this.DZ.Text = "";
            this.NZ.Text = "";
            this.XZ.Text = "";
            for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
            {
                string wr = "";
                int nn = i + 1;
                if (nn == dataGridView2.Rows.Count)
                {
                    nn = 0;
                }
                string jzdh = this.dataGridView2["MBBSM", i].Value.ToString().Trim();
                string lzdjh = this.dataGridView2["LZDDJH", i].Value.ToString().Trim();
                string zjr = this.dataGridView2["BZ", i].Value.ToString().Trim();
                if (jzdh == "1")
                {
                    bj = "1";
                }
                if (jzdh == ATT.DongBai.ToString())
                {
                    bj = "2";
                }
                if (jzdh == ATT.DongNan.ToString())
                {
                    bj = "3";
                }
                if (jzdh == ATT.XiNan.ToString())
                {
                    bj = "4";
                }
                wr = this.dataGridView2["MBBSM", i].Value.ToString().Trim() + "-" + this.dataGridView2["MBBSM", nn].Value.ToString().Trim();
                if (lzdjh != "")
                {
                    if (zjr != "")
                    {
                        //wr = wr + ":" + zjr;
                        wr = zjr;
                    }
                    else
                    {
                        int hh = Convert.ToInt32(lzdjh.Substring(12, 5));
                        //wr = wr + ":" + hh.ToString() + "号宗地";
                        wr = hh.ToString() + "号宗地";
                    }
                }
                else
                {
                    //wr = wr + ":" + zjr;
                    wr = zjr;
                }

                if (bj == "1")
                {
                    string dh = ";";
                    if (this.BZ.Text.Trim() == "")
                    {
                        dh = "";
                    }
                    this.BZ.Text = this.BZ.Text.Trim() + dh + wr;
                }
                if (bj == "2")
                {
                    string dh = ";";
                    if (this.DZ.Text.Trim() == "")
                    {
                        dh = "";
                    }
                    this.DZ.Text = this.DZ.Text.Trim() + dh + wr;
                }
                if (bj == "3")
                {
                    string dh = ";";
                    if (this.NZ.Text.Trim() == "")
                    {
                        dh = "";
                    }
                    this.NZ.Text = this.NZ.Text.Trim() + dh + wr;
                }
                if (bj == "4")
                {
                    string dh = ";";
                    if (this.XZ.Text.Trim() == "")
                    {
                        dh = "";
                    }
                    this.XZ.Text = this.XZ.Text.Trim() + dh + wr;
                }
            }
        }

        // “向上箭头”按钮
        private void button8_Click(object sender, EventArgs e)
        {
            save();
            int n = 0;
            n = HZLB.SelectedIndex - 1;
            if (n < 0) { n = 0; }
            HZLB.SelectedIndex = n;
            FillData();
        }

        // “向下箭头”按钮
        private void button9_Click(object sender, EventArgs e)
        {
            save();
            int n = 0;
            n = HZLB.SelectedIndex + 1;
            if (n >= HZLB.Items.Count) { n = HZLB.Items.Count - 1; }
            HZLB.SelectedIndex = n;
            FillData();
        }

        // “删除宗地”按钮
        private void button3_Click(object sender, EventArgs e)
        {
            //save();
            #region 条件检查与准备
            if (this.HZLB.SelectedIndex == -1)
            {
                return;
            }
            ListBoxsx listBoxsx = (ListBoxsx)HZLB.Items[HZLB.SelectedIndex];
            string ndjh = listBoxsx.NHBH;
            if (ndjh != ATT.DJH)
            {
                MessageBox.Show("请双击此宗地以选中该宗地", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确实要删除此宗地吗?", "重要提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            int n = 0;
            n = HZLB.SelectedIndex;
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            string selstring = "SELECT * FROM DCB where DJH = '" + ndjh + "'";
            string selstring_ZDD = "SELECT  *  FROM D_ZDD where DJH = '" + ndjh + "'";
            string selstring_ZDX = "SELECT  *  FROM D_ZDX where DJH = '" + ndjh + "'";
            da_ZD.Clear();
            da_ZDD.Clear();
            da_ZDX.Clear();
            #endregion
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                #region 删除“DCB”表中数据
                DP = Tools.DataBasic.DCB_Initadapter(selstring, connectionString);
                DP.Fill(da_ZD, "DCB");//填充Dataset
                TB_DCB.Rows[0].Delete();
                DP.Update(da_ZD, "DCB");
                #endregion
                #region 删除“D_ZDD”表中数据
                DPD = Tools.DataBasic.D_ZDD_Initadapter(selstring_ZDD, connectionString);
                DPD.Fill(da_ZDD, "D_ZDD");//填充Dataset
                for (int i = 0; i < da_ZDD.Tables["D_ZDD"].Rows.Count; i++)
                {
                    da_ZDD.Tables["D_ZDD"].Rows[i].Delete();
                }
                DPD.Update(da_ZDD, "D_ZDD");
                #endregion
                #region 删除“D_ZDX”表中数据
                DPX = Tools.DataBasic.D_ZDX_Initadapter(selstring_ZDX, connectionString);
                DPX.Fill(da_ZDX, "D_ZDX");//填充Dataset
                for (int i = 0; i < da_ZDX.Tables["D_ZDX"].Rows.Count; i++)
                {
                    da_ZDX.Tables["D_ZDX"].Rows[i].Delete();
                }
                DPX.Update(da_ZDX, "D_ZDX");
                #endregion
                #region 刷新列表
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                string selstring_DCB = "SELECT * FROM DCB where DJH like '" + XZDM + "%%%%%'";
                SqlDataAdapter Adapter_TB = Tools.DataBasic.DCB_Initadapter(selstring_DCB, connectionString);
                DataSet dataset_DCB = new DataSet();
                Adapter_TB.Fill(dataset_DCB, "DCB");//填充Dataset
                AddHuzhulistbox(dataset_DCB);//刷新宗地列表框

                if (n >= HZLB.Items.Count) { n = HZLB.Items.Count - 1; }
                HZLB.SelectedIndex = n;
                FillData();
                #endregion
                connection.Close();
            }

        }

        // 生成单行文字
        /// <summary>
        /// 生成单行文字
        /// </summary>
        /// <param name="TEXT">注记内容</param>
        /// <param name="point">注记点</param>
        /// <param name="zg">字高</param>
        /// <param name="ys">颜色</param>
        /// <param name="spdqfx">水平对齐方向</param>
        /// <param name="czdqfx">垂直对齐方向</param>
        /// <param name="tc">图层</param>
        /// <param name="zt">字体</param>
        /// <param name="gkb">高宽比</param>
        /// <returns></returns>
        public DBText scText(string TEXT, Point3d point, double zg, Int32 ys, TextHorizontalMode spdqfx, TextVerticalMode czdqfx, string tc, ObjectId zt, double gkb, double xzjd)
        {
            DBText text = new DBText();
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    text.TextString = TEXT;
                    text.Oblique = 0;
                    text.Rotation = xzjd;
                    //text.Position = point;
                    text.Height = zg;
                    text.WidthFactor = gkb;
                    text.ColorIndex = ys;
                    text.HorizontalMode = spdqfx;//水平对齐方式
                    text.VerticalMode = czdqfx;
                    text.AlignmentPoint = point;
                    text.Layer = tc;
                    text.TextStyleId = zt;
                    btr.AppendEntity(text);
                    trans.AddNewlyCreatedDBObject(text, true);
                    trans.Commit();
                    trans.Dispose();
                }
            }
            return text;
        }

        // 生成多行文字
        /// <summary>
        /// 生成多行文字
        /// </summary>
        /// <param name="text">注记内容</param>
        /// <param name="point">注记点</param>
        /// <param name="zg">字高</param>
        /// <param name="ys">颜色</param>
        /// <param name="dqfs">对齐方式</param>
        /// <param name="tc">图层</param>
        /// <param name="zt">字体</param>
        /// <returns></returns>
        public MText scMtext(string text, Point3d point, double zg, Int32 ys, AttachmentPoint dqfs, string tc, ObjectId zt)
        {
            MText mtext = new MText();
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    mtext.Contents = text;
                    mtext.Location = point;
                    mtext.TextHeight = zg;
                    mtext.ColorIndex = ys;
                    mtext.Attachment = dqfs;
                    mtext.Layer = tc;
                    mtext.TextStyleId = zt;
                    btr.AppendEntity(mtext);
                    trans.AddNewlyCreatedDBObject(mtext, true);
                    trans.Commit();
                    trans.Dispose();
                }
            }
            return mtext;
        }

        // 生成界址点及消隐点
        /// <summary>
        /// 生成界址点及消隐点
        /// </summary>
        /// <param name="X">X坐标</param>
        /// <param name="Y">Y坐标</param>
        /// <param name="BJ">圆的半径</param>
        public void scjzd(Point3d poin_JZD, double BJ)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Document doc = AcadApplication.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    Circle circle = new Circle();
                    circle.Center = poin_JZD;
                    circle.Radius = BJ;
                    Tools.CADTools.AddNewLayer("JZD", "Continuous", 1);
                    circle.Layer = "JZD";
                    circle.ColorIndex = 1;
                    btr.AppendEntity(circle);
                    trans.AddNewlyCreatedDBObject(circle, true);
                    #region 生成界址点消隐
                    Point2dCollection points = new Point2dCollection(37);
                    for (int i = 0; i < 36; i++)
                    {
                        Point3d p1 = Tools.CADTools.GetNewPoint(poin_JZD, BJ, i * 10 * Math.PI / 180.0);
                        points.Add(new Point2d(p1.X, p1.Y));
                    }
                    Point3d p2 = Tools.CADTools.GetNewPoint(poin_JZD, BJ, 0);
                    points.Add(new Point2d(p2.X, p2.Y));
                    Wipeout wipeout = new Wipeout();
                    wipeout.SetDatabaseDefaults(db);
                    wipeout.SetFrom(points, new Vector3d(0.0, 0.0, 0.1));
                    btr.AppendEntity(wipeout);
                    trans.AddNewlyCreatedDBObject(wipeout, true);
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }

        // 打印预览宗地草图
        /// <summary>
        /// 打印预览宗地草图
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ZDT"></param>
        public void X_ZDCT(int i, bool ZDT)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 关闭原先打开的文档，创建宗地图路径

                if (ATT.Document_ZDT_SC != null && !ATT.Document_ZDT_SC.IsDisposed)
                {
                    ATT.Document_ZDT_SC.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
                }
                if (ATT.Document_ZDT_XG != null && !ATT.Document_ZDT_XG.IsDisposed)
                {
                    ATT.Document_ZDT_XG.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
                }
                if (ATT.Document_ZDT_LL != null && !ATT.Document_ZDT_LL.IsDisposed)
                {
                    ATT.Document_ZDT_LL.CloseAndDiscard();//不保存
                }
                if (ATT.Document_ZDT_DY != null && !ATT.Document_ZDT_DY.IsDisposed)
                {
                    ATT.Document_ZDT_DY.CloseAndDiscard();//不保存
                }
                double jft_blc = (double)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("LTSCALE");//街坊图比例尺

                string ZDT_Path1 = Tools.ReadWriteReg.read_reg("宗地草图存放路径");
                string ts = "“宗地草图存放路径”没有设置";

                DirectoryInfo mydir = new DirectoryInfo(ZDT_Path1);
                if (!mydir.Exists)
                {
                    MessageBox.Show(ts, "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                #endregion
                #region 打印宗地图
                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                string ZDT_Path3 = ZDT_Path1 + "\\" + XZDM.Substring(6, 3) + "\\" + XZDM.Substring(9, 3) + "\\";
                DirectoryInfo mydir3 = new DirectoryInfo(ZDT_Path3);
                string zdtfile3 = zdtfile3 = ZDT_Path3 + ATT.DJH + "-草图" + ".dwg";

                ATT.ZDT_File = zdtfile3;
                FileInfo fileInfo3 = new FileInfo(zdtfile3);
                if (fileInfo3.Exists)
                {
                    if (SCDY.Checked)
                    {
                        ATT.Document_ZDT_DY = AcadApplication.DocumentManager.Open(zdtfile3, true);//打开宗地图
                        ZDTZHCL zdtzhcl = new ZDTZHCL();//打印宗地图
                        if (DYJMC.SelectedIndex == -1 || A3DYZ.SelectedIndex == -1 || A4DYZ.SelectedIndex == -1)
                        {
                            MessageBox.Show("请选择打印机和打印纸！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        zdtzhcl.DY(DYJMC.SelectedItem.ToString(), A3DYZ.SelectedItem.ToString(), A4DYZ.SelectedItem.ToString());
                        ATT.Document_ZDT_DY.CloseAndDiscard();//关闭而不保存文档
                    }
                    else
                    {
                        ATT.Document_ZDT_LL = AcadApplication.DocumentManager.Open(zdtfile3, true);//打开宗地图
                    }
                }
                else
                {
                    MessageBox.Show("宗地草图没有生成！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                #endregion
            }
        }

        // 打印预览宗地图 
        /// <summary>
        /// 打印预览宗地图
        /// </summary>
        /// <param name="ZDT">True  宗地图 false 宗地草图</param>
        public void X_ZDT(int i, bool ZDT)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 关闭原先打开的文档，创建宗地图路径

                if (ATT.Document_ZDT_SC != null && !ATT.Document_ZDT_SC.IsDisposed)
                {
                    ATT.Document_ZDT_SC.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
                }
                if (ATT.Document_ZDT_XG != null && !ATT.Document_ZDT_XG.IsDisposed)
                {
                    ATT.Document_ZDT_XG.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
                }
                if (ATT.Document_ZDT_LL != null && !ATT.Document_ZDT_LL.IsDisposed)
                {
                    ATT.Document_ZDT_LL.CloseAndDiscard();//不保存
                }
                if (ATT.Document_ZDT_DY != null && !ATT.Document_ZDT_DY.IsDisposed)
                {
                    ATT.Document_ZDT_DY.CloseAndDiscard();//不保存
                }
                // double jft_blc = (double)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("LTSCALE");//街坊图比例尺

                string ZDT_Path1 = Tools.ReadWriteReg.read_reg("宗地图存放路径");
                string ts = "“宗地图存放路径”没有设置";

                DirectoryInfo mydir = new DirectoryInfo(ZDT_Path1);
                if (!mydir.Exists)
                {
                    MessageBox.Show(ts, "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                #endregion
                #region 打印宗地图
                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                string ZDT_Path3 = ZDT_Path1 + "\\" + XZDM.Substring(6, 3) + "\\" + XZDM.Substring(9, 3) + "\\";
                DirectoryInfo mydir3 = new DirectoryInfo(ZDT_Path3);
                string zdtfile3 = ZDT_Path3 + ATT.DJH + ".dwg";//宗地图文件名

                ATT.ZDT_File = zdtfile3;
                FileInfo fileInfo3 = new FileInfo(zdtfile3);
                if (fileInfo3.Exists)
                {
                    if (SCDY.Checked)
                    {
                        ATT.Document_ZDT_DY = AcadApplication.DocumentManager.Open(zdtfile3, true);//打开宗地图
                        ZDTZHCL zdtzhcl = new ZDTZHCL();//打印宗地图
                        if (DYJMC.SelectedIndex == -1 || A3DYZ.SelectedIndex == -1 || A4DYZ.SelectedIndex == -1)
                        {
                            MessageBox.Show("请选择打印机和打印纸！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        zdtzhcl.DY(DYJMC.SelectedItem.ToString(), A3DYZ.SelectedItem.ToString(), A4DYZ.SelectedItem.ToString());
                        ATT.Document_ZDT_DY.CloseAndDiscard();//关闭而不保存文档
                    }
                    else
                    {
                        ATT.Document_ZDT_LL = AcadApplication.DocumentManager.Open(zdtfile3, true);//打开宗地图
                    }
                }
                else
                {
                    MessageBox.Show("宗地图没有生成！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                #endregion
            }
        }

        // 打印
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="zdtfile3">文件名</param>
        public void dytx(string zdtfile3)
        {
            ATT.ZDT_File = zdtfile3;
            FileInfo fileInfo3 = new FileInfo(zdtfile3);
            if (fileInfo3.Exists)
            {
                if (SCDY.Checked)
                {
                    ATT.Document_ZDT_DY = AcadApplication.DocumentManager.Open(zdtfile3, true);//打开宗地图
                    ZDTZHCL zdtzhcl = new ZDTZHCL();//打印宗地图
                    if (DYJMC.SelectedIndex == -1 || A3DYZ.SelectedIndex == -1 || A4DYZ.SelectedIndex == -1)
                    {
                        MessageBox.Show("请选择打印机和打印纸！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    zdtzhcl.DY(DYJMC.SelectedItem.ToString(), A3DYZ.SelectedItem.ToString(), A4DYZ.SelectedItem.ToString());
                    ATT.Document_ZDT_DY.CloseAndDiscard();//关闭而不保存文档
                }
                else
                {
                    ATT.Document_ZDT_LL = AcadApplication.DocumentManager.Open(zdtfile3, true);//打开宗地图
                }
            }
            else
            {
                MessageBox.Show("宗地图没有生成！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }

        // 分层分户图
        public void X_FCFHT(int i, bool ZDT)
        {
            #region 关闭原先打开的文档，创建宗地图路径

            if (ATT.Document_ZDT_SC != null && !ATT.Document_ZDT_SC.IsDisposed)
            {
                ATT.Document_ZDT_SC.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
            }
            if (ATT.Document_ZDT_XG != null && !ATT.Document_ZDT_XG.IsDisposed)
            {
                ATT.Document_ZDT_XG.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
            }
            if (ATT.Document_ZDT_LL != null && !ATT.Document_ZDT_LL.IsDisposed)
            {
                ATT.Document_ZDT_LL.CloseAndDiscard();//不保存
            }
            if (ATT.Document_ZDT_DY != null && !ATT.Document_ZDT_DY.IsDisposed)
            {
                ATT.Document_ZDT_DY.CloseAndDiscard();//不保存
            }
            //double jft_blc = (double)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("LTSCALE");//街坊图比例尺

            string ZDT_Path1 = Tools.ReadWriteReg.read_reg("宗地图存放路径");
            string ts = "“宗地图存放路径”没有设置";

            DirectoryInfo mydir = new DirectoryInfo(ZDT_Path1);
            if (!mydir.Exists)
            {
                MessageBox.Show(ts, "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            #endregion
            #region 打印宗地图
            ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
            string XZDM = treeView1.SelectedNode.Tag.ToString();
            ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
            string ZDT_Path3 = ZDT_Path1 + "\\" + XZDM.Substring(6, 3) + "\\" + XZDM.Substring(9, 3) + "\\";
            DirectoryInfo mydir3 = new DirectoryInfo(ZDT_Path3);
            if (ZZDY.Checked)
            {
                FCFHLB fcfhlb;
                if (FCFHTLB.SelectedItems.Count >= 1)
                {
                    fcfhlb = (FCFHLB)FCFHTLB.SelectedItem;
                }
                else
                {
                    MessageBox.Show("请选择请选择图幅!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                string zdtfile3 = ZDT_Path3 + fcfhlb.tfm + ".dwg";//宗地图文件名
                dytx(zdtfile3);
            }
            else
            {
                List<FCFHLB> lb = HQFCFHTLB();
                for (int j = 0; j < lb.Count; j++)
                {
                    string zdtfile3 = ZDT_Path3 + lb[j].tfm + ".dwg";//宗地图文件名
                    dytx(zdtfile3);
                }
            }
            #endregion
        }

        // 使房屋线按顺时针方向
        /// <summary>
        /// 顺时针
        /// 使房屋线按顺时针方向
        /// </summary>
        /// <param name="DWX"></param>
        /// <returns></returns>
        public ACDBPolyline ssz(ACDBPolyline DWX)
        {
            #region 使房屋线按顺时针方向
            ArrayList zb = new ArrayList();
            double JD = 0;
            int DDS = DWX.NumberOfVertices;
            for (int j = 0; j < DDS; j++)
            {
                Point2d D0 = new Point2d();
                Point2d D1 = new Point2d();
                Point2d D2 = new Point2d();
                zb.Add(DWX.GetPoint2dAt(j));
                if (j == 0)
                {
                    D0 = DWX.GetPoint2dAt(DDS - 1);
                    D1 = DWX.GetPoint2dAt(0);
                    D2 = DWX.GetPoint2dAt(1);
                }
                if ((j >= 1) && (j < DDS - 1))
                {
                    D0 = DWX.GetPoint2dAt(j - 1);
                    D1 = DWX.GetPoint2dAt(j);
                    D2 = DWX.GetPoint2dAt(j + 1);
                }
                if (j == DDS - 1)
                {
                    D0 = DWX.GetPoint2dAt(j - 1);
                    D1 = DWX.GetPoint2dAt(j);
                    D2 = DWX.GetPoint2dAt(0);
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
                    DWX.SetPointAt(p, (Point2d)zb[DDS - p - 1]);
                }
            }
            return DWX;
            #endregion
        }

        // 图表输出 - 下面宗地图的“执行”按钮
        private void ZX_Click(object sender, EventArgs e)
        {
            #region 关闭原先打开的文档，创建宗地图路径

            if (ATT.Document_ZDT_SC != null && !ATT.Document_ZDT_SC.IsDisposed)
            {
                ATT.Document_ZDT_SC.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
            }
            if (ATT.Document_ZDT_XG != null && !ATT.Document_ZDT_XG.IsDisposed)
            {
                ATT.Document_ZDT_XG.CloseAndSave(ATT.ZDT_File);//保存且退出刚刚生成的宗地图文档
            }
            if (ATT.Document_ZDT_LL != null && !ATT.Document_ZDT_LL.IsDisposed)
            {
                ATT.Document_ZDT_LL.CloseAndDiscard();//不保存
            }
            if (ATT.Document_ZDT_DY != null && !ATT.Document_ZDT_DY.IsDisposed)
            {
                ATT.Document_ZDT_DY.CloseAndDiscard();//不保存
            }
            double jft_blc = (double)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("LTSCALE");//街坊图比例尺

            string ZDT_Path1 = Tools.ReadWriteReg.read_reg("宗地图存放路径");
            string ts = "“宗地图存放路径”没有设置";
            if (ZDCT.Checked)
            {
                ZDT_Path1 = Tools.ReadWriteReg.read_reg("宗地草图存放路径");
                ts = "“宗地草图存放路径”没有设置";
            }
            DirectoryInfo mydir = new DirectoryInfo(ZDT_Path1);
            if (!mydir.Exists)
            {
                MessageBox.Show(ts, "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            #endregion
            if (ATT.DJH == "")
            {
                MessageBox.Show(ts, "请选择宗地(双击)!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            #region 获取完整路径
            string ZDT_Path2 = ZDT_Path1 + "\\" + ATT.DJH.Substring(6, 3) + "\\" + ATT.DJH.Substring(9, 3) + "\\";
            DirectoryInfo mydir2 = new DirectoryInfo(ZDT_Path2);
            if (!mydir2.Exists)
            {
                mydir2.Create();
            }
            string zdtfile = ZDT_Path2 + ATT.DJH + ".dwg";//宗地图文件名
            if (ZDCT.Checked)
            {
                zdtfile = ZDT_Path2 + ATT.DJH + "-草图" + ".dwg";
            }
            ATT.ZDT_File = zdtfile;
            FileInfo fileInfo = new FileInfo(zdtfile);
            #endregion
            #region 生成宗地图
            if ((SCZDT.Checked && ZDT.Checked) || (SCZDT.Checked && ZDCT.Checked))//生成宗地图
            {
                if (fwsxjg.Count == 0)
                {
                    Write_FW_SX_to_LC();
                }
                #region 数据准备、将选中宗地范围写块到文件
                //ATT.Document_ZDT_SC.Dispose();
                System.Data.DataTable jzx_TB = da_ZDX.Tables["D_ZDD"];
                if (fileInfo.Exists)
                {
                    if (DialogResult.Yes != MessageBox.Show("该宗地图已经存在，是否重新生成?", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        return;
                    }
                }
                double A3H = 345;
                double A3W = 237;
                double A4H = 235;
                double A4W = 160;

                DataRow row = TB_DCB.Rows[0];
                double Xmin0 = (double)row["Xmin"];
                double Ymin0 = (double)row["Ymin"];
                double Xmax0 = (double)row["Xmax"];
                double Ymax0 = (double)row["Ymax"];
                double kzjl = Convert.ToDouble(JZXWKBL.Text) * (Xmax0 - Xmin0 + Ymax0 - Ymin0) / 2.0;//外扩距离
                double Xmin = Xmin0 - kzjl;//实际选择范围
                double Ymin = Ymin0 - kzjl;//实际选择范围
                double Xmax = Xmax0 + kzjl;//实际选择范围
                double Ymax = Ymax0 + kzjl;//实际选择范围
                Point2d cir = new Point2d((Xmin + Xmax) / 2.0, (Ymin + Ymax) / 2.0);
                Zoom(cir, (Ymax - Ymin) * 1.2, (Xmax - Xmin) * 1.2);
                if (row["WZDJH"].ToString().Trim() == "")
                {
                    if (MessageBox.Show("此宗地尚未录入调查信息，是否继续？", "重要提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
                {
                    Database db = HostApplicationServices.WorkingDatabase;
                    Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
                    TypedValue[] filList = new TypedValue[1];
                    filList[0] = new TypedValue((int)DxfCode.Start, "*");
                    SelectionFilter filter = new SelectionFilter(filList);
                    PromptSelectionResult res = ed.SelectCrossingWindow(new Point3d(Xmin, Ymin, 0), new Point3d(Xmax, Ymax, 0), filter);
                    SelectionSet SS = res.Value;
                    if (SS == null)
                    {
                        MessageBox.Show("没有找到图形，请打开街坊图", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }
                    ObjectId[] objectId = SS.GetObjectIds();
                    ObjectIdCollection objectIdCollection = new ObjectIdCollection();
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        objectIdCollection.Add(objectId[i]);
                    }
                    Database db3 = HostApplicationServices.WorkingDatabase.Wblock(objectIdCollection, new Point3d(0, 0, 0));//将选择集写入数据库
                    db3.SaveAs(zdtfile, DwgVersion.Current);//保存数据库到dwg文件

                }
                #endregion
                ATT.Document_ZDT_SC = AcadApplication.DocumentManager.Open(zdtfile, false);//打开宗地图
                using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
                {

                    #region 计算宗地比例尺、计算纸张使用方向
                    double H = Ymax - Ymin;
                    double W = Xmax - Xmin;
                    ATT.CEN_X = (Xmax + Xmin) / 2.0;
                    ATT.CEN_Y = (Ymax + Ymin) / 2.0;
                    if (ATT.JZDGS < Convert.ToInt32(this.A4BMJZDS.Text) && this.A3.Checked == false)
                    {
                        int blch1 = (int)(H * 1000 / (A4H - Convert.ToInt32(this.SXYLKJ.Text.Trim())));
                        blch1 = ((int)(1 + blch1 / Convert.ToInt32(this.TDJ.Text.Trim()))) * Convert.ToInt32(this.TDJ.Text.Trim());
                        int blch2 = (int)(W * 1000 / (A4W));
                        blch2 = ((int)(1 + blch2 / Convert.ToInt32(this.TDJ.Text.Trim()))) * Convert.ToInt32(this.TDJ.Text.Trim());
                        int blchS = blch1;
                        if (blch1 < blch2)
                        {
                            blchS = blch2;
                        }
                        blch1 = (int)(H * 1000 / (A4W - Convert.ToInt32(this.SXYLKJ.Text.Trim())));
                        blch1 = ((int)(1 + blch1 / Convert.ToInt32(this.TDJ.Text.Trim()))) * Convert.ToInt32(this.TDJ.Text.Trim());
                        blch2 = (int)(W * 1000 / (A4H));
                        blch2 = ((int)(1 + blch2 / Convert.ToInt32(this.TDJ.Text.Trim()))) * Convert.ToInt32(this.TDJ.Text.Trim());
                        int blchH = blch1;
                        if (blch1 < blch2)
                        {
                            blchH = blch2;
                        }
                        if (blchS > blchH)
                        {
                            ATT.BLCH = blchH;
                            ATT.A4A3_HS = "A4H";
                        }
                        else
                        {
                            ATT.BLCH = blchS;
                            ATT.A4A3_HS = "A4S";
                        }
                    }
                    else
                    {
                        int blch1 = (int)(H * 1000 / (A3H - Convert.ToInt32(this.SXYLKJ.Text.Trim())));
                        blch1 = ((int)(1 + blch1 / Convert.ToInt32(this.TDJ.Text.Trim()))) * Convert.ToInt32(this.TDJ.Text.Trim());
                        int blch2 = (int)(W * 1000 / (A3W));
                        blch2 = ((int)(1 + blch2 / Convert.ToInt32(this.TDJ.Text.Trim()))) * Convert.ToInt32(this.TDJ.Text.Trim());
                        int blchS = blch1;
                        if (blch1 < blch2)
                        {
                            blchS = blch2;
                        }
                        blch1 = (int)(H * 1000 / (A3W - Convert.ToInt32(this.SXYLKJ.Text.Trim())));
                        blch1 = ((int)(1 + blch1 / Convert.ToInt32(this.TDJ.Text.Trim()))) * Convert.ToInt32(this.TDJ.Text.Trim());
                        blch2 = (int)(W * 1000 / (A3H));
                        blch2 = ((int)(1 + blch2 / Convert.ToInt32(this.TDJ.Text.Trim()))) * Convert.ToInt32(this.TDJ.Text.Trim());
                        int blchH = blch1;
                        if (blch1 < blch2)
                        {
                            blchH = blch2;
                        }
                        if (blchS > blchH)
                        {
                            ATT.BLCH = blchH;
                            ATT.A4A3_HS = "A3H";
                        }
                        else
                        {
                            ATT.BLCH = blchS;
                            ATT.A4A3_HS = "A3S";
                        }
                    }

                    Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("LTSCALE", ATT.BLCH / 1000.0);
                    #endregion

                    #region 获取宗地内的所有对象
                    Database db = HostApplicationServices.WorkingDatabase;
                    Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
                    PromptSelectionResult res = ed.SelectAll();
                    SelectionSet SS = res.Value;
                    if (SS == null)
                    {
                        return;
                    }
                    ObjectId[] objectId = SS.GetObjectIds();
                    #endregion

                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        #region 接口
                        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                        SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                        #endregion
                        bool Isjzx = false;//是否存在本宗界址线
                        int wsxdx = 0;//无属性对象个数
                        for (int i = 0; i < objectId.Length; i++)
                        {
                            Entity entity = (Entity)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                            ResultBuffer resultBuffer = entity.XData;

                            #region 检查属性、删除无用对象
                            if (resultBuffer == null)//无属性
                            {
                                wsxdx++;
                                continue;
                            }
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                            if (tv3 == null)
                            {
                                wsxdx++;
                                continue;
                            }
                            if (tv3.Length <= 1)
                            {
                                wsxdx++;
                                continue;
                            }
                            string STDM = tv3[1].Value.ToString();
                            if (STDM == "302001") { entity.Erase(); continue; }//删除分子、分母线
                            if (STDM == "301000") { entity.Erase(); continue; }//删除所有界址点
                            if (STDM == "302020") { entity.Erase(); continue; }//删除所有界址点
                            if (STDM == "144301-1") { entity.Erase(); continue; }//删除所有围墙附属线
                            if (STDM == "144301-2") { entity.Erase(); continue; }//删除所有围墙短线
                            if (STDM == "202111") { entity.Erase(); continue; }//删除高程点注记
                            if (STDM == "202101") { entity.Erase(); continue; }//删除高程点
                            if (STDM == "202101") { entity.Erase(); continue; }//删除高程点
                            if (STDM == "171201") { entity.Erase(); continue; }//删除低压线骨干线
                            if (STDM == "171101") { entity.Erase(); continue; }//删除高压线骨干线
                            if (STDM == "172001") { entity.Erase(); continue; }//删除通信线骨干线
                            if (entity.GetType().Name.ToLower() == "dbtext" && entity.Layer.ToLower() == "jmd") { entity.Erase(); continue; }
                            if (entity.GetType().Name.ToLower() == "dbtext" && entity.Layer.ToLower() == "jzd") { entity.Erase(); continue; }
                            if (entity.GetType().Name.ToLower() == "dbpoint") { entity.Erase(); continue; }

                            #endregion

                            #region 查找本宗地界址线
                            if (STDM == "300000")
                            {

                                string djh = (string)tv3[2].Value.ToString();
                                djh = djh.Substring(0, 6) + djh.Substring(6, djh.Length - 6).PadLeft(5, '0');
                                //djh = djh.Substring(6, djh.Length - 6).PadLeft(5, '0');
                                if (ATT.DJH.Substring(6, 11) == djh)
                                {
                                    Tools.CADTools.AddNewLayer("本宗界址线", "Continuous", 1);
                                    entity.Layer = "本宗界址线";
                                    ACDBPolyline jzx = (ACDBPolyline)entity;
                                    if (Math.Abs(((double)row["ZDMJ"] - jzx.Area)) > 0.1)
                                    {
                                        MessageBox.Show("数据库宗地面积和图形宗地面积不一致！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        return;
                                    }
                                    ATT.JZDGS = jzx.NumberOfVertices;

                                    jzx.ConstantWidth = 0.3;
                                    Isjzx = true;

                                }
                                else
                                {
                                    Tools.CADTools.AddNewLayer("邻宗界址线", "Continuous", 1);
                                    ACDBPolyline jzx = (ACDBPolyline)entity;
                                    //jzx.ConstantWidth = jft_blc * 0.3;
                                    jzx.ConstantWidth = 0.3;
                                    entity.Layer = "邻宗界址线";
                                }
                            }
                            #endregion
                        }

                        #region 没有本宗界址线则退出
                        if (!Isjzx)
                        {
                            MessageBox.Show("此宗地没有界址线，也可能是街坊号和街道号的位数不正确。");
                            trans.Commit();
                            trans.Dispose();
                            return;
                        }
                        #endregion

                        #region 剪裁及注记
                        #region 创建范围线的面和线
                        ACDBPolyline fwx = new ACDBPolyline();//创建范围线
                        fwx.AddVertexAt(0, new Point2d(Xmin, Ymin), 0, 0, 0);
                        fwx.AddVertexAt(1, new Point2d(Xmin, Ymax), 0, 0, 0);
                        fwx.AddVertexAt(2, new Point2d(Xmax, Ymax), 0, 0, 0);
                        fwx.AddVertexAt(3, new Point2d(Xmax, Ymin), 0, 0, 0);
                        fwx.Closed = true;
                        Topology.Geometries.Polygon Polygon_fwx = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(fwx);//范围线面
                        Topology.Geometries.LineString LineString_fwx = (Topology.Geometries.LineString)Tools.CADTools.ConvertToLineString(fwx);//范围线线
                        #endregion
                        #region 再次获取宗地的所有对象、定义剪裁最小删除面积和最小删除长度
                        double zxscmj = 6.25 * ATT.BLCH / 1000.0;//最小删除面积
                        double zxsccd = 2.0 * ATT.BLCH / 1000.0;//最小删除长度
                        PromptSelectionResult res2 = ed.SelectAll();
                        SelectionSet SS2 = res2.Value;
                        if (SS2 == null)
                        {
                            return;
                        }
                        ObjectId[] objectId2 = SS2.GetObjectIds();
                        #endregion
                        ObjectId zt2 = Tools.CADTools.AddNewTextStyle("宋体", "宋体", 1, 0, false);
                        for (int i = 0; i < objectId2.Length; i++)
                        {
                            Entity entity = (Entity)trans.GetObject(objectId2[i], ACDBOpenMode.ForWrite);
                            #region 修改文字和图块
                            if (entity.GetType().Name.ToLower() == "dbtext")
                            {

                                DBText text = (DBText)entity;
                                Topology.Geometries.Point point = Tools.CADTools.ConvertToPoint(text.Position.X, text.Position.Y);
                                if (!Polygon_fwx.Intersects(point))
                                {
                                    entity.Erase();
                                    continue;
                                }
                                text.TextStyleId = Tools.CADTools.AddNewTextStyle("宋体", "宋体", 1, 0, false);
                                text.Height = ATT.BLCH * 2.0 / 1000.0;
                                text.WidthFactor = 1.0;
                                text.ColorIndex = 7;
                                continue;
                            }
                            if (entity.GetType().Name.ToLower() == "blockreference")
                            {
                                BlockReference block = (BlockReference)entity;
                                Topology.Geometries.Point point = Tools.CADTools.ConvertToPoint(block.Position.X, block.Position.Y);
                                if (!Polygon_fwx.Intersects(point))
                                {
                                    entity.Erase();
                                    continue;
                                }
                                if (!Polygon_fwx.Intersects(point))
                                {
                                    entity.Erase();
                                    continue;
                                }
                                block.ScaleFactors = new Scale3d(ATT.BLCH / (1000.0 * jft_blc), ATT.BLCH / (1000.0 * jft_blc), ATT.BLCH / (1000.0 * jft_blc));
                                continue;
                            }
                            #endregion
                            #region 属性检查
                            ResultBuffer resultBuffer = entity.XData;
                            if (resultBuffer == null)//无属性
                            {
                                continue;
                            }
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                            if (tv3 == null)
                            {
                                continue;
                            }
                            if (tv3.Length <= 1)
                            {
                                continue;
                            }
                            #endregion
                            if (entity.GetType().Name.ToLower() == "polyline")
                            {
                                ACDBPolyline pline = (ACDBPolyline)entity;
                                Topology.Geometries.LineString LineString_pline = (Topology.Geometries.LineString)Tools.CADTools.ConvertToLineString(pline);
                                #region 删除不在范围内的polyline线
                                if (!Polygon_fwx.Intersects(LineString_pline))//如果线不在范围类则删除
                                {
                                    entity.Erase();
                                    continue;
                                }
                                #endregion
                                if (pline.Closed)
                                {
                                    Topology.Geometries.Polygon Polygon_pline = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(pline);//宗地图上对象的面
                                    Point3d zjd = new Point3d(Polygon_pline.Centroid.X, Polygon_pline.Centroid.Y, 0);//注记点位置
                                    #region 找出被范围线裁切后剩下的区域的注记点位置
                                    double zdmj = -1;//最大面积 
                                    Topology.Geometries.Polygon zdm = null;//最大面积的块
                                    if (LineString_fwx.Intersects(Polygon_pline))//如果范围线和宗地图中的面相交
                                    {
                                        Topology.Geometries.Geometry geometry = (Topology.Geometries.Geometry)Polygon_fwx.Intersection(Polygon_pline);
                                        if (geometry.Area < zxscmj)
                                        {
                                            entity.Erase();
                                            continue;
                                        }
                                        for (int p = 0; p < geometry.NumGeometries; p++)
                                        {
                                            Topology.Geometries.Geometry geometry_n = (Topology.Geometries.Geometry)geometry.GetGeometryN(p);
                                            if (geometry_n.GeometryType.ToLower() == "polygon")
                                            {
                                                if (geometry_n.Area > zdmj)
                                                {
                                                    zdmj = geometry_n.Area;
                                                    zdm = (Topology.Geometries.Polygon)geometry_n;
                                                    //ACDBPolyline  xxx=  Tools.CADTools.WriteToDwg(zdm);
                                                    //btr.AppendEntity(xxx);
                                                    //trans.AddNewlyCreatedDBObject(xxx, true);
                                                }
                                            }
                                            if (zdm.Area < zxscmj)
                                            {
                                                entity.Erase();
                                                continue;
                                            }
                                            else
                                            {
                                                zjd = new Point3d(zdm.Centroid.X, zdm.Centroid.Y, 0);
                                            }
                                        }
                                    }
                                    #endregion
                                    #region 注记宗地内权利人、宗地号、用途
                                    ObjectId zt = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                                    if (pline.Layer == "本宗界址线")
                                    {
                                        string dlbm = row["DLBM"].ToString().Trim();//实际地类编码
                                        string qlr = row["QLR"].ToString().Trim();//权利人
                                        string zdh = row["WZDJH"].ToString().Trim().Substring(12, 7);//宗地号
                                        MText mtext = new MText();
                                        mtext.Contents = "\\A1;" + qlr + "{\\H0.7x\\S" + zdh + "/" + dlbm + ";}";
                                        mtext.TextHeight = 2.7 * ATT.BLCH / 1000.0;
                                        mtext.TextStyleId = zt;
                                        mtext.Location = zjd;
                                        mtext.ColorIndex = 1;
                                        mtext.Attachment = AttachmentPoint.MiddleCenter;
                                        mtext.LayerId = Tools.CADTools.AddNewLayer("本宗地号注记", "Continuous", 1);
                                        btr.AppendEntity(mtext);
                                        trans.AddNewlyCreatedDBObject(mtext, true);
                                    }
                                    if (pline.Layer == "邻宗界址线")
                                    {
                                        string XZDM2 = treeView1.SelectedNode.Tag.ToString();
                                        //string JDH2 = XZDM2.Substring(6, 3);
                                        //string JFH2 = XZDM2.Substring(9, 3);
                                        string XZQH2 = XZDM2.Substring(0, 6);
                                        string DM2 = tv3[2].Value.ToString().Trim();//界址中的宗地代码
                                        string ZDH2 = DM2.Substring(6);
                                        ZDH2 = ZDH2.PadLeft(5, '0');
                                        string DJH2 = XZQH2 + DM2.Substring(0, 6) + ZDH2;

                                        string qlr = "";//权利人
                                        string wzzdh = "";
                                        string dlbm = "";
                                        #region 查找邻权利人、完整地籍号、土地用途等
                                        string connectionString = Tools.Uitl.LJstring();
                                        using (SqlConnection Lzconn = new SqlConnection())
                                        {
                                            Lzconn.ConnectionString = connectionString;
                                            Lzconn.Open();
                                            if (Lzconn.State == ConnectionState.Broken)
                                            {
                                                MessageBox.Show("数据库连接失败！");
                                                return;
                                            }
                                            string Lzdjh = DJH2;
                                            if (Lzdjh != "")
                                            {
                                                string selconne = "SELECT * FROM DCB WHERE (DJH = '" + Lzdjh + "')";
                                                SqlCommand command = new SqlCommand(selconne, Lzconn);//连接数据库
                                                SqlDataReader reader = command.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    qlr = reader["QLR"].ToString().Trim();
                                                    wzzdh = reader["WZDJH"].ToString().Trim();
                                                    if (wzzdh != "")
                                                    {
                                                        wzzdh = reader["WZDJH"].ToString().Trim().Substring(12, 7);
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show(Lzdjh + "号宗地信息还没有录入。", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                                        reader.Close();
                                                        return;
                                                    }
                                                    dlbm = reader["DLBM"].ToString().Trim();
                                                }
                                                reader.Close();
                                            }
                                            Lzconn.Close();
                                        }
                                        #endregion
                                        string text = "\\A1;" + qlr + "{\\H0.7x\\S" + wzzdh + "/" + dlbm + ";}";
                                        Tools.CADTools.AddNewLayer("邻宗地号注记", "Continuous", 7);
                                        scMtext(text, zjd, 2.5 * ATT.BLCH / 1000.0, 7, AttachmentPoint.MiddleCenter,
                                           "邻宗地号注记", zt);

                                    }
                                    #endregion
                                    #region 注记房屋结构性质
                                    Tools.CADTools.AddNewLayer("房屋边长注记", "Continuous", 2);
                                    if (tv3[1].Value.ToString().Trim() == "FWSX")
                                    {
                                        FWSXJG fwsx = GetFWsx(pline);
                                        if (fwsx.建筑物类型0 == "房屋" || fwsx.建筑物类型0 == "门廊" || fwsx.建筑物类型0 == "车棚")
                                        {
                                            #region 注记房屋边长
                                            if (ZDT.Checked)
                                            {
                                                if (ZJFWBC.Checked && fwsx.地籍号1 == ATT.DJH.Substring(6, 11))
                                                {
                                                    for (int k = 0; k < pline.NumberOfVertices; k++)
                                                    {
                                                        int N = k + 1;
                                                        if (N == pline.NumberOfVertices)
                                                        {
                                                            N = 0;
                                                        }
                                                        Point3d point11 = pline.GetPoint3dAt(k);
                                                        Point3d point12 = pline.GetPoint3dAt(N);
                                                        Point3d zjd3 = new Point3d((point11.X + point12.X) / 2.0, (point11.Y + point12.Y) / 2.0, 0);
                                                        double jjdd = Tools.CADTools.GetAngle(point11, point12) + Math.PI * 1.0 / 2.0;
                                                        Point3d bczjd = Tools.CADTools.GetNewPoint(zjd3, 1.7 * ATT.BLCH / 1000.0, jjdd);
                                                        double jl2 = point11.DistanceTo(point12);
                                                        double wzjd = Tools.CADTools.GetAngle(point11, point12);//文字旋转角度
                                                        if (wzjd > Math.PI * 2.0)
                                                        {
                                                            wzjd = wzjd - Math.PI * 2.0;
                                                        }
                                                        if (wzjd < 0)
                                                        {
                                                            wzjd = wzjd + Math.PI * 2.0;
                                                        }
                                                        if ((wzjd >= Math.PI / 4.0 && wzjd < Math.PI * 3.0 / 4.0) || (wzjd >= Math.PI * 5.0 / 4.0 && wzjd < Math.PI * 7.0 / 4.0))
                                                        {
                                                            wzjd = wzjd + Math.PI;
                                                        }
                                                        if (wzjd > Math.PI * 2.0)
                                                        {
                                                            wzjd = wzjd - Math.PI * 2.0;
                                                        }
                                                        if (wzjd > Math.PI * 0.5 && wzjd < Math.PI * 3.0 / 2.0)
                                                        {
                                                            wzjd = wzjd + Math.PI;
                                                        }
                                                        scText(jl2.ToString("0.00"), bczjd, ATT.BLCH * 2.0 / 1000.0, 3, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "房屋边长注记", zt2, 0.8, wzjd);
                                                    }
                                                }
                                            }
                                            #endregion

                                            if (fwsx.所在层数12 == "1")
                                            {
                                                string fwjg = fwsx.房屋结构13;
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
                                                jhjg = jhjg + fwsx.总层数11;
                                                double jzzmj = 0;
                                                for (int p = 0; p < fwsxjg.Count; p++)
                                                {
                                                    if (fwsxjg[p].地籍号1 == fwsx.地籍号1 && fwsxjg[p].房屋幢号9 == fwsx.房屋幢号9)
                                                    {
                                                        jzzmj = jzzmj + fwsxjg[p].本层建筑面积29;
                                                    }
                                                }

                                                double jzzddmj = fwsx.本层建筑面积29;
                                                string cb = "";
                                                if (fwsx.房屋产别3 == "国有房产")
                                                {
                                                    cb = "1";
                                                }
                                                if (fwsx.房屋产别3 == "集体所有房产")
                                                {
                                                    cb = "2";
                                                }
                                                if (fwsx.房屋产别3 == "私有房产")
                                                {
                                                    cb = "3";
                                                }
                                                if (fwsx.房屋产别3 == "联营企业房产")
                                                {
                                                    cb = "4";
                                                }
                                                if (fwsx.房屋产别3 == "股份制企业房产")
                                                {
                                                    cb = "5";
                                                }
                                                if (fwsx.房屋产别3 == "港、澳、台投资房产")
                                                {
                                                    cb = "6";
                                                }
                                                if (fwsx.房屋产别3 == "涉外房产")
                                                {
                                                    cb = "7";
                                                }
                                                if (fwsx.房屋产别3 == "其他房产")
                                                {
                                                    cb = "8";
                                                }

                                                string[] yt ={"成套住宅","非成套住宅","集体宿舍","工业","公用设施",
                                                                    "铁路","民航","航运","公交运输","仓储","商业服务",
                                                                    "经营","旅游","金融保险","电讯信息","教育","医疗卫生",
                                                                     "科研","文化","新闻","娱乐","园林绿化","体育","办公",
                                                                     "军事","涉外","宗教","监狱"};
                                                string[] ytdm ={"11","12","13","21","22","23","24","25","26","27",
                                                                    "31","32","33","34","35","41","42","43","51","52","53",
                                                                    "54","55","61","71","81","82","83"};
                                                string nyt = "";
                                                for (int k = 0; k < yt.Length; k++)
                                                {
                                                    if (yt[k] == fwsx.房屋用途4)
                                                    {
                                                        nyt = ytdm[k];
                                                    }
                                                }
                                                string fz = jhjg;
                                                string fm = cb + "\\/" + nyt;
                                                if (ZDT.Checked)
                                                {
                                                    fz += "\\/" + jzzmj.ToString("0.00");
                                                    fm += "\\/" + fwsx.本层建筑面积29.ToString("0.00");
                                                }

                                                MText mtext = new MText();
                                                if (fwsx.地籍号1 == ATT.DJH.Substring(6, 11))
                                                {
                                                    mtext.Contents = "\\A1;" + "{\\H0.7x;\\S" + fz + "/" + fm + ";}";
                                                    // mtext.Contents = "DFD";
                                                }
                                                else
                                                {
                                                    mtext.Contents = jhjg;
                                                }
                                                mtext.TextHeight = ATT.BLCH * 2.7 / 1000.0;
                                                ObjectId ZT3 = Tools.CADTools.AddNewTextStyle("宋体", "宋体", 1, 0, false);
                                                mtext.TextStyleId = ZT3;
                                                mtext.Location = zjd;
                                                mtext.ColorIndex = 6;
                                                mtext.Attachment = AttachmentPoint.MiddleCenter;
                                                mtext.LayerId = Tools.CADTools.AddNewLayer("房屋属性注记", "Continuous", 6);
                                                btr.AppendEntity(mtext);
                                                trans.AddNewlyCreatedDBObject(mtext, true);
                                                double minxy = 999999999999;
                                                Point2d xnjd = new Point2d();
                                                if (fwsx.地籍号1 == ATT.DJH.Substring(6, 11))
                                                {
                                                    for (int k = 0; k < pline.NumberOfVertices; k++)
                                                    {
                                                        if (minxy > pline.GetPoint2dAt(k).X + pline.GetPoint2dAt(k).Y)
                                                        {
                                                            minxy = pline.GetPoint2dAt(k).X + pline.GetPoint2dAt(k).Y;
                                                            xnjd = pline.GetPoint2dAt(k);
                                                        }

                                                    }
                                                    //Tools.CADTools.AddNewLayer("幢号注记", "Continuous", 6);
                                                    //scText("(" + fwsx.房屋幢号9 + ")", new Point3d((xnjd.X + zjd.X) / 12.0, (xnjd.Y + zjd.Y) / 2.0, 0), ATT.BLCH * 2.0 / 1000.0, 2, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "幢号注记", ZT3, 0.8, 0);//注记幢号
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                }
                                #region 剪裁、修改线宽、切换围墙

                                if (LineString_fwx.Intersects(LineString_pline))
                                {
                                    Topology.Geometries.Geometry geometry = (Topology.Geometries.Geometry)Polygon_fwx.Intersection(LineString_pline);
                                    for (int p = 0; p < geometry.NumGeometries; p++)
                                    {
                                        Topology.Geometries.Geometry geometry_n = (Topology.Geometries.Geometry)geometry.GetGeometryN(p);
                                        if (geometry_n.GeometryType.ToLower() == "linestring")
                                        {
                                            if (geometry_n.Length < zxsccd)//如果面积或长度过小则不增加
                                            {
                                                continue;
                                            }
                                            ACDBPolyline pline_n = Tools.CADTools.WriteToDwg((Topology.Geometries.LineString)geometry_n);
                                            pline_n.Layer = pline.Layer;
                                            pline_n.LinetypeId = pline.LinetypeId;
                                            pline_n.Color = pline.Color;
                                            pline_n.ConstantWidth = pline.GetStartWidthAt(0) * ATT.BLCH / (1000.0 * jft_blc);
                                            btr.AppendEntity(pline_n);
                                            trans.AddNewlyCreatedDBObject(pline_n, true);
                                            #region 如果是围墙，切换围墙
                                            if (tv3[1].Value.ToString() == "144301" || tv3[1].Value.ToString() == "144302")//如果是围墙，切换围墙
                                            {
                                                Tools.CADTools.AddNewLinetype("WQ", Tools.CADTools.GetAppPath() + "ACADISO.LIN");
                                                pline_n.Linetype = "WQ";
                                                pline_n.ConstantWidth = 0;
                                                DBObjectCollection wqs = pline_n.GetOffsetCurves(0.5 * ATT.BLCH / (-1000.0));
                                                for (int nn = 0; nn < wqs.Count; nn++)
                                                {
                                                    if (wqs[nn].GetType().Name.ToLower() == "polyline")
                                                    {
                                                        ACDBPolyline wq = (ACDBPolyline)wqs[nn];
                                                        wq.LayerId = Tools.CADTools.AddNewLayer("围墙辅助线", "Continuous", 7);
                                                        wq.Linetype = "Continuous";
                                                        wq.ColorIndex = 7;
                                                        wq.ConstantWidth = 0;
                                                        btr.AppendEntity(wq);
                                                        trans.AddNewlyCreatedDBObject(wq, true);
                                                    }
                                                }

                                            }
                                            #endregion
                                        }
                                    }
                                    pline.Erase();
                                    continue;
                                }
                                else
                                {
                                    pline.ConstantWidth = pline.GetStartWidthAt(0) * ATT.BLCH / (1000.0 * jft_blc);
                                    #region 如果是围墙，切换围墙
                                    if (tv3[1].Value.ToString() == "144301" || tv3[1].Value.ToString() == "144302")//如果是围墙，切换围墙
                                    {
                                        Tools.CADTools.AddNewLinetype("WQ", Tools.CADTools.GetAppPath() + "ACADISO.LIN");
                                        pline.Linetype = "WQ";
                                        pline.ConstantWidth = 0;
                                        DBObjectCollection wqs = pline.GetOffsetCurves(0.5 * ATT.BLCH / (-1000.0));
                                        for (int nn = 0; nn < wqs.Count; nn++)
                                        {
                                            if (wqs[nn].GetType().Name.ToLower() == "polyline")
                                            {
                                                ACDBPolyline wq = (ACDBPolyline)wqs[nn];
                                                wq.LayerId = Tools.CADTools.AddNewLayer("围墙辅助线", "Continuous", 7);
                                                wq.Linetype = "Continuous";
                                                wq.ColorIndex = 7;
                                                wq.ConstantWidth = 0;
                                                btr.AppendEntity(wq);
                                                trans.AddNewlyCreatedDBObject(wq, true);
                                            }
                                        }

                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #region 剪裁直线
                            if (entity.GetType().Name.ToLower() == "line")
                            {
                                Line line = (Line)entity;
                                Topology.Geometries.LineString LineString_pline = (Topology.Geometries.LineString)Tools.CADTools.ConvertToLineString(line);
                                if (!Polygon_fwx.Intersects(LineString_pline))//如果线不在范围类则删除
                                {
                                    entity.Erase();
                                    continue;
                                }
                                if (LineString_fwx.Intersects(LineString_pline))
                                {
                                    Topology.Geometries.Geometry geometry = (Topology.Geometries.Geometry)Polygon_fwx.Intersection(LineString_pline);
                                    for (int p = 0; p < geometry.NumGeometries; p++)
                                    {
                                        Topology.Geometries.Geometry geometry_n = (Topology.Geometries.Geometry)geometry.GetGeometryN(p);
                                        if (geometry_n.GeometryType.ToLower() == "linestring")
                                        {
                                            if (geometry_n.Length < zxsccd)//如果面积或长度过小则不增加
                                            {
                                                continue;
                                            }
                                            ACDBPolyline pline_n = Tools.CADTools.WriteToDwg((Topology.Geometries.LineString)geometry_n);
                                            pline_n.Layer = line.Layer;
                                            pline_n.LinetypeId = line.LinetypeId;
                                            pline_n.Color = line.Color;
                                            pline_n.ConstantWidth = 0;
                                            btr.AppendEntity(pline_n);
                                            trans.AddNewlyCreatedDBObject(pline_n, true);
                                        }
                                    }
                                    entity.Erase();
                                    continue;
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region 绘制外图廓线
                        double[] D1 = new double[2];
                        double[] D2 = new double[2];
                        double[] D3 = new double[2];
                        double[] D4 = new double[2];
                        double Xmin_dy = 0;
                        double Ymin_dy = 0;
                        double Xmax_dy = 0;
                        double Ymax_dy = 0;
                        if (ATT.A4A3_HS == "A4S")
                        {
                            D1[0] = ATT.CEN_X - A4W * ATT.BLCH / 2000.0;
                            D1[1] = ATT.CEN_Y - A4H * ATT.BLCH / 2000.0;
                            D2[0] = ATT.CEN_X - A4W * ATT.BLCH / 2000.0;
                            D2[1] = ATT.CEN_Y + A4H * ATT.BLCH / 2000.0;
                            D3[0] = ATT.CEN_X + A4W * ATT.BLCH / 2000.0;
                            D3[1] = ATT.CEN_Y + A4H * ATT.BLCH / 2000.0;
                            D4[0] = ATT.CEN_X + A4W * ATT.BLCH / 2000.0;
                            D4[1] = ATT.CEN_Y - A4H * ATT.BLCH / 2000.0;
                        }
                        if (ATT.A4A3_HS == "A4H")
                        {
                            D1[0] = ATT.CEN_X - A4H * ATT.BLCH / 2000.0;
                            D1[1] = ATT.CEN_Y - A4W * ATT.BLCH / 2000.0;
                            D2[0] = ATT.CEN_X - A4H * ATT.BLCH / 2000.0;
                            D2[1] = ATT.CEN_Y + A4W * ATT.BLCH / 2000.0;
                            D3[0] = ATT.CEN_X + A4H * ATT.BLCH / 2000.0;
                            D3[1] = ATT.CEN_Y + A4W * ATT.BLCH / 2000.0;
                            D4[0] = ATT.CEN_X + A4H * ATT.BLCH / 2000.0;
                            D4[1] = ATT.CEN_Y - A4W * ATT.BLCH / 2000.0;
                        }
                        if (ATT.A4A3_HS == "A3S")
                        {
                            D1[0] = ATT.CEN_X - A3W * ATT.BLCH / 2000.0;
                            D1[1] = ATT.CEN_Y - A3H * ATT.BLCH / 2000.0;
                            D2[0] = ATT.CEN_X - A3W * ATT.BLCH / 2000.0;
                            D2[1] = ATT.CEN_Y + A3H * ATT.BLCH / 2000.0;
                            D3[0] = ATT.CEN_X + A3W * ATT.BLCH / 2000.0;
                            D3[1] = ATT.CEN_Y + A3H * ATT.BLCH / 2000.0;
                            D4[0] = ATT.CEN_X + A3W * ATT.BLCH / 2000.0;
                            D4[1] = ATT.CEN_Y - A3H * ATT.BLCH / 2000.0;
                        }
                        if (ATT.A4A3_HS == "A3H")
                        {
                            D1[0] = ATT.CEN_X - A3H * ATT.BLCH / 2000.0;
                            D1[1] = ATT.CEN_Y - A3W * ATT.BLCH / 2000.0;
                            D2[0] = ATT.CEN_X - A3H * ATT.BLCH / 2000.0;
                            D2[1] = ATT.CEN_Y + A3W * ATT.BLCH / 2000.0;
                            D3[0] = ATT.CEN_X + A3H * ATT.BLCH / 2000.0;
                            D3[1] = ATT.CEN_Y + A3W * ATT.BLCH / 2000.0;
                            D4[0] = ATT.CEN_X + A3H * ATT.BLCH / 2000.0;
                            D4[1] = ATT.CEN_Y - A3W * ATT.BLCH / 2000.0;
                        }
                        Xmin_dy = D1[0];
                        Ymin_dy = D1[1];
                        Xmax_dy = D3[0];
                        Ymax_dy = D3[1];

                        ACDBPolyline tkx = new ACDBPolyline();
                        tkx.AddVertexAt(0, new Point2d(D1), 0, 0, 0);
                        tkx.AddVertexAt(1, new Point2d(D2), 0, 0, 0);
                        tkx.AddVertexAt(2, new Point2d(D3), 0, 0, 0);
                        tkx.AddVertexAt(3, new Point2d(D4), 0, 0, 0);
                        tkx.Closed = true;
                        Tools.CADTools.AddNewLayer("图廓线", "Continuous", 7);
                        tkx.Layer = "图廓线";
                        tkx.ColorIndex = 7;
                        tkx.Linetype = "Continuous";
                        btr.AppendEntity(tkx);
                        trans.AddNewlyCreatedDBObject(tkx, true);

                        double[] Dpoint1 = new double[3];
                        double[] Dpoint2 = new double[3];
                        Dpoint1[0] = D1[0];
                        Dpoint1[1] = D2[1] - ATT.BLCH * 20.0 / 1000.0;
                        Dpoint2[0] = D3[0];
                        Dpoint2[1] = Dpoint1[1];

                        if (ZDT.Checked)
                        {
                            Tools.CADTools.AddNewLayer("图廓线2", "Continuous", 7);
                            ACDBPolyline tkx2 = new ACDBPolyline();
                            tkx2.AddVertexAt(0, new Point2d(Dpoint1), 0, 0, 0);
                            tkx2.AddVertexAt(1, new Point2d(Dpoint2), 0, 0, 0);
                            //tkx2.Closed = true;
                            Tools.CADTools.AddNewLayer("图廓线2", "Continuous", 7);
                            tkx2.Layer = "图廓线2";
                            tkx2.ColorIndex = 7;
                            tkx2.Linetype = "Continuous";
                            btr.AppendEntity(tkx2);
                            trans.AddNewlyCreatedDBObject(tkx2, true);
                        }
                        #endregion

                        #region 绘制图廓整饰
                        Point2d point1 = new Point2d(D1);//左下
                        Point2d point2 = new Point2d(D2);//左上
                        Point2d point3 = new Point2d(D3);//右上
                        Point2d point4 = new Point2d(D4);//右下

                        double[] Dtext = new double[3];
                        Dtext[0] = (D2[0] + D3[0]) / 2.0;
                        Dtext[1] = D2[1] + ATT.BLCH * 5.5 / 1000.0;
                        if (ZDT.Checked)
                        {
                            MText mtext5 = new MText();
                            mtext5.Contents = "宗 地 图";
                            mtext5.TextHeight = ATT.BLCH * 4.5 / 1000.0;
                            mtext5.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext5.Location = new Point3d(Dtext);
                            mtext5.ColorIndex = 7;
                            mtext5.Attachment = AttachmentPoint.MiddleCenter;
                            mtext5.LayerId = Tools.CADTools.AddNewLayer("宗地图注记", "Continuous", 7);
                            btr.AppendEntity(mtext5);
                            trans.AddNewlyCreatedDBObject(mtext5, true);
                            Dtext[0] = D3[0];
                            Dtext[1] = D2[1] + ATT.BLCH * 4.0 / 1000.0;
                            MText mtext6 = new MText();
                            mtext6.Contents = "单位:m";
                            mtext6.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                            mtext6.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext6.Location = new Point3d(Dtext);
                            mtext6.ColorIndex = 7;
                            mtext6.Attachment = AttachmentPoint.MiddleRight;
                            mtext6.LayerId = Tools.CADTools.AddNewLayer("单位注记", "Continuous", 7);
                            btr.AppendEntity(mtext6);
                            trans.AddNewlyCreatedDBObject(mtext6, true);


                            Dtext[0] = D2[0] + ATT.BLCH * 10.0 / 1000.0;
                            Dtext[1] = D2[1] - ATT.BLCH * 10.0 / 1000.0;
                            MText mtext7 = new MText();
                            string zj1 = "宗地代码:" + row["WZDJH"].ToString().Trim() + "\r\n";
                            zj1 = zj1 + "所在图幅号:" + row["TFH"].ToString().Trim() + "\r\n";
                            double jzzmj = (double)row["JZMJ"];
                            zj1 = zj1 + "建筑总面积：" + jzzmj.ToString("0.00");
                            mtext7.Contents = zj1;
                            mtext7.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                            mtext7.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext7.Location = new Point3d(Dtext);
                            mtext7.ColorIndex = 7;
                            mtext7.Attachment = AttachmentPoint.MiddleLeft;
                            mtext7.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                            btr.AppendEntity(mtext7);
                            trans.AddNewlyCreatedDBObject(mtext7, true);

                            Dtext[0] = (D2[0] + D3[0]) / 2.0 + ATT.BLCH * 10.0 / 1000.0;
                            Dtext[1] = D2[1] - ATT.BLCH * 10 / 1000.0;
                            MText mtext8 = new MText();
                            string zj2 = "土地权利人:" + row["QLR"].ToString().Trim() + "\r\n";

                            double zdmj2 = Convert.ToDouble(ZDMJ.Text.Trim());
                            double pzmj = Convert.ToDouble(FZMJ.Text.Trim());
                            //if (pzmj < 0.00001)
                            //{
                            //    MessageBox.Show("发证面积没有填写", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //}
                            //zj2 = zj2 + "宗地面积:" + zdmj2.ToString("0.00") + "  " + "发证面积:" + pzmj.ToString("0.00") + "\r\n";
                            switch (CQ)
                            {
                                case "阜阳":
                                    if (pzmj < 0.00001)
                                    {
                                        MessageBox.Show("发证面积没有填写", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    zj2 = zj2 + "宗地面积:" + zdmj2.ToString("0.00") + "  " + "发证面积:" + pzmj.ToString("0.00") + "\r\n";
                                    break;
                                case "默认":
                                case "金寨":
                                case "芜湖":
                                case "宣城":
                                default:
                                    zj2 = zj2 + "宗地面积:" + zdmj2.ToString("0.00") + "\r\n";
                                    break;
                            }


                            double jzzdmj = (double)row["JZZDMJ"];
                            zj2 = zj2 + "建筑占地总面积：" + jzzdmj.ToString("0.00");
                            mtext8.Contents = zj2;
                            mtext8.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                            mtext8.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext8.Location = new Point3d(Dtext);
                            mtext8.ColorIndex = 7;
                            mtext8.Attachment = AttachmentPoint.MiddleLeft;
                            mtext8.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                            btr.AppendEntity(mtext8);
                            trans.AddNewlyCreatedDBObject(mtext8, true);



                            Dtext[0] = D1[0];
                            Dtext[1] = D1[1] - ATT.BLCH * 2.5 / 1000.0;
                            MText mtext11 = new MText();
                            string zj4 = "";
                            if (CLRQ.Text.ToString().Trim() != "")
                            {
                                zj4 = CLRQ.Text.ToString().Trim() + "解析法测绘界址点" + "\r\n";
                            }
                            else
                            {
                                zj4 = "    年  月  日" + "图解法测绘界址点" + "\r\n";
                            }
                            zj4 = zj4 + "制图日期:" + SCRQ.Text.ToString().Trim() + "\r\n";
                            zj4 = zj4 + "审核日期:" + SHRQ.Text.ToString().Trim() + "\r\n";
                            mtext11.Contents = zj4;
                            mtext11.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                            mtext11.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext11.Location = new Point3d(Dtext);
                            mtext11.ColorIndex = 7;
                            mtext11.Attachment = AttachmentPoint.TopLeft;
                            mtext11.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                            btr.AppendEntity(mtext11);
                            trans.AddNewlyCreatedDBObject(mtext11, true);


                            Dtext[0] = (D2[0] + D3[0]) / 2.0;
                            Dtext[1] = D1[1] - ATT.BLCH * 4.0 / 1000.0;
                            MText mtext10 = new MText();
                            mtext10.Contents = "1:" + ATT.BLCH.ToString();
                            mtext10.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                            mtext10.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext10.Location = new Point3d(Dtext);
                            mtext10.ColorIndex = 7;
                            mtext10.Attachment = AttachmentPoint.MiddleCenter;
                            mtext10.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                            btr.AppendEntity(mtext10);
                            trans.AddNewlyCreatedDBObject(mtext10, true);

                            Dtext[0] = D4[0] - ATT.BLCH * 20 / 1000.0;
                            Dtext[1] = D4[1] - ATT.BLCH * 2.5 / 1000.0;
                            MText mtext12 = new MText();
                            string zj5 = "制图者:" + HTY.Text.Trim() + "\r\n";
                            zj5 = zj5 + "审核者:" + SHRXM.Text.Trim();
                            mtext12.Contents = zj5;
                            mtext12.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                            mtext12.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext12.Location = new Point3d(Dtext);
                            mtext12.ColorIndex = 7;
                            mtext12.Attachment = AttachmentPoint.TopLeft;
                            mtext12.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                            btr.AppendEntity(mtext12);
                            trans.AddNewlyCreatedDBObject(mtext12, true);
                        }
                        if (ZDCT.Checked)
                        {
                            Dtext[0] = (D2[0] + D3[0]) / 2.0;
                            Dtext[1] = D2[1] - ATT.BLCH * 12.0 / 1000.0;
                            MText mtext51 = new MText();
                            mtext51.Contents = "宗地草图";
                            mtext51.TextHeight = ATT.BLCH * 4.5 / 1000.0;
                            mtext51.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext51.Location = new Point3d(Dtext);
                            mtext51.ColorIndex = 7;
                            mtext51.Attachment = AttachmentPoint.MiddleCenter;
                            mtext51.LayerId = Tools.CADTools.AddNewLayer("宗地草图注记", "Continuous", 7);
                            btr.AppendEntity(mtext51);
                            trans.AddNewlyCreatedDBObject(mtext51, true);

                            Dtext[0] = D1[0];
                            Dtext[1] = D1[1] - ATT.BLCH * 2.5 / 1000.0;
                            MText mtext22 = new MText();
                            string zj5 = "丈量者:" + DCYXM.Text.Trim().PadRight(6, ' ') + "\r\n";
                            zj5 = zj5 + "检查者:" + SHRXM.Text.Trim().PadRight(6, ' ');
                            mtext22.Contents = zj5;
                            mtext22.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                            mtext22.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext22.Location = new Point3d(Dtext);
                            mtext22.ColorIndex = 7;
                            mtext22.Attachment = AttachmentPoint.TopLeft;
                            mtext22.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                            btr.AppendEntity(mtext22);
                            trans.AddNewlyCreatedDBObject(mtext22, true);

                            Dtext[0] = D4[0] - ATT.BLCH * 20 / 1000.0;
                            Dtext[1] = D4[1] - ATT.BLCH * 2.5 / 1000.0;
                            MText mtext42 = new MText();
                            zj5 = "丈量日期:" + DCRQ.Text.PadRight(14, ' ') + "\r\n";
                            zj5 = zj5 + "检查日期:" + SHRQ.Text.PadRight(14, ' ');
                            mtext42.Contents = zj5;
                            mtext42.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                            mtext42.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext42.Location = new Point3d(Dtext);
                            mtext42.ColorIndex = 7;
                            mtext42.Attachment = AttachmentPoint.TopRight;
                            mtext42.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                            btr.AppendEntity(mtext42);
                            trans.AddNewlyCreatedDBObject(mtext42, true);

                            Dtext[0] = (D2[0] + D3[0]) / 2.0;
                            Dtext[1] = D1[1] - ATT.BLCH * 3.5 / 1000.0;
                            MText mtext10 = new MText();
                            mtext10.Contents = "概略比例尺  1:" + ATT.BLCH.ToString();
                            mtext10.TextHeight = ATT.BLCH * 2.5 / 1000.0;
                            mtext10.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                            mtext10.Location = new Point3d(Dtext);
                            mtext10.ColorIndex = 7;
                            mtext10.Attachment = AttachmentPoint.MiddleCenter;
                            mtext10.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                            btr.AppendEntity(mtext10);
                            trans.AddNewlyCreatedDBObject(mtext10, true);
                        }
                        #endregion

                        #region 指北针
                        ACDBPolyline zbz = new ACDBPolyline();

                        Point2d p1 = new Point2d(D3[0] - ATT.BLCH * 10.6966 / 1000.0, D3[1] - ATT.BLCH * 5 / 1000.0 - ATT.BLCH * 38.4294 / 1000.0);
                        Point2d p2 = new Point2d(D3[0] - ATT.BLCH * 7.4491 / 1000.0, D3[1] - ATT.BLCH * 5 / 1000.0 - ATT.BLCH * 23.9406 / 1000.0);
                        Point2d p3 = new Point2d(D3[0] - ATT.BLCH * 4.2016 / 1000.0, D3[1] - ATT.BLCH * 5 / 1000.0 - ATT.BLCH * 38.4294 / 1000.0);
                        Point2d p4 = new Point2d(D3[0] - ATT.BLCH * 7.4491 / 1000.0, D3[1] - ATT.BLCH * 5 / 1000.0 - ATT.BLCH * 33.0398 / 1000.0);
                        zbz.AddVertexAt(0, p1, 0, 0, 0);
                        zbz.AddVertexAt(1, p2, 0, 0, 0);
                        zbz.AddVertexAt(2, p3, 0, 0, 0);
                        zbz.AddVertexAt(3, p4, 0, 0, 0);
                        zbz.Closed = true;
                        Tools.CADTools.AddNewLayer("指北针", "Continuous", 7);
                        zbz.Layer = "指北针";
                        zbz.ColorIndex = 7;
                        zbz.Linetype = "Continuous";
                        btr.AppendEntity(zbz);
                        trans.AddNewlyCreatedDBObject(zbz, true);
                        //////对本宗地块进行图案填充
                        Hatch hatch = new Hatch();
                        Vector3d vector3d = new Vector3d(0, 0, 1);
                        hatch.Normal = vector3d;
                        hatch.PatternScale = 1;
                        hatch.LayerId = Tools.CADTools.AddNewLayer("图案填充", "Continuous", 7);
                        hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                        btr.AppendEntity(hatch);
                        trans.AddNewlyCreatedDBObject(hatch, true);
                        hatch.Associative = true;
                        ObjectIdCollection objectIdCollection = new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();
                        objectIdCollection.Add(zbz.ObjectId);
                        hatch.AppendLoop(HatchLoopTypes.External, objectIdCollection);
                        hatch.EvaluateHatch(true);

                        Dtext[0] = D3[0] - ATT.BLCH * 7.2545 / 1000.0;
                        Dtext[1] = D3[1] - ATT.BLCH * 21.4806 / 1000.0 - ATT.BLCH * 5 / 1000.0;
                        MText mtext15 = new MText();
                        mtext15.Contents = "北";
                        mtext15.TextHeight = ATT.BLCH * 2.7 / 1000.0;
                        mtext15.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext15.Location = new Point3d(Dtext);
                        mtext15.ColorIndex = 7;
                        mtext15.Attachment = AttachmentPoint.MiddleCenter;
                        mtext15.LayerId = Tools.CADTools.AddNewLayer("指北针", "Continuous", 7);
                        btr.AppendEntity(mtext15);
                        trans.AddNewlyCreatedDBObject(mtext15, true);
                        //////对本宗地块进行图案填充
                        #endregion

                        #region 标注界址点
                        System.Data.DataTable TB_ZDD = da_ZDD.Tables["D_ZDD"];
                        //ObjectId zt2 = Tools.CADTools.AddNewTextStyle("宋体", "宋体", 1, 0, false);
                        for (int J = 0; J < TB_ZDD.Rows.Count - 1; J++)
                        {
                            int N = J - 1;
                            if (N < 0)
                            {
                                N = TB_ZDD.Rows.Count - 2;
                            }
                            DataRow ZDD_row1 = TB_ZDD.Rows[N];
                            DataRow ZDD_row2 = TB_ZDD.Rows[J];
                            DataRow ZDD_row3 = TB_ZDD.Rows[J + 1];
                            Point3d point11 = new Point3d((double)ZDD_row1["X"], (double)ZDD_row1["Y"], 0);
                            Point3d point12 = new Point3d((double)ZDD_row2["X"], (double)ZDD_row2["Y"], 0);
                            Point3d point13 = new Point3d((double)ZDD_row3["X"], (double)ZDD_row3["Y"], 0);
                            double jd1 = Tools.CADTools.GetAngle(point11, point12);
                            double jd2 = Tools.CADTools.GetAngle(point13, point12);
                            double Ang = jd1 - jd2;
                            if (Ang < 0) { Ang = Ang + Math.PI * 2.0; }
                            double JPFX = jd2 + Ang / 2.0 + Math.PI;
                            string jzd;
                            if (ZDCT.Checked)
                            {
                                JZDXH.Checked = true;
                            }
                            if (JZDXH.Checked)
                            {
                                jzd = "J" + ZDD_row2["MBBSM"].ToString().Trim();
                            }
                            else
                            {
                                jzd = "J" + ZDD_row2["JZDH"].ToString().Trim();
                            }
                            double xwjl = 3.0 * ATT.BLCH / 1000.0;//界址点号离开界址点距离
                            if (jzd.Length <= 3) { xwjl = 2.5 * ATT.BLCH / 1000.0; }
                            if (jzd.Length == 4) { xwjl = 3.0 * ATT.BLCH / 1000.0; }
                            if (jzd.Length >= 5) { xwjl = 3.5 * ATT.BLCH / 1000.0; }
                            Point3d crd = Tools.CADTools.GetNewPoint(point12, xwjl, JPFX);
                            Tools.CADTools.AddNewLayer("界址点号", "Continuous", 2);
                            scText(jzd, crd, ATT.BLCH * 2.0 / 1000.0, 2, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "界址点号", zt2, 0.8, 0);
                        }
                        #endregion

                        #region 标注距离
                        if (!BZJ.Checked)
                        {
                            string zjlr = "";//注记内容
                            int pp = 0;
                            Tools.CADTools.AddNewLayer("边长注记", "Continuous", 7);
                            for (int J = 0; J < TB_ZDD.Rows.Count - 1; J++)
                            {
                                int N = J - 1;
                                if (N < 0)
                                {
                                    N = TB_ZDD.Rows.Count - 1;
                                }
                                DataRow ZDD_row2 = TB_ZDD.Rows[J];
                                DataRow ZDD_row3 = TB_ZDD.Rows[J + 1];
                                #region 在右下角注记
                                if (YXJ.Checked)//在右下角注记
                                {
                                    string jzdh_q;
                                    string jzdh_z;
                                    if (ZDCT.Checked)
                                    {
                                        JZDXH.Checked = true;
                                    }
                                    if (JZDXH.Checked)//如果注记序号
                                    {
                                        jzdh_q = ZDD_row2["MBBSM"].ToString().Trim();
                                        jzdh_z = ZDD_row3["MBBSM"].ToString().Trim();
                                        if (J == TB_ZDD.Rows.Count - 2)
                                        { jzdh_z = "1"; }
                                        jzdh_q = "J" + jzdh_q;
                                        jzdh_z = "J" + jzdh_z;
                                        jzdh_q = jzdh_q.PadLeft(4, ' ');
                                        jzdh_z = jzdh_z.PadLeft(4, ' ');
                                    }
                                    else//如果界址点号
                                    {
                                        jzdh_q = ZDD_row2["JZDH"].ToString().Trim();
                                        jzdh_z = ZDD_row3["JZDH"].ToString().Trim();
                                        jzdh_q = "J" + jzdh_q;
                                        jzdh_z = "J" + jzdh_z;
                                        jzdh_q = jzdh_q.PadLeft(5, ' ');
                                        jzdh_z = jzdh_z.PadLeft(5, ' ');
                                    }
                                    double jl = (double)ZDD_row2["JZJJ"];
                                    zjlr = zjlr + jzdh_q + "-" + jzdh_z + ":" + jl.ToString("0.00") + "\n";
                                    int vv = J + 1;
                                    if ((((int)(vv / 5)) * 5 == vv) || J == TB_ZDD.Rows.Count - 2)
                                    {
                                        Point3d ZJD = new Point3d(D1[0] + (pp * 25 + 2) * ATT.BLCH / 1000.0, D1[1] + 3 * ATT.BLCH / 1000.0, 0);
                                        if (zjlr != "")
                                        {
                                            scMtext(zjlr, ZJD, 2.0 * ATT.BLCH / 1000.0, 7, AttachmentPoint.BottomLeft, "边长注记", zt2);
                                        }
                                        zjlr = "";
                                        pp++;
                                    }
                                }
                                #endregion
                                #region 注记与界址边
                                if (BSH.Checked)//在右下角注记
                                {
                                    ZDD_row2 = TB_ZDD.Rows[J];
                                    ZDD_row3 = TB_ZDD.Rows[J + 1];
                                    Point3d point11 = new Point3d((double)ZDD_row2["X"], (double)ZDD_row2["Y"], 0);
                                    Point3d point12 = new Point3d((double)ZDD_row3["X"], (double)ZDD_row3["Y"], 0);
                                    Point3d zjd = new Point3d((point11.X + point12.X) / 2.0, (point11.Y + point12.Y) / 2.0, 0);
                                    double jjdd = Tools.CADTools.GetAngle(point11, point12) + Math.PI * 1.0 / 2.0;
                                    Point3d bczjd = Tools.CADTools.GetNewPoint(zjd, 1.7 * ATT.BLCH / 1000.0, jjdd);
                                    double jl2 = (double)ZDD_row2["JZJJ"];
                                    double wzjd = Tools.CADTools.GetAngle(point11, point12);//文字旋转角度
                                    if (wzjd > Math.PI * 2.0)
                                    {
                                        wzjd = wzjd - Math.PI * 2.0;
                                    }
                                    if (wzjd < 0)
                                    {
                                        wzjd = wzjd + Math.PI * 2.0;
                                    }
                                    if ((wzjd >= Math.PI / 4.0 && wzjd < Math.PI * 3.0 / 4.0) || (wzjd >= Math.PI * 5.0 / 4.0 && wzjd < Math.PI * 7.0 / 4.0))
                                    {
                                        wzjd = wzjd + Math.PI;
                                    }
                                    if (wzjd > Math.PI * 2.0)
                                    {
                                        wzjd = wzjd - Math.PI * 2.0;
                                    }
                                    if (wzjd > Math.PI * 0.5 && wzjd < Math.PI * 3.0 / 2.0)
                                    {
                                        wzjd = wzjd + Math.PI;
                                    }
                                    scText(jl2.ToString("0.00"), bczjd, ATT.BLCH * 2.0 / 1000.0, 2, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "边长注记", zt2, 0.8, wzjd);
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region 标注四至
                        Tools.CADTools.AddNewLayer("四至注记", "Continuous", 5);
                        System.Data.DataTable TB_ZDX = da_ZDX.Tables["D_ZDX"];
                        for (int J = 0; J < TB_ZDX.Rows.Count; J++)
                        {
                            int N = J + 1;
                            if (N >= TB_ZDX.Rows.Count)
                            {
                                N = 0;
                            }
                            DataRow ZDX_row1 = TB_ZDX.Rows[J];
                            DataRow ZDX_row2 = TB_ZDX.Rows[N];
                            int jzdxh1 = (int)ZDX_row1["MBBSM"];
                            int jzdxh2 = (int)ZDX_row2["MBBSM"];
                            string lzdjh = ZDX_row1["LZDDJH"].ToString().Trim();
                            if (lzdjh != "")
                            {
                                continue;
                            }
                            string bz = ZDX_row1["BZ"].ToString().Trim();
                            DataRow ZDD_row1 = TB_ZDD.Rows[jzdxh1 - 1];
                            DataRow ZDD_row2 = TB_ZDD.Rows[jzdxh2 - 1];
                            Point2d PD1 = new Point2d((double)ZDD_row1["X"], (double)ZDD_row1["Y"]);
                            Point2d PD2 = new Point2d((double)ZDD_row2["X"], (double)ZDD_row2["Y"]);
                            Point2d PD3 = new Point2d(0, 0);

                            double JD = Tools.CADTools.GetAngle(PD1, PD2);
                            double JD2 = JD + Math.PI / 2.0;

                            if (JD >= Math.PI / 4.0 && JD < Math.PI / 4.0 + Math.PI / 2.0)
                            {
                                PD3 = PD1;
                                PD1 = PD2;
                                PD2 = PD3;
                            }
                            if (JD >= Math.PI - Math.PI / 4.0 && JD < Math.PI + Math.PI / 4.0)
                            {
                                PD3 = PD1;
                                PD1 = PD2;
                                PD2 = PD3;
                            }
                            JD = Tools.CADTools.GetAngle(PD1, PD2);
                            string[] CFlzdh = new string[bz.Length];
                            for (int p = 0; p < bz.Length; p++)
                            {
                                CFlzdh[p] = bz.Substring(p, 1);
                            }
                            double DIS1 = PD1.GetDistanceTo(PD2);
                            double DIS2 = DIS1 / (CFlzdh.Length + 1);
                            for (int k = 0; k < CFlzdh.Length; k++)
                            {
                                Point2d px = Tools.CADTools.GetNewPoint(PD1, DIS2 * (k + 1), JD);
                                Point2d INSpoint = Tools.CADTools.GetNewPoint(px, 6.25 * ATT.BLCH / 1000.0, JD2);
                                Point3d insp = new Point3d(INSpoint.X, INSpoint.Y, 0);
                                scText(CFlzdh[k], insp, ATT.BLCH * 2.5 / 1000.0, 5, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "四至注记", zt2, 1, 0);
                            }
                        }
                        #endregion

                        #region 保存打印信息、生成界址点及消隐界址点
                        TypedValue[] filList6 = new TypedValue[2];
                        filList6[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                        filList6[1] = new TypedValue(8, "本宗界址线");
                        SelectionFilter filter6 = new SelectionFilter(filList6);
                        // Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                        PromptSelectionResult res6 = ed.SelectAll(filter6);
                        SelectionSet SS6 = res6.Value;
                        if (SS6 == null)
                        {
                            MessageBox.Show("此没有界址线！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        ObjectId[] objectId6 = SS6.GetObjectIds();
                        ACDBPolyline Pline6 = (ACDBPolyline)trans.GetObject(objectId6[0], ACDBOpenMode.ForRead);

                        for (int j = 0; j < Pline6.NumberOfVertices; j++)
                        {
                            Point2d p22 = Pline6.GetPoint2dAt(j);
                            Point3d p33 = new Point3d(p22.X, p22.Y, 0);
                            scjzd(p33, ATT.BLCH * 0.4 / 1000.0);
                        }
                        // ResultBuffer resultBuffer6;
                        #region 增加扩展属性
                        TypedValue[] tv = new TypedValue[9];
                        tv[0] = new TypedValue(1001, "ZDTDY");
                        tv[1] = new TypedValue(1000, ATT.A4A3_HS.Substring(0, 2));//什么纸张
                        tv[2] = new TypedValue(1000, ATT.A4A3_HS.Substring(2, 1));//横放还是竖放
                        tv[3] = new TypedValue(1070, ATT.BLCH);//比例尺
                        tv[4] = new TypedValue(1040, Xmin_dy - 2 * ATT.BLCH / 1000.0);//小X
                        tv[5] = new TypedValue(1040, Ymin_dy - 14.0 * ATT.BLCH / 1000.0);//小Y
                        tv[6] = new TypedValue(1040, Xmax_dy + 2.0 * ATT.BLCH / 1000.0);//大X
                        tv[7] = new TypedValue(1040, Ymax_dy + 10.0 * ATT.BLCH / 1000.0);//大Y
                        if (ZDT.Checked)
                        {
                            tv[8] = new TypedValue(1000, "宗地图");//大Y
                        }
                        else
                        {
                            tv[8] = new TypedValue(1000, "宗地草图");//大Y
                        }
                        RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                        regAppTableRecord.Name = "ZDTDY";
                        if (!symbolTable.Has("ZDTDY"))
                        {
                            symbolTable.Add(regAppTableRecord);
                            trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                        }
                        if (!Pline6.IsWriteEnabled)
                        {
                            Pline6.UpgradeOpen();
                        }
                        Pline6.XData = new ResultBuffer(tv);
                        #endregion
                        #endregion

                        #region 结束处理
                        if (wsxdx > 0)
                        {
                            MessageBox.Show("此宗地附近存在" + wsxdx.ToString() + " 处无属性对象", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        trans.Commit();
                        trans.Dispose();
                        #endregion
                    }
                }
            }
            #endregion
            #region 修改图形
            if (XG.Checked)
            {
                if (ZDT.Checked || ZDCT.Checked)
                {
                    if (fileInfo.Exists)
                    {
                        ATT.Document_ZDT_XG = AcadApplication.DocumentManager.Open(zdtfile, false);//打开宗地图
                    }
                    else
                    {
                        MessageBox.Show("宗地图没有生成！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
                if (FCFHT.Checked)
                {

                    FCFHLB fcfhlb;
                    if (FCFHTLB.SelectedItems.Count >= 1)
                    {
                        fcfhlb = (FCFHLB)FCFHTLB.SelectedItem;
                    }
                    else
                    {
                        MessageBox.Show("请选择请选择图幅!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    string ZDT_Path3 = ZDT_Path1 + "\\" + ATT.DJH.Substring(6, 3) + "\\" + ATT.DJH.Substring(9, 3) + "\\";
                    DirectoryInfo mydir3 = new DirectoryInfo(ZDT_Path3);
                    if (!mydir3.Exists)
                    {
                        mydir3.Create();
                    }
                    string zdtfile3 = ZDT_Path3 + fcfhlb.tfm + ".dwg";//宗地图文件名

                    ATT.FCFHT_File = zdtfile3;
                    FileInfo fileInfo3 = new FileInfo(zdtfile3);

                    if (fileInfo3.Exists)
                    {
                        ATT.Document_ZDT_XG = AcadApplication.DocumentManager.Open(zdtfile3, false);//打开宗地图
                    }
                    else
                    {
                        MessageBox.Show("分层分户图没有生成！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }
            #endregion
            #region 生成分层分户图
            if (SCZDT.Checked && FCFHT.Checked)
            {
                #region 条件
                if (HZLB.SelectedItems.Count <= 0)
                {
                    MessageBox.Show("请选择宗地列表");
                    return;
                }
                FCFHLB fcfhlb;
                if (FCFHTLB.SelectedItems.Count >= 1)
                {
                    fcfhlb = (FCFHLB)FCFHTLB.SelectedItem;
                }
                else
                {
                    MessageBox.Show("请选择请选择图幅!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                #endregion
                #region 获取完整路径
                string ZDT_Path3 = ZDT_Path1 + "\\" + ATT.DJH.Substring(6, 3) + "\\" + ATT.DJH.Substring(9, 3) + "\\";
                DirectoryInfo mydir3 = new DirectoryInfo(ZDT_Path3);
                if (!mydir3.Exists)
                {
                    mydir3.Create();
                }
                string zdtfile3 = ZDT_Path3 + fcfhlb.tfm + ".dwg";//宗地图文件名
                ATT.ZDT_File = zdtfile3;
                FileInfo fileInfo3 = new FileInfo(zdtfile3);
                if (fileInfo3.Exists)
                {
                    if (DialogResult.Yes != MessageBox.Show("该分层分户图已经存在，是否重新生成?", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        return;
                    }
                }
                #endregion
                #region 数据准备
                System.Data.DataTable jzx_TB = da_ZDX.Tables["D_ZDD"];
                DataRow row = TB_DCB.Rows[0];
                if (row["WZDJH"].ToString().Trim() == "")
                {
                    if (MessageBox.Show("此宗地尚未录入调查信息，是否继续？", "重要提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                #endregion
                using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
                {
                    ObjectIdCollection objectIdCollection = new ObjectIdCollection();
                    Database db3 = HostApplicationServices.WorkingDatabase.Wblock(objectIdCollection, new Point3d(0, 0, 0));//将选择集写入数据库
                    db3.SaveAs(zdtfile3, DwgVersion.Current);//保存数据库到dwg文件
                }
                //ATT.Document_ZDT_SC = AcadApplication.DocumentManager.Add("");//新建地图窗口
                ATT.Document_ZDT_SC = AcadApplication.DocumentManager.Open(zdtfile3, false);//打开宗地图
                using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
                {
                    Database db = HostApplicationServices.WorkingDatabase;
                    using (Transaction trans = db.TransactionManager.StartTransaction())
                    {
                        #region 接口
                        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                        SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                        #endregion
                        Tools.CADTools.AddNewLinetype("X32", Tools.CADTools.GetAppPath() + "ACADISO.LIN");
                        Tools.CADTools.AddNewLayer("本宗界址线", "Continuous", 7);
                        ATT.BLCH = 1000;
                        #region 绘制外图廓线
                        double[] D1 = new double[2];
                        double[] D2 = new double[2];
                        double[] D3 = new double[2];
                        double[] D4 = new double[2];
                        double Xmin_dy = 0;
                        double Ymin_dy = 0;
                        double Xmax_dy = 0;
                        double Ymax_dy = 0;
                        #region 使用A4纸
                        D1[0] = 0;
                        D1[1] = 0;
                        D2[0] = 0;
                        D2[1] = 235.0000;
                        D3[0] = 160.0000;
                        D3[1] = 235.0000;
                        D4[0] = 160.0000;
                        D4[1] = 0;
                        #endregion
                        Xmin_dy = D1[0];
                        Ymin_dy = D1[1];
                        Xmax_dy = D3[0];
                        Ymax_dy = D3[1];

                        ACDBPolyline tkx = new ACDBPolyline();
                        tkx.AddVertexAt(0, new Point2d(D1), 0, 0, 0);
                        tkx.AddVertexAt(1, new Point2d(D2), 0, 0, 0);
                        tkx.AddVertexAt(2, new Point2d(D3), 0, 0, 0);
                        tkx.AddVertexAt(3, new Point2d(D4), 0, 0, 0);
                        tkx.Closed = true;
                        Tools.CADTools.AddNewLayer("本宗界址线", "Continuous", 7);
                        tkx.Layer = "本宗界址线";
                        tkx.ColorIndex = 7;
                        tkx.Linetype = "Continuous";
                        btr.AppendEntity(tkx);
                        trans.AddNewlyCreatedDBObject(tkx, true);

                        double[] Dpoint1 = new double[3];
                        double[] Dpoint2 = new double[3];
                        Dpoint1[0] = D1[0];
                        Dpoint1[1] = D2[1] - ATT.BLCH * 20.0 / 1000.0;
                        Dpoint2[0] = D3[0];
                        Dpoint2[1] = Dpoint1[1];
                        Tools.CADTools.AddNewLayer("图廓线2", "Continuous", 7);
                        ACDBPolyline tkx2 = new ACDBPolyline();
                        tkx2.AddVertexAt(0, new Point2d(Dpoint1), 0, 0, 0);
                        tkx2.AddVertexAt(1, new Point2d(Dpoint2), 0, 0, 0);
                        //tkx2.Closed = true;
                        Tools.CADTools.AddNewLayer("图廓线2", "Continuous", 7);
                        tkx2.Layer = "图廓线2";
                        tkx2.ColorIndex = 7;
                        tkx2.Linetype = "Continuous";
                        btr.AppendEntity(tkx2);
                        trans.AddNewlyCreatedDBObject(tkx2, true);
                        #endregion
                        #region 绘制图廓整饰
                        Point2d point1 = new Point2d(D1);//左下
                        Point2d point2 = new Point2d(D2);//左上
                        Point2d point3 = new Point2d(D3);//右上
                        Point2d point4 = new Point2d(D4);//右下

                        double[] Dtext = new double[3];
                        Dtext[0] = (D2[0] + D3[0]) / 2.0;

                        Dtext[1] = D2[1] + ATT.BLCH * 5.5 / 1000.0;

                        MText mtext5 = new MText();
                        mtext5.Contents = "分层分户图";
                        mtext5.TextHeight = ATT.BLCH * 4.5 / 1000.0;
                        mtext5.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext5.Location = new Point3d(Dtext);
                        mtext5.ColorIndex = 7;
                        mtext5.Attachment = AttachmentPoint.MiddleCenter;
                        mtext5.LayerId = Tools.CADTools.AddNewLayer("宗地图注记", "Continuous", 7);
                        btr.AppendEntity(mtext5);
                        trans.AddNewlyCreatedDBObject(mtext5, true);
                        Dtext[0] = D3[0];
                        Dtext[1] = D2[1] + ATT.BLCH * 4.0 / 1000.0;
                        MText mtext6 = new MText();
                        mtext6.Contents = "单位:米/平方米";
                        mtext6.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                        mtext6.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext6.Location = new Point3d(Dtext);
                        mtext6.ColorIndex = 7;
                        mtext6.Attachment = AttachmentPoint.MiddleRight;
                        mtext6.LayerId = Tools.CADTools.AddNewLayer("单位注记", "Continuous", 7);
                        btr.AppendEntity(mtext6);
                        trans.AddNewlyCreatedDBObject(mtext6, true);


                        Dtext[0] = D2[0] + ATT.BLCH * 10.0 / 1000.0;
                        Dtext[1] = D2[1] - ATT.BLCH * 10.0 / 1000.0;
                        MText mtext7 = new MText();
                        string zj1 = "宗地代码:" + row["WZDJH"].ToString().Trim() + "\r\n";
                        //zj1 = zj1 + "所在图幅号:" + row["TFH"].ToString().Trim() + "\r\n";
                        double jzzmj = (double)row["JZMJ"];
                        zj1 = zj1 + "建筑总面积：" + jzzmj.ToString("0.00");
                        mtext7.Contents = zj1;
                        mtext7.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                        mtext7.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext7.Location = new Point3d(Dtext);
                        mtext7.ColorIndex = 7;
                        mtext7.Attachment = AttachmentPoint.MiddleLeft;
                        mtext7.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext7);
                        trans.AddNewlyCreatedDBObject(mtext7, true);

                        Dtext[0] = (D2[0] + D3[0]) / 2.0 + ATT.BLCH * 10.0 / 1000.0;
                        Dtext[1] = D2[1] - ATT.BLCH * 10 / 1000.0;
                        MText mtext8 = new MText();
                        string zj2 = "土地权利人:" + row["QLR"].ToString().Trim() + "\r\n";
                        double zdmj2 = Convert.ToDouble(ZDMJ.Text.Trim());
                        double pzmj = Convert.ToDouble(FZMJ.Text.Trim());
                        if (pzmj < 0.00001)
                        {
                            MessageBox.Show("发证面积没有填写", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        //zj2 = zj2 +  "宗地面积:" + zdmj2.ToString("0.00") + "  " + "发证面积:" + pzmj.ToString("0.00") + "\r\n";
                        double jzzdmj = (double)row["JZZDMJ"];
                        zj2 = zj2 + "建筑占地总面积：" + jzzdmj.ToString("0.00");
                        mtext8.Contents = zj2;
                        mtext8.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                        mtext8.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext8.Location = new Point3d(Dtext);
                        mtext8.ColorIndex = 7;
                        mtext8.Attachment = AttachmentPoint.MiddleLeft;
                        mtext8.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext8);
                        trans.AddNewlyCreatedDBObject(mtext8, true);



                        Dtext[0] = D1[0];
                        Dtext[1] = D1[1] - ATT.BLCH * 2.5 / 1000.0;
                        MText mtext11 = new MText();
                        string zj4 = "";
                        if (CLRQ.Text.ToString().Trim() != "")
                        {
                            zj4 = CLRQ.Text.ToString().Trim() + "解析法测绘界址点" + "\r\n";
                        }
                        else
                        {
                            zj4 = "    年  月  日" + "图解法测绘界址点" + "\r\n";
                        }
                        zj4 = zj4 + "制图日期:" + SCRQ.Text.ToString().Trim() + "\r\n";
                        zj4 = zj4 + "审核日期:" + SHRQ.Text.ToString().Trim() + "\r\n";
                        mtext11.Contents = zj4;
                        mtext11.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                        mtext11.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext11.Location = new Point3d(Dtext);
                        mtext11.ColorIndex = 7;
                        mtext11.Attachment = AttachmentPoint.TopLeft;
                        mtext11.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext11);
                        trans.AddNewlyCreatedDBObject(mtext11, true);


                        Dtext[0] = (D2[0] + D3[0]) / 2.0;
                        Dtext[1] = D1[1] - ATT.BLCH * 4.0 / 1000.0;


                        Dtext[0] = D4[0] - ATT.BLCH * 20 / 1000.0;
                        Dtext[1] = D4[1] - ATT.BLCH * 2.5 / 1000.0;
                        MText mtext12 = new MText();
                        string zj5 = "制图者:" + HTY.Text.Trim() + "\r\n";
                        zj5 = zj5 + "审核者:" + SHRXM.Text.Trim();
                        mtext12.Contents = zj5;
                        mtext12.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                        mtext12.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext12.Location = new Point3d(Dtext);
                        mtext12.ColorIndex = 7;
                        mtext12.Attachment = AttachmentPoint.TopLeft;
                        mtext12.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext12);
                        trans.AddNewlyCreatedDBObject(mtext12, true);
                        #endregion
                        #region 绘制房屋
                        if (fcfhlb.gs == 1)
                        {
                            double gd = 215.0;
                            double kd = 106.0;
                            Point2d cen = new Point2d(80, 107.5000);
                            HZFW(0, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                        }
                        if (fcfhlb.gs == 2)
                        {
                            #region 绘制分割线
                            ACDBPolyline tkx22 = new ACDBPolyline();
                            tkx22.AddVertexAt(0, new Point2d(0, 107.5), 0, 0, 0);
                            tkx22.AddVertexAt(1, new Point2d(160, 107.5), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx22.Layer = "图廓线2";
                            tkx22.ColorIndex = 7;
                            tkx22.Linetype = "x32";
                            btr.AppendEntity(tkx22);
                            trans.AddNewlyCreatedDBObject(tkx22, true);
                            #endregion
                            double gd = 107.5;
                            double kd = 106.0;
                            Point2d cen = new Point2d(80, 161.2500);
                            HZFW(0, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                            cen = new Point2d(80, 53.7500);
                            HZFW(1, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                        }
                        if (fcfhlb.gs == 3)
                        {
                            #region 绘制分割线
                            ACDBPolyline tkx22 = new ACDBPolyline();
                            tkx22.AddVertexAt(0, new Point2d(0, 143.3333), 0, 0, 0);
                            tkx22.AddVertexAt(1, new Point2d(160, 143.3333), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx22.Layer = "图廓线2";
                            tkx22.ColorIndex = 7;
                            tkx22.Linetype = "x32";
                            btr.AppendEntity(tkx22);
                            trans.AddNewlyCreatedDBObject(tkx22, true);

                            ACDBPolyline tkx33 = new ACDBPolyline();
                            tkx33.AddVertexAt(0, new Point2d(0, 71.6667), 0, 0, 0);
                            tkx33.AddVertexAt(1, new Point2d(160, 71.6667), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx33.Layer = "图廓线2";
                            tkx33.ColorIndex = 7;
                            tkx33.Linetype = "x32";
                            btr.AppendEntity(tkx33);
                            trans.AddNewlyCreatedDBObject(tkx33, true);

                            #endregion
                            double gd = 71.6667;
                            double kd = 106.0;
                            Point2d cen = new Point2d(80, 179.1667);
                            HZFW(0, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                            cen = new Point2d(80, 107.5000);
                            HZFW(1, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                            cen = new Point2d(80, 35.8333);
                            HZFW(2, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                        }
                        if (fcfhlb.gs == 4)
                        {
                            #region 绘制分割线
                            ACDBPolyline tkx22 = new ACDBPolyline();
                            tkx22.AddVertexAt(0, new Point2d(0, 107.5), 0, 0, 0);
                            tkx22.AddVertexAt(1, new Point2d(160, 107.5), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx22.Layer = "图廓线2";
                            tkx22.ColorIndex = 7;
                            tkx22.Linetype = "x32";
                            btr.AppendEntity(tkx22);
                            trans.AddNewlyCreatedDBObject(tkx22, true);

                            ACDBPolyline tkx44 = new ACDBPolyline();
                            tkx44.AddVertexAt(0, new Point2d(80, 215), 0, 0, 0);
                            tkx44.AddVertexAt(1, new Point2d(80, 0), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx44.Layer = "图廓线2";
                            tkx44.ColorIndex = 7;
                            tkx44.Linetype = "x32";
                            btr.AppendEntity(tkx44);
                            trans.AddNewlyCreatedDBObject(tkx44, true);
                            #endregion
                            double gd = 107.5;
                            double kd = 80.0;
                            Point2d cen = new Point2d(40, 161.2500);
                            HZFW(0, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                            cen = new Point2d(120, 161.2500);
                            HZFW(1, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);

                            cen = new Point2d(40, 53.7500);
                            HZFW(2, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);

                            cen = new Point2d(120, 53.7500);
                            HZFW(3, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                        }
                        if (fcfhlb.gs == 5)
                        {
                            #region 绘制分割线
                            ACDBPolyline tkx22 = new ACDBPolyline();
                            tkx22.AddVertexAt(0, new Point2d(0, 143.3333), 0, 0, 0);
                            tkx22.AddVertexAt(1, new Point2d(160, 143.3333), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx22.Layer = "图廓线2";
                            tkx22.ColorIndex = 7;
                            tkx22.Linetype = "x32";
                            btr.AppendEntity(tkx22);
                            trans.AddNewlyCreatedDBObject(tkx22, true);

                            ACDBPolyline tkx33 = new ACDBPolyline();
                            tkx33.AddVertexAt(0, new Point2d(0, 71.6667), 0, 0, 0);
                            tkx33.AddVertexAt(1, new Point2d(160, 71.6667), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx33.Layer = "图廓线2";
                            tkx33.ColorIndex = 7;
                            tkx33.Linetype = "x32";
                            btr.AppendEntity(tkx33);
                            trans.AddNewlyCreatedDBObject(tkx33, true);

                            ACDBPolyline tkx44 = new ACDBPolyline();
                            tkx44.AddVertexAt(0, new Point2d(80, 215), 0, 0, 0);
                            tkx44.AddVertexAt(1, new Point2d(80, 0), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx44.Layer = "图廓线2";
                            tkx44.ColorIndex = 7;
                            tkx44.Linetype = "x32";
                            btr.AppendEntity(tkx44);
                            trans.AddNewlyCreatedDBObject(tkx44, true);

                            #endregion
                            double gd = 71.6667;
                            double kd = 106.0;
                            Point2d cen = new Point2d(40, 179.1667);
                            HZFW(0, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                            cen = new Point2d(120, 179.1667);
                            HZFW(1, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                            cen = new Point2d(40, 107.5000);
                            HZFW(2, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);

                            cen = new Point2d(120, 107.5000);
                            HZFW(3, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);

                            cen = new Point2d(40, 35.8333);
                            HZFW(4, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                        }
                        if (fcfhlb.gs == 6)
                        {
                            #region 绘制分割线
                            ACDBPolyline tkx22 = new ACDBPolyline();
                            tkx22.AddVertexAt(0, new Point2d(0, 143.3333), 0, 0, 0);
                            tkx22.AddVertexAt(1, new Point2d(160, 143.3333), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx22.Layer = "图廓线2";
                            tkx22.ColorIndex = 7;
                            tkx22.Linetype = "x32";
                            btr.AppendEntity(tkx22);
                            trans.AddNewlyCreatedDBObject(tkx22, true);

                            ACDBPolyline tkx33 = new ACDBPolyline();
                            tkx33.AddVertexAt(0, new Point2d(0, 71.6667), 0, 0, 0);
                            tkx33.AddVertexAt(1, new Point2d(160, 71.6667), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx33.Layer = "图廓线2";
                            tkx33.ColorIndex = 7;
                            tkx33.Linetype = "x32";
                            btr.AppendEntity(tkx33);
                            trans.AddNewlyCreatedDBObject(tkx33, true);

                            ACDBPolyline tkx44 = new ACDBPolyline();
                            tkx44.AddVertexAt(0, new Point2d(80, 215), 0, 0, 0);
                            tkx44.AddVertexAt(1, new Point2d(80, 0), 0, 0, 0);
                            Tools.CADTools.AddNewLayer("图廓线2", "x32", 7);
                            tkx44.Layer = "图廓线2";
                            tkx44.ColorIndex = 7;
                            tkx44.Linetype = "x32";
                            btr.AppendEntity(tkx44);
                            trans.AddNewlyCreatedDBObject(tkx44, true);

                            #endregion
                            double gd = 71.6667;
                            double kd = 106.0;
                            Point2d cen = new Point2d(40, 179.1667);
                            HZFW(0, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                            cen = new Point2d(120, 179.1667);
                            HZFW(1, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                            cen = new Point2d(40, 107.5000);
                            HZFW(2, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);

                            cen = new Point2d(120, 107.5000);
                            HZFW(3, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);

                            cen = new Point2d(40, 35.8333);
                            HZFW(4, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);

                            cen = new Point2d(120, 35.8333);
                            HZFW(5, fcfhlb.fwsx, fcfhlb.fw_Polygon, gd, kd, cen);
                        }
                        #endregion
                        #region 保存打印信息
                        ACDBPolyline Pline6 = tkx;
                        #region 增加扩展属性
                        TypedValue[] tv = new TypedValue[9];
                        tv[0] = new TypedValue(1001, "ZDTDY");
                        tv[1] = new TypedValue(1000, "A4");//什么纸张
                        tv[2] = new TypedValue(1000, "S");//横放还是竖放
                        tv[3] = new TypedValue(1070, ATT.BLCH);//比例尺
                        tv[4] = new TypedValue(1040, Xmin_dy - 2 * ATT.BLCH / 1000.0);//小X
                        tv[5] = new TypedValue(1040, Ymin_dy - 14.0 * ATT.BLCH / 1000.0);//小Y
                        tv[6] = new TypedValue(1040, Xmax_dy + 2.0 * ATT.BLCH / 1000.0);//大X
                        tv[7] = new TypedValue(1040, Ymax_dy + 10.0 * ATT.BLCH / 1000.0);//大Y
                        tv[8] = new TypedValue(1000, "分层分户图");

                        RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                        regAppTableRecord.Name = "ZDTDY";
                        if (!symbolTable.Has("ZDTDY"))
                        {
                            symbolTable.Add(regAppTableRecord);
                            trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                        }
                        if (!Pline6.IsWriteEnabled)
                        {
                            Pline6.UpgradeOpen();
                        }
                        Pline6.XData = new ResultBuffer(tv);
                        #endregion
                        #endregion
                        trans.Commit();
                        trans.Dispose();
                    }

                    Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("LTSCALE", 1);//设置比例尺
                }
                AcadApplication.DocumentManager.MdiActiveDocument.SendStringToExecute("Zoom\nE\n", false, false, false);
            }
            #endregion
        }

        // 绘制房屋
        /// <summary>
        /// 绘制房屋
        /// </summary>
        /// <param name="xh">第几个房屋</param>
        /// <param name="SX">房屋属性</param>
        /// <param name="TX">几何图形</param>
        /// <param name="DG">绘制框高度</param>
        /// <param name="KD">绘制框宽度</param>
        /// <param name="cen">中心点坐标</param>
        public void HZFW(int xh, List<FWSXJG> SX, List<Topology.Geometries.Polygon> TX, double GD, double KD, Point2d cen)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 定义
                Database db = HostApplicationServices.WorkingDatabase;
                int blc = 1000;
                Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                ObjectId zt2 = Tools.CADTools.AddNewTextStyle("宋体", "宋体", 1, 0, false);
                Tools.CADTools.AddNewLayer("房屋边长注记", "Continuous", 7);
                Tools.CADTools.AddNewLayer("房屋", "Continuous", 7);
                Tools.CADTools.AddNewLayer("附属设施", "Continuous", 1);
                Tools.CADTools.AddNewLayer("说明注记", "Continuous", 2);
                #endregion
                // Tools.CADTools.AddNewLinetype("X32", Tools.CADTools.GetAppPath() + "ACADISO.LIN");
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    #region 接口
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    #endregion
                    #region 找出所有阳台等
                    FWSXJG n_sx = SX[xh];
                    Topology.Geometries.Polygon n_Tx = TX[xh];
                    int dh = Convert.ToInt32(n_sx.房屋幢号9.Trim());
                    int cs = Convert.ToInt32(n_sx.所在层数12.Trim());
                    List<FWSXJG> YT_SX = new List<FWSXJG>();//本层及含阳台的数据集合
                    List<Topology.Geometries.Polygon> YT_TX = new List<Topology.Geometries.Polygon>();//本层及含阳台的数据集合
                    YT_SX.Add(n_sx);
                    YT_TX.Add(n_Tx);
                    for (int i = 0; i < SX.Count; i++)
                    {
                        if (SX[i].建筑物类型0 == "阳台" || SX[i].建筑物类型0 == "楼梯" || SX[i].建筑物类型0 == "檐廊")
                        {
                            if (dh == Convert.ToInt32(SX[i].房屋幢号9.Trim()) && cs == Convert.ToInt32(SX[i].所在层数12.Trim()))
                            {
                                YT_SX.Add(SX[i]);
                                YT_TX.Add(TX[i]);
                            }
                        }
                    }
                    #endregion
                    #region 计算比例尺
                    Topology.Geometries.Polygon[] fws = new Topology.Geometries.Polygon[YT_SX.Count];
                    for (int i = 0; i < YT_TX.Count; i++)
                    {
                        fws[i] = YT_TX[i];
                    }
                    Topology.Geometries.MultiPolygon mtx = new Topology.Geometries.MultiPolygon(fws);
                    double max = mtx.EnvelopeInternal.MaxX;
                    double may = mtx.EnvelopeInternal.MaxY;
                    double mix = mtx.EnvelopeInternal.MinX;
                    double miy = mtx.EnvelopeInternal.MinY;
                    double blc1 = 1000 * (max - mix) / (KD - KD * 0.4);
                    double blc2 = 1000 * (may - miy) / (GD - GD * 0.4);

                    if (blc1 > blc2)
                    {
                        blc = 10 * ((int)(blc1 / 10.0) + 1);
                    }
                    else
                    {
                        blc = 10 * ((int)(blc2 / 10.0) + 1);
                    }
                    #endregion
                    #region 简化对象
                    ACDBPolyline[] pls = Tools.CADTools.WriteToDwg(mtx);
                    for (int i = 0; i < pls.Length; i++)
                    {
                        ACDBPolyline pl = pls[i];
                        for (int j = 0; j < pl.NumberOfVertices; j++)
                        {
                            int qd = j - 1;
                            if (qd < 0)
                            {
                                qd = pl.NumberOfVertices - 1;
                            }
                            int hd = j + 1;
                            if (hd == pl.NumberOfVertices)
                            {
                                hd = 0;
                            }

                            ACDBPolyline pl_n = new ACDBPolyline();
                            pl_n.AddVertexAt(0, pl.GetPoint2dAt(qd), 0, 0, 0);
                            pl_n.AddVertexAt(1, pl.GetPoint2dAt(hd), 0, 0, 0);
                            pl_n.AddVertexAt(2, pl.GetPoint2dAt(j), 0, 0, 0);
                            double mj = pl_n.Area;
                            double jl = mj * 2.0 / (pl.GetPoint2dAt(qd).GetDistanceTo(pl.GetPoint2dAt(hd)) + 1e-15);
                            if (jl < 0.001)
                            {
                                pl.RemoveVertexAt(j);
                                j--;
                            }
                        }

                    }
                    #endregion
                    #region 缩放
                    for (int i = 0; i < pls.Length; i++)
                    {
                        ACDBPolyline pl = pls[i];
                        for (int j = 0; j < pl.NumberOfVertices; j++)
                        {
                            pl.SetPointAt(j, new Point2d(1000.0 * pl.GetPoint2dAt(j).X / blc, 1000.0 * pl.GetPoint2dAt(j).Y / blc));
                        }
                        pls[i] = pl;
                    }
                    #endregion
                    #region 平移
                    double px = cen.X - 1000 * mtx.Centroid.X / blc;
                    double py = cen.Y - 1000 * mtx.Centroid.Y / blc;

                    for (int i = 0; i < pls.Length; i++)
                    {
                        ACDBPolyline pl = pls[i];
                        for (int j = 0; j < pl.NumberOfVertices; j++)
                        {
                            pl.SetPointAt(j, new Point2d(pl.GetPoint2dAt(j).X + px, pl.GetPoint2dAt(j).Y + py));
                        }
                        pls[i] = pl;
                    }
                    #endregion
                    #region 注记边长
                    // TODO:检查注记边长是否有错误
                    for (int i = 0; i < pls.Length; i++)
                    {
                        ACDBPolyline pline = pls[i];
                        pline = ssz(pline);
                        for (int k = 0; k < pline.NumberOfVertices; k++)
                        {
                            int N = k + 1;
                            if (N == pline.NumberOfVertices)
                            {
                                N = 0;
                            }
                            Point3d point11 = pline.GetPoint3dAt(k);
                            Point3d point12 = pline.GetPoint3dAt(N);
                            Point3d zjd3 = new Point3d((point11.X + point12.X) / 2.0, (point11.Y + point12.Y) / 2.0, 0);
                            double jjdd = Tools.CADTools.GetAngle(point11, point12) + Math.PI * 1.0 / 2.0;
                            Point3d bczjd = Tools.CADTools.GetNewPoint(zjd3, 1.7 * ATT.BLCH / 1000.0, jjdd);
                            double jl2 = point11.DistanceTo(point12) * blc / 1000.0;
                            double wzjd = Tools.CADTools.GetAngle(point11, point12);//文字旋转角度
                            if (wzjd > Math.PI * 2.0)
                            {
                                wzjd = wzjd - Math.PI * 2.0;
                            }
                            if (wzjd < 0)
                            {
                                wzjd = wzjd + Math.PI * 2.0;
                            }
                            if ((wzjd >= Math.PI / 4.0 && wzjd < Math.PI * 3.0 / 4.0) || (wzjd >= Math.PI * 5.0 / 4.0 && wzjd < Math.PI * 7.0 / 4.0))
                            {
                                wzjd = wzjd + Math.PI;
                            }
                            if (wzjd > Math.PI * 2.0)
                            {
                                wzjd = wzjd - Math.PI * 2.0;
                            }
                            if (wzjd > Math.PI * 0.5 && wzjd < Math.PI * 3.0 / 2.0)
                            {
                                wzjd = wzjd + Math.PI;
                            }
                            scText(jl2.ToString("0.00"), bczjd, 2.0, 3, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "房屋边长注记", zt2, 0.8, wzjd);
                        }
                    }
                    #endregion
                    #region 注记比例尺
                    Point3d blczjwz = new Point3d(cen.X, cen.Y - GD / 2.0 + 5, 0);
                    scText("1:" + blc.ToString(), blczjwz, 2.3, 7, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "房屋边长注记", zt2, 1, 0);
                    #endregion
                    #region 注记   说明
                    for (int i = 0; i < YT_SX.Count; i++)
                    {
                        FWSXJG fwsx = YT_SX[i];
                        double mj = YT_TX[i].Area;
                        Point3d d1 = pls[i].Bounds.Value.MaxPoint;
                        Point3d d2 = pls[i].Bounds.Value.MinPoint;
                        Point3d zjd = new Point3d((d1.X + d2.X) / 2.0, (d1.Y + d2.Y) / 2.0, 0);
                        if (fwsx.建筑物类型0 == "房屋" || fwsx.建筑物类型0 == "门廊" || fwsx.建筑物类型0 == "车棚")
                        {
                            #region 分析简注记
                            string fwjg = fwsx.房屋结构13;
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
                            #endregion
                            #region 注记
                            jhjg = jhjg + fwsx.总层数11;
                            string fz = fwsx.房屋幢号9 + "幢" + fwsx.所在层数12 + "层";
                            MText mtext = new MText();
                            mtext.Contents = "\\A1;" + "{\\H0.7x;\\S" + fz + "/" + jhjg + ";}" + mj.ToString("0.00");
                            mtext.TextHeight = 2.7;
                            ObjectId ZT3 = Tools.CADTools.AddNewTextStyle("宋体", "宋体", 1, 0, false);
                            mtext.TextStyleId = ZT3;
                            mtext.Location = zjd;
                            mtext.ColorIndex = 6;
                            mtext.Attachment = AttachmentPoint.MiddleCenter;
                            mtext.LayerId = Tools.CADTools.AddNewLayer("说明注记", "Continuous", 6);
                            btr.AppendEntity(mtext);
                            trans.AddNewlyCreatedDBObject(mtext, true);
                            #endregion
                        }
                        else
                        {
                            double mj2 = mj * Convert.ToDouble(fwsx.面积折扣系数22);
                            string fz = fwsx.建筑物类型0 + "/" + mj2.ToString("0.00");
                            scText(fz, zjd, 2.5, 6, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "说明注记", zt2, 1, 0);
                        }

                    }
                    #endregion
                    #region 设置线外观，添加到图形
                    for (int i = 0; i < pls.Length; i++)
                    {
                        ACDBPolyline pline = pls[i];
                        FWSXJG fwsx = YT_SX[i];
                        if (fwsx.建筑物类型0 == "房屋" || fwsx.建筑物类型0 == "门廊" || fwsx.建筑物类型0 == "车棚")
                        {
                            pline.Layer = "房屋";
                            pline.ColorIndex = 1;
                            btr.AppendEntity(pline);
                            trans.AddNewlyCreatedDBObject(pline, true);
                        }
                        else
                        {
                            pline.Layer = "房屋";
                            pline.ColorIndex = 2;
                            pline.Linetype = "x32";
                            pline.Plinegen = true;
                            btr.AppendEntity(pline);
                            trans.AddNewlyCreatedDBObject(pline, true);
                        }
                    }
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }

        private void SCZDT_CheckedChanged(object sender, EventArgs e)
        {
            if (SCZDT.Checked)
            {
                HZLB.SelectionMode = System.Windows.Forms.SelectionMode.One;
            }
        }

        private void ZDCT_CheckedChanged(object sender, EventArgs e)
        {
            if (ZDCT.Checked)
            {
                JZDXH.Checked = true;
            }
        }

        private void PLDY_CheckedChanged(object sender, EventArgs e)
        {
            if (PLDY.Checked)
            {
                HZLB.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            }
            else
            {
                HZLB.SelectionMode = System.Windows.Forms.SelectionMode.One;
            }
        }

        private void DYJMC_SelectedIndexChanged(object sender, EventArgs e)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    LayoutManager layoutManager = LayoutManager.Current;
                    Autodesk.AutoCAD.DatabaseServices.Layout layout = (Autodesk.AutoCAD.DatabaseServices.Layout)trans.GetObject(layoutManager.GetLayoutId(layoutManager.CurrentLayout), ACDBOpenMode.ForRead);
                    Autodesk.AutoCAD.PlottingServices.PlotInfo plotInfo = new Autodesk.AutoCAD.PlottingServices.PlotInfo();
                    plotInfo.Layout = layout.ObjectId;
                    PlotSettings plotSettings = new PlotSettings(layout.ModelType);
                    plotSettings.CopyFrom(layout);
                    PlotSettingsValidator plotSettingsValidator = PlotSettingsValidator.Current;
                    System.Collections.Specialized.StringCollection PlotDeviceList = plotSettingsValidator.GetPlotDeviceList();
                    //for (int i = 0; i < PlotDeviceList.Count; i++)
                    //{
                    //    DYJMC.Items.Add(PlotDeviceList[i].ToString());
                    //}
                    ////plotSettingsValidator.SetPlotWindowArea(plotSettings, new Extents2d(1, 1, 5, 5));
                    //plotSettingsValidator.SetPlotType(plotSettings, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);
                    plotSettingsValidator.SetPlotConfigurationName(plotSettings, DYJMC.SelectedItem.ToString(), null);
                    System.Collections.Specialized.StringCollection dyz = plotSettingsValidator.GetCanonicalMediaNameList(plotSettings);//纸张
                    A3DYZ.Items.Clear();
                    A4DYZ.Items.Clear();
                    for (int i = 0; i < dyz.Count; i++)
                    {
                        A3DYZ.Items.Add(dyz[i].ToString());
                        A4DYZ.Items.Add(dyz[i].ToString());
                    }
                    // plotInfo.OverrideSettings = plotSettings;
                    trans.Commit();
                }
            }
        }

        // 图表输出 - “绘制阴影”按钮
        private void button30_Click(object sender, EventArgs e)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                #region 条件检查与准备
                DataRow DCB_row1 = TB_DCB.Rows[0];
                double zdmj = (double)DCB_row1["ZDMJ"];//宗地面积
                // double pzmj = (double)DCB_row1["FZMJ"];//发证面积
                double pzmj = Convert.ToDouble(FZMJ.Text.Trim());
                double yymj = zdmj - pzmj;//阴影部分面积
                if (zdmj - pzmj < 0.01)
                {
                    MessageBox.Show("发证面积和宗地面积相同，不需要绘制阴影。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                if (pzmj < 1.0)
                {
                    MessageBox.Show("发证面积没有赋值。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                if (pzmj - zdmj > 0)
                {
                    MessageBox.Show("发证面积大于宗地面积，有错误。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                if (zdmj - pzmj < 1)
                {
                    if (DialogResult.No == MessageBox.Show("阴影面积小于一个平方米，是否绘制？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Stop))
                    {
                        return;
                    }
                }
                Database db = HostApplicationServices.WorkingDatabase;
                #endregion
                Tools.CADTools.AddNewLayer("构面辅助线", "Continuous", 7);
                ObjectId plineID = Tools.CADTools.CreatPolyline("0", "Continuous", 0, 7, 0, 0, 1);
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    #region 获取本宗界址线、删除阴影
                    TypedValue[] filList = new TypedValue[2];
                    filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                    filList[1] = new TypedValue(8, "本宗界址线");
                    SelectionFilter filter = new SelectionFilter(filList);
                    Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                    PromptSelectionResult res = ed.SelectAll(filter);
                    SelectionSet SS = res.Value;
                    if (SS == null)
                    {
                        MessageBox.Show("没有本宗界址线", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    ObjectId[] objectId = SS.GetObjectIds();
                    if (objectId.Length != 1)
                    {
                        MessageBox.Show("本宗界址线太多了", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    TypedValue[] filList2 = new TypedValue[1];
                    filList2[0] = new TypedValue(8, "阴影线");
                    SelectionFilter filter2 = new SelectionFilter(filList2);

                    PromptSelectionResult res2 = ed.SelectAll(filter2);
                    SelectionSet SS2 = res2.Value;
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    ACDBPolyline pline_jzx = (ACDBPolyline)trans.GetObject(objectId[0], ACDBOpenMode.ForWrite);
                    if (Math.Abs(pline_jzx.Area - zdmj) > 0.1)
                    {
                        MessageBox.Show("CAD图形中的宗地面积和数据库中的宗地面积不一致", "重大问题", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    /////删除原有阴影
                    if (SS2 != null)
                    {
                        ObjectId[] objectId2 = SS2.GetObjectIds();
                        for (int i = 0; i < objectId2.Length; i++)
                        {
                            Entity entity = (Entity)trans.GetObject(objectId2[i], ACDBOpenMode.ForWrite);
                            entity.Erase();
                        }
                    }
                    /////删除原有阴影
                    #endregion
                    #region 求阴影部分范围线
                    ACDBPolyline pline = (ACDBPolyline)trans.GetObject(plineID, ACDBOpenMode.ForWrite);
                    #region 使阴影线按顺时针方向
                    ArrayList zb = new ArrayList();
                    double JD = 0;
                    int DDS = pline.NumberOfVertices;
                    for (int j = 0; j < DDS; j++)
                    {
                        Point2d D0 = new Point2d();
                        Point2d D1 = new Point2d();
                        Point2d D2 = new Point2d();
                        zb.Add(pline.GetPoint2dAt(j));
                        if (j == 0)
                        {
                            D0 = pline.GetPoint2dAt(DDS - 1);
                            D1 = pline.GetPoint2dAt(0);
                            D2 = pline.GetPoint2dAt(1);
                        }
                        if ((j >= 1) && (j < DDS - 1))
                        {
                            D0 = pline.GetPoint2dAt(j - 1);
                            D1 = pline.GetPoint2dAt(j);
                            D2 = pline.GetPoint2dAt(j + 1);
                        }
                        if (j == DDS - 1)
                        {
                            D0 = pline.GetPoint2dAt(j - 1);
                            D1 = pline.GetPoint2dAt(j);
                            D2 = pline.GetPoint2dAt(0);
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
                            pline.SetPointAt(p, (Point2d)zb[DDS - p - 1]);
                        }
                    }
                    #endregion
                    if (pline.Area < 0.01 || pline.NumberOfVertices < 3)
                    {
                        MessageBox.Show("给定初始范围线有问题。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        pline.Erase();
                        trans.Commit();
                        return;
                    }
                    pline.Closed = true;
                    Topology.Geometries.Polygon polygon_jzx = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(pline_jzx);//由界址线构成的面
                    Topology.Geometries.Polygon polygon = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(pline);//
                    Topology.Geometries.Geometry yy = (Topology.Geometries.Geometry)polygon.Intersection(polygon_jzx);
                    double fx;
                    if (yy.Area >= zdmj - pzmj)
                    {
                        fx = 1.0;
                    }
                    else
                    {
                        fx = -1.0;
                    }
                    if (fx > 0)
                    {
                        double setp = 0.01;
                        while (yy.Area >= zdmj - pzmj)
                        {
                            DBObjectCollection dbobj = pline.GetOffsetCurves(setp);
                            if (dbobj.Count != 1)
                            {
                                MessageBox.Show("阴影范围线绘制得不好或方向反了，请重新绘制", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                pline.Erase();
                                trans.Commit();
                                return;
                            }
                            ACDBPolyline pline_x = (ACDBPolyline)dbobj[0];
                            Topology.Geometries.Polygon polygon_x = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(pline_x);//
                            yy = (Topology.Geometries.Geometry)polygon_x.Intersection(polygon_jzx);
                            setp = setp + 0.01;
                        }
                        setp = setp - 0.015;
                        DBObjectCollection dbobj2 = pline.GetOffsetCurves(setp);
                        ACDBPolyline pline_x2 = (ACDBPolyline)dbobj2[0];
                        Topology.Geometries.Polygon polygon_x2 = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(pline_x2);//
                        yy = (Topology.Geometries.Geometry)polygon_x2.Intersection(polygon_jzx);
                    }
                    else
                    {
                        double setp = -0.01;
                        while (yy.Area < zdmj - pzmj)
                        {
                            DBObjectCollection dbobj = pline.GetOffsetCurves(setp);
                            if (dbobj.Count != 1)
                            {
                                MessageBox.Show("阴影范围线绘制得不好，请重新绘制", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                pline.Erase();
                                trans.Commit();
                                return;
                            }
                            ACDBPolyline pline_x = (ACDBPolyline)dbobj[0];
                            Topology.Geometries.Polygon polygon_x = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(pline_x);//
                            yy = (Topology.Geometries.Geometry)polygon_x.Intersection(polygon_jzx);
                            setp = setp - 0.01;
                        }
                        setp = setp + 0.015;
                        DBObjectCollection dbobj2 = pline.GetOffsetCurves(setp);
                        ACDBPolyline pline_x2 = (ACDBPolyline)dbobj2[0];
                        Topology.Geometries.Polygon polygon_x2 = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(pline_x2);//
                        yy = (Topology.Geometries.Geometry)polygon_x2.Intersection(polygon_jzx);
                    }
                    #endregion
                    pline.Erase();
                    #region 绘制阴影范围
                    for (int i = 0; i < yy.NumGeometries; i++)
                    {
                        Topology.Geometries.Geometry geometry_n = (Topology.Geometries.Geometry)yy.GetGeometryN(i);
                        if (geometry_n.GeometryType.ToLower() == "polygon")
                        {
                            Tools.CADTools.AddNewLayer("阴影线", "Continuous", 7);
                            ACDBPolyline yym = Tools.CADTools.WriteToDwg((Topology.Geometries.Polygon)geometry_n);
                            yym.Layer = "阴影线";
                            yym.Linetype = "Continuous";
                            btr.AppendEntity(yym);
                            trans.AddNewlyCreatedDBObject(yym, true);
                            //////对本宗地块进行图案填充
                            Hatch hatch = new Hatch();
                            Vector3d vector3d = new Vector3d(0, 0, 1);
                            hatch.Normal = vector3d;
                            double ZDT_blc = 1000 * (double)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("LTSCALE");//街坊图比例尺
                            hatch.PatternScale = 1.5 * ZDT_blc / 200.0;
                            //hatch.PatternScale = 2.0;
                            hatch.LayerId = Tools.CADTools.AddNewLayer("阴影线", "Continuous", 7);
                            hatch.SetHatchPattern(HatchPatternType.PreDefined, "ANSI31");
                            btr.AppendEntity(hatch);
                            trans.AddNewlyCreatedDBObject(hatch, true);
                            hatch.Associative = true;
                            ObjectIdCollection objectIdCollection = new Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection();
                            objectIdCollection.Add(yym.ObjectId);
                            hatch.AppendLoop(HatchLoopTypes.External, objectIdCollection);
                            hatch.EvaluateHatch(true);
                            //////对本宗地块进行图案填充
                        }
                    }
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                }

            }
        }

        // 图表输出 - “草转宗”按钮        
        private void button31_Click(object sender, EventArgs e)
        {// TODO:检查是否有错误

            string file = AcadApplication.DocumentManager.MdiActiveDocument.Database.Filename;
            AcadApplication.DocumentManager.MdiActiveDocument.CloseAndSave(file);
            ATT.Document_ZDT_SC = AcadApplication.DocumentManager.Open(file);

            string zdtfile = "";
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {

                Database db = HostApplicationServices.WorkingDatabase;
                DataRow row = TB_DCB.Rows[0];
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    #region 查看图形是宗地图还是宗地草图
                    TypedValue[] filList = new TypedValue[2];
                    filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                    filList[1] = new TypedValue(8, "本宗界址线");
                    SelectionFilter filter = new SelectionFilter(filList);
                    Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

                    PromptSelectionResult res = ed.SelectAll(filter);
                    SelectionSet SS = res.Value;
                    if (SS == null)
                    {
                        MessageBox.Show("没有本宗界址线", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    ObjectId[] objectId = SS.GetObjectIds();
                    if (objectId.Length != 1)
                    {
                        MessageBox.Show("本宗界址线太多了", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    ACDBPolyline pline_jzx = (ACDBPolyline)trans.GetObject(objectId[0], ACDBOpenMode.ForWrite);
                    ResultBuffer resultBuffer = pline_jzx.XData;
                    if (resultBuffer == null)//无属性
                    {
                        MessageBox.Show("本宗界址线无属性", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    TypedValue[] tv2 = resultBuffer.AsArray();
                    TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "ZDTDY");
                    if (tv3 == null)
                    {
                        MessageBox.Show("本宗界址线无属性", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    string xz = tv3[8].Value.ToString();
                    int TXBLC = Convert.ToInt32(tv3[3].Value);
                    ATT.BLCH = TXBLC;

                    #endregion
                    if (xz == "宗地图")
                    {
                        MessageBox.Show("此图本身就是宗地图，不需转换", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        trans.Commit();
                        trans.Dispose();
                        return;
                    }
                    if (xz == "宗地草图")
                    {

                        #region 修改扩展属性
                        tv3[8] = new TypedValue(1000, "宗地图");
                        RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                        regAppTableRecord.Name = "ZDTDY";
                        if (!symbolTable.Has("ZDTDY"))
                        {
                            symbolTable.Add(regAppTableRecord);
                            trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                        }
                        if (!pline_jzx.IsWriteEnabled)
                        {
                            pline_jzx.UpgradeOpen();
                        }
                        pline_jzx.XData = new ResultBuffer(tv3);
                        #endregion
                        #region 删除对象
                        ACDBPolyline TKX = new ACDBPolyline();
                        TypedValue[] filList2 = new TypedValue[1];
                        filList2[0] = new TypedValue(8, "*");
                        SelectionFilter filter2 = new SelectionFilter(filList2);
                        PromptSelectionResult res2 = ed.SelectAll(filter2);
                        SelectionSet SS2 = res2.Value;
                        if (SS2 == null)
                        {
                            MessageBox.Show("没有对象", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        ObjectId[] objectId2 = SS2.GetObjectIds();
                        for (int i = 0; i < objectId2.Length; i++)
                        {
                            Entity entity = (Entity)trans.GetObject(objectId2[i], ACDBOpenMode.ForWrite);
                            if (entity.Layer == "整饰" || entity.Layer == "宗地草图注记")
                            {
                                entity.Erase();
                            }
                            if (entity.Layer == "图廓线")
                            {
                                TKX = (ACDBPolyline)entity;
                            }
                        }
                        #endregion
                        #region 绘横线
                        double[] D1 = TKX.GetPoint2dAt(0).ToArray();
                        double[] D2 = TKX.GetPoint2dAt(1).ToArray();
                        double[] D3 = TKX.GetPoint2dAt(2).ToArray();
                        double[] D4 = TKX.GetPoint2dAt(3).ToArray();


                        double[] Dpoint1 = new double[3];
                        double[] Dpoint2 = new double[3];
                        Dpoint1[0] = D1[0];
                        Dpoint1[1] = D2[1] - ATT.BLCH * 15.0 / 1000.0;
                        Dpoint2[0] = D3[0];
                        Dpoint2[1] = Dpoint1[1];


                        Point2d P1 = TKX.GetPoint2dAt(0);
                        Point2d P2 = TKX.GetPoint2dAt(2);
                        Tools.CADTools.AddNewLayer("图廓线2", "Continuous", 7);
                        ACDBPolyline tkx2 = new ACDBPolyline();
                        tkx2.AddVertexAt(0, new Point2d(Dpoint1), 0, 0, 0);
                        tkx2.AddVertexAt(1, new Point2d(Dpoint2), 0, 0, 0);
                        tkx2.Closed = true;
                        Tools.CADTools.AddNewLayer("图廓线2", "Continuous", 7);
                        tkx2.Layer = "图廓线2";
                        tkx2.ColorIndex = 7;
                        tkx2.Linetype = "Continuous";
                        btr.AppendEntity(tkx2);
                        trans.AddNewlyCreatedDBObject(tkx2, true);
                        #endregion
                        #region 绘制其他整饰



                        double[] Dtext = new double[3];
                        Dtext[0] = (D2[0] + D3[0]) / 2.0;
                        Dtext[1] = D2[1] + ATT.BLCH * 5.5 / 1000.0;

                        MText mtext5 = new MText();
                        mtext5.Contents = "宗 地 图";
                        mtext5.TextHeight = ATT.BLCH * 4.5 / 1000.0;
                        mtext5.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext5.Location = new Point3d(Dtext);
                        mtext5.ColorIndex = 7;
                        mtext5.Attachment = AttachmentPoint.MiddleCenter;
                        mtext5.LayerId = Tools.CADTools.AddNewLayer("宗地图注记", "Continuous", 7);
                        btr.AppendEntity(mtext5);
                        trans.AddNewlyCreatedDBObject(mtext5, true);
                        Dtext[0] = D3[0];
                        Dtext[1] = D2[1] + ATT.BLCH * 4.0 / 1000.0;
                        MText mtext6 = new MText();
                        mtext6.Contents = "单位:m";
                        mtext6.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                        mtext6.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext6.Location = new Point3d(Dtext);
                        mtext6.ColorIndex = 7;
                        mtext6.Attachment = AttachmentPoint.MiddleRight;
                        mtext6.LayerId = Tools.CADTools.AddNewLayer("单位注记", "Continuous", 7);
                        btr.AppendEntity(mtext6);
                        trans.AddNewlyCreatedDBObject(mtext6, true);


                        Dtext[0] = D2[0] + ATT.BLCH * 10.0 / 1000.0;
                        Dtext[1] = D2[1] - ATT.BLCH * 7.5 / 1000.0;
                        MText mtext7 = new MText();
                        string zj1 = "宗地代码:" + row["WZDJH"].ToString().Trim() + "\r\n";
                        zj1 = zj1 + "所在图幅号:" + row["TFH"].ToString().Trim();
                        mtext7.Contents = zj1;
                        mtext7.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                        mtext7.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext7.Location = new Point3d(Dtext);
                        mtext7.ColorIndex = 7;
                        mtext7.Attachment = AttachmentPoint.MiddleLeft;
                        mtext7.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext7);
                        trans.AddNewlyCreatedDBObject(mtext7, true);

                        Dtext[0] = (D2[0] + D3[0]) / 2.0 + ATT.BLCH * 10.0 / 1000.0;
                        Dtext[1] = D2[1] - ATT.BLCH * 7.5 / 1000.0;
                        MText mtext8 = new MText();
                        string zj2 = "土地权利人:" + row["QLR"].ToString().Trim() + "\r\n";
                        double zdmj2 = Convert.ToDouble(ZDMJ.Text.Trim());
                        double pzmj = Convert.ToDouble(FZMJ.Text.Trim());
                        if (pzmj < 0.00001)
                        {
                            MessageBox.Show("发证面积没有填写", "重要提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        zj2 = zj2 + "宗地面积:" + zdmj2.ToString("0.00") + "  " + "发证面积:" + pzmj.ToString("0.00");
                        mtext8.Contents = zj2;
                        mtext8.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                        mtext8.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext8.Location = new Point3d(Dtext);
                        mtext8.ColorIndex = 7;
                        mtext8.Attachment = AttachmentPoint.MiddleLeft;
                        mtext8.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext8);
                        trans.AddNewlyCreatedDBObject(mtext8, true);

                        Dtext[0] = D1[0];
                        Dtext[1] = D1[1] - ATT.BLCH * 2.5 / 1000.0;
                        MText mtext11 = new MText();
                        string zj4 = "";
                        if (CLRQ.Text.ToString().Trim() != "")
                        {
                            zj4 = CLRQ.Text.ToString().Trim() + "图解法测绘界址点" + "\r\n";
                        }
                        else
                        {
                            zj4 = "    年  月  日" + "图解法测绘界址点" + "\r\n";
                        }
                        zj4 = zj4 + "制图日期:" + SCRQ.Text.ToString().Trim() + "\r\n";
                        zj4 = zj4 + "审核日期:" + SHRQ.Text.ToString().Trim() + "\r\n";
                        mtext11.Contents = zj4;
                        mtext11.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                        mtext11.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext11.Location = new Point3d(Dtext);
                        mtext11.ColorIndex = 7;
                        mtext11.Attachment = AttachmentPoint.TopLeft;
                        mtext11.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext11);
                        trans.AddNewlyCreatedDBObject(mtext11, true);


                        Dtext[0] = (D2[0] + D3[0]) / 2.0;
                        Dtext[1] = D1[1] - ATT.BLCH * 4.0 / 1000.0;
                        MText mtext10 = new MText();
                        mtext10.Contents = "1:" + ATT.BLCH.ToString();
                        mtext10.TextHeight = ATT.BLCH * 3.0 / 1000.0;
                        mtext10.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext10.Location = new Point3d(Dtext);
                        mtext10.ColorIndex = 7;
                        mtext10.Attachment = AttachmentPoint.MiddleCenter;
                        mtext10.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext10);
                        trans.AddNewlyCreatedDBObject(mtext10, true);

                        Dtext[0] = D4[0] - ATT.BLCH * 20 / 1000.0;
                        Dtext[1] = D4[1] - ATT.BLCH * 2.5 / 1000.0;
                        MText mtext12 = new MText();
                        string zj5 = "制图者:" + HTY.Text.Trim() + "\r\n";
                        zj5 = zj5 + "审核者:" + SHRXM.Text.Trim();
                        mtext12.Contents = zj5;
                        mtext12.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                        mtext12.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext12.Location = new Point3d(Dtext);
                        mtext12.ColorIndex = 7;
                        mtext12.Attachment = AttachmentPoint.TopLeft;
                        mtext12.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext12);
                        trans.AddNewlyCreatedDBObject(mtext12, true);
                        #endregion
                    }
                    trans.Commit();
                    trans.Dispose();

                    #region 创建宗地图路径、获取宗地图完整路径
                    string ZDT_Path1 = Tools.ReadWriteReg.read_reg("宗地图存放路径");
                    string ts = "“宗地图存放路径”没有设置";
                    DirectoryInfo mydir = new DirectoryInfo(ZDT_Path1);
                    if (!mydir.Exists)
                    {
                        MessageBox.Show(ts, "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    string ZDT_Path2 = ZDT_Path1 + "\\" + ATT.DJH.Substring(6, 3) + "\\" + ATT.DJH.Substring(9, 3) + "\\";
                    DirectoryInfo mydir2 = new DirectoryInfo(ZDT_Path2);
                    if (!mydir2.Exists)
                    {
                        mydir2.Create();
                    }
                    zdtfile = ZDT_Path2 + ATT.DJH + ".dwg";//宗地图文件名

                    ATT.ZDT_File = zdtfile;
                    FileInfo fileInfo = new FileInfo(zdtfile);
                    if (fileInfo.Exists)
                    {
                        if (DialogResult.Yes != MessageBox.Show("该宗地图已经存在，是否重新生成?", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        {
                            return;
                        }
                    }
                    db.SaveAs(zdtfile, DwgVersion.Current);
                    #endregion
                }
            }
            AcadApplication.DocumentManager.MdiActiveDocument.CloseAndDiscard();
            ATT.Document_ZDT_SC = AcadApplication.DocumentManager.Open(zdtfile, false);//打开宗地图
        }

        // 图表输出 - “宗转草”按钮
        private void button32_Click(object sender, EventArgs e)
        {
            #region 保存当前文件

            string file = AcadApplication.DocumentManager.MdiActiveDocument.Database.Filename;
            AcadApplication.DocumentManager.MdiActiveDocument.CloseAndSave(file);
            ATT.Document_ZDT_SC = AcadApplication.DocumentManager.Open(file);
            #endregion
            string zdtfile = "";
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                Database db = HostApplicationServices.WorkingDatabase;
                DataRow row = TB_DCB.Rows[0];
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    SymbolTable symbolTable = (SymbolTable)trans.GetObject(db.RegAppTableId, ACDBOpenMode.ForWrite);
                    #region 查看图形是宗地图还是宗地草图
                    TypedValue[] filList = new TypedValue[2];
                    filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                    filList[1] = new TypedValue(8, "本宗界址线");
                    SelectionFilter filter = new SelectionFilter(filList);
                    Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

                    PromptSelectionResult res = ed.SelectAll(filter);
                    SelectionSet SS = res.Value;
                    if (SS == null)
                    {
                        MessageBox.Show("没有本宗界址线", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    ObjectId[] objectId = SS.GetObjectIds();
                    if (objectId.Length != 1)
                    {
                        MessageBox.Show("本宗界址线太多了", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    ACDBPolyline pline_jzx = (ACDBPolyline)trans.GetObject(objectId[0], ACDBOpenMode.ForWrite);
                    ResultBuffer resultBuffer = pline_jzx.XData;
                    if (resultBuffer == null)//无属性
                    {
                        MessageBox.Show("本宗界址线无属性", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    TypedValue[] tv2 = resultBuffer.AsArray();
                    TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "ZDTDY");
                    if (tv3 == null)
                    {
                        MessageBox.Show("本宗界址线无属性", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    string xz = tv3[8].Value.ToString();
                    int TXBLC = Convert.ToInt32(tv3[3].Value);
                    ATT.BLCH = TXBLC;

                    #endregion
                    if (xz == "宗地草图")
                    {
                        MessageBox.Show("此图本身就是宗地草图，不需转换", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        trans.Commit();
                        trans.Dispose();
                        return;
                    }
                    if (xz == "宗地图")
                    {
                        #region 修改扩展属性
                        tv3[8] = new TypedValue(1000, "宗地草图");
                        RegAppTableRecord regAppTableRecord = new RegAppTableRecord();
                        regAppTableRecord.Name = "ZDTDY";
                        if (!symbolTable.Has("ZDTDY"))
                        {
                            symbolTable.Add(regAppTableRecord);
                            trans.AddNewlyCreatedDBObject(regAppTableRecord, true);
                        }
                        if (!pline_jzx.IsWriteEnabled)
                        {
                            pline_jzx.UpgradeOpen();
                        }
                        pline_jzx.XData = new ResultBuffer(tv3);
                        #endregion
                        #region 删除宗地图中不需要的内容
                        ACDBPolyline TKX = new ACDBPolyline();
                        TypedValue[] filList2 = new TypedValue[1];
                        filList2[0] = new TypedValue(8, "*");
                        SelectionFilter filter2 = new SelectionFilter(filList2);
                        PromptSelectionResult res2 = ed.SelectAll(filter2);
                        SelectionSet SS2 = res2.Value;
                        if (SS2 == null)
                        {
                            MessageBox.Show("没有对象", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        ObjectId[] objectId2 = SS2.GetObjectIds();
                        for (int i = 0; i < objectId2.Length; i++)
                        {
                            Entity entity = (Entity)trans.GetObject(objectId2[i], ACDBOpenMode.ForWrite);
                            if (entity.Layer == "整饰" || entity.Layer == "宗地图注记" || entity.Layer == "图廓线2" || entity.Layer == "单位注记" || entity.Layer == "阴影线")
                            {
                                entity.Erase();
                            }
                            if (entity.Layer == "图廓线")
                            {
                                TKX = (ACDBPolyline)entity;
                            }
                        }
                        #endregion
                        #region 绘制整饰
                        double[] D1 = TKX.GetPoint2dAt(0).ToArray();
                        double[] D2 = TKX.GetPoint2dAt(1).ToArray();
                        double[] D3 = TKX.GetPoint2dAt(2).ToArray();
                        double[] D4 = TKX.GetPoint2dAt(3).ToArray();
                        double[] Dtext = new double[3];
                        Dtext[0] = (D2[0] + D3[0]) / 2.0;
                        Dtext[1] = D2[1] - ATT.BLCH * 12.0 / 1000.0;
                        MText mtext51 = new MText();
                        mtext51.Contents = "宗地草图";
                        mtext51.TextHeight = ATT.BLCH * 4.5 / 1000.0;
                        mtext51.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext51.Location = new Point3d(Dtext);
                        mtext51.ColorIndex = 7;
                        mtext51.Attachment = AttachmentPoint.MiddleCenter;
                        mtext51.LayerId = Tools.CADTools.AddNewLayer("宗地草图注记", "Continuous", 7);
                        btr.AppendEntity(mtext51);
                        trans.AddNewlyCreatedDBObject(mtext51, true);

                        Dtext[0] = D1[0];
                        Dtext[1] = D1[1] - ATT.BLCH * 2.5 / 1000.0;
                        MText mtext22 = new MText();
                        string zj5 = "丈量者:" + DCYXM.Text.Trim().PadRight(6, ' ') + "\r\n";
                        zj5 = zj5 + "检查者:" + SHRXM.Text.Trim().PadRight(6, ' ');
                        mtext22.Contents = zj5;
                        mtext22.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                        mtext22.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext22.Location = new Point3d(Dtext);
                        mtext22.ColorIndex = 7;
                        mtext22.Attachment = AttachmentPoint.TopLeft;
                        mtext22.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext22);
                        trans.AddNewlyCreatedDBObject(mtext22, true);

                        Dtext[0] = D4[0] - ATT.BLCH * 20 / 1000.0;
                        Dtext[1] = D4[1] - ATT.BLCH * 2.5 / 1000.0;
                        MText mtext42 = new MText();
                        zj5 = "丈量日期:" + DCRQ.Text.PadRight(14, ' ') + "\r\n";
                        zj5 = zj5 + "检查日期:" + SHRQ.Text.PadRight(14, ' ');
                        mtext42.Contents = zj5;
                        mtext42.TextHeight = ATT.BLCH * 2.0 / 1000.0;
                        mtext42.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext42.Location = new Point3d(Dtext);
                        mtext42.ColorIndex = 7;
                        mtext42.Attachment = AttachmentPoint.TopRight;
                        mtext42.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext42);
                        trans.AddNewlyCreatedDBObject(mtext42, true);

                        Dtext[0] = (D2[0] + D3[0]) / 2.0;
                        Dtext[1] = D1[1] - ATT.BLCH * 3.5 / 1000.0;
                        MText mtext10 = new MText();
                        mtext10.Contents = "概略比例尺  1:" + ATT.BLCH.ToString();
                        mtext10.TextHeight = ATT.BLCH * 2.5 / 1000.0;
                        mtext10.TextStyleId = Tools.CADTools.AddNewTextStyle("黑体", "黑体", 1, 0, false);
                        mtext10.Location = new Point3d(Dtext);
                        mtext10.ColorIndex = 7;
                        mtext10.Attachment = AttachmentPoint.MiddleCenter;
                        mtext10.LayerId = Tools.CADTools.AddNewLayer("整饰", "Continuous", 7);
                        btr.AppendEntity(mtext10);
                        trans.AddNewlyCreatedDBObject(mtext10, true);
                        #endregion
                    }
                    trans.Commit();
                    trans.Dispose();
                    #region 创建宗地图路径、获取宗地图完整路径
                    string ZDT_Path1 = Tools.ReadWriteReg.read_reg("宗地草图存放路径");
                    string ts = "“宗地草图存放路径”没有设置";
                    DirectoryInfo mydir = new DirectoryInfo(ZDT_Path1);
                    if (!mydir.Exists)
                    {
                        MessageBox.Show(ts, "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    string ZDT_Path2 = ZDT_Path1 + "\\" + ATT.DJH.Substring(6, 3) + "\\" + ATT.DJH.Substring(9, 3) + "\\";
                    DirectoryInfo mydir2 = new DirectoryInfo(ZDT_Path2);
                    if (!mydir2.Exists)
                    {
                        mydir2.Create();
                    }
                    zdtfile = ZDT_Path2 + ATT.DJH + "-草图.dwg";//宗地图文件名
                    ATT.ZDT_File = zdtfile;
                    FileInfo fileInfo = new FileInfo(zdtfile);
                    if (fileInfo.Exists)
                    {
                        if (DialogResult.Yes != MessageBox.Show("该宗地草图已经存在，是否重新生成?", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        {
                            return;
                        }
                    }
                    db.SaveAs(zdtfile, DwgVersion.Current);
                    #endregion
                }
            }
            AcadApplication.DocumentManager.MdiActiveDocument.CloseAndDiscard();
            ATT.Document_ZDT_SC = AcadApplication.DocumentManager.Open(zdtfile, false);//打开宗地草图
        }

        // 图表输出 - “标注距离”按钮
        private void button12_Click_1(object sender, EventArgs e)
        {// TODO:检查是否有错误
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                ObjectId zt2 = Tools.CADTools.AddNewTextStyle("宋体", "宋体", 1, 0, false);
                Database db = HostApplicationServices.WorkingDatabase;
                Tools.CADTools.AddNewLayer("距离标注线", "Continuous", 7);
                Tools.CADTools.AddNewLayer("距离标注", "Continuous", 7);
                Tools.CADTools.AddNewLinetype("X32", Tools.CADTools.GetAppPath() + "ACADISO.LIN");
                ObjectId plid = Tools.CADTools.CreatPolyline("距离标注线", "X32", 0, 7, 0, 0, 1);
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    ACDBPolyline pline = (ACDBPolyline)trans.GetObject(plid, ACDBOpenMode.ForWrite);
                    for (int i = 0; i < pline.NumberOfVertices - 1; i++)
                    {
                        Point2d p1 = pline.GetPoint2dAt(i);
                        Point2d p2 = pline.GetPoint2dAt(i + 1);
                        double jjdd = Tools.CADTools.GetAngle(p1, p2) + Math.PI * 1.0 / 2.0;
                        Point3d zjd = new Point3d((p1.X + p2.X) / 2.0, (p1.Y + p2.Y) / 2.0, 0);
                        Point3d bczjd = Tools.CADTools.GetNewPoint(zjd, 1.7 * ATT.BLCH / 1000.0, jjdd);
                        double jl2 = p1.GetDistanceTo(p2);
                        double wzjd = Tools.CADTools.GetAngle(p1, p2);//文字旋转角度
                        if (wzjd > Math.PI * 2.0)
                        {
                            wzjd = wzjd - Math.PI * 2.0;
                        }
                        if (wzjd < 0)
                        {
                            wzjd = wzjd + Math.PI * 2.0;
                        }
                        if ((wzjd >= Math.PI / 4.0 && wzjd < Math.PI * 3.0 / 4.0) || (wzjd >= Math.PI * 5.0 / 4.0 && wzjd < Math.PI * 7.0 / 4.0))
                        {
                            wzjd = wzjd + Math.PI;
                        }
                        if (wzjd > Math.PI * 2.0)
                        {
                            wzjd = wzjd - Math.PI * 2.0;
                        }
                        if (wzjd > Math.PI * 0.5 && wzjd < Math.PI * 3.0 / 2.0)
                        {
                            wzjd = wzjd + Math.PI;
                        }
                        scText(jl2.ToString("0.00"), bczjd, ATT.BLCH * 2.0 / 1000.0, 7, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, "距离标注", zt2, 0.8, wzjd);
                    }
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }

        // “刷新签章表”按钮
        private void button10_Click(object sender, EventArgs e)
        {
            save();
            if (treeView1.SelectedNode.Level == 4)
            {
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                string JDH = XZDM.Substring(6, 3);
                string JFH = XZDM.Substring(9, 3);
                string XZQH = XZDM.Substring(0, 6);
                string connectionString = Tools.Uitl.LJstring();//写连接字符串
                string selstring = "SELECT * FROM DCB where DJH like '" + XZDM + "%%%%%'";
                if (Tools.DataBasic.Create_DCB_table(connectionString, "DCB", Tools.DataBasicString.Create_DCB))//如果”DCB“不存在则创建DCB
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlDataAdapter Adapter_TB = Tools.DataBasic.DCB_Initadapter(selstring, connectionString);
                        DataSet dataset_DCB = new DataSet();
                        Adapter_TB.Fill(dataset_DCB, "DCB");//填充Dataset
                        for (int i = 0; i < dataset_DCB.Tables["DCB"].Rows.Count; i++)//循环调查表
                        {
                            DataRow dcbRow = dataset_DCB.Tables["DCB"].Rows[i];
                            string dcbdjh = dcbRow["DJH"].ToString().Trim();//调查表上的地籍号
                            string selstring_ZDX = "SELECT * FROM D_ZDX WHERE (DJH = '" + dcbdjh + "')";//准备搜索界址签章表
                            SqlDataAdapter Adapter_ZDX = Tools.DataBasic.D_ZDX_Initadapter(selstring_ZDX, connectionString);
                            DataSet dataset_ZDX = new DataSet();
                            Adapter_ZDX.Fill(dataset_ZDX, "D_ZDX");//填充Dataset
                            for (int j = 0; j < dataset_ZDX.Tables["D_ZDX"].Rows.Count; j++)//循环界址签章表
                            {
                                DataRow zdxRow = dataset_ZDX.Tables["D_ZDX"].Rows[j];
                                string Lzdjh = zdxRow["LZDDJH"].ToString().Trim();
                                string yzjr = zdxRow["LZDZJRXM"].ToString().Trim();//指界表中的指界人
                                string yqlr = zdxRow["BZ"].ToString().Trim();//指界表中的权利人
                                if (Lzdjh != "")
                                {
                                    string selconne = "SELECT * FROM DCB WHERE (DJH = '" + Lzdjh + "')";
                                    SqlCommand command = new SqlCommand(selconne, connection);//连接数据库
                                    SqlDataReader reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        string bzdzjr = reader["FRDBXM"].ToString().Trim();
                                        string dlr = reader["JBB_DLRXM"].ToString().Trim();
                                        string zjr = "";//根据调查表得到的指界人
                                        string qlr = "";//根据调查表得到的权利人
                                        if (dlr != "")
                                        {
                                            zjr = dlr;
                                        }
                                        else
                                        {
                                            zjr = bzdzjr;
                                        }
                                        qlr = reader["QLR"].ToString().Trim();

                                        if (yzjr == zjr && yqlr == qlr)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            #region 替换四至
                                            if (yqlr != qlr)
                                            {
                                                zdxRow["LZDZJRXM"] = zjr;
                                                zdxRow["BZ"] = qlr;
                                                if (qlr == "")
                                                {
                                                    int hh = Convert.ToInt32(Lzdjh.Substring(12, 5));
                                                    qlr = hh.ToString() + "号宗地";
                                                }
                                                if (yqlr == "")
                                                {
                                                    int hh = Convert.ToInt32(Lzdjh.Substring(12, 5));
                                                    yqlr = hh.ToString() + "号宗地";
                                                }
                                                int nnn = j + 1;
                                                if (nnn >= dataset_ZDX.Tables["D_ZDX"].Rows.Count)
                                                {
                                                    nnn = 0;
                                                }
                                                DataRow zdxRow_xyg = dataset_ZDX.Tables["D_ZDX"].Rows[nnn];//下一行记录
                                                qlr = zdxRow["MBBSM"].ToString().Trim() + "-" + zdxRow_xyg["MBBSM"].ToString().Trim() + ":" + qlr;
                                                yqlr = zdxRow["MBBSM"].ToString().Trim() + "-" + zdxRow_xyg["MBBSM"].ToString().Trim() + ":" + yqlr;
                                                string bh = zdxRow["MBBSM"].ToString().Trim() + "-" + zdxRow_xyg["MBBSM"].ToString().Trim() + ":";//标号
                                                string bz = dcbRow["BZ"].ToString().Trim();
                                                string dz = dcbRow["DZ"].ToString().Trim();
                                                string nz = dcbRow["NZ"].ToString().Trim();
                                                string xz = dcbRow["XZ"].ToString().Trim();
                                                string fh = ";";
                                                if (bz.IndexOf(bh) >= 0)
                                                {
                                                    string[] aa = bz.Split(';');
                                                    bz = "";
                                                    for (int p = 0; p < aa.Length; p++)
                                                    {
                                                        if (p == aa.Length - 1)
                                                        {
                                                            fh = "";
                                                        }
                                                        if (aa[p].IndexOf(bh) >= 0)
                                                        {
                                                            aa[p] = qlr;
                                                        }
                                                        bz = bz + aa[p] + fh;
                                                    }
                                                    dcbRow["BZ"] = bz;
                                                }
                                                if (dz.IndexOf(bh) >= 0)
                                                {
                                                    string[] aa = dz.Split(';');
                                                    bz = "";
                                                    for (int p = 0; p < aa.Length; p++)
                                                    {
                                                        if (p == aa.Length - 1)
                                                        {
                                                            fh = "";
                                                        }
                                                        if (aa[p].IndexOf(bh) >= 0)
                                                        {
                                                            aa[p] = qlr;
                                                        }
                                                        bz = bz + aa[p] + fh;
                                                    }
                                                    dcbRow["DZ"] = bz;
                                                }
                                                if (nz.IndexOf(bh) >= 0)
                                                {
                                                    string[] aa = nz.Split(';');
                                                    bz = "";
                                                    for (int p = 0; p < aa.Length; p++)
                                                    {
                                                        if (p == aa.Length - 1)
                                                        {
                                                            fh = "";
                                                        }
                                                        if (aa[p].IndexOf(bh) >= 0)
                                                        {
                                                            aa[p] = qlr;
                                                        }
                                                        bz = bz + aa[p] + fh;
                                                    }
                                                    dcbRow["NZ"] = bz;
                                                }
                                                if (xz.IndexOf(bh) >= 0)
                                                {
                                                    string[] aa = xz.Split(';');
                                                    bz = "";
                                                    for (int p = 0; p < aa.Length; p++)
                                                    {
                                                        if (p == aa.Length - 1)
                                                        {
                                                            fh = "";
                                                        }
                                                        if (aa[p].IndexOf(bh) >= 0)
                                                        {
                                                            aa[p] = qlr;
                                                        }
                                                        bz = bz + aa[p] + fh;
                                                    }
                                                    dcbRow["XZ"] = bz;
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    reader.Close();
                                }
                            }
                            #region 保存界址签章表
                            try
                            {
                                Adapter_ZDX.Update(dataset_ZDX, "D_ZDX");
                            }
                            catch (System.Exception ee)
                            {
                                MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                            #endregion
                        }
                        #region 保存调查表
                        try
                        {
                            Adapter_TB.Update(dataset_DCB, "DCB");
                        }
                        catch (System.Exception ee)
                        {
                            MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        #endregion
                        connection.Close();
                        MessageBox.Show("宗地四至及界址签章表修改完毕！", "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        // “库属性回填”按钮
        private void button11_Click(object sender, EventArgs e)
        {
            #region 条件检查、数据准备
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
            if (DialogResult.No == MessageBox.Show("注意：本功能只将数据库中的权利人名称、宗地号和地类代码回填到图形，其他属性一律不予修改。是否继续？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return;
            }
            string XZDM = treeView1.SelectedNode.Tag.ToString();
            string JDH = XZDM.Substring(6, 3);
            string JFH = XZDM.Substring(9, 3);
            string XZQH = XZDM.Substring(0, 6);
            string connectionString = Tools.Uitl.LJstring();
            #endregion
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
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        for (int i = 0; i < objectId.Length; i++)
                        {
                            ACDBPolyline pline_jzx = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                            try
                            {
                                ResultBuffer resultBuffer = pline_jzx.XData;
                                TypedValue[] tv2 = resultBuffer.AsArray();
                                TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                                string txdjh = tv3[2].Value.ToString().Trim();
                                txdjh = XZQH + txdjh.Substring(0, 6) + txdjh.Substring(6).PadLeft(5, '0');
                                string selconne = "SELECT * FROM DCB WHERE (DJH = '" + txdjh + "')";
                                string dlbm = "";
                                string qlr = "";
                                string wzzdh = "";
                                bool iszd = false;//是否找到了
                                SqlCommand command = new SqlCommand(selconne, connection);//连接数据库
                                SqlDataReader reader = command.ExecuteReader();
                                while (reader.Read())
                                {
                                    qlr = reader["QLR"].ToString().Trim();
                                    dlbm = reader["DLBM"].ToString().Trim();
                                    string wzdjh = reader["DJH"].ToString().Trim();
                                    wzzdh = wzdjh.Substring(6, 11);
                                    if (dlbm == "")
                                    {
                                        MessageBox.Show(wzzdh + "宗地没有土地权属类型码", "ACAD", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        continue;
                                    }

                                    iszd = true;
                                }
                                reader.Close();

                                if (iszd == false)
                                {
                                    MessageBox.Show("数据库中没有发现 " + txdjh + " 宗地!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    continue;
                                }
                                tv3[4] = new TypedValue(1000, dlbm);
                                tv3[3] = new TypedValue(1000, qlr);
                                tv3[2] = new TypedValue(1000, wzzdh);
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
                    }
                    trans.Commit();
                    trans.Dispose();
                    MessageBox.Show("属性回填完成。", "友情提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        // 根据界址点号查找界址点序号
        /// <summary>
        /// 根据界址点号查找界址点序号
        /// </summary>
        /// <param name="dataset_ZDD"></param>
        /// <param name="jzdh"></param>
        /// <returns></returns>
        public string getjzdxh(DataSet dataset_ZDD, int jzdh)
        {
            string xh = "";
            for (int i = 0; i < dataset_ZDD.Tables["D_ZDD"].Rows.Count; i++)
            {
                if ((int)dataset_ZDD.Tables["D_ZDD"].Rows[i]["JZDH"] == jzdh)
                {
                    xh = "J" + dataset_ZDD.Tables["D_ZDD"].Rows[i]["MBBSM"].ToString();
                    break;
                }
            }
            return xh;
        }

        // 打印、预览调查表
        /// <summary>
        /// 打印、预览调查表
        /// </summary>
        public void X_DCB(int i)
        {
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string wzdjh = reader["WZDJH"].ToString().Trim();
                        #region 地籍调查表封面
                        string flxfile1 = Tools.CADTools.GetReportsFolder() + "不动产权籍调查表封面.flx";
                        grid1.OpenFile(flxfile1);
                        grid1.Cell(7, 3).Text = wzdjh;//宗地代码
                        grid1.Cell(8, 3).Text = Tools.ReadWriteReg.read_reg("测量单位全称");
                        //grid1.Cell(1, 7).Text = ATT.DJH.Substring(6, 11);//编号

                        string qlr = reader["QLR"].ToString().Trim();
                        //grid1.Cell(20, 4).Text = qlr;//权利人
                        string dcbrq = reader["DCBRQ"].ToString().Trim();
                        if (dcbrq == "")
                        {
                            grid1.Cell(10, 1).Text = "调查时间：  年  月  日";
                        }
                        else
                        {
                            grid1.Cell(10, 1).Text = "调查时间：" + dcbrq;
                        }
                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion
                        #region 基本表
                        string flxfilel2 = Tools.CADTools.GetReportsFolder() + "调查表-基本表.flx";
                        grid1.OpenFile(flxfilel2);

                        //所有权
                        if (reader["TEXTBZ1"].ToString().Trim() == "")
                        {
                            grid1.Cell(2, 3).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(2, 3).Text = reader["TEXTBZ1"].ToString().Trim();
                        }
                        //使用权
                        if (reader["QLR"].ToString().Trim() == "")
                        {
                            grid1.Cell(3, 3).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 3).Text = reader["QLR"].ToString().Trim();
                        }
                        //权利人类型 
                        if (reader["DWXZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(3, 10).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 10).Text = reader["DWXZ"].ToString().Trim();
                        }
                        //证件种类	
                        if (reader["ZRXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(4, 10).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(4, 10).Text = reader["ZRXM"].ToString().Trim();
                        }
                        //证件号	
                        if (reader["KCR"].ToString().Trim() == "")
                        {
                            grid1.Cell(5, 10).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(5, 10).Text = reader["KCR"].ToString().Trim();
                        }
                        //通讯地址	
                        if (reader["TXDZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(6, 10).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(6, 10).Text = reader["TXDZ"].ToString().Trim();
                        }
                        //权利类型
                        if (reader["SYQLX"].ToString().Trim() == "")
                        {
                            grid1.Cell(7, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(7, 4).Text = reader["SYQLX"].ToString().Trim();
                        }
                        //权利性质		
                        if (reader["QSXZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(7, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(7, 8).Text = reader["QSXZ"].ToString().Trim();
                        }
                        //土地权属来源证明材料	
                        if (reader["SQDJLR"].ToString().Trim() == "")
                        {
                            grid1.Cell(7, 11).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(7, 11).Text = reader["SQDJLR"].ToString().Trim();
                        }
                        //坐落	
                        if (reader["TDZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(8, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(8, 4).Text = reader["TDZL"].ToString().Trim();
                        }
                        //法定代表人	
                        if (reader["FRDBXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(9, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(9, 4).Text = reader["FRDBXM"].ToString().Trim();
                        }
                        //法定代表人证件种类	
                        if (reader["FRDBZJZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(9, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(9, 8).Text = reader["FRDBZJZL"].ToString().Trim();
                        }
                        //法定代表人证件号	
                        if (reader["FRDBZJBH"].ToString().Trim() == "")
                        {
                            grid1.Cell(10, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(10, 8).Text = reader["FRDBZJBH"].ToString().Trim();
                        }
                        //法定代表人电话
                        if (reader["FRDBDH"].ToString().Trim() == "")
                        {
                            grid1.Cell(9, 12).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(9, 12).Text = reader["FRDBDH"].ToString().Trim();
                        }
                        //法定代表代理人姓名		
                        if (reader["JBB_DLRXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(11, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(11, 4).Text = reader["JBB_DLRXM"].ToString().Trim();
                        }
                        //法定代表代理人证件种类		
                        if (reader["JBB_DLRZJZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(11, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(11, 8).Text = reader["JBB_DLRZJZL"].ToString().Trim();
                        }
                        //法定代表代理人证件号	
                        if (reader["JBB_DLRZJBH"].ToString().Trim() == "")
                        {
                            grid1.Cell(12, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(12, 8).Text = reader["JBB_DLRZJBH"].ToString().Trim();
                        }
                        //法定代表代理人电话	
                        if (reader["JBB_DLRDH"].ToString().Trim() == "")
                        {
                            grid1.Cell(11, 12).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(11, 12).Text = reader["JBB_DLRDH"].ToString().Trim();
                        }
                        //权利设定方式		
                        if (reader["QLSLQJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(13, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(13, 4).Text = reader["QLSLQJ"].ToString().Trim();
                        }
                        //国民经济行业	
                        if (reader["GMJJHYDM"].ToString().Trim() == "")
                        {
                            grid1.Cell(14, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(14, 4).Text = reader["GMJJHYDM"].ToString().Trim();
                        }
                        //预编宗地代码		
                        if (reader["YBZDDM"].ToString().Trim() == "")
                        {
                            grid1.Cell(16, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 4).Text = reader["YBZDDM"].ToString().Trim();
                        }
                        //宗地代码	
                        if (reader["YBZDDM"].ToString().Trim() == "")
                        {
                            grid1.Cell(16, 10).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 10).Text = wzdjh;
                        }
                        //不动产单元号	
                        // wzdjh = reader["WZDJH"].ToString().Trim();
                        string BDCDYH = wzdjh.Substring(0, 6) + "  " + wzdjh.Substring(6, 6) + "  " +
                                          wzdjh.Substring(12, 7) + "  " + "F00010001";

                        grid1.Cell(17, 4).Text = BDCDYH;
                        //	比例尺分母	
                        if (Tools.ReadWriteReg.read_reg("比例尺分母") == "")
                        {
                            grid1.Cell(18, 6).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(18, 6).Text = "1:" + Tools.ReadWriteReg.read_reg("比例尺分母");
                        }
                        //图幅号
                        if (reader["TFH"].ToString().Trim() == "")
                        {
                            grid1.Cell(19, 6).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(19, 6).Text = reader["TFH"].ToString().Trim();
                        }
                        //北至
                        if (reader["BZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(20, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(20, 4).Text = "北：" + reader["BZ"].ToString().Trim();
                        }
                        //东至
                        if (reader["DZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(21, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(21, 4).Text = "东：" + reader["DZ"].ToString().Trim();
                        }
                        //南至
                        if (reader["NZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(22, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(22, 4).Text = "南：" + reader["NZ"].ToString().Trim();
                        }
                        //西至
                        if (reader["XZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(23, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(23, 4).Text = "西：" + reader["XZ"].ToString().Trim();
                        }
                        //等级
                        if (reader["DLJGDH2"].ToString().Trim() == "")
                        {
                            grid1.Cell(24, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(24, 4).Text = reader["DLJGDH2"].ToString().Trim();
                        }
                        //价格 元		
                        double jg = (double)reader["TDJG"];
                        if (jg < 0.001)
                        {
                            grid1.Cell(24, 11).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(24, 11).Text = jg.ToString("0.00");
                        }
                        //批准用途
                        if (reader["PZYT"].ToString().Trim() == "")
                        {
                            grid1.Cell(25, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(25, 4).Text = reader["PZYT"].ToString().Trim();
                        }
                        //地类代码(批准用途)
                        if (reader["PZYT_DLDM"].ToString().Trim() == "")
                        {
                            grid1.Cell(26, 6).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(26, 6).Text = reader["PZYT_DLDM"].ToString().Trim();
                        }
                        //实际用途
                        if (reader["SJYT"].ToString().Trim() == "")
                        {
                            grid1.Cell(25, 10).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(25, 10).Text = reader["SJYT"].ToString().Trim();
                        }
                        //实际用途地类编码
                        if (reader["DLBM"].ToString().Trim() == "")
                        {
                            grid1.Cell(26, 11).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(26, 11).Text = reader["DLBM"].ToString().Trim();
                        }
                        //批准面积
                        double fzmj = (double)reader["TDDYMJ"];
                        if (fzmj < 0.0001)
                        {
                            grid1.Cell(27, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(27, 4).Text = fzmj.ToString("0.00");
                        }
                        //宗地面积
                        double zdmj = (double)reader["ZDMJ"];
                        grid1.Cell(27, 8).Text = zdmj.ToString("0.00");
                        //建筑占地面积
                        double jzzdmj = (double)reader["JZZDMJ"];
                        grid1.Cell(27, 12).Text = jzzdmj.ToString("0.00");
                        //建筑总面积
                        double jzmj = (double)reader["JZMJ"];
                        grid1.Cell(28, 12).Text = jzmj.ToString("0.00");
                        //土地使用期限	
                        string syqxq = reader["SYQX_Q"].ToString().Trim();//使用期限(起）
                        string syqxz = reader["SYQX_Z"].ToString().Trim();//使用期限(止)                        
                        if (syqxq == "" || syqxz == "")
                        {
                            grid1.Cell(29, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(29, 4).Text = syqxq + " 起至 " + syqxz + " 止";
                        }
                        //共有/共用权利人情况
                        if (reader["GYQLRQK"].ToString().Trim() == "")
                        {
                            grid1.Cell(30, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(30, 4).Text = reader["GYQLRQK"].ToString().Trim();
                        }
                        //说明
                        if (reader["SM"].ToString().Trim() == "")
                        {
                            grid1.Cell(31, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(31, 4).Text = reader["SM"].ToString().Trim();
                        }

                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion
                        #region 审核表
                        string flxfilel3 = Tools.CADTools.GetReportsFolder() + "地籍调查审核表.flx";
                        grid1.OpenFile(flxfilel3);
                        grid1.Cell(3, 3).Text = reader["QSDCJS"].ToString();//权属调查记事
                        grid1.Cell(16, 4).Text = reader["DCYXM"].ToString().Trim();//调查员姓名
                        grid1.Cell(16, 7).Text = reader["DCRQ"].ToString().Trim();//调查日期

                        grid1.Cell(18, 3).Text = reader["DJKZJS"].ToString();//地籍测量记事
                        grid1.Cell(29, 4).Text = reader["CLYXM"].ToString().Trim();//测量员姓名
                        grid1.Cell(29, 7).Text = reader["CLRQ"].ToString().Trim();//测量日期

                        grid1.Cell(31, 3).Text = reader["DJDCJGSHYJ"].ToString().Trim();//地籍调查结果审核意见
                        grid1.Cell(45, 4).Text = reader["SHRXM"].ToString().Trim();//审核人姓名
                        grid1.Cell(45, 7).Text = reader["SHRQ"].ToString().Trim();//审核日期
                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion
                    }

                    #region 界址表示表
                    string selstring_ZDD = "SELECT * FROM D_ZDD WHERE (DJH = '" + ATT.DJH + "')";
                    SqlDataAdapter Adapter_ZDD = Tools.DataBasic.D_ZDD_Initadapter(selstring_ZDD, connectionString);
                    DataSet dataset_ZDD = new DataSet();
                    Adapter_ZDD.Fill(dataset_ZDD, "D_ZDD");//填充Dataset
                    string flxfilel4 = Tools.CADTools.GetReportsFolder() + "界址表示表.flx";
                    grid1.OpenFile(flxfilel4);
                    int zys = (int)(dataset_ZDD.Tables["D_ZDD"].Rows.Count / 33.0) + 1;//总页数
                    int dqys = 1;//当前页数
                    int zs = dataset_ZDD.Tables["D_ZDD"].Rows.Count;//总行数
                    int nn = 6;//当前在表格的行数
                    for (int j = 0; j < zs; j++)//循环调查表
                    {
                        if (dqys * 33 == j && j != zs - 1)
                        {
                            #region 加入最后一行


                            DataRow zddRow2 = dataset_ZDD.Tables["D_ZDD"].Rows[j];
                            //string XH = "(J" + zddRow2["MBBSM"].ToString() + ")";
                            //if (j == zs - 1)
                            //{
                            //    XH = "(J1)";
                            //}

                            //grid1.Cell(nn, 1).Text = "J" + zddRow2["JZDH"].ToString() + XH;
                            #region 输出界址点号
                            string XH = string.Empty;
                            switch (JzdOutputType)
                            {
                                case "仅宗内编号":
                                    XH = "J" + zddRow2["MBBSM"].ToString();
                                    if (j == zs - 1)
                                    {
                                        XH = "J1";
                                    }
                                    grid1.Cell(nn, 1).Text = XH;
                                    break;
                                case "仅子区内编号":
                                    grid1.Cell(nn, 1).Text = "J" + zddRow2["JZDH"].ToString();
                                    break;
                                case "子区和宗内编号":
                                    XH = "(J" + zddRow2["MBBSM"].ToString() + ")";
                                    if (j == zs - 1)
                                    {
                                        XH = "(J1)";
                                    }
                                    grid1.Cell(nn, 1).Text = "J" + zddRow2["JZDH"].ToString() + XH;
                                    break;
                                default:
                                    XH = "(J" + zddRow2["MBBSM"].ToString() + ")";
                                    if (j == zs - 1)
                                    {
                                        XH = "(J1)";
                                    }
                                    grid1.Cell(nn, 1).Text = "J" + zddRow2["JZDH"].ToString() + XH;
                                    break;
                            }
                            #endregion

                            #region 界标种类
                            string jbzl2 = zddRow2["JBZL"].ToString().Trim();
                            switch (jbzl2)
                            {
                                case "钢钉":
                                    grid1.Cell(nn, 3).Text = "√";
                                    break;
                                case "水泥桩":
                                    grid1.Cell(nn, 4).Text = "√";
                                    break;
                                case "喷涂":
                                    grid1.Cell(nn, 5).Text = "√";
                                    break;
                                case "其他":
                                    grid1.Cell(nn, 6).Text = "√";
                                    break;
                                case "无标志":
                                    grid1.Cell(nn, 7).Text = "√";
                                    break;
                                default:
                                    grid1.Cell(nn, 6).Text = "√";
                                    break;
                            }
                            #endregion

                            #region 界址线类别
                            switch (zddRow2["JZXLB"].ToString().Trim())
                            {
                                case "围墙":
                                    grid1.Cell(nn + 1, 10).Text = "√";
                                    break;
                                case "墙壁":
                                    grid1.Cell(nn + 1, 11).Text = "√";
                                    break;
                                case "栅栏":
                                    grid1.Cell(nn + 1, 12).Text = "√";
                                    break;
                                case "铁丝网":
                                    grid1.Cell(nn + 1, 13).Text = "√";
                                    break;
                                case "路涯线":
                                    grid1.Cell(nn + 1, 14).Text = "√";
                                    break;
                                case "空地":
                                    grid1.Cell(nn + 1, 15).Text = "√";
                                    break;
                                case "两点连线":
                                    grid1.Cell(nn + 1, 16).Text = "√";
                                    break;
                                case "其他":
                                    grid1.Cell(nn + 1, 17).Text = "√";
                                    break;
                                default:
                                    grid1.Cell(nn + 1, 17).Text = "√";
                                    break;
                            }
                            #endregion
                            #region 界址线位置
                            string jzxwz2 = zddRow2["JZXWZ"].ToString().Trim();
                            switch (jzxwz2)
                            {
                                case "内":
                                    grid1.Cell(nn + 1, 18).Text = "√";
                                    break;
                                case "中":
                                    grid1.Cell(nn + 1, 19).Text = "√";
                                    break;
                                case "外":
                                    grid1.Cell(nn + 1, 20).Text = "√";
                                    break;
                            }

                            #endregion
                            grid1.Cell(nn + 1, 21).Text = zddRow2["SM"].ToString().Trim();//说明


                            #endregion
                            #region 加入第一行
                            if (SCYL.Checked)
                            {
                                grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                            }
                            else
                            {
                                grid1.Print(1, 100, true);
                            }
                            grid1.OpenFile(flxfilel4);
                            nn = 6;
                            DataRow zddRow = dataset_ZDD.Tables["D_ZDD"].Rows[j];
                            //grid1.Cell(nn, 1).Text = "J" + zddRow["JZDH"].ToString() + XH;
                            #region 输出界址点号
                            switch (JzdOutputType)
                            {
                                case "仅宗内编号":
                                    grid1.Cell(nn, 1).Text = XH;
                                    break;
                                case "仅子区内编号":
                                    grid1.Cell(nn, 1).Text = "J" + zddRow["JZDH"].ToString();
                                    break;
                                case "子区和宗内编号":
                                    grid1.Cell(nn, 1).Text = "J" + zddRow["JZDH"].ToString() + XH;
                                    break;
                                default:
                                    grid1.Cell(nn, 1).Text = "J" + zddRow["JZDH"].ToString() + XH;
                                    break;
                            }
                            #endregion

                            #region 界标种类
                            string jbzl = zddRow["JBZL"].ToString().Trim();
                            switch (jbzl)
                            {
                                case "钢钉":
                                    grid1.Cell(nn, 3).Text = "√";
                                    break;
                                case "水泥桩":
                                    grid1.Cell(nn, 4).Text = "√";
                                    break;
                                case "喷涂":
                                    grid1.Cell(nn, 5).Text = "√";
                                    break;
                                case "其他":
                                    grid1.Cell(nn, 6).Text = "√";
                                    break;
                                case "无标志":
                                    grid1.Cell(nn, 7).Text = "√";
                                    break;
                                default:
                                    grid1.Cell(nn, 6).Text = "√";
                                    break;
                            }
                            #endregion
                            double jl = (double)zddRow["KZBC"];//界址间距
                            if (j != zs - 1)//如果不是最后一行
                            {
                                grid1.Cell(nn + 1, 8).Text = jl.ToString("0.00");
                            }
                            #region 界址线类别
                            switch (zddRow["JZXLB"].ToString().Trim())
                            {
                                case "围墙":
                                    grid1.Cell(nn + 1, 10).Text = "√";
                                    break;
                                case "墙壁":
                                    grid1.Cell(nn + 1, 11).Text = "√";
                                    break;
                                case "栅栏":
                                    grid1.Cell(nn + 1, 12).Text = "√";
                                    break;
                                case "铁丝网":
                                    grid1.Cell(nn + 1, 13).Text = "√";
                                    break;
                                case "路涯线":
                                    grid1.Cell(nn + 1, 14).Text = "√";
                                    break;
                                case "空地":
                                    grid1.Cell(nn + 1, 15).Text = "√";
                                    break;
                                case "两点连线":
                                    grid1.Cell(nn + 1, 16).Text = "√";
                                    break;
                                case "其他":
                                    grid1.Cell(nn, 17).Text = "√";
                                    break;
                                default:
                                    grid1.Cell(nn + 1, 17).Text = "√";
                                    break;
                            }
                            #endregion
                            #region 界址线位置
                            string jzxwz = zddRow["JZXWZ"].ToString().Trim();
                            switch (jzxwz)
                            {
                                case "内":
                                    grid1.Cell(nn + 1, 18).Text = "√";
                                    break;
                                case "中":
                                    grid1.Cell(nn + 1, 19).Text = "√";
                                    break;
                                case "外":
                                    grid1.Cell(nn + 1, 20).Text = "√";
                                    break;
                            }

                            #endregion
                            grid1.Cell(nn + 1, 21).Text = zddRow["SM"].ToString().Trim();//说明
                            #endregion
                            dqys++;
                            nn = nn + 2;
                        }
                        else
                        {
                            DataRow zddRow = dataset_ZDD.Tables["D_ZDD"].Rows[j];

                            #region 输出界址点号
                            string XH = string.Empty;
                            switch (JzdOutputType)
                            {
                                case "仅宗内编号":
                                    XH = "J" + zddRow["MBBSM"].ToString();
                                    if (j == zs - 1)
                                    {
                                        XH = "J1";
                                    }
                                    grid1.Cell(nn, 1).Text = XH;
                                    break;
                                case "仅子区内编号":
                                    grid1.Cell(nn, 1).Text = "J" + zddRow["JZDH"].ToString();
                                    break;
                                case "子区和宗内编号":
                                    XH = "(J" + zddRow["MBBSM"].ToString() + ")";
                                    if (j == zs - 1)
                                    {
                                        XH = "(J1)";
                                    }
                                    grid1.Cell(nn, 1).Text = "J" + zddRow["JZDH"].ToString() + XH;
                                    break;
                                default:
                                    XH = "(J" + zddRow["MBBSM"].ToString() + ")";
                                    if (j == zs - 1)
                                    {
                                        XH = "(J1)";
                                    }
                                    grid1.Cell(nn, 1).Text = "J" + zddRow["JZDH"].ToString() + XH;
                                    break;
                            }
                            #endregion

                            if (j != zs - 1)//最后一行
                            {
                                #region 界标种类
                                string jbzl = zddRow["JBZL"].ToString().Trim();
                                switch (jbzl)
                                {
                                    case "钢钉":
                                        grid1.Cell(nn, 3).Text = "√";
                                        break;
                                    case "水泥桩":
                                        grid1.Cell(nn, 4).Text = "√";
                                        break;
                                    case "喷涂":
                                        grid1.Cell(nn, 5).Text = "√";
                                        break;
                                    case "其他":
                                        grid1.Cell(nn, 6).Text = "√";
                                        break;
                                    case "无标志":
                                        grid1.Cell(nn, 7).Text = "√";
                                        break;
                                    default:
                                        grid1.Cell(nn, 6).Text = "√";
                                        break;
                                }
                                #endregion

                                double jl = (double)zddRow["KZBC"];//界址间距
                                grid1.Cell(nn + 1, 8).Text = jl.ToString("0.00");

                                #region 界址线类别
                                switch (zddRow["JZXLB"].ToString().Trim())
                                {
                                    case "围墙":
                                        grid1.Cell(nn + 1, 10).Text = "√";
                                        break;
                                    case "墙壁":
                                        grid1.Cell(nn + 1, 11).Text = "√";
                                        break;
                                    case "栅栏":
                                        grid1.Cell(nn + 1, 12).Text = "√";
                                        break;
                                    case "铁丝网":
                                        grid1.Cell(nn + 1, 13).Text = "√";
                                        break;
                                    case "路涯线":
                                        grid1.Cell(nn + 1, 14).Text = "√";
                                        break;
                                    case "空地":
                                        grid1.Cell(nn + 1, 15).Text = "√";
                                        break;
                                    case "两点连线":
                                        grid1.Cell(nn + 1, 16).Text = "√";
                                        break;
                                    case "其他":
                                        grid1.Cell(nn + 1, 17).Text = "√";
                                        break;
                                    default:
                                        grid1.Cell(nn + 1, 17).Text = "√";
                                        break;
                                }
                                #endregion
                                #region 界址线位置
                                string jzxwz = zddRow["JZXWZ"].ToString().Trim();
                                switch (jzxwz)
                                {
                                    case "内":
                                        grid1.Cell(nn + 1, 18).Text = "√";
                                        break;
                                    case "中":
                                        grid1.Cell(nn + 1, 19).Text = "√";
                                        break;
                                    case "外":
                                        grid1.Cell(nn + 1, 20).Text = "√";
                                        break;
                                }

                                #endregion
                                grid1.Cell(nn + 1, 21).Text = zddRow["SM"].ToString().Trim();//说明
                                nn = nn + 2;
                            }
                        }

                    }
                    if (SCYL.Checked)
                    {
                        grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                    }
                    else
                    {
                        grid1.Print(1, 100, true);
                    }
                    #endregion

                    #region 界址说明表
                    string flxfilel6 = Tools.CADTools.GetReportsFolder() + "界址说明表.flx";
                    grid1.OpenFile(flxfilel6);
                    if (SCYL.Checked)
                    {
                        grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                    }
                    else
                    {
                        grid1.Print(1, 100, true);
                    }
                    #endregion

                    #region 界址签章表
                    string selstring_ZDX = "SELECT * FROM D_ZDX WHERE (DJH = '" + ATT.DJH + "')";
                    SqlDataAdapter Adapter_ZDX = Tools.DataBasic.D_ZDX_Initadapter(selstring_ZDX, connectionString);
                    DataSet dataset_ZDX = new DataSet();
                    Adapter_ZDX.Fill(dataset_ZDX, "D_ZDX");//填充Dataset
                    string flxfilel5 = Tools.CADTools.GetReportsFolder() + "界址签章表.flx";
                    grid1.OpenFile(flxfilel5);
                    int zys2 = (int)(dataset_ZDX.Tables["D_ZDX"].Rows.Count / 21.0) + 1;//总页数
                    int dqys2 = 1;//当前页数
                    int zs2 = dataset_ZDX.Tables["D_ZDX"].Rows.Count;//总行数
                    int nn2 = 6;//当前在表格的行数
                    for (int j = 0; j < zs2; j++)//循环调查表
                    {
                        if (dqys2 * 21 == j && j != zs2 - 1)
                        {
                            if (SCYL.Checked)
                            {
                                grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                            }
                            else
                            {
                                grid1.Print(1, 100, true);
                            }
                            grid1.OpenFile(flxfilel5);
                            nn2 = 6;
                        }
                        else
                        {
                            DataRow zdxRow = dataset_ZDX.Tables["D_ZDX"].Rows[j];
                            //grid1.Cell(nn2, 1).Text = "J" + zdxRow["QDH"].ToString().Trim() + getjzdxh(dataset_ZDD, (int)zdxRow["QDH"]);//起点号
                            #region 输出界址点号
                            switch (JzdOutputType)
                            {
                                case "仅宗内编号":
                                    grid1.Cell(nn2, 1).Text = getjzdxh(dataset_ZDD, (int)zdxRow["QDH"]);//起点号
                                    break;
                                case "仅子区内编号":
                                    grid1.Cell(nn2, 1).Text = "J" + zdxRow["QDH"].ToString().Trim();//起点号
                                    break;
                                case "子区和宗内编号":
                                    grid1.Cell(nn2, 1).Text = "J" + zdxRow["QDH"].ToString().Trim() + "(" + getjzdxh(dataset_ZDD, (int)zdxRow["QDH"]) + ")";//起点号
                                    break;
                                default:
                                    grid1.Cell(nn2, 1).Text = "J" + zdxRow["QDH"].ToString().Trim() + "(" + getjzdxh(dataset_ZDD, (int)zdxRow["QDH"]) + ")";//起点号
                                    break;
                            }
                            #endregion
                            string zjdh = zdxRow["ZJDH"].ToString().Trim();//中间点号
                            if (zjdh != "")
                            {
                                //zjdh = "J" + zjdh + getjzdxh(dataset_ZDD, (int)zdxRow["ZJDH"]);
                                #region 输出界址点号
                                switch (JzdOutputType)
                                {
                                    case "仅宗内编号":
                                        zjdh = getjzdxh(dataset_ZDD, (int)zdxRow["ZJDH"]);
                                        break;
                                    case "仅子区内编号":
                                        zjdh = "J" + zjdh;
                                        break;
                                    case "子区和宗内编号":
                                        zjdh = "J" + zjdh + "(" + getjzdxh(dataset_ZDD, (int)zdxRow["ZJDH"]) + ")";
                                        break;
                                    default:
                                        zjdh = "J" + zjdh + "(" + getjzdxh(dataset_ZDD, (int)zdxRow["ZJDH"]) + ")";
                                        break;
                                }
                                #endregion
                            }
                            grid1.Cell(nn2, 3).Text = zjdh;
                            //grid1.Cell(nn2, 5).Text = "J" + zdxRow["ZDH"].ToString().Trim() + getjzdxh(dataset_ZDD, (int)zdxRow["ZDH"]);//终点号
                            #region 输出界址点号
                            switch (JzdOutputType)
                            {
                                case "仅宗内编号":
                                    grid1.Cell(nn2, 5).Text = getjzdxh(dataset_ZDD, (int)zdxRow["ZDH"]);//终点号
                                    break;
                                case "仅子区内编号":
                                    grid1.Cell(nn2, 5).Text = "J" + zdxRow["ZDH"].ToString().Trim();//终点号
                                    break;
                                case "子区和宗内编号":
                                    grid1.Cell(nn2, 5).Text = "J" + zdxRow["ZDH"].ToString().Trim() + "(" + getjzdxh(dataset_ZDD, (int)zdxRow["ZDH"]) + ")";//终点号
                                    break;
                                default:
                                    grid1.Cell(nn2, 5).Text = "J" + zdxRow["ZDH"].ToString().Trim() + "(" + getjzdxh(dataset_ZDD, (int)zdxRow["ZDH"]) + ")";//终点号
                                    break;
                            }
                            #endregion
                            string lzzdh = zdxRow["LZDDJH"].ToString().Trim();//邻宗地地籍号
                            if (lzzdh != "")
                            {
                                lzzdh = lzzdh.Substring(12, 5) + "\r\n";
                            }
                            string bz = zdxRow["BZ"].ToString().Trim();
                            grid1.Cell(nn2, 7).Text = lzzdh + bz;//终点号
                            grid1.Cell(nn2, 21).Text = zdxRow["ZJRQ"].ToString().Trim();//指界日期
                            nn2 = nn2 + 2;
                        }
                    }
                    if (SCYL.Checked)
                    {
                        grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                    }
                    else
                    {
                        grid1.Print(1, 100, true);
                    }
                    #endregion
                }

            }
        }



        private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
        {
            //nonNumberEntered = false;

        }

        // 界址表示 - 快捷键填写表格相关内容
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            int nn = dataGridView1.CurrentCell.ColumnIndex;
            string lx = dataGridView1.Columns[nn].HeaderText;
            if (lx == "界标种类")
            {

                if (e.KeyCode == Keys.F1) { dataGridView1.CurrentCell.Value = "钢钉"; } //dataGridView1.CurrentCell.Value = "钢钉";
                if (e.KeyCode == Keys.F2) { dataGridView1.CurrentCell.Value = "水泥桩"; }
                if (e.KeyCode == Keys.F3) { dataGridView1.CurrentCell.Value = "石灰桩"; }
                if (e.KeyCode == Keys.F4) { dataGridView1.CurrentCell.Value = "喷涂"; }
                if (e.KeyCode == Keys.F5) { dataGridView1.CurrentCell.Value = "瓷标志"; }
                if (e.KeyCode == Keys.F6) { dataGridView1.CurrentCell.Value = "无标志"; }
                if (e.KeyCode == Keys.F7) { dataGridView1.CurrentCell.Value = "其他"; }
            }
            if (lx == "界址类别")
            {
                if (e.KeyCode == Keys.F1) { dataGridView1.CurrentCell.Value = "围墙"; }
                if (e.KeyCode == Keys.F2) { dataGridView1.CurrentCell.Value = "墙壁"; }
                if (e.KeyCode == Keys.F3) { dataGridView1.CurrentCell.Value = "栅栏"; }
                if (e.KeyCode == Keys.F4) { dataGridView1.CurrentCell.Value = "铁丝网"; }
                if (e.KeyCode == Keys.F5) { dataGridView1.CurrentCell.Value = "滴水线"; }
                if (e.KeyCode == Keys.F6) { dataGridView1.CurrentCell.Value = "路涯线"; }
                if (e.KeyCode == Keys.F7) { dataGridView1.CurrentCell.Value = "两点连线"; }
                if (e.KeyCode == Keys.F8) { dataGridView1.CurrentCell.Value = "空地"; }
                if (e.KeyCode == Keys.F9) { dataGridView1.CurrentCell.Value = "其他"; }

            }
            if (lx == "界线位置")
            {
                if (e.KeyCode == Keys.F1) { dataGridView1.CurrentCell.Value = "内"; }
                if (e.KeyCode == Keys.F2) { dataGridView1.CurrentCell.Value = "中"; }
                if (e.KeyCode == Keys.F3) { dataGridView1.CurrentCell.Value = "外"; }
            }
        }

        // 建房申请审批表  【宣城不用】
        /// <summary>
        /// 建房申请审批表
        /// </summary>
        /// <param name="i"></param>
        public void X_JFSQSPB(int i)
        {
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
            int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

            ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
            string XZDM = treeView1.SelectedNode.Tag.ToString();
            ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                connection.Open();
                SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string wzdjh = reader["WZDJH"].ToString().Trim();

                    #region 建房申请审批表
                    string flxfilel2 = Tools.CADTools.GetReportsFolder() + "农村居民宅基地、建房申请审批表.flx";
                    grid1.OpenFile(flxfilel2);


                    if (reader["QLR"].ToString().Trim() == "")
                    {
                        grid1.Cell(2, 2).Text = "/";
                    }
                    else
                    {
                        grid1.Cell(2, 2).Text = reader["QLR"].ToString().Trim();//申请人
                    }

                    if (reader["TDZL"].ToString().Trim() == "")
                    {
                        grid1.Cell(2, 5).Text = "/";
                    }
                    else
                    {
                        grid1.Cell(2, 5).Text = reader["TDZL"].ToString().Trim();//坐落	
                    }
                    double fzmj = (double)reader["TDDYMJ"];
                    if (fzmj < 0.0001)
                    {
                        grid1.Cell(3, 3).Text = "/";
                    }
                    else
                    {
                        grid1.Cell(3, 3).Text = fzmj.ToString("0.00");//批准面积,申请宅基地面积(㎡)
                    }
                    //double zdmj = (double)reader["ZDMJ"];
                    //grid1.Cell(27, 8).Text = zdmj.ToString("0.00");//宗地面积
                    double jzmj = (double)reader["JZMJ"];//建筑面积
                    grid1.Cell(5, 3).Text = jzmj.ToString("0.00");
                    double jzzdmj = (double)reader["JZZDMJ"];//建筑占地面积
                    grid1.Cell(6, 3).Text = jzzdmj.ToString("0.00");
                    int zdcs = -10;
                    string jhjg = ""; //简化后的结构
                    for (int j = 0; j < fwsxjg.Count; j++)//找出同宗所有楼层
                    {
                        if (fwsxjg[j].地籍号1 == ATT.DJH.Substring(6))//如果地籍号相同
                        {
                            int zdcs_n = Convert.ToInt32(fwsxjg[j].总层数11.Trim());
                            if (zdcs_n > zdcs)
                            {
                                zdcs = zdcs_n;
                                #region 房屋结构
                                string fwjg = fwsxjg[j].房屋结构13.Trim();
                                if (fwjg == "钢结构")
                                {
                                    jhjg = "钢";
                                }
                                if (fwjg == "钢、钢筋混凝土结构")
                                {
                                    jhjg = "钢、砼";
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
                                if (fwjg == "")
                                {
                                    jhjg = "/";
                                }
                                #endregion
                            }
                        }
                    }
                    // 建筑结构
                    grid1.Cell(6, 5).Text = jhjg;
                    // 层数
                    if (zdcs != -10)
                        grid1.Cell(6, 8).Text = zdcs.ToString();
                    // 土地权属性质
                    grid1.Cell(7, 8).Text = "集体";
                    // 北至
                    if (reader["BZ"].ToString().Trim() == "")
                    {
                        grid1.Cell(8, 3).Text = "/";
                    }
                    else
                    {
                        string sz = reader["BZ"].ToString().Trim();
                        if (sz != "")
                        {
                            sz = sz.Substring(sz.IndexOf(":") + 1);
                        }
                        grid1.Cell(8, 3).Text = sz; 
                    }
                    // 东至
                    if (reader["DZ"].ToString().Trim() == "")
                    {
                        grid1.Cell(8, 6).Text = "/";
                    }
                    else
                    {
                        string sz = reader["DZ"].ToString().Trim();
                        if (sz != "")
                        {
                            sz = sz.Substring(sz.IndexOf(":") + 1);
                        }
                        grid1.Cell(8, 6).Text = sz;
                    }
                    // 南至
                    if (reader["NZ"].ToString().Trim() == "")
                    {
                        grid1.Cell(9, 3).Text = "/";
                    }
                    else
                    {
                        string sz = reader["NZ"].ToString().Trim();
                        if (sz != "")
                        {
                            sz = sz.Substring(sz.IndexOf(":") + 1);
                        }
                        grid1.Cell(9, 3).Text = sz;//北至
                    }
                    // 西至
                    if (reader["XZ"].ToString().Trim() == "")
                    {
                        grid1.Cell(9, 6).Text = "/";
                    }
                    else
                    {
                        string sz = reader["XZ"].ToString().Trim();
                        if (sz != "")
                        {
                            sz = sz.Substring(sz.IndexOf(":") + 1);
                        }
                        grid1.Cell(9, 6).Text = sz;
                    }
                    if (SCYL.Checked)
                    {
                        grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                    }
                    else
                    {
                        grid1.Print(1, 100, true);
                    }
                    #endregion
                }
                connection.Close();
            }
        }

        // 具结书结构    【宣城不用】
        /// <summary>
        /// 具结书结构
        /// </summary>
        struct jjsjg
        {
            /// <summary>
            /// 房屋名称
            /// </summary>
            public string fwmc;
            /// <summary>
            /// 房屋结构
            /// </summary>
            public string fwjg;
            /// <summary>
            /// 层数
            /// </summary>
            public string cs;
            /// <summary>
            /// 间数
            /// </summary>
            public int js;
            /// <summary>
            /// 建筑面积
            /// </summary>
            public double jzmj;
            /// <summary>
            /// 建成年份
            /// </summary>
            public string jclf;
        }

        // 具结承诺证明书  【宣城不用】
        /// <summary>
        /// 具结承诺证明书
        /// </summary>
        /// <param name="i"></param>
        public void X_JJCLZMS(int i)
        {
            if (fwsxjg.Count == 0)
            {
                Write_FW_SX_to_LC();
            }
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    int ZCS = 0;//总层数
                    double jzmj = 0;
                    double jzzdmj = 0;
                    while (reader.Read())
                    {
                        string wzdjh = reader["WZDJH"].ToString().Trim();
                        #region 找出本宗地内所有楼层
                        List<FWSXJG> TZLC = new List<FWSXJG>();//同宗所有栋楼层集合
                        for (int j = 0; j < fwsxjg.Count; j++)//找出同宗所有楼层
                        {
                            if (fwsxjg[j].地籍号1 == ATT.DJH.Substring(6))//如果地籍号相同
                            {

                                TZLC.Add(fwsxjg[j]);
                                jzmj = jzmj + fwsxjg[j].本层建筑面积29;
                                if (fwsxjg[j].所在层数12 == "1")
                                {
                                    jzzdmj = jzzdmj + fwsxjg[j].本层建筑面积29;
                                }
                            }
                        }
                        ZCS = TZLC.Count;
                        if (TZLC.Count <= 0)
                        {
                            break;
                        }
                        #region 对各层进行排序


                        TZLC.Sort((x, y) =>//对各层进行排序
                        {
                            int dh_x = Convert.ToInt32(x.房屋幢号9) * 1000;
                            int dh_y = Convert.ToInt32(y.房屋幢号9) * 1000;
                            int cs_x = Convert.ToInt32(x.所在层数12);
                            int cs_y = Convert.ToInt32(y.所在层数12);

                            if (dh_x + cs_x > dh_y + cs_y)
                            {
                                return 1;
                            }
                            else if (dh_x + cs_x == dh_y + cs_y)
                            {
                                return 0;
                            }
                            else
                            {
                                return -1;
                            }
                        });
                        #endregion
                        #endregion
                        string flxfile1 = Tools.CADTools.GetReportsFolder() + "具结承诺证明书.flx";
                        grid1.OpenFile(flxfile1);
                        if (reader["TDZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(3, 3).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 3).Text = reader["TDZL"].ToString().Trim();//房地坐落
                        }
                        #region 循环写入各栋
                        List<jjsjg> DSS = new List<jjsjg>();//每栋房信息集合
                        int zdmjxh = 0;//面积最大的房屋所在序号
                        double zdmj = -1;//最大面积
                        while (TZLC.Count > 0)
                        {
                            FWSXJG dic2 = TZLC[0];
                            jjsjg JG = new jjsjg();
                            #region 房屋结构
                            string fwjg = dic2.房屋结构13.Trim();
                            string jhjg = "";
                            if (fwjg == "钢结构")
                            {
                                jhjg = "钢";
                            }
                            if (fwjg == "钢、钢筋混凝土结构")
                            {
                                jhjg = "钢、砼";
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
                            if (fwjg == "")
                            {
                                jhjg = "/";
                            }
                            #endregion
                            JG.fwjg = jhjg;
                            JG.fwmc = dic2.建筑物类型0;
                            JG.jclf = dic2.竣工时间14;
                            JG.cs = dic2.总层数11;
                            string zrcs = dic2.房屋幢号9.Trim();//自然幢
                            double mj = 0;
                            for (int m = 0; m < TZLC.Count; m++)// 
                            {
                                FWSXJG dic3 = TZLC[m];
                                if (zrcs == dic3.房屋幢号9.Trim())
                                {
                                    mj = mj + dic3.本层建筑面积29;
                                    TZLC.RemoveAt(m);
                                    m--;
                                }
                            }
                            JG.jzmj = mj;
                            if (mj > zdmj)
                            {
                                zdmj = mj;
                                zdmjxh = DSS.Count;
                            }
                            DSS.Add(JG);
                        }
                        jjsjg dic4 = DSS[zdmjxh];
                        grid1.Cell(6, 1).Text = "主房";
                        grid1.Cell(6, 3).Text = dic4.fwjg;
                        grid1.Cell(6, 4).Text = dic4.cs;
                        grid1.Cell(6, 6).Text = dic4.jzmj.ToString("0.00");
                        grid1.Cell(6, 7).Text = dic4.jclf;
                        DSS.RemoveAt(zdmjxh);
                        for (int m = 0; m < DSS.Count; m++)//写各层的数据
                        {
                            jjsjg dic5 = DSS[m];
                            string fwmc2 = "副房";
                            if (dic5.fwmc != "房屋")
                            {
                                fwmc2 = dic5.fwmc;
                            }
                            grid1.Cell(m + 7, 1).Text = fwmc2;
                            grid1.Cell(m + 7, 3).Text = dic5.fwjg;
                            grid1.Cell(m + 7, 4).Text = dic5.cs;
                            grid1.Cell(m + 7, 6).Text = dic5.jzmj.ToString("0.00");
                            grid1.Cell(m + 7, 7).Text = dic5.jclf;
                            if (m >= 8)
                            {
                                break;
                            }
                        }
                        #endregion
                        grid1.Cell(20, 6).Text = reader["QLR"].ToString().Trim();
                        grid1.Cell(21, 6).Text = "20    年    月   日";
                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                    }
                    connection.Close();
                    reader.Dispose();
                }
            }
        }

        // 权属来源证明   【宣城不用】
        /// <summary>
        /// 权属来源证明
        /// </summary>
        /// <param name="i"></param>
        public void X_QSLYZM(int i)
        {
            if (fwsxjg.Count == 0)
            {
                Write_FW_SX_to_LC();
            }
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string wzdjh = reader["WZDJH"].ToString().Trim();
                        #region 填表
                        string flxfilel2 = Tools.CADTools.GetReportsFolder() + "不动产登记权属来源证明.flx";
                        grid1.OpenFile(flxfilel2);

                        #region 填写建筑年代和建筑物类型
                        int zdcs = -10;
                        string jhjg = ""; //简化后的结构
                        string jzld = "";
                        for (int j = 0; j < fwsxjg.Count; j++)//找出同宗所有楼层
                        {
                            if (fwsxjg[j].地籍号1 == ATT.DJH.Substring(6))//如果地籍号相同
                            {
                                int zdcs_n = Convert.ToInt32(fwsxjg[j].总层数11.Trim());
                                if (zdcs_n > zdcs)
                                {
                                    zdcs = zdcs_n;
                                    jzld = fwsxjg[j].竣工时间14;
                                    #region 房屋结构
                                    string fwjg = fwsxjg[j].房屋结构13.Trim();
                                    if (fwjg == "钢结构")
                                    {
                                        jhjg = "钢";
                                    }
                                    if (fwjg == "钢、钢筋混凝土结构")
                                    {
                                        jhjg = "钢、砼";
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
                                    if (fwjg == "")
                                    {
                                        jhjg = "/";
                                    }
                                    #endregion
                                }
                            }
                        }
                        grid1.Cell(10, 7).Text = jhjg;//建筑物类型	
                        grid1.Cell(9, 7).Text = jzld;//建筑年代
                        // grid1.Cell(9, 3).Text = jzld;

                        #endregion
                        if (reader["YBZDDM"].ToString().Trim() == "")
                        {
                            grid1.Cell(2, 6).Text = "/";
                        }
                        else
                        {
                            string bh = reader["YBZDDM"].ToString().Trim();
                            bh = bh.Substring(6, bh.Length - 6);
                            grid1.Cell(2, 6).Text = bh;//编号	
                        }

                        if (reader["QLR"].ToString().Trim() == "")
                        {
                            grid1.Cell(3, 3).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 3).Text = reader["QLR"].ToString().Trim();//使用权
                        }
                        if (reader["KCR"].ToString().Trim() == "")
                        {
                            grid1.Cell(3, 7).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 7).Text = reader["KCR"].ToString().Trim();//身份证号		
                        }

                        string BDCDYH = wzdjh.Substring(0, 6) + "  " + wzdjh.Substring(6, 6) + "  " +
                                          wzdjh.Substring(12, 7) + "  " + "F00010001";

                        grid1.Cell(4, 3).Text = BDCDYH;//不动产单元号	

                        if (reader["TDZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(5, 3).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(5, 3).Text = reader["TDZL"].ToString().Trim();//坐落	
                        }

                        double zdmj = (double)reader["ZDMJ"];
                        grid1.Cell(7, 7).Text = zdmj.ToString("0.00");//宗地面积

                        double jzzdmj = (double)reader["JZZDMJ"];//建筑占地面积
                        grid1.Cell(8, 3).Text = jzzdmj.ToString("0.00");

                        double jzmj = (double)reader["JZMJ"];//建筑总面积
                        grid1.Cell(8, 7).Text = jzmj.ToString("0.00");

                        #region 找出本宗地内所有楼层
                        List<FWSXJG> TZLC = new List<FWSXJG>();//同宗所有栋楼层集合
                        for (int j = 0; j < fwsxjg.Count; j++)//找出同宗所有楼层
                        {
                            if (fwsxjg[j].地籍号1 == ATT.DJH.Substring(6))//如果地籍号相同
                            {

                                TZLC.Add(fwsxjg[j]);
                                jzmj = jzmj + fwsxjg[j].本层建筑面积29;
                                if (fwsxjg[j].所在层数12 == "1")
                                {
                                    jzzdmj = jzzdmj + fwsxjg[j].本层建筑面积29;
                                }
                            }
                        }
                        //string jgsj = "/";
                        //double zdmj2 = -1;

                        //for (int j = 0; j < TZLC.Count; j++)
                        //{
                        //    if (TZLC[j].本层建筑面积29 > zdmj)
                        //    {
                        //        zdmj2 = TZLC[j].本层建筑面积29;
                        //        jgsj = TZLC[j].竣工时间14;
                        //    }
                        //}

                        #endregion

                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion
                    }
                    connection.Close();
                    reader.Dispose();
                }
            }
        }

        public void X_QSLYZM_WH(int i) // 权属来源证明 - 芜湖
        {
            if (fwsxjg.Count == 0)
            {
                Write_FW_SX_to_LC();
            }
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string wzdjh = reader["WZDJH"].ToString().Trim();

                        #region 填表
                        string flxfilel2 = Tools.CADTools.GetReportsFolder() + "权属来源证明-芜湖.flx";
                        grid1.OpenFile(flxfilel2);

                        #region 填写建筑年代和建筑物类型
                        int zdcs = -10;
                        string jhjg = ""; //简化后的结构
                        string jzld = "";
                        for (int j = 0; j < fwsxjg.Count; j++)//找出同宗所有楼层
                        {
                            if (fwsxjg[j].地籍号1 == ATT.DJH.Substring(6))//如果地籍号相同
                            {
                                int zdcs_n = Convert.ToInt32(fwsxjg[j].总层数11.Trim());
                                if (zdcs_n > zdcs)
                                {
                                    zdcs = zdcs_n;
                                    jzld = fwsxjg[j].竣工时间14;
                                    #region 房屋结构
                                    string fwjg = fwsxjg[j].房屋结构13.Trim();
                                    if (fwjg == "钢结构")
                                    {
                                        jhjg = "钢";
                                    }
                                    if (fwjg == "钢、钢筋混凝土结构")
                                    {
                                        jhjg = "钢、砼";
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
                                    if (fwjg == "")
                                    {
                                        jhjg = "/";
                                    }
                                    #endregion
                                }
                            }
                        }
                        // 建筑物类型	
                        grid1.Cell(15, 7).Text = jhjg;
                        // 建房时间
                        grid1.Cell(14, 7).Text = jzld;
                        // 占地时间
                        grid1.Cell(14, 2).Text = jzld;
                        #endregion

                        // 申请权利人
                        if (reader["QLR"].ToString().Trim() == "")
                        {
                            grid1.Cell(3, 2).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 2).Text = reader["QLR"].ToString().Trim();
                        }
                        // 身份证号
                        if (reader["KCR"].ToString().Trim() == "")
                        {
                            grid1.Cell(3, 7).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 7).Text = reader["KCR"].ToString().Trim();
                        }
                        // 不动产单元号
                        string BDCDYH = wzdjh.Substring(0, 6) + "  " + wzdjh.Substring(6, 6) + "  " +
                                          wzdjh.Substring(12, 7) + "  " + "F00010001";
                        grid1.Cell(4, 2).Text = BDCDYH;
                        //坐落
                        if (reader["TDZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(5, 2).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(5, 2).Text = reader["TDZL"].ToString().Trim();
                        }
                        //宗地面积
                        double zdmj = (double)reader["ZDMJ"];
                        grid1.Cell(12, 2).Text = zdmj.ToString("0.00");
                        //超占面积
                        if (zdmj > FDMJ)
                            grid1.Cell(12, 7).Text = (zdmj - FDMJ).ToString("0.00");
                        else
                            grid1.Cell(12, 7).Text = "0.00";

                        //建筑占地面积
                        double jzzdmj = (double)reader["JZZDMJ"];
                        grid1.Cell(13, 7).Text = jzzdmj.ToString("0.00");
                        //建筑总面积
                        double jzmj = (double)reader["JZMJ"];
                        grid1.Cell(13, 2).Text = jzmj.ToString("0.00");


                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion
                    }
                    connection.Close();
                    reader.Dispose();
                }
            }
        }

        // 坐标册
        /// <summary>
        ///  坐标册
        /// </summary>
        public void X_ZBC(int i)
        {
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    string flxfile1 = Tools.CADTools.GetReportsFolder() + "界址点成果表1.flx";
                    grid1.OpenFile(flxfile1);
                    // string zdh = ATT.DJH.Substring(6, 11);
                    string qlr = "";
                    double zdmj = 0;//宗地面积
                    double jzzdmj = 0;//建筑占地面积
                    string shy = "";
                    string wzjdh = "";
                    while (reader.Read())
                    {
                        qlr = reader["QLR"].ToString().Trim();
                        zdmj = (double)reader["ZDMJ"];
                        jzzdmj = (double)reader["JZZDMJ"];
                        shy = reader["SHRXM"].ToString().Trim();
                        wzjdh = reader["WZDJH"].ToString().Trim();
                    }
                    reader.Close();
                    string selstring_ZDD = "SELECT * FROM D_ZDD WHERE (DJH = '" + ATT.DJH + "')";
                    SqlDataAdapter Adapter_ZDD = Tools.DataBasic.D_ZDD_Initadapter(selstring_ZDD, connectionString);
                    DataSet dataset_ZDD = new DataSet();
                    Adapter_ZDD.Fill(dataset_ZDD, "D_ZDD");//填充Dataset
                    int zys = (int)(dataset_ZDD.Tables["D_ZDD"].Rows.Count / 27.0) + 1;//总页数
                    int dqys = 1;//当前页数
                    int zs = dataset_ZDD.Tables["D_ZDD"].Rows.Count;//总行数
                    int nn = 11;//当前在表格的行数
                    grid1.Cell(1, 10).Text = "第 " + dqys.ToString() + " 页";
                    grid1.Cell(2, 10).Text = "共 " + zys.ToString() + " 页";
                    grid1.Cell(3, 2).Text = wzjdh;
                    grid1.Cell(4, 2).Text = qlr;
                    grid1.Cell(5, 3).Text = zdmj.ToString("0.00");
                    grid1.Cell(6, 3).Text = jzzdmj.ToString("0.00");
                    grid1.Cell(67, 2).Text = HTY.Text.Trim();
                    grid1.Cell(67, 5).Text = shy;
                    grid1.Cell(67, 9).Text = SCRQ.Text.Trim();
                    for (int j = 0; j < zs; j++)//循环调查表
                    {
                        if (dqys * 27 == j && j != zs - 1)
                        {
                            DataRow zddRow2 = dataset_ZDD.Tables["D_ZDD"].Rows[j];
                            grid1.Cell(nn, 1).Text = zddRow2["MBBSM"].ToString();
                            //grid1.Cell(nn, 2).Text = "J" + zddRow2["JZDH"].ToString();
                            #region 输出界址点号
                            string XH = string.Empty;
                            switch (JzdOutputType)
                            {
                                case "仅宗内编号":
                                    XH = "J" + zddRow2["MBBSM"].ToString();
                                    if (j == zs - 1)
                                    {
                                        XH = "J1";
                                    }
                                    grid1.Cell(nn, 2).Text = XH;
                                    break;
                                case "仅子区内编号":
                                    grid1.Cell(nn, 2).Text = "J" + zddRow2["JZDH"].ToString();
                                    break;
                                case "子区和宗内编号":
                                    XH = "(J" + zddRow2["MBBSM"].ToString() + ")";
                                    if (j == zs - 2)
                                    {
                                        XH = "(J1)";
                                    }
                                    grid1.Cell(nn, 2).Text = "J" + zddRow2["JZDH"].ToString() + XH;
                                    break;
                                default:
                                    XH = "(J" + zddRow2["MBBSM"].ToString() + ")";
                                    if (j == zs - 1)
                                    {
                                        XH = "(J1)";
                                    }
                                    grid1.Cell(nn, 2).Text = "J" + zddRow2["JZDH"].ToString() + XH;
                                    break;
                            }
                            #endregion
                            double x = (double)zddRow2["X"];
                            double y = (double)zddRow2["Y"];
                            grid1.Cell(nn, 3).Text = y.ToString("0.000");
                            grid1.Cell(nn, 6).Text = x.ToString("0.000");
                            if (SCYL.Checked)
                            {
                                grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                            }
                            else
                            {
                                grid1.Print(1, 100, true);
                            }
                            dqys++;
                            string flxfile2 = Tools.CADTools.GetReportsFolder() + "界址点成果表2.flx";
                            grid1.OpenFile(flxfile2);
                            grid1.Cell(1, 10).Text = "第 " + dqys.ToString() + " 页";
                            grid1.Cell(2, 10).Text = "共 " + zys.ToString() + " 页";
                            grid1.Cell(3, 2).Text = wzjdh;
                            grid1.Cell(4, 2).Text = qlr;
                            grid1.Cell(67, 2).Text = HTY.Text.Trim();
                            grid1.Cell(67, 5).Text = shy;
                            grid1.Cell(67, 9).Text = SCRQ.Text.Trim();
                            nn = 11;
                            DataRow zddRow3 = dataset_ZDD.Tables["D_ZDD"].Rows[j];
                            grid1.Cell(nn, 1).Text = zddRow3["MBBSM"].ToString();
                            grid1.Cell(nn, 2).Text = "J" + zddRow3["JZDH"].ToString();
                            double x1 = (double)zddRow3["X"];
                            double y1 = (double)zddRow3["Y"];
                            double jl = (double)zddRow3["JZJJ"];
                            grid1.Cell(nn, 3).Text = y1.ToString("0.000");
                            grid1.Cell(nn, 6).Text = x1.ToString("0.000");
                            grid1.Cell(nn + 1, 12).Text = jl.ToString("0.00");
                            nn = nn + 2;
                        }
                        else
                        {
                            DataRow zddRow2 = dataset_ZDD.Tables["D_ZDD"].Rows[j];

                            grid1.Cell(nn, 1).Text = zddRow2["MBBSM"].ToString();
                            //grid1.Cell(nn, 2).Text = "J" + zddRow2["JZDH"].ToString();
                            #region 输出界址点号
                            string XH = string.Empty;
                            switch (JzdOutputType)
                            {
                                case "仅宗内编号":
                                    XH = "J" + zddRow2["MBBSM"].ToString();
                                    if (j == zs - 1)
                                    {
                                        XH = "J1";
                                    }
                                    grid1.Cell(nn, 2).Text = XH;
                                    break;
                                case "仅子区内编号":
                                    grid1.Cell(nn, 2).Text = "J" + zddRow2["JZDH"].ToString();
                                    break;
                                case "子区和宗内编号":
                                    XH = "(J" + zddRow2["MBBSM"].ToString() + ")";
                                    if (j == zs - 1)
                                    {
                                        XH = "(J1)";
                                    }
                                    grid1.Cell(nn, 2).Text = "J" + zddRow2["JZDH"].ToString() + XH;
                                    break;
                                default:
                                    XH = "(J" + zddRow2["MBBSM"].ToString() + ")";
                                    if (j == zs - 1)
                                    {
                                        XH = "(J1)";
                                    }
                                    grid1.Cell(nn, 2).Text = "J" + zddRow2["JZDH"].ToString() + XH;
                                    break;
                            }
                            #endregion
                            double x = (double)zddRow2["X"];
                            double y = (double)zddRow2["Y"];
                            if (j != zs - 1)//最后一行
                            {
                                double jl = (double)zddRow2["JZJJ"];
                                grid1.Cell(nn + 1, 12).Text = jl.ToString("0.00");
                            }
                            grid1.Cell(nn, 3).Text = y.ToString("0.000");
                            grid1.Cell(nn, 6).Text = x.ToString("0.000");
                            nn = nn + 2;
                        }
                    }
                    if (SCYL.Checked)
                    {
                        grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                    }
                    else
                    {
                        grid1.Print(1, 100, true);
                    }
                }
            }
        }

        // 公示报表
        /// <summary>
        /// 公示报表
        /// </summary>
        public void X_GSBB()
        {
            if (treeView1.SelectedNode.Level == 4)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                string JDH = XZDM.Substring(6, 3);
                string JFH = XZDM.Substring(9, 3);
                string XZQH = XZDM.Substring(0, 6);
                string flxfile2 = Tools.CADTools.GetReportsFolder() + "公示报表.flx";
                string XZmc = treeView1.SelectedNode.FullPath;//土地坐落
                string[] zl = XZmc.Split('\\');
                XZmc = zl[3] + zl[4];
                string connectionString = Tools.Uitl.LJstring();//写连接字符串
                string selstring = "SELECT * FROM DCB where DJH like '" + XZDM + "%%%%%'";
                if (Tools.DataBasic.Create_DCB_table(connectionString, "DCB", Tools.DataBasicString.Create_DCB))//如果”DCB“不存在则创建DCB
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlDataAdapter Adapter_TB = Tools.DataBasic.DCB_Initadapter(selstring, connectionString);
                        DataSet dataset_DCB2 = new DataSet();
                        Adapter_TB.Fill(dataset_DCB2, "DCB");//填充Dataset
                        System.Data.DataTable TB_DCB2 = dataset_DCB2.Tables["DCB"];
                        grid1.OpenFile(flxfile2);
                        grid1.Cell(1, 1).Text = XZmc + "公示报表";
                        grid1.InsertRow(4, TB_DCB2.Rows.Count);
                        grid1.Refresh();

                        for (int i = 0; i < TB_DCB2.Rows.Count; i++)
                        {
                            DataRow dcbRow2 = TB_DCB2.Rows[i];
                            int nn = i + 4;
                            grid1.Cell(nn, 1).Text = (i + 1).ToString();
                            grid1.Cell(nn, 2).Text = dcbRow2["QLR"].ToString().Trim();
                            grid1.Cell(nn, 3).Text = dcbRow2["DJH"].ToString().Trim().Substring(6, 11);
                            grid1.Cell(nn, 4).Text = dcbRow2["TDZL"].ToString().Trim();
                            grid1.Cell(nn, 5).Text = dcbRow2["SJYT"].ToString().Trim();
                            double zdmj = (double)dcbRow2["ZDMJ"];
                            grid1.Cell(nn, 6).Text = zdmj.ToString("0.00");
                            double fzmj = (double)dcbRow2["FZMJ"];
                            grid1.Cell(nn, 7).Text = fzmj.ToString("0.00");
                            bool pstg = (bool)dcbRow2["CSHG"];
                            if (pstg == false)
                            {
                                grid1.Cell(nn, 8).Text = "审批未通过";
                            }
                            else
                            {
                                grid1.Cell(nn, 8).Text = "通过";
                            }
                        }
                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        connection.Close();
                    }
                }
            }
        }

        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        // 写 申请审批表 - 2015年规范 - 【阜阳、金寨】
        /// <summary>
        /// 申请审批表
        /// </summary>
        public void X_SQSBB(int i)
        {
            if (fwsxjg.Count == 0)
            {
                Write_FW_SX_to_LC();
            }
            string apppath = Tools.CADTools.GetAppPath();
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        #region 审批表1
                        string flxfile1 = Tools.CADTools.GetReportsFolder() + "不动产登记申请审批表1.flx";
                        grid1.OpenFile(flxfile1);
                        grid1.Cell(3, 5).Text = ATT.DJH.Substring(6, 11);//编号
                        if (reader["SQRQ"].ToString().Trim() == "")//收件日期
                        {
                            grid1.Cell(4, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(4, 5).Text = reader["SQRQ"].ToString().Trim();
                        }
                        if (reader["LXR"].ToString().Trim() == "")//收件人
                        {
                            grid1.Cell(3, 17).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 17).Text = reader["LXR"].ToString().Trim();
                        }
                        grid1.Cell(4, 26).Text = "1";//平方米 
                        grid1.Cell(4, 30).Text = "0";//公顷 
                        grid1.Cell(4, 34).Text = "0";//亩)	
                        grid1.Cell(4, 36).Text = "0";//万元	

                        if (reader["SQRLX"].ToString().Trim() == "国有建设用地使用权")
                        {
                            grid1.Cell(6, 6).Text = "1";
                        }
                        if (reader["SQRLX"].ToString().Trim() == "国有土地宅基地使用权")
                        {
                            grid1.Cell(6, 15).Text = "1";
                        }
                        if (reader["SQRLX"].ToString().Trim() == "集体土地宅基地使用权")
                        {
                            grid1.Cell(6, 15).Text = "1";
                        }
                        if (reader["SQRLX"].ToString().Trim() == "集体建设用地使用权")
                        {
                            grid1.Cell(6, 22).Text = "1";
                        }
                        grid1.Cell(7, 20).Text = "1";//房屋所有权
                        grid1.Cell(9, 3).Text = "1";//首次登记
                        if (reader["SQRLX2"].ToString().Trim() == "总登记")
                        {
                            grid1.Cell(9, 5).Text = "1";
                        }
                        if (reader["SQRLX2"].ToString().Trim() == "初始登记")
                        {
                            grid1.Cell(9, 9).Text = "1";
                        }

                        if (reader["QLR"].ToString().Trim() == "")//权利人姓名（名称）				
                        {
                            grid1.Cell(12, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(12, 8).Text = reader["QLR"].ToString().Trim();
                        }
                        if (reader["ZRXM"].ToString().Trim() == "")//权利人身份证件种类 				
                        {
                            grid1.Cell(13, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(13, 8).Text = reader["ZRXM"].ToString().Trim();
                        }

                        if (reader["KCR"].ToString().Trim() == "")//权利人证件号								
                        {
                            grid1.Cell(13, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(13, 22).Text = reader["KCR"].ToString().Trim();
                        }

                        if (reader["TXDZ"].ToString().Trim() == "")//通讯地址 
                        {
                            grid1.Cell(14, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(14, 8).Text = reader["TXDZ"].ToString().Trim();
                        }

                        if (reader["YZBM"].ToString().Trim() == "")//邮 编		
                        {
                            grid1.Cell(14, 25).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(14, 25).Text = reader["YZBM"].ToString().Trim();
                        }

                        if (reader["FRDBXM"].ToString().Trim() == "")//法定代表人或负责人 	
                        {
                            grid1.Cell(15, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(15, 8).Text = reader["FRDBXM"].ToString().Trim();
                        }
                        if (reader["FRDBDH"].ToString().Trim() == "")//法定代表人或负责人联系电话	 
                        {
                            grid1.Cell(15, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(15, 22).Text = reader["FRDBDH"].ToString().Trim();
                        }
                        if (reader["JBB_DLRXM"].ToString().Trim() == "")//代理人姓名 
                        {
                            grid1.Cell(16, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 8).Text = reader["JBB_DLRXM"].ToString().Trim();
                        }
                        if (reader["JBB_DLRDH"].ToString().Trim() == "")//代理人联系电话 
                        {
                            grid1.Cell(16, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 22).Text = reader["JBB_DLRDH"].ToString().Trim();
                        }
                        if (reader["CMZYJ"].ToString().Trim() == "")//代理机构名称 
                        {
                            grid1.Cell(17, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(17, 8).Text = reader["CMZYJ"].ToString().Trim();
                        }
                        if (reader["QLR2"].ToString().Trim() == "")//义务人姓名（名称） 
                        {
                            grid1.Cell(19, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(19, 8).Text = reader["QLR2"].ToString().Trim();
                        }
                        if (reader["QLR_ZJZL2"].ToString().Trim() == "")//义务人身份证件种类 
                        {
                            grid1.Cell(20, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(20, 8).Text = reader["QLR_ZJZL2"].ToString().Trim();
                        }
                        if (reader["QLR_ZJBH2"].ToString().Trim() == "")//义务人证件号 
                        {
                            grid1.Cell(20, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(20, 22).Text = reader["QLR_ZJBH2"].ToString().Trim();
                        }
                        if (reader["TXDZ2"].ToString().Trim() == "")//义务人通讯地址 
                        {
                            grid1.Cell(21, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(21, 8).Text = reader["TXDZ2"].ToString().Trim();
                        }
                        if (reader["YZBM2"].ToString().Trim() == "")//义务人邮 编 
                        {
                            grid1.Cell(21, 25).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(21, 25).Text = reader["YZBM2"].ToString().Trim();
                        }
                        if (reader["FRDBXM2"].ToString().Trim() == "")//义务人法定代表人或负责人 
                        {
                            grid1.Cell(22, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(22, 8).Text = reader["FRDBXM2"].ToString().Trim();
                        }
                        if (reader["LXDH2"].ToString().Trim() == "")//义务人法定代表人或负责人联系电话 
                        {
                            grid1.Cell(22, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(22, 22).Text = reader["LXDH2"].ToString().Trim();
                        }
                        if (reader["SQS_DLRXM2"].ToString().Trim() == "")//义务人代理人姓名 
                        {
                            grid1.Cell(23, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(23, 8).Text = reader["SQS_DLRXM2"].ToString().Trim();
                        }
                        if (reader["LXDH2"].ToString().Trim() == "")//义务人代理人联系电话	 
                        {
                            grid1.Cell(23, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(23, 22).Text = reader["LXDH2"].ToString().Trim();
                        }
                        if (reader["TDZL"].ToString().Trim() == "")//坐  落 
                        {
                            grid1.Cell(24, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(24, 8).Text = reader["TDZL"].ToString().Trim();
                        }
                        string wzdjh = reader["WZDJH"].ToString().Trim();
                        string BDCDYH = wzdjh.Substring(0, 6) + " " + wzdjh.Substring(6, 6) + " " +
                                       wzdjh.Substring(12, 7) + " " + "F00010001";
                        grid1.Cell(25, 8).Text = BDCDYH;//不动产单元号	
                        grid1.Cell(25, 30).Text = reader["DLJGDH"].ToString().Trim() + "/房屋";
                        double zdmj = (double)reader["ZDMJ"];
                        double jzmj = (double)reader["JZMJ"];//建筑面积
                        grid1.Cell(26, 8).Text = zdmj.ToString("0.00") + "/" + jzmj.ToString("0.00");//不动产面  积
                        string fwyt1 = reader["SJYT"].ToString().Trim();
                        string fwyt2 = "";
                        for (int k = 0; k < fwsxjg.Count; k++)
                        {
                            if (fwsxjg[k].地籍号1 == ATT.DJH.Substring(6))
                            {
                                fwyt2 = fwsxjg[k].房屋用途4;
                                break;
                            }
                        }
                        grid1.Cell(26, 22).Text = fwyt1 + "/" + fwyt2;
                        if (reader["ZYZGZSH"].ToString().Trim() == "")//原不动产权证书号 
                        {
                            grid1.Cell(27, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(27, 8).Text = reader["ZYZGZSH"].ToString().Trim();
                        }
                        grid1.Cell(27, 22).Text = "/";//用海类型 
                        if (reader["DWXZ2"].ToString().Trim() == "")//构筑物类型 
                        {
                            grid1.Cell(28, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(28, 8).Text = reader["DWXZ2"].ToString().Trim();
                        }
                        grid1.Cell(28, 22).Text = "/";//林  种 

                        double dbse = (double)reader["TDDYJE"];//被担保债权数额 
                        if (dbse < 0.00001)
                        {
                            grid1.Cell(29, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(29, 8).Text = dbse.ToString("0.000");
                        }



                        if (reader["DYSX"].ToString().Trim() == "")//债务履行期限 
                        {
                            grid1.Cell(29, 24).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(29, 24).Text = reader["DYSX"].ToString().Trim();
                        }
                        if (reader["TEXTBZ2"].ToString().Trim() == "")//在建建筑物抵押范围 
                        {
                            grid1.Cell(31, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(31, 8).Text = reader["TEXTBZ2"].ToString().Trim();
                        }

                        if (reader["XYDZL"].ToString().Trim() == "")//需役地坐落 
                        {
                            grid1.Cell(32, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(32, 8).Text = reader["XYDZL"].ToString().Trim();
                        }
                        grid1.Cell(33, 8).Text = "/";//需役地不动产单元号 

                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion
                        #region 审批表2
                        flxfile1 = apppath + "不动产登记申请审批表2.flx";
                        grid1.OpenFile(flxfile1);
                        if (reader["LXR2"].ToString().Trim() == "")
                        {
                            grid1.Cell(1, 6).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(1, 6).Text = reader["LXR2"].ToString().Trim();//登记原因
                        }
                        //LXR2
                        string zmwj = reader["SQDJLR"].ToString().Trim();//登记原因 证明文件
                        if (zmwj == "")
                        {
                            grid1.Cell(2, 6).Text = "/";
                            grid1.Cell(3, 6).Text = "/";
                            grid1.Cell(4, 6).Text = "/";
                            grid1.Cell(5, 6).Text = "/";
                            grid1.Cell(6, 6).Text = "/";
                            grid1.Cell(7, 6).Text = "/";
                        }
                        else
                        {
                            string[] zmwj_sz = zmwj.Split('\n');
                            for (int k = 0; k < zmwj_sz.Length; k++)
                            {
                                grid1.Cell(k + 2, 6).Text = zmwj_sz[k].Trim();
                            }
                        }
                        if (reader["DYQX_Q"].ToString().Trim() == "单一版")//申请证书版式 
                        {
                            grid1.Cell(8, 4).Text = "1";
                        }
                        else
                        {
                            grid1.Cell(8, 7).Text = "1";
                        }
                        if (reader["DYQX_Z"].ToString().Trim() == "是")//申请分别持证 
                        {
                            grid1.Cell(8, 17).Text = "1";
                        }
                        else
                        {
                            grid1.Cell(8, 19).Text = "1";
                        }

                        if (reader["SQSBZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(9, 2).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(9, 2).Text = reader["SQSBZ"].ToString().Trim();//备注
                        }
                        if (reader["FRDBXM"].ToString().Trim() == "")
                        {
                            // grid1.Cell(12, 6).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(13, 6).Text = reader["FRDBXM"].ToString().Trim();//申请人1（签章）： 
                        }
                        if (reader["JBB_DLRXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(13, 6).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(12, 6).Text = reader["JBB_DLRXM"].ToString().Trim();//代理人1（签章）： 
                        }
                        if (reader["FRDBXM2"].ToString().Trim() == "")
                        {
                            grid1.Cell(12, 16).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(12, 16).Text = reader["FRDBXM2"].ToString().Trim();//申请人2（签章）：			
                        }
                        if (reader["SQS_DLRXM2"].ToString().Trim() == "")
                        {
                            grid1.Cell(13, 16).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(13, 16).Text = reader["SQS_DLRXM2"].ToString().Trim();//申请人2代理人（签章） 
                        }
                        if (reader["DYSQRJZRQ"].ToString().Trim() == "")
                        {
                            grid1.Cell(14, 1).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(14, 1).Text = reader["DYSQRJZRQ"].ToString().Trim();//申请人1  年   月   日 
                        }
                        if (reader["DESQRJZRQ"].ToString().Trim() == "")
                        {
                            grid1.Cell(14, 16).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(14, 16).Text = reader["DESQRJZRQ"].ToString().Trim();//申请人1  年   月   日 
                        }
                        if (reader["CSYJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(16, 2).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 2).Text = reader["CSYJ"].ToString().Trim();//初  审 
                        }
                        if (reader["SHYJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(16, 10).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 10).Text = reader["SHYJ"].ToString().Trim();//复  审 
                        }
                        if (reader["PZYJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(16, 16).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 16).Text = reader["PZYJ"].ToString().Trim();//核  定	 
                        }
                        if (reader["CSSCR"].ToString().Trim() == "")
                        {
                            //grid1.Cell(19, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(19, 5).Text = reader["CSSCR"].ToString().Trim();//审查人1： 
                        }

                        grid1.Cell(19, 12).Text = reader["SHFZR"].ToString().Trim();//审查人2： 

                        grid1.Cell(19, 18).Text = reader["PZR"].ToString().Trim();//负责人 
                        if (reader["CSRJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(20, 2).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(20, 2).Text = reader["CSRJ"].ToString().Trim();// 1  年   月   日 	
                        }
                        if (reader["SHRQ_SPB"].ToString().Trim() == "")
                        {
                            grid1.Cell(20, 10).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(20, 10).Text = reader["SHRQ_SPB"].ToString().Trim();//2  年   月   日 	
                        }
                        if (reader["PZRQ"].ToString().Trim() == "")
                        {
                            grid1.Cell(20, 16).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(20, 16).Text = reader["PZRQ"].ToString().Trim();//3  年   月   日 	
                        }
                        if (reader["SPBBZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(21, 2).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(21, 2).Text = reader["SPBBZ"].ToString().Trim();//备 注
                        }
                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion
                    }
                    reader.Close();
                }
            }
        }
        // 写 申请书和审批表 - 2016年规范 - 【宣城、芜湖】
        /// <summary>
        /// 写 申请书、审批表
        /// </summary>
        public void X_SQS_SPB_GB(int i)
        {
            if (fwsxjg.Count == 0)
            {
                Write_FW_SX_to_LC();
            }
            string apppath = Tools.CADTools.GetAppPath();
            // 连接字符串
            string connectionString = Tools.Uitl.LJstring();

            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    // 连接数据库
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        #region 写申请书
                        string flxfile1 = Tools.CADTools.GetReportsFolder() + "不动产登记申请书.flx";
                        grid1.OpenFile(flxfile1);

                        #region 表头
                        // 编号
                        grid1.Cell(3, 5).Text = ATT.DJH.Substring(6, 11);
                        // 收件日期
                        if (reader["SQRQ"].ToString().Trim() == "")
                        {
                            grid1.Cell(4, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(4, 5).Text = reader["SQRQ"].ToString().Trim();
                        }
                        // 收件人
                        if (reader["LXR"].ToString().Trim() == "")
                        {
                            grid1.Cell(3, 17).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(3, 17).Text = reader["LXR"].ToString().Trim();
                        }
                        // 平方米
                        grid1.Cell(4, 26).Text = "1";
                        // 公顷
                        grid1.Cell(4, 30).Text = "0";
                        // 亩
                        grid1.Cell(4, 34).Text = "0";
                        // 万元
                        //grid1.Cell(4, 36).Text = "0";
                        #endregion

                        #region 登记事由
                        // 土地所有权
                        grid1.Cell(6, 3).Text = "0";
                        // 国有建设用地使用权
                        if (reader["SQRLX"].ToString().Trim() == "国有建设用地使用权")
                        {
                            grid1.Cell(6, 6).Text = "1";
                        }
                        // 宅基地使用权
                        if (reader["SQRLX"].ToString().Trim() == "国有土地宅基地使用权"
                            || reader["SQRLX"].ToString().Trim() == "集体土地宅基地使用权")
                        {
                            grid1.Cell(6, 15).Text = "1";
                        }
                        //if (reader["SQRLX"].ToString().Trim() == "集体土地宅基地使用权")
                        //{
                        //    grid1.Cell(6, 15).Text = "1";
                        //}

                        // 集体建设用地使用权
                        if (reader["SQRLX"].ToString().Trim() == "集体建设用地使用权")
                        {
                            grid1.Cell(6, 22).Text = "1";
                        }
                        // 房屋所有权
                        grid1.Cell(7, 20).Text = "1";
                        // 首次登记
                        grid1.Cell(9, 3).Text = "1";

                        //// 总登记
                        //if (reader["SQRLX2"].ToString().Trim() == "总登记")
                        //{
                        //    grid1.Cell(9, 5).Text = "1";
                        //}
                        //// 初始登记
                        //if (reader["SQRLX2"].ToString().Trim() == "初始登记")
                        //{
                        //    grid1.Cell(9, 9).Text = "1";
                        //}
                        #endregion

                        #region 申请人情况 权利人
                        // 权利人姓名（名称）
                        if (reader["QLR"].ToString().Trim() == "")
                        {
                            grid1.Cell(12, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(12, 8).Text = reader["QLR"].ToString().Trim();
                        }
                        // 权利人身份证件种类
                        if (reader["ZRXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(13, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(13, 8).Text = reader["ZRXM"].ToString().Trim();
                        }
                        // 权利人证件号	
                        if (reader["KCR"].ToString().Trim() == "")
                        {
                            grid1.Cell(13, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(13, 22).Text = reader["KCR"].ToString().Trim();
                        }
                        // 通讯地址 
                        if (reader["TXDZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(14, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(14, 8).Text = reader["TXDZ"].ToString().Trim();
                        }
                        // 邮编
                        if (reader["YZBM"].ToString().Trim() == "")
                        {
                            grid1.Cell(14, 25).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(14, 25).Text = reader["YZBM"].ToString().Trim();
                        }
                        // 法定代表人或负责人 
                        if (reader["FRDBXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(15, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(15, 8).Text = reader["FRDBXM"].ToString().Trim();
                        }
                        // 法定代表人或负责人联系电话	 
                        if (reader["FRDBDH"].ToString().Trim() == "")
                        {
                            grid1.Cell(15, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(15, 22).Text = reader["FRDBDH"].ToString().Trim();
                        }
                        // 代理人姓名 
                        if (reader["JBB_DLRXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(16, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 8).Text = reader["JBB_DLRXM"].ToString().Trim();
                        }
                        // 代理人联系电话
                        if (reader["JBB_DLRDH"].ToString().Trim() == "")
                        {
                            grid1.Cell(16, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(16, 22).Text = reader["JBB_DLRDH"].ToString().Trim();
                        }
                        // 代理机构名称 
                        if (reader["CMZYJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(17, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(17, 8).Text = reader["CMZYJ"].ToString().Trim();
                        }
                        #endregion

                        #region 申请人情况 义务人
                        // 义务人姓名（名称） 
                        if (reader["QLR2"].ToString().Trim() == "")
                        {
                            grid1.Cell(19, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(19, 8).Text = reader["QLR2"].ToString().Trim();
                        }
                        // 义务人身份证件种类 
                        if (reader["QLR_ZJZL2"].ToString().Trim() == "")
                        {
                            grid1.Cell(20, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(20, 8).Text = reader["QLR_ZJZL2"].ToString().Trim();
                        }
                        // 义务人证件号 
                        if (reader["QLR_ZJBH2"].ToString().Trim() == "")
                        {
                            grid1.Cell(20, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(20, 22).Text = reader["QLR_ZJBH2"].ToString().Trim();
                        }
                        // 义务人通讯地址 
                        if (reader["TXDZ2"].ToString().Trim() == "")
                        {
                            grid1.Cell(21, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(21, 8).Text = reader["TXDZ2"].ToString().Trim();
                        }
                        // 义务人邮编 
                        if (reader["YZBM2"].ToString().Trim() == "")
                        {
                            grid1.Cell(21, 25).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(21, 25).Text = reader["YZBM2"].ToString().Trim();
                        }
                        // 义务人法定代表人或负责人 
                        if (reader["FRDBXM2"].ToString().Trim() == "")
                        {
                            grid1.Cell(22, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(22, 8).Text = reader["FRDBXM2"].ToString().Trim();
                        }
                        // 义务人法定代表人或负责人联系电话 
                        if (reader["LXDH2"].ToString().Trim() == "")
                        {
                            grid1.Cell(22, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(22, 22).Text = reader["LXDH2"].ToString().Trim();
                        }
                        // 义务人代理人姓名 
                        if (reader["SQS_DLRXM2"].ToString().Trim() == "")
                        {
                            grid1.Cell(23, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(23, 8).Text = reader["SQS_DLRXM2"].ToString().Trim();
                        }
                        // 义务人代理人联系电话	
                        if (reader["LXDH2"].ToString().Trim() == "")
                        {
                            grid1.Cell(23, 22).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(23, 22).Text = reader["LXDH2"].ToString().Trim();
                        }

                        // 义务人代理人联系电话
                        // TODO:
                        // 在此处添加写义务人代理机构信息的代码
                        // 注意：
                        // 数据库中和图形属性表中没有义务人相关的代理机构名称字段，需检查添加
                        //if (reader["义务人代理机构"].ToString().Trim() == "")
                        if (reader["LXDH2"].ToString().Trim() == "")
                        {
                            grid1.Cell(24, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(24, 8).Text = reader["LXDH2"].ToString().Trim();
                        }
                        #endregion

                        #region 不动产情况
                        // 坐落 
                        if (reader["TDZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(25, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(25, 8).Text = reader["TDZL"].ToString().Trim();
                        }
                        //不动产单元号
                        string wzdjh = reader["WZDJH"].ToString().Trim();
                        string BDCDYH = wzdjh.Substring(0, 6) + " " + wzdjh.Substring(6, 6) + " " +
                                       wzdjh.Substring(12, 7) + " " + "F00010001";
                        grid1.Cell(26, 8).Text = BDCDYH;
                        // 不动产类型
                        grid1.Cell(26, 30).Text = reader["DLJGDH"].ToString().Trim() + "/房屋";
                        // 面积
                        double zdmj = (double)reader["ZDMJ"]; //宗地面积
                        double jzmj = (double)reader["JZMJ"]; //建筑面积
                        grid1.Cell(27, 8).Text = zdmj.ToString("0.00") + "/" + jzmj.ToString("0.00");
                        // 用途
                        string fwyt1 = reader["SJYT"].ToString().Trim();
                        string fwyt2 = "";
                        for (int k = 0; k < fwsxjg.Count; k++)
                        {
                            if (fwsxjg[k].地籍号1 == ATT.DJH.Substring(6))
                            {
                                fwyt2 = fwsxjg[k].房屋用途4;
                                break;
                            }
                        }
                        grid1.Cell(27, 22).Text = fwyt1 + "/" + fwyt2;
                        // 原不动产权属证书号 
                        if (reader["ZYZGZSH"].ToString().Trim() == "")
                        {
                            grid1.Cell(28, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(28, 8).Text = reader["ZYZGZSH"].ToString().Trim();
                        }
                        // 用海类型 
                        grid1.Cell(28, 22).Text = "/";
                        // 构筑物类型 
                        if (reader["DWXZ2"].ToString().Trim() == "")
                        {
                            grid1.Cell(29, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(28, 8).Text = reader["DWXZ2"].ToString().Trim();
                        }
                        //林  种 
                        grid1.Cell(29, 22).Text = "/";
                        #endregion

                        #region 抵押情况
                        // 被担保债权数额 
                        double dbse = (double)reader["TDDYJE"];
                        if (dbse < 0.00001)
                        {
                            grid1.Cell(31, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(31, 8).Text = dbse.ToString("0.000");
                        }
                        // 债务履行期限 
                        if (reader["DYSX"].ToString().Trim() == "")
                        {
                            grid1.Cell(31, 24).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(31, 24).Text = reader["DYSX"].ToString().Trim();
                        }
                        // 在建建筑物抵押范围 
                        if (reader["TEXTBZ2"].ToString().Trim() == "")
                        {
                            grid1.Cell(33, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(33, 8).Text = reader["TEXTBZ2"].ToString().Trim();
                        }
                        #endregion

                        #region 地役权情况
                        // 需役地坐落 
                        if (reader["XYDZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(34, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(34, 8).Text = reader["XYDZL"].ToString().Trim();
                        }
                        // 需役地不动产单元号 
                        grid1.Cell(35, 8).Text = "/";
                        #endregion

                        #region 登记原因及证明
                        // 登记原因
                        if (reader["LXR2"].ToString().Trim() == "")
                        {
                            grid1.Cell(36, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(36, 8).Text = reader["LXR2"].ToString().Trim();
                        }
                        // 登记原因证明文件
                        string zmwj = reader["SQDJLR"].ToString().Trim();
                        if (zmwj == "")
                        {
                            grid1.Cell(37, 8).Text = "/";
                            grid1.Cell(38, 8).Text = "/";
                            grid1.Cell(39, 8).Text = "/";
                            grid1.Cell(40, 8).Text = "/";
                            grid1.Cell(41, 8).Text = "/";
                            grid1.Cell(42, 8).Text = "/";
                        }
                        else
                        {
                            string[] zmwj_sz = zmwj.Split('\n');
                            for (int k = 0; k < zmwj_sz.Length; k++)
                            {
                                grid1.Cell(k + 37, 8).Text = zmwj_sz[k].Trim();
                            }
                        }
                        #endregion

                        #region 申请证书版式
                        if (reader["DYQX_Q"].ToString().Trim() == "单一版")
                        {
                            grid1.Cell(43, 6).Text = "1";
                        }
                        else
                        {
                            grid1.Cell(43, 11).Text = "1";
                        }
                        #endregion

                        #region 申请分别持证
                        if (reader["DYQX_Z"].ToString().Trim() == "是")
                        {
                            grid1.Cell(43, 30).Text = "1";
                        }
                        else
                        {
                            grid1.Cell(43, 34).Text = "1";
                        }
                        #endregion

                        #region 备注
                        if (reader["SQSBZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(44, 3).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(44, 3).Text = reader["SQSBZ"].ToString().Trim();
                        }
                        #endregion

                        #region 签章部分
                        // 申请人1（签章）： 
                        if (reader["FRDBXM"].ToString().Trim() == "")
                        {
                            // grid1.Cell(46, 7).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(46, 7).Text = reader["FRDBXM"].ToString().Trim();
                        }
                        // 代理人1（签章）： 
                        if (reader["JBB_DLRXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(47, 7).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(47, 7).Text = reader["JBB_DLRXM"].ToString().Trim();
                        }
                        // 申请人2（签章）：	
                        if (reader["FRDBXM2"].ToString().Trim() == "")
                        {
                            grid1.Cell(46, 26).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(46, 26).Text = reader["FRDBXM2"].ToString().Trim();
                        }
                        // 代理人2（签章） 
                        if (reader["SQS_DLRXM2"].ToString().Trim() == "")
                        {
                            grid1.Cell(47, 26).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(47, 26).Text = reader["SQS_DLRXM2"].ToString().Trim();
                        }
                        //申请人1  年月日 
                        if (reader["DYSQRJZRQ"].ToString().Trim() == "")
                        {
                            grid1.Cell(48, 7).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(48, 7).Text = reader["DYSQRJZRQ"].ToString().Trim();
                        }
                        //申请人2  年月日 
                        if (reader["DESQRJZRQ"].ToString().Trim() == "")
                        {
                            grid1.Cell(48, 26).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(48, 26).Text = reader["DESQRJZRQ"].ToString().Trim();
                        }
                        #endregion

                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion

                        #region 写审批表
                        flxfile1 = Tools.CADTools.GetReportsFolder() + "不动产登记审批表.flx";
                        grid1.OpenFile(flxfile1);

                        #region 二维码
                        //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        //string TimeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        //QRCodeData qrCodeData = qrGenerator.CreateQrCode("NF" + TimeStamp, QRCodeGenerator.ECCLevel.Q);
                        //QRCode qrCode = new QRCode(qrCodeData);
                        //Bitmap qrCodeImage = qrCode.GetGraphic(40);
                        //grid1.Images.Add(qrCodeImage, "QRMAP");
                        //grid1.Cell(3, 3).SetImage(grid1.Images.Item("QRMAP").Key);
                        #endregion

                        #region 申请人情况
                        // 权利人姓名（名称）1
                        if (reader["QLR"].ToString().Trim() == "")
                        {
                            grid1.Cell(9, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(9, 5).Text = reader["QLR"].ToString().Trim();
                        }
                        // 权利人身份证件种类 1
                        if (reader["ZRXM"].ToString().Trim() == "")
                        {
                            grid1.Cell(10, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(10, 5).Text = reader["ZRXM"].ToString().Trim();
                        }
                        // 权利人证件号	1
                        if (reader["KCR"].ToString().Trim() == "")
                        {
                            grid1.Cell(10, 8).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(10, 8).Text = reader["KCR"].ToString().Trim();
                        }
                        // 权利人姓名（名称）2
                        //if (reader["QLR"].ToString().Trim() == "")
                        //{
                        grid1.Cell(12, 5).Text = "/";
                        //}
                        //else
                        //{
                        //    grid1.Cell(9, 5).Text = reader["QLR"].ToString().Trim();
                        //}
                        //// 权利人身份证件种类 2
                        //if (reader["ZRXM"].ToString().Trim() == "")
                        //{
                        grid1.Cell(13, 5).Text = "/";
                        //}
                        //else
                        //{
                        //    grid1.Cell(10, 5).Text = reader["ZRXM"].ToString().Trim();
                        //}
                        //// 权利人证件号	2
                        //if (reader["KCR"].ToString().Trim() == "")
                        //{
                        grid1.Cell(13, 8).Text = "/";
                        //}
                        //else
                        //{
                        //    grid1.Cell(10, 8).Text = reader["KCR"].ToString().Trim();
                        //}
                        #endregion

                        #region 不动产情况
                        // 坐落 
                        if (reader["TDZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(14, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(14, 5).Text = reader["TDZL"].ToString().Trim();
                        }
                        //不动产单元号
                        grid1.Cell(15, 5).Text = BDCDYH;
                        // 不动产类型
                        grid1.Cell(15, 8).Text = reader["DLJGDH"].ToString().Trim() + "/房屋";
                        // 面积
                        grid1.Cell(16, 5).Text = zdmj.ToString("0.00") + "/" + jzmj.ToString("0.00");
                        // 用途                        
                        grid1.Cell(16, 8).Text = fwyt1 + "/" + fwyt2;
                        //权利性质		
                        if (reader["QSXZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(17, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(17, 5).Text = reader["QSXZ"].ToString().Trim();
                        }
                        // 用海类型 
                        grid1.Cell(17, 8).Text = "/";
                        // 构筑物类型 
                        if (reader["DWXZ2"].ToString().Trim() == "")
                        {
                            grid1.Cell(18, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(18, 5).Text = reader["DWXZ2"].ToString().Trim();
                        }
                        //林  种 
                        grid1.Cell(18, 8).Text = "/";
                        // 原不动产权属证书号 
                        if (reader["ZYZGZSH"].ToString().Trim() == "")
                        {
                            grid1.Cell(19, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(19, 5).Text = reader["ZYZGZSH"].ToString().Trim();
                        }
                        #endregion

                        #region 地役权情况
                        // 需役地坐落 
                        if (reader["XYDZL"].ToString().Trim() == "")
                        {
                            grid1.Cell(20, 5).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(20, 5).Text = reader["XYDZL"].ToString().Trim();
                        }
                        // 需役地不动产单元号 
                        grid1.Cell(21, 5).Text = "/";
                        #endregion

                        #region 审批情况
                        //初审 
                        if (reader["CSYJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(22, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(22, 4).Text = reader["CSYJ"].ToString().Trim();
                        }
                        //初审人
                        //grid1.Cell(24, 5).Text = "/";
                        //if (reader["CSSCR"].ToString().Trim() == "")
                        //{
                        //    //grid1.Cell(24, 5).Text = "/";
                        //}
                        //else
                        //{
                        //    grid1.Cell(24, 5).Text = reader["CSSCR"].ToString().Trim();
                        //}
                        // 初审日期	
                        if (reader["CSRJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(24, 7).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(24, 7).Text = reader["CSRJ"].ToString().Trim();
                        }

                        //核准
                        if (reader["PZYJ"].ToString().Trim() == "")
                        {
                            grid1.Cell(25, 4).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(25, 4).Text = reader["PZYJ"].ToString().Trim();
                        }
                        //核准人 
                        //grid1.Cell(27, 5).Text = "/";
                        //grid1.Cell(27, 5).Text = reader["PZR"].ToString().Trim();
                        //核准日期	
                        if (reader["PZRQ"].ToString().Trim() == "")
                        {
                            grid1.Cell(27, 7).Text = "年   月   日";
                        }
                        else
                        {
                            grid1.Cell(27, 7).Text = reader["PZRQ"].ToString().Trim();
                        }
                        #endregion

                        #region 备注
                        if (reader["SPBBZ"].ToString().Trim() == "")
                        {
                            grid1.Cell(28, 3).Text = "/";
                        }
                        else
                        {
                            grid1.Cell(28, 3).Text = reader["SPBBZ"].ToString().Trim();
                        }
                        #endregion

                        if (SCYL.Checked)
                        {
                            grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                        }
                        else
                        {
                            grid1.Print(1, 100, true);
                        }
                        #endregion
                    }
                    reader.Close();
                }
            }
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑


        public void hc(RichTextBox text)
        {
            int nn = text.SelectionStart;
            text.Text = text.Text.Substring(0, nn) + "\r\n" + text.Text.Substring(nn, text.Text.Length - nn);
            text.SelectionStart = nn + 1;
            text.SelectionLength = 0;
        }

        private void SQDJLR_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(SQDJLR);
            }
        }

        private void SQSBZ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(SQSBZ);
            }
        }

        private void GYQLRQK_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(GYQLRQK);
            }
        }

        private void QSDCJS_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(QSDCJS);
            }
        }

        private void DJKZJS_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(DJKZJS);
            }
        }

        private void DJDCJGSHYJ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(DJDCJGSHYJ);
            }
        }

        private void CSYJ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(CSYJ);
            }
        }

        private void SHYJ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(SHYJ);
            }
        }

        private void PZYJ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(PZYJ);
            }
        }

        private void SPBBZ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(SPBBZ);
            }
        }

        private void SM_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                hc(SM);
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        // 图表输出 - “土地证书”按钮
        private void button13_Click_1(object sender, EventArgs e)
        {
            if (this.HZLB.SelectedIndex == -1)
            {
                MessageBox.Show("请选择权利人！", "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            ListBoxsx listBoxsx = (NCZJDDJFZ.DCB.DCB_JBB.ListBoxsx)HZLB.Items[HZLB.SelectedIndex];
            string ndjh = listBoxsx.NHBH;
            ATT.DJH = ndjh;

            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            string selstring = "SELECT * FROM DCB where DJH = '" + ATT.DJH + "'";
            DataSet da_ZD = new DataSet();//定义DataSet 
            SqlDataAdapter DP = new SqlDataAdapter();//初始化适配器
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DP = Tools.DataBasic.DCB_Initadapter(selstring, connectionString);
                DP.Fill(da_ZD, "DCB");//填充Dataset
                System.Data.DataTable TB_DCB = da_ZD.Tables["DCB"];
                if (TB_DCB == null)
                {
                    return;
                }
                DataRow row = TB_DCB.Rows[0];
                bool pztg = (bool)row["PZTG"];
                if (!pztg)
                {
                    MessageBox.Show("审批没通过！不能打印土地证书!", "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                bool isdy = (bool)row["TDZSFDY"];
                if (isdy)
                {
                    if (MessageBox.Show("土地证书已经打印过了，是否继续打印？", "系统提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                connection.Close();
            }

            TDZS tdzs = new TDZS();
            tdzs.ShowDialog();

        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        // 档案封面
        /// <summary>
        /// 档案封面
        /// </summary>
        public void X_DAFM(int i)
        {
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                ReadOrWritexml readOrWritexml = new ReadOrWritexml();
                readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
                readOrWritexml.CreateXmlDocument("上下偏离-档案", this.SXPL.Value.ToString().Trim());
                readOrWritexml.CreateXmlDocument("左右偏离-档案", this.ZYPL.Value.ToString().Trim());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    string flxfile1 = Tools.CADTools.GetReportsFolder() + "档案封面.flx";
                    grid1.OpenFile(flxfile1);
                    // string zdh = ATT.DJH.Substring(6, 11);
                    string qlr = "";
                    string tdzl = "";
                    string qsxz = "";
                    string lf = DJKRQ.Text.Trim().Substring(0, 4);
                    string yf = DJKRQ.Text.Trim().Substring(6, 2);
                    string wzdjh = "";
                    string qzmc = Tools.ReadWriteReg.read_reg("主管单位全称");

                    while (reader.Read())
                    {
                        qlr = reader["QLR"].ToString().Trim();
                        tdzl = reader["TDZL"].ToString().Trim();
                        qsxz = reader["QSXZ"].ToString().Trim();
                        wzdjh = reader["WZDJH"].ToString().Trim();
                    }
                    reader.Close();
                    grid1.Cell(2, 2).Text = qzmc;
                    grid1.Cell(15, 4).Text = lf;
                    grid1.Cell(15, 7).Text = yf;
                    grid1.Cell(5, 9).Text = qlr;
                    grid1.Cell(7, 9).Text = tdzl;
                    grid1.Cell(9, 9).Text = qsxz;
                    grid1.Cell(11, 9).Text = lf + ATT.DJH.Substring(6, 11);
                    grid1.Cell(13, 9).Text = ATT.DJH.Substring(6, 11);
                    grid1.Cell(20, 18).Text = lf + "-" + ATT.DJH.Substring(6, 11);

                    FlexCell.PageSetup pageSetup = grid1.PageSetup;
                    pageSetup.TopMargin = Decimal.ToSingle(SXPL.Value) / 10;
                    pageSetup.LeftMargin = Decimal.ToSingle(ZYPL.Value) / 10;
                    if (SCYL.Checked)
                    {
                        grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                    }
                    else
                    {
                        grid1.Print(1, 100, true);
                    }
                }
            }
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

        private void SQRLX_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SQRLX.Text == "国有建设用地使用权")
            {
                SYQLX.SelectedItem = "国有建设用地使用权";
                TEXTBZ1.Text = "国有";
            }
            if (SQRLX.Text == "国有土地宅基地使用权")
            {
                SYQLX.SelectedItem = "宅基地使用权";
                TEXTBZ1.Text = "国有";
            }
            if (SQRLX.Text == "集体土地宅基地使用权")
            {
                SYQLX.SelectedItem = "宅基地使用权";
                TEXTBZ1.Text = TDZL.Text;
            }
            if (SQRLX.Text == "集体建设用地使用权")
            {
                SYQLX.SelectedItem = "集体建设用地使用权";
                DWXZ.SelectedItem = "企业";
                TEXTBZ1.Text = TDZL.Text;
            }
        }

        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        // 房屋调查表
        // 非国家规范
        // 房屋权界线示意图栏改为面积汇总数据栏
        /// <summary>
        /// 房屋调查表
        /// </summary>
        public void X_FWDCCB(int i)
        {
            if (fwsxjg.Count == 0)
            {
                Write_FW_SX_to_LC();
            }
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    int ZCS = 0;//总层数
                    double jzmj = 0;
                    double jzzdmj = 0;
                    while (reader.Read())
                    {
                        string wzdjh = reader["WZDJH"].ToString().Trim();
                        #region 房屋调查表   一栋房一张表
                        #region 找出本宗地内所有楼层
                        List<FWSXJG> TZLC = new List<FWSXJG>();//同宗所有栋楼层集合
                        for (int j = 0; j < fwsxjg.Count; j++)//找出同宗所有楼层
                        {
                            if (fwsxjg[j].地籍号1 == ATT.DJH.Substring(6))//如果地籍号相同
                            {

                                TZLC.Add(fwsxjg[j]);
                                jzmj = jzmj + fwsxjg[j].本层建筑面积29;
                                if (fwsxjg[j].所在层数12 == "1")
                                {
                                    jzzdmj = jzzdmj + fwsxjg[j].本层建筑面积29;
                                }
                            }
                        }
                        ZCS = TZLC.Count;
                        if (TZLC.Count <= 0)
                        {
                            break;
                        }

                        #region 对各层进行排序


                        TZLC.Sort((x, y) =>//对各层进行排序
                        {
                            int dh_x = Convert.ToInt32(x.房屋幢号9) * 1000;
                            int dh_y = Convert.ToInt32(y.房屋幢号9) * 1000;
                            int cs_x = Convert.ToInt32(x.所在层数12);
                            int cs_y = Convert.ToInt32(y.所在层数12);

                            if (dh_x + cs_x > dh_y + cs_y)
                            {
                                return 1;
                            }
                            else if (dh_x + cs_x == dh_y + cs_y)
                            {
                                return 0;
                            }
                            else
                            {
                                return -1;
                            }
                        });
                        #endregion

                        #endregion
                        int zzs = (int)((ZCS - 1) / 12.0 + 0.0000001) + 1;//此宗地需要打印的总张数
                        for (int p = 0; p < zzs; p++)//循环每一页
                        {
                            string flxfile1 = Tools.CADTools.GetReportsFolder() + "房屋基本信息调查表.flx";
                            grid1.OpenFile(flxfile1);
                            #region 写入表的共同部分
                            FWSXJG dic = TZLC[0];//取出第一层，也可能是-1层
                            grid1.Cell(2, 2).Text = XZDM.Substring(0, 6);//市区名称或代码
                            grid1.Cell(2, 6).Text = ATT.DJH.Substring(6, 3);//地籍区 
                            grid1.Cell(2, 9).Text = ATT.DJH.Substring(9, 3);//地籍子区
                            grid1.Cell(2, 11).Text = LIS.N0_Name.Substring(0, 5); ;//宗地号
                            if (reader["TDZL"].ToString().Trim() == "")
                            {
                                grid1.Cell(4, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(4, 2).Text = reader["TDZL"].ToString().Trim();//房地坐落
                            }
                            if (reader["YZBM"].ToString().Trim() == "")
                            {
                                grid1.Cell(4, 16).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(4, 16).Text = reader["YZBM"].ToString().Trim();//邮政编码	
                            }
                            if (reader["QLR"].ToString().Trim() == "")
                            {
                                grid1.Cell(5, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(5, 2).Text = reader["QLR"].ToString().Trim();//所有权人	
                            }
                            if (reader["ZRXM"].ToString().Trim() == "")
                            {
                                grid1.Cell(5, 11).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(5, 11).Text = reader["ZRXM"].ToString().Trim();//证件种类		
                            }
                            if (reader["KCR"].ToString().Trim() == "")
                            {
                                grid1.Cell(6, 11).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(6, 11).Text = reader["KCR"].ToString().Trim();//证件号			
                            }
                            if (reader["TXDZ"].ToString().Trim() == "")
                            {
                                grid1.Cell(7, 8).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(7, 8).Text = reader["TXDZ"].ToString().Trim();//通讯住址		
                            }

                            if (reader["ZZXM"].ToString().Trim() == "")
                            {
                                grid1.Cell(7, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(7, 2).Text = reader["ZZXM"].ToString().Trim();//电    话		
                            }
                            if (reader["DWXZ"].ToString().Trim() == "")
                            {
                                grid1.Cell(8, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(8, 2).Text = reader["DWXZ"].ToString().Trim();//权利人类型		
                            }
                            string dzwdm = dic.房屋幢号9.PadLeft(4, '0') + "0001";//定着物（房屋）代码	 
                            grid1.Cell(2, 15).Text = dzwdm;//定着物（房屋）代码	 
                            string BDCDYH = wzdjh.Substring(0, 6) + "  " + wzdjh.Substring(6, 6) + "  " +
                                           wzdjh.Substring(12, 7) + "  " + "F00010001";
                            grid1.Cell(3, 2).Text = BDCDYH;//不动产单元号;
                            grid1.Cell(8, 9).Text = reader["XZGHBMFZR"].ToString().Trim();//项目名称	
                            if (dic.房屋性质2.Trim() == "")
                            {
                                grid1.Cell(9, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(9, 2).Text = dic.房屋性质2.Trim();//房屋性质	
                            }
                            if (dic.房屋产别3.Trim() == "")
                            {
                                grid1.Cell(9, 9).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(9, 9).Text = dic.房屋产别3.Trim();
                            }
                            if (dic.房屋用途4.Trim() == "")
                            {
                                grid1.Cell(10, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(10, 2).Text = dic.房屋用途4.Trim();
                            }
                            if (dic.规划用途5.Trim() == "")
                            {
                                grid1.Cell(10, 9).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(10, 9).Text = dic.规划用途5.Trim();
                            }
                            if (dic.共有情况8.Trim() == "")
                            {
                                grid1.Cell(7, 12).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(7, 12).Text = dic.共有情况8.Trim();
                            }
                            if (dic.附加说明18.Trim() == "")
                            {
                                grid1.Cell(27, 12).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(27, 12).Text = dic.附加说明18.Trim();
                            }
                            if (dic.调查意见20.Trim() == "")
                            {
                                grid1.Cell(28, 12).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(28, 12).Text = dic.调查意见20.Trim();
                            }
                            if (dic.调查员21.Trim() == "")
                            {
                                grid1.Cell(29, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(29, 2).Text = dic.调查员21.Trim();
                            }
                            if (dic.调查日期19.Trim() == "")
                            {
                                grid1.Cell(29, 12).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(29, 12).Text = dic.调查日期19.Trim();
                            }

                            grid1.Cell(27, 2).Text = "  本宗地建筑总面积= " + jzmj.ToString("0.00") + " 平方米" + "\r\n" + "\r\n" +
                                                     "  本宗地房屋建筑占地总面积= " + jzzdmj.ToString("0.00") + " 平方米";

                            #endregion
                            int jl = (p + 1) * 12;
                            if (jl > ZCS)
                            {
                                jl = ZCS;
                            }
                            int qsjl = 15;
                            #region 循环写入各栋、各层
                            for (int m = p * 12; m < jl; m++)//写各层的数据
                            {
                                FWSXJG dic2 = TZLC[m];
                                grid1.Cell(qsjl, 2).Text = "1";//幢号
                                grid1.Cell(qsjl, 3).Text = "1";//户号
                                grid1.Cell(qsjl, 4).Text = dic2.房屋幢号9.Trim();//自然幢
                                grid1.Cell(qsjl, 5).Text = dic2.总层数11.Trim();
                                grid1.Cell(qsjl, 6).Text = dic2.所在层数12.Trim();
                                #region 房屋结构
                                string fwjg = dic2.房屋结构13.Trim();
                                string jhjg = "";
                                if (fwjg == "钢结构")
                                {
                                    jhjg = "钢";
                                }
                                if (fwjg == "钢、钢筋混凝土结构")
                                {
                                    jhjg = "钢、砼";
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
                                if (fwjg == "")
                                {
                                    jhjg = "/";
                                }
                                grid1.Cell(qsjl, 7).Text = jhjg;
                                #endregion
                                if (dic2.竣工时间14.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 8).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 8).Text = dic2.竣工时间14.Trim();
                                }

                                if (dic2.所在层数12 == "1")
                                {
                                    grid1.Cell(qsjl, 9).Text = dic2.本层建筑面积29.ToString("0.00");
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 9).Text = "/";
                                }
                                grid1.Cell(qsjl, 10).Text = dic2.本层建筑面积29.ToString("0.00");
                                if (dic2.专有建筑面积15.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 11).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 11).Text = dic2.专有建筑面积15.Trim();
                                }
                                if (dic2.分摊建筑面积16.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 12).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 12).Text = dic2.分摊建筑面积16.Trim();
                                }
                                if (dic2.产权来源7.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 13).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 13).Text = dic2.产权来源7.Trim();
                                }
                                if (dic2.北23.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 14).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 14).Text = dic2.北23.Trim();
                                }
                                if (dic2.东24.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 15).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 15).Text = dic2.东24.Trim();
                                }

                                if (dic2.南25.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 16).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 16).Text = dic2.南25.Trim();
                                }
                                if (dic2.西26.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 17).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 17).Text = dic2.西26.Trim();
                                }
                                qsjl++;
                            }

                            #endregion
                            if (SCYL.Checked)
                            {
                                grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                            }
                            else
                            {
                                grid1.Print(1, 100, true);
                            }
                        }

                        #endregion
                    }
                    connection.Close();
                    reader.Dispose();
                }
            }
        }
        // 房屋调查表
        // 国家规范
        // 有房屋权界线示意图栏
        /// <summary>
        /// 房屋调查表
        /// </summary>
        public void X_FWDCCB_GB(int i)
        {
            if (fwsxjg.Count == 0)
            {
                Write_FW_SX_to_LC();
            }
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            if (HZLB.SelectedIndex != -1)
            {
                int width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int height = System.Windows.Forms.SystemInformation.WorkingArea.Height;

                ListBoxsx LIS = (ListBoxsx)HZLB.SelectedItems[i];
                string XZDM = treeView1.SelectedNode.Tag.ToString();
                ATT.DJH = XZDM + LIS.N0_Name.Substring(0, 5);
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string selstring_DCB = "SELECT * FROM DCB WHERE (DJH = '" + ATT.DJH + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(selstring_DCB, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    int ZCS = 0;//总层数
                    double jzmj = 0;
                    double jzzdmj = 0;
                    while (reader.Read())
                    {
                        string wzdjh = reader["WZDJH"].ToString().Trim();
                        #region 房屋调查表   一栋房一张表
                        #region 找出本宗地内所有楼层
                        List<FWSXJG> TZLC = new List<FWSXJG>();//同宗所有栋楼层集合
                        for (int j = 0; j < fwsxjg.Count; j++)//找出同宗所有楼层
                        {
                            if (fwsxjg[j].地籍号1 == ATT.DJH.Substring(6))//如果地籍号相同
                            {

                                TZLC.Add(fwsxjg[j]);
                                jzmj = jzmj + fwsxjg[j].本层建筑面积29;
                                if (fwsxjg[j].所在层数12 == "1")
                                {
                                    jzzdmj = jzzdmj + fwsxjg[j].本层建筑面积29;
                                }
                            }
                        }
                        ZCS = TZLC.Count;
                        if (TZLC.Count <= 0)
                        {
                            break;
                        }

                        #region 对各层进行排序


                        TZLC.Sort((x, y) =>//对各层进行排序
                            {
                                int dh_x = Convert.ToInt32(x.房屋幢号9) * 1000;
                                int dh_y = Convert.ToInt32(y.房屋幢号9) * 1000;
                                int cs_x = Convert.ToInt32(x.所在层数12);
                                int cs_y = Convert.ToInt32(y.所在层数12);

                                if (dh_x + cs_x > dh_y + cs_y)
                                {
                                    return 1;
                                }
                                else if (dh_x + cs_x == dh_y + cs_y)
                                {
                                    return 0;
                                }
                                else
                                {
                                    return -1;
                                }
                            });
                        #endregion

                        #endregion
                        int zzs = (int)((ZCS - 1) / 12.0 + 0.0000001) + 1;//此宗地需要打印的总张数
                        for (int p = 0; p < zzs; p++)//循环每一页
                        {
                            string flxfile1 = Tools.CADTools.GetReportsFolder() + "房屋基本信息调查表.flx";
                            grid1.OpenFile(flxfile1);
                            #region 写入表的共同部分
                            FWSXJG dic = TZLC[0];//取出第一层，也可能是-1层
                            //市区名称或代码
                            grid1.Cell(2, 2).Text = XZDM.Substring(0, 6);
                            //地籍区 
                            grid1.Cell(2, 6).Text = ATT.DJH.Substring(6, 3);
                            //地籍子区
                            grid1.Cell(2, 9).Text = ATT.DJH.Substring(9, 3);
                            //宗地号
                            grid1.Cell(2, 11).Text = LIS.N0_Name.Substring(0, 5);
                            //房地坐落
                            if (reader["TDZL"].ToString().Trim() == "")
                            {
                                grid1.Cell(4, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(4, 2).Text = reader["TDZL"].ToString().Trim();
                            }
                            //邮政编码	
                            if (reader["YZBM"].ToString().Trim() == "")
                            {
                                grid1.Cell(4, 16).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(4, 16).Text = reader["YZBM"].ToString().Trim();
                            }
                            //所有权人	
                            if (reader["QLR"].ToString().Trim() == "")
                            {
                                grid1.Cell(5, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(5, 2).Text = reader["QLR"].ToString().Trim();
                            }
                            //证件种类		
                            if (reader["ZRXM"].ToString().Trim() == "")
                            {
                                grid1.Cell(5, 11).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(5, 11).Text = reader["ZRXM"].ToString().Trim();
                            }
                            //证件号			
                            if (reader["KCR"].ToString().Trim() == "")
                            {
                                grid1.Cell(6, 11).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(6, 11).Text = reader["KCR"].ToString().Trim();
                            }
                            //通讯住址		
                            if (reader["TXDZ"].ToString().Trim() == "")
                            {
                                grid1.Cell(7, 8).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(7, 8).Text = reader["TXDZ"].ToString().Trim();
                            }
                            //电    话		
                            if (reader["ZZXM"].ToString().Trim() == "")
                            {
                                grid1.Cell(7, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(7, 2).Text = reader["ZZXM"].ToString().Trim();
                            }
                            //权利人类型		
                            if (reader["DWXZ"].ToString().Trim() == "")
                            {
                                grid1.Cell(8, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(8, 2).Text = reader["DWXZ"].ToString().Trim();
                            }
                            //定着物（房屋）代码	 
                            string dzwdm = dic.房屋幢号9.PadLeft(4, '0') + "0001";
                            grid1.Cell(2, 15).Text = dzwdm;
                            //不动产单元号
                            string BDCDYH = wzdjh.Substring(0, 6) + "  " + wzdjh.Substring(6, 6) + "  " +
                                           wzdjh.Substring(12, 7) + "  " + "F00010001";
                            grid1.Cell(3, 2).Text = BDCDYH;
                            //项目名称	
                            grid1.Cell(8, 9).Text = reader["XZGHBMFZR"].ToString().Trim();
                            //房屋性质	
                            if (dic.房屋性质2.Trim() == "")
                            {
                                grid1.Cell(9, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(9, 2).Text = dic.房屋性质2.Trim();
                            }
                            if (dic.房屋产别3.Trim() == "")
                            {
                                grid1.Cell(9, 9).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(9, 9).Text = dic.房屋产别3.Trim();
                            }
                            if (dic.房屋用途4.Trim() == "")
                            {
                                grid1.Cell(10, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(10, 2).Text = dic.房屋用途4.Trim();
                            }
                            if (dic.规划用途5.Trim() == "")
                            {
                                grid1.Cell(10, 9).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(10, 9).Text = dic.规划用途5.Trim();
                            }
                            if (dic.共有情况8.Trim() == "")
                            {
                                grid1.Cell(7, 12).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(7, 12).Text = dic.共有情况8.Trim();
                            }
                            if (dic.附加说明18.Trim() == "")
                            {
                                grid1.Cell(27, 12).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(27, 12).Text = dic.附加说明18.Trim();
                            }
                            if (dic.调查意见20.Trim() == "")
                            {
                                grid1.Cell(28, 12).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(28, 12).Text = dic.调查意见20.Trim();
                            }
                            if (dic.调查员21.Trim() == "")
                            {
                                grid1.Cell(29, 2).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(29, 2).Text = dic.调查员21.Trim();
                            }
                            if (dic.调查日期19.Trim() == "")
                            {
                                grid1.Cell(29, 12).Text = "/";
                            }
                            else
                            {
                                grid1.Cell(29, 12).Text = dic.调查日期19.Trim();
                            }

                            //grid1.Cell(27, 2).Text = "  本宗地建筑总面积= " + jzmj.ToString("0.00") + " 平方米" + "\r\n" + "\r\n" +
                            //                         "  本宗地房屋建筑占地总面积= " + jzzdmj.ToString("0.00") + " 平方米";

                            #endregion
                            int jl = (p + 1) * 12;
                            if (jl > ZCS)
                            {
                                jl = ZCS;
                            }
                            int qsjl = 15;
                            #region 循环写入各栋、各层
                            for (int m = p * 12; m < jl; m++)//写各层的数据
                            {
                                FWSXJG dic2 = TZLC[m];
                                grid1.Cell(qsjl, 2).Text = "1";//幢号
                                grid1.Cell(qsjl, 3).Text = "1";//户号
                                grid1.Cell(qsjl, 4).Text = dic2.房屋幢号9.Trim();//自然幢
                                grid1.Cell(qsjl, 5).Text = dic2.总层数11.Trim();
                                grid1.Cell(qsjl, 6).Text = dic2.所在层数12.Trim();
                                #region 房屋结构
                                string fwjg = dic2.房屋结构13.Trim();
                                string jhjg = "";
                                if (fwjg == "钢结构")
                                {
                                    jhjg = "钢";
                                }
                                if (fwjg == "钢、钢筋混凝土结构")
                                {
                                    jhjg = "钢、砼";
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
                                if (fwjg == "")
                                {
                                    jhjg = "/";
                                }
                                grid1.Cell(qsjl, 7).Text = jhjg;
                                #endregion
                                if (dic2.竣工时间14.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 8).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 8).Text = dic2.竣工时间14.Trim();
                                }

                                if (dic2.所在层数12 == "1")
                                {
                                    grid1.Cell(qsjl, 9).Text = dic2.本层建筑面积29.ToString("0.00");
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 9).Text = "/";
                                }
                                grid1.Cell(qsjl, 10).Text = dic2.本层建筑面积29.ToString("0.00");
                                if (dic2.专有建筑面积15.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 11).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 11).Text = dic2.专有建筑面积15.Trim();
                                }
                                if (dic2.分摊建筑面积16.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 12).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 12).Text = dic2.分摊建筑面积16.Trim();
                                }
                                if (dic2.产权来源7.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 13).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 13).Text = dic2.产权来源7.Trim();
                                }
                                if (dic2.北23.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 14).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 14).Text = dic2.北23.Trim();
                                }
                                if (dic2.东24.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 15).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 15).Text = dic2.东24.Trim();
                                }

                                if (dic2.南25.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 16).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 16).Text = dic2.南25.Trim();
                                }
                                if (dic2.西26.Trim() == "")
                                {
                                    grid1.Cell(qsjl, 17).Text = "/";
                                }
                                else
                                {
                                    grid1.Cell(qsjl, 17).Text = dic2.西26.Trim();
                                }
                                qsjl++;
                            }

                            #endregion
                            if (SCYL.Checked)
                            {
                                grid1.PrintPreview(true, true, true, 1, 0, 0, width, height);
                            }
                            else
                            {
                                grid1.Print(1, 100, true);
                            }
                        }

                        #endregion
                    }
                    connection.Close();
                    reader.Dispose();
                }
            }
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
        private void DLBM_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            SJYT.Text = find_dlmc(DLBM.Text);
        }

        // 申请书（一） - “使用上次值”按钮
        private void SYSCZ_Click(object sender, EventArgs e)
        {
            SQRQ.Text = ATT.SQRQ_m;
            SQRLX.SelectedIndex = -1;
            SQRLX.SelectedItem = ATT.SQRLX_m;
            SQRLX2.SelectedIndex = -1;
            SQRLX2.SelectedItem = ATT.SQRLX2_m;
            LXR.Text = ATT.LXR_m;//收 件 人
            TDZL.Text = ATT.TDZL_m;//坐落
            TEXTBZ1.Text = ATT.TEXTBZ1_m;
            YZBM.Text = ATT.YZBM_m;
            TXDZ.Text = ATT.TXDZ_m;
            XZGHBMFZR.Text = ATT.XZGHBMFZR_m;
        }

        // 图标输出 - “重新将房屋数据加入内存”
        private void button20_Click_1(object sender, EventArgs e)
        {
            Write_FW_SX_to_LC();
        }

        private void TDZL_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            TXDZ.Text = TDZL.Text;
            TEXTBZ1.Text = TDZL.Text;
        }

        private void FZMJ_TextChanged(object sender, EventArgs e)
        {
            TDDYMJ.Text = FZMJ.Text;
        }


        private void SQRQ_TextChanged(object sender, EventArgs e)
        {

            if (SQRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(SQRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“收件日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }

        private void DYSQRJZRQ_TextChanged(object sender, EventArgs e)
        {
            if (DYSQRJZRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(DYSQRJZRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“第一申请人签章日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void DESQRJZRQ_TextChanged(object sender, EventArgs e)
        {
            if (DESQRJZRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(DESQRJZRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“第二申请人签章日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void DCBRQ_TextChanged(object sender, EventArgs e)
        {
            if (DCBRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(DCBRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“调查日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void SYQX_Q_TextChanged(object sender, EventArgs e)
        {
            if (SYQX_Q.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(SYQX_Q.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“使用期限  自日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void SYQX_Z_TextChanged(object sender, EventArgs e)
        {
            if (SYQX_Z.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(SYQX_Z.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“使用期限  止 日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void ZJRQ_TextChanged(object sender, EventArgs e)
        {
            if (ZJRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(ZJRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“指界日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void DCRQ_TextChanged(object sender, EventArgs e)
        {
            if (DCRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(DCRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“调查记事日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void CLRQ_TextChanged(object sender, EventArgs e)
        {
            if (CLRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(CLRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“测量日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void SHRQ_TextChanged(object sender, EventArgs e)
        {
            if (SHRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(SHRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“审核日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void CSRJ_TextChanged(object sender, EventArgs e)
        {
            if (CSRJ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(CSRJ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“初  审日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void SHRQ_SPB_TextChanged(object sender, EventArgs e)
        {
            if (SHRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(SHRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“复  审日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void PZRQ_TextChanged(object sender, EventArgs e)
        {
            if (PZRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(PZRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“核  定日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void DJKRQ_TextChanged(object sender, EventArgs e)
        {
            if (DJKRQ.Text.Trim() == "" && YXRQWK.Checked) { return; }
            try
            {
                Convert.ToDateTime(DJKRQ.Text.Trim());
            }
            catch
            {
                MessageBox.Show("“登记卡、归户卡选项日期”格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        // 计算日历对话框的插入点坐标
        /// <summary>
        /// 计算日历对话框的插入点坐标
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public System.Drawing.Point GetDate(System.Drawing.Point startPoint, int Width, int Height)
        {
            //System.Drawing.Point   startPoint = PointToScreen(button1.Location);
            int PM_width = System.Windows.Forms.SystemInformation.WorkingArea.Width;
            int PM_height = System.Windows.Forms.SystemInformation.WorkingArea.Height;


            int X = 0, Y = 0;
            if (startPoint.Y < Height)
            {
                Y = startPoint.Y + button1.Height;
            }
            else if (startPoint.Y + Height >= PM_height)
            {
                Y = startPoint.Y - Height;
            }
            else
            {
                Y = startPoint.Y + button1.Height;
            }

            if (startPoint.X < 0)
            {
                X = 0;
            }
            else if (startPoint.X + Width > PM_width)
            {
                X = startPoint.X - Width;
            }
            else
            {
                X = startPoint.X;
            }
            return new System.Drawing.Point(X, Y);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button5.Location);
            startPoint.Y += button5.Parent.Location.Y;
            startPoint.X += button5.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            SQRQ.Text = date.sj;
        }

        private void button39_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button39.Location);
            startPoint.Y += button39.Parent.Location.Y;
            startPoint.X += button39.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            DYSQRJZRQ.Text = date.sj;
        }

        private void button42_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button42.Location);
            startPoint.Y += button42.Parent.Location.Y;
            startPoint.X += button42.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            DESQRJZRQ.Text = date.sj;
        }

        private void button34_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button34.Location);
            startPoint.Y += button34.Parent.Location.Y;
            startPoint.X += button34.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            DCBRQ.Text = date.sj;
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button23.Location);
            startPoint.Y += button23.Parent.Location.Y;
            startPoint.X += button23.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            SYQX_Q.Text = date.sj;
        }

        private void button36_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button36.Location);
            startPoint.Y += button36.Parent.Location.Y;
            startPoint.X += button36.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            SYQX_Z.Text = date.sj;
        }

        private void button38_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button38.Location);
            startPoint.Y += button38.Parent.Location.Y;
            startPoint.X += button38.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            ZJRQ.Text = date.sj;
        }

        private void button33_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button33.Location);
            startPoint.Y += button33.Parent.Location.Y;
            startPoint.X += button33.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            DCRQ.Text = date.sj;
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button21.Location);
            startPoint.Y += button21.Parent.Location.Y;
            startPoint.X += button21.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            CLRQ.Text = date.sj;
        }

        private void button35_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button35.Location);
            startPoint.Y += button35.Parent.Location.Y;
            startPoint.X += button35.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            SHRQ.Text = date.sj;
        }

        private void button37_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button37.Location);
            startPoint.Y += button37.Parent.Location.Y;
            startPoint.X += button37.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            CSRJ.Text = date.sj;
        }

        private void button41_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button41.Location);
            startPoint.Y += button41.Parent.Location.Y;
            startPoint.X += button41.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            SHRQ_SPB.Text = date.sj;
        }

        private void button29_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button29.Location);
            startPoint.Y += button29.Parent.Location.Y;
            startPoint.X += button29.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            PZRQ.Text = date.sj;
        }

        private void button40_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button40.Location);
            startPoint.Y += button40.Parent.Location.Y;
            startPoint.X += button40.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            DJKRQ.Text = date.sj;
        }

        private void TDZL_TextChanged(object sender, EventArgs e)
        {
            TXDZ.Text = TDZL.Text.Trim();
            TEXTBZ1.Text = TDZL.Text.Trim();
        }

        private void KCR_Click(object sender, EventArgs e)
        {
            if (KCR.Text == "" && ZRXM.Text == "居民身份证")
            {
                KCR.Text = treeView1.SelectedNode.Tag.ToString().Substring(0, 6) + "19";
                KCR.SelectionStart = 8;
            }
        }

        // 界址点结构信息
        /// <summary>
        /// 界址点结构信息
        /// </summary>
        public struct JZDXX
        {
            public int jzdh;
            public Point2d point;
        }

        // “图库一致性”按钮
        /// <summary>
        /// 图、库一致性检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click_1(object sender, EventArgs e)
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

            arrerr.Clear();
            arrtext.Clear();
            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            int xh = 0;
            string XZDM = treeView1.SelectedNode.Tag.ToString();
            List<string> ArrZDD = new List<string>();//数据库中的宗地号
            List<string> ArrZDD_TX = new List<string>();//图形中的宗地号
            List<ACDBPolyline> Arr_JZX = new List<ACDBPolyline>();//图形中的界址线实体
            List<Circle> Arr_JZD = new List<Circle>();//图形中的界址点实体
            List<JZDXX> JZD_TX = new List<JZDXX>();//图形中的界址点

            #region 查找此街坊的所有宗地
            using (SqlConnection Sqlcon = new SqlConnection())//定义连接
            {
                Sqlcon.ConnectionString = connectionString;
                string queryString = "SELECT * " + "FROM DCB " + "WHERE (DJH LIKE " + "'" + XZDM + "%%%%%" + "')";//查询字符串
                SqlCommand command = new SqlCommand(queryString, Sqlcon);//连接数据库
                Sqlcon.Open();
                if (Sqlcon.State == ConnectionState.Broken)
                {
                    MessageBox.Show("数据库连接失败！");
                    return;
                }
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ArrZDD.Add(reader["DJH"].ToString().Trim().Substring(6, 11));
                }
                reader.Close();
                Sqlcon.Close();
            }
            #endregion
            #region 创建选择集获取界址线、界址点数据
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
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
            ProgressMeter progressmeter = new ProgressMeter();
            progressmeter.Start("正在进行一一对应检查...");
            progressmeter.SetLimit(objectId.Length);
            #region 将界址点信息写入数组
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForRead, true);
                JZDXX jzdxx = new JZDXX();
                for (int i = 0; i < objectId_jzd.Length; i++)
                {
                    Circle cir = (Circle)trans.GetObject(objectId_jzd[i], ACDBOpenMode.ForRead);
                    ResultBuffer resultBuffer = cir.GetXDataForApplication("SOUTH");
                    TypedValue[] tv3 = resultBuffer.AsArray();
                    if (tv3.Length != 5 || tv3[4].Value.ToString() != "BDC")
                    {
                        MessageBox.Show("界址点属性有错误，请先检查界址点属性！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    jzdxx.jzdh = (int)tv3[3].Value;
                    jzdxx.point = new Point2d(cir.Center.X, cir.Center.Y);
                    JZD_TX.Add(jzdxx);
                    Arr_JZD.Add(cir);
                }
                trans.Commit();
                trans.Dispose();
            }
            #endregion
            #region 检查图形中的宗地在数据库中是否存在
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForRead, true);
                for (int i = 0; i < objectId.Length; i++)
                {
                    ACDBPolyline JZX;
                    JZX = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForRead);

                    int DDS = JZX.NumberOfVertices;
                    Extents3d JZXBounds = (Extents3d)JZX.Bounds;
                    ResultBuffer resultBuffer = JZX.GetXDataForApplication("SOUTH");
                    Extents3d point = (Extents3d)JZX.Bounds;
                    try
                    {
                        TypedValue[] tv3 = resultBuffer.AsArray();
                        if (tv3[1].Value.ToString().Trim() != "300000") { continue; }//如果是街坊界
                        string djh = tv3[2].Value.ToString();
                        string jdh = djh.Substring(0, 3);
                        string jfh = djh.Substring(3, 3);
                        string zdh = djh.Substring(6, djh.Length - 6);
                        zdh = zdh.PadLeft(5, '0');
                        zdh = jdh + jfh + zdh;
                        ArrZDD_TX.Add(zdh);
                        Arr_JZX.Add(JZX);

                        if (ArrZDD.IndexOf(zdh) < 0)
                        {
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "CAD实体对象";
                            er.cwsm = "数据库中无此宗地，此宗地未入库！。";
                            er.minx = point.MinPoint.X;
                            er.miny = point.MinPoint.Y;
                            er.maxX = point.MaxPoint.X;
                            er.maxy = point.MaxPoint.Y;
                            er.jb = JZX.Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        else
                        {

                        }
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("界址线属性有错误，请在第二步修改无问题后在重新生成界址点。", "地籍处理工具", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }

                }
                trans.Commit();
                trans.Dispose();
            }
            #endregion
            #region 检查数据库中的宗地在图形中是否存在
            for (int i = 0; i < ArrZDD.Count; i++)
            {
                Extents3d point = (Extents3d)Arr_JZX[0].Bounds;
                if (ArrZDD_TX.IndexOf(ArrZDD[i]) < 0)
                {
                    xh++;
                    err er = new err();
                    er.xh = xh;
                    er.xz = "必须修改";
                    er.dxlx = "数据库中的对象";
                    er.cwsm = "数据库中宗地号为 " + ArrZDD[i] + " 的宗地在图形中不存在！";
                    er.minx = point.MinPoint.X - 500;
                    er.miny = point.MinPoint.Y - 500;
                    er.maxX = point.MaxPoint.X + 500;
                    er.maxy = point.MaxPoint.Y + 500;
                    er.jb = "-1";
                    arrerr.Add(er);
                    continue;
                }
            }
            #endregion
            #region 检查界址点号一一对应 、检查数据库中点和图形界址线中的点一一对应
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                for (int i = 0; i < ArrZDD.Count; i++)
                {
                    progressmeter.MeterProgress();
                    List<JZDXX> JZD_SJK = new List<JZDXX>();//数据库中的界址点
                    #region 从数据库中获取宗地信息
                    JZDXX jzdxx = new JZDXX();
                    string zdh = XZDM.Substring(0, 6) + ArrZDD[i];
                    string selstring_ZDD = "SELECT * FROM D_ZDD  WHERE (DJH = '" + zdh + "')";
                    SqlCommand command = new SqlCommand(selstring_ZDD, connection);//连接数据库
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        jzdxx.jzdh = (int)reader["JZDH"];
                        jzdxx.point = new Point2d((double)reader["X"], (double)reader["Y"]);
                        JZD_SJK.Add(jzdxx);//单个宗地
                    }
                    reader.Close();
                    #endregion
                    #region 一一对应检查
                    int ii = ArrZDD_TX.IndexOf(ArrZDD[i]);
                    if (ii >= 0)
                    {
                        if (JZD_SJK.Count != Arr_JZX[ii].NumberOfVertices + 1)
                        {
                            Extents3d point = (Extents3d)Arr_JZX[ii].Bounds;
                            xh++;
                            err er = new err();
                            er.xh = xh;
                            er.xz = "必须修改";
                            er.dxlx = "CAD实体对象";
                            er.cwsm = "数据库中的界址点个数和图形中的界址点个数不一致！。";
                            er.minx = point.MinPoint.X;
                            er.miny = point.MinPoint.Y;
                            er.maxX = point.MaxPoint.X;
                            er.maxy = point.MaxPoint.Y;
                            er.jb = Arr_JZX[ii].Handle.Value.ToString();
                            arrerr.Add(er);
                            continue;
                        }
                        for (int p = 0; p < JZD_SJK.Count - 1; p++)
                        {
                            int nn = p + 1;
                            if (JZD_SJK[p].point.GetDistanceTo(Arr_JZX[ii].GetPoint2dAt(p)) > 0.001)
                            {
                                Extents3d point = (Extents3d)Arr_JZX[ii].Bounds;
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "CAD实体对象";
                                er.cwsm = "数据库中的第 " + nn + " 号界址点与图形中的第 " + nn + " 号界址点位置不一致，必须重新入库！。";
                                er.minx = point.MinPoint.X;
                                er.miny = point.MinPoint.Y;
                                er.maxX = point.MaxPoint.X;
                                er.maxy = point.MaxPoint.Y;
                                er.jb = Arr_JZX[ii].Handle.Value.ToString();
                                arrerr.Add(er);
                            }
                        }
                        for (int p = 0; p < JZD_SJK.Count - 1; p++)
                        {
                            bool isfx = false;
                            for (int k = 0; k < JZD_TX.Count; k++)
                            {
                                if (JZD_SJK[p].jzdh == JZD_TX[k].jzdh)
                                {
                                    isfx = true;
                                    if ((JZD_SJK[p].point.GetDistanceTo(JZD_TX[k].point)) > 0.001)
                                    {

                                        xh++;
                                        err er = new err();
                                        er.xh = xh;
                                        er.xz = "必须修改";
                                        er.dxlx = "CAD实体对象";
                                        er.cwsm = JZD_SJK[p].jzdh + " 号界址点，数据库中坐标和图形中的坐标不一致！";
                                        er.minx = JZD_TX[k].point.X - 10;
                                        er.miny = JZD_TX[k].point.Y - 10;
                                        er.maxX = JZD_TX[k].point.X + 10;
                                        er.maxy = JZD_TX[k].point.Y + 10;
                                        er.jb = Arr_JZD[k].Handle.Value.ToString();
                                        arrerr.Add(er);
                                    }
                                }
                            }
                            if (!isfx)
                            {
                                xh++;
                                err er = new err();
                                er.xh = xh;
                                er.xz = "必须修改";
                                er.dxlx = "CAD实体对象";
                                er.cwsm = JZD_SJK[p].jzdh + " 号界址点在图形中不存在！";
                                er.minx = JZD_SJK[p].point.X - 10;
                                er.miny = JZD_SJK[p].point.Y - 10;
                                er.maxX = JZD_SJK[p].point.X + 10;
                                er.maxy = JZD_SJK[p].point.Y + 10;
                                er.jb = "-1";
                                arrerr.Add(er);
                            }
                        }
                    }
                    #endregion
                }
                connection.Close();
            }
            #endregion
            progressmeter.Stop();
            Class1.WritErr(arrerr);

        }

        private void XZGHBMFZR_TextChanged(object sender, EventArgs e)
        {
            string tdzl = treeView1.SelectedNode.FullPath;//土地坐落
            string[] zl = tdzl.Split('\\');
            tdzl = zl[2] + zl[3] + zl[4];
            TDZL.Text = tdzl + XZGHBMFZR.Text.Trim();
        }

        private void button13_Click_2(object sender, EventArgs e)
        {
            int n = BGLB.SelectedIndex;
            if (n >= 0)
            {
                object n_item = BGLB.Items[n];
                bool xz = BGLB.GetItemChecked(n);
                int m = n - 1;
                BGLB.Items.RemoveAt(n);
                if (m < 0)
                {
                    m = 0;
                }
                BGLB.Items.Insert(m, n_item);
                BGLB.SelectedIndex = m;
                BGLB.SetItemChecked(m, xz);
            }
        }

        private void button16_Click_2(object sender, EventArgs e)
        {
            int n = BGLB.SelectedIndex;
            if (n >= 0)
            {
                object n_item = BGLB.Items[n];
                bool xz = BGLB.GetItemChecked(n);
                BGLB.Items.RemoveAt(n);
                int m = n + 1;
                if (m >= BGLB.Items.Count)
                {
                    m = BGLB.Items.Count;
                }
                BGLB.Items.Insert(m, n_item);
                BGLB.SelectedIndex = m;
                BGLB.SetItemChecked(m, xz);
            }
        }


        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        // 图表输出 - 上面表格输出的“执行”按钮
        private void button17_Click_2(object sender, EventArgs e)
        {
            string cq = Tools.ReadWriteReg.read_reg("测区名称");

            if (HZLB.SelectedIndex != -1)
            {
                if (HZLB.SelectedItems.Count > 0)
                {

                    for (int i = 0; i < HZLB.SelectedItems.Count; i++)
                    {
                        for (int j = 0; j < BGLB.Items.Count; j++)
                        {
                            if (BGLB.GetItemChecked(j) == true)
                            {
                                string xmmc = BGLB.GetItemText(BGLB.Items[j]);

                                switch (cq)
                                {
                                    case "阜阳":
                                        #region 输出表格
                                        if (xmmc == "申请审批表")
                                        {
                                            X_SQSBB(i);
                                        }
                                        if (xmmc == "调查表")
                                        {
                                            X_DCB(i);
                                        }
                                        if (xmmc == "房屋调查表")
                                        {
                                            X_FWDCCB(i);
                                        }
                                        if (xmmc == "宗地草图")
                                        {
                                            X_ZDCT(i, false);
                                        }
                                        if (xmmc == "宗地图")
                                        {
                                            X_ZDT(i, true);
                                        }
                                        if (xmmc == "分层分户图")
                                        {
                                            X_FCFHT(i, true);
                                        }
                                        if (xmmc == "界址点坐标册")
                                        {
                                            X_ZBC(i);
                                        }
                                        if (xmmc == "权属来源证明")
                                        {
                                            X_QSLYZM(i);
                                        }
                                        if (xmmc == "具结承诺证明书")
                                        {
                                            X_JJCLZMS(i);
                                        }
                                        if (xmmc == "建房申请审批表")
                                        {
                                            X_JFSQSPB(i);
                                        }
                                        if (xmmc == "档案封面")
                                        {// TODO:
                                            X_DAFM(i);
                                        }
                                        #endregion
                                        break;
                                    case "金寨":
                                        #region 输出表格
                                        if (xmmc == "申请审批表")
                                        {
                                            X_SQSBB(i);
                                        }
                                        if (xmmc == "调查表")
                                        {
                                            X_DCB(i);
                                        }
                                        if (xmmc == "房屋调查表")
                                        {
                                            X_FWDCCB(i);
                                        }
                                        if (xmmc == "宗地草图")
                                        {
                                            X_ZDCT(i, false);
                                        }
                                        if (xmmc == "宗地图")
                                        {
                                            X_ZDT(i, true);
                                        }
                                        if (xmmc == "分层分户图")
                                        {
                                            X_FCFHT(i, true);
                                        }
                                        if (xmmc == "界址点坐标册")
                                        {
                                            X_ZBC(i);
                                        }
                                        if (xmmc == "权属来源证明")
                                        {
                                            X_QSLYZM(i);
                                        }
                                        if (xmmc == "具结承诺证明书")
                                        {
                                            X_JJCLZMS(i);
                                        }
                                        if (xmmc == "建房申请审批表")
                                        {
                                            X_JFSQSPB(i);
                                        }

                                        #endregion
                                        break;
                                    case "芜湖":
                                        #region 输出表格
                                        if (xmmc == "申请书和审批表")
                                        {
                                            X_SQS_SPB_GB(i);
                                        }
                                        if (xmmc == "调查表")
                                        {
                                            X_DCB(i);
                                        }
                                        if (xmmc == "房屋调查表")
                                        {
                                            X_FWDCCB_GB(i);
                                        }
                                        if (xmmc == "宗地草图")
                                        {
                                            X_ZDCT(i, false);
                                        }
                                        if (xmmc == "宗地图")
                                        {
                                            X_ZDT(i, true);
                                        }
                                        if (xmmc == "分层分户图")
                                        {
                                            X_FCFHT(i, true);
                                        }
                                        if (xmmc == "界址点坐标册")
                                        {
                                            X_ZBC(i);
                                        }

                                        if (xmmc == "权属来源证明")
                                        {
                                            X_QSLYZM_WH(i);
                                        }

                                        if (xmmc == "申请书与证明书")//if (xmmc == "具结承诺证明书")
                                        {// TODO:
                                            X_JJCLZMS(i);
                                        }

                                        if (xmmc == "建房申请审批表")
                                        {
                                            X_JFSQSPB(i);
                                        }

                                        #endregion
                                        break;
                                    case "宣城":
                                        #region 输出表格
                                        if (xmmc == "申请书和审批表")
                                        {
                                            X_SQS_SPB_GB(i);
                                        }
                                        if (xmmc == "调查表")
                                        {
                                            X_DCB(i);
                                        }
                                        if (xmmc == "房屋调查表")
                                        {
                                            X_FWDCCB_GB(i);
                                        }
                                        if (xmmc == "宗地草图")
                                        {
                                            X_ZDCT(i, false);
                                        }
                                        if (xmmc == "宗地图")
                                        {
                                            X_ZDT(i, true);
                                        }
                                        if (xmmc == "分层分户图")
                                        {
                                            X_FCFHT(i, true);
                                        }
                                        if (xmmc == "界址点坐标册")
                                        {
                                            X_ZBC(i);
                                        }
                                        if (xmmc == "确认申请审核表")
                                        {// TODO:
                                            //X_QSLYZM(i);
                                        }

                                        #endregion
                                        break;
                                    default:
                                        #region 输出表格
                                        if (xmmc == "申请书和审批表")
                                        {
                                            X_SQS_SPB_GB(i);
                                        }
                                        if (xmmc == "调查表")
                                        {
                                            X_DCB(i);
                                        }
                                        if (xmmc == "房屋调查表")
                                        {
                                            X_FWDCCB_GB(i);
                                        }
                                        if (xmmc == "宗地草图")
                                        {
                                            X_ZDCT(i, false);
                                        }
                                        if (xmmc == "宗地图")
                                        {
                                            X_ZDT(i, true);
                                        }
                                        if (xmmc == "分层分户图")
                                        {
                                            X_FCFHT(i, true);
                                        }
                                        if (xmmc == "界址点坐标册")
                                        {
                                            X_ZBC(i);
                                        }
                                        if (xmmc == "权属来源证明")
                                        {
                                            X_QSLYZM(i);
                                        }
                                        if (xmmc == "具结承诺证明书")
                                        {
                                            X_JJCLZMS(i);
                                        }
                                        if (xmmc == "建房申请审批表")
                                        {
                                            X_JFSQSPB(i);
                                        }
                                        if (xmmc == "档案封面")
                                        {
                                            X_DAFM(i);
                                        }
                                        #endregion
                                        break;


                                }


                            }
                        }
                    }
                }
            }
            else
            {

            }
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

        // ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        // 图表输出 - “公示报表”按钮
        private void button22_Click(object sender, EventArgs e)
        {
            X_GSBB();
        }
        // ↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

        private void XG_CheckedChanged(object sender, EventArgs e)
        {
            if (XG.Checked)
            {
                HZLB.SelectionMode = System.Windows.Forms.SelectionMode.One;
            }
        }

        // 获取分层分户图列表
        /// <summary>
        /// 获取分层分户图列表
        /// </summary>
        public List<FCFHLB> HQFCFHTLB()
        {
            if (ATT.DJH == "")
            {
                MessageBox.Show("请选择宗地(双击)!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return null;
            }
            if (FCHE_fwsxjg.Count == 0)
            {
                FCFH_Write_FW_SX_to_LC();
            }
            List<FWSXJG> fcfh_sx = new List<FWSXJG>();//属性
            List<Topology.Geometries.Polygon> fcfh_Polygon = new List<Topology.Geometries.Polygon>();//几何图形
            for (int i = 0; i < FCHE_fwsxjg.Count; i++)
            {
                string djh = ATT.DJH.Substring(6, 11);
                if (FCHE_fwsxjg[i].地籍号1 == djh)
                {
                    fcfh_sx.Add(FCHE_fwsxjg[i]);
                    fcfh_Polygon.Add(FCHE_Polygon[i]);
                }
            }
            List<FWSXJG> fcfh_sx_FW = new List<FWSXJG>();//属性
            List<Topology.Geometries.Polygon> fcfh_Polygon_FW = new List<Topology.Geometries.Polygon>();//几何图形t
            #region 获取房屋等
            for (int i = 0; i < fcfh_sx.Count; i++)
            {
                if (fcfh_sx[i].建筑物类型0 == "房屋" || fcfh_sx[i].建筑物类型0 == "门廊" || fcfh_sx[i].建筑物类型0 == "车棚")
                {
                    fcfh_sx_FW.Add(fcfh_sx[i]);
                    fcfh_Polygon_FW.Add(fcfh_Polygon[i]);
                }
            }
            #endregion
            #region 排序
            for (int i = 0; i < fcfh_sx_FW.Count; i++)
            {
                int dh1 = Convert.ToInt32(fcfh_sx_FW[i].房屋幢号9);
                for (int j = i + 1; j < fcfh_sx_FW.Count; j++)
                {
                    FWSXJG sx2 = fcfh_sx_FW[j];
                    int dh2 = Convert.ToInt32(sx2.房屋幢号9);
                    if (dh1 >= dh2)
                    {
                        FWSXJG gd = fcfh_sx_FW[i];
                        Topology.Geometries.Polygon jhtx = fcfh_Polygon_FW[i];
                        fcfh_sx_FW[i] = sx2;
                        fcfh_sx_FW[j] = gd;
                        fcfh_Polygon_FW[i] = fcfh_Polygon_FW[j];
                        fcfh_Polygon_FW[j] = jhtx;
                        dh1 = Convert.ToInt32(fcfh_sx_FW[i].房屋幢号9);
                    }
                }
            }
            for (int i = 0; i < fcfh_sx_FW.Count; i++)
            {

                int dh1 = Convert.ToInt32(fcfh_sx_FW[i].房屋幢号9);
                int czcs1 = Convert.ToInt32(fcfh_sx_FW[i].所在层数12);
                for (int j = i + 1; j < fcfh_sx_FW.Count; j++)
                {

                    int dh2 = Convert.ToInt32(fcfh_sx_FW[j].房屋幢号9);
                    int czcs2 = Convert.ToInt32(fcfh_sx_FW[j].所在层数12);
                    if (dh1 == dh2 && czcs1 >= czcs2)
                    {
                        FWSXJG gd = fcfh_sx_FW[i];
                        Topology.Geometries.Polygon jhtx = fcfh_Polygon_FW[i];
                        fcfh_sx_FW[i] = fcfh_sx_FW[j];
                        fcfh_sx_FW[j] = gd;
                        fcfh_Polygon_FW[i] = fcfh_Polygon_FW[j];
                        fcfh_Polygon_FW[j] = jhtx;
                        dh1 = Convert.ToInt32(fcfh_sx_FW[i].房屋幢号9);
                        czcs1 = Convert.ToInt32(fcfh_sx_FW[i].所在层数12);
                    }
                }
            }
            #endregion
            #region 添加列表
            int tfs = 1 + (int)((fcfh_sx_FW.Count - 0.5) / 6);//图幅数
            int gs = fcfh_sx_FW.Count;

            List<FCFHLB> fcfh_lb = new List<FCFHLB>();
            for (int i = 0; i < tfs; i++)
            {
                string tfm = ATT.DJH + "分层分户图" + "-" + i;
                List<FWSXJG> fcfh_sx_t = new List<FWSXJG>();
                List<Topology.Geometries.Polygon> fcfh_Polygon_t = new List<Topology.Geometries.Polygon>();

                if (i + 1 != tfs)
                {
                    for (int j = i * 6; j < 6 * (i + 1); j++)
                    {
                        fcfh_sx_t.Add(fcfh_sx_FW[j]);
                        fcfh_Polygon_t.Add(fcfh_Polygon_FW[j]);
                    }
                    gs = 6;
                }
                else
                {
                    for (int j = i * 6; j < fcfh_sx_FW.Count; j++)
                    {
                        fcfh_sx_t.Add(fcfh_sx_FW[j]);
                        fcfh_Polygon_t.Add(fcfh_Polygon_FW[j]);
                    }
                    gs = fcfh_sx_FW.Count - (tfs - 1) * 6;
                }
                int xh = fcfh_sx_t.Count;
                for (int j = 0; j < xh; j++)
                {
                    int cs = Convert.ToInt32(fcfh_sx_t[j].所在层数12);
                    int dh = Convert.ToInt32(fcfh_sx_t[j].房屋幢号9);
                    for (int jj = 0; jj < fcfh_sx.Count; jj++)
                    {
                        string lx = fcfh_sx[jj].建筑物类型0;
                        int cs2 = Convert.ToInt32(fcfh_sx[jj].所在层数12);
                        int dh2 = Convert.ToInt32(fcfh_sx[jj].房屋幢号9);
                        if (lx == "阳台" || lx == "楼梯" || lx == "檐廊")
                        {
                            if (cs == cs2 && dh == dh2)
                            {
                                fcfh_sx_t.Add(fcfh_sx[jj]);
                                fcfh_Polygon_t.Add(fcfh_Polygon[jj]);
                            }
                        }
                    }
                }
                FCFHLB fcfh = new FCFHLB(tfm, gs, fcfh_sx_t, fcfh_Polygon_t);
                fcfh_lb.Add(fcfh);
            }
            Add_FCFHT_LB(fcfh_lb);
            return fcfh_lb;
            #endregion

        }

        // 图表输出 - “获取列表”按钮
        private void button24_Click(object sender, EventArgs e)
        {
            HQFCFHTLB();//获取分层分户图列表
        }

        // 增加邻宗地列表
        /// <summary>
        /// 增加邻宗地列表
        /// </summary>
        /// <param name="Lzdsx"></param>
        public void Add_FCFHT_LB(List<FCFHLB> Lzdsx)
        {
            this.FCFHTLB.DataSource = null;
            this.FCFHTLB.Items.Clear();
            this.FCFHTLB.DataSource = Lzdsx;
            this.FCFHTLB.ValueMember = "gs";//属性值
            this.FCFHTLB.DisplayMember = "tfm";//显示名称
            this.FCFHTLB.EndUpdate();
            this.FCFHTLB.ClearSelected();//清除选择
        }

        // 根据界址线计算图幅号
        /// <summary>
        /// 根据界址线计算图幅号
        /// </summary>
        /// <param name="JZX">界址线</param>
        /// <returns>图幅号名称</returns>
        public string jstfh(ACDBPolyline JZX)
        {
            string strblc = Tools.ReadWriteReg.read_reg("比例尺分母");
            double blc = 1000;
            if (strblc != "")
            {
                blc = Convert.ToInt32(strblc);
            }
            Extents3d point = (Extents3d)JZX.Bounds;
            Point3d pot = new Point3d((point.MaxPoint.X + point.MinPoint.X) / 2.0, (point.MaxPoint.Y + point.MinPoint.Y) / 2.0, 0);

            // Topology.Geometries.IPoint point = jzxPolygon.Centroid;
            int Xzb = (int)((int)(pot.X / (blc / 2.0)) * (blc / 20));
            int Yzb = (int)((int)(pot.Y / (blc / 2.0)) * (blc / 20));

            string sXzb = Xzb.ToString().PadLeft(5, '0');
            sXzb = sXzb.Substring(sXzb.Length - 5, 5);
            string sYzb = Yzb.ToString().PadLeft(5, '0');
            sYzb = sYzb.Substring(sYzb.Length - 5, 5);
            return sYzb.Insert(sYzb.Length - 2, ".") + "-" +
                         sXzb.Insert(sXzb.Length - 2, ".");
        }

        // "数据入库"按钮
        private void button25_Click(object sender, EventArgs e)
        {
            #region 条件检查
            if (Tools.ReadWriteReg.read_reg("界标种类") == "")
            {
                MessageBox.Show("请在系统设置里选择默认“界标种类”", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
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
            #region 选择街坊宗地数据
            ZDRK zdrk = new ZDRK();
            zdrk.ShowDialog();
            if (zdrk.DialogResult != DialogResult.OK)
            {
                return;
            }
            #endregion
            #region 定义变量
            string XZDM = treeView1.SelectedNode.Tag.ToString();
            string JDH = XZDM.Substring(6, 3);
            string JFH = XZDM.Substring(9, 3);
            string XZQH = XZDM.Substring(0, 6);

            string connectionString = Tools.Uitl.LJstring();//写连接字符串
            //arrerr.Clear();
            //int xh = 0;
            ArrayList arrJZD = new ArrayList();//界址点
            List<Topology.Geometries.Geometry> arrDWX = new List<Topology.Geometries.Geometry>();//地物线
            // ArrayList arrFW = new ArrayList();//房屋
            Tools.DataBasic.Create_DCB_table(connectionString, "DCB", Tools.DataBasicString.Create_DCB);//创建调查表
            Tools.DataBasic.Create_DCB_table(connectionString, "D_ZDD", Tools.DataBasicString.Create_D_ZDD);//创建界址表示表
            Tools.DataBasic.Create_DCB_table(connectionString, "D_ZDX", Tools.DataBasicString.Create_D_ZDX);//创建界址表示表
            if (da_ZD != null) { da_ZD.Clear(); }//清除da_ZD
            if (da_ZDD != null) { da_ZDD.Clear(); }//清除da_ZD
            if (da_ZDX != null) { da_ZDX.Clear(); }//清除da_ZD
            //string selstr = "SELECT * " + "FROM DCB ";
            //string selstrD = "SELECT * " + "FROM D_ZDD ";
            //string selstrX = "SELECT * " + "FROM D_ZDX ";
            //DP = Tools.DataBasic.DCB_Initadapter(selstr, connectionString);
            //DPD = Tools.DataBasic.D_ZDD_Initadapter(selstrD, connectionString);
            // DPX = Tools.DataBasic.D_ZDX_Initadapter(selstrX, connectionString);
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            //DP.Fill(da_ZD, "DCB");
            //DPD.Fill(da_ZDD, "D_ZDD");
            // DPX.Fill(da_ZDX, "D_ZDX");

            //System.Data.DataTable DCB_B = da_ZD.Tables["DCB"];
            //System.Data.DataTable D_ZDD_B = da_ZDD.Tables["D_ZDD"];
            //System.Data.DataTable D_ZDX_B = da_ZDX.Tables["D_ZDX"];
            //System.Data.DataTable D_SZZD_B = da_ZDX.Tables["DataDic"];
            #endregion
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                //Tools.CADTools.AddNewLayer("错误标记", "Continuous", 1);
                #region 创建选择集获取界址线数据
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[2];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res;
                if (zdrk.radioButton1.Checked == true)
                {
                    res = ed.SelectAll(filter);
                }
                else
                {
                    res = ed.GetSelection(filter);
                }
                SelectionSet SS = res.Value;
                if (SS == null)
                {
                    con.Close();
                    con.Dispose();
                    MessageBox.Show("没有界址线。", "入库失败", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                ObjectId[] objectId = SS.GetObjectIds();//界址线

                TypedValue[] filList_jzd = new TypedValue[2];
                filList_jzd[0] = new TypedValue((int)DxfCode.Start, "CIRCLE");
                filList_jzd[1] = new TypedValue(8, "JZP");
                SelectionFilter filter_jzd = new SelectionFilter(filList_jzd);
                PromptSelectionResult res_jzd = ed.SelectAll(filter_jzd);
                SelectionSet SS_jzd = res_jzd.Value;
                //ObjectId[] objectId_jzd = new ObjectId[0];
                if (SS_jzd == null)
                {
                    con.Close();
                    con.Dispose();
                    MessageBox.Show("没有界址点。", "入库失败", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                    //objectId_jzd = SS_jzd.GetObjectIds();
                }
                ObjectId[] objectId_jzd = SS_jzd.GetObjectIds();

                TypedValue[] filList_dw = new TypedValue[1];
                filList_dw[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                SelectionFilter filter_dw = new SelectionFilter(filList_dw);
                PromptSelectionResult res_dw = ed.SelectAll(filter_dw);
                SelectionSet SS_dw = res_dw.Value;
                ObjectId[] objectId_dw = new ObjectId[0];
                if (SS_dw != null)
                {
                    objectId_dw = SS_dw.GetObjectIds();//地物
                }

                #endregion
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead, false);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite, true);
                    #region 读入地物线数据
                    for (int i = 0; i < objectId_dw.Length; i++)
                    {
                        ACDBPolyline DWX = (ACDBPolyline)trans.GetObject(objectId_dw[i], ACDBOpenMode.ForWrite);
                        Topology.Geometries.LineString DW_pline = (Topology.Geometries.LineString)Tools.CADTools.ConvertToLineString(DWX);
                        try
                        {
                            ResultBuffer resultBuffer = DWX.XData;
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                            DW_pline.UserData = tv3[1].Value.ToString().Trim();
                            arrDWX.Add(DW_pline);
                            //if (tv3[1].Value.ToString().IndexOf("141") >= 0 || tv3[1].Value.ToString().IndexOf("158800") >= 0
                            //    || tv3[1].Value.ToString().IndexOf("153600") >= 0)
                            if (tv3[1].Value.ToString() == "FWSX")
                            {
                                if (DWX.Closed == true)
                                {
                                    #region 使房屋线按顺时针方向
                                    ArrayList zb = new ArrayList();
                                    double JD = 0;
                                    int DDS = DWX.NumberOfVertices;
                                    for (int j = 0; j < DDS; j++)
                                    {
                                        Point2d D0 = new Point2d();
                                        Point2d D1 = new Point2d();
                                        Point2d D2 = new Point2d();
                                        zb.Add(DWX.GetPoint2dAt(j));
                                        if (j == 0)
                                        {
                                            D0 = DWX.GetPoint2dAt(DDS - 1);
                                            D1 = DWX.GetPoint2dAt(0);
                                            D2 = DWX.GetPoint2dAt(1);
                                        }
                                        if ((j >= 1) && (j < DDS - 1))
                                        {
                                            D0 = DWX.GetPoint2dAt(j - 1);
                                            D1 = DWX.GetPoint2dAt(j);
                                            D2 = DWX.GetPoint2dAt(j + 1);
                                        }
                                        if (j == DDS - 1)
                                        {
                                            D0 = DWX.GetPoint2dAt(j - 1);
                                            D1 = DWX.GetPoint2dAt(j);
                                            D2 = DWX.GetPoint2dAt(0);
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
                                            DWX.SetPointAt(p, (Point2d)zb[DDS - p - 1]);
                                        }
                                    }
                                    #endregion
                                    int censu = 1;
                                    if (tv3.Length >= 3)
                                    {
                                        censu = Convert.ToInt32(tv3[2].Value);
                                    }
                                    Topology.Geometries.Polygon FW_pline = (Topology.Geometries.Polygon)Tools.CADTools.ConvertToPolygon(DWX);
                                    FW_pline.UserData = censu;
                                    //arrFW.Add(FW_pline);
                                }
                            }
                            //else
                            //{
                            //    arrDWX.Add(DW_pline);
                            //}
                        }
                        catch (System.Exception)
                        {
                            con.Close();
                            con.Dispose();
                            trans.Commit();
                            trans.Dispose();
                            MessageBox.Show("存在没有属性或属性错误的地物线。请进行属性检查", "入库失败", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                    #endregion
                    #region 读入界址点数据
                    for (int i = 0; i < objectId_jzd.Length; i++)
                    {
                        Circle circle = (Circle)trans.GetObject(objectId_jzd[i], ACDBOpenMode.ForWrite);
                        Topology.Geometries.Point jzd_point = Tools.CADTools.ConvertToPoint(circle.Center.X, circle.Center.Y);
                        try
                        {
                            ResultBuffer resultBuffer = circle.XData;
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                            if (tv3.Length != 5)
                            {
                                MessageBox.Show("请用“界址点属性检查”工具检查界址点属性，删除由CASS生成的界址点。", "入库失败", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                            jzd_point.UserData = (int)tv3[3].Value;
                            arrJZD.Add(jzd_point);
                        }
                        catch (System.Exception)
                        {
                            con.Close();
                            con.Dispose();
                            trans.Commit();
                            trans.Dispose();
                            MessageBox.Show("存在没有属性或属性错误的界址点。请检查！", "入库失败", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                    #endregion
                    #region 显示进度条
                    ProgressMeter progressmeter = new ProgressMeter();
                    progressmeter.Start("正在...");
                    progressmeter.SetLimit(objectId.Length);
                    #endregion
                    #region 将宗地信息写入调查表和界址表示表
                    for (int i = 0; i < objectId.Length; i++)
                    {
                        progressmeter.MeterProgress();
                        ACDBPolyline JZX = (ACDBPolyline)trans.GetObject(objectId[i], ACDBOpenMode.ForWrite);
                        Extents3d JZXBounds = (Extents3d)JZX.Bounds;
                        int DDS = JZX.NumberOfVertices;
                        try
                        {
                            #region 获取界址线信息
                            ResultBuffer resultBuffer = JZX.XData;
                            TypedValue[] tv2 = resultBuffer.AsArray();
                            TypedValue[] tv3 = Tools.CADTools.GetApp(tv2, "SOUTH");
                            if (tv3[1].Value.ToString().Trim() != "300000")
                            {
                                continue;//不是界址线
                            }
                            string DM = tv3[2].Value.ToString().Trim();
                            string ZDH = DM.Substring(6);
                            ZDH = ZDH.PadLeft(5, '0');
                            string DJH = XZQH + DM.Substring(0, 6) + ZDH;
                            #endregion
                            #region 写调查表
                            if (JDH + JFH != DM.Substring(0, 6))//不是本地籍子区的宗地
                            {
                                continue;
                            }
                            #region 读取建筑面积和建筑占地面积
                            double jzmj = 0;//建筑面积
                            double jzzdmj = 0;//建筑占地面积
                            MapApplication mapApp = HostMapApplicationServices.Application;
                            Tables tables = mapApp.ActiveProject.ODTables;
                            Autodesk.Gis.Map.ObjectData.Table table;
                            table = tables["建筑面积表"];
                            Records records = table.GetObjectTableRecords(0, JZX, Autodesk.Gis.Map.Constants.OpenMode.OpenForRead, true);
                            if (records.Count > 0)
                            {
                                foreach (Autodesk.Gis.Map.ObjectData.Record record in records)
                                {
                                    Autodesk.Gis.Map.Utilities.MapValue val;
                                    val = record[0];
                                    jzmj = val.DoubleValue;
                                    val = record[1];
                                    jzzdmj = val.DoubleValue;
                                }
                            }
                            records.Dispose();
                            #endregion
                            #region 在数据库中查找宗地
                            string selstr = "SELECT * " + "FROM DCB WHERE DJH= '" + DJH + "'";
                            DP = Tools.DataBasic.DCB_Initadapter(selstr, connectionString);
                            DP.Fill(da_ZD, "DCB");
                            System.Data.DataTable DCB_B = da_ZD.Tables["DCB"];
                            #endregion
                            if (DCB_B.Rows.Count > 0)// 如果没有此宗地
                            {
                                #region 修改字段
                                DataRow Row_ZD = DCB_B.Rows[0];
                                Row_ZD["TFH"] = jstfh(JZX);//图幅号
                                Row_ZD["JZZDMJ"] = Math.Round(jzzdmj, 2);//建筑占地面积
                                Row_ZD["JZMJ"] = Math.Round(jzmj, 2);//建筑面积
                                //Row_ZD["ZDMJ"] = JZX.Area;//宗地面积
                                Row_ZD["ZDMJ"] = Math.Round(JZX.Area, 2);//宗地面积
                                Row_ZD["Xmax"] = JZXBounds.MaxPoint.X;// 外接矩形坐标
                                Row_ZD["Xmin"] = JZXBounds.MinPoint.X;
                                Row_ZD["Ymax"] = JZXBounds.MaxPoint.Y;
                                Row_ZD["Ymin"] = JZXBounds.MinPoint.Y;
                                string Sfzmj = Tools.ReadWriteReg.read_reg("法定面积");
                                double fzmj = 0;
                                try
                                {
                                    fzmj = Convert.ToDouble(Sfzmj);
                                }
                                catch
                                {
                                    MessageBox.Show("法定面积填写错误！入库失败！", "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    con.Close();
                                    con.Dispose();
                                    trans.Commit();
                                    trans.Dispose();
                                    progressmeter.Stop();
                                    return;
                                }
                                if (JZX.Area <= fzmj)
                                {
                                    Row_ZD["FZMJ"] = Math.Round(JZX.Area, 2);//发证面积
                                    Row_ZD["TDDYMJ"] = Math.Round(JZX.Area, 2);//批准面积
                                }
                                else
                                {
                                    Row_ZD["FZMJ"] = fzmj;//发证面积
                                    Row_ZD["TDDYMJ"] = fzmj;//批准面积
                                }
                                #endregion
                                #region 入库
                                try
                                {
                                    DP.Update(da_ZD, "DCB");
                                }
                                catch (System.Exception ee)
                                {
                                    MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    con.Close();
                                    con.Dispose();
                                    trans.Commit();
                                    trans.Dispose();
                                    progressmeter.Stop();
                                    return;
                                }
                                #endregion
                            }
                            else//如果没有此宗地则写入该宗地
                            {
                                #region 写入字段
                                DataRow Row_ZD = DCB_B.NewRow();
                                Row_ZD["DJH"] = DJH;//地籍号
                                int zdh = Convert.ToInt32(ZDH);
                                Row_ZD["SQSBH"] = JDH + JFH + zdh.ToString();
                                Row_ZD["SPBH"] = JDH + JFH + zdh.ToString();
                                //Row_ZD["SBB_BH"] = JDH + JFH + zdh.ToString();
                                Row_ZD["GHKBH"] = JDH + JFH + zdh.ToString();
                                Row_ZD["YBZDDM"] = DM.Substring(0, 6) + ZDH;
                                string dlbm = tv3[4].Value.ToString().Trim().PadLeft(3, '0');
                                Row_ZD["DLBM"] = dlbm;//地类代码(实际用途)
                                Row_ZD["PZYT_DLDM"] = dlbm;//地类代码(批准用途)
                                Row_ZD["SJYT"] = find_dlmc(dlbm);//实际用途
                                Row_ZD["PZYT"] = find_dlmc(dlbm);//批准用途
                                Row_ZD["QLR"] = tv3[3].Value.ToString().Trim();//权利人
                                Row_ZD["TFH"] = jstfh(JZX);//写入图幅号
                                Row_ZD["SQRLX2"] = "总登记";
                                Row_ZD["SQRLX"] = "集体土地宅基地使用权";//权属类型
                                Row_ZD["DWXZ"] = "个人";

                                Row_ZD["ZRXM"] = "居民身份证";

                                string tdzl = treeView1.SelectedNode.FullPath;//土地坐落
                                string[] zl = tdzl.Split('\\');
                                tdzl = zl[2] + zl[3] + zl[4];
                                Row_ZD["TDZL"] = tdzl;
                                Row_ZD["TXDZ"] = tdzl;
                                Row_ZD["TEXTBZ1"] = tdzl;//所有权人


                                Row_ZD["DLJGDH"] = "土地";//不动产类型
                                Row_ZD["DYQX_Q"] = "单一版";//单一版
                                Row_ZD["DYQX_Z"] = "否";//申请分别持证

                                Row_ZD["QSXZ"] = "批准拨用";
                                Row_ZD["SYQLX"] = "宅基地使用权";
                                Row_ZD["QLSLQJ"] = "地表";
                                //Row_ZD["FRDBZJZL"] = "居民身份证";
                                Row_ZD["SFTXSBB"] = 0;
                                Row_ZD["Xmax"] = JZXBounds.MaxPoint.X;
                                Row_ZD["Xmin"] = JZXBounds.MinPoint.X;
                                Row_ZD["Ymax"] = JZXBounds.MaxPoint.Y;
                                Row_ZD["Ymin"] = JZXBounds.MinPoint.Y;

                                //Row_ZD["ZDMJ"] = JZX.Area;
                                Row_ZD["ZDMJ"] = Math.Round(JZX.Area, 2);
                                //Row_ZD["FBZ1"] = Math.Round(JZX.Area, 2);//拟建宅基地面积
                                Row_ZD["FBZ1"] = JZX.Area;//拟建宅基地面积
                                Row_ZD["TDJG"] = 0.0;

                                Row_ZD["TDDYJE"] = 0.0;
                                Row_ZD["JZZDMJ"] = Math.Round(jzzdmj, 2);
                                Row_ZD["JZMJ"] = Math.Round(jzmj, 2);
                                Row_ZD["SHHG"] = 1;
                                string Sfzmj = Tools.ReadWriteReg.read_reg("法定面积");
                                double fzmj = 0;
                                try
                                {
                                    fzmj = Convert.ToDouble(Sfzmj);
                                }
                                catch
                                {
                                    MessageBox.Show("法定面积填写错误！入库失败！", "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    con.Close();
                                    con.Dispose();
                                    trans.Commit();
                                    trans.Dispose();
                                    progressmeter.Stop();
                                    return;
                                }
                                if (JZX.Area <= fzmj)
                                {
                                    Row_ZD["FZMJ"] = Math.Round(JZX.Area, 2);
                                    Row_ZD["TDDYMJ"] = Math.Round(JZX.Area, 2);
                                }
                                else
                                {
                                    Row_ZD["FZMJ"] = fzmj;
                                    Row_ZD["TDDYMJ"] = fzmj;
                                }
                                Row_ZD["CSHG"] = 1;
                                Row_ZD["SHTG"] = 1;
                                Row_ZD["PZTG"] = 1;
                                Row_ZD["JTRK"] = 0;
                                Row_ZD["RKJG"] = 0;
                                Row_ZD["YYLFJS"] = 0;
                                Row_ZD["YYLFMJ"] = 0.0;
                                Row_ZD["YYPFJS"] = 0;
                                Row_ZD["YYPFMJ"] = 0;
                                Row_ZD["LJLFJS"] = 0;
                                Row_ZD["LJLFMJ"] = 0;
                                Row_ZD["LJPFJS"] = 0;
                                Row_ZD["LJPFMJ"] = 0;
                                Row_ZD["STMJ"] = 0;
                                Row_ZD["HDMJ"] = 0;
                                Row_ZD["CDMJ"] = 0;
                                Row_ZD["YZJDMJ"] = 0;
                                Row_ZD["KXDMJ"] = 0;
                                Row_ZD["HSMJ"] = 0;
                                Row_ZD["SBBSFDY"] = 0;
                                Row_ZD["DYMJ"] = 0;
                                Row_ZD["FTMJ"] = 0;
                                Row_ZD["TDZSFDY"] = 0;
                                Row_ZD["GHKSFDY"] = 0;
                                Row_ZD["DJKSFDY"] = 0;
                                Row_ZD["FBZ2"] = 0;
                                Row_ZD["FBZ3"] = 0;
                                Row_ZD["IBZ1"] = 0;
                                Row_ZD["IBZ2"] = 0;
                                Row_ZD["IBZ3"] = 0;
                                DCB_B.Rows.Add(Row_ZD);
                                #endregion
                                #region 入库
                                try
                                {
                                    DP.Update(da_ZD, "DCB");
                                }
                                catch (System.Exception ee)
                                {
                                    MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    con.Close();
                                    con.Dispose();
                                    trans.Commit();
                                    trans.Dispose();
                                    progressmeter.Stop();
                                    return;
                                }
                                #endregion
                            }
                            #endregion
                            #region 写界址表示表
                            #region 在数据库中查找界址点
                            selstr = "SELECT * " + "FROM  D_ZDD WHERE DJH= '" + DJH + "'";
                            DPD = Tools.DataBasic.D_ZDD_Initadapter(selstr, connectionString);
                            DPD.Fill(da_ZDD, "D_ZDD");
                            System.Data.DataTable D_ZDD_B = da_ZDD.Tables["D_ZDD"];

                            for (int i9 = 0; i9 < da_ZDD.Tables["D_ZDD"].Rows.Count; i9++)
                            {
                                da_ZDD.Tables["D_ZDD"].Rows[i9].Delete();
                            }


                            //if (D_ZDD_B.Rows.Count > 0)//如果存在该宗地界址点则删除
                            //{
                            //    D_ZDD_B.Rows.Clear();
                            //   // DPD.Update(da_ZDD, "D_ZDD");
                            //}
                            #endregion
                            int dygdJzdh = 0;//第一个点界址点号
                            for (int j = 0; j < DDS; j++)
                            {
                                #region 查找每一个界址线线段的其他地物线
                                int nn = j + 1;
                                if (nn == DDS)
                                {
                                    nn = 0;
                                }
                                Topology.Geometries.LineString JZX_XD = (Topology.Geometries.LineString)Tools.CADTools.ConvertToLineString(new Line(JZX.GetPoint3dAt(j), JZX.GetPoint3dAt(nn)));
                                Topology.Geometries.Geometry JZX_XD_buff = (Topology.Geometries.Geometry)JZX_XD.Buffer(0.08, Topology.Operation.Buffer.BufferStyle.CapButt);//本段界址线生成的缓冲区
                                ArrayList arrdwxlb = new ArrayList();//在界址线缓冲区内的地物线集合，含本宗地界址线
                                for (int q = 0; q < arrDWX.Count; q++)
                                {
                                    Topology.Geometries.Geometry yxx = arrDWX[q];
                                    if (JZX_XD_buff.Intersects(yxx))
                                    {
                                        Topology.Geometries.Geometry xx = (Topology.Geometries.Geometry)JZX_XD_buff.Intersection((Topology.Geometries.LineString)arrDWX[q]);
                                        xx.UserData = yxx.UserData;
                                        arrdwxlb.Add(xx);
                                    }
                                }
                                double wqcd = 0;//围墙长度
                                double qbcd = 0;//墙壁长度
                                double zlcd = 0;//栅栏长度
                                double dkcd = 0;//陡坎长度
                                bool IsczJZX = false;//是否存在其他宗界址线
                                string jzxlx = "";//界线类型
                                string jzxwz = "外";//界线位置
                                double zdcd = 0;//最大长度
                                double jzxcd = 0;
                                Topology.Geometries.Geometry wqggx = null;//围墙骨干线
                                Topology.Geometries.Geometry wqfsx = null;//围墙附属线
                                Topology.Geometries.Geometry zlx = null;//栅栏线
                                double wqggx_cd = 0;
                                double wqfsx_cd = 0;
                                double zlx_cd = 0;

                                bool Lqb = false;//内边墙壁线
                                bool Wqb = false;//外边墙壁线
                                for (int r = 0; r < arrdwxlb.Count; r++)
                                {
                                    Topology.Geometries.Geometry LL = (Topology.Geometries.Geometry)arrdwxlb[r];
                                    string lx = (string)LL.UserData;
                                    if (lx.IndexOf("1443") >= 0) { wqcd = wqcd + LL.Length; }//围墙线集合
                                    if (lx.IndexOf("1444") >= 0) { zlcd = zlcd + LL.Length; }//栅栏线集合
                                    if (lx.IndexOf("2042") >= 0) { dkcd = dkcd + LL.Length; }//陡坎线集合
                                    if (lx.IndexOf("144301-1") >= 0) //找出最长的围墙附属线
                                    {
                                        if (LL.Length > wqfsx_cd)
                                        {
                                            wqfsx_cd = LL.Length;
                                            wqfsx = LL;
                                        }
                                    }
                                    if (lx.IndexOf("144301") >= 0 || lx.IndexOf("144302") >= 0)//找出最长的围墙骨干线
                                    {
                                        if (LL.Length > wqggx_cd)
                                        {
                                            wqggx_cd = LL.Length;
                                            wqggx = LL;
                                        }
                                    }
                                    if (lx.IndexOf("144400") >= 0)//找出最长的栅栏线
                                    {
                                        if (LL.Length > zlx_cd)
                                        {
                                            zlx_cd = LL.Length;
                                            zlx = LL;
                                        }
                                    }
                                    if (lx == "300000") { jzxcd = jzxcd + LL.Length; }//所有界址线长度总和
                                    // if (lx.IndexOf("141") >= 0 || lx.IndexOf("158800") >= 0 || lx.IndexOf("153600") >= 0)//
                                    if (lx.IndexOf("FWSX") >= 0)
                                    {
                                        qbcd = qbcd + LL.Length;//找出属于墙体的所有线段长度总和
                                        if (LL.Length > JZX_XD.Length / 2.0)
                                        {
                                            if (LL.GeometryType.ToLower() == "linestring")
                                            {
                                                double jzdjd = Tools.CADTools.GetAngle(JZX.GetPoint3dAt(j), JZX.GetPoint3dAt(nn));
                                                Topology.Geometries.LineString lzstr = (Topology.Geometries.LineString)LL;
                                                Topology.Geometries.ICoordinate[] coor = lzstr.Coordinates;
                                                Point2d d1 = new Point2d(coor[0].X, coor[0].Y);
                                                Point2d d2 = new Point2d(coor[coor.Length - 1].X, coor[coor.Length - 1].Y);
                                                double jd = Tools.CADTools.GetAngle(d1, d2);
                                                double jj = jd - jzdjd;
                                                if (jj < 0) { jj = jj + Math.PI * 2.0; }
                                                jj = jj * 180.0 / Math.PI;
                                                if (jj >= 340 || jj < 20)
                                                {
                                                    Lqb = true;
                                                }
                                                else
                                                {
                                                    Wqb = true;
                                                }
                                                //if (jj >= 160 && jj < 200)
                                                //{
                                                //    Wqb = true;
                                                //}
                                            }
                                            if (LL.GeometryType.ToLower() == "multilinestring")
                                            {
                                                for (int vv = 0; vv < LL.NumGeometries; vv++)
                                                {
                                                    Topology.Geometries.LineString mll = (Topology.Geometries.LineString)LL.GetGeometryN(vv);
                                                    double jzdjd = Tools.CADTools.GetAngle(JZX.GetPoint3dAt(j), JZX.GetPoint3dAt(nn));
                                                    Topology.Geometries.LineString lzstr = (Topology.Geometries.LineString)mll;
                                                    Topology.Geometries.ICoordinate[] coor = lzstr.Coordinates;
                                                    Point2d d1 = new Point2d(coor[0].X, coor[0].Y);
                                                    Point2d d2 = new Point2d(coor[coor.Length - 1].X, coor[coor.Length - 1].Y);
                                                    double jd = Tools.CADTools.GetAngle(d1, d2);
                                                    double jj = jd - jzdjd;
                                                    if (jj < 0) { jj = jj + Math.PI * 2.0; }
                                                    jj = jj * 180.0 / Math.PI;
                                                    if (jj >= 340 || jj < 20)
                                                    {
                                                        Lqb = true;
                                                    }
                                                    if (jj >= 160 && jj < 200)
                                                    {
                                                        Wqb = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region 分析界线类型

                                if (wqcd > zdcd) { zdcd = wqcd; jzxlx = "围墙"; }
                                if (qbcd > zdcd) { zdcd = qbcd; jzxlx = "墙壁"; }
                                if (zlcd > zdcd) { zdcd = zlcd; jzxlx = "栅栏"; }
                                if (dkcd > zdcd) { zdcd = dkcd; jzxlx = "陡坎"; }//找出最长的线是那一个

                                if (qbcd > JZX_XD.Length / 2.0) { jzxlx = "墙壁"; }
                                if (dkcd > JZX_XD.Length / 2.0) { jzxlx = "陡坎"; }
                                if (zlcd > JZX_XD.Length / 2.0) { jzxlx = "栅栏"; }
                                if (wqcd > JZX_XD.Length / 2.0) { jzxlx = "围墙"; }
                                if (wqcd < JZX_XD.Length * 0.4 && qbcd < JZX_XD.Length * 0.4 &&
                                  zlcd < JZX_XD.Length * 0.4 && dkcd < JZX_XD.Length * 0.4)
                                {
                                    jzxlx = "两点连线"; //各种类型长度都小于界址线段长度的一半jzxlx为"空地"
                                }

                                #endregion
                                #region 分析界线位置
                                if (jzxcd - JZX_XD.Length > JZX_XD.Length * 0.5)
                                {
                                    IsczJZX = true;//存在邻宗地界址线
                                }

                                if (IsczJZX == false) { jzxwz = "外"; }

                                #region 分析墙壁
                                if (Lqb && Wqb && IsczJZX && jzxlx == "墙壁")
                                {
                                    jzxwz = "中";
                                }
                                if (Lqb && !Wqb && IsczJZX && jzxlx == "墙壁")
                                {
                                    jzxwz = "外";
                                }
                                if (!Lqb && Wqb && IsczJZX && jzxlx == "墙壁")
                                {
                                    jzxwz = "内";
                                }
                                #endregion
                                if (jzxlx == "两点连线") { jzxwz = "中"; }
                                if (IsczJZX && jzxlx == "陡坎") { jzxwz = "外"; }
                                #region 分析围墙
                                if (IsczJZX && jzxlx == "围墙")
                                {
                                    // string wq = "";//种类
                                    Topology.Geometries.Geometry lz = null;//找出“围墙骨干线”、“栅栏”、“围墙附属线”中最长的一种
                                    double cd = -1;//最长的一种的长度
                                    //if (wqggx_cd > cd) { string wq = "骨干"; lz = wqggx; cd = wqggx_cd; }
                                    //if (wqfsx_cd > cd) { string wq = "附属"; lz = wqfsx; cd = wqfsx_cd; }
                                    //if (zlx_cd > cd) { string wq = "骨干"; lz = zlx; cd = zlx_cd; }
                                    if (wqggx_cd > cd) { lz = wqggx; cd = wqggx_cd; }
                                    if (wqfsx_cd > cd) { lz = wqfsx; cd = wqfsx_cd; }
                                    if (zlx_cd > cd) { lz = zlx; cd = zlx_cd; }
                                    double jzdjd = Tools.CADTools.GetAngle(JZX.GetPoint3dAt(j), JZX.GetPoint3dAt(nn));
                                    if (cd > JZX_XD.Length / 2.0 && lz.GeometryType.ToLower() == "linestring")
                                    {
                                        Topology.Geometries.LineString lzstr = (Topology.Geometries.LineString)lz;
                                        Topology.Geometries.ICoordinate[] coor = lzstr.Coordinates;
                                        Point2d d1 = new Point2d(coor[0].X, coor[0].Y);
                                        Point2d d2 = new Point2d(coor[coor.Length - 1].X, coor[coor.Length - 1].Y);
                                        double jd = Tools.CADTools.GetAngle(d1, d2);
                                        double jj = jd - jzdjd;
                                        if (jj < 0) { jj = jj + Math.PI * 2.0; }
                                        jj = jj * 180.0 / Math.PI;
                                        if (jj >= 340 || jj < 20)
                                        {
                                            jzxwz = "内";
                                        }
                                        else
                                        {
                                            jzxwz = "外";
                                        }
                                    }
                                }
                                #endregion
                                #endregion
                                #region 查找界址点
                                int jzdh = 0;
                                bool Isjzd = false;
                                for (int k = 0; k < arrJZD.Count; k++)
                                {
                                    Topology.Geometries.Point jzd_jzx = Tools.CADTools.ConvertToPoint(JZX.GetPoint3dAt(j).X, JZX.GetPoint3dAt(j).Y);
                                    Topology.Geometries.Point jzd_jzd = (Topology.Geometries.Point)arrJZD[k];
                                    if (jzd_jzx.Distance(jzd_jzd) < 0.0001)
                                    {
                                        jzdh = (int)jzd_jzd.UserData;
                                        Isjzd = true;
                                        break;
                                    }
                                }
                                if (Isjzd == false)
                                {
                                    MessageBox.Show("界址线上没有界址点", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    con.Close();
                                    con.Dispose();
                                    trans.Commit();
                                    trans.Dispose();
                                    progressmeter.Stop();
                                    return;
                                }
                                #endregion
                                #region 写入字段
                                DataRow Row_ZDD = D_ZDD_B.NewRow();
                                Row_ZDD["DJH"] = DJH;//地籍号
                                Row_ZDD["MBBSM"] = j + 1;//序号
                                Row_ZDD["JZXLB"] = jzxlx;//界址线类别
                                Row_ZDD["JZXWZ"] = jzxwz;//界址线位置
                                Row_ZDD["X"] = JZX.GetPoint3dAt(j).X;
                                Row_ZDD["Y"] = JZX.GetPoint3dAt(j).Y;
                                Row_ZDD["JZJJ"] = JZX_XD.Length;
                                Row_ZDD["KZBC"] = Math.Round(JZX_XD.Length, 2);
                                Row_ZDD["JBZL"] = Tools.ReadWriteReg.read_reg("界标种类");
                                Row_ZDD["JZDH"] = jzdh;
                                D_ZDD_B.Rows.Add(Row_ZDD);
                                if (j == 0) { dygdJzdh = jzdh; }
                                #endregion
                            }
                            #region 写入字段
                            DataRow Row_ZDD2 = D_ZDD_B.NewRow();
                            Row_ZDD2["DJH"] = DJH;//地籍号
                            Row_ZDD2["MBBSM"] = DDS + 1;//序号
                            Row_ZDD2["JZDH"] = dygdJzdh;//序号
                            Row_ZDD2["X"] = JZX.GetPoint3dAt(0).X;
                            Row_ZDD2["Y"] = JZX.GetPoint3dAt(0).Y;
                            D_ZDD_B.Rows.Add(Row_ZDD2);
                            #endregion
                            #region  入库
                            try
                            {
                                DPD.Update(da_ZDD, "D_ZDD");
                            }
                            catch (System.Exception ee)
                            {
                                MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                con.Close();
                                con.Dispose();
                                trans.Commit();
                                trans.Dispose();
                                return;
                            }
                            #endregion
                            #endregion
                        }
                        catch (System.Exception)
                        {
                            MessageBox.Show("您的图形有错误，请重新检查", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            con.Close();
                            con.Dispose();
                            trans.Commit();
                            trans.Dispose();
                            return;
                        }
                        da_ZD.Clear();
                        da_ZDD.Clear();
                    }
                    #endregion
                    trans.Commit();
                    trans.Dispose();
                    progressmeter.Stop();
                }
            }
            con.Close();
            con.Dispose();
            da_ZD.Clear();
            da_ZDD.Clear();
            da_ZDX.Clear();

            MessageBox.Show("宗地数据入库完成！", "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    /// <summary>
    /// 分层分户列表
    /// </summary>
    public class FCFHLB
    {
        private List<FWSXJG> n_fwsx = new List<FWSXJG>();
        private string n_tfm = "";
        private int n_gs = 0;
        private List<Topology.Geometries.Polygon> n_fw_Polygon = new List<Topology.Geometries.Polygon>();

        /// <summary>
        /// 构建分层分幅图表
        /// </summary>
        /// <param name="tfm">图名</param>
        /// <param name="gs">图块个数</param>
        /// <param name="fwsx">属性</param>
        /// <param name="fw_Polygon">几何图形</param>
        public FCFHLB(string tfm, int gs, List<FWSXJG> fwsx, List<Topology.Geometries.Polygon> fw_Polygon)
        {
            this.n_fwsx = fwsx;
            this.n_tfm = tfm;
            this.n_gs = gs;
            this.n_fw_Polygon = fw_Polygon;

        }

        /// <summary>
        /// 个数
        /// </summary>
        public int gs
        {
            set
            {
                n_gs = value;
            }
            get
            {
                return n_gs;
            }
        }

        /// <summary>
        /// 图幅名称
        /// </summary>
        public string tfm
        {
            set
            {
                n_tfm = value;
            }
            get
            {
                return n_tfm;
            }
        }

        /// <summary>
        /// 属性
        /// </summary>
        public List<FWSXJG> fwsx
        {
            set
            {
                n_fwsx = value;
            }
            get
            {
                return n_fwsx;
            }
        }

        /// <summary>
        /// 几何图形
        /// </summary>
        public List<Topology.Geometries.Polygon> fw_Polygon
        {
            set
            {
                n_fw_Polygon = value;
            }
            get
            {
                return n_fw_Polygon;
            }
        }

    }
}
