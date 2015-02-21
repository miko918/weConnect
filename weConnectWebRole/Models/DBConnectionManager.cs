using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;


namespace com.web.server.weconnect.weconnectwebrole.models
{

    /// <summary>
    /// Summary description for DBConnectionManager
    /// </summary>
    public class DBConnectionManager
    {
        private static DBConnectionManager _dbConnMgr = null;
        private static int _defaultPoolSize = 100;
        private static int _actualPoolSize = _defaultPoolSize;
        private static uint _allocatedCnt = 0;

        /// <summary>
        /// Lock object to protect sync issues.
        /// </summary>
        private static object _syncLock = new object();
        //Conneciton pool list.
        private static Queue<DBConnection> _availableDBConnList = null;
        private static Queue<DBConnection> _allocatedDBConnList = null;

        /// <summary>
        /// Private constructor to create the DB connection pool.
        /// </summary>
        static DBConnectionManager()
        {
            DBConnectionManager._availableDBConnList    = new Queue<DBConnection>(DBConnectionManager._defaultPoolSize);
            DBConnectionManager._actualPoolSize         = DBConnectionManager._defaultPoolSize;
            DBConnectionManager._allocatedDBConnList    = new Queue<DBConnection>(0);

            for (int i = 0; i < _defaultPoolSize; ++i)
            {
                DBConnection tmpDbConn = new DBConnection();
                _availableDBConnList.Enqueue(tmpDbConn);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task OpenConnectionsAsync()
        { 
            foreach (DBConnection dbConn in  _availableDBConnList)
            {
                DBConnection tmpDbConn = new DBConnection();
                await dbConn.OpenConnectionAsync();               
            }
        }

        /// <summary>
        /// Singleton GetInstance method.
        /// </summary>
        /// <returns></returns>
        public static DBConnectionManager GetInstance()
        {
            lock (_syncLock)
            {
                if (null == DBConnectionManager._dbConnMgr)
                {
                    //Create the DBMgr object.
                    DBConnectionManager._dbConnMgr = new DBConnectionManager();                    
                }
            }

            return DBConnectionManager._dbConnMgr;
        }        

        /// <summary>
        /// Singleton GetInstance method.
        /// </summary>
        /// <returns></returns>
        public static DBConnectionManager GetInstance(int poolSize)
        {
            lock (_syncLock)
            {
                if (null == DBConnectionManager._dbConnMgr)
                {
                    //Create the DBMgr object.
                    DBConnectionManager._dbConnMgr = new DBConnectionManager();
                }
            }

            return DBConnectionManager._dbConnMgr;
        }

        /// <summary>
        /// enqueue connection back on queue
        /// </summary>
        /// <param name="conn"></param>
        /// <returns>false: when deallocating when deallocaiton list if full</returns>
        public bool Deallocate(DBConnection conn)
        {
            bool rtn = true;
            if (conn != null)
            {
                if (0 == _allocatedCnt)
                {
                    System.Diagnostics.Debug.WriteLine("Cannot deallocate object as list is full");
                    rtn = false;
                }
                DBConnectionManager._availableDBConnList.Enqueue(conn);
                _allocatedCnt--;
            }
            else
            {
                throw new System.Exception("Exception: Cannot deallocate DB connection object that is null");
            }

            return rtn;
        }

        public DBConnection Allocate()
        {
            if (_allocatedCnt == _actualPoolSize)
            {
                System.Diagnostics.Debug.WriteLine("Cannot Allocate object as list is empty.  SHOULD WE RESIZE??");
                return null;
            }
            _allocatedCnt++;
            return DBConnectionManager._availableDBConnList.Dequeue();
        }


        /// <summary>
        /// Enaqueueu all allocated connection strings to available queue.
        /// cleanup all available queue and dispose each connection object.
        /// </summary>
        public void Dispose()
        {
            foreach (DBConnection conn in DBConnectionManager._allocatedDBConnList)
            {
                DBConnectionManager._availableDBConnList.Enqueue(conn);
            }

            foreach (DBConnection conn in DBConnectionManager._availableDBConnList)
            {
                conn.Dispose();
            }
        }
    }
}


  