using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IndiaGovernsReportTool.Helpers;
using IndiaGovernsReportTool.Properties;

namespace IndiaGovernsReportTool
{
    public partial class Step2 : UserControl, ISaveSettings
    {
        private const string Step2Columns1 = "Step2Columns1";
        private const string Step2Columns2 = "Step2Columns2";
        private const string Step2Group1 = "Step2Group1";
        private const string Step2Group2 = "Step2Group2";

        /// <summary>
        /// Selected values in the first listview
        /// </summary>
        public String[] Group1Columns
        {
            get {
                return Utility.GetListSelectedValues(listView1);
            }
        }

        /// <summary>
        /// Selected values in the second listview
        /// </summary>
        public String[] Group2Columns
        {
            get
            {
                return Utility.GetListSelectedValues(listView2);
            }
        }

        /// <summary>
        /// First Group name
        /// </summary>
        public String Group1Name
        {
            get { return txtGroup1Name.Text; }
        }

        /// <summary>
        /// Second Group Name
        /// </summary>
        public String Group2Name
        {
            get { return txtGroup2Name.Text; }
        }

        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="settings"> </param>
        public Step2()
        {
            InitializeComponent();
        }

        public void LoadSettings(Settings settings)
        {
            foreach (var columnName in settings.AllPossibleColumns)
            {
                listView1.Items.Add(columnName);
                listView2.Items.Add(columnName);
            }
            listView1.CheckItemsWithText(settings.Group1Columns);
            listView2.CheckItemsWithText(settings.Group2Columns);
        }

        public void SaveSettings(Settings settings)
        {
            settings.Group1Columns = Group1Columns;
            settings.Group2Columns = Group2Columns;
            settings.Group1Name = Group1Name;
            settings.Group2Name = Group2Name;
        }
    }
}
