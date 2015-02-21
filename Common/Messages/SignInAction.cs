using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
//using Newtonsoft.Json;
using com.web.server.weconnect.serviceutilities;

namespace com.web.server.weconnect.messages
{
    public class SignInAction : IMessageAction
    {
        public SignInAction()
        { }

        /// <summary>
        /// Method that will perform all necessary steps in order to sign in a user.
        /// This will entail scrutinizing input, validating identity, authorizing, and 
        /// </summary>
        /// <param name="msgReq"></param>
        public ResourceResponse ExecuteAction(ResourceRequest msgReq)
        {
            ResourceResponse rsp = new ResourceResponse();
            //TODO: Verify username contains a valid URI.
            rsp.CallId = msgReq.CallId;
            rsp.From = msgReq.From;
            rsp.Action = StringConstants.SIGNIN_ACTION;
            rsp.Status = StringConstants.SUCCESS_STATUS;
            rsp.MessageType = StringConstants.RESOURCE_MESSAGE_TYPE;
            rsp.RequestType = StringConstants.MESSAGE_RESPONSE;

            return rsp;
        }

        public void Dispose()
        {
        }
    }
    
}
