using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Linq;

namespace Vargainc.Timm.Extentions
{
    public static class DatabaseEx
    {
        public static async Task<IList<dynamic>> SqlQuery(this Database database, string sql, params object[] parameters)
        {
            List<dynamic> result = new List<dynamic>();
            using (System.Data.IDbCommand command = database.Connection.CreateCommand())
            {
                try
                {
                    await database.Connection.OpenAsync();
                    command.CommandText = sql;
                    command.CommandTimeout = command.Connection.ConnectionTimeout;
                    foreach (var param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    List<string> columns = new List<string>();
                    using (System.Data.IDataReader reader = command.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();
                        foreach (System.Data.DataRow row in schema.Rows)
                        {
                            string name = (string)row["ColumnName"];
                            columns.Add(name);
                            
                        }
                        
                        while (reader.Read())
                        {
                            var resultRow = new ExpandoObject();
                            for (var i = 0; i < columns.Count; i++)
                            {
                                AddProperty(resultRow, columns[i], reader.GetValue(i));
                            }
                            result.Add(resultRow);
                        }
                    }
                }
                finally
                {
                    database.Connection.Close();
                    command.Parameters.Clear();
                }
            }
            return result;
        }

        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
