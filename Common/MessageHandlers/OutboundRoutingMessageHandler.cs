using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;

namespace com.web.server.weconnect.messagehandlers
{
    public class OutboundRoutingMessageHandler: MessageHandler
    {
        public OutboundRoutingMessageHandler(Byte[] rawMsg)
            :base(rawMsg)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void ProcessMessasge(string message)
        {
            //base class will implement
            int i = 0;

        }

        /// <summary>
        /// Message Handler that will process the already supplied message.
        /// </summary>
        /// <param name="message"></param>
        public override void ProcessMessasge()
        {
            int i = 0;

        }
    }
}
