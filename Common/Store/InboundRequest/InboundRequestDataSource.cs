using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;


namespace com.web.server.weconnect.store.inboundrequest
{
    /// <summary>
    /// The GuestBookEntryDataSource class allows for the creation of data 
    /// source objects that can be bound to ASP.NET data controls. These 
    /// controls enable the user to perform Create, Read, Update and Delete 
    /// (CRUD) operations. 
    /// </summary>
    public class InboundRequestEntityDataSource
    {
        // Storage services account information.
        private static CloudStorageAccount _storageAccount;

        // Context that allows the use of Windows Azure Table Services.
        private InboundRequestDataContext _context;

        /// <summary>
        /// Initializes storage account information and creates a table 
        /// using the defined context.
        /// <remarks>
        /// This constructor creates a table using the schema (model) 
        /// defined by the
        /// GuestBookDataContext class and the storage account information 
        /// contained in the configuration connection string settings.
        /// Declaring the constructor static assures that the initialization 
        /// tasks are performed only once.
        /// </remarks>
        /// </summary>
        static InboundRequestEntityDataSource()
        {
            // Create a new instance of a CloudStorageAccount object from a specified configuration setting. 
            // This method may be called only after the SetConfigurationSettingPublisher 
            // method has been called to configure the global configuration setting publisher.
            // You can call the SetConfigurationSettingPublisher method in the OnStart method
            // of the web or worker role before calling FromConfigurationSetting.
            // If you do not do this, the system raises an exception.
            //string cxnString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedDatingStorageConnectonString");
            //Microsoft.WindowsAzure.Storage.CloudStorageAccount account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(cxnString);
            _storageAccount =
                CloudStorageAccount.FromConfigurationSetting("SpeedDatingStorageConnectonString");

            // Create table using the schema (model) defined by the
            // GuestBookDataContext class and the storage account information. 
            CloudTableClient.CreateTablesFromModel(
                typeof(InboundRequestDataContext),
                _storageAccount.TableEndpoint.AbsoluteUri,
                _storageAccount.Credentials);
        }

        /// <summary>
        /// Initialize context used to access table storage and the retry policy.
        /// </summary>
        public InboundRequestEntityDataSource()
        {
            // Initialize context using account information.
            _context =
                new InboundRequestDataContext(_storageAccount.TableEndpoint.AbsoluteUri,
            _storageAccount.Credentials);
            // Initialize retry update policy.
            _context.RetryPolicy = RetryPolicies.Retry(3,
            TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets the contents of the GuestBookentry table.
        /// </summary>
        /// <returns>
        /// results: the GuestBookEntry table contents.
        /// </returns>
        /// <remarks>
        /// This method retrieves today guest book entries by using the current
        /// date as the partition key. The web role uses this method to 
        /// bind the results to a data grid to display the guest book.
        /// </remarks>
        public IEnumerable<InboundRequestTableEntity> Select()
        {
            var results = from g in _context.InboundRequestTableEntry
                          where g.PartitionKey.ToString().Equals(DateTime.UtcNow.ToString("MMddyyyy"))
                          select g;
            return results;
        }

        /// <summary>
        /// Insert new entries in the InboundRequestTableEntry table.
        /// </summary>
        /// <param name="newItem"></param>
        public void AddInboundRequestTableEntry(InboundRequestTableEntity newItem)
        {

            _context.AddObject("InboundRequestTableEntity", newItem);
            _context.SaveChanges();
        }

        ///// <summary>
        ///// Update the thumbnail URL for a table entry.
        ///// </summary>
        ///// <param name="partitionKey"></param>
        ///// <param name="rowKey"></param>
        ///// <param name="thumbUrl"></param>
        //public void UpdateImageThumbnail(string partitionKey,
        //    string rowKey, string thumbUrl)
        //{
        //    var results = from g in _context.InboundRequestTableEntry
        //                  where g.PartitionKey == partitionKey && g.RowKey ==
        //        rowKey
        //                  select g;

        //    var entry = results.FirstOrDefault<InboundRequestTableEntry>();
        //    entry.ThumbnailUrl = thumbUrl;
        //    _context.UpdateObject(entry);
        //    _context.SaveChanges();
        //}
    }

}
