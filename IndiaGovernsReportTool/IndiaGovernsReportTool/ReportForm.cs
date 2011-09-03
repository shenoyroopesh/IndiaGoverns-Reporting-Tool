using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace IndiaGovernsReportTool
{
    public partial class ReportForm : Form
    {

        public ReportForm(DataTable generalData, DataTable group1Data, DataTable group2Data,
            String group1Name, String group2Name, String Intro, String Comment, String chart1Column, String chart2Column)
        {
            InitializeComponent();
            dataGridView1.DataSource = generalData;
            dataGridView2.DataSource = group1Data;
            dataGridView3.DataSource = group2Data;
            lblIntro.Text = Intro;
            lblNorms.Text = Comment;

            lblGroup1.Text = group1Name;
            lblGroup2.Text = group2Name;

            //chart data
            var chart1Data = group1Data.Rows.Cast<DataRow>().Where(p => p["Data"].ToString() == chart1Column).First();
            var chart2Data = group2Data.Rows.Cast<DataRow>().Where(p => p["Data"].ToString() == chart2Column).First();


            var xvalues1 = group1Data.Columns.Cast<DataColumn>()
                                .Select(p => p.ColumnName)
                                .Where(name => name != "Data").ToArray();

            var xvalues2 = group2Data.Columns.Cast<DataColumn>()
                                .Select(p => p.ColumnName)
                                .Where(name => name != "Data").ToArray();


            ArrayList yvalues1 = new ArrayList();

            for (int i = 1; i < chart1Data.ItemArray.Count(); i++ )
            {
                yvalues1.Add(Convert.ToDouble(chart1Data.ItemArray[i]));
            }

            Double[] yvalues1Array = (Double[])yvalues1.ToArray(typeof(Double));

            ArrayList yvalues2 = new ArrayList();

            for (int i = 1; i < chart2Data.ItemArray.Count(); i++)
            {
                yvalues2.Add(Convert.ToDouble(chart2Data.ItemArray[i]));
            }

            Double[] yvalues2Array = (Double[])yvalues1.ToArray(typeof(Double));

            chart1.Series[0].Points.DataBindXY(xvalues1, yvalues1Array);
            chart2.Series[0].Points.DataBindXY(xvalues2, yvalues2Array);

            chart1.Titles.Add(chart1Column);
            chart2.Titles.Add(chart2Column);

        }


        private void ReportForm_Load(object sender, EventArgs e)
        {

        }
    }
}
