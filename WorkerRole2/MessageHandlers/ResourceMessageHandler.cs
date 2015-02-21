using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.web.server.weconnect.resourceworkerrole.messagehandlers
{
    public class ResourceMessageHandler: MessageHandler
    {

        public ResourceMessageHandler()
            :base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void ProcessMessasge(string message)
        {
            //base class will implement
            int i = 0;

        }
    }
}
