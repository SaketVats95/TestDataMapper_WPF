using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        string filePath = "";
        string sheetName = "";
        public MainWindow()
        {
            InitializeComponent();

            
        }
        
    
        public DataTable ReadExcelSheet(string fileName, string fileExt, string sheetName)
        {
            string conn = string.Empty;
            DataTable dtexcel = new DataTable(sheetName);
            if (fileExt.CompareTo(".csv") == 0)
            {

                var connString = string.Format(
                    @"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};Extended Properties=""Text;HDR=YES;FMT=Delimited""",
                    System.IO.Path.GetDirectoryName(fileName)
                );
                using (var connection = new OleDbConnection(connString))
                {
                    connection.Open();
                    var query = "SELECT * FROM [" + System.IO.Path.GetFileName(fileName) + "]";
                    using (var adapter = new OleDbDataAdapter(query, connection))
                    {
                        // var ds = new DataSet("CSV File");
                        adapter.Fill(dtexcel);
                    }
                }
            }
            else
            {
                if (fileExt.CompareTo(".xls") == 0)
                    conn = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HRD=Yes;IMEX=1';MAXSCANROWS=0;ImportMixedTypes=Text;"; //for below excel 2007  
                else
                   // conn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;MAXSCANROWS=0;ImportMixedTypes=Text';"; //for above excel 2007  
                conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;MAXSCANROWS=0;ImportMixedTypes=Text';"; //for above excel 2007  

                using (OleDbConnection con = new OleDbConnection(conn))
                {
                    try
                    {
                        OleDbDataAdapter oleAdpt = new OleDbDataAdapter("select * from [" + sheetName + "]", con); //here we read data from sheet1  
                        oleAdpt.Fill(dtexcel); //fill excel data into dataTable  
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return dtexcel;
        }
       
        public void LoadExcelSheet()
        {
            string fileName = filePath; //txtboxTestFileName.Text;
            FileInfo fi = new FileInfo(fileName);
            string extension = fi.Extension;

           // string sheetName = GetSelectedRadioButton(stPanelSheetNames);

           DataTable dt = ReadExcelSheet(fileName, extension,sheetName);
            DataTable testTable = ChangeColumnType(dt);
            dt.Dispose();
            if (!object.ReferenceEquals(currentProcessingTable, null))
            {

                currentProcessingTable.Clear();
                currentProcessingTable.Dispose();
                currentProcessingTable = null;
                GC.Collect();
            }
            currentProcessingTable = testTable;
            #region Commented Region
            //testTable.Clear();
            // currentProcessingTable = dt;
            //dgLoadTable.DataContext = currentProcessingTable;
            //DeleteAllChildElement(stPanelColumnsName);
            //GenetateRadioButtonList(GetAllColumnNames(currentProcessingTable),stPanelColumnsName);
            #endregion
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

        private DataTable ChangeColumnType(DataTable dt)
        {
            DataTable dataTable = new DataTable(dt.TableName);
            //DataRow firstRow = dt.Rows[0];
            //int count = 1;
            foreach (DataColumn col in dt.Columns)
            {
              dataTable.Columns.Add(col.ColumnName, typeof(string));
               // count++;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
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

            if (fileExt.CompareTo(".csv") == 0)
            {
                if (File.Exists(fileName))
                {
                    allSheetName.Add(f.Name);

                }

            }

            else
            {
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



                // skip those that do not end correctly
                foreach (DataRow row in dataTable.Rows)
                {
                    string sheetName = row["TABLE_NAME"].ToString();
                    if (!sheetName.EndsWith("$") && !sheetName.EndsWith("$'"))
                        continue;
                    allSheetName.Add(row["TABLE_NAME"].ToString());
                }
            }
        }

        public void GenetateRadioButtonList(List<string> names, StackPanel st)
        {
            int i = 0;
            foreach(string name in names)
            {
                RadioButton rb = new RadioButton() { Content = name, IsChecked = i == 0 };
                rb.Background = Brushes.Cyan;
                rb.FontSize = 14;
                rb.FontStyle = FontStyles.Italic;
                rb.FontWeight = FontWeights.DemiBold;
                //rb.Checked += (sender, args) =>
                //{
                //    Console.WriteLine("Pressed " + (sender as RadioButton).Tag);
                //};
                //rb.Unchecked += (sender, args) => { /* Do stuff */ };
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
         //   openFileDlg.Filter = "Excel Files (*.xlsx)|*.xlsx|(*.xlsb)|*.xlsb";
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
            if (txtboxTestFileName.Text != "")
            {
                UpdateAllSheetNames(txtboxTestFileName.Text);
                DeleteAllChildElement(stPanelSheetNames);
                GenetateRadioButtonList(allSheetName, stPanelSheetNames);
            }

            
        }
        public void Show_PleaseWaitMessage()
        {
            rectDisableWindow.Visibility = Visibility.Visible;
            txtBlockWaitMessage.Visibility = Visibility.Visible;
        }
        public void Hide_PleaseWaitMessage()
        {
            rectDisableWindow.Visibility = Visibility.Hidden;
            txtBlockWaitMessage.Visibility = Visibility.Hidden;
        }
        private async void btnLoadExcelSheet_Click(object sender, RoutedEventArgs e)
        {
            Show_PleaseWaitMessage();

            sheetName = GetSelectedRadioButton(stPanelSheetNames);
            filePath = txtboxTestFileName.Text;

            Task task = new Task(LoadExcelSheet);
            task.Start();
            await task;

            // LoadExcelSheet();

            DataTable dt2 = currentProcessingTable.Clone();
            int count = 0;
           foreach (DataRow dr in currentProcessingTable.Rows)
            {
                if (count < 100)
                    dt2.Rows.Add(dr.ItemArray);
                else
                    break;
                count++;
            }
            dgLoadTable.DataContext = dt2;
            DeleteAllChildElement(stPanelColumnsName);
            GenetateRadioButtonList(GetAllColumnNames(currentProcessingTable), stPanelColumnsName);

            mItemColumnValue.IsEnabled = true;
            mIemColumnName.IsEnabled = true;
            mIemExpressionBuilder.IsEnabled = true;
            mIemSelectFolder.IsEnabled = true;

            Hide_PleaseWaitMessage();

        }
        private void btnExecuteServerRequest_Click(object sender, RoutedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(currentProcessingTable.Copy());
            string filename = "XPathMapper.xlsx";
            string fileExt = ".xlsx";
          DataTable datatableMapper=  ReadExcelSheet(filename, fileExt, "Sheet2$");
            datatableMapper.TableName = "MapperTable";
            ds.Tables.Add(datatableMapper);
            DCTAsyncReuestHandling.RequestAddResponse rq = new DCTAsyncReuestHandling.RequestAddResponse(ds, currentProcessingTable.TableName, datatableMapper.TableName);
            rq.ProcessAllInputData();
            if (ConfigurationManager.AppSettings["WriteToExcel"].ToString() == "1")
                {
                ReadWriteExcelSheet readWriteExcelSheet = new ReadWriteExcelSheet();
                readWriteExcelSheet.WriteToExcel(ds.Tables[currentProcessingTable.TableName]);
               }
            ds.Dispose();
            GC.Collect();
            MessageBox.Show("Request Processing Completed");

        }

        private  void BtnProcessCol_Click(object sender, RoutedEventArgs e)
        {
            
            string seletedCol = GetSelectedRadioButton(stPanelColumnsName);

            #region Treading Unblock MainWindow 
            //Task task = Task.Run(() => OpenColumnMapping(seletedCol));
            //task.Start();
            //await task;
            //OpenColumnMapping(seletedCol);
            Thread newWindowThread = new Thread(()=> OpenColumnMapping(seletedCol));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();
            #endregion
        }
        public void OpenColumnMapping(string seletedCol)
        {
            List<string> uniqueValue = new List<string>();
            if (seletedCol != "")
            {
                DataTable_Processing dataTable_Processing = new DataTable_Processing();
                uniqueValue = dataTable_Processing.GetColUniqueValues(currentProcessingTable, seletedCol);

                if (uniqueValue.Count > 0)
                {

                    DataTable dt = CreateColumnMapperTable(uniqueValue, seletedCol);
                    //ChildWindow chldWindow = new ChildWindow(dt);
                    //chldWindow.Show();
                    ThreadStartingPoint(dt);
                    mIemExecuteCurrentDir.IsEnabled = true;
                   // return dt;
                }
            }
            //return new DataTable("");
        }
        private void ThreadStartingPoint(DataTable dt)
        {
            ChildWindow chldWindow = new ChildWindow(dt);
            try
            {
                
                chldWindow.Show();
                System.Windows.Threading.Dispatcher.Run();

            }
            catch (ThreadAbortException)
            {
                chldWindow.Close();
                //System.Windows.Threading.Dispatcher.InvokeShutdown();
            }
            //the CLR will "rethrow" thread abort exception automatically
        }
        public void DeleteAllChildElement(StackPanel st)
        {
            UIElementCollection childElements = st.Children;
            int count = childElements.Count;
            for (int i = 0; i<count; i++)
            {
                //if (child.GetType() == typeof(RadioButton))
                //{
                //    RadioButton rb = (RadioButton)child;
                //    if ((bool)rb.IsChecked)
                //    {
                //        
                //    }
                //}
                st.Children.RemoveAt(0);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //DataTable dt = CreateColumnMapperTable(, columnNames);
            //ChildWindow chldWindow = new ChildWindow();
            //MessageBox.Show(chldWindow.Getmessage());
            //chldWindow.ShowDialog();

            //var wnd = new Window();
            //wnd.Owner = this;
            //wnd.Show();

            MappingWindow chldWindow = new MappingWindow(currentProcessingTable);
            chldWindow.Show();

            //DataTablePreview chldWindow = new DataTablePreview(currentProcessingTable);
            //chldWindow.Show();

        }
        public DataTable CreateColumnMapperTable(List<string> uniqueColumnValues, string columnName)
        {

            DataTable dt = new DataTable(currentProcessingTable.TableName+"|"+columnName+"|Column");
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

        private void BtnPreviewDT_Click(object sender, RoutedEventArgs e)
        {
            DataTablePreview dataTablePreview = new DataTablePreview(currentProcessingTable);
            dataTablePreview.Show();
        }

        private void BtnExecuteMapping_Click(object sender, RoutedEventArgs e)
        {
            MappingWindow mappingWindow = new MappingWindow(currentProcessingTable);
            mappingWindow.Show();
        }

        private void BtnProcessColName_Click(object sender, RoutedEventArgs e)
        {
            string seletedCol = "ColumnNameList";
            List<string> uniqueValue = new List<string>();
            foreach (DataColumn dc in currentProcessingTable.Columns)
            {
                uniqueValue.Add(dc.ColumnName);
            }
            if (uniqueValue.Count > 0)
            {
                DataTable dt = CreateColumnMapperTable(uniqueValue, seletedCol);
                ChildWindow chldWindow = new ChildWindow(dt);
                chldWindow.Show();
            }

        }

        private void BtnExpressionBuilder_Click(object sender, RoutedEventArgs e)
        {
            ExpressionBuilder expressionBuilder = new ExpressionBuilder(currentProcessingTable);
            expressionBuilder.Show();
            mIemExecuteCurrentDir.IsEnabled = true;
        }

        private void BtnExecuteCurrentFolder_Click(object sender, RoutedEventArgs e)
        {
            string mappingInfoFolder = "";
            string folderName = currentProcessingTable.TableName + ChildWindow.datetimeString;
            string dirPath = mappingInfoFolder != "" ? mappingInfoFolder + "//" + folderName : folderName;
            MappingWindow map = new MappingWindow(currentProcessingTable, dirPath);
            map.Show();
        }
    }
}
