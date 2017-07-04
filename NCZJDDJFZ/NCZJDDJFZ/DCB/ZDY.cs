using ReadOrWriteXML;
using System;
using System.Windows.Forms;

namespace NCZJDDJFZ.DCB
{
    public partial class ZDY : Form
    {
        public ZDY()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            readOrWritexml.CreateXmlDocument("界址签章表自定义1", textBox1.Text.Trim());
            readOrWritexml.CreateXmlDocument("界址签章表自定义1", textBox1.Text.Trim());
            readOrWritexml.CreateXmlDocument("界址签章表自定义2", textBox2.Text.Trim());
            readOrWritexml.CreateXmlDocument("界址签章表自定义3", textBox3.Text.Trim());
            this.Close();
        }

        private void ZDY_Load(object sender, EventArgs e)
        {
            ReadOrWritexml readOrWritexml = new ReadOrWritexml();
            readOrWritexml.XmlFile = Tools.CADTools.GetSettingsFolder() + "自定义.xml";
            textBox1.Text = readOrWritexml.ReadXml("//界址签章表自定义1");
            textBox2.Text = readOrWritexml.ReadXml("//界址签章表自定义2");
            textBox3.Text = readOrWritexml.ReadXml("//界址签章表自定义3");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
