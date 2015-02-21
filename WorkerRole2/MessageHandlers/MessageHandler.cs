using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace com.web.server.weconnect.resourceworkerrole.messagehandlers
{
    public abstract class MessageHandler
    {           
        /// <summary>
        /// Delegate that will process incoming message and return result to web role via supplied callback
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Callback"></param>
        public delegate void ProcessMessageDelagate(string message);
        protected ProcessMessageDelagate _pmd = null;
        public CloudQueueMessage Message { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public MessageHandler()
        {
            _pmd = new ProcessMessageDelagate(this.ProcessMessasge);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadCompleteCallback"></param>
        /// <param name="state">message to process.</param>
        public virtual void ProcessMessasge(string message)
        {
            //base class will implement
            
        }

        public ProcessMessageDelagate ProcessMessageHandler { get { return _pmd; } }
    }
}
