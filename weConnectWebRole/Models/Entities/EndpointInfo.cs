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
    /// <summary>
    /// 
    /// </summary>
    public enum ClientType
    {
        IE = 0,
        IPHONE,
        WINDOWS_PHONE,
        ANDROID,
        WINDOWS
    };

    /// <summary>
    /// 
    /// </summary>
    [DataServiceKey("HttpIP", "Port")]
    public class EndpointInfo
    {
        /// Movie Category is the partition key public string PartitionKey { get; set; }
        /// Movie Title is the row key public string RowKey { get; set; }
        public Guid ResourceId { get; set; }
        public bool IsMobile { get; set; }
        public string Subnet { get; set; }
        public ClientType ClientEndpointType { get; set; }

    }
}