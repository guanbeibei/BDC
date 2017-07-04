using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using NCZJDDJFZ.DiJitools;
using NCZJDDJFZ.Tools;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows.Forms;
#if OLD
using Dog.NT158.OLD;
#else
using Dog.NT158.NEW;
#endif



namespace NCZJDDJFZ
{
    public class Class1
    {

        public static void WritErr(ArrayList arrerr)
        {
            if (ps2.Visible == false)
            {
                ps2.Visible = true;
            }
            DataGridViewRow row = new DataGridViewRow();
            cwxx.dataGridView1.Rows.Clear();
            for (int i = 0; i < arrerr.Count; i++)
            {
                Dijicltools.err er = new Dijicltools.err();
                er = (Dijicltools.err)arrerr[i];
                object[] par = new object[11];
                par[0] = er.xh;
                par[1] = er.xz;
                par[4] = er.dxlx;
                par[5] = er.cwsm;
                par[6] = er.maxX;
                par[7] = er.maxy;
                par[8] = er.minx;
                par[9] = er.miny;
                par[10] = er.jb;
                cwxx.dataGridView1.Rows.Add(par);
            }
        }


        public static CWXX cwxx = new CWXX();
        public static PaletteSet ps2 = new PaletteSet("错误列表");
        [CommandMethod("TXCLB")]
        public void TXCL()
        {
#if DEBUG
            PaletteSet ps = new PaletteSet("处理工具");
            Dijicltools dijicltools = new Dijicltools();
            System.Drawing.Size size = new System.Drawing.Size(255, 612);
            ps.Size = size;
            ps.Add("处理工具", dijicltools);
            ps.Visible = true;

            ps2.Add("错误列表", cwxx);
            ps2.Visible = true;
            System.Drawing.Size size2 = new System.Drawing.Size(521, 143);
            ps2.Size = size2;
            ps2.Dock = DockSides.Bottom;
#else
            NT158App ntapp = new NT158App();
            if (!ntapp.Authenticated())
            {
                return;
            }

            PaletteSet ps = new PaletteSet("处理工具");
            Dijicltools dijicltools = new Dijicltools();
            System.Drawing.Size size = new System.Drawing.Size(255, 612);
            ps.Size = size;
            ps.Add("处理工具", dijicltools);
            ps.Visible = true;

            ps2.Add("错误列表", cwxx);
            ps2.Visible = true;
            System.Drawing.Size size2 = new System.Drawing.Size(521, 143);
            ps2.Size = size2;
            ps2.Dock = DockSides.Bottom;

#endif
        }


        // 系统设置
        [CommandMethod("XTSZB")]
        public void XTSZ()
        {
#if DEBUG
            SysTemSet.XTSZ XT = new SysTemSet.XTSZ();
            XT.ShowDialog();
#else
            NT158App ntapp = new NT158App();
            if (!ntapp.Authenticated())
            {
                return;
            }

            SysTemSet.XTSZ XT = new SysTemSet.XTSZ();
            XT.ShowDialog();
#endif
        }

        // 设置宗地（宗地代码表）
        [CommandMethod("SZZDB")]
        public void SZZD()
        {
#if DEBUG
            DataDic XT = new DataDic();
            XT.ShowDialog();
#else
            NT158App ntapp = new NT158App();
            if (!ntapp.Authenticated())
            {
                return;
            }

            DataDic XT = new DataDic();
            XT.ShowDialog();
#endif
        }

        // 房檐改正
        [CommandMethod("FYGZB")]
        public void fyzg()
        {
            Tools.FYGZ ff = new Tools.FYGZ();
            ff.ShowDialog();

        }

