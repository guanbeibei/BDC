using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NCZJDDJFZ.Tools;
using NPOI.HSSF.UserModel;
using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;

namespace NCZJDDJFZ
{
    public partial class DataDic : Form
    {
        public DataDic()
        {
            InitializeComponent();
        }
        TreeNode nod = new TreeNode();
        private void DataDic_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string excelfilename = "";
            try
            {
                if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    excelfilename = this.openFileDialog1.FileName.Trim();

                    FileStream fs = new FileStream(excelfilename, FileMode.Open, FileAccess.Read);
                    HSSFWorkbook workbook = new HSSFWorkbook(fs);
                    HSSFSheet sheet = workbook.GetSheetAt(0);
                    ArrayList shx = new ArrayList();
                    if (sheet != null)
                    {
                        int zhang = sheet.LastRowNum;
                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            string string1 = "";
                            HSSFCell cell = sheet.GetRow(i).GetCell(0);
                            if (cell.CellType == 3)
                            {
                                int dn = i + 1;
                                MessageBox.Show("第 " + dn.ToString() + " 行空行！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                            if (cell.CellType == 0)
                            {
                                string1 = sheet.GetRow(i).GetCell(0).NumericCellValue.ToString();
                            }
                            else
                            {
                                string1 = sheet.GetRow(i).GetCell(0).StringCellValue.ToString();
                            }
                            if (string1.Length != 12)
                            {
                                int dn = i + 1;
                                MessageBox.Show("第 " + dn.ToString() + " 行代码位数不是12位！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                            string ss = string1 + sheet.GetRow(i).GetCell(1).StringCellValue;
                            shx.Add(ss);
                        }
                    }
                    fs.Close();
                    fs.Dispose();
                    Tools.Uitl uitl = new Uitl();
                    uitl.DrawTree_XZC(this.treeView1, shx);//画节点树
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        /// <summary>
        /// 读excel电子表格到DataSet内存表
        /// </summary>
        /// <param name="Path"></param>
        /// xcel电子表格文件名
        /// <returns></returns>
        public DataSet ExcelToDS(string Path)
        {
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 12.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from [sheet1$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");
            conn.Close();
            return ds;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (DocumentLock documentLock = AcadApplication.DocumentManager.MdiActiveDocument.LockDocument())//锁住文档以便进行写操作
            {
                string connectionString = Uitl.GetConnectionString();//写连接字符串
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
                    ArrayList array = new ArrayList();
                    int count = TB.Rows.Count;
                    #region 将DataTable的代码和名称天然数组
                    for (int i = 0; i < count; i++)
                    {
                        DataRow row = TB.Rows[i];
                        string xzdm = row[0].ToString().Trim();
                        if (xzdm.Length != 12)
                        {
                            return;
                        }
                        array.Add(xzdm + row[1].ToString().Trim());
                    }
                    #endregion
                    uitl.DrawTree_XZC(this.treeView1, array);//画节点树
                    Adapter.Dispose();
                    connection.Close();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string Filename = "";
                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Filename = this.saveFileDialog1.FileName.Trim();
                    GetAllnodes Allnodes = new GetAllnodes();
                    Allnodes.GetAllnode(this.treeView1.Nodes);
                    ArrayList dm = Allnodes.ArrayNodeName;
                    ArrayList nm = Allnodes.ArrayNodeText;
                    ArrayList ph = Allnodes.ArrayNodeFullPath;
                    ArrayList nn = Allnodes.ArrayNodeTag;
                    if (Filename != "")
                    {
                        HSSFWorkbook hSSFWorkbook = new HSSFWorkbook();
                        HSSFSheet sheet1 = hSSFWorkbook.CreateSheet("Sheet1");
                        hSSFWorkbook.CreateSheet("Sheet2");
                        hSSFWorkbook.CreateSheet("Sheet3");
                        HSSFRow row1 = sheet1.CreateRow(0);
                        row1.CreateCell(0).SetCellValue("行政代码");
                        row1.CreateCell(1).SetCellValue("单位名称");
                        for (int i = 0; i < dm.Count; i++)
                        {
                            HSSFRow row2 = sheet1.CreateRow(i + 1);
                            row2.CreateCell(0).SetCellValue(nn[i].ToString());
                            row2.CreateCell(1).SetCellValue(nm[i].ToString());
                        }
                        FileStream FileStreamfile = new FileStream(Filename, FileMode.Create);
                        hSSFWorkbook.Write(FileStreamfile);
                        FileStreamfile.Close();
                        MessageBox.Show("保存成功！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("文件名不能为空！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (nod.Level >= 4)
                {
                    MessageBox.Show("不能再添加行政村（地籍子区）以下的节点", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                TreeNode addnode = nod.Nodes.Add("新增名称");

                addnode.Name = addnode.Parent.Name + addnode.Index.ToString("000");
                //this.maskedTextBox1.Text = addnode.Name;
                int fint = addnode.Parent.Level;
                string ym = "";
                if (fint == 0) { ym = addnode.Parent.Tag.ToString().Substring(0, 2) + "  00000000"; }
                if (fint == 1) { ym = addnode.Parent.Tag.ToString().Substring(0, 4) + "  000000"; }
                if (fint == 2) { ym = addnode.Parent.Tag.ToString().Substring(0, 6) + "   000"; }
                if (fint == 3) { ym = addnode.Parent.Tag.ToString().Substring(0, 9) + "   "; }
                if (fint == 4) { ym = addnode.Parent.Tag.ToString().Substring(0, 12); }
                addnode.Tag = ym;
                this.treeView1.SelectedNode = addnode;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            nod.Remove();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            string connectionString = Uitl.GetConnectionString();//写连接字符串
            string selstring = "SELECT * FROM DataDic";
            SqlConnection Connection1 = new SqlConnection(connectionString);
            try
            {
                Connection1.Open();
                if (Tools.DataBasic.Create_Dic_table(connectionString))//如果"DataDic"表创建成功或已经存在
                {
                    SqlDataAdapter Adapter = DataBasic.Dic_Initadapter(selstring, connectionString);
                    DataSet dataset = new DataSet();
                    Adapter.Fill(dataset, "DataDic");//填充Dataset
                    GetAllnodes Allnodes = new GetAllnodes();
                    Allnodes.GetAllnode(this.treeView1.Nodes);
                    ArrayList nm = Allnodes.ArrayNodeText;
                    ArrayList ph = Allnodes.ArrayNodeFullPath;
                    ArrayList nn = Allnodes.ArrayNodeTag;
                    if (MessageBox.Show("数据库中原有内容将被全部覆盖!", "系统提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        for (int i = 0; i < dataset.Tables["DataDic"].Rows.Count; i++)
                        {
                            DataRow Row = dataset.Tables["DataDic"].Rows[i];
                            Row.Delete();
                        }
                        System.Data.DataTable TB = dataset.Tables["DataDic"];
                        DataRowCollection rowCollection = TB.Rows;
                        for (int i = 0; i < nn.Count; i++)
                        {
                            string xzdm = nn[i].ToString();
                            DataRow row = dataset.Tables["DataDic"].NewRow();
                            row["ZXDM"] = xzdm;
                            row["MC"] = nm[i].ToString();
                            row["SJDM"] = xzdm.Substring(0, 2);
                            row["DSDM"] = xzdm.Substring(2, 2);
                            row["XQDM"] = xzdm.Substring(4, 2);
                            row["XZDM"] = xzdm.Substring(6, 3);
                            row["XZCDM"] = xzdm.Substring(9, 3);
                            //row["CMZDM"] = xzdm.Substring(12, 2);
                            row["WZMC"] = ph[i].ToString();
                            dataset.Tables["DataDic"].Rows.Add(row);
                        }
                        Adapter.Update(dataset, "DataDic");
                        MessageBox.Show("数据字典上传完毕", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                Connection1.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                if (Connection1.State == ConnectionState.Open)
                {
                    Connection1.Close();
                }
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.maskedTextBox1.Text = e.Node.Text;
            if (e.Node.Tag != null)
            {
                this.maskedTextBox2.Text = e.Node.Tag.ToString();
            }
            nod = e.Node;
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void maskedTextBox1_Leave(object sender, EventArgs e)
        {
            nod.Text = this.maskedTextBox1.Text;
        }

        private void maskedTextBox2_Leave(object sender, EventArgs e)
        {
            nod.Tag = this.maskedTextBox2.Text;
            string str = this.maskedTextBox2.Text.Trim();
            int lev = nod.Level;
            if (str.Length != 12)
            {
                MessageBox.Show("行政代码长度不对！", "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }
    }
}
