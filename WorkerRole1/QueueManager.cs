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

namespace com.server.speeddating.ResourceWorkerRole.StorageManager
{
    public class QueueManager
    {
        public CloudQueue ResourceQueueClient { get; set; }
        public CloudQueue OutboundQueueClient { get; set; }
        
        public void InitQueues()
        {
            //string cxnString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedaDatingEmulatorConnectionString");
            string cxnString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedDatingStorageConnectonString");
            Microsoft.WindowsAzure.Storage.CloudStorageAccount account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(cxnString);
            //Should we create tables?            
            CloudQueueClient queueClient = account.CreateCloudQueueClient();
            ResourceQueueClient = queueClient.GetQueueReference(StringConstants.RESOURCE_QUEUE_NAME);
            ResourceQueueClient.CreateIfNotExists();

            //Create outbound queue to that worker roles have a way to return response to web role.
            OutboundQueueClient = queueClient.GetQueueReference(StringConstants.OUTBOUND_QUEUE_NAME);
            OutboundQueueClient.CreateIfNotExists();
        }
    }
}
