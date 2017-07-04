using ReadOrWriteXML;
using System;
using System.Windows.Forms;

namespace NCZJDDJFZ.DCB
{
    public partial class SBB : Form
    {
        public SBB()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("组长姓名", ZZXM.Text.Trim());
            readOrWritexml.CreateXmlDocument("组长姓名", ZZXM.Text.Trim());
            readOrWritexml.CreateXmlDocument("村民组意见", CMZYJ.Text);
            readOrWritexml.CreateXmlDocument("主任姓名", ZRXM.Text.Trim());
            readOrWritexml.CreateXmlDocument("村委会意见", CWHYJ.Text);
            readOrWritexml.CreateXmlDocument("勘察人", KCR.Text.Trim());
            readOrWritexml.CreateXmlDocument("现场勘察意见", XCKCYJ.Text);
            readOrWritexml.CreateXmlDocument("镇规划部门负责人", XZGHBMFZR.Text.Trim());
            readOrWritexml.CreateXmlDocument("镇规划部门意见", XZGHBMYJ.Text);
            readOrWritexml.CreateXmlDocument("县住建委规划负责人", XZJWFZR.Text.Trim());
            readOrWritexml.CreateXmlDocument("县住建委规划意见", XZJWGHYJ.Text);
            readOrWritexml.CreateXmlDocument("乡政府审批意见", XZZFSPYJ.Text);
            readOrWritexml.CreateXmlDocument("县国土部门审查人", XGTBMSCR.Text.Trim());
            readOrWritexml.CreateXmlDocument("县国土部门审查意见", XGTBMSCYJ.Text);
            readOrWritexml.CreateXmlDocument("县政府审查意见", XZFSCYJ.Text);
            readOrWritexml.CreateXmlDocument("申报表编号", SBB_BH.Text.Trim());
            readOrWritexml.CreateXmlDocument("申请理由", SQLY.Text);
            if (CMZQZRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("村民组签字日期", CMZQZRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("村民组签字日期", "");
            }
            if (CWHQZRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("村委会签字日期", CWHQZRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("村委会签字日期", "");
            }
            if (KCRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("现场勘察签字日期", KCRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("现场勘察签字日期", "");
            }
            if (XZGHBMQZRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("镇规划部门签字日期", XZGHBMQZRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("镇规划部门签字日期", "");
            }
            if (XZJWQZRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("县住建委签字日期", XZJWQZRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("县住建委签字日期", "");
            }
            if (XZZFSPRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("乡政府签字日期", XZZFSPRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("乡政府签字日期", "");
            }
            if (XGTBMSCRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("县国土部门签字日期", XGTBMSCRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("县国土部门签字日期", "");
            }
            if (XZFSCRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("县政府签字日期", XZFSCRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("县政府签字日期", "");
            }
            if (SBB_TBRQ.Checked == true)
            {
                readOrWritexml.CreateXmlDocument("申报表填表日期", SBB_TBRQ.Text.Trim());
            }
            else
            {
                readOrWritexml.CreateXmlDocument("申报表填表日期", "");
            }
            this.Close();
        }

        private void SBB_Load(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            ZZXM.Text = readOrWritexml.ReadXml("//组长姓名");
            CMZYJ.Text = readOrWritexml.ReadXml("//村民组意见");
            ZRXM.Text = readOrWritexml.ReadXml("//主任姓名");
            CWHYJ.Text = readOrWritexml.ReadXml("//村委会意见");
            KCR.Text = readOrWritexml.ReadXml("//勘察人");
            XCKCYJ.Text = readOrWritexml.ReadXml("//现场勘察意见");
            XZGHBMFZR.Text = readOrWritexml.ReadXml("//镇规划部门负责人");
            XZGHBMYJ.Text = readOrWritexml.ReadXml("//镇规划部门意见");
            XZJWFZR.Text = readOrWritexml.ReadXml("//县住建委规划负责人");
            XZJWGHYJ.Text = readOrWritexml.ReadXml("//县住建委规划意见");
            XZZFSPYJ.Text = readOrWritexml.ReadXml("//乡政府审批意见");
            XGTBMSCR.Text = readOrWritexml.ReadXml("//县国土部门审查人");
            XGTBMSCYJ.Text = readOrWritexml.ReadXml("//县国土部门审查意见");
            XZFSCYJ.Text = readOrWritexml.ReadXml("//县政府审查意见");
            SBB_BH.Text = readOrWritexml.ReadXml("//申报表编号");
            SQLY.Text = readOrWritexml.ReadXml("//申请理由");
            if (readOrWritexml.ReadXml("//村民组签字日期")!="")
            {
                CMZQZRQ.Checked = true;
                CMZQZRQ.Text = readOrWritexml.ReadXml("//村民组签字日期");  
            }
            else
            {
                CMZQZRQ.Checked = false;
            }
            if (readOrWritexml.ReadXml("//村委会签字日期") != "")
            {
                CWHQZRQ.Checked = true;
                CWHQZRQ.Text = readOrWritexml.ReadXml("//村民组签字日期");  
            }
            else
            {
                CWHQZRQ.Checked = false;
            }
            if (readOrWritexml.ReadXml("//现场勘察签字日期") != "")
            {
                KCRQ.Checked = true;
                KCRQ.Text = readOrWritexml.ReadXml("//现场勘察签字日期");
            }
            else
            {
                KCRQ.Checked = false;
            }
            if (readOrWritexml.ReadXml("//镇规划部门签字日期") != "")
            {
                XZGHBMQZRQ.Checked = true;
                XZGHBMQZRQ.Text = readOrWritexml.ReadXml("//镇规划部门签字日期");
            }
            else
            {
                XZGHBMQZRQ.Checked = false;
            }
            if (readOrWritexml.ReadXml("//县住建委签字日期") != "")
            {
                XZJWQZRQ.Checked = true;
                XZJWQZRQ.Text = readOrWritexml.ReadXml("//县住建委签字日期");
            }
            else
            {
                XZJWQZRQ.Checked = false;
            }
            if (readOrWritexml.ReadXml("//乡政府签字日期") != "")
            {
                XZZFSPRQ.Checked = true;
                XZZFSPRQ.Text = readOrWritexml.ReadXml("//乡政府签字日期");
            }
            else
            {
                XZZFSPRQ.Checked = false;
            }
            if (readOrWritexml.ReadXml("//县国土部门签字日期") != "")
            {
                XGTBMSCRQ.Checked = true;
                XGTBMSCRQ.Text = readOrWritexml.ReadXml("//县国土部门签字日期");
            }
            else
            {
                XGTBMSCRQ.Checked = false;
            }
            if (readOrWritexml.ReadXml("//县政府签字日期") != "")
            {
                XZFSCRQ.Checked = true;
                XZFSCRQ.Text = readOrWritexml.ReadXml("//县政府签字日期");
            }
            else
            {
                XZFSCRQ.Checked = false;
            }
            if (readOrWritexml.ReadXml("//申报表填表日期") != "")
            {
                SBB_TBRQ.Checked = true;
                SBB_TBRQ.Text = readOrWritexml.ReadXml("//申报表填表日期");
            }
            else
            {
                SBB_TBRQ.Checked = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}
