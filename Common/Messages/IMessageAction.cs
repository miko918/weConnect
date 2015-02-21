using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.web.server.weconnect.messages
{
    //Base class that will contain derived classes that are specific to the action that needs processing.
    public interface IMessageAction: IDisposable
    {        
        /// <summary>
        /// Method that will execute action. Return value will be the msg response object for the assciated request.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        ResourceResponse ExecuteAction(ResourceRequest msg);
    }
}
