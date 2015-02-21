using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
using com.web.server.weconnect.store;
using com.web.server.weconnect.store.inboundrequest;

namespace com.web.server.weconnect
{
    public class OutboundRoutingHandler
    {
        public void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("Outbound routing handler entry point called");

            while (true)
            {
                //Thread.Sleep(10000);
                Trace.TraceInformation("ResourceWorkerRole Working", "Information");

                //Read message from Queue.           
                CloudQueueMessage retrievedMessage = QueueManager.OutboundQueueClient.GetMessage();                
                //Assuming the invisibility timeout is less than 30 secs. We may need to increase this.
                //Once we are done with the message we need to delete the message.
                if (null != retrievedMessage)
                {
                    MessageHandler m = new OutboundRoutingMessageHandler(retrievedMessage.AsBytes);
                    try
                    {
                        //Start a thread here to process message.  The callback will be called when the thread completes or an error occurs along the way                    
                        m.Message = retrievedMessage;
                        IAsyncResult ar = m.ProcessMessageHandler.BeginInvoke(this.MessageProcessingComplete, m);
                    }
                    catch (System.Exception ex)
                    {
                        Trace.TraceError("Error occured while processing message:\n{0}", retrievedMessage.AsString + ex.ToString());
                    }
                    finally
                    {
                        //if (null != queuemanager.outboundqueueclient.peekmessage())
                        //{
                        //    queuemanager.outboundqueueclient.deletemessage(m.message);
                        //}
                    }
                    //Process the message in less than 30 seconds, and then delete the message
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectedMsg"></param>
        /// <returns></returns>
        public static MessageBase GetMessageFromOutboundQueue(MessageBase expectedMsg)
        {
            CloudQueueMessage tmpMsg        = null;
            MessageBase msg                 = null;
            MemoryStream stream1            = null;
            DataContractJsonSerializer ser  = null;
            bool foundMsg                   = false;
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("Outbound Routing Handler called to return a response for a request that was received");
            //wait 25 seconds.
            DateTime waitTime = new DateTime().AddSeconds((double)25);
            //ensure tto wait until the message is found or the time is expired.
            while (!foundMsg /*&& waitTime.ToUniversalTime() > DateTime.Now.ToUniversalTime()*/)
            {

                //Get the message
                //Get the current message to process. There really shouldn't be any more messages in the queue as this is on demand basis.
                tmpMsg = QueueManager.OutboundQueueClient.GetMessage();
                
                if (null != tmpMsg)
                {
                    Trace.TraceInformation("Find a message in outbound routing queue after we peeked inside the queue.");
                    //Delete right away.  we may need to enqueue again.
                    QueueManager.OutboundQueueClient.DeleteMessage(tmpMsg);
                    //We need to find the message that we are looking for.  If we find it, return it to the caller so that 
                    //a response will be sent.
                    msg = new ResourceResponse();
                    stream1 = new MemoryStream(tmpMsg.AsBytes);
                    stream1.Position = 0;
                    ser = new DataContractJsonSerializer(typeof(ResourceResponse));
                    msg = ser.ReadObject(stream1) as ResourceResponse;
                    if ((msg.CallId.ToLowerInvariant().Equals(expectedMsg.CallId.ToLowerInvariant())) &&
                        (msg.Action.ToLowerInvariant().Equals(expectedMsg.Action.ToLowerInvariant())) &&
                        (msg.RequestType.Equals(expectedMsg.RequestType.ToLowerInvariant())) &&
                        (msg.From.ToLowerInvariant().Equals(expectedMsg.From.ToLower())))
                    {
                        //we found the message.  Delete it from the queue
                        foundMsg = true;
                        //need to delete the message from the queue.
                        //QueueManager.OutboundQueueClient.DeleteMessage(tmpMsg);
                        Trace.TraceInformation("Deleted message from queue.");
                        break;
                    }
                    else
                    {
                        Trace.TraceInformation("Error.  Received a message but message propterties were not what was expected.");
                        Trace.TraceError("Expected CallId :{0}; recevied CallId: {1}",expectedMsg.CallId, msg.CallId);
                        Trace.TraceError("Expected Action :{0}; recevied Action: {1}",expectedMsg.Action, msg.Action);                       
                        Trace.TraceError("Expected RequestType :{0}; recevied RequestType: {1}",expectedMsg.RequestType, msg.RequestType);
                        Trace.TraceError("Pushing message bcak onto outbound queue.");
                        QueueManager.OutboundQueueClient.AddMessage(new CloudQueueMessage(tmpMsg.AsBytes));                        
                    }
                }
                else
                {
                    //TODO
                    Trace.TraceInformation("Did not find a message in outbound routing queue after we peeked inside the queue. Will look again.  Loop expires in {0} seconds.", waitTime - DateTime.Now.ToUniversalTime());
                }
            }

            //return the message to the caller.
            return msg;
        }


        /// <summary>
        /// Will look at Message Type and creat the appropriate message handler to process the message.
        /// </summary>
        /// <param name="gm"></param>
        public void MessageProcessingComplete(IAsyncResult ar)
        {
            HttpRequestMessage httpReq = null;
            if (!ar.IsCompleted)
            {
                throw new System.Threading.ThreadStateException("Thread processing is not complete.");
            }
            
            OutboundRoutingMessageHandler m = ar.AsyncState as OutboundRoutingMessageHandler;
            ResourceRequest msg = new ResourceRequest();
            //Set up Person object...
            MemoryStream stream1 = new MemoryStream(m.Message.AsBytes);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResourceRequest));
            ser.WriteObject(stream1, msg);

            //User the Message to look up the requst from the Azure Table.
            InboundRequestEntityDataSource ds = new InboundRequestEntityDataSource();
            IEnumerable<InboundRequestTableEntity> reqList = ds.Select();
            string tableKey = msg.CallId + ";" + msg.From;
            byte[] requestMsgArray = null;

            foreach (InboundRequestTableEntity iter in reqList)
            {
                if (iter.RowKey.Equals(msg.CallId + tableKey))
                {
                    //Found our Request message. Use it to create the response
                    requestMsgArray = iter.RequestMessage;                                       
                    break;
                }
            }

        /// <summary>
        /// 

            //Serialize byte array to object
            httpReq = SerializeToObject(requestMsgArray) as HttpRequestMessage;
            CreateAndSendResponse(httpReq);
            
            //Delete Message from queue since we have completed the callback.
            if (null != QueueManager.OutboundQueueClient.PeekMessage())
            {
                QueueManager.OutboundQueueClient.DeleteMessage(m.Message.Id, m.Message.PopReceipt);
            }
            m.ProcessMessageHandler.EndInvoke(ar);           
        }
        /// </summary>
        /// <param name="array"></param>
        private object SerializeToObject(byte[] array)
        {
            try
            {
                System.IO.MemoryStream ms = new MemoryStream(array);
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binFormatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                ms.Position = 0;
                return binFormatter.Deserialize(ms);
            }
            catch (Exception e)
            {
                //LOG HERE
            }
            return null;
        }

        /// <summary>
        /// Send Message Back to client.
        /// </summary>
        /// <param name="httpReq"></param>
        private void CreateAndSendResponse(HttpRequestMessage httpReq)
        {
            HttpResponseMessage resp = httpReq.CreateResponse();
            resp.Content = httpReq.Content;
            resp.StatusCode = HttpStatusCode.OK;
            System.Web.HttpResponse rr = new System.Web.HttpResponse(null);
            
            //httpReq.GetConfiguration().
        }
    }
}
