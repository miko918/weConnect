using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using com.web.server.weconnect.messages;

namespace com.web.server.weconnect.messagehandlers
{
    public abstract class MessageHandler
    {           
        /// <summary>
        /// Delegate that will process incoming message and return result to web role via supplied callback
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Callback"></param>
        public delegate void ProcessMessageDelagate();
        protected ProcessMessageDelagate _pmd = null;
        public CloudQueueMessage Message { get; set; }
        protected Byte[] _rawReqMsg = null;
        protected Byte[] _rawRspMsg = null;

        /// <summary>
        /// ctor
        /// </summary>
        public MessageHandler(Byte[] rawReqMsg)
        {
            _pmd = new ProcessMessageDelagate(this.ProcessMessasge);
            ///
            _rawReqMsg = rawReqMsg;
        }

        /// <summary>
        /// Return byte array representation of message. no need to make it thread safe as these objects are created on demand.
        /// </summary>
        public Byte[] RawRspMsg
        {            
            get
            {
                return _rawRspMsg;
            }
            set
            {
                _rawRspMsg = value;
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadCompleteCallback"></param>
        /// <param name="state">message to process.</param>
        public virtual void ProcessMessasge()
        {

        }

        /// <summary>
        /// Virtual method that will deserialize raw bytes into its object representation.
        /// </summary>
        /// <returns></returns>
        public virtual MessageBase GetMessageAsObject()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual void SerializeToByteArray(MessageBase msg)
        {
            throw new NotImplementedException();
        }

        public ProcessMessageDelagate ProcessMessageHandler { get { return _pmd; } }
    }
}
