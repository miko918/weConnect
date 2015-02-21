using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using System.Data.Services.Common;
using System.IO;
using System.Runtime.Serialization.Json;
using com.web.server.weconnect.messages;
using com.web.server.weconnect.serviceutilities;
using com.web.server.weconnect.resourceworkerrole;
using com.web.server.weconnect.messagehandlers;
using com.web.server.weconnect.store;

namespace com.web.server.weconnect.resourceworkerrole
{
    public class ResourceWorkerRole : RoleEntryPoint
    {
        //private QueueManager _qm = new QueueManager();
        //ResourceTableManager _rtm       = new ResourceTableManager();
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("WorkerRole1 entry point called", "Information");

            while (true)
            {                
                Trace.TraceInformation("ResourceWorkerRole Working", "Information");

                //Read message from Queue.           
                CloudQueueMessage retrievedMessage = QueueManager.ResourceQueueClient.GetMessage();
                
                //Assuming the invisibility timeout is less than 30 secs. We may need to increase this.
                //Once we are done with the message we need to delete the message.
                if (null != retrievedMessage)
                {
                    MessageHandler m = new ResourceMessageHandler(retrievedMessage.AsBytes);
                    try
                    {
                        //Start a thread here to process message.  The callback will be called when the thread completes or an error occurs along the way                    
                        m.Message = retrievedMessage;
                        IAsyncResult ar = m.ProcessMessageHandler.BeginInvoke(this.MessageProcessingComplete, m);
                    }
                    catch (System.Exception ex)
                    {
                        Trace.TraceError("Error occured while processing message:\n{0}", retrievedMessage.AsString+ex.ToString());
                    }
                    finally
                    {
                        
                        QueueManager.ResourceQueueClient.DeleteMessage(m.Message);
                        
                    }
                    //Process the message in less than 30 seconds, and then delete the message
                }

            }
        }

        /// <summary>
        /// Will look at Message Type and creat the appropriate message handler to process the message.
        /// </summary>
        /// <param name="gm"></param>
        public void MessageProcessingComplete(IAsyncResult ar)
        {
            //TODO.
            //We need to write the result back to the response queue so that the web role can push it back to the client.
            //Get the Message object and call endinvoke on it to end the process.  
            if (!ar.IsCompleted)
            {
                throw new System.Threading.ThreadStateException("Thread processing is not complete.");
            }

            ResourceMessageHandler m = ar.AsyncState as ResourceMessageHandler;                     
            
            m.ProcessMessageHandler.EndInvoke(ar);

            //Where do I need to write to so that the response is returend to the client.  I need to write to teh outbound queues
            try
            {
                //create response and dispatch to outbound queue.
                //ResourceResponse response =(ResourceResponse) CreateResponseForRequest(m);    
                //TODO: Add trace statement on message being added for outbound here.
                //System.Diagnostics.Trace.TraceInformation("Added message {0} to ResourceQueue",response);
                //Add message to the queue
                QueueManager.OutboundQueueClient.AddMessage(
                    new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(m.RawRspMsg)); 
                
            }                        
            catch (System.Exception ex)
            {
                Trace.TraceError("Error creating resource resposne");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        private MessageBase CreateResponseForRequest(MessageHandler m)
        {
            //We have access to the request to pull out call ID and other associated request info.
            ResourceRequest req = m.GetMessageAsObject() as ResourceRequest;

            string msg                  = m.Message.ToString();
            ResourceResponse rsp        = new ResourceResponse();
            rsp.CallId                  = new Guid().ToString();           
            rsp.Action                  = StringConstants.SIGNIN_ACTION;
            rsp.Status                  = StringConstants.SUCCESS_STATUS;
            rsp.MessageType             = StringConstants.RESOURCE_MESSAGE_TYPE;
            rsp.RequestType             = StringConstants.MESSAGE_RESPONSE;
            //TODO Add an Auth token to ensure authorization is valid.  Any ther client will need to obtain their own authtoken when they sign in.
            //The authtoken is specific perf endpoint.
            //rsp.AuthToken               = 
            MemoryStream memStream      = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(rsp.GetType());
            ser.WriteObject(memStream, rsp);
            return rsp;
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            //_rtm.InitializeTables();
            QueueManager.InitQueues();
            //RegisterCallbacks to that the message handlers know what to call when complete.
            //InitializeQueues();
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
