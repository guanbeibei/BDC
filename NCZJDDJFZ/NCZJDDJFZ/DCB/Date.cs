using System.Windows.Forms;

namespace NCZJDDJFZ.DCB
{
    public partial class Date : Form
    {
        public Date()
        {
            InitializeComponent();
        }
        public   string nsj = "";
        public string sj
        {
            get { return  this.nsj; }
        }
        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
                nsj = monthCalendar1.SelectionStart.ToString("yyyy年MM月dd日");
        }

        //private void Date_Deactivate(object sender, EventArgs e)
        //{
        //    this.Close();
        //    nsj = "";
        //}

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            this.Close();
        }
    }
}
