using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System.Data.Services.Common;

namespace com.web.server.weconnect.weconnectwebrole.models.entities
{
    [DataServiceKey("Hash_firstletter_AndDomain", "username")]
    public class EndpointActivityHistory
    {
        /// Movie Category is the partition key public string PartitionKey { get; set; }
        /// Movie Title is the row key public string RowKey { get; set; }
        public string LastActiveIP { get; set; }
        public short LastActvePort { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}