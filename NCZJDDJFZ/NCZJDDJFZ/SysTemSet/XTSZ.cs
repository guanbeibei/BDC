using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace NCZJDDJFZ.SysTemSet
{
    public partial class XTSZ : Form
    {
        public XTSZ()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.JBZL.Text = Tools.ReadWriteReg.read_reg("界标种类");
            this.zdtcflj.Text = Tools.ReadWriteReg.read_reg("宗地图存放路径");
            this.zdctcflj.Text = Tools.ReadWriteReg.read_reg("宗地草图存放路径");
            this.smyxgjlj.Text = Tools.ReadWriteReg.read_reg("扫描影像挂接路径");
            this.zgdwqc.Text = Tools.ReadWriteReg.read_reg("主管单位全称");
            this.cldwqc.Text = Tools.ReadWriteReg.read_reg("测量单位全称");
            this.blcfm.Text = Tools.ReadWriteReg.read_reg("比例尺分母");
            this.fwqmc.Text = Tools.ReadWriteReg.read_reg("服务器名称");
            this.yhmc.Text = Tools.ReadWriteReg.read_reg("用户名称");
            this.yhmm.Text = Tools.ReadWriteReg.read_reg("用户密码");
            this.sjkmc.Text = Tools.ReadWriteReg.read_reg("数据库名称");
            this.FZJG.Text = Tools.ReadWriteReg.read_reg("发证机关");
            this.zgdwqc.Text = Tools.ReadWriteReg.read_reg("主管单位全称");
            this.sjkwjlj.Text = Tools.ReadWriteReg.read_reg("数据库文件存放路径");
            this.FDMJ.Text = Tools.ReadWriteReg.read_reg("法定面积");

            this.jbntsjwjlj.Text = Tools.ReadWriteReg.read_reg("基本农田SHP文件");

            string cq = Tools.ReadWriteReg.read_reg("测区名称");
            if (string.IsNullOrEmpty(cq))
                this.comboBox1.SelectedItem = "默认";
            else
                this.comboBox1.SelectedItem = Tools.ReadWriteReg.read_reg("测区名称");


            string JzdOutputType = Tools.ReadWriteReg.read_reg("界址点编号输出方式");
            if (string.IsNullOrEmpty(JzdOutputType))
                this.cbJzdOutputType.SelectedItem = "子区和宗内编号";
            else
                this.cbJzdOutputType.SelectedItem = Tools.ReadWriteReg.read_reg("界址点编号输出方式");
        }

        private void qd_Click_1(object sender, EventArgs e)
        {
            Tools.ReadWriteReg.write_reg("界标种类", this.JBZL.Text);
            Tools.ReadWriteReg.write_reg("宗地图存放路径", this.zdtcflj.Text.Trim());
            Tools.ReadWriteReg.write_reg("宗地草图存放路径", this.zdctcflj.Text.Trim());
            Tools.ReadWriteReg.write_reg("扫描影像挂接路径", this.smyxgjlj.Text.Trim());
            Tools.ReadWriteReg.write_reg("主管单位全称", this.zgdwqc.Text.Trim());
            Tools.ReadWriteReg.write_reg("测量单位全称", this.cldwqc.Text.Trim());
            Tools.ReadWriteReg.write_reg("比例尺分母", this.blcfm.Text.Trim());
            Tools.ReadWriteReg.write_reg("服务器名称", this.fwqmc.Text.Trim());
            Tools.ReadWriteReg.write_reg("用户名称", this.yhmc.Text.Trim());
            Tools.ReadWriteReg.write_reg("用户密码", this.yhmm.Text.Trim());
            Tools.ReadWriteReg.write_reg("数据库名称", this.sjkmc.Text.Trim());
            Tools.ReadWriteReg.write_reg("发证机关", this.FZJG.Text.Trim());
            Tools.ReadWriteReg.write_reg("主管单位全称", this.zgdwqc.Text.Trim());
            Tools.ReadWriteReg.write_reg("数据库文件存放路径", this.sjkwjlj.Text.Trim());
            Tools.ReadWriteReg.write_reg("法定面积", this.FDMJ.Text.Trim());

            Tools.ReadWriteReg.write_reg("基本农田SHP文件", this.jbntsjwjlj.Text.Trim());
            Tools.ReadWriteReg.write_reg("测区名称", this.comboBox1.Text.Trim());
            Tools.ReadWriteReg.write_reg("界址点编号输出方式", this.cbJzdOutputType.Text.Trim());
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.smyxgjlj.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.zdtcflj.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void qx_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.sjkwjlj.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Tools.ReadWriteReg.write_reg("宗地图存放路径", this.zdtcflj.Text.Trim());
            Tools.ReadWriteReg.write_reg("宗地草图存放路径", this.zdctcflj.Text.Trim());
            Tools.ReadWriteReg.write_reg("扫描影像挂接路径", this.smyxgjlj.Text.Trim());
            Tools.ReadWriteReg.write_reg("主管单位全称", this.zgdwqc.Text.Trim());
            Tools.ReadWriteReg.write_reg("测量单位全称", this.cldwqc.Text.Trim());
            Tools.ReadWriteReg.write_reg("比例尺分母", this.blcfm.Text.Trim());
            Tools.ReadWriteReg.write_reg("服务器名称", this.fwqmc.Text.Trim());
            Tools.ReadWriteReg.write_reg("用户名称", this.yhmc.Text.Trim());
            Tools.ReadWriteReg.write_reg("用户密码", this.yhmm.Text.Trim());
            Tools.ReadWriteReg.write_reg("数据库名称", this.sjkmc.Text.Trim());
            Tools.ReadWriteReg.write_reg("发证机关", this.FZJG.Text.Trim());
            Tools.ReadWriteReg.write_reg("主管单位全称", this.zgdwqc.Text.Trim());
            Tools.ReadWriteReg.write_reg("数据库文件存放路径", this.sjkwjlj.Text.Trim());
            Tools.ReadWriteReg.write_reg("法定面积", this.FDMJ.Text.Trim());

            Tools.ReadWriteReg.write_reg("基本农田SHP文件", this.jbntsjwjlj.Text.Trim());
            Tools.ReadWriteReg.write_reg("测区名称", this.comboBox1.Text.Trim());
            Tools.ReadWriteReg.write_reg("界址点编号输出方式", this.cbJzdOutputType.Text.Trim());

            string DataName = sjkmc.Text;//数据库名称
            DataName = DataName.Trim();
            string path = sjkwjlj.Text;//路径
            path = path.Trim() + "\\";
            string Syhmc = yhmc.Text.Trim();//用户名称
            string Syhmm1 = yhmm.Text.Trim();//用户密码
            string Sfwqmc = fwqmc.Text.Trim();
            if (DataName == "" || path == "")
            {
                MessageBox.Show("数据库名称或文件路径为空，数据库创建失败！", "创建数据库", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            //SqlConnection myConn = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");
            SqlConnection myConn = new SqlConnection("Server = '" + Sfwqmc + "' ;Integrated security=SSPI;database=master");
            string str = "CREATE DATABASE " + DataName + " ON PRIMARY " +
                                 "(NAME = " + DataName + "_Data, " +
                                 "FILENAME = '" + path + DataName + "_Data.mdf ')" +
                                 "LOG ON (NAME = " + DataName + "_Log, " +
                                 "FILENAME = '" + path + DataName + "_Log.ldf ') ";
            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                MessageBox.Show("数据库创建成功！", "创建数据库", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString(), "创建数据库", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.zdctcflj.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }


        // TODO:添加设置基本农田shp文件的配置信息
        // 浏览基本农田shp文件存放位置按钮
        private void button6_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.jbntsjwjlj.Text = this.openFileDialog1.FileName;
            }
        }

    }
}
