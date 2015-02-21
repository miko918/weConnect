using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
//using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using com.server.speeddating.ResourceWorkerRole.StorageManager;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using System.Data.Services.Common;
using System.IO;
using System.Runtime.Serialization.Json;
using com.server.speeddating.ResourceWorkerRole.MessageHandlers;
using com.server.speeddating.Messages;
using com.server.speeddating.ServiceUtilities;

namespace com.server.speeddating.ResourceWorkerRole
{
    public class ResourceWorkerRole : RoleEntryPoint
    {
        private QueueManager _qm = new QueueManager();
        //ResourceTableManager _rtm       = new ResourceTableManager();
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("WorkerRole1 entry point called", "Information");

            while (true)
            {
                Thread.Sleep(5000);
                Trace.TraceInformation("Working", "Information");

                //Read message from Queue.                
                CloudQueueMessage retrievedMessage = _qm.ResourceQueueClient.GetMessage();
                MessageHandler m = new ResourceMessageHandler();
                //Assuming the invisibility timeout is less than 30 secs. We may need to increase this.
                //Once we are done with the message we need to delete the message.
                try
                {
                    //Start a thread here to process message.  The callback will be called when the thread completes or an error occurs along the way                    
                    m.Message = retrievedMessage;
                    IAsyncResult ar = m.ProcessMessageHandler.BeginInvoke(retrievedMessage.AsString, this.MessageProcessingComplete, m);
                }
                catch (System.Exception ex)
                {
                    Trace.TraceError("Error occured while processing message:\n{0}", retrievedMessage.AsString);
                }
                finally
                {
                    _qm.ResourceQueueClient.DeleteMessage(m.Message);
                }
                //Process the message in less than 30 seconds, and then delete the message

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

            MessageHandler m = ar.AsyncState as ResourceMessageHandler;
            //Delete Message from queue since we have completed the callback.
            _qm.ResourceQueueClient.DeleteMessage(m.Message);
            m.ProcessMessageHandler.EndInvoke(ar);

            //Where do I need to write to so that the response is returend to the client.  I need to write to teh outbound queues
            try
            {
                CreateResponseForRequest(m);
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

            string msg = m.Message.ToString();
            ResourceResponse rsp = new ResourceResponse();
            rsp.CallId = new Guid().ToString();
            rsp.ActionType = StringConstants.SIGNIN_ACTION;
            rsp.Status = StringConstants.SUCCESS_STATUS;
            rsp.MessasgeType = StringConstants.RESOURCE_MESSAGE_TYPE;
            rsp.RequestType = StringConstants.MESSAGE_RESPONSE;

            MemoryStream memStream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(rsp.GetType());
            ser.WriteObject(memStream, rsp);

            return rsp;
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            //_rtm.InitializeTables();
            _qm.InitQueues();
            //RegisterCallbacks to that the message handlers know what to call when complete.
            //InitializeQueues();
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
