using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using System.Data.Services;
using com.web.server.weconnect.messages;
using com.web.server.weconnect.serviceutilities;
using com.web.server.weconnect.resourceworkerrole;




namespace com.web.server.weconnect.store
{
    public class QueueManager
    {
        //public static CloudQueue ResourceQueueClient { get; set; }

        //public static void InitQueues()
        //{
        //    string cxnString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedaDatingEmulatorConnectionString");
        //    Microsoft.WindowsAzure.Storage.CloudStorageAccount account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(cxnString);
        //    //Should we create tables?            
        //    CloudQueueClient queueClient = account.CreateCloudQueueClient();
        //    ResourceQueueClient = queueClient.GetQueueReference("ResourceQueue");
        //    ResourceQueueClient.CreateIfNotExists();   

        public static CloudQueue ResourceQueueClient { get; set; }
        public static CloudQueue OutboundQueueClient { get; set; }

        public static void InitQueues()
        {
            //string cxnString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedaDatingEmulatorConnectionString");
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("SpeedDatingStorageConnectonString"));

            string cxnString = CloudConfigurationManager.GetSetting("SpeedDatingStorageConnectonString");
            Microsoft.WindowsAzure.Storage.CloudStorageAccount account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(cxnString);
            //Should we create tables?            
            CloudQueueClient resourceQueueClient = account.CreateCloudQueueClient();
            ResourceQueueClient = resourceQueueClient.GetQueueReference(StringConstants.RESOURCE_QUEUE_NAME);
            ResourceQueueClient.CreateIfNotExists();

            //Create outbound queue to that worker roles have a way to return response to web role.
            CloudQueueClient outboundQueueClient = account.CreateCloudQueueClient();
            OutboundQueueClient = outboundQueueClient.GetQueueReference(StringConstants.OUTBOUND_QUEUE_NAME);
            OutboundQueueClient.CreateIfNotExists();
        }
    }
}