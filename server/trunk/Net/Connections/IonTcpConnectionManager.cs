using System;
using System.Collections.Generic;

using Ion.HabboHotel.Client;

namespace Ion.Net.Connections
{
    /// <summary>
    /// Manages accepted IonTcpConnections and enables them to interact with the Ion environment.
    /// </summary>
    public class IonTcpConnectionManager
    {
        #region Fields
        /// <summary>
        /// A 32 bit integer that holds the maximum amount of connections in the connection manager.
        /// </summary>
        private readonly int mMaxSimultaneousConnections;
        /// <summary>
        /// A System.Collections.Generic.Dictionary with client IDs as keys and IonTcpConnections as values. This collection holds active IonTcpConnections.
        /// </summary>
        private Dictionary<uint, IonTcpConnection> mConnections;
        private IonTcpConnectionListener mListener;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs an instance of IonTcpConnectionManager, constructs an IonTcpConnectionListener, binds it to a given local IP and TCP port and sets the maximum amount of connections.
        /// </summary>
        /// <param name="sLocalIP">The local IP address to bind the listener to.</param>
        /// <param name="Port">The TCP port number to bind the listener to.</param>
        /// <param name="maxSimultaneousConnections">The maximum amount of connections in the connection manager.</param>
        public IonTcpConnectionManager(string sLocalIP, int Port, int maxSimultaneousConnections)
        {
            int initialCapacity = maxSimultaneousConnections;
            if (maxSimultaneousConnections > 4)
                initialCapacity /= 4; // Set 1/4 of max connections as initial capacity to avoid too much resizing

            mConnections = new Dictionary<uint, IonTcpConnection>(initialCapacity);
            mMaxSimultaneousConnections = maxSimultaneousConnections;

            mListener = new IonTcpConnectionListener(sLocalIP, Port, this);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Destroys all resources in the connection manager.
        /// </summary>
        public void DestroyManager()
        {
            mConnections.Clear();
            mConnections = null;
            mListener = null;
        }

        /// <summary>
        /// Returns true if the connection collection currently contains a connection with a given client ID.
        /// </summary>
        /// <param name="clientID">The client ID to check.</param>
        public bool ContainsConnection(uint clientID)
        {
            return mConnections.ContainsKey(clientID);
        }
        /// <summary>
        /// Tries to return the IonTcpConnection instance of a given client ID. Null is returned if the connection is not in the manager.
        /// </summary>
        /// <param name="ID">The ID of the client to get connection of as an unsigned 32 bit integer.</param>
        public IonTcpConnection GetConnection(uint clientID)
        {
            try { return mConnections[clientID]; }
            catch { return null; }
        }
        /// <summary>
        /// Returns the IonTcpConnection listener instance.
        /// </summary>
        public IonTcpConnectionListener GetListener()
        {
            return mListener;
        }

        /// <summary>
        /// Handles a newly created IonTcpConnection and performs some checks, before adding it to the connection collection and starting the client session.
        /// </summary>
        /// <param name="connection">The IonTcpConnection instance representing the new connection to handle.</param>
        public void HandleNewConnection(IonTcpConnection connection)
        {
            // TODO: check max simultaneous connections
            // TODO: check max simultaneous connections per IP
            // TODO: check project specific actions

            // INFO: client ID = connection ID, client ID = session ID
            // Add connection to collection
            mConnections.Add(connection.ID, connection);

            // Create session for new client
            IonEnvironment.GetHabboHotel().GetClients().StartClient(connection.ID);
        }
        public void DropConnection(uint clientID)
        {
            IonTcpConnection connection = GetConnection(clientID);
            if (connection != null)
            {
                IonEnvironment.GetLog().WriteInformation("Dropped IonTcpConnection [" + clientID + "] of " + connection.ipAddress);
                
                connection.Stop();
                mConnections.Remove(clientID);
            }
        }
        public bool TestConnection(uint clientID)
        {
            IonTcpConnection connection = GetConnection(clientID);
            if (connection != null)
                return connection.TestConnection(); // Try to send data

            return false; // Connection not here!
        }
        #endregion
    }
}
