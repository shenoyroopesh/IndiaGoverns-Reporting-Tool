using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndiaGovernsReportTool
{
    public partial class Step5 : UserControl
    {
        public String[] CommentColumns 
        {
            get {
                return Utility.getListSelectedValues(listView1);
            }
        }

        public Step5(String[] possibleColumns)
        {
            InitializeComponent();
            possibleColumns
                .ToList<String>()
                .ForEach(p => this.listView1.Items.Add(p));

            listView1.Items.Cast<ListViewItem>().ToList().ForEach(p => p.Selected = true);
        }
    }
}
