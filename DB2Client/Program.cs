using System;
using IBM.Data.DB2.Core;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DB2Client
{
    class Program
    {
        public static string serverIP = "172.17.0.2";
        public static string port = "50000";
        public static string database = "SAMPLE";
        public static string username = "";
        public static string password = "";

        static void Main(string[] args)
        {

            try
            {

                var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

                var secretProvider = config.Providers.First();

                if (Environment.GetEnvironmentVariable("username") == null) { if (!secretProvider.TryGet("username", out username)) throw new Exception("Could not find Secret"); } else { username = Environment.GetEnvironmentVariable("username"); }
                if (Environment.GetEnvironmentVariable("password") == null) { if (!secretProvider.TryGet("password", out password)) throw new Exception("Could not find Secret"); } else { password = Environment.GetEnvironmentVariable("password"); }


                string connString = $"Server={serverIP}:{port};Database={database};UID={username};PWD={password};";

                DB2Connection conn = new DB2Connection(connString);

                DB2Command cmd = new DB2Command("SELECT CID, INFO, HISTORY FROM DB2INST1.CUSTOMER", conn);

                conn.Open();


                DB2DataReader reader = cmd.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));

                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }
                finally
                {
                    reader.Close();
                    conn.Close();
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }


        }
    }
}
