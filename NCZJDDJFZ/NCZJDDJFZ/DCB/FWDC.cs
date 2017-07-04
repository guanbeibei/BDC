using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using ReadOrWriteXML;
using System;
using System.Windows.Forms;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using ACDBPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace NCZJDDJFZ.DCB
{
    public partial class FWDC : UserControl
    {
        public FWDC()
        {
            InitializeComponent();
        }
        public void Set_cm_xx(string cm, string xx, short ys)
        {
            NCZJDDJFZ.Tools.CADTools.AddNewLinetype(xx, Tools.CADTools.GetAppPath() + "ACADISO.LIN");
            if (cm == "权属界") { ys = 1; }
            NCZJDDJFZ.Tools.CADTools.AddNewLayer(cm, xx, ys);
        }
        private void GJRJZDFX_CheckedChanged(object sender, EventArgs e)
        {
            if (GJRJZDFX.Checked)
            {
                DJH.Enabled = false;
            }
            else
            {
                DJH.Enabled = true;
            }
        }

        private void FWYT_SelectedIndexChanged(object sender, EventArgs e)
        {
            GHYT.Text = FWYT.Text.Trim();
        }

        private void ZCS_ValueChanged(object sender, EventArgs e)
        {
            if (ZCS.Value.ToString() == "1")
            {
                SZCS.Value = 1;
            }
            else
            {
                SZCS.Value = 0;
            }

        }

        private void GYQK_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GYQK.Text.Trim() == "单独所有")
            {
                ZYMJ.Enabled = false;
                FTMJ.Enabled = false;
            }
                
            else
            {
                ZYMJ.Enabled = true;
                FTMJ.Enabled = true;
            }
        }

        private void FWXZ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FWXZ.Text.Trim() == "自建房")
            {
                FCLY.Text = "自建房";
            }
        }

        private void FWDC_Load(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            DCRQ.Text = readOrWritexml.ReadXml("//房屋调查日期");
            DCY.Text = readOrWritexml.ReadXml("//房屋调查员");
            DCYJ.Text = readOrWritexml.ReadXml("//房屋调查意见");
            //DCY.Text = readOrWritexml.ReadXml("//房屋竣工日期");
            init();

        }
        
        private void DCRQ_Leave(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("房屋调查日期", DCRQ.Text.Trim());
        }

        private void DCY_Leave(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("房屋调查员", DCY.Text.Trim());
        }
        
        private void DCYJ_Leave(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("房屋调查意见", DCYJ.Text.Trim());
        }
        private void JGSJ_Leave(object sender, EventArgs e)
        {
            //ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            //readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            //readOrWritexml.CreateXmlDocument("房屋竣工日期", DCY.Text.Trim());
        }
        /// <summary>打开、解冻、解琐指定图层
        /// 打开、解冻、解琐指定图层
        /// </summary>
        /// <param name="cm"></param>
        public void On_layer(string cm)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    LayerTable Layertable = (LayerTable)trans.GetObject(db.LayerTableId, ACDBOpenMode.ForWrite);
                    foreach (ObjectId layerid in Layertable)
                    {

                        LayerTableRecord LayertableRecord = (LayerTableRecord)trans.GetObject(layerid, ACDBOpenMode.ForWrite);//层记录表
                        if (LayertableRecord.Name == cm)
                        {
                            LayertableRecord.IsOff = false;
                            LayertableRecord.IsLocked = false;
                            if (LayertableRecord.IsFrozen)
                            {
                                LayertableRecord.IsFrozen = false;
                            }
                        }
                    }
                }
                finally
                {
                    trans.Commit();
                    trans.Dispose();
                }
            }
        }
        /// <summary>关闭指定层
        /// 关闭指定层
        /// </summary>
        /// <param name="cm"></param>
        public void Off_Layer(string cm)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {

                    LayerTable Layertable = (LayerTable)trans.GetObject(db.LayerTableId, ACDBOpenMode.ForWrite);
                    foreach (ObjectId layerid in Layertable)
                    {

                        LayerTableRecord LayertableRecord = (LayerTableRecord)trans.GetObject(layerid, ACDBOpenMode.ForWrite);//层记录表
                        if (LayertableRecord.Name == cm)
                        {
                            LayertableRecord.IsOff = true;
                        }
                    }
                }
                finally
                {
                    trans.Commit();
                    trans.Dispose();
                }
            }

        }



        // “绘制”按钮
        private void ZH_Click(object sender, EventArgs e)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                if (!zhunbei())
                {
                    return;
                }
                string[] sx = FZ();
                double bg = 3 * ((double)SZCS.Value - 1);
                ObjectId plid = ObjectId.Null;
                FLPP(sx, plid, true, bg);
                init();
            }
        }
        
        private void ML_CheckedChanged(object sender, EventArgs e)
        {
            if (ML.Checked)
            {
                //FWGL.Text = "门廊";
                ZCS.Value = 1;
                SZCS.Value = 1;
            }
            else
            {
                //FWGL.SelectedIndex = -1;
                ZCS.Value = 0;
                SZCS.Value = 0;
            }
        }

        private void CP_CheckedChanged(object sender, EventArgs e)
        {
            if (CP.Checked)
            {
                //FWGL.Text = "车棚";
                ZCS.Value = 1;
                SZCS.Value = 1;
            }
            else
            {
                //FWGL.SelectedIndex = -1;
                ZCS.Value =0;
                SZCS.Value = 0;
            }
        }

        private void YL_CheckedChanged(object sender, EventArgs e)
        {
            if (YL.Checked)
            {
                MJZKSX.Value = (decimal)0.5;
            }
            else
            {
                MJZKSX.Value = (decimal)0;
            }
        }

        private void YT_CheckedChanged(object sender, EventArgs e)
        {
            if (YT.Checked)
            {
                MJZKSX.Value = (decimal)0.5;
            }
            else
            {
                MJZKSX.Value = (decimal)0;
            }
        }

        private void LT_CheckedChanged(object sender, EventArgs e)
        {
            if (LT.Checked)
            {
                MJZKSX.Value = (decimal)0.5;
            }
            else
            {
                MJZKSX.Value = (decimal)0;
            }
        }
        /// <summary>初始化界面值
        /// 初始化界面值
        /// </summary>
        private void init()
        {
            #region 赋初始值
            GJRJZDFX.Checked = true;
            FWXZ.Text = "自建房";
            FWCB.Text = "私有房产";
            FWYT.Text = "非成套住宅";
            GHYT.Text = "非成套住宅";
            QLRLX.Text = "个人";
            FCLY.Text = "自建";

            GYQK.Text = "单独所有";

            if (!TDF.Checked)
            {
                FWDH.Value = 0;
                ZCS.Value = 0;
                FWJG.SelectedIndex = -1;
                //FWGL.SelectedIndex = -1;
            }

            
            HH.Text = "";
            SZCS.Value = 0;
            //JGSJ.Text = "";
            ZYMJ.Text = "";
            FTMJ.Text = "";

            MJZKSX.Value = 1;

            BEI.Text = "自有墙";
            DONG.Text = "自有墙";
            NAN.Text = "自有墙";
            XI.Text = "自有墙";

            FW.Checked = true ;
            ML.Checked = false;
            CP.Checked = false;
            YL.Checked = false;
            YT.Checked = false;
            LT.Checked = false;

            #endregion
        }
        /// <summary>准备、检查
        /// 准备、检查
        /// </summary>
        /// <returns></returns>
        public bool zhunbei()
        {
            #region 准备、检查
            if (ML.Checked || CP.Checked)
            {
                ZCS.Value = 1;
                SZCS.Value = 1;
            }

            if (!FW.Checked && !YL.Checked && !YT.Checked && !LT.Checked && !CP.Checked && !ML.Checked)
            {
                MessageBox.Show("请选择建筑物类型！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (!GJRJZDFX.Checked)
            {
                if (DJH.Text.Trim() == "")
                {
                    MessageBox.Show("地籍号不准为空！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
                if (DJH.Text.Trim().Length != 14)
                {
                    MessageBox.Show("地籍号位数不对！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

            }
            if (FWDH.Value == 0)
            {
                MessageBox.Show("幢号必须大于零！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (ZCS.Value == 0)
            {
                MessageBox.Show("总层数必须大于零！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (SZCS.Value == 0)
            {
                MessageBox.Show("所在层数不能等于零！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (SZCS.Value > ZCS.Value)
            {
                MessageBox.Show("所在层数不能大于总层数！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if ( FWJG.Text == "")
            {
                MessageBox.Show("必须选择房屋结构！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            //if (FW.Checked && FWGL.Text == "")
            //{
            //    MessageBox.Show("必须选择房屋功能！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //    return false;
            //}
            if (DCRQ.Text == "")
            {
                MessageBox.Show("必须填写调查日期！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (DCY.Text == "")
            {
                MessageBox.Show("必须填写调查员！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            if (MJZKSX.Value.ToString("0.00") == "0.00")
            {
                MessageBox.Show("面积折扣系数不可为零！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            return true;
            #endregion
        }
        /// <summary>
        /// 将界面值赋给数组
        /// </summary>
        /// <returns></returns>
        public string[] FZ()
        {
            string[] sx = new string[30];
            sx[1] = DJH.Text.Trim();
            sx[2] = FWXZ.Text.Trim();
            sx[3] = FWCB.Text.Trim();
            sx[4] = FWYT.Text.Trim();
            sx[5] = GHYT.Text.Trim();
            sx[6] = QLRLX.Text.Trim();
            sx[7] = FCLY.Text.Trim();
            sx[8] = GYQK.Text.Trim();
            sx[9] = FWDH.Value.ToString("0");
            sx[10] = HH.Text.Trim();
            sx[11] = ZCS.Value.ToString("0");
            sx[12] = SZCS.Value.ToString("0");
            sx[13] = FWJG.Text.Trim();
            sx[14] = JGSJ.Text.Trim();
            sx[15] = ZYMJ.Text.Trim();
            sx[16] = FTMJ.Text.Trim();
            sx[17] = FWGL.Text.Trim();
            sx[18] = FJSM.Text.Trim();
            sx[19] = DCRQ.Text.Trim();
            sx[20] = DCYJ.Text.Trim();
            sx[21] = DCY.Text.Trim();
            sx[22] = MJZKSX.Value.ToString("0.00");
            sx[23] = BEI.Text.Trim();
            sx[24] = DONG.Text.Trim();
            sx[25] = NAN.Text.Trim();
            sx[26] = XI.Text.Trim();
            if (GJRJZDFX.Checked)
            {
                sx[27] = "是";
            }
            else
            {
                sx[27] = "否";
            }
            sx[28] = "";
            return sx;
        }
         /// <summary>分类匹配或绘制
        /// 分类匹配或绘制
         /// </summary>
         /// <param name="sx"></param>
         /// <param name="plid"></param>
         /// <param name="ppORhz">true表示绘制，false 表示匹配</param>
        public void FLPP(string[] sx, ObjectId plid,bool ppORhz,double bg)
        {
            Tools.CADTools cadtools = new Tools.CADTools();
            if (FW.Checked)
            {
                sx[0] = "房屋";
                sx[29] = "FWSX";
                string cm = "房屋" + "-" + SZCS.Value.ToString("0");
                Set_cm_xx(cm, "Continuous", 6);
                On_layer(cm);
                if (ppORhz)
                {
                      plid = NCZJDDJFZ.Tools.CADTools.CreatPolyline(cm, "Continuous", 0, 6, 0, bg, 1);
                }
                cadtools.FSX(plid, sx, cm, "Continuous", 0, 6, bg, 1);
                if (ZDGB.Checked)
                {
                    Off_Layer(cm);
                }
               
            }
            if (ML.Checked)
            {
                sx[0] = "门廊";
                sx[29] = "FWSX";
                string cm = "门廊";
                Set_cm_xx(cm, "X32", 5);
                On_layer(cm);
                if (ppORhz)
                {
                      plid = NCZJDDJFZ.Tools.CADTools.CreatPolyline(cm, "X32", 0, 5, 0, bg, 1);
                }
                cadtools.FSX(plid, sx, cm, "X32", 0, 5, bg, 1);
                if (ZDGB.Checked)
                {
                    Off_Layer(cm);
                }
            }
            if (CP.Checked)
            {
                sx[0] = "车棚";
                sx[29] = "FWSX";
                string cm = "车棚";
                Set_cm_xx(cm, "X32", 7);
                On_layer(cm);
                if (ppORhz)
                {
                      plid = NCZJDDJFZ.Tools.CADTools.CreatPolyline(cm, "X32", 0, 7, 0, bg, 1);
                }
                cadtools.FSX(plid, sx, cm, "X32", 0, 7, bg, 1);
                if (ZDGB.Checked)
                {
                    Off_Layer(cm);
                }
            }
            if (YL.Checked)
            {
                sx[0] = "檐廊";
                sx[29] = "FWSX";
                string cm = "檐廊" + "-" + SZCS.Value.ToString("0");
                Set_cm_xx(cm, "X32", 2);
                On_layer(cm);
                if (ppORhz)
                {
                     plid = NCZJDDJFZ.Tools.CADTools.CreatPolyline(cm, "X32", 0, 2, 0, bg, 1);
                }
                cadtools.FSX(plid, sx, cm, "X32", 0, 2, bg, 1);
                if (ZDGB.Checked)
                {
                    Off_Layer(cm);
                }
            }
            if (YT.Checked)
            {
                sx[0] = "阳台";
                sx[29] = "FWSX";
                string cm = "阳台" + "-" + SZCS.Value.ToString("0");
                Set_cm_xx(cm, "X32", 3);
                On_layer(cm);
                if (ppORhz)
                {
                     plid = NCZJDDJFZ.Tools.CADTools.CreatPolyline(cm, "X32", 0, 3, 0, bg, 1);
                }
                cadtools.FSX(plid, sx, cm, "X32", 0, 3, bg, 1);
                if (ZDGB.Checked)
                {
                    Off_Layer(cm);
                }
            }
            if (LT.Checked)
            {
                sx[0] = "楼梯";
                sx[29] = "FWSX";
                string cm = "楼梯" + "-" + SZCS.Value.ToString("0");
                Set_cm_xx(cm, "Continuous", 4);
                On_layer(cm);
                if (ppORhz)
                {
                     plid = NCZJDDJFZ.Tools.CADTools.CreatPolyline(cm, "Continuous", 0, 4, 0, bg, 1);
                }
                cadtools.FSX(plid, sx, cm, "Continuous", 0, 4, bg, 1);
                if (ZDGB.Checked)
                {
                    Off_Layer(cm);
                }
            }

        }

        // “匹配”按钮
        private void button2_Click(object sender, EventArgs e)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                if (!zhunbei())
                {
                    return;
                }
                string[] sx = FZ();
                #region 创建选择集
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[1];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                //filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res;
                res = ed.GetSelection(filter);
                SelectionSet SS = res.Value;
                if (SS == null)
                {
                    return;
                }
                ObjectId[] objectId = SS.GetObjectIds();
                ObjectId plid = objectId[0];
                #endregion
                double bg = 3 * ((double)SZCS.Value - 1);
                FLPP(sx, plid, false, bg);
                init();
               
            }
        }

       

         

        private void DCRQ_TextChanged(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("房屋调查日期", DCRQ.Text.Trim());
        }

        private void FW_CheckedChanged(object sender, EventArgs e)
        {
            if (FW.Checked)
            {
                MJZKSX.Value = (decimal)1;
            }
            else
            {
                MJZKSX.Value = (decimal)0;
            }
        }

        private void TDF_CheckedChanged(object sender, EventArgs e)
        {
            if (TDF.Checked)
            {
                
                if (FWDH.Value == 0)
                {
                    MessageBox.Show("幢号必须大于零！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    TDF.Checked = false;
                    return;
                }
                if (ZCS.Value == 0)
                {
                    MessageBox.Show("总层数必须大于零！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    TDF.Checked = false;
                    return;
                }
                //if (FW.Checked && FWGL.Text == "")
                //{
                //    MessageBox.Show("必须选择房屋功能！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    TDF.Checked = false;
                //    return;
                //}
                if (DCRQ.Text == "")
                {
                    MessageBox.Show("必须填写调查日期！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    TDF.Checked = false;
                    return;
                }
                if (DCY.Text == "")
                {
                    MessageBox.Show("必须填写调查员！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    TDF.Checked = false;
                    return;
                }
                FWDH.Enabled = false;
                ZCS.Enabled = false;
                FWJG.Enabled = false;
                JGSJ.Enabled = false;
                //FWGL.Enabled = false;
                DCRQ.Enabled = false;
                DCYJ.Enabled = false;
                DCY.Enabled = false;
                ML.Enabled = false;
                CP.Enabled = false;
                COPY.Enabled = true;
            }
            else
            {
                FWDH.Enabled = true;
                ZCS.Enabled = true;
                FWJG.Enabled = true;
                JGSJ.Enabled = true;
                //FWGL.Enabled = true;
                DCRQ.Enabled = true;
                DCYJ.Enabled = true;
                DCY.Enabled = true;
                ML.Enabled = true;
                CP.Enabled = true;
                COPY.Enabled = false;
            }
        }

        // “复制”按钮
        private void COPY_Click(object sender, EventArgs e)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                if (!zhunbei())
                {
                    return;
                }
                string[] sx = FZ();
                #region 创建选择集
                Database db = HostApplicationServices.WorkingDatabase;
                Editor ed = AcadApplication.DocumentManager.MdiActiveDocument.Editor;
                TypedValue[] filList = new TypedValue[1];
                filList[0] = new TypedValue((int)DxfCode.Start, "LWPOLYLINE");
                //filList[1] = new TypedValue(8, "JZD");
                SelectionFilter filter = new SelectionFilter(filList);
                PromptSelectionResult res;
                res = ed.GetSelection(filter);
                SelectionSet SS = res.Value;
                if (SS == null)
                {
                    return;
                }
                ObjectId[] objectId = SS.GetObjectIds();
                ObjectId plid = objectId[0];
                #endregion
                double bg = 3 * ((double)SZCS.Value - 1);
                ACDBPolyline PLOBJ2;
                using (Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, ACDBOpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], ACDBOpenMode.ForWrite);
                    ACDBPolyline PLOBJ = (ACDBPolyline)trans.GetObject(plid, ACDBOpenMode.ForWrite);
                    PLOBJ2 = (ACDBPolyline)PLOBJ.Clone();
                    btr.AppendEntity(PLOBJ2);
                    trans.AddNewlyCreatedDBObject(PLOBJ2, true);
                    trans.Commit();
                    trans.Dispose();
                }
                FLPP(sx, PLOBJ2.ObjectId, false, bg);
                init();
            }
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            Date date = new Date();
            System.Drawing.Point startPoint = PointToScreen(button1.Location);
            startPoint.Y += button1.Parent.Location.Y;
            startPoint.X += button1.Parent.Location.X;
            date.StartPosition = FormStartPosition.Manual;
            date.Location = GetDate(startPoint, date.Width, date.Height);
            date.ShowDialog();
            DCRQ.Text = date.sj;
        }
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
            string text = date.sj;
            text = text.Substring(0, text.IndexOf("年")+1);
            JGSJ.Text = text;
        }

        
    }
}
