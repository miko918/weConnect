using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using com.web.server.weconnect.weconnectwebrole.models.xml;
using System.Xml.Serialization;
using com.web.server.weconnect.weconnectwebrole.models;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Web;
using System.Diagnostics;
using Newtonsoft.Json;
using com.web.server.weconnect.store;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using com.web.server.weconnect.messages;
using System.Runtime.Serialization.Json;
using com.web.server.weconnect.serviceutilities;
using com.web.server.weconnect.store.inboundrequest;
using com.web.server.weconnect;

namespace com.web.server.weconnect.weconnectwebrole.controllers
{
    public class UserController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        private CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(
               Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedDatingStorageConnectonString"));
        /// <summary>
        /// This method shall sign in a user to the service.  The user's credntials shall be available in 
        /// the body of the message. We'll need to send the token along with each message to prove 
        /// that they are signed in.  The token should be refreshed every 30 mins or so.  Perhaps use 
        /// Facebok auth to sign in and we manage the token?
        /// </summary>
        /// <returns></returns>        
        public async Task<HttpResponseMessage> Put()
        {            
            //Deserialize into object.
            HttpResponseMessage rsp             = null;
            //HttpStatusCode httpStatusCode       = HttpStatusCode.OK;
            string exStr                        = String.Empty;
            //bool abort                          = false;            
            string srcHttp_IP                   = String.Empty;
            //int srcHttp_Port                    = -1;
            //Task<Guid> newEpid                  = null;            
            //Task<Guid> rid                      = null;
            string jsonText                     = null;
            MemoryStream stream1                = null;

            try
            {
                jsonText = await this.Request.Content.ReadAsStringAsync();
                stream1 = new MemoryStream(Encoding.UTF8.GetBytes(jsonText));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResourceRequest));
                stream1.Position = 0;
                Console.Write("Deserialize Message Base");
                //Deserialize into object to parse headers.
                MessageBase reqMsg = ser.ReadObject(stream1) as MessageBase;
                //WriteRequestToStore(reqMsg);
                    
                if (reqMsg.MessageType.ToLower().Equals(StringConstants.RESOURCE_MESSAGE_TYPE.ToLower()))
                {
                    stream1.Position = 0;
                    //This will dispatch the message to the message queue for processing by the resource worker role.
                    DispatchToResourceQueue(stream1.ToArray());
                    //Use a task and wait for the message to return from the outbound queue. Pass the source message so that we 
                    //can compare the messages read from the outbound queue.  This method shall return in 30 seconds.                    
                    //MessageBase tresult = await Task.Factory.StartNew<MessageBase>(
                    //                () => OutboundRoutingHandler.GetMessageFromOutboundQueue(reqMsg)
                    //);
                    //wait 25 seconds.
                    DateTime waitTime = DateTime.Now.AddSeconds((double)25);
                    //Ensure to wait until a message is found or the time is expired.
                    //while (!foundMsg /*&& waitTime.ToUniversalTime() > DateTime.Now.ToUniversalTime()*/)
                    ResourceResponse tmpResponse = OutboundRoutingHandler.GetMessageFromOutboundQueue(reqMsg) as ResourceResponse;
                    while (null == tmpResponse /*&& waitTime.ToUniversalTime() > DateTime.Now.ToUniversalTime()*/)
                    {
                        System.Threading.Thread.Sleep(10000);
                        tmpResponse = OutboundRoutingHandler.GetMessageFromOutboundQueue(reqMsg) as ResourceResponse;
                    }


                    if (null == tmpResponse)
                    {
                        Console.Write("Did not receive response from outbound queue in time.  Need to send back a generic error for client to process.");
                        Trace.TraceInformation("Did not receive response from outbound queue in time.  Need to send back a generic error for client to process.");
                        //TODO: Send back a generic error.
                    }                    

                    //Create the http response.
                    rsp = this.Request.CreateResponse(GetHttpStatusCode(tmpResponse.Status));
                                    }
            }
            catch (System.Exception se)
            {
                //remove message from azure table and clean up.
            }
            finally
            {
                stream1.Close();
                stream1.Dispose();
                //RemoveRequestFromStore();
            }

            return rsp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private HttpStatusCode GetHttpStatusCode(string status)
        {
            HttpStatusCode rtn = HttpStatusCode.InternalServerError;
            if (status.Equals(StringConstants.SUCCESS_STATUS))
            {
                rtn = HttpStatusCode.OK;
            }

            return rtn;
        }

