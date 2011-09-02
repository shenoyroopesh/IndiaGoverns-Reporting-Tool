using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            var chart1Data = group1Data.Rows.Cast<DataRow>().Where(p => p["Data"].ToString() == chart1Column);
            var chart2Data = group2Data.Rows.Cast<DataRow>().Where(p => p["Data"].ToString() == chart2Column);

            chart1.DataSource = chart1Data;
            chart1.DataBind();

            chart2.DataSource = chart2Data;
            chart2.DataBind();
        }


        private void ReportForm_Load(object sender, EventArgs e)
        {

        }
    }
}
