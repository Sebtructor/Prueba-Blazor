using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Utilidades
{
    public class DataTableUtility
    {
        private static DataTable CreateDataTableForPropertiesOfType<T>()
        {
            DataTable dt = new DataTable();
            PropertyInfo[] piT = typeof(T).GetProperties();

            foreach (PropertyInfo pi in piT)
            {
                Type propertyType = null;
                if (pi.PropertyType.IsGenericType)
                {
                    propertyType = pi.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    propertyType = pi.PropertyType;
                }
                DataColumn dc = new DataColumn(pi.Name, propertyType);

                if (pi.CanRead)
                {
                    dt.Columns.Add(dc);
                }
            }

            return dt;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            var table = CreateDataTableForPropertiesOfType<T>();
            PropertyInfo[] piT = typeof(T).GetProperties();

            foreach (var item in items)
            {
                var dr = table.NewRow();

                for (int property = 0; property < table.Columns.Count; property++)
                {
                    if (piT[property].CanRead)
                    {
                        dr[property] = piT[property].GetValue(item, null);
                    }
                }

                table.Rows.Add(dr);
            }
            return table;
        }
    }
}
