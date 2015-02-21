using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;


namespace com.web.server.weconnect.store.inboundrequest
{
    class InboundRequestDataContext : TableServiceContext
    {

        /// <summary>
        /// Create an instance of the GuestBookDataContext class 
        /// and initialize the base
        /// class with storage access information.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="credentials"></param>
        public InboundRequestDataContext(string baseAddress,
            StorageCredentials credentials)
            : base(baseAddress, credentials)
        { }

        /// <summary>
        /// Define the property that returns the GuestBookEntry table.
        /// </summary>
        public IQueryable<InboundRequestTableEntity> InboundRequestTableEntry
        {
            get
            {
                return this.CreateQuery<InboundRequestTableEntity>("InboundRequestTableEntity");
            }
        }
    }

    
}
