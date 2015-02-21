using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.web.server.weconnect.messages;
using System.IO;
using System.Runtime.Serialization.Json;
using com.web.server.weconnect.serviceutilities;

namespace com.web.server.weconnect.messagehandlers
{
    public class ResourceMessageHandler : MessageHandler
    {

        public ResourceMessageHandler(Byte[] rawMsg)
            : base(rawMsg)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void ProcessMessasge(string message)
        {

        }


        /// <summary>
        /// Message Handler that will process the already supplied message.
        /// </summary>
        /// <param name="message"></param>
        public override void ProcessMessasge()
        {
            //Take the stream and deserialize back into an object.            
            MemoryStream stream1            = new MemoryStream(this._rawReqMsg);
            stream1.Position                = 0;
            DataContractJsonSerializer ser  = new DataContractJsonSerializer(typeof(ResourceRequest));
            ResourceRequest req             = ser.ReadObject(stream1) as ResourceRequest;
            ResourceResponse rsp = null;          
       
            
            //This is the signin action.
            if (req.Action.ToLower().Equals(StringConstants.SIGNIN_ACTION.ToLower()))
            {
                //TODO: Consider pooling these objects to ensure that we don't overuse new for each message.
                //Pool size needs to be dynamic.
                using (IMessageAction action = new SignInAction())
                {
                    try
                    {
                        rsp = action.ExecuteAction(req);
                        //Serialize the message to byte array and then assign to property.  
                        //Calling functional will reference property for response to caller.
                        if (null != rsp)
                        {
                            SerializeToByteArray(rsp);
                        }
                        
                    }
                    catch(System.Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError("Failed to execute signin action.  Will throw an exception so that parent can catch and bahave.");
                        throw ex;
                
                    }
                }   
            }
        }

        /// <summary>
        /// Returns the byte array as the ResourceRequest object.
        /// </summary>
        /// <returns></returns>
        public override MessageBase GetMessageAsObject()
        {
            //Take the steram and deserialize back into an object.            
            MemoryStream stream1 = new MemoryStream(this._rawReqMsg);
            stream1.Position = 0;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResourceRequest));
            ResourceRequest req = ser.ReadObject(stream1) as ResourceRequest;
            return req;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override void SerializeToByteArray(MessageBase msg)
        {
            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(msg.GetType());
            ser.WriteObject(memStream, msg);
            //TODO: Asssign the byte array.  Confirm that this should be a deep copy.
            this.RawRspMsg = memStream.ToArray();
        }
    }
}
