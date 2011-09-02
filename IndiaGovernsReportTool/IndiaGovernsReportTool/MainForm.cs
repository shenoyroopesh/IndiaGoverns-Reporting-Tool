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
    /// <summary>
    /// Main form for the Report Generator, orchestrates the various user controls and controls the data flow between them
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Main data that is used to generate the reports
        /// </summary>
        DataSet inputData;

        String group1Name;
        
        String group2Name;

        String[] group1Columns;

        String[] group2Columns;

        String chart1Column;

        String chart2Column;


        /// <summary>
        /// Constructor for ReportGenerator
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            loadControl(new Step1());
        }

        /// <summary>
        /// Loads the particular usercontrol into the form panel
        /// </summary>
        /// <param name="uc"></param>
        private void loadControl(UserControl uc)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(uc);
        }

        /// <summary>
        /// Controls the flow of the application, depending on the current step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            Step1 step1;
            Step2 step2;
            Step3 step3;
            Step4 step4;

            UserControl control = (UserControl)panel1.Controls[0];
            //current state is determined by what control is present in the panel
            switch (control.GetType().ToString())
            {
                case "IndiaGovernsReportTool.Step1":
                    step1 = (Step1)control;
                    if (step1.data != null)
                    {
                        inputData = step1.data;
                        step2 = new Step2(inputData.Tables[0].Columns.Cast<DataColumn>()
                                                        .Select(p=>p.ColumnName)
                                                        .ToArray<String>());
                        loadControl(step2);
                    }
                    break;

                case "IndiaGovernsReportTool.Step2":
                    step2 = (Step2)control;
                    group1Name = step2.group1Name;
                    group2Name = step2.group2Name;
                    group1Columns = step2.group1Columns;
                    group2Columns = step2.group2Columns;
                    //only the group columns can be used in charts
                    step3 = new Step3(group1Columns, group2Columns);
                    loadControl(step3);
                    break;

                case "IndiaGovernsReportTool.Step3":
                    step3 = (Step3)control;
                    chart1Column = step3.chart1Column;
                    chart2Column = step3.chart2Column;
                    generateReports();
                    step4 = new Step4();
                    loadControl(step4);
                    break;

                case "IndiaGovernsReportTool.Step4":
                    break;
            }
        }


        private void generateReports()
        {
            //logic is - first break into mp constituencies, then break into mla constituencies

            //get distinct mpConstituencies
            var mpConstituencies = inputData.Tables[0].Rows.Cast<DataRow>()
                                        .Select(p => p["MpConstituency"].ToString())
                                        .Distinct<String>();

            //loop for each mpConstituency
            foreach (var mpc in mpConstituencies)
            {
                //do not process the first row, which has only averages data
                if (mpc == "State Avg")
                    continue;

                var mlaConstituencies = inputData.Tables[0].Rows.Cast<DataRow>()
                                            .Where(p => p["MPConstituency"].ToString() == mpc.ToString());

                //generate a separate report for each mla constinuency
                foreach(var mla in mlaConstituencies)
                {
                    var otherConstituencies = mlaConstituencies.Where(p => p["MLAConstituency"] != mla["MLAConstituency"]);

                    String intro = "How is "+mla["MLAConstituency"]+ " MLA Constituency performing on important health" +
                                    "indicators? How does it compare with some other constituencies " +
                                    "within the " + mla["MPConstituency"] + " MP constituency? Do these numbers collated by " +
                                    "government reflect the actual situation of Bilgi constituency? \n\n" +
                                    "What role can the MLA play in highlighting these issues with the " +
                                    "government? Can the MLA make sure there is tangible improvement " +
                                    "in health facilities and indicators in the constituency?";
                   
                    //Table1: get the general data
                    DataTable generalData = new DataTable();

                    String[] generalDataRows = { "Total constituency Population", "% Population Data available for" };

                    fillTable(mla, otherConstituencies, generalData, generalDataRows, false, false);


                    //Table2: get the Health Facilities
                    DataTable group1Data = new DataTable();

                    fillTable(mla, otherConstituencies, group1Data, group1Columns, true, false);

                    //Table3: get the Health Personnel
                    DataTable group2Data = new DataTable();
                    
                    fillTable(mla, otherConstituencies, group2Data, group2Columns, false, true);


                    String Comment = "";

                    try
                    {
                        Comment = mla["Comment"].ToString();
                    }
                    catch
                    { }

                    ReportForm report = new ReportForm(generalData, group1Data, group2Data, group1Name, group2Name,
                        intro, Comment, chart1Column, chart2Column);

                    report.Show();

                    //temporarily added to test only one report
                    return;

                }
            }

        }

        private String convertToPercentage(Decimal number)
        {
            return Convert.ToInt32(number * 100).ToString() + "%";
        }

        private void fillTable(DataRow mla, IEnumerable<DataRow> otherConstituencies, 
            DataTable generalData, String[] generalDataRows, bool norms, bool avg)
        {
            generalData.Columns.Add("Data");
            if (norms) generalData.Columns.Add("Norms");
            if (avg) generalData.Columns.Add("State Avg");
            generalData.Columns.Add(mla["MLAConstituency"].ToString());

            foreach (var c in otherConstituencies)
            {
                generalData.Columns.Add(c["MLAConstituency"].ToString());
            }

            foreach (var r in generalDataRows)
            {
                DataRow row = generalData.NewRow();
                row["Data"] = r;
                if (norms)
                {
                    try
                    {
                        row["Norms"] = Convert.ToInt32(Convert.ToDecimal(mla["Norms for " + r]));
                    }
                    catch
                    {
                        row["Norms"] = "-";
                    }                                    
                }

                if (avg)
                {
                    row["State Avg"] = inputData.Tables[0].Rows[0][r].ToString();
                }

                row[mla["MLAConstituency"].ToString()] = r.Contains("%")? 
                    convertToPercentage(Convert.ToDecimal(mla[r])): mla[r].ToString();

                foreach (var c in otherConstituencies)
                {
                    row[c["MLAConstituency"].ToString()] = r.Contains("%")?
                        convertToPercentage(Convert.ToDecimal(c[r])) : c[r].ToString();
                }
                generalData.Rows.Add(row);
            }
        }
    }
}
