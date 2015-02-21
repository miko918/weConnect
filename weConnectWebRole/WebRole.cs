using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using com.web.server.weconnect.weconnectwebrole.models;
using com.web.server.weconnect.store;
using System.Threading;
using System.Net.Http;


namespace com.web.server.weconnect
{
    public class WebRole : RoleEntryPoint
    {
        //public static OutboundRoutingHandler _obrh = new OutboundRoutingHandler();
        public static Dictionary<int,HttpRequestMessage> s_httpRequestTable = new Dictionary<int, HttpRequestMessage>();
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            //_rtm.InitializeTables();
            QueueManager.InitQueues();
            //StartOutboundRoutingThread();
            return base.OnStart();
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartOutboundRoutingThread()
        {
            //OutboundRoutingHandler obrh = new OutboundRoutingHandler();

            // Create the thread object, passing in the Alpha.Beta method
            // via a ThreadStart delegate. This does not start the thread.
            //Thread oThread = new Thread(new ThreadStart(obrh.Run));


            try
            {
                Console.WriteLine("Try to restart the Alpha.Beta thread");
                //oThread.Start();
            }
            catch (ThreadStateException)
            {
                Console.Write("ThreadStateException trying to restart Alpha.Beta. ");
                Console.WriteLine("Expected since aborted threads cannot be restarted.");
            }
        }
    }
}
