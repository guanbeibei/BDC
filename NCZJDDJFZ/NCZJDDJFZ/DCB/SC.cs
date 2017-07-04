using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.OleDb;
using System.Data.Common;
using NCZJDDJFZ.Tools;
using JiHeJiSuanTool;
using System.Collections;
using System.IO;
using Microsoft.VisualBasic;
using ReadOrWriteXML;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.GraphicsInterface;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;

using AcadApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using ACDBOpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode;
using ACDBApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using ACDBColor = Autodesk.AutoCAD.Colors.Color;

namespace NCZJDDJFZ.DCB
{
    public partial class SC : UserControl
    {
        public SC()
        {
            InitializeComponent();
        }

        private void SC_Load(object sender, EventArgs e)
        {
            DCB_JBB dcb = new DCB_JBB();
            dcb.DrawDataTree();// 画数据字典节点树
        }

        public void AddHuzhulistbox(DataSet dataset_DCB)
        {
            
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {

        }
    }
}
