using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
//using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using System.Data.Services.Common;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage;
using com.web.server.weconnect.resourceworkerrole.storagemanager;
using com.web.server.weconnect.resourceworkerrole.messagehandlers;
using com.web.server.weconnect.messages;
using com.web.server.weconnect.serviceutilities;

namespace com.web.server.weconnect.resourceworkerrole.storagemanager
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
