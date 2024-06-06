using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Connection
{
    public class SqlConfiguration
    {
        public SqlConfiguration(string connectionString) => ConnectionString = connectionString;

        public string ConnectionString { get; set; }
    }
}
