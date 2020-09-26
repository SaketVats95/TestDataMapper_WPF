﻿using System;
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
        public List<string> GetColUniqueValues(DataTable dt, string columnName)
        {
            // To Copy distinct values from col1 to a different datatable
            DataTable uniqueCols = dt.DefaultView.ToTable(true, columnName);
            return uniqueCols.AsEnumerable()
                            .Select(r => r.Field<string>(columnName))
                            .ToList();

        }
        public string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
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