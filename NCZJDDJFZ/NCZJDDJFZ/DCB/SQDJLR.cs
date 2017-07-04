using ReadOrWriteXML;
using System;
using System.Windows.Forms;

namespace NCZJDDJFZ.DCB
{
    public partial class SQDJLR : Form
    {
        public SQDJLR()
        {
            InitializeComponent();
        }

        private void SQDJLR_Load(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            richTextBox1.Text = readOrWritexml.ReadXml("//自定义申请登记的内容");
            richTextBox2.Text = readOrWritexml.ReadXml("//登记原因");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("自定义申请登记的内容", richTextBox1.Text.Trim());
            readOrWritexml.CreateXmlDocument("自定义申请登记的内容", richTextBox1.Text.Trim());
            readOrWritexml.CreateXmlDocument("登记原因", richTextBox2.Text.Trim());
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
