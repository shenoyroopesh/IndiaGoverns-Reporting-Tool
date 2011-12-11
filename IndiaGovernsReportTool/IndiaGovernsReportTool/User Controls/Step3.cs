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
        public String Chart1Column1
        {
            get { return chart1Column1.SelectedValue.ToString(); }
        }

        public String Chart1Column2
        {
            get { return chart1Column2.SelectedValue.ToString(); }
        }

        public String Chart2Column
        {
            get { return chart2Column.SelectedValue.ToString(); }
        }

        public Step3(String[] columns1, String[] columns2)
        {
            InitializeComponent();
            String[] columns1Copy = new String[columns1.Length];
            columns1.CopyTo(columns1Copy, 0);
            chart1Column1.DataSource = columns1;
            chart1Column2.DataSource = columns1Copy;
            chart2Column.DataSource = columns2;
        }
    }
}