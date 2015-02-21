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
    [DataServiceKey("HashDomain", "Username")]
    public class Resource
    {
        /// Movie Category is the partition key public string PartitionKey { get; set; }
        /// Movie Title is the row key public string RowKey { get; set; }
        public string HashDomain { get; set; }
        public Guid ResourceId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin    { get; set; }        
    }
}