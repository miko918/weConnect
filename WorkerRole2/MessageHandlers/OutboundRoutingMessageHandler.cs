using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.web.server.weconnect.resourceworkerrole.messagehandlers
{
    class OutboundRoutingMessageHandler: MessageHandler
    {
        public OutboundRoutingMessageHandler()
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
