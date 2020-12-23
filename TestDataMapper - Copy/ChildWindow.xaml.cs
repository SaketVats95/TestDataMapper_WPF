using System;
using System.Collections.Generic;
using System.Data;
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


namespace TestDataMapper
{
    /// <summary>
    /// Interaction logic for ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {
        DataTable originalTable;
        public static string datetimeString = DateTime.Now.ToString("yyyyMMdd_HHmmssfff_");
        public ChildWindow()
        {
            InitializeComponent();
        }
        public ChildWindow(DataTable dt)
        {
            InitializeComponent();
            originalTable = dt.Copy();
            lblColumnName.Content = dt.TableName;
            dgLoadMappingTable.DataContext = dt;
            lblTotalCount.Content = "Unique Count: "+ dt.Rows.Count;

            //DataTable d = (DataTable)dgLoadMappingTable.DataContext;
        }
        public string Getmessage()
        {
            return "Hi Saket";
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            DataTable_Processing dataTable_Processing = new DataTable_Processing();
            DataTable dt = (DataTable)dgLoadMappingTable.DataContext;
            string mappingDataJsonString = dataTable_Processing.DataTableToJSONWithJSONNet(dt);
            string allInfo = (string)lblColumnName.Content;
            string folderName = allInfo.Split('|')[0]+datetimeString;
            string fileName = allInfo.Split('|')[1];
            string mappingInfoFolder = "";
            string dirPath =mappingInfoFolder!=""? mappingInfoFolder + "//" + folderName:folderName;

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string filePath = dirPath != ""? dirPath+"//"+fileName : fileName;
            
            if (File.Exists(filePath+ ".json"))
            {
                File.Delete(filePath +".json");
            }
            using (StreamWriter sw = new StreamWriter((filePath+".json")))
            {
                sw.Write(mappingDataJsonString);
            }
        }
        public void saveFile(string folderName, string fileName, string mappingInfoFolder, string jsonString)
        {
            string dirPath = mappingInfoFolder != "" ? mappingInfoFolder + "//" + folderName : folderName;

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string filePath = dirPath != "" ? dirPath + "//" + fileName : fileName;

            if (File.Exists(filePath + ".json"))
            {
                File.Delete(filePath + ".json");
            }
            using (StreamWriter sw = new StreamWriter((filePath + ".json")))
            {
                sw.Write(jsonString);
            }
        }

        private void btnResetChanges_Click(object sender, RoutedEventArgs e)
        {
            string message = "Are you Sure??? To Reset the Changes!!!!!!!!!!";
            string caption = "Confirmation";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.OK)
            {
                dgLoadMappingTable.DataContext = originalTable;
            }
            else
            {
                // Something not yet decided 
            }

        }
    }
}
