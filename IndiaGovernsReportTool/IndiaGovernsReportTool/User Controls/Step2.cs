using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace IndiaGovernsReportTool
{
    public partial class Step2 : UserControl
    {
        /// <summary>
        /// Selected values in the first listview
        /// </summary>
        public String[] group1Columns
        {
            get {
                return getListSelectedValues(listView1);
            }
        }

        /// <summary>
        /// Selected values in the second listview
        /// </summary>
        public String[] group2Columns
        {
            get
            {
                return getListSelectedValues(listView2);
            }
        }

        /// <summary>
        /// First Group name
        /// </summary>
        public String group1Name
        {
            get { return txtGroup1Name.Text; }
        }

        /// <summary>
        /// Second Group Name
        /// </summary>
        public String group2Name
        {
            get { return txtGroup2Name.Text; }
        }

        /// <summary>
        /// Private method for getting a list of values selected given a listview
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private String[] getListSelectedValues(ListView list)
        {
            ArrayList selectedValues = new ArrayList();
            foreach (ListViewItem item in list.CheckedItems)
            {
                selectedValues.Add(item.Text);
            }
            return (String[])selectedValues.ToArray(typeof(String));
        }

        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="columnNames">List of column names to be displayed for selection for both the groups</param>
        public Step2(String[] columnNames)
        {
            InitializeComponent();
            foreach (String columnName in columnNames)
            {
                listView1.Items.Add(columnName);
                listView2.Items.Add(columnName);
            }
        }
    }
}
