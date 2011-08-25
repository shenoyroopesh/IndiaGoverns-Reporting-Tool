using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndiaGovernsReportTool
{
    public partial class Step3 : UserControl
    {
        public String chart1Column
        {
            get { return comboBox1.SelectedValue.ToString(); }
        }

        public String chart2Column
        {
            get { return comboBox2.SelectedValue.ToString(); }
        }

        public Step3(String[] columns)
        {
            comboBox1.DataSource = columns;
            comboBox2.DataSource = columns;
        }
    }
}
