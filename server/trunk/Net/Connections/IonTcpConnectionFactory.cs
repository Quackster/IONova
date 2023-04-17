using System;
using System.Net.Sockets;

namespace Ion.Net.Connections
{
    /// <summary>
    /// A factory for creating IonTcpConnections.
    /// </summary>
    public class IonTcpConnectionFactory
    {
        #region Fields
        /// <summary>
        /// A 32 bit unsigned integer that is incremented everytime a connection is added.
        /// </summary>
        private uint mConnectionCounter;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the total amount of created connections.
        /// </summary>
        public uint Count
        {
            get { return mConnectionCounter; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates an IonTcpConnection instance for a given socket and assigns it an unique ID.
        /// </summary>
        /// <param name="pSocket">The System.Net.Socket.Sockets object to base the connection on.</param>
        /// <returns>IonTcpConnection</returns>
        public IonTcpConnection CreateConnection(Socket pSocket)
        {
            if (pSocket == null)
                return null;

            IonTcpConnection connection = new IonTcpConnection(++mConnectionCounter, pSocket);
            IonEnvironment.GetLog().WriteInformation(string.Format("Created IonTcpConnection [{0}] for {1}.", connection.ID, connection.ipAddress));
            
            return connection;
        }
        #endregion
    }
}
