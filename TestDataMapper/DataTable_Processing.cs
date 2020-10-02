using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestDataMapper
{
    class DataTable_Processing
    {
        #region Properties
        //public static string dataFolderName = ConfigurationManager.AppSettings["DataFolder"] + "//Debug";
        //public static string logFileName { get; set; } = dataFolderName + @"/LogsCollection/DataTableProcessing_Log_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".csv";

        //public StreamWriter streamW = new StreamWriter(logFileName, true);
        #endregion

        #region Methods
        //public void WriteLogs(string text)
        //{
        //    try
        //    {
        //        streamW.WriteLine(text);
        //        streamW.Flush();
        //    }
        //    catch (Exception e)
        //    { }

        //}
        public void AddNewColumnAirthmenticOp(ref DataTable dt, string columnName, string expression)
        {
            DataColumn newColumn = new DataColumn(columnName);
            
            newColumn.DataType = System.Type.GetType("System.String");
            newColumn.Expression = expression;

            // Add columns to DataTable.
            dt.Columns.Add(newColumn);
        }
        public void AddNewColumn(ref DataTable dt, string columnName, string expression)
        {
            DataColumn newColumn = new DataColumn(columnName);
            newColumn.DataType = System.Type.GetType("System.String");
            newColumn.Expression = expression;
            try
            {
                // Add columns to DataTable.
                dt.Columns.Add(newColumn);
            }
            catch(Exception ex) { }
        }
        public DataTable ConvertJsonStrToDT(string jsonStr)
        {
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonStr, (typeof(DataTable)));
            return dt;
        }
        public void MappingOnDataTableColumn(ref DataTable dt,string columnName, Dictionary<string,string> mappingData)
        {
            // To Copy distinct values from col1 to a different datatable
            if(dt.Columns.Contains(columnName))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        dr[columnName] = mappingData[dr[columnName].ToString()];
                    }
                    catch (Exception ex)
                    {
                           
                    }
                }
            }
            

        }
        public List<string> GetColUniqueValues(DataTable dt, string columnName)
        {
            // To Copy distinct values from col1 to a different datatable
           //DataTable uniqueCols = dt.DefaultView.ToTable(true, columnName);
        //    uniqueCols.Columns[columnName].DataType.="string" ;
            return dt.DefaultView.ToTable(true, columnName).AsEnumerable()
                            .Select(r => r.Field<string>(columnName))
                            .ToList();

        }
        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }
        public Dictionary<string, string> GetDictinaryFromJson(string jsonData)
        {
            Dictionary<string, string> mappingDict = new Dictionary<string, string>();
            var values = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonData);
            foreach (Dictionary<string, string> dict in values)
            {
                string key = "";
                string value = "";
                int count = 0;
                foreach (KeyValuePair<string, string> keyValue in dict)
                {
                    if (count == 0)
                        //if (keyValue.Value == null)
                        //    key = "";
                        //else
                            key = keyValue.Value ?? "";// keyValue.Value;
                        else
                            value = keyValue.Value ?? "";
                    count++;
                }
                try
                {
                    if (!mappingDict.ContainsKey(key))
                        mappingDict.Add(key, value);
                }
                catch (Exception ex)
                { }
            }
            return mappingDict;
            //return values;
        }
        public static void ExecuteQueries(ref DataSet ds, string tableName, Dictionary<string, string> queries)
        {
            DataTable dt = ds.Tables[tableName];

            foreach (KeyValuePair<string, string> query in queries)
            {
                dt.Columns.Add(query.Key, typeof(double));
            }
            foreach (DataRow dr in dt.Rows)
            {
                foreach (KeyValuePair<string, string> query in queries)
                {
                    string[] exp = query.Value.Split(' ');
                    if (exp[1] == "-")
                        try
                        {
                            dr[query.Key] = Convert.ToDouble(dr[exp[0]]) - Convert.ToDouble(dr[exp[2]]);
                        }
                        catch (Exception ex)
                        {

                        }
                }
            }
        }
        public static DataView SelectFormTable()
        {
            return new DataView();
        }
        public static DataTable CreateDataTableFromDataRows(DataRow[] drs, string tableName)
        {
            DataTable dt = drs[0].Table;
            dt.TableName = tableName;
            foreach (DataRow row in drs)
            {
                dt.ImportRow(row);
            }
            return dt;
        }

        public static DataRow[] ExecuteSelectQuery(DataTable dt, string selectQuery)
        {
            return dt.Select("");
        }
        public static void RemoveDataTable(ref DataSet ds, string tableName)
        {
            if (ds.Tables.Contains(tableName) && ds.Tables.CanRemove(ds.Tables[tableName]))
                ds.Tables.Remove(tableName);
        }
        #endregion
    }
}
