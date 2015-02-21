using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using com.web.server.weconnect.weconnectwebrole.models.xml;
using System.Xml.Serialization;


namespace com.web.server.weconnect.weconnectwebrole.models
{
    public static class CoreDBHelper
    {   
        private static string _databaseName = "SPCoreDB.dbo.";
                
        /// <summary>
        /// Rae method to execute SQL on the DB.
        /// </summary>
        /// <param name="sqlStatment"></param>
        public static void ExecuteSql(string sqlStatment, SqlConnection conn)
        {
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="sqlConn"></param>
        /// <returns></returns>
        public static async Task<Guid> VerifyUserExists(string username, SqlConnection sqlConn)
        {
            Guid rid = new Guid(0,0,0,0,0,0,0,0,0,0,0);
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ResourceId_PK from ");
            sb.Append(_databaseName + " Resource");
            sb.Append(" WHERE Username = @username");
            SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConn);
            cmd.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
            try
            {
                using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                {
                    if (rdr.Read())
                    {
                        try
                        {
                            rid = rdr.GetFieldValue<Guid>(0);
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Trace.TraceError("Error reading column value from Resource Table");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error excuting reader.");
            }            
            
            return rid;
        }

        public static bool ValidateUser(string userName, string password, SqlConnection sqlConn)
        {
            return true;
        }

        public static async Task<bool> DoesEndpointIDExist(Guid epID, SqlConnection sqlConn)
        {
            bool rtn = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(1) from ");
            sb.Append(_databaseName + "ResourceEndpointInfo");
            sb.Append(" WHERE endpointID = @epId");            
            SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConn);
            cmd.Parameters.Add("epId", SqlDbType.UniqueIdentifier).Value = epID;
            int result = (int)await cmd.ExecuteScalarAsync();
            if (result == 1)
                rtn = true;
            return rtn;            
        }

        /// <summary>
        /// Helper function to insert information into the appropriate DB Table.
        /// </summary>
        /// <param name="http_ip">source IP address</param>
        /// <param name="http_port">source IP port.</param>
        /// <param name="resourceid">user resource ID</param>
        /// <param name="sqlConn">SQL connection object to perform database connections.</param>
        /// <returns></returns>
        public static async Task<Guid> InsertIntoResourceEndpointInfo(string http_ip, 
            int http_port, Task<Guid> resourceid, bool isMobileDevice, SqlConnection sqlConn)
        {
            Guid newEpID                = new Guid();
            StringBuilder sb            = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(_databaseName + "EndpointInfo");
            sb.Append(" (HTTP_IP, HTTP_Port, DeviceType, " +
                "ResourceId_FK, EndpointID_PK)");
            sb.Append(" VALUES (@http_ip, @http_port, @devicetype, @resourceId, @endpointid);");
            SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConn);
            cmd.Parameters.Add("http_ip",       SqlDbType.NVarChar).Value = http_ip;
            cmd.Parameters.Add("http_port",     SqlDbType.Int).Value = http_port;
            cmd.Parameters.Add("resourceId",    SqlDbType.UniqueIdentifier).Value = resourceid;
            cmd.Parameters.Add("deviceType",    SqlDbType.SmallInt).Value = isMobileDevice;
            cmd.Parameters.Add("endpointid", SqlDbType.UniqueIdentifier).Value = newEpID;

            int result = await cmd.ExecuteNonQueryAsync();
            if (result<=0)
            {
                throw new System.Exception("EXCEPTION: Could not insert into DB Table: ResourceEndpointInfo.");
            }
            return newEpID;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="http_ip"></param>
        /// <param name="resourceid"></param>
        /// <returns></returns>
        public static async Task<bool> InsertIntoResourceEndpointActivityHistory(Task<Guid> lastActiveEndpointID, 
            SqlConnection sqlConn)
        {
            bool rtn = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");            
            sb.Append(_databaseName + "ResourceEndpointActivityHistory");
            sb.Append(" (LastActiveEndpointID_FK, LastEndpointActivityDateTime)");
            sb.Append(" VALUES (@lastactiveendpointID_FK, @lastendpointactivitydatetime);");
            SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConn);
            cmd.Parameters.Add("lastactiveendpointid_FK", SqlDbType.NVarChar).Value = lastActiveEndpointID;                        
            cmd.Parameters.Add("lastactivitydatetime", SqlDbType.DateTime).Value = DateTime.Now;            
            int result = await cmd.ExecuteNonQueryAsync();
            if (result > 0)
                rtn = true;           
            return rtn;            
        }

        /// <summary>
        /// This method shall query the database to return all active sessions
        /// </summary>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetAllActiveSessions(SqlConnection sqlConn,
            HttpRequestMessage req)
        {
            HttpResponseMessage rsp = req.CreateResponse();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM ");
            sb.Append(_databaseName + "Session");
            sb.Append(" WHERE DateActive > @todaysDate)");            
            SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConn);
            cmd.Parameters.Add("todaysDate", SqlDbType.DateTime).Value = System.DateTime.Now;
            SqlDataReader rdr = await cmd.ExecuteReaderAsync();
            if (rdr.HasRows)
            {
                //seriailize results into and array of session objects.
            }
               
            return rsp;

        }



        #region test helpers
        public static bool CreateResource(SqlConnection sqlConn, string username)
        {            
            bool rtn = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(_databaseName + "Resource");
            sb.Append(" (ResourceId_PK, Username, Pass, IsAdmin)");
            sb.Append(" VALUES (@resourceId_pk, @username, @pass, @isadmin);");
            SqlCommand cmd = new SqlCommand(sb.ToString(), sqlConn);
            cmd.Parameters.Add("resourceId_pk", SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
            cmd.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
            cmd.Parameters.Add("pass", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("isadmin", SqlDbType.Bit).Value = 1;
            int result = cmd.ExecuteNonQuery();
            if (result > 0)
                rtn = true;
            return rtn;            
        }
        #endregion

    }
}