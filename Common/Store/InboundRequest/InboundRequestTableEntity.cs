using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.StorageClient;
using System.Net.Http;

namespace com.web.server.weconnect.store.inboundrequest
{
    public class InboundRequestTableEntity : TableServiceEntity
    {
        /// <summary>
        /// Create a table entry
        /// </summary>
        /// <param name="partitionKey">Msg from user login info is the partition key</param>
        /// <param name="rowKey">Call ID</param>        
        public InboundRequestTableEntity(string monthDayYear, string rowKey)
        {
            // The partition key allows partitioning the data so that
            // there is a separate partition for each day of guest 
            // book entries. The partition key is used to assure 
            // load balancing for data access across fabric nodes (servers).
            PartitionKey = monthDayYear;
            //DateTime.UtcNow.ToString("MMddyyyy");
            //Row Key is the callid and from uri. <callId;FromUri
            RowKey = rowKey;
        }

        public InboundRequestTableEntity()
        {
            PartitionKey = System.DateTime.UtcNow.ToString("MMddyyy");
            RowKey = String.Empty;
           
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] RequestMessage { get; set; }
        /// <summary>
        /// The from user's uri.
        /// </summary>
        public string PartitionKey { get; set; }
        /// <summary>
        /// The Request call ID
        /// </summary>    
        public string RowKey { get; set; }
        //public string GuestName { get; set; }
        //public string PhotoUrl { get; set; }
        //public string ThumbnailUrl { get; set; }
    }
}
