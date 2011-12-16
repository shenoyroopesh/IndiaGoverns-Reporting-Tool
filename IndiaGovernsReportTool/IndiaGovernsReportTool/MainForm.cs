using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.Collections;

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

        String chart1Column1;

        String chart1Column2;

        String chart2Column;

        String rank1Column;
        String rank2Column;
        String rank3Column;

        String[] commentColumns;

        const string _DATAYEAR_ = "2010-11";

        ArrayList reports = new ArrayList();
        int reportCounter = 0;

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
            Step5 step5;
            Step6 step6;

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
                    step3 = new Step3(group1Columns, group1Columns.Concat(group2Columns).ToArray());
                    loadControl(step3);
                    break;

                case "IndiaGovernsReportTool.Step3":
                    step3 = (Step3)control;
                    chart1Column1 = step3.Chart1Column1;
                    chart1Column2 = step3.Chart1Column2;
                    chart2Column = step3.Chart2Column;
                    step4 = new Step4(group1Columns.Concat(group2Columns).ToArray());
                    loadControl(step4);
                    break;

                case "IndiaGovernsReportTool.Step4":
                    step4 = (Step4)control;
                    rank1Column = step4.rank1Column;
                    rank2Column = step4.rank2Column;
                    rank3Column = step4.rank3Column;
                    
                    step5 = new Step5(group1Columns.Concat(group2Columns).ToArray());
                    loadControl(step5);
                    break;

                case "IndiaGovernsReportTool.Step5":
                    step5 = (Step5)control;
                    commentColumns = step5.CommentColumns;

                    generateReports();
                    step6 = new Step6();
                    loadControl(step6);
                    break;
            }
        }


        private void generateReports()
        {
            //calculate all the averages add avg columns

            Dictionary<String, Int32> avgCols = new Dictionary<string, int>();
        
            foreach (var col in inputData.Tables[0].Columns.Cast<DataColumn>())
            {
                //if avg column already exists, don't calculate again
                int avgCount = inputData.Tables[0].Columns.Cast<DataColumn>()
                                        .Where(p => p.ColumnName == "Avg of " + col.ColumnName)
                                        .Count();

                if (avgCount > 0) continue;

                int colAvg = 0;
                try
                {
                    colAvg = Convert.ToInt32(inputData.Tables[0].Rows.Cast<DataRow>()
                                                .Select(p => Convert.ToInt32(p[col.ColumnName]))
                                                .Average());
                }
                catch(Exception e)
                {
                    continue;
                }

                avgCols.Add("Avg of " + col.ColumnName, colAvg);        
            }

            foreach (var col in avgCols) 
                inputData.Tables[0].Columns.Add(col.Key.ToString(), typeof(Int32), col.Value.ToString());
        
        
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
                    //use only 3 other constituencies for the sake of saving space
                    var otherConstituencies = mlaConstituencies.Where(p => p["MLAConstituency"] != mla["MLAConstituency"]).Take(3);

                    String intro = "How is "+mla["MLAConstituency"]+ " MLA Constituency performing on important health" +
                                    " indicators? How does it compare with some other constituencies " +
                                    "within the " + mla["MPConstituency"] + " MP constituency? Do these numbers collated by " +
                                    "government reflect the actual situation of Bilgi constituency? \n\n" +
                                    "What role can the MLA play in highlighting these issues with the " +
                                    "government? Can the MLA make sure there is tangible improvement " +
                                    "in health facilities and indicators in the constituency?";
                   
                    //Table1: get the general data
                    DataTable generalData = new DataTable();

                    String[] generalDataRows;
                    
                    //use % column only if required
                    generalDataRows = (inputData.Tables[0].Columns.Contains("% Population Data available for"))?
                            new String[] { "Total constituency Population", "% Population Data available for" } : 
                            new String[] { "Total constituency Population" };

                    fillTable(mla, otherConstituencies, generalData, generalDataRows, false, false);


                    //Table2: First Group
                    DataTable group1Data = new DataTable();
                    fillTable(mla, otherConstituencies, group1Data, group1Columns, false, true);

                    //Table3: Second Group
                    DataTable group2Data = new DataTable();                    
                    fillTable(mla, otherConstituencies, group2Data, group2Columns, false, true);
                    

                    //comment logic - figure out which attribute to comment on.

                    String mlaToBecompared = "";
                    float maxDifference = 0;
                    String columnToBeCompared = "";


                    foreach (var column in commentColumns)
                    {
                        try
                        {
                            var maxValue = otherConstituencies.ToList().Select(p => float.Parse(p[column].ToString())).Max();
                            var difference = (maxValue - float.Parse(mla[column].ToString())) * 100 / maxValue; //percentage deviation of current mla from max mla

                            if (difference > maxDifference)
                            {
                                maxDifference = difference;
                                mlaToBecompared = otherConstituencies.ToList()
                                                        .Where(p => float.Parse(p[column].ToString()) == maxValue)
                                                        .First()["MLAConstituency"].ToString();
                                columnToBeCompared = column;
                            }

                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }

                    //Generate the comment

                    String comment = "The above data shows that " + columnToBeCompared + " in " + mla["MLAConstituency"].ToString() + " constituency "+
                        "is lower than neighbouring constituency such as "+ mlaToBecompared  +". \n\nIs this government data correct? "+
                        "Can this be brought to the government's notice?";

                    //TODO: note - depending on desc or ascending - need to make this generic
                    int rank1 = mlaConstituencies.Where(p => Convert.ToDouble(p[rank1Column]) < 
                        Convert.ToDouble(mla[rank1Column])).Count() + 1;

                    int rank2 = mlaConstituencies.Where(p => Convert.ToDouble(p[rank2Column]) <
                        Convert.ToDouble(mla[rank2Column])).Count() + 1;

                    int rank3 = mlaConstituencies.Where(p => Convert.ToDouble(p[rank3Column]) >
                        Convert.ToDouble(mla[rank3Column])).Count() + 1;


                    String rank = mla["MLAConstituency"].ToString() + " MLA Constituency Rank\n" +
                        "among " + mlaConstituencies.Count().ToString() + " MLA Constituencies in the " +
                        mpc.ToString() + " MP Constituency. \n\n" + 
                        "Rank "+rank1 + " in the " + rank1Column.Replace("2010-11", "") +
                        "\nRank " + rank2 + " in the " + rank2Column.Replace("2010-11", "").Replace("(in Rs.)", "") +
                        "\nRank " + rank3 + " in the " + rank3Column.Replace("2010-11", "").Replace("(in Rs. Lakh)", "");


                    Report report = new Report {
                        ReportName = mla["MLAConstituency"].ToString(),
                        GeneralData = generalData,
                        Group1Data = group1Data,
                        Group2Data = group2Data,
                        Group1Name = group1Name,
                        Group2Name = group2Name,
                        Intro = intro,
                        Comment = comment,
                        Chart1Column1 = chart1Column1,
                        Chart1Column2 = chart1Column2,
                        Chart2Column = chart2Column,
                        Rank = rank
                    };

                    reports.Add(report);

                    //todo: remove this
                    //break;
                }
                //break;
            }
            publishNextReport();
        }


        private void publishNextReport()
        {
            //exit condition
            if (reportCounter == reports.Count) return;
            
            Report report = (Report)reports[reportCounter];
            reportCounter += 1;
            ReportForm reportform = new ReportForm(report);
            //for next report
            reportform.FormClosed += new FormClosedEventHandler(reportform_FormClosed);
            reportform.Show();
        }

        void reportform_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((ReportForm)sender).FormClosed -= new FormClosedEventHandler(reportform_FormClosed);
            publishNextReport();
        }

        private String convertToPercentage(Decimal number)
        {
            return Convert.ToInt32(number * 100).ToString() + "%";
        }

        private void fillTable(DataRow mla, IEnumerable<DataRow> otherConstituencies, 
            DataTable generalData, String[] generalDataRows, bool norms, bool avg)
        {
            generalData.Columns.Add(String.Concat(_DATAYEAR_, " Data"));
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
                row[_DATAYEAR_ + " Data"] = r;
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
                    //row["State Avg"] = inputData.Tables[0].Rows[0]["Avg of "+ r].ToString();
                    row["State Avg"] = (inputData.Tables[0].Rows.Cast<DataRow>().Last())[r].ToString(); //assuming last row is avg
                }

                row[mla["MLAConstituency"].ToString()] = mla[r].ToString();

                foreach (var c in otherConstituencies)
                {
                    row[c["MLAConstituency"].ToString()] = c[r].ToString();
                }

                generalData.Rows.Add(row);
            }
        }
    }
}