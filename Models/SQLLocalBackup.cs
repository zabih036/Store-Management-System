using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.IO;

namespace GeneralSalesDb.Models
{

    public class SQLLocalBackup
    {
        private readonly IConfiguration configuration;
        protected SqlConnection _conn;
        protected string _address;
        protected string _user;
        protected string _pass;
        protected string _dbname = "GeneralSalesDb";
        public SQLLocalBackup()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            configuration = builder.Build();

            try
            {
                _conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
                if (_conn.State != System.Data.ConnectionState.Open)
                    _conn.Open();

            }
           catch(Exception)
            {

                _conn.Close();
                SqlConnection.ClearAllPools();
            }

        }
        /// <summary>
        /// /// This function checks for temporary tables since we don't want to interfere with other programs/functions
        /// </summary>
        /// <returns></returns>
        protected string findUniqueTemporaryTableName()
        {
            string name = "GeneralSalesDb";
            int counter = 0;
            string sql = "";
            SqlCommand _mycommand = new SqlCommand();
            _mycommand.Connection = _conn;
            while (true)
            {
                ++counter;
                sql = String.Format("SELECT OBJECT_ID('tempdb..##{0}') as xyz", name + counter.ToString());
                _mycommand.CommandText = sql;
                if (_mycommand.ExecuteScalar().ToString() == "")
                {
                    return name + counter.ToString();
                }
            }


        }
        /// <summary>
        /// Main backuping function
        /// </summary>
        /// <param name="AremoteTempPath">You can specify what folder do you wish to be set for your backup</param>
        /// <param name="AlocalPath">Local path where copy of your backup file</param>
        /// <param name="AtempTableName">Specify temp table name so you wont collide with other programs</param>
        public string DoLocalBackup(string AlocalPath)
        {
            string AremoteTempPath = @"c:\Backup";
            try
            {
                if (_conn == null)
                    return "No Connection";
                SqlCommand _command = new SqlCommand();
                _command.Connection = _conn;
                string fileName = _dbname + "  " + DateTime.Now.ToString().Replace(":", ".").Replace("/", "-"); ;

                string _sql;

                _sql = String.Format("BACKUP DATABASE {0} TO DISK = N'{1}\\{2}.bak' WITH FORMAT, COPY_ONLY, INIT, NAME = N'{0} - Full Database Backup', SKIP ", _dbname, AremoteTempPath, fileName);
                _command.CommandText = _sql;
                _command.ExecuteNonQuery();
                _conn.Close();
                SqlConnection.ClearAllPools();
                return "په کامیابۍ سره خوندي شو ";

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public string Restoredb(string AlocalPath)
        {
            // string AremoteTempPath = @"c:\Backup";//~/Backup/
            DoLocalBackup("");
            string sql = "";

            // var ToDisk = AlocalPath;// Path.GetFileName(AlocalPath.FileName);

            try
            {

                if (_conn == null)
                    return "No Connection";
                SqlCommand _command = new SqlCommand();
                _command.Connection = _conn;

                sql = "Alter Database " + _dbname + " SET Offline WITH ROLLBACK IMMEDIATE;";
                sql += "Restore Database " + _dbname + " from Disk='" + AlocalPath + "' WITH Replace;";
                sql += "Alter Database " + _dbname + " SET Online;";
                _command.CommandText = sql;
                _command.ExecuteNonQuery();
                _conn.Close();
                SqlConnection.ClearAllPools();
                return "په کامیابۍ سره ریسټور شو ";

            }
            catch (Exception)
            {
                _conn.Close();
                SqlConnection.ClearAllPools();
                return "";
            }
        }
    }
}
