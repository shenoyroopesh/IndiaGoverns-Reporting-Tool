using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IndiaGovernsReportTool.Helpers;

namespace IndiaGovernsReportTool
{
    public partial class Step4 : UserControl, ISaveSettings
    {
        public Step4()
        {
            InitializeComponent();
        }

        public String Rank1Column
        {
            get { return comboBox1.GetSelection(); }
        }

        public String Rank2Column
        {
            get { return comboBox2.GetSelection(); }
        }

        public String Rank3Column
        {
            get { return comboBox3.GetSelection(); }
        }

        public void SaveSettings(Settings settings)
        {
            settings.Rank1Column = Rank1Column;
            settings.Rank2Column = Rank2Column;
            settings.Rank3Column = Rank3Column;
        }

        public void LoadSettings(Settings settings)
        {
            var chosenColumns = settings.Group1Columns.Concat(settings.Group2Columns).ToArray();
            comboBox1.AddStringArrayAsDataSource((String[])chosenColumns.Clone());
            comboBox2.AddStringArrayAsDataSource((String[])chosenColumns.Clone());
            comboBox3.AddStringArrayAsDataSource((String[])chosenColumns.Clone());

            comboBox1.SetValue(settings.Rank1Column);
            comboBox2.SetValue(settings.Rank2Column);
            comboBox3.SetValue(settings.Rank3Column);
        }
    }
}
