using ReadOrWriteXML;
using System;
using System.Windows.Forms;

namespace NCZJDDJFZ.DCB
{
    public partial class SPB : Form
    {
        public SPB()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml"; 
            readOrWritexml.CreateXmlDocument("国土资源行政主管部门初审意见", CSYJ.Text);
            readOrWritexml.CreateXmlDocument("国土资源行政主管部门初审意见", CSYJ.Text);
            readOrWritexml.CreateXmlDocument("审查人", CSSCR.Text.Trim());
            readOrWritexml.CreateXmlDocument("审查人土地登记上岗资格证号", CSRTDDJSGZH.Text.Trim());
            if (CSRJ.Checked)
            {
                readOrWritexml.CreateXmlDocument("审查日期", CSRJ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("审查日期", "");
            }
            if (CSHG.Checked)
            {
                readOrWritexml.CreateXmlDocument("初审合格", "1");
            }
            else
            {
                readOrWritexml.CreateXmlDocument("初审合格", "0");
            }
            readOrWritexml.CreateXmlDocument("国土资源行政主管部门审核意见", SHYJ.Text);
            readOrWritexml.CreateXmlDocument("审核负责人", SHFZR.Text.Trim());
            readOrWritexml.CreateXmlDocument("审核负责人土地登记上岗资格证号", SHRTDDJSGZH.Text.Trim());
            if (SHRQ_SPB.Checked)
            {
                readOrWritexml.CreateXmlDocument("审核日期", SHRQ_SPB.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("审核日期", "");
            }
            if (SHTG.Checked)
            {
                readOrWritexml.CreateXmlDocument("审核通过", "1");
            }
            else
            {
                readOrWritexml.CreateXmlDocument("审核通过", "0");
            }
            readOrWritexml.CreateXmlDocument("人民政府批准意见", PZYJ.Text);
            readOrWritexml.CreateXmlDocument("批准人", PZR.Text.Trim());
            if (PZRQ.Checked)
            {
                readOrWritexml.CreateXmlDocument("政府批准日期", PZRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("政府批准日期", "");
            }
            if (PZTG.Checked)
            {
                readOrWritexml.CreateXmlDocument("通过批准", "1");
            }
            else
            {
                readOrWritexml.CreateXmlDocument("通过批准", "0");
            }
            readOrWritexml.CreateXmlDocument("备注", SPBBZ.Text);
            readOrWritexml.CreateXmlDocument("土地证书记事", FZJS.Text);
            this.Close();
        }

        private void SPB_Load(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml"; 
            CSYJ.Text = readOrWritexml.ReadXml("//国土资源行政主管部门初审意见");
            CSSCR.Text = readOrWritexml.ReadXml("//审查人");
            CSRTDDJSGZH.Text = readOrWritexml.ReadXml("//审查人土地登记上岗资格证号");
            if (readOrWritexml.ReadXml("//审查日期") != "")
            {
                CSRJ.Checked = true;
                CSRJ.Text = readOrWritexml.ReadXml("//审查日期");
            }
            else
            {
                CSRJ.Checked = false;
            }

            if (readOrWritexml.ReadXml("//初审合格") != "1")
            {
                CSHG.Checked = false;
            }
            else
            {
                CSHG.Checked = true;
            }
            SHYJ.Text = readOrWritexml.ReadXml("//国土资源行政主管部门审核意见");
            SHFZR.Text = readOrWritexml.ReadXml("//审核负责人");
            SHRTDDJSGZH.Text = readOrWritexml.ReadXml("//审核负责人土地登记上岗资格证号");
            if (readOrWritexml.ReadXml("//审核日期") != "")
            {
                SHRQ_SPB.Checked = true;
                SHRQ_SPB.Text = readOrWritexml.ReadXml("//审核日期");
            }
            else
            {
                SHRQ_SPB.Checked = false;
            }

            if (readOrWritexml.ReadXml("//审核通过") != "1")
            {
                SHTG.Checked = false;
            }
            else
            {
                SHTG.Checked = true;
            }
            PZYJ.Text = readOrWritexml.ReadXml("//人民政府批准意见");
            PZR.Text = readOrWritexml.ReadXml("//批准人");
            if (readOrWritexml.ReadXml("//政府批准日期") != "")
            {
                PZRQ.Checked = true;
                PZRQ.Text = readOrWritexml.ReadXml("//政府批准日期");
            }
            else
            {
                PZRQ.Checked = false;
            }
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
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupBox15_Enter(object sender, EventArgs e)
        {

        }
    }
}
