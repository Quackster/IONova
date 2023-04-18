using System;
using System.Threading;
using Ion.Specialized.Utilities;

using Ion.Net.Messages;
using DotNetty.Transport.Channels;
using System.Threading.Tasks;

namespace Ion.Net.Connections
{
    /// <summary>
    /// Represents a TCP network connection accepted by an IonTcpConnectionListener. Provides methods for sending and receiving data, aswell as disconnecting the connection.
    /// </summary>
    public class IonTcpConnection
    {
        #region Fields
        /// <summary>
        /// The amount of milliseconds to sleep when receiving data before processing the message. When this constant is 0, the data will be processed immediately.
        /// </summary>
        private static int RECEIVEDATA_MILLISECONDS_DELAY = 0;
        
        /// <summary>
        /// The ID of this connection as a 32 bit unsigned integer.
        /// </summary>
        private readonly uint mID;
        /// <summary>
        /// A DateTime object representing the date and time this connection was created.
        /// </summary>
        private readonly DateTime mCreatedAt;
       
        /// <summary>
        /// The System.Net.Sockets.Socket object providing the connection between client and server.
        /// </summary>
        private IChannel mSocket = null;

        /// <summary>
        /// The AsyncCallback instance for the thread for receiving data asynchronously.
        /// </summary>
        private AsyncCallback mDataReceivedCallback;
        /// <summary>
        /// The RouteReceivedDataCallback to route received data to another object.
        /// </summary>
        private RouteReceivedDataCallback mRouteReceivedDataCallback;

        #endregion

        #region Members
        public delegate void RouteReceivedDataCallback(ClientMessage message);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ID of this connection as a 32 bit unsigned integer.
        /// </summary>
        public uint ID
        {
            get { return mID; }
        }
        /// <summary>
        /// Gets the DateTime object representing the date and time this connection was created at.
        /// </summary>
        public DateTime createdAt
        {
            get { return mCreatedAt; }
        }
        /// <summary>
        /// Gets the age of this connection in whole seconds, by comparing the current date and time to the date and time this connection was created at.
        /// </summary>
        public int ageInSeconds
        {
            get
            {
                int Seconds = (int)(DateTime.Now - mCreatedAt).TotalSeconds;
                if (Seconds <= 0)
                    return 0;

                return Seconds;
            }
        }
        /// <summary>
        /// Gets the IP address of this connection as a string.
        /// </summary>
        public string ipAddress
        {
            get
            {
                if (mSocket == null)
                    return "";

                return mSocket.RemoteAddress.ToString().Split(':')[0];
            }
        }
        /// <summary>
        /// Gets if this connection still posesses a socket object.
        /// </summary>
        public bool Alive
        {
            get { return (mSocket != null); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of IonTcpConnection for a given connection identifier and socket.
        /// </summary>
        /// <param name="ID">The unique ID used to identify this connection in the environment.</param>
        /// <param name="pSocket">The System.Net.Sockets.Socket of the new connection.</param>
        public IonTcpConnection(uint ID, IChannel pSocket)
        {
            mID = ID;
            mSocket = pSocket;
            mCreatedAt = DateTime.Now;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the connection, prepares the received data buffer and waits for data.
        /// </summary>
        public void Start(RouteReceivedDataCallback dataRouter)
        {
            mRouteReceivedDataCallback = dataRouter;
        }

        /// <summary>
        /// Stops the connection, disconnects the socket and disposes used resources.
        /// </summary>
        public void Stop()
        {
            if (!this.Alive)
                return; // Already stopped

            mSocket.CloseAsync();

            mSocket = null;
            mDataReceivedCallback = null;
        }
        public bool TestConnection()
        {
            try
            {
                mSocket.WriteAndFlushAsync(new byte[] { 0 });
                //mSocket.Send(new byte[] { 0 });
                return true;
            }
            catch { }

            return false;
        }
        private void ConnectionDead()
        {
            IonEnvironment.GetHabboHotel().GetClients().StopClient(mID);
        }

        public async Task SendData(byte[] Data)
        {
            if (this.Alive)
            {
                try
                {
                    await mSocket.WriteAndFlushAsync(Data);
                }
                catch (ObjectDisposedException)
                {
                    ConnectionDead();
                }
                catch (Exception ex)
                {
                    IonEnvironment.GetLog().WriteUnhandledExceptionError("IonTcpConnection.Send", ex);
                }
            }
        }
        public async Task SendData(string sData)
        {
            await SendData(IonEnvironment.GetDefaultTextEncoding().GetBytes(sData));
        }
        public async Task SendMessage(ServerMessage message)
        {
            IonEnvironment.GetLog().WriteLine(" [" + mID + "] <-- " + message.Header + message.GetContentString());

            await SendData(message.GetBytes());
        }

        /// <summary>
        /// Routes a byte array passed as reference to another object.
        /// </summary>
        /// <param name="Data">The byte array to route.</param>
        public void RouteData(ClientMessage message)
        {
            if (mRouteReceivedDataCallback != null)
            {
                mRouteReceivedDataCallback.Invoke(message);
            }
        }
        #endregion
    }
}
