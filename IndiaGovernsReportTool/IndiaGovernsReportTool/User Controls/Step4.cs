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
    public partial class Step4 : UserControl
    {
        public Step4(String[] chosenColumns)
        {
            InitializeComponent();
            comboBox1.DataSource = (String[])chosenColumns.Clone();
            comboBox2.DataSource = (String[])chosenColumns.Clone();
            comboBox3.DataSource = (String[])chosenColumns.Clone();
        }

        public String rank1Column
        {
            get { return comboBox1.SelectedValue.ToString(); }
        }

        public String rank2Column
        {
            get { return comboBox2.SelectedValue.ToString(); }
        }

        public String rank3Column
        {
            get { return comboBox3.SelectedValue.ToString(); }
        }
    }
}
