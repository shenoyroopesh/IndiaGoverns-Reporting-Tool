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
    public partial class Step3 : UserControl, ISaveSettings
    {
        public String Chart1Column1
        {
            get { return chart1Column1.GetSelection(); }
        }

        public String Chart1Column2
        {
            get { return chart1Column2.GetSelection(); }
        }

        public String Chart2Column1
        {
            get { return chart2Column1.GetSelection(); }
        }

        public String Chart2Column2
        {
            get { return null; }  //chart2Column2.SelectedValue.ToString(); }
        }

        public Step3() 
        {
            InitializeComponent();
        }

        public void SaveSettings(Settings settings)
        {
            settings.Chart1Column1 = Chart1Column1;
            settings.Chart1Column2 = Chart1Column2;
            settings.Chart2Column1 = Chart2Column1;
            settings.Chart2Column2 = Chart2Column2;
        }

        public void LoadSettings(Settings settings)
        {
            var columns1 = settings.Group1Columns;
            var columns2 = settings.Group2Columns.Concat(settings.Group1Columns).ToArray();

            var columns1Copy = new String[columns1.Length];
            var columns2Copy = new String[columns2.Length];
            
            columns1.CopyTo(columns1Copy, 0);
            columns2.CopyTo(columns2Copy, 0);
            chart1Column1.AddStringArrayAsDataSource(columns1);
            chart1Column2.AddStringArrayAsDataSource(columns1Copy);
            chart2Column1.AddStringArrayAsDataSource(columns2);
            chart2Column2.AddStringArrayAsDataSource(columns2Copy);

            chart1Column1.SetValue(settings.Chart1Column1);
            chart1Column2.SetValue(settings.Chart1Column2);
            chart2Column1.SetValue(settings.Chart2Column1);
            chart2Column2.SetValue(settings.Chart2Column2);
        }
    }
}