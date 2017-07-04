using ReadOrWriteXML;
using System;
using System.Windows.Forms;

namespace NCZJDDJFZ.DCB
{
    public partial class SHBPZ : Form
    {
        public SHBPZ()
        {
            InitializeComponent();
        }

        private void SHBPZ_Load(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            QSDCJS1.Text=readOrWritexml.ReadXml("//权属调查记事1");
            DCYXM1.Text = readOrWritexml.ReadXml("//调查员姓名1");
            if (readOrWritexml.ReadXml("//调查日期1") == "")
            {
                DCRQ1.Checked = false;
            }
            else
            {
                DCRQ1.Checked = true;
                DCRQ1.Text = readOrWritexml.ReadXml("//调查日期1");
            }
            DJKZJS1.Text = readOrWritexml.ReadXml("//地籍测量记事1");
            CLYXM1.Text = readOrWritexml.ReadXml("//测量员姓名1");
            if (readOrWritexml.ReadXml("//测量日期1") == "")
            {
                CLRQ1.Checked = false;
            }
            else
            {
                CLRQ1.Checked = true;
                CLRQ1.Text = readOrWritexml.ReadXml("//测量日期1");
            }
            DJDCJGSHYJ1.Text = readOrWritexml.ReadXml("//地籍调查结果审核意见1");
            SHRXM1.Text = readOrWritexml.ReadXml("//审核人姓名1");
            if (readOrWritexml.ReadXml("//审核日期1") == "")
            {
                SHRQ1.Checked = false;
            }
            else
            {
                SHRQ1.Checked = true;
                SHRQ1.Text = readOrWritexml.ReadXml("//审核日期1");
            }
            if (readOrWritexml.ReadXml("//审核合格1") == "1")
            {
                SHHG1.Checked = true;
            }
            else
            {
                SHHG1.Checked = false;
            }



            QSDCJS2.Text = readOrWritexml.ReadXml("//权属调查记事2");
            DCYXM2.Text = readOrWritexml.ReadXml("//调查员姓名2");
            if (readOrWritexml.ReadXml("//调查日期2") == "")
            {
                DCRQ2.Checked = false;
            }
            else
            {
                DCRQ2.Checked = true;
                DCRQ2.Text = readOrWritexml.ReadXml("//调查日期2");
            }
            DJKZJS2.Text = readOrWritexml.ReadXml("//地籍测量记事2");
            CLYXM2.Text = readOrWritexml.ReadXml("//测量员姓名2");
            if (readOrWritexml.ReadXml("//测量日期2") == "")
            {
                CLRQ2.Checked = false;
            }
            else
            {
                CLRQ2.Checked = true;
                CLRQ2.Text = readOrWritexml.ReadXml("//测量日期2");
            }
            DJDCJGSHYJ2.Text = readOrWritexml.ReadXml("//地籍调查结果审核意见2");
            SHRXM2.Text = readOrWritexml.ReadXml("//审核人姓名2");
            if (readOrWritexml.ReadXml("//审核日期2") == "")
            {
                SHRQ2.Checked = false;
            }
            else
            {
                SHRQ2.Checked = true;
                SHRQ2.Text = readOrWritexml.ReadXml("//审核日期2");
            }
            if (readOrWritexml.ReadXml("//审核合格2") == "1")
            {
                SHHG2.Checked = true;
            }
            else
            {
                SHHG2.Checked = false;
            }
             
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("权属调查记事1", QSDCJS1.Text);
            readOrWritexml.CreateXmlDocument("权属调查记事1", QSDCJS1.Text);
            readOrWritexml.CreateXmlDocument("调查员姓名1", DCYXM1.Text.Trim());
            if (DCRQ1.Checked)
            {
                readOrWritexml.CreateXmlDocument("调查日期1", DCRQ1.Text);
            }
            else
            {
                readOrWritexml.CreateXmlDocument("调查日期1", "");
            }
            readOrWritexml.CreateXmlDocument("地籍测量记事1", DJKZJS1.Text);
            readOrWritexml.CreateXmlDocument("测量员姓名1", CLYXM1.Text.Trim());
            if (CLRQ1.Checked)
            {
                readOrWritexml.CreateXmlDocument("测量日期1", CLRQ1.Text);
            }
            else
            {
                readOrWritexml.CreateXmlDocument("测量日期1", "");
            }
            readOrWritexml.CreateXmlDocument("地籍调查结果审核意见1", DJDCJGSHYJ1.Text);
            readOrWritexml.CreateXmlDocument("审核人姓名1", SHRXM1.Text.Trim());
            if (SHRQ1.Checked)
            {
                readOrWritexml.CreateXmlDocument("审核日期1", SHRQ1.Text);
            }
            else
            {
                readOrWritexml.CreateXmlDocument("审核日期1", "");
            }
            if (SHHG1.Checked)
            {
                readOrWritexml.CreateXmlDocument("审核合格1", "1");
            }
            else
            {
                readOrWritexml.CreateXmlDocument("审核合格1", "0");
            }

            readOrWritexml.CreateXmlDocument("权属调查记事2", QSDCJS2.Text);
            readOrWritexml.CreateXmlDocument("调查员姓名2", DCYXM2.Text.Trim());
            if (DCRQ2.Checked)
            {
                readOrWritexml.CreateXmlDocument("调查日期2", DCRQ2.Text);
            }
            else
            {
                readOrWritexml.CreateXmlDocument("调查日期2", "");
            }
            readOrWritexml.CreateXmlDocument("地籍测量记事2", DJKZJS2.Text);
            readOrWritexml.CreateXmlDocument("测量员姓名2", CLYXM2.Text.Trim());
            if (CLRQ2.Checked)
            {
                readOrWritexml.CreateXmlDocument("测量日期2", CLRQ2.Text);
            }
            else
            {
                readOrWritexml.CreateXmlDocument("测量日期2", "");
            }
            readOrWritexml.CreateXmlDocument("地籍调查结果审核意见2", DJDCJGSHYJ2.Text);
            readOrWritexml.CreateXmlDocument("审核人姓名2", SHRXM2.Text.Trim());
            if (SHRQ2.Checked)
            {
                readOrWritexml.CreateXmlDocument("审核日期2", SHRQ2.Text);
            }
            else
            {
                readOrWritexml.CreateXmlDocument("审核日期2", "");
            }
            if (SHHG2.Checked)
            {
                readOrWritexml.CreateXmlDocument("审核合格2", "1");
            }
            else
            {
                readOrWritexml.CreateXmlDocument("审核合格2", "0");
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
