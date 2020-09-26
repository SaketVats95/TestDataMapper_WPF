﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace TestDataMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> allSheetName = null;
        DataTable currentProcessingTable;
        public MainWindow()
        {
            InitializeComponent();

            
        }
        
    
        public DataTable ReadExcelSheet(string fileName, string fileExt, string sheetName)
        {
            string conn = string.Empty;
            DataTable dtexcel = new DataTable(sheetName);
            if (fileExt.CompareTo(".xls") == 0)
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
            else
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from ["+sheetName+"]", con); //here we read data from sheet1  
                    oleAdpt.Fill(dtexcel); //fill excel data into dataTable  
                }
                catch( Exception ex)
                {
                    
                }
            }
            return dtexcel;
        }
        public void LoadExcelSheet()
        {
            string fileName = txtboxTestFileName.Text;
            FileInfo fi = new FileInfo(fileName);
            string extension = fi.Extension;
            string sheetName = GetSelectedRadioButton(stPanelSheetNames);
           DataTable dt = ReadExcelSheet(fileName, extension,sheetName);
            DataTable testTable = ChangeColumnName(dt);
            if (!object.ReferenceEquals(currentProcessingTable, null))
            {
                currentProcessingTable.Dispose();
            }
            currentProcessingTable = testTable;
            dgLoadTable.DataContext = currentProcessingTable;
            GenetateRadioButtonList(GetAllColumnNames(currentProcessingTable),stPanelColumnsName);
        }

        private List<string> GetAllColumnNames(DataTable testTable)
        {
            List<string> allColNames = new List<string>();
            foreach (DataColumn dataColumn in testTable.Columns)
            {
                allColNames.Add(dataColumn.ColumnName);
            }
            return allColNames;
        }

        private DataTable ChangeColumnName(DataTable dt)
        {
            DataTable dataTable = new DataTable(dt.TableName);
            DataRow firstRow = dt.Rows[0];
            int count = 1;
            foreach (DataColumn col in dt.Columns)
            {
                string colName = firstRow[col.ColumnName].ToString();
                dataTable.Columns.Add(colName!=""? colName : "C" + count , typeof(string));
                count++;
            }

            for (int i = 1; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                DataRow row = dataTable.NewRow();
                int colNum = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    string colValue = dr[colNum].ToString();
                    row[colNum] = colValue;
                    colNum++;
                    
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        private string GetSelectedRadioButton(StackPanel stPanelSheetNames)
        {
            UIElementCollection childElements = stPanelSheetNames.Children;
            foreach(UIElement child in childElements)
            {
                if (child.GetType() == typeof(RadioButton))
                {
                    RadioButton rb = (RadioButton)child;
                    if ((bool)rb.IsChecked)
                    {
                        return rb.Content.ToString(); 
                    }
                }
            }
            return "";
        }

        public void UpdateAllSheetNames(string fileName)
        {
            allSheetName = new List<string>();
            #region Commented
            //Excel.Application xlApp = new Excel.Application();
            //Excel.Workbook excelBook = xlApp.Workbooks.Open(fileName);
            //foreach (Excel.Worksheet wSheet in excelBook.Worksheets)
            //{
            //    allSheetName.Add(wSheet.Name);
            //}

            //excelBook.Close();
            #endregion

            string conn = string.Empty;
            DataTable dataTable = new DataTable("SchemaTable");
            FileInfo f = new FileInfo(fileName);
            string fileExt = f.Extension;
            if (fileExt.CompareTo(".xls") == 0)
                conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';"; //for below excel 2007  
            else
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO';"; //for above excel 2007  
            using (OleDbConnection con = new OleDbConnection(conn))
            {
                try
                {
                    con.Open();
                    dataTable = con.GetSchema("Tables");  
                }
                catch (Exception ex)
                {
                }
                con.Close();
            }
            foreach (DataRow dr in dataTable.Rows)
            {
                allSheetName.Add(dr["TABLE_NAME"].ToString()); 
            }

        }

        public void GenetateRadioButtonList(List<string> names, StackPanel st)
        {
            int i = 0;
            foreach(string name in names)
            {
                RadioButton rb = new RadioButton() { Content = name, IsChecked = i == 0 };
                rb.Checked += (sender, args) =>
                {
                    Console.WriteLine("Pressed " + (sender as RadioButton).Tag);
                };
                rb.Unchecked += (sender, args) => { /* Do stuff */ };
                rb.Tag = i;
                if(i == 0)
                {
                    rb.IsChecked = true;
                }
                st.Children.Add(rb);
                i++;
            }
        }
        private void BtnTestFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "Excel Files (*.xlsx)|*.xlsx|(*.xlsb)|*.xlsb";
            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                txtboxTestFileName.Text = openFileDlg.FileName;
            }

            #region OpenDilog Props
            //openFileDialog.Multiselect = true;
            //openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //if (openFileDialog.ShowDialog() == true)
            //{
            //    foreach (string filename in openFileDialog.FileNames)
            //        lbFiles.Items.Add(Path.GetFileName(filename));
            //}
            #endregion

            UpdateAllSheetNames(txtboxTestFileName.Text);
           GenetateRadioButtonList(allSheetName, stPanelSheetNames);
        }

        private void btnLoadExcelSheet_Click(object sender, RoutedEventArgs e)
        {
            LoadExcelSheet();
        }

        private void BtnProcessCol_Click(object sender, RoutedEventArgs e)
        {
            string seletedCol = GetSelectedRadioButton(stPanelColumnsName);
            List<string> uniqueValue = new List<string>();
            if (seletedCol != "")
            {
                DataTable_Processing dataTable_Processing = new DataTable_Processing();
                uniqueValue = dataTable_Processing.GetColUniqueValues(currentProcessingTable, seletedCol);
                if (uniqueValue.Count > 0)
                {
                    DataTable dt = CreateColumnMapperTable(uniqueValue, seletedCol);
                    ChildWindow chldWindow = new ChildWindow(dt);
                    chldWindow.Show();
                }
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //DataTable dt = CreateColumnMapperTable(, columnNames);
                ChildWindow chldWindow = new ChildWindow();
            //MessageBox.Show(chldWindow.Getmessage());
            //chldWindow.ShowDialog();
            chldWindow.Show();
            //var wnd = new Window();
            //wnd.Owner = this;
            //wnd.Show();


        }
        public DataTable CreateColumnMapperTable(List<string> uniqueColumnValues, string columnName)
        {
            DataTable dt = new DataTable(currentProcessingTable.TableName+"-"+columnName+"-Column");
            dt.Columns.Add("OriginalValue",typeof(string));
            dt.Columns.Add("MappingValue", typeof(string));
            foreach (string value in uniqueColumnValues)
            {
                DataRow dr = dt.NewRow();
                dr[0] = value;
                dr[1] = value;
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}