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

        String[] chart1Columns;

        String[] chart2Columns;


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
                    step3 = new Step3(inputData.Tables[0].Columns.Cast<DataColumn>()
                                                        .Select(p=>p.ColumnName)
                                                        .ToArray<String>());
                    loadControl(step3);
                    break;

                case "IndiaGovernsReportTool.Step3":
                    step4 = new Step4();
                    loadControl(step4);
                    break;

                case "IndiaGovernsReportTool.Step4":
                    break;
            }
        }
    }
}
