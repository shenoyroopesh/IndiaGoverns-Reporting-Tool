using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IndiaGovernsReportTool
{
    public partial class Step2 : UserControl
    {
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
        /// <param name="columnNames">List of column names to be displayed for selection for both the groups</param>
        public Step2(IEnumerable<string> columnNames)
        {
            InitializeComponent();
            foreach (var columnName in columnNames)
            {
                listView1.Items.Add(columnName);
                listView2.Items.Add(columnName);
            }
        }
    }
}
