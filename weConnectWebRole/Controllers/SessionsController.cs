using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.web.server.weConnect.models.xml;
using System.Xml.Serialization;
using com.web.server.weConnect.models;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Web;

namespace com.web.server.weConnect.controllers
{
    /// <summary>
    /// Controller class that allows access to a list of sessions or a specific session.
    /// </summary>
    public class SessionsController : ApiController
    {
        /// <summary>
        /// Method will return all created sessions.  
        /// WARNING, this may be a large XML.
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get()
        {
           //Deserialize into object.
            HttpResponseMessage rsp             = null;
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;
            string exStr                        = String.Empty;
            bool abort                          = false;            
            string srcHttp_IP                   = String.Empty;
            int srcHttp_Port                    = -1;
            int rid                             = -1;
            Guid newEpid                        = new Guid(0,0,0,0,0,0,0,0,0,0,0);
            Sessions activeSessionsdocument     = null;

            //Deserialize content into SignIn object.
            DBConnectionManager  dbConnMgr      = DBConnectionManager.GetInstance();                        

            /*Query DB for all sessions that have been created.  the Expired sessions shall not be returned.
             * * */
            DBConnection conn = dbConnMgr.Allocate();
            try
            {
                rsp = await conn.GetAllActiveSessions(Request);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error, trying to verify user login information.");                
                exStr = "Error, trying to verify user login information.\n Exception: " + ex.ToString();
                rsp = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exStr);
            }

            return rsp;
        }

        /// <summary>
        /// Method will return all all sessions that are either active or inactive.
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> Get(bool isactive)
        {
            throw new NotImplementedException("");
        }

        /// <summary>
        /// Method will return the information for a specific sesson including roster.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> Get(Guid sessionId)
        {
            throw new NotImplementedException("");
        }

        /// <summary>
        /// Method will return all sessions that a user is part of.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> Get(string username)
        {
            throw new NotImplementedException("");
        }
    }
}
