using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.server.speeddating.ResourceWorkerRole.StorageManager
{
    public class ResourceTableManager: TableManager
    {
        /// <summary>
        /// Methd that will determine which table to add to.
        /// </summary>
        /// <param name="data">Jaon Text</param>
        public override void AddDataToTable(string data)
        {
           //Look at header of data to determine the table that the information eeds to be added to.
           //Create the entity object with partition and rowkey and call teh appropriate method to add to the table.
            //using (JsonTextReader reader = new JsonTextReader(new StringReader(data)))
            //{
            //    while (reader.Read())
            //    {
            //        if (readerTokenClass == JsonTokenClass.String &&
            //            reader.Text.StartsWith("A"))
            //        {
            //            Console.WriteLine(reader.Text);
            //        }
            //    }
            //}


        }

       
            

//// Add movie object to the context context.AddObject(tableName, new Movie("Action", "White Water Rapids Survival"));


//// The object is not sent to the Table service until SaveChanges is // called. SaveChangesWithRetries wraps the SaveChanges but as the name // suggest, it also provides retries. context.SaveChangesWithRetries();

//// We should use a new DataServiceContext for this operation // but for brevity, we will skip this best practice in the code snippet // Query for action movies that are rated > 4 var q = (from movie in context.CreateQuery<Movie>(tableName) 
//        where movie.PartitionKey == "Action" && movie.Rating > 4.0
//        select movie).AsTableServiceQuery<Movie>();


//// Make each of the movie that is returned in the result set my favorite // Using the AsTableServiceQuery extension above means that the below // iteration handles continuation tokens since this is not a single point query. // See Queries section for more details on query efficiency and continuation tokens. foreach (Movie movieToUpdate in q)
//{
//    movieToUpdate.Favorite = true;

//    // This sets the entity to be updated in the // context and no request is sent until SaveChanges is called. This // issues an update with optimistic concurrency check. With the above query, // the context tracks this entity with the associated Etag value. The following // update will set the If-Match header such that entity is updated only if etag // matches with the entity representation on server. context.UpdateObject(movieToUpdate);
//}

//// The batch SaveChangesOptions ensures atomic transaction for all updates context.SaveChangesWithRetries(SaveChangesOptions.Batch);
//        }

        /// Methd that will determine which table to add to.
        /// </summary>
        /// <param name="data"></param>
        public void AddDataToEndpointInfoTable(string data)
        {
        }

         /// Methd that will determine which table to add to.
        /// </summary>
        /// <param name="data"></param>
        public void AddDataToResourceEndpointActivityHistoryTable(string data)
        {
        }
        
    }
    
}
