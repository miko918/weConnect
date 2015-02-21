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
    public class MessageBase
    {
        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public virtual string CallId { get; set; }

        [DataMember]
        public string MessageType { get; set; }

        [DataMember]
        public string RequestType { get; set; }

        [DataMember]
        public string Action { get; set; }
    }
}
