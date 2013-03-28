using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndiaGovernsReportTool.Helpers
{
    public static class ComboBoxExtensionMethods
    {
        public static void SetValue(this ComboBox comboBox, string value)
        {
            if (String.IsNullOrEmpty(value)) return;
            comboBox.SelectedValue = value;
            
        }


        public static string GetSelection(this ComboBox comboBox)
        {
            return comboBox.SelectedValue.ToString();
        }

        public static void AddStringArrayAsDataSource(this ComboBox comboBox, String[] list)
        {
            comboBox.DataSource = list.Select(p => new {Name = p, Value = p}).ToList();
            comboBox.ValueMember = "Value";
        }
    }
}
