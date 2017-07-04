using System;
using System.Windows.Forms;

namespace NCZJDDJFZ.Analysing
{
    public partial class Overlay : UserControl
    {
        public Overlay()
        {
            InitializeComponent();
        }

        private void RunOverlay_Click(object sender, EventArgs e)
        {
            Tools.AnalysingTool.Overlay();
        }

        private void Overlay_Load(object sender, EventArgs e)
        {

        }

        // 只导出没有侵占基本农田的宗地数据至Excel,当报表
        private void Exp2XLS_Click(object sender, System.EventArgs e)
        {
            Tools.AnalysingTool.ExpReportXLS();
        }



        // 导出所有宗地数据至Excel
        private void ExpZD2XLS_Click(object sender, System.EventArgs e)
        {
            Tools.AnalysingTool.ExpAll2XLS();
        }
        // 导出所有房屋数据至Excel
        private void ExpFW2XLS_Click(object sender, EventArgs e)
        {

        }
    }
}
