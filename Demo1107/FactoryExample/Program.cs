using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration; // requires reference update
using System.Data.Common;
using System.Data.SqlClient;
using static System.Console;

namespace FactoryExample
{
    class Program
    {
        static void Main(string[] args)
        {
            #region get config info
            string dataProvider = ConfigurationManager.AppSettings["provider"];
            WriteLine("\tprovider: {0}", dataProvider);

            // alternate see p. 833
            var cnStringBuilder = new SqlConnectionStringBuilder
            {
                InitialCatalog = "Northwind",
                DataSource = @"(localdb)\mssqllocaldb",
                ConnectTimeout = 30,
                IntegratedSecurity = true
            };

            WriteLine($"\tBuilt Connection String: " +
                $"{cnStringBuilder.ConnectionString}\n");
            #endregion

            // get factory
            DbProviderFactory factory =
                DbProviderFactories.GetFactory(dataProvider);

            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection == null)
                {
                    WriteLine("There was an issue creating the connection");
                    return;
                }
                else WriteLine("-> Connection created");

                connection.ConnectionString = cnStringBuilder.ConnectionString;
                connection.Open();

                DbCommand myCommand = factory.CreateCommand();
                if (myCommand == null)
                {
                    WriteLine("There is an issue creating the command");
                    return;
                }
                else WriteLine($"Your command object is a " +
                    $"{myCommand.GetType().Name}");

                myCommand.Connection = connection;
                myCommand.CommandText = "Select * from Shippers";

                using (DbDataReader dataReader = myCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        WriteLine($"-> shipper # {dataReader["ShipperId"]} " +
                            $"name is a {dataReader[1]} phone: {dataReader[2]}");
                    }
                }
            }
        }
    }
}
