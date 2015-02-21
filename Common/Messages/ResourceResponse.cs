using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace com.web.server.weconnect.messages
{
     [DataContract]
    public class ResourceResponse: MessageBase
    {
        [DataMember]
        public string Status { get; set; }
    }
}
