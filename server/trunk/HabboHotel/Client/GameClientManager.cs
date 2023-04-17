using System;
using System.Threading;
using System.Collections.Generic;

using Ion.Net.Connections;
using Ion.Net.Messages;

using Ion.HabboHotel.Habbos;

namespace Ion.HabboHotel.Client
{
    /// <summary>
    /// Manages connected clients, checks their pings and manages the Habbo they are logged in on.
    /// </summary>
    public class GameClientManager
    {
        #region Fields
        private Thread mConnectionChecker;
        private Dictionary<uint, GameClient> mClients;
        #endregion

        #region Constructor
        public GameClientManager()
        {
            mClients = new Dictionary<uint, GameClient>();
        }
        #endregion

        #region Methods
        public void Clear()
        {
            mClients.Clear();
        }

        public void StartConnectionChecker()
        {
            if (mConnectionChecker == null)
            {
                mConnectionChecker = new Thread(TestClientConnections);
                mConnectionChecker.Priority = ThreadPriority.BelowNormal;

                mConnectionChecker.Start();
            }
        }
        public void StopConnectionChecker()
        {
            if (mConnectionChecker != null)
            {
                try { mConnectionChecker.Abort(); }
                catch { }

                mConnectionChecker = null;
            }
        }
        private void TestClientConnections()
        {
            int interval = IonEnvironment.Configuration.TryParseInt32("projects.habbo.server.pinginterval");
            while (true)
            {
                try
                {
                    // Prepare PING data
                    byte[] PINGDATA = new ServerMessage(ResponseOpcodes.Ping).GetBytes(); // "@r"
                    
                    // Gather timed out clients and reset ping status for in-time clients
                    List<uint> timedOutClients = new List<uint>();
                    lock (mClients)
                    {
                        foreach (GameClient client in mClients.Values)
                        {
                            // Get current status and flip pong status
                            if (client.PingOK)
                            {
                                client.PingOK = true;
                                client.GetConnection().SendData(PINGDATA);
                            }
                            else
                            {
                                timedOutClients.Add(client.ID);
                            }
                        }

                        // Stop the gathered timed out clients
                        foreach (uint timedOutClientID in timedOutClients)
                        {
                            this.StopClient(timedOutClientID);
                        }
                    }

                    // Sleep for 30 seconds and repeat!
                    Thread.Sleep(interval);
                }
                catch (ThreadAbortException) { } // Nothing special!
            }
        }

        public GameClient GetClient(uint clientID)
        {
            if (mClients.ContainsKey(clientID))
            {
                return mClients[clientID];
            }
            else
            {
                return null;
            }

        }
        public bool RemoveClient(uint clientID)
        {
            return mClients.Remove(clientID);
        }

        public void StartClient(uint clientID)
        {
            GameClient client = new GameClient(clientID);
            mClients.Add(clientID, client);

            client.StartConnection();
        }
        public void StopClient(uint clientID)
        {
            GameClient client = GetClient(clientID);
            if (client != null)
            {
                // Stop & drop connection
                IonEnvironment.GetTcpConnections().DropConnection(clientID);

                // Stop client
                client.Stop();

                // Drop client
                mClients.Remove(clientID);

                // Log event
                IonEnvironment.GetLog().WriteInformation("Stopped client " + clientID);
            }
        }

        public GameClient GetClientOfHabbo(uint accountID)
        {
            lock (mClients)
            {
                foreach (GameClient client in mClients.Values)
                {
                    if (client.GetHabbo() != null && client.GetHabbo().ID == accountID)
                    {
                        return client;
                    }
                }
            }

            return null;
        }
        public uint GetClientIdOfHabbo(uint accountID)
        {
            GameClient client = this.GetClientOfHabbo(accountID);
            
            return (client != null) ? client.ID : 0;
        }

        public void KillClientOfHabbo(uint accountID)
        {
            uint clientID = this.GetClientIdOfHabbo(accountID);
            if (clientID > 0)
            {
                this.StopClient(clientID);
            }
        }
        #endregion
    }
}
