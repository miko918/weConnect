using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System.Data.Services;
using System.IO;
using com.web.server.weconnect.serviceutilities;

namespace com.web.server.weconnect.store
{
    public abstract class TableManager
    {
        private CloudTableClient _tableClient = null;
        private string _cxnString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedDatingStorageConnectonString");
        //private string _cxnString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedaDatingEmulatorConnectionString");
        //private string _cxnString = "DefaultEndpointsProtocol=https;AccountName=michajohstorage01;AccountKey=7Rjk8ncfara/3GvM69FATHI3PwgIvMXTBdTR0Z9JYy3OKUl0jrpxEHVyxGT9gpFF7f4u1EvtZNGrjzSaUK/I5g==";
        //private string _emulatorCxnString = "UseDevelopmentStorage=true";

        private const string RESOURCE_TABLENAME = "Resource";
        private const string INBOUND_REQUEST_TABLENAME = "InboundRequest";
        private const string ENDPOINTINFO_TABLENAME = "EndpointInfo";
        private const string RESOURCEENDPOINTACTIVITYHISTORY_TABLENAME = "ResoureceEndpointActivityHistory";
        private const string REQUEST_TABLENAME = "Request";



        /// <summary>
        /// 
        /// </summary>
        private CloudTable InboundRequestTable { get; set; }
        private CloudTable ResourceTable { get; set; }
        private CloudTable EndpointInfoTable { get; set; }
        private CloudTable ResoureceEndpointActivityHistoryTable { get; set; }
        private CloudTable RequestTable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TableManager()
        {
            InitializeTables();
        }

        /// <summary>
        /// Creates all the tables if they don't exist.
        /// </summary>
        public void InitializeTables()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(_cxnString);
            //Should we create tables?            
            _tableClient = account.CreateCloudTableClient();

            // Create Ttable if they don't exist.
            InboundRequestTable = _tableClient.GetTableReference(StringConstants.INBOUND_REQUEST_TABLENAME);
            InboundRequestTable.CreateIfNotExists();

            //EndpointInfoTable = _tableClient.GetTableReference(ENDPOINTINFO_TABLENAME);
            //EndpointInfoTable.CreateIfNotExists();

            //ResoureceEndpointActivityHistoryTable = _tableClient.GetTableReference(RESOURCEENDPOINTACTIVITYHISTORY_TABLENAME);
            //ResoureceEndpointActivityHistoryTable.CreateIfNotExists();

            //RequestTable = _tableClient.GetTableReference(REQUEST_TABLENAME);
            //RequestTable.CreateIfNotExists();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public virtual void AddDataToTable(string data)
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
    }
}