using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.Threading.Tasks;
using System.Net.Http;


namespace com.web.server.weconnect.weconnectwebrole.models
{
    /// <summary>
    /// Summary description for DBConnection
    /// </summary>
    public class DBConnection
    {        
        private System.Configuration.ConnectionStringSettings _connStr  = null;
        private SqlConnection _sqlConn                                  = null;



        public DBConnection()
        {
            System.Configuration.ConnectionStringSettingsCollection connStringCollection =
                    System.Web.Configuration.WebConfigurationManager.ConnectionStrings;
            //System.Configuration.ConnectionStringSettings connString;
            if (connStringCollection.Count > 0)
            {
                _connStr =
                    connStringCollection[1];
                if (_connStr != null)
                {
                    Console.WriteLine("SPCoreConnectionString connection string = \"{0}\"",
                        _connStr.ConnectionString);

                }
                else
                    Console.WriteLine("SPCoreConnectionString connection string");
            }


            _sqlConn = new SqlConnection(_connStr.ConnectionString);
        }
            

        /// <summary>
        /// 
        /// </summary>
        public async Task OpenConnectionAsync()
        {
            await _sqlConn.OpenAsync();
        }


     
        /// <summary>
        /// 
        /// </summary>
        ~DBConnection()
        {

            _sqlConn.Close();
            _sqlConn.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {            
            _sqlConn.Dispose();
            _sqlConn.Close();
        }


        /// <summary>
        /// Public accessor that returns the connection string
        /// </summary>
        public SqlConnection Connection
        {
            get
            {
                return _sqlConn;
            }
            set
            {
                _sqlConn = value;
            }

        }

        /// <summary>
        /// Rae method to execute SQL on the DB.
        /// </summary>
        /// <param name="sqlStatment"></param>
        public void ExecuteSql(string sqlStatment)
        {
            return;
        }

        /// <summary>
        /// Verify whether user exists.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<Guid> VerifyUserExists(string username)
        {
            return await CoreDBHelper.VerifyUserExists(username, _sqlConn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateUser(string userName, string password)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="http_ip"></param>
        /// <param name="http_port"></param>
        /// <param name="resourceid"></param>
        /// <param name="isMobileDevice"></param>
        /// <returns></returns>
        public async Task<Guid> InsertIntoResourceEndpointInfoTable(string http_ip,
            int http_port, Task<Guid> resourceid,bool isMobileDevice)
        {
           return await CoreDBHelper.InsertIntoResourceEndpointInfo(http_ip, http_port,
                resourceid, isMobileDevice, _sqlConn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="http_ip"></param>
        /// <param name="http_port"></param>
        /// <param name="resourceid"></param>
        /// <param name="isMobileDevice"></param>
        /// <returns></returns>
        public async Task<bool> InsertIntoResourceEndpointActivityHistory(Task<Guid> lastactiveendpointID)
        {
            return await CoreDBHelper.InsertIntoResourceEndpointActivityHistory(lastactiveendpointID,
                _sqlConn);
        }

        /// <summary>
        /// Returns the http response message that contains all active sessions.
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAllActiveSessions(HttpRequestMessage req)
        {
            return await CoreDBHelper.GetAllActiveSessions(_sqlConn, req);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CreateResource(string username)
        {
            return CoreDBHelper.CreateResource(_sqlConn, username);
        }
    }
}