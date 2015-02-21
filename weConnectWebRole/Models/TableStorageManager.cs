using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System.Data.Services.Common;

namespace com.web.server.weConnect.models
{
    public static class TableStorageManager
    {
        private CloudTableClient _tableClient = null;
        private object _syncObj = new object();
        public static TableStorageManager();
        //private string cxnString = CloudConfigurationManager.GetSetting("SpeedDatingStorageConnectonString");
        private string cxnString = CloudConfigurationManager.GetSetting("SpeedaDtingEmulatorConnectionString");
        //private string _cxnString = "DefaultEndpointsProtocol=https;AccountName=michajohstorage01;AccountKey=7Rjk8ncfara/3GvM69FATHI3PwgIvMXTBdTR0Z9JYy3OKUl0jrpxEHVyxGT9gpFF7f4u1EvtZNGrjzSaUK/I5g==";
        //private string _emulatorCxnString = "UseDevelopmentStorage=true";


        /// <summary>
        /// 
        /// </summary>
        public void InitializeTable()
        {
            // Connection string is of the format:             
            CloudStorageAccount account = CloudStorageAccount.Parse(cxnString);
            //Should we create tables?            
            _tableClient = account.CreateCloudTableClient();
            TableServiceContext context = _tableClient.GetDataServiceContext();
            _tableClient.CreateTableIfNotExist("Resource");
            _tableClient.CreateTableIfNotExist("Endpoint");


            // Create Movie Table string tableName = "Movies";
        }

        /// <summary>
        /// 
        /// </summary>
        public TableServiceContext TableContext
        {
            get
            {
                return _tableClient.GetTableServiceContext();
            }
        }
    }
}


   
   



// Add movie object to the context context.AddObject(tableName, new Movie("Action", "White Water Rapids Survival"));


// The object is not sent to the Table service until SaveChanges is // called. SaveChangesWithRetries wraps the SaveChanges but as the name // suggest, it also provides retries. context.SaveChangesWithRetries();

// We should use a new DataServiceContext for this operation // but for brevity, we will skip this best practice in the code snippet // Query for action movies that are rated > 4 var q = (from movie in context.CreateQuery<Movie>(tableName) 
     //   where movie.PartitionKey == "Action" && movie.Rating > 4.0
       /* select movie).AsTableServiceQuery<Movie>();


// Make each of the movie that is returned in the result set my favorite // Using the AsTableServiceQuery extension above means that the below // iteration handles continuation tokens since this is not a single point query. // See Queries section for more details on query efficiency and continuation tokens. foreach (Movie movieToUpdate in q)
{
    movieToUpdate.Favorite = true;

    // This sets the entity to be updated in the // context and no request is sent until SaveChanges is called. This // issues an update with optimistic concurrency check. With the above query, // the context tracks this entity with the associated Etag value. The following // update will set the If-Match header such that entity is updated only if etag // matches with the entity representation on server. context.UpdateObject(movieToUpdate);
}

// The batch SaveChangesOptions ensures atomic transaction for all updates context.SaveChangesWithRetries(SaveChangesOptions.Batch);
}*/