using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Forms;
namespace ClassLibrary2
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
            this.AfterInstall  +=new InstallEventHandler(Installer1_AfterInstall);
            this.BeforeInstall +=new InstallEventHandler(Installer1_BeforeInstall);
        }
        private void Installer1_BeforeInstall(object o, InstallEventArgs e)
        {
            foreach (Process item in Process.GetProcesses())
            {
                if (item.ProcessName.ToUpper() == "ACAD")
                {
                    MessageBox.Show("请先退出ACAD应用程序后再安装！","土地整理",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                    base.Rollback(e.SavedState);
                }
            }

        }
        private void Installer1_AfterInstall(object o, InstallEventArgs e)
        {
            if (!SetCAD_FilePah())
            {
                MessageBox.Show("CAD搜索文件路径配置没有成功！", "土地整理", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                base.Rollback(e.SavedState);
            }
        }
        /// <summary>
        /// 获取安置时的路径
        /// </summary>
        /// <returns></returns>
        private  string GetAppPath()
        {
            string path = ""; 
            RegistryKey rsg = null;
            rsg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\竣工图制作");
            if (rsg != null)
            {
                path = rsg.GetValue("JGTZZ", "").ToString();
            }
            rsg.Close();
            return path;
        }
        /// <summary>
        /// 修改CAD支持文件路径
        /// </summary>
        /// <returns></returns>
        public bool SetCAD_FilePah()
        {
            bool Iscg = true;
            RegistryKey rsg = null;
            try
            {
                rsg = Registry.CurrentUser.OpenSubKey(@"Software\Autodesk\AutoCAD\R18.2\ACAD-A002:804\Profiles\<<未命名配置>>\General", true);
                string path = rsg.GetValue("ACAD", "").ToString().ToUpper();
                string xlj = GetAppPath().ToUpper();
                if (path != "" && xlj != "")
                {
                    xlj = xlj.Substring(0, xlj.Length - 1);
                    if (path.IndexOf(xlj) < 0)
                    {
                        rsg.SetValue("ACAD", xlj + ";" + path);
                    }
                }
                else
                {
                    Iscg = false;
                }
                return Iscg;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "管理系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                rsg.Close();
                return false;
            }
        }
    }
}
