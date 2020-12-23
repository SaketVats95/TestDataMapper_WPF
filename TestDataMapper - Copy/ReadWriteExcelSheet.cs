using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataMapper
{
    class ReadWriteExcelSheet
    {

        #region Methods
        public void ExportDataSetToExcel(DataSet ds, string fileName)
        {

            // Creating an instance 
            // of ExcelPackage 
            ExcelPackage excel = new ExcelPackage();

            // name of the sheet 
            foreach (DataTable table in ds.Tables)
            {
                try
                {
                    //WriteLogs(dr.Key.ToString());
                    //Add a new worksheet to workbook with the Datatable name
                    var workSheet = excel.Workbook.Worksheets.Add(table.TableName);
                    // setting the properties 
                    // of the work sheet  
                    workSheet.TabColor = System.Drawing.Color.Black;
                    workSheet.DefaultRowHeight = 12;

                    // Setting the properties 
                    // of the first row 
                    //workSheet.Row(1).Height = 20;
                    //workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //workSheet.Row(1).Style.Font.Bold = true;

                    for (int i = 1; i < table.Columns.Count + 1; i++)
                    {
                        workSheet.Cells[1, i].Value = table.Columns[i - 1].ColumnName;
                    }

                    for (int j = 0; j < table.Rows.Count; j++)
                    {
                        for (int k = 0; k < table.Columns.Count; k++)
                        {
                            try
                            {
                                workSheet.Cells[j + 2, k + 1].Value = table.Rows[j].ItemArray[k].ToString();
                            }
                            catch (Exception e)
                            {
                                //WriteLogs(e.Message + e.StackTrace);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    //AppendText(e.Message + e.StackTrace);
                }
            }

            // file name with .xlsx extension  
            string p_strPath = fileName;

            if (File.Exists(p_strPath))
                File.Delete(p_strPath);

            // Create excel file on physical disk  
            FileStream objFileStrm = File.Create(p_strPath);
            objFileStrm.Close();

            // Write content to excel file  
            File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
            //Close Excel package 
            excel.Dispose();

        }

        public void ExportDataSetToCSV(DataSet ds)
        {
            foreach (DataTable dt in ds.Tables)
            {
                WriteToCSV(dt);
            }
        }
        public void WriteToCSV(DataTable dt, string fileName = "")
        {
            if (fileName == "")
                fileName = dt.TableName + ".csv";
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join("|", columnNames));


            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join("|", fields));
            }
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllText(fileName, sb.ToString());
        }
        public void WriteToExcel(DataTable table, string fileName = "")
        {
            ExcelPackage excel = new ExcelPackage();
           
            //Add a new worksheet to workbook with the Datatable name
            var workSheet = excel.Workbook.Worksheets.Add(table.TableName.Replace("'", "").Trim());
            //excelWorkSheet.Name = dr.Key.ToString();
            // setting the properties 
            // of the work sheet  
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            for (int i = 1; i < table.Columns.Count + 1; i++)
            {
                workSheet.Cells[1, i].Value = table.Columns[i - 1].ColumnName;
            }

            for (int j = 0; j < table.Rows.Count; j++)
            {
                for (int k = 0; k < table.Columns.Count; k++)
                {
                    try
                    {
                        workSheet.Cells[j + 2, k + 1].Style.Numberformat.Format = "@";
                        workSheet.Cells[j + 2, k + 1].Value = table.Rows[j].ItemArray[k].ToString();
                    }
                    catch (Exception e)
                    {
                        //WriteLogs(e.Message + e.StackTrace);
                    }
                }
            }
            if (fileName == "") fileName = table.TableName + ".xlsx";
            string p_strPath = fileName;

            if (File.Exists(p_strPath))
                File.Delete(p_strPath);

            // Create excel file on physical disk  
            FileStream objFileStrm = File.Create(p_strPath);
            objFileStrm.Close();

            // Write content to excel file  
            File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
            //Close Excel package 
            excel.Dispose();
        }
       
       
        #endregion
    }

}
