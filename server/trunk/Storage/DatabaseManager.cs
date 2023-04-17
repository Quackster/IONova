using System;
using System.Data;
using System.Threading;

using MySql.Data.MySqlClient;

namespace Ion.Storage
{
    /// <summary>
    /// DatabaseManager acts as a proxy towards an encapsulated Database at a DatabaseServer.
    /// </summary>
    public class DatabaseManager
    {
        #region Fields
        private DatabaseServer mServer;
        private Database mDatabase;

        private DatabaseClient[] mClients = new DatabaseClient[0];
        private bool[] mClientAvailable = new bool[0];
        private int mClientStarvationCounter;

        private Thread mClientMonitor;
        #endregion

        #region Properties

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a DatabaseManager for a given DatabaseServer and Database.
        /// </summary>
        /// <param name="pServer">The DatabaseServer for this database proxy.</param>
        /// <param name="pDatabase">The Database for this database proxy.</param>
        public DatabaseManager(DatabaseServer pServer, Database pDatabase)
        {
            mServer = pServer;
            mDatabase = pDatabase;
        }
        /// <summary>
        /// Constructs a DatabaseManager for given database server and database details.
        /// </summary>
        /// <param name="sServer">The network host of the database server, eg 'localhost' or '127.0.0.1'.</param>
        /// <param name="Port">The network port of the database server as an unsigned 32 bit integer.</param>
        /// <param name="sUser">The username to use when connecting to the database.</param>
        /// <param name="sPassword">The password to use in combination with the username when connecting to the database.</param>
        /// <param name="sDatabase">The name of the database to connect to.</param>
        /// <param name="minPoolSize">The minimum connection pool size for the database.</param>
        /// <param name="maxPoolSize">The maximum connection pool size for the database.</param>
        public DatabaseManager(string sServer, uint Port, string sUser, string sPassword, string sDatabase, uint minPoolSize, uint maxPoolSize)
        {
            mServer = new DatabaseServer(sServer, Port, sUser, sPassword);
            mDatabase = new Database(sDatabase, minPoolSize, maxPoolSize);

            mClientMonitor = new Thread(MonitorClientsLoop);
            mClientMonitor.Priority = ThreadPriority.Lowest;

            mClientMonitor.Start();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the client monitor thread. The client monitor disconnects inactive clients etc.
        /// </summary>
        public void StartMonitor()
        {
            mClientMonitor = new Thread(MonitorClientsLoop);
            mClientMonitor.Priority = ThreadPriority.Lowest;

            mClientMonitor.Start();
        }
        /// <summary>
        /// Stops the client monitor thread.
        /// </summary>
        public void StopMonitor()
        {
            if (mClientMonitor != null)
            {
                mClientMonitor.Abort();
            }
        }

        /// <summary>
        /// Disconnects and destroys all database clients.
        /// </summary>
        public void DestroyClients()
        {
            lock (this)
            {
                for (int i = 0; i < mClients.Length; i++)
                {
                    mClients[i].Destroy();
                    mClients[i] = null;
                }
            }
        }
        /// <summary>
        /// Nulls all instance fields of the database manager.
        /// </summary>
        public void DestroyManager()
        {
            mServer = null;
            mDatabase = null;
            mClients = null;
            mClientAvailable = null;

            mClientMonitor = null;
        }

        /// <summary>
        /// Closes the connections of database clients that have been inactive for too long. Connections can be opened again when needed.
        /// </summary>
        private void MonitorClientsLoop()
        {
            while (true)
            {
                try
                {
                    lock (this)
                    {
                        DateTime dtNow = DateTime.Now;
                        for (int i = 0; i < mClients.Length; i++)
                        {
                            if (mClients[i].State != ConnectionState.Closed)
                            {
                                if (mClients[i].Inactivity >= 60) // Not used in the last %x% seconds
                                {
                                    mClients[i].Disconnect(); // Temporarily close connection

                                    IonEnvironment.GetLog().WriteInformation("Disconnected database client #" + mClients[i].Handle);
                                }
                            }
                        }
                    }

                    Thread.Sleep(10000); // 10 seconds
                }
                catch (ThreadAbortException) { } // Nothing special
                catch (Exception ex)
                {
                    IonEnvironment.GetLog().WriteError(ex.Message);
                }
            }
        }
        /// <summary>
        /// Creates the connection string for this database proxy.
        /// </summary>
        public string CreateConnectionString()
        {
            MySqlConnectionStringBuilder pCSB = new MySqlConnectionStringBuilder();

            // Server
            pCSB.Server = mServer.Host;
            pCSB.Port = mServer.Port;
            pCSB.UserID = mServer.User;
            pCSB.Password = mServer.Password;

            // Database
            pCSB.Database = mDatabase.Name;
            pCSB.MinimumPoolSize = mDatabase.minPoolSize;
            pCSB.MaximumPoolSize = mDatabase.maxPoolSize;

            return pCSB.ToString();
        }

        public DatabaseClient GetClient()
        {
            // Let other threads wait if they contact this DatabaseManager while we're busy with it
            lock (this)
            {
                // Try to find an available client
                for (uint i = 0; i < mClients.Length; i++)
                {
                    // Somebody here?
                    if (mClientAvailable[i] == true)
                    {
                        // No starvation anymore
                        mClientStarvationCounter = 0;

                        // Is this connection broken?
                        //if (mClients[i].State == ConnectionState.Broken)
                        //{
                        //    mClients[i] = new DatabaseClient((i + 1), this); // Create new client
                        //}

                        // Is this connection closed?
                        if (mClients[i].State == ConnectionState.Closed)
                        {
                            // TODO: exception handling
                            mClients[i].Connect();

                            IonEnvironment.GetLog().WriteInformation("Opening connection for database client #" + mClients[i].Handle);
                        }

                        // Is this client ready?
                        if (mClients[i].State == ConnectionState.Open)
                        {
                            IonEnvironment.GetLog().WriteLine("Handed out client #" + mClients[i].Handle);

                            mClientAvailable[i] = false; // BRB

                            mClients[i].UpdateLastActivity();
                            return mClients[i];
                        }
                    }
                }

                // No clients?
                mClientStarvationCounter++;

                // Are we having a structural lack of clients?
                if (mClientStarvationCounter >= ((mClients.Length + 1) / 2)) // Database hungry much?
                {
                    // Heal starvation
                    mClientStarvationCounter = 0;

                    // Increase client amount by 0.3
                    SetClientAmount((uint)(mClients.Length + 1 * 1.3f));

                    // Re-enter this method
                    return GetClient();
                }

                DatabaseClient pAnonymous = new DatabaseClient(0, this);
                pAnonymous.Connect();
                
                IonEnvironment.GetLog().WriteLine("Handed out anonymous client.");

                return pAnonymous;
            }
        }
        public void ReleaseClient(uint Handle)
        {
            if (mClients.Length >= (Handle - 1)) // Ensure client exists
            {
                mClientAvailable[Handle - 1] = true;
                IonEnvironment.GetLog().WriteLine("Released client #" + Handle);
            }
        }

        /// <summary>
        /// Sets the amount of clients that will be available to requesting methods. If the new amount is lower than the current amount, the 'excluded' connections are destroyed. If the new connection amount is higher than the current amount, new clients are prepared. Already existing clients and their state will be maintained.
        /// </summary>
        /// <param name="Amount">The new amount of clients.</param>
        public void SetClientAmount(uint Amount)
        {
            lock (this)
            {
                if (mClients.Length == Amount)
                    return;

                if (Amount < mClients.Length) // Client amount shrinks, dispose clients that will die
                {
                    for (uint i = Amount; i < mClients.Length; i++)
                    {
                        mClients[i].Destroy();
                        mClients[i] = null;
                    }
                }

                DatabaseClient[] pClients = new DatabaseClient[Amount];
                bool[] pClientAvailable = new bool[Amount];
                for (uint i = 0; i < Amount; i++)
                {
                    if (i < mClients.Length) // Keep the existing client and it's available state
                    {
                        pClients[i] = mClients[i];
                        pClientAvailable[i] = mClientAvailable[i];
                    }
                    else // We are in need of more clients, so make another one
                    {
                        pClients[i] = new DatabaseClient((i + 1), this);
                        pClientAvailable[i] = true; // Elegant?
                    }
                }

                // Update the instance fields
                mClients = pClients;
                mClientAvailable = pClientAvailable;
            }
        }

        public bool INSERT(IDataObject obj)
        {
            using (DatabaseClient dbClient = GetClient())
            {
                return obj.INSERT(dbClient);
            }
        }

        public bool DELETE(IDataObject obj)
        {
            using (DatabaseClient dbClient = GetClient())
            {
                return obj.DELETE(dbClient);
            }
        }

        public bool UPDATE(IDataObject obj)
        {
            using (DatabaseClient dbClient = GetClient())
            {
                return obj.UPDATE(dbClient);
            }
        }

        public override string ToString()
        {
            return mServer.ToString() + ":" + mDatabase.Name;
        }
        #endregion
    }
}