        /// <summary>
        /// Method that gets byte[] from object.
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
        private byte[] ObjectToByteArray(object obj)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binFormatter =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Trace.TraceInformation("Exception caught in process {0}", e.ToString());
            }

            //Error occured. Return null
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void WriteRequestToStore(MessageBase msg)
        {
            byte[] serializedRequest = this.ObjectToByteArray(this.Request);    
            
            string rowkey = msg.CallId + ";" + msg.From;
            // Create a new customer entity.
            InboundRequestTableEntity inboundReqTableEntity         = new InboundRequestTableEntity(System.DateTime.UtcNow.ToString("MMddyyy"), rowkey);
            inboundReqTableEntity.RequestMessage                    = serializedRequest;
            InboundRequestEntityDataSource ds                       = new InboundRequestEntityDataSource();
            ds.AddInboundRequestTableEntry(inboundReqTableEntity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="QueueName"></param>
        private void DispatchToQueue(string messageType, string msg)
        {
            string queueName = String.Empty;
            if (String.IsNullOrEmpty(messageType) || 
                String.IsNullOrEmpty(msg))
            {
                throw new ArgumentException("message type or queue name are null or empty. \nMessage Type: " +  
                    messageType + " \nQueuename: " + queueName);
            }

            //At this point we should dispatch on a thread to write to a queue.
            if (messageType.ToLower().Equals("resource"))
            {               
                //Add message to the queue
                QueueManager.ResourceQueueClient.AddMessage(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(msg)); 
                System.Diagnostics.Trace.TraceInformation("Added message {0} to ResourceQueue",msg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="QueueName"></param>
        private void DispatchToResourceQueue(Byte[] msg)
        {            
            if (0 == msg.Length)
            {
                throw new ArgumentException("Dispatch to Queue: Message is null");
            }

            //Add message to the queue
            QueueManager.ResourceQueueClient.AddMessage(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(msg));
            System.Diagnostics.Trace.TraceInformation("Added message ResourceQueue: {0}", msg);
            //wait fot signal to set that response is ready.
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="QueueName"></param>
        private void DispatchToResourceQueue(ResourceRequest msg)
        {
            string queueName = String.Empty;
            
            if (null == msg)
            {
                throw new ArgumentException("Dispatch to Queue: Message is null");
            }

            //Need to create the json string and add it to the queue.
            MemoryStream stream1            = new MemoryStream();
            DataContractJsonSerializer ser  = new DataContractJsonSerializer(typeof(ResourceRequest));
            ser.WriteObject(stream1, msg);
            stream1.Position                = 0;
            StreamReader sr                 = new StreamReader(stream1);
            stream1.ToString();
            Console.Write("JSON form of Person object: ");
            Console.WriteLine(sr.ReadToEnd());           
                
            //Add message to the queue
            QueueManager.ResourceQueueClient.AddMessage(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(sr.ReadToEnd())); 
            System.Diagnostics.Trace.TraceInformation("Added message {0} to ResourceQueue",msg);
        }

            

            //At this point we should dispatch on a thread to write to a queue.
            //if (msg.ToLower().Equals("resource"))
            //{
                //Add message to the queue
                //QueueManager.ResourceQueueClient.AddMessage(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage(msg));
                //System.Diagnostics.Trace.TraceInformation("Added message {0} to ResourceQueue", msg);
            //}
       // }


            //Parse Json hear to understand undersand while Queue to send to
            //SignInUserInfo siui                 = new SignInUserInfo();           
            //XmlSerializer xs1                   = new XmlSerializer(siui.GetType());
            ////wait for thread to complete.
            //Task.WaitAll();

            ////using (TextReader tr1 = new StringReader(t))
            ////{
            ////    // Deserialize user info to object.
            ////    siui = (SignInUserInfo)xs1.Deserialize(tr1);
            ////}

            ///*If Resource Id exsits, then validate username and password in EndPointinfo table. 
            ////Once user is validatd, store the endpoint type and signed in state
            // * Insert the last active IP
            //* in state table and return 200OK
            //* If RESOURCEId doesn't exist, error out and indicate that the user needs to 
            //* sign up.
            // * */

            //DBConnection conn = DBConnectionManager.GetInstance().Allocate();
            //try
            //{
            //   rid = conn.VerifyUserExists(siui.username);
               
            //}
            //catch (System.Exception ex)
            //{
            //    System.Diagnostics.Trace.TraceError("Error, trying to verify user login information.");
            //    httpStatusCode = HttpStatusCode.Forbidden;
            //    exStr = "Error, trying to verify user login information.\n Exception: " + ex.ToString();
            //    abort = true;
            //}

            //if (null == rid)
            //{
            //    System.Diagnostics.Trace.TraceError("user login information not found.  please consider registering with us.");                
            //    exStr = "user is forbidden to sign in";
            //    Request.CreateErrorResponse(HttpStatusCode.Forbidden, exStr);
            //    abort = true;
            //}

           

            //if (!abort)
            //{
            //    Task.WaitAll();
            //    //Get client IP, port and device type
            //    srcHttp_IP = HttpContext.Current.Request.UserHostAddress;
            //    srcHttp_Port = Request.RequestUri.Port;
            //    bool isMobileDevice = HttpContext.Current.Request.Browser.IsMobileDevice;                
            //    //TODO: Get USER AGNT STRING FROM CLIENT.
            //    try
            //    {
            //        //The user exists in the DB. Let's store their endpoint information and 
            //        //return a 200 Ok back to the user.
            //        newEpid = conn.InsertIntoResourceEndpointInfoTable(srcHttp_IP, srcHttp_Port,
            //            rid, isMobileDevice);
            //        if (null == newEpid)
            //        {
            //            System.Diagnostics.Trace.TraceError("Error inserting endpoint information into DB.");                    
            //            exStr = "Could not complete operation on inserting endpoint information into DB.";
            //            rsp = Request.CreateErrorResponse(HttpStatusCode.InternalServerError,exStr);
            //            abort = true;
            //        }
            //    }
            //    catch (System.Exception ex)
            //    {
            //        System.Diagnostics.Trace.TraceError("Exception, trying to insert user endpoint information into DB.");
            //        httpStatusCode = HttpStatusCode.InternalServerError;
            //        exStr = "Exception, trying to insert user endpoint information into DB.\nException: " + ex.ToString();
            //        rsp = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exStr);
            //        abort = true;
            //    }
            //}            
          
            ////Insert signed in state
            //if (!abort)
            //{
            //    try
            //    {
            //        Task<bool> tt = conn.InsertIntoResourceEndpointActivityHistory(newEpid);
            //        if (!tt.Result)
            //        {
            //            System.Diagnostics.Trace.TraceError("Error inserting endpoint state into EndpointState DB table.");
            //            exStr = "Could not complete operation on inserting endpoint state into EndpointState DB table.";
            //            rsp = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exStr);
            //            abort = true;
            //        }
            //    }
            //    catch (System.Exception ex)
            //    {
            //        System.Diagnostics.Trace.TraceError("Exception inserting endpoint state into EndpointState DB table.");
            //        exStr = "Exception inserting endpoint state into EndpointState DB table.\nException: " + ex.ToString();
            //        rsp = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exStr);
            //        abort = true;
            //    }
            //}                           

            ////Create response if no errors have occured.
            //if (!abort)
            //{
            //    rsp = Request.CreateResponse(HttpStatusCode.OK);
            //}

            ////deallocated db connecton object.
            //DBConnectionManager.GetInstance().Deallocate(conn);

            //return rsp;
        

        

        public HttpResponseMessage TestInsertResource(uint numresources)
        {
            //Deserialize into object.
            HttpResponseMessage rsp             = null;
            string exStr                        = String.Empty;
            bool abort                          = false;            

            //Deserialize content into SignIn object.
            DBConnectionManager  dbConnMgr      = DBConnectionManager.GetInstance();
            DBConnection conn = dbConnMgr.Allocate();
            try
            {
                for (int i = 0; i < numresources && !abort;++i)
                {
                    abort = conn.CreateResource("test"+i+"@test.com");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Exception, trying to insert resource.");
                exStr = "Exception, trying to insert resource.\n Exception: " + ex.ToString();
                rsp = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exStr);                                
            }

            if (abort)
            {
                System.Diagnostics.Trace.TraceError("Error, trying to insert resource.");
                exStr = "Error, trying to insert resource.";
                rsp = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exStr);      
            }

            return rsp;         

        }
    }
}
