using System;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Windows.Forms;

namespace IndiaGovernsReportTool
{
    public class Utility
    {
        /// <summary>
        /// Utility method for getting a list of values selected given a listview
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String[] GetListSelectedValues(ListView list)
        {
            var selectedValues = new ArrayList();
            foreach (ListViewItem item in list.CheckedItems)
            {
                selectedValues.Add(item.Text);
            }
            return (String[])selectedValues.ToArray(typeof(String));
        }

        /// <summary>
        /// This method converts data in a xlsx Excel file into a DataSet, with each sheet data in a separate datatable
        /// </summary>
        /// <param name="fileName">Full path of the file to be read</param>
        /// <param name="hasHeaders">Whether the data has headers or not - if yes, then the first row is taken as the header names of the columns</param>
        /// <returns>Dataset with the data inside it</returns>
        public static DataSet ExcelToDataSet(string fileName, bool hasHeaders)
        {
            var ds = new DataSet();
            var hdr = hasHeaders ? "Yes" : "No";
            var strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=" + hdr + ";IMEX=1\"";
            
            using (var conn = new OleDbConnection(strConn))
            {
                conn.Open();
                var dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                foreach (DataRow row in dt.Rows)
                {
                    var sheet = row["TABLE_NAME"].ToString();
                    var cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn) {CommandType = CommandType.Text};
                    var outputTable = new System.Data.DataTable(sheet);

                    ds.Tables.Add(outputTable);
                    new OleDbDataAdapter(cmd).Fill(outputTable);

                    //Roopesh: Hack to handle decimals in the sheet
                    //in each table, if cell is number, then convert to int
                    foreach (DataRow outputRow in outputTable.Rows)
                    {
                        foreach (DataColumn column in outputTable.Columns)
                        {
                            decimal value;
                            if (Decimal.TryParse(outputRow[column.ColumnName].ToString(), out value))
                            {
                                outputRow[column.ColumnName] = Convert.ToInt32(value).ToString();
                            }
                        }
                    }

                    //this is done to revert the conversion of . to # by oldedb
                    foreach (var column in outputTable.Columns.Cast<DataColumn>())
                    {
                        //fixing some wierd problem where " " is replaced by _
                        column.ColumnName = column.ColumnName.Replace("#", ".").Replace("_", " ");
                        
                    }
                }
            }
            return ds;
        }
    }
}
