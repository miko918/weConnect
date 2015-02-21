using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using com.web.server.weConnect.models;


namespace com.web.server.weConnect.controllers
{
    /// <summary>
    /// Class the represents a user.
    /// </summary>
    public class UsersController : ApiController
    {
       
        // GET register
        //When called will return the specific user baed on user name
        public Task<Guid> Get(string uri)
        {
            DBConnection conn = DBConnectionManager.GetInstance().Allocate();
            Task<Guid> rtn = conn.VerifyUserExists(uri);
            if (null != rtn)
            {
                System.Diagnostics.Trace.TraceInformation("User {0} exists", uri);
            }
            else
            {
                System.Diagnostics.Trace.TraceInformation("User {0} does not exist", uri);
            }
            DBConnectionManager.GetInstance().Deallocate(conn);
            return rtn;
        }

        // GET register
        //When called will return the specific user baed on user name
        public async Task<bool> Get(int position)
        {
            return true;
        }

        // GET register
        //When called will return the list of users that are present in the system
        public IEnumerable<string> Get()
        {
            List<string> list = new List<string>();
            list.Add("Michael");
            list.Add("Joe");
            return list;
            
        }

        // POST api/register
        // Add a user to the table if registered and signed in.
        //public void Post([FromBody]string value)
        //{
        //    //Verify if user is sigend in.  If so then they can 
        //}

        // PUT (modify) api/register/{username}?password
        //Sign in the user.
        public Guid Put(int id, [FromBody]string value)
        {
            //Assume user is created in resource table.
            //Determine whether user is authorized to sign in.
            //If user is authorized to sign in then write to the DB and sign in.
            //AuthorizeUser(username)
            return SignInUserEndpoint("", "");
            
        }

        // DELETE api/values/5
        //This will sign out the user.
        public void Delete(int id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private Guid SignInUserEndpoint(string username, string password)
        {
            return new Guid();
        }
    }
}
