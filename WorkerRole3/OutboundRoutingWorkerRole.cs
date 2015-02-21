using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
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
using com.web.server.weconnect.weconnectwebrole.models;
using com.web.server.weconnect.messagehandlers;

namespace com.web.server.weconnect.outboundroutingworkerrole
{
    public class OutboundRoutingWorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("WorkerRole3 entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("ResourceWorkerRole Working", "Information");

                //Read message from Queue.           
                CloudQueueMessage retrievedMessage = QueueManager.OutboundQueueClient.GetMessage();
                MessageHandler m = new OutboundRoutingMessageHandler();
                //Assuming the invisibility timeout is less than 30 secs. We may need to increase this.
                //Once we are done with the message we need to delete the message.
                if (null != retrievedMessage)
                {
                    try
                    {
                        //Start a thread here to process message.  The callback will be called when the thread completes or an error occurs along the way                    
                        m.Message = retrievedMessage;
                        IAsyncResult ar = m.ProcessMessageHandler.BeginInvoke(retrievedMessage.AsString, this.MessageProcessingComplete, m);
                    }
                    catch (System.Exception ex)
                    {
                        Trace.TraceError("Error occured while processing message:\n{0}", retrievedMessage.AsString + ex.ToString());
                    }
                    finally
                    {
                        if (null != QueueManager.OutboundQueueClient.PeekMessage())
                        {
                            QueueManager.OutboundQueueClient.DeleteMessage(m.Message);
                        }
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

            MessageHandler m = ar.AsyncState as ResourceMessageHandler;
            //Delete Message from queue since we have completed the callback.
            if (null != QueueManager.OutboundQueueClient.PeekMessage())
            {
                QueueManager.OutboundQueueClient.DeleteMessage(m.Message.Id, m.Message.PopReceipt);
            }
            m.ProcessMessageHandler.EndInvoke(ar);

           //Look up the response to send back to clients.
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
