using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using com.web.server.weconnect.weconnectwebrole.models;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System.Data.Services.Common;
using Microsoft.WindowsAzure.Storage.Queue;

namespace com.web.server.weconnect.weconnectwebrole
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
             try
            {
                AreaRegistration.RegisterAllAreas();                
                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                this.InitQueues();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        
                //Register DB Connections            
                //DBConnectionManager dbConnMgr       = DBConnectionManager.GetInstance();
                //dbConnMgr.OpenConnectionsAsync();

                //ConfigureEventLogging();
            }
         
        /// <summary>
        /// 
        /// </summary>
        protected void ConfigureEventLogging()
        {
            DiagnosticMonitorConfiguration diagConfig = new DiagnosticMonitorConfiguration();
            diagConfig.WindowsEventLog.DataSources.Add("System!*");
            diagConfig.WindowsEventLog.DataSources.Add("Application!*");
            diagConfig.WindowsEventLog.DataSources.Add("weConnect*");
            diagConfig.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(1.0);
            diagConfig.WindowsEventLog.ScheduledTransferLogLevelFilter = LogLevel.Error;
            //DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", diagConfig);            
            
        }

        public void InitQueues()
        {
            //string cxnString = CloudConfigurationManager.GetSetting("SpeedDatingStorageEmulatorConnectonString");
            string cxnString = Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SpeedDatingStorageConnectonString");

             Microsoft.WindowsAzure.Storage.CloudStorageAccount account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(cxnString);
            //Should we create tables?            
             Microsoft.WindowsAzure.Storage.Queue.CloudQueueClient queueClient = account.CreateCloudQueueClient();            
            var queue = queueClient.GetQueueReference("resourcequeue");
            queue.CreateIfNotExists();
        }
        
    }
}