using System;
using System.Threading;
using System.Net.Sockets;

using Ion.Security.RC4;
using Ion.Specialized.Utilities;

using Ion.Net.Messages;

using Ion.HabboHotel.Client;

namespace Ion.Net.Connections
{
    /// <summary>
    /// Represents a TCP network connection accepted by an IonTcpConnectionListener. Provides methods for sending and receiving data, aswell as disconnecting the connection.
    /// </summary>
    public class IonTcpConnection
    {
        #region Fields
        /// <summary>
        /// The buffer size for receiving data.
        /// </summary>
        private const int RECEIVEDATA_BUFFER_SIZE = 512;
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
        private Socket mSocket = null;

        /// <summary>
        /// The byte array holding the buffer for receiving data from client.
        /// </summary>
        private byte[] mDataBuffer = null;
        /// <summary>
        /// The AsyncCallback instance for the thread for receiving data asynchronously.
        /// </summary>
        private AsyncCallback mDataReceivedCallback;
        /// <summary>
        /// The RouteReceivedDataCallback to route received data to another object.
        /// </summary>
        private RouteReceivedDataCallback mRouteReceivedDataCallback;

        private bool mEncryptionStarted;
        /// <summary>
        /// The HabboHexRC4 instance providing the deciphering of enciphered client messages.
        /// </summary>
        private HabboHexRC4 mRc4;
        #endregion

        #region Members
        public delegate void RouteReceivedDataCallback(ref byte[] Data);
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

                return mSocket.RemoteEndPoint.ToString().Split(':')[0];
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
        public IonTcpConnection(uint ID, Socket pSocket)
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
            mDataBuffer = new byte[RECEIVEDATA_BUFFER_SIZE];
            mDataReceivedCallback = new AsyncCallback(DataReceived);
            mRouteReceivedDataCallback = dataRouter;

            WaitForData();
        }
        /// <summary>
        /// Stops the connection, disconnects the socket and disposes used resources.
        /// </summary>
        public void Stop()
        {
            if (!this.Alive)
                return; // Already stopped

            mSocket.Close();
            mSocket = null;
            mDataBuffer = null;
            mDataReceivedCallback = null;
            mRc4 = null;
        }
        public bool TestConnection()
        {
            try
            {
                return mSocket.Send(new byte[] { 0 }) > 0;
                //mSocket.Send(new byte[] { 0 });
                //return true;
            }
            catch { }

            return false;
        }
        private void ConnectionDead()
        {
            IonEnvironment.GetHabboHotel().GetClients().StopClient(mID);
        }

        public void SendData(byte[] Data)
        {
            if (this.Alive)
            {
                try
                {
                    mSocket.Send(Data);
                }
                catch (SocketException)
                {
                    ConnectionDead();
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
        public void SendData(string sData)
        {
            SendData(IonEnvironment.GetDefaultTextEncoding().GetBytes(sData));
        }
        public void SendMessage(ServerMessage message)
        {
            IonEnvironment.GetLog().WriteLine(" [" + mID + "] <-- " + message.Header + message.GetContentString());

            SendData(message.GetBytes());
        }

        public void SetEncryption(string sPublicKey)
        {
            mRc4 = new HabboHexRC4(sPublicKey);
            mEncryptionStarted = true;
        }

        /// <summary>
        /// Starts the asynchronous wait for new data.
        /// </summary>
        private void WaitForData()
        {
            if (this.Alive)
            {
                try
                {
                    mSocket.BeginReceive(mDataBuffer, 0, RECEIVEDATA_BUFFER_SIZE, SocketFlags.None, mDataReceivedCallback, null);
                }
                catch (SocketException)
                {
                    ConnectionDead();
                }
                catch (ObjectDisposedException)
                {
                    ConnectionDead();
                }
                catch (Exception ex)
                {
                    IonEnvironment.GetLog().WriteUnhandledExceptionError("IonTcpConnection.WaitForData", ex);
                    ConnectionDead();
                }
            }
        }
        private void DataReceived(IAsyncResult iAr)
        {
            // Connection not stopped yet?
            if (this.Alive == false)
                return;

            // Do an optional wait before processing the data
            if (RECEIVEDATA_MILLISECONDS_DELAY > 0)
                Thread.Sleep(RECEIVEDATA_MILLISECONDS_DELAY);

            // How many bytes has server received?
            int numReceivedBytes = 0;
            try
            {
                numReceivedBytes = mSocket.EndReceive(iAr);
            }
            catch (ObjectDisposedException)
            {
                ConnectionDead();
                return;
            }
            catch (Exception ex)
            {
                IonEnvironment.GetLog().WriteUnhandledExceptionError("IonTcpConnection.DataReceived", ex);
                
                ConnectionDead();
                return;
            }

            if (numReceivedBytes > 0)
            {
                // Copy received data buffer
                byte[] dataToProcess = ByteUtility.ChompBytes(mDataBuffer, 0, numReceivedBytes);

                // Decipher received data?
                if (mEncryptionStarted)
                {
                    dataToProcess = mRc4.Decipher(dataToProcess, numReceivedBytes);
                }

                // Route data to GameClient to parse and process messages
                RouteData(ref dataToProcess);
                //Environment.GetHabboHotel().GetClients().GetClient(this.ID).HandleConnectionData(ref dataToProcess);
            }

            // Wait for new data
            WaitForData();
        }

        /// <summary>
        /// Routes a byte array passed as reference to another object.
        /// </summary>
        /// <param name="Data">The byte array to route.</param>
        private void RouteData(ref byte[] Data)
        {
            if (mRouteReceivedDataCallback != null)
            {
                mRouteReceivedDataCallback.Invoke(ref Data);
            }
        }
        #endregion
    }
}
