using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Utilidades
{
    public class DataTableQuery
    {
        private string ConnectionString;

        public DataTableQuery(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DataTable getDataTableQuery(string query, Dictionary<string,string> parametros)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);

            DataTable datatable = new DataTable();

            try
            {
                conn.Open();
                SqlCommand cmdIns = new SqlCommand(query, conn);

                foreach(var p in parametros)
                {
                    cmdIns.Parameters.AddWithValue(p.Key, p.Value);
                }

                cmdIns.CommandTimeout = 10000;
                SqlDataReader reader = cmdIns.ExecuteReader();
                datatable.Load(reader); 
            }
            catch(Exception exe)
            {
                Log.Error(exe, "Error al generar reporte");
            }
            finally
            {
                conn.Close();
            }

            return datatable;
        }
    }
}
