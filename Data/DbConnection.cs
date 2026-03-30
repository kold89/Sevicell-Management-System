using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace WpfApp1.Data
{
    class DbConnection
    {
        private readonly string connectionString = "Server=localhost;Database=SevicellDB;User Id=sa;Password=sa;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
