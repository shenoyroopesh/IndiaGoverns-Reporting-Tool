using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
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
        DataSet _inputData;

        String _group1Name;
        
        String _group2Name;

        String[] _group1Columns;

        String[] _group2Columns;

        String _chart1Column1;

        String _chart1Column2;

        String _chart2Column1;
        String _chart2Column2;

        String _rank1Column;
        String _rank2Column;
        String _rank3Column;

        String[] _commentColumns;

        bool _singleReport;

        public string Datayear = "2011-12";

        readonly ArrayList _reports = new ArrayList();
        int _reportCounter = 0;

        const string SuperscriptDigits =
            "\u2070\u00b9\u00b2\u00b3\u2074\u2075\u2076\u2077\u2078\u2079";

        /// <summary>
        /// Constructor for ReportGenerator
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            LoadControl(new Step1());
        }

        /// <summary>
        /// Loads the particular usercontrol into the form panel
        /// </summary>
        /// <param name="uc"></param>
        private void LoadControl(Control uc)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(uc);
        }

        Step1 _step1;
        Step2 _step2;
        Step3 _step3;
        Step4 _step4;
        Step5 _step5;
        Step6 _step6;

        /// <summary>
        /// Controls the flow of the application, depending on the current step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNextClick(object sender, EventArgs e)
        {

            var control = (UserControl)panel1.Controls[0];
            //current state is determined by what control is present in the panel
            switch (control.GetType().ToString())
            {
                case "IndiaGovernsReportTool.Step1":
                    _step1 = (Step1)control;
                    if (_step1.Data != null)
                    {
                        _inputData = _step1.Data;
                        Datayear = _step1.DataYear;

                        _step2 = new Step2(_inputData.Tables[0].Columns.Cast<DataColumn>()
                                                        .Select(p=>p.ColumnName)
                                                        .ToArray<String>());
                        LoadControl(_step2);
                    }
                    break;

                case "IndiaGovernsReportTool.Step2":
                    _step2 = (Step2)control;
                    _group1Name = _step2.Group1Name;
                    _group2Name = _step2.Group2Name;
                    _group1Columns = _step2.Group1Columns;
                    _group2Columns = _step2.Group2Columns;
                    //only the group columns can be used in charts
                    _step3 = new Step3(_group1Columns, _group1Columns.Concat(_group2Columns).ToArray());
                    LoadControl(_step3);
                    break;

                case "IndiaGovernsReportTool.Step3":
                    _step3 = (Step3)control;
                    _chart1Column1 = _step3.Chart1Column1;
                    _chart1Column2 = _step3.Chart1Column2;
                    _chart2Column1 = _step3.Chart2Column1;
                    _chart2Column2 = _step3.Chart2Column2;
                    _step4 = new Step4(_group1Columns.Concat(_group2Columns).ToArray());
                    LoadControl(_step4);
                    break;

                case "IndiaGovernsReportTool.Step4":
                    _step4 = (Step4)control;
                    _rank1Column = _step4.Rank1Column;
                    _rank2Column = _step4.Rank2Column;
                    _rank3Column = _step4.Rank3Column;
                    
                    _step5 = new Step5(_group1Columns.Concat(_group2Columns).ToArray());
                    LoadControl(_step5);
                    break;

                case "IndiaGovernsReportTool.Step5":
                    _step5 = (Step5)control;
                    _commentColumns = _step5.CommentColumns;
                    _singleReport = _step5.SingleReport;

                    GenerateReports();
                    _step6 = new Step6();
                    LoadControl(_step6);
                    break;
            }
        }

        private void BtnBackClick(object sender, EventArgs e)
        {
            var control = (UserControl)panel1.Controls[0];
            //current state is determined by what control is present in the panel
            switch (control.GetType().ToString())
            {
                case "IndiaGovernsReportTool.Step1":
                    break;

                case "IndiaGovernsReportTool.Step2":
                    LoadControl(_step1);
                    break;

                case "IndiaGovernsReportTool.Step3":
                    LoadControl(_step2);
                    break;

                case "IndiaGovernsReportTool.Step4":
                    LoadControl(_step3);
                    break;

                case "IndiaGovernsReportTool.Step5":
                    LoadControl(_step4);
                    break;

                case "IndiaGovernsReportTool.Step6":
                    LoadControl(_step5);
                    break;
            }
        }

        private void GenerateReports()
        {
            //calculate all the averages add avg columns

            var avgCols = new Dictionary<string, int>();
        
            foreach (var col in _inputData.Tables[0].Columns.Cast<DataColumn>())
            {
                //if avg column already exists, don't calculate again
                var avgCount = _inputData.Tables[0].Columns.Cast<DataColumn>().Count(p => p.ColumnName == "Avg of " + col.ColumnName);

                if (avgCount > 0) continue;

                var colAvg = 0;
                try
                {
                    colAvg = Convert.ToInt32(_inputData.Tables[0].Rows.Cast<DataRow>()
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
                _inputData.Tables[0].Columns.Add(col.Key.ToString(), typeof(Int32), col.Value.ToString());
        
        
            //logic is - first break into mp constituencies, then break into mla constituencies
            //get distinct mpConstituencies
        
            var mpConstituencies = _inputData.Tables[0].Rows.Cast<DataRow>()
                                        .Select(p => p["MpConstituency"].ToString())
                                        .Distinct<String>();

            //loop for each mpConstituency
            foreach (var mpc in mpConstituencies)
            {
                //do not process the first row, which has only averages data
                if (mpc == "State Avg")
                    continue;

                string mpc1 = mpc;
                var mlaConstituencies = _inputData.Tables[0].Rows.Cast<DataRow>()
                                            .Where(p => p["MPConstituency"].ToString() == mpc1.ToString());

                //generate a separate report for each mla constinuency
                var constituencies = mlaConstituencies as List<DataRow> ?? mlaConstituencies.ToList();
                foreach(var mla in constituencies)
                {
                    //use only 3 other constituencies for the sake of saving space
                    var mla1 = mla;
                    var otherConstituencies = constituencies.Where(p => p["MLAConstituency"] != mla1["MLAConstituency"]).Take(3);

                    const string intro = "Education is a fundamental right for all children from the ages of " +
                                         "6-14 years (up to Class 8.) The indicators in this report help MLAs " +
                                         "and citizens track the status of education indicators in their MLA " +
                                         "constituency. The report helps compare the constituency status with " +
                                         "respect to state average and neighbouring constituencies.\n\n" +
                                         "Can the MLA use this government data to demand more resources " +
                                         "for the constituency? Can citizens ask the MLA what specifically can " +
                                         "be done to improve education status in the constituency?";

                    #region commented out for education report
                    //var intro = "How is " + mla["MLAConstituency"] + " MLA Constituency performing on important MNREGA" +
                    //                " indicators? How does it compare with some other constituencies " +
                    //                "within the " + mla["MPConstituency"] + " MP constituency? Do these numbers collated by " +
                    //                "government reflect the actual situation of " + mla["MLAConstituency"] + " constituency? \n\n" +
                    //                "What role can the MLA play in highlighting these issues with the " +
                    //                "government? Can the MLA make sure there is tangible improvement " +
                    //                "in MNREGA implementation in the constituency?";
                    #endregion


                    //use % column only if required
                    var generalDataRows = (_inputData.Tables[0].Columns.Contains("% Population Data available for")) ?
                            new String[] { "Total constituency Population", "% Population Data available for" } : 
                            new String[] { "Total constituency Population" };

                    var dataRows = otherConstituencies as DataRow[] ?? otherConstituencies.ToArray();
                    var generalData = FillTable(mla, dataRows, generalDataRows, false, false);


                    //Table2: First Group
                    var group1Data = FillTable(mla, dataRows, _group1Columns, false, true);

                    //Table3: Second Group
                    var group2Data = FillTable(mla, dataRows, _group2Columns, false, true);

                    var comment = String.Empty;
                    //comment logic - figure out which attribute to comment on.

                    var mlaToBecompared = "";
                    float maxDifference = 0;
                    var columnToBeCompared = "";


                    try
                    {
                        comment = mla["Comment"].ToString();
                    }
                    catch (Exception)
                    {
                        //do nothing just continue
                    }
                    
                    if(String.IsNullOrEmpty(comment))
                    {
                        foreach (var column in _commentColumns)
                        {
                            try
                            {
                                var maxValue = dataRows.ToList().Select(p => float.Parse(p[column].ToString())).Max();
                                var difference = (maxValue - float.Parse(mla[column].ToString())) * 100 / maxValue; //percentage deviation of current mla from max mla

                                if (!(difference > maxDifference)) continue;
                                maxDifference = difference;
                                mlaToBecompared = dataRows.ToList().First(p => float.Parse(p[column].ToString()) == maxValue)["MLAConstituency"].ToString();
                                columnToBeCompared = column;
                            }
                            catch (Exception e)
                            {
                                continue;
                            }
                        }

                        //Generate the comment
                        comment = "The above data shows that " + columnToBeCompared.Replace(Datayear, "") + " in " + mla["MLAConstituency"].ToString() + " constituency " +
                            "is lower than in " + mlaToBecompared + ". \n\nIs this data correct? " +
                            "Can this be brought to the government's notice?";
                    }

                    //TODO: note - depending on desc or ascending - need to make this generic
                    var rank1 = constituencies.Count(p => Convert.ToDouble(p[_rank1Column]) > 
                                                             Convert.ToDouble(mla[_rank1Column])) + 1;

                    var rank2 = constituencies.Count(p => Convert.ToDouble(p[_rank2Column]) >
                                                             Convert.ToDouble(mla[_rank2Column])) + 1;

                    var rank3 = constituencies.Count(p => Convert.ToDouble(p[_rank3Column]) >
                                                             Convert.ToDouble(mla[_rank3Column])) + 1;

                    var rank = mla["MLAConstituency"].ToString() + " MLA Constituency Rank\n" +
                        "among " + constituencies.Count().ToString() + " MLA Constituencies in the " +
                        mpc.ToString() + " MP Constituency. \n\n" +
                        "Rank " + rank1 + " in the " + _rank1Column.Replace(Datayear, "") +
                        //hardcoding this replace below, no other way to do this 
                        "\nRank " + rank2 + " in the " + _rank2Column.Replace(Datayear, "").Replace("% Schools with Water Facility Govt.", "% Govt. schools with Water facility") +
                        "\nRank " + rank3 + " in the " + _rank3Column.Replace(Datayear, "");

                    _reports.Add(new Report
                    {
                        ReportName = mla["MLAConstituency"].ToString(),
                        DataYear = this.Datayear,
                        GeneralData = generalData,
                        Group1Data = group1Data,
                        Group2Data = group2Data,
                        Group1Name = _group1Name,
                        Group2Name = _group2Name,
                        Intro = intro,
                        Comment = comment,
                        Chart1Column1 = _chart1Column1,
                        Chart1Column2 = _chart1Column2,
                        Chart2Column1 = _chart2Column1,
                        Chart2Column2 = _chart2Column2,
                        Rank = rank
                    });

                    if(_singleReport)
                        break;
                }

                if(_singleReport)
                    break;
            }
            PublishNextReport();
        }

        private void PublishNextReport()
        {
            //exit condition
            if (_reportCounter == _reports.Count) return;
            
            var report = (Report)_reports[_reportCounter];
            _reportCounter += 1;
            var reportform = new ReportForm(report);
            //for next report
            reportform.FormClosed += ReportformFormClosed;
            reportform.Show();
        }

        void ReportformFormClosed(object sender, FormClosedEventArgs e)
        {
            ((ReportForm)sender).FormClosed -= ReportformFormClosed;
            PublishNextReport();
        }

        private String ConvertToPercentage(Decimal number)
        {
            return Convert.ToInt32(number * 100).ToString() + "%";
        }

        private DataTable FillTable(DataRow mla, IEnumerable<DataRow> otherConstituencies, 
            IEnumerable<string> generalDataRows, bool norms, bool avg)
        {
            var generalData = new DataTable();
            generalData.Columns.Add(String.Concat(Datayear, " Data"));
            if (norms) generalData.Columns.Add("Norms");
            if (avg) generalData.Columns.Add("State Avg");
            generalData.Columns.Add(mla["MLAConstituency"].ToString());

            foreach (var c in otherConstituencies)
            {
                generalData.Columns.Add(c["MLAConstituency"].ToString());
            }

            foreach (var r in generalDataRows)
            {
                var row = generalData.NewRow();
                row[Datayear + " Data"] = r;
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
                    row["State Avg"] = (_inputData.Tables[0].Rows.Cast<DataRow>().First())[r].ToString(); //assuming last row is avg
                }

                row[mla["MLAConstituency"].ToString()] = mla[r].ToString();

                foreach (var c in otherConstituencies)
                {
                    row[c["MLAConstituency"].ToString()] = c[r].ToString();
                }

                generalData.Rows.Add(row);
            }
            return generalData;
        }
    }
}