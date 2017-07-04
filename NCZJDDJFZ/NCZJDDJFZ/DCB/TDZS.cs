using ReadOrWriteXML;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace NCZJDDJFZ.DCB
{
    public partial class TDZS : Form
    {
        public TDZS()
        {
            InitializeComponent();
        }

        private void TDZS_Load(object sender, EventArgs e)
        {
            string connectionString = Tools.Uitl.GetConnectionString();//写连接字符串
            string selstring = "SELECT * FROM DCB where DJH = '" + ATT.DJH + "'";
            DataSet da_ZD = new DataSet();//定义DataSet 
            SqlDataAdapter DP = new SqlDataAdapter();//初始化适配器
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DP = Tools.DataBasic.DCB_Initadapter(selstring, connectionString);
                DP.Fill(da_ZD, "DCB");//填充Dataset
                System.Data.DataTable TB_DCB = da_ZD.Tables["DCB"];
                DataRow row = TB_DCB.Rows[0];
                this.GY.Text = row["TDZ_JY"].ToString().Trim();
                this.LF.Text = row["TDZ_LF"].ToString().Trim();
                this.H.Text = ATT.DJH.Substring(6, 11);
                this.TDSYQR.Text = row["QLR"].ToString().Trim();
                this.ZL.Text = row["TDZL"].ToString().Trim();
                this.DH.Text = ATT.DJH.Substring(6, 11);
                this.TH.Text = row["TFH"].ToString().Trim();
                this.YT.Text = row["SJYT"].ToString().Trim();
                double qdjg = (double)row["FBZ2"];
                if (qdjg < 0.0001)
                {
                    this.QDJG.Text = "/";
                }
                else
                {
                    this.QDJG.Text = qdjg.ToString("0.000").Trim();
                }

                this.SYQLX.Text = row["SYQLX"].ToString().Trim();

                if (row["ZZRQ"].ToString().Trim() == "")
                {
                    this.ZZRQ.Text = "/";
                }
                else
                {
                    this.ZZRQ.Text = row["ZZRQ"].ToString().Trim();
                }
                double mj = (double)row["FZMJ"];
                this.SYQMJ.Text = mj.ToString("0.00");
                double dymj = (double)row["DYMJ"];
                if (dymj < 0.01)
                {
                    this.DYMJ.Text = "/";
                }
                else
                {
                    this.DYMJ.Text = dymj.ToString("0.000");
                }
                double ftmj = (double)row["FTMJ"];
                if (ftmj < 0.01)
                {
                    this.FTMJ.Text = "/";
                }
                else
                {
                    this.FTMJ.Text = ftmj.ToString("0.000");
                }
                this.FZRQ.Text = row["FZRQ"].ToString().Trim();
                this.DJRQ.Text = row["FZRQ"].ToString().Trim();
                this.JS.Text = row["GYQLRQK"].ToString() + "\n" + row["SM"].ToString();


                ReadOrWritexml readOrWritexml = new ReadOrWritexml();
                readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
                if (row["TDZ_JY"].ToString().Trim() == "")
                {
                    this.GY.Text = readOrWritexml.ReadXml("//集用");
                }
                if (row["TDZ_LF"].ToString().Trim() == "")
                {
                    this.LF.Text = readOrWritexml.ReadXml("//年份");
                }
                if (row["FZRQ"].ToString().Trim() == "")
                {
                    this.FZRQ.Text = DateTime.Now.ToLongDateString();
                    this.DJRQ.Text = DateTime.Now.ToLongDateString();
                }
                this.RMZF.Text = readOrWritexml.ReadXml("//人民政府");


                if (readOrWritexml.ReadXml("//上下偏离") != "")
                {
                    this.SXPL.Value = Convert.ToDecimal(readOrWritexml.ReadXml("//上下偏离"));
                }
                else
                {
                    this.SXPL.Value = 72;
                }
                if (readOrWritexml.ReadXml("//左右偏离") != "")
                {
                    this.ZYPL.Value = Convert.ToDecimal(readOrWritexml.ReadXml("//左右偏离"));
                }
                else
                {
                    this.ZYPL.Value = 30;
                }
                connection.Close();
            }
        }

        private void DY_Click(object sender, EventArgs e)
        {
            string connectionString = Tools.Uitl.GetConnectionString();//写连接字符串
            string selstring = "SELECT * FROM DCB where DJH = '" + ATT.DJH + "'";
            DataSet da_ZD = new DataSet();//定义DataSet 
            SqlDataAdapter DP = new SqlDataAdapter();//初始化适配器
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DP = Tools.DataBasic.DCB_Initadapter(selstring, connectionString);
                DP.Fill(da_ZD, "DCB");//填充Dataset
                System.Data.DataTable TB_DCB = da_ZD.Tables["DCB"];
                DataRow row = TB_DCB.Rows[0];
                row["TDZ_JY"] = this.GY.Text.Trim();
                row["TDZ_LF"] = this.LF.Text.Trim();
                row["FZRQ"] = this.FZRQ.Text.Trim();
                row["TDZSFDY"] = 1;
                double dymj = 0, fdmj = 0;
                try
                {
                    dymj = Convert.ToDouble(this.DYMJ.Text.Trim());
                    fdmj = Convert.ToDouble(this.FTMJ.Text.Trim());
                    row["DYMJ"] = dymj;
                    row["FTMJ"] = fdmj;
                }
                catch (System.Exception)
                {

                }

                try
                {
                    DP.Update(da_ZD, "DCB");
                }
                catch (System.Exception ee)
                {
                    MessageBox.Show(ee.Message, "安徽四院", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                connection.Close();
            }
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("集用", this.GY.Text.Trim());
            readOrWritexml.CreateXmlDocument("年份", this.LF.Text.Trim());
            readOrWritexml.CreateXmlDocument("上下偏离", this.SXPL.Value.ToString().Trim());
            readOrWritexml.CreateXmlDocument("左右偏离", this.ZYPL.Value.ToString().Trim());
            readOrWritexml.CreateXmlDocument("人民政府", this.RMZF.Text.Trim());

            string flxfile1 = Tools.CADTools.GetReportsFolder() + "\\集体土地使用证模板.flx";
            grid1.OpenFile(flxfile1);
            grid1.Cell(2, 1).Text = GY.Text.Trim();
            grid1.Cell(2, 4).Text = LF.Text.Trim();
            grid1.Cell(1, 5).Text = H.Text.Trim();
            grid1.Cell(4, 3).Text = TDSYQR.Text.Trim();
            grid1.Cell(6, 3).Text = ZL.Text.Trim();
            grid1.Cell(7, 3).Text = DH.Text.Trim();
            grid1.Cell(7, 14).Text = TH.Text.Trim();
            grid1.Cell(8, 3).Text = YT.Text.Trim();
            grid1.Cell(8, 14).Text = QDJG.Text.Trim();
            grid1.Cell(9, 3).Text = SYQLX.Text.Trim();
            grid1.Cell(9, 14).Text = ZZRQ.Text.Trim();
            grid1.Cell(10, 3).Text = SYQMJ.Text.Trim();
            grid1.Cell(10, 14).Text = DYMJ.Text.Trim();
            grid1.Cell(11, 14).Text = FTMJ.Text.Trim();
            grid1.Cell(16, 7).Text = RMZF.Text.Trim();
            DateTime fzdate = DateTime.Now;
            DateTime djdate = DateTime.Now;
            try
            {
                fzdate = Convert.ToDateTime(this.FZRQ.Text.Trim());
                djdate = Convert.ToDateTime(this.DJRQ.Text.Trim());
            }
            catch (Exception)
            {
                MessageBox.Show("日期格式错误！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            grid1.Cell(20, 8).Text = fzdate.Year.ToString();
            grid1.Cell(20, 13).Text = fzdate.Month.ToString();
            grid1.Cell(20, 16).Text = fzdate.Day.ToString();

            grid1.Cell(17, 21).Text = djdate.Year.ToString();
            grid1.Cell(17, 23).Text = djdate.Month.ToString();
            grid1.Cell(17, 25).Text = djdate.Day.ToString();

            grid1.Cell(7, 21).Text = JS.Text;

            FlexCell.PageSetup pageSetup = grid1.PageSetup;

            pageSetup.TopMargin = Decimal.ToSingle(SXPL.Value) / 10;
            pageSetup.LeftMargin = Decimal.ToSingle(ZYPL.Value) / 10;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                pageSetup.PrinterName = printDialog1.PrinterSettings.PrinterName;
                grid1.Print(1, 100, true);
            }
            this.Close();
        }

        private void GB_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