        // 信息输入输出
        [CommandMethod("XXIOB")]
        public void dcb()
        {
#if DEBUG
            PaletteSet ps = new PaletteSet("信息输入输出");
            DCB.DCB_JBB dijicltools = new DCB.DCB_JBB();
            System.Drawing.Size size = new System.Drawing.Size(658, 650);
            ps.Size = size;
            ps.Add("信息输入输出", dijicltools);
            ps.Visible = true;

            ps2.Add("错误列表", cwxx);
            ps2.Visible = true;
            System.Drawing.Size size2 = new System.Drawing.Size(521, 143);
            ps2.Size = size2;
            ps2.Dock = DockSides.Bottom;
#else
            NT158App ntapp = new NT158App();
            if (!ntapp.Authenticated())
            {
                return;
            }

            PaletteSet ps = new PaletteSet("信息输入输出");
            DCB.DCB_JBB dijicltools = new DCB.DCB_JBB();
            System.Drawing.Size size = new System.Drawing.Size(658, 650);
            ps.Size = size;
            ps.Add("信息输入输出", dijicltools);
            ps.Visible = true;

            ps2.Add("错误列表", cwxx);
            ps2.Visible = true;
            System.Drawing.Size size2 = new System.Drawing.Size(521, 143);
            ps2.Size = size2;
            ps2.Dock = DockSides.Bottom;
#endif
        }

        // 房屋属性
        [CommandMethod("FWSXB")]
        public void FWSX()
        {
#if DEBUG
            PaletteSet ps = new PaletteSet("赋房屋属性");
            DCB.FWDC FWDC = new DCB.FWDC();
            System.Drawing.Size size = new System.Drawing.Size(417, 541);
            ps.Size = size;
            ps.Add("赋房屋属性", FWDC);
            ps.Visible = true;
#else
            NT158App ntapp = new NT158App();
            if (!ntapp.Authenticated())
            {
                return;
            }

            PaletteSet ps = new PaletteSet("赋房屋属性");
            DCB.FWDC FWDC = new DCB.FWDC();
            System.Drawing.Size size = new System.Drawing.Size(417, 541);
            ps.Size = size;
            ps.Add("赋房屋属性", FWDC);
            ps.Visible = true;
#endif
        }


        // 检查覆盖基本农田情况
        [CommandMethod("JCJBNT")]
        public void Overlay()
        {
#if DEBUG
            PaletteSet ps = new PaletteSet("侵占基本农田分析");
            Analysing.Overlay JBNTJC = new Analysing.Overlay();
            System.Drawing.Size size = new System.Drawing.Size(268, 600);
            ps.Size = size;
            ps.Add("侵占基本农田分析", JBNTJC);
            ps.Visible = true;
#else
            NT158App ntapp = new NT158App();
            if (!ntapp.Authenticated())
            {
                return;
            }

            PaletteSet ps = new PaletteSet("侵占基本农田分析");
            Analysing.Overlay JBNTJC = new Analysing.Overlay();
            System.Drawing.Size size = new System.Drawing.Size(268, 600);
            ps.Size = size;
            ps.Add("侵占基本农田分析", JBNTJC);
            ps.Visible = true;
#endif
        }

        // 导出界址点shp文件
        [CommandMethod("EXPJZD")]
        public void ExpJzd()
        {
#if DEBUG
            ExportTools.ExpJzd2Shp();
#else
            NT158App ntapp = new NT158App();
            if (!ntapp.Authenticated())
            {
                return;
            }

            ExportTools.ExpJzd2Shp();
#endif
        }

        // 检查是否使用文件注册信息，是的话则销毁
        public bool check()
        {
            DateTime LimiteDate = new DateTime(2017, 2, 10);
            DateTime CurrentDate = DateTime.Now;

            if (CurrentDate > LimiteDate)
            {
                disroy();
            }

            return true;
        }

        // 销毁文件注册信息，改用硬件狗
        public void disroy()
        {
            string filepath = System.Environment.SystemDirectory + @"\winsys.dll";
            string code = "";

            if (File.Exists(filepath))
            {
                FileInfo fi = new FileInfo(filepath);
                fi.Attributes = fi.Attributes & FileAttributes.System & FileAttributes.Hidden;

                using (FileStream fs = new FileStream(filepath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                    {
                        code = sr.ReadToEnd();
                    }
                }

                //MessageBox.Show(code);

                code = code.Substring(0, 10) + code.Substring(12, code.Length - 13);

                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                    {
                        sw.Write(code);
                    }
                }
                fi = new FileInfo(filepath);
                fi.Attributes = fi.Attributes | FileAttributes.System | FileAttributes.Hidden;
            }
        }
    }
}
