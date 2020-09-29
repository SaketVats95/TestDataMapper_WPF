using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Forms;
using Ookii.Dialogs.Wpf;
using System.Data;

namespace TestDataMapper
{
    /// <summary>
    /// Interaction logic for MappingWindow.xaml
    /// </summary>
    public partial class MappingWindow : Window
    {
        List<string> allfiles = new List<string>();
        DataTable dataTable = new DataTable();
        DataTable backupTable;

        public MappingWindow()
        {
            InitializeComponent();
        }
        public MappingWindow(DataTable dt)
        {
            InitializeComponent();
            dataTable = dt;

        }
        public List<string> GetAllFiles(string dirPath)
        {
            List<string> files = new List<string>();
            foreach (string fName in Directory.GetFiles(dirPath))
            {
                FileInfo f = new FileInfo(fName);
                if (f.Extension.ToLower() == ".json")
                {
                    files.Add(f.FullName);
                }
            }
            allfiles = files;
            return files;
        }

        private void btnMappingFolder_Click(object sender, RoutedEventArgs e)
        {
           
            var ookiiDialog = new VistaFolderBrowserDialog();
            if (ookiiDialog.ShowDialog() == true)
            {
                //MessageBox.Show(ookiiDialog.SelectedPath);
                txtboxMappingFolderPath.Text = ookiiDialog.SelectedPath;
            }
            if (txtboxMappingFolderPath.Text!="") {
                List<string> filesPath = GetAllFiles(txtboxMappingFolderPath.Text);
                foreach (string f in filesPath)
                {
                    FileInfo file = new FileInfo(f);
                   wdPanelMapColList.Children.Add(addLabelWithClickEvent(file.Name));
                }
            }
           
        }
        private void btnSavetoExcelSheet_Click(object sender, RoutedEventArgs e)
        {
            if (dataTable != null)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Book1"; // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    string filename = dlg.FileName;
                    ReadWriteExcelSheet readWriteExcelSheet = new ReadWriteExcelSheet();
                    readWriteExcelSheet.WriteToExcel(dataTable, filename);
                }
            }
        }

        public Label addLabelWithClickEvent(string content)
        {
            Label l = new Label();
            l.Content = content;
            l.Margin = new Thickness {Top = 10, Bottom= 10, Left = 10, Right = 10 };
            l.Height = 50;
            l.Background = Brushes.LightSeaGreen;
            l.HorizontalContentAlignment = HorizontalAlignment.Center;
            l.VerticalContentAlignment = VerticalAlignment.Center;
            l.FontSize = 18;
            l.FontWeight = FontWeights.Bold;
            l.FontFamily = new FontFamily("SimSun");
            l.MouseDoubleClick += MyButton_DoubleClick; 
            //l.MouseDown
            return l;

        }
        public void MyButton_DoubleClick(object sender, RoutedEventArgs e)
        {
            Label l = (Label)sender;
            //MessageBox.Show(l.Content.ToString());
            string jsonData = ReadFileDataContent(txtboxMappingFolderPath.Text + "\\" + l.Content.ToString());
            if (jsonData != "")
            {
                DataTable_Processing dataTable_Processing = new DataTable_Processing();
                DataTable dt = dataTable_Processing.ConvertJsonStrToDT(jsonData);
                ChildWindow chldWindow = new ChildWindow(dt);
                chldWindow.Show();
            }
        }

        public void btnProcessMapping_Click(object sender, RoutedEventArgs e)
        {
            backupTable = dataTable.Copy();
            foreach (string f in allfiles)
            {
                Dictionary<string, string> mappingDataDic = ReadMappingFile(f);
                if (mappingDataDic.Count > 0)
                {
                    DataTable_Processing dataTable_Processing = new DataTable_Processing();

                    dataTable_Processing.MappingOnDataTableColumn(ref dataTable, System.IO.Path.GetFileNameWithoutExtension(f), mappingDataDic);
                }
            }
            
        }

        public Dictionary<string,string> ReadMappingFile(string filePath)
        {
            Dictionary < string, string> mappingDataDict= new Dictionary<string, string>();
            string jsonData = ReadFileDataContent(filePath);
            if(jsonData != "")
                {
                DataTable_Processing dataTable_Processing = new DataTable_Processing();
                 mappingDataDict = dataTable_Processing.GetDictinaryFromJson(jsonData);
            }
            return mappingDataDict;
        }
        public string ReadFileDataContent(string filePath)
        {
            string jsonData = "";
            using (StreamReader sr = new StreamReader(filePath))
            {
                jsonData = sr.ReadToEnd();
            }
            return jsonData;
        }

        private void BtnPreviewDT_Click(object sender, RoutedEventArgs e)
        {
            DataTablePreview dataTablePreview = new DataTablePreview(dataTable);
            dataTablePreview.Show();
        }

        private void BtnColNameMapping_Click(object sender, RoutedEventArgs e)
        {
            backupTable = dataTable.Copy();
            string fileName = "ColumnNameList.json";
                Dictionary<string, string> mappingDataDic = ReadMappingFile(txtboxMappingFolderPath.Text + "\\" +fileName);
                if (mappingDataDic.Count > 0)
                {
                foreach (KeyValuePair<string,string> keyValuePair in mappingDataDic)
                {
                    try
                    {
                        dataTable.Columns[keyValuePair.Key].ColumnName = keyValuePair.Value;
                     }
                    catch(Exception ex)
                    {

                    }
                }
                }
            
        }
    }
}
