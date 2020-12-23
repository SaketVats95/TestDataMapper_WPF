using DCTAsyncReuestHandling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Xml;
using Syncfusion.Windows.Edit;
using System.Xaml;

namespace TestDataMapper
{
    /// <summary>
    /// Interaction logic for SingleRequestWindow.xaml
    /// </summary>
    public partial class SingleRequestWindow : Window
    {
        public DataSet dataSet_InputProcess { get; set; }
        public string mainTableName { get; set; }
        public string mappingTableName { get; set; }
        public int totalReqCount { get; set; } = 0;
        public string SingleRequestName { get; set; }
        public string ExampleServer { get; set; }
        public string DataFileName { get; set; }
        public string OutputCsvFileName { get; set; }
        public string ReqRes { get; set; }
        public string RequestSession { get; set; }
        public int RowNumber { get; set; }
       public string singleReq { get; set; }
        public string singleRes { get; set; }

       
        public SingleRequestWindow()
        {
            InitializeComponent();


            // Add paragraphs to the FlowDocument.
            

        }
        public SingleRequestWindow(DataSet ds, string mainTable, string mappingTable, string requestType = "", string rowNumber = "")
        {
            InitializeComponent();
            RowNumber = rowNumber != "" ? int.Parse(rowNumber) : int.Parse(ConfigurationManager.AppSettings["RowNumber"].ToString());

            SingleRequestName = requestType != "" ? requestType : ConfigurationManager.AppSettings["SingleRequestName"].ToString();
            ExampleServer = ConfigurationManager.AppSettings["ExampleServer"].ToString();
            //DataFileName = ConfigurationManager.AppSettings["DataFileName"].ToString();
            OutputCsvFileName = ConfigurationManager.AppSettings["OutputCsvFileName"].ToString();
            ReqRes = ConfigurationManager.AppSettings["ReqRes"].ToString();
            RequestSession = ConfigurationManager.AppSettings["RequestSession"].ToString();
            dataSet_InputProcess = ds;
            mainTableName = mainTable;
            mappingTableName = mappingTable;
            cmbInsurer.Items.Add("NRMA");
            cmbInsurer.Items.Add("GIO");
            cmbInsurer.Items.Add("AAMI");
            cmbInsurer.Items.Add("Allianz");
            cmbInsurer.Items.Add("YOUI");
            cmbInsurer.SelectedItem = ConfigurationManager.AppSettings["insurerName"].ToString();
        }
        public void ProcessSingleInputData()
        {
            XmlDocument req = new XmlDocument();
            totalReqCount = dataSet_InputProcess.Tables[mainTableName].Rows.Count;
           
          
            req.Load(RequestSession);
            totalReqCount = 1;
           
            SingleRequest(RowNumber, req);
            
           
            // datagrid1.ItemsSource = singleReq;
        }
        protected string FormatXml(XmlNode xmlNode)
        {
            StringBuilder bob = new StringBuilder();


            // We will use stringWriter to push the formated xml into our StringBuilder bob.
            using (StringWriter stringWriter = new StringWriter(bob))
            {
                // We will use the Formatting of our xmlTextWriter to provide our indentation.
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    xmlNode.WriteTo(xmlTextWriter);
                }
            }


            return bob.ToString();
        }

        private bool SingleRequest(int rowNum, XmlDocument req)
        {
             //WriteLogs(rowNum.ToString());
             singleReq = CreateServerRequest(req, rowNum);
            XmlDocument reqdoc = new XmlDocument();
            reqdoc.LoadXml(singleReq);
         
            string request= FormatXml(reqdoc);
         
            txtRequestblock.Text = request;
             singleRes = ProcessRequest(singleReq, ExampleServer);
            XmlDocument resdoc = new XmlDocument();
            resdoc.LoadXml(singleRes);
        //    lbl12months.Content = resdoc.SelectSingleNode("server/responses/Session.getDocumentRs/" + "session/data/policy/line/Comparer/YOUI/YOUI12MonthsPremium/Premium").InnerText;
          
            string response = FormatXml(resdoc);
            txtResponseBlock.Text = response;
            return true;
        }
        private string ProcessRequest(string xml, string serverUrl)
        {
            string str = string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            string serverPath = ExampleServer;
            WebRequest request = WebRequest.Create(serverPath);
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.ContentLength = bytes.Length;
            int serverTimeout = int.MaxValue;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerTimeout"]))
                if (!int.TryParse(ConfigurationManager.AppSettings["ServerTimeout"], out serverTimeout))
                    serverTimeout = int.MaxValue;


            request.Timeout = serverTimeout;


            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream2 = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream2))
                    {
                        str = reader.ReadToEnd();
                        reader.Close();
                        stream2.Close();
                    }
                }
                response.Close();
            }
            //if (ConfigurationManager.AppSettings["DebugCode"].ToString() == "1")
            //WriteLogs("Response :: " + str);
            return str;
        }
        private string CreateServerRequest(XmlDocument req, int rowNum)
        {
            //   string requestXPathHeader = ConfigurationManager.AppSettings["RequestXPathHeader"].ToString();
            string requestXPathHeader = "server/requests/Session.setDocumentRq/";
            lock (dataSet_InputProcess)
            {
                try
                {

                    EvaluateExpression obj_evalE = new EvaluateExpression();


                    Dictionary<string, string> allInputCalculation = new Dictionary<string, string>();
                    foreach (DataRow dr in dataSet_InputProcess.Tables[mappingTableName].Rows)
                    {
                        if (dr["InputOutputType"].ToString() == "Input")
                        {
                            allInputCalculation.Add(dr["FieldName"].ToString(), dataSet_InputProcess.Tables[mainTableName].Rows[rowNum][dr["FieldName"].ToString()].ToString());
                        }
                    
                    }





                    foreach (DataRow dr in dataSet_InputProcess.Tables[mappingTableName].Rows)
                    {
                        if ((dr["InputOutputType"].ToString() == "Input" || dr["InputOutputType"].ToString() == "CalculationInput") && (dr["XPath"].ToString() != ""))
                        {

                            req.SelectSingleNode(requestXPathHeader + dr["XPath"]).InnerText = dataSet_InputProcess.Tables[mainTableName].Rows[rowNum][dr["FieldName"].ToString()].ToString();
                        }
                    }
                    req.SelectSingleNode("server/requests/Session.setDocumentRq/session/data/policy/RequestNumber").InnerText = rowNum.ToString();

                }
                catch (Exception ex)
                {
                    
                }
            }
            //WriteLogs(req.OuterXml);
            //WriteLogs(rowNum.ToString());
            return req.OuterXml;
        }

        private void btnSingleRequest_Click(object sender, RoutedEventArgs e)
        {
            singleReq = txtRequestblock.Text;
            singleRes = ProcessRequest(singleReq, ExampleServer);
            XmlDocument resdoc = new XmlDocument();
            resdoc.LoadXml(singleRes);
            string response = FormatXml(resdoc);
            txtResponseBlock.Text = response;
        }
    }
}
