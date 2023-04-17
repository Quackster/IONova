using System;
using Ion.Net.Connections;
using Ion.Net.Messages;
using Ion.Specialized.Encoding;

using Ion.HabboHotel.Habbos;
using Ion.HabboHotel.Messenger;

namespace Ion.HabboHotel.Client
{
    /// <summary>
    /// Represents a connected HABBO game client. Holds information about the connection and the logged in user.
    /// </summary>
    public class GameClient
    {
        #region Fields
        private readonly uint mID;
        private IonTcpConnection mConnection;
        private ClientMessageHandler mMessageHandler;
        private Habbo mHabbo;
        private bool mPonged;

        // Components
        private MessengerComponent mMessenger;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ID of this client as a 32 bit unsigned integer.
        /// </summary>
        public uint ID
        {
            get { return mID; }
        }
        /// <summary>
        /// Gets the logged in status of this client as a boolean value.
        /// </summary>
        public bool LoggedIn
        {
            get { return (mHabbo != null); }
        }
        public bool PingOK
        {
            get { return mPonged; }
            set { mPonged = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a GameClient instance for a given client ID.
        /// </summary>
        /// <param name="clientID">The ID of this client.</param>
        public GameClient(uint clientID)
        {
            mID = clientID;
            mConnection = IonEnvironment.GetTcpConnections().GetConnection(clientID);
            mMessageHandler = new ClientMessageHandler(this);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the IonTcpConnection instance of this client's connection.
        /// </summary>
        public IonTcpConnection GetConnection()
        {
            return mConnection;
        }
        /// <summary>
        /// Returns the ClientMessageHandler instance of this client.
        /// </summary>
        public ClientMessageHandler GetMessageHandler()
        {
            return mMessageHandler;
        }
        /// <summary>
        /// Returns the Habbo instance holding the information of the Habbo this client is logged in to.
        /// </summary>
        public Habbo GetHabbo()
        {
            return mHabbo;
        }
        /// <summary>
        /// Returns the MessengerComponent instance of this client.
        /// </summary>
        public MessengerComponent GetMessenger()
        {
            return mMessenger;
        }

        /// <summary>
        /// Starts the connection for this client, prepares the message handler and sends HELLO to the client.
        /// </summary>
        public void StartConnection()
        {
            if (mConnection != null)
            {
                // Ping OK!
                mPonged = true;

                // Register request handlers
                mMessageHandler.RegisterGlobal();
                mMessageHandler.RegisterPreLogin();

                // Create data router and start waiting for data
                IonTcpConnection.RouteReceivedDataCallback dataRouter = new IonTcpConnection.RouteReceivedDataCallback(HandleConnectionData);
                mConnection.Start(dataRouter);
            }
        }
        /// <summary>
        /// Stops the client, removes this user from room, updates last online values etc.
        /// </summary>
        public void Stop()
        {
            // Leave room
            // Update last online
            mHabbo = null;
            mConnection = null;

            mMessageHandler.Destroy();
            mMessageHandler = null;
        }

        /// <summary>
        /// Handles a given amount of data in a given byte array, by attempting to parse messages from the received data and process them in the message handler.
        /// </summary>
        /// <param name="Data">The byte array with the data to process.</param>
        /// <param name="numBytesToProcess">The actual amount of bytes in the byte array to process.</param>
        public void HandleConnectionData(ref byte[] data)
        {
            // Gameclient protocol or policyrequest?
            if (data[0] != 64)
            {
                IonEnvironment.GetLog().WriteInformation("Client " + mID + " sent non-gameclient message: " + IonEnvironment.GetDefaultTextEncoding().GetString(data));

                string xmlPolicy =
                                   "<?xml version=\"1.0\"?>\r\n" +
                                   "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                                   "<cross-domain-policy>\r\n" +
                                   "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                                   "</cross-domain-policy>\x0";

                IonEnvironment.GetLog().WriteInformation("Client " + mID + ": sending XML cross domain policy file: " + xmlPolicy);
                mConnection.SendData(xmlPolicy);

                mMessageHandler.GetResponse().Initialize(ResponseOpcodes.SecretKey); // "@A"
                mMessageHandler.GetResponse().Append("ION/Deltar");
                mMessageHandler.SendResponse();
            }
            else
            {
                int pos = 0;
                while (pos < data.Length)
                {
                    try
                    {
                        // Total length of message (without this): 3 Base64 bytes
                        int messageLength = Base64Encoding.DecodeInt32(new byte[] { data[pos++], data[pos++], data[pos++] });

                        // ID of message: 2 Base64 bytes
                        uint messageID = Base64Encoding.DecodeUInt32(new byte[] { data[pos++], data[pos++] });

                        // Data of message: (messageLength - 2) bytes
                        byte[] Content = new byte[messageLength - 2];
                        for (int i = 0; i < Content.Length; i++)
                        {
                            Content[i] = data[pos++];
                        }

                        // Create message object
                        ClientMessage message = new ClientMessage(messageID, Content);

                        // Handle message object
                        mMessageHandler.HandleRequest(message);
                    }
                    catch (IndexOutOfRangeException) // Bad formatting!
                    {
                        IonEnvironment.GetHabboHotel().GetClients().StopClient(mID);
                    }
                    catch (Exception ex)
                    {
                        IonEnvironment.GetLog().WriteUnhandledExceptionError("GameClient.HandleConnectionData", ex);
                    }
                }
            }
        }

        public void Login(string sTicket)
        {
            try
            {
                // Try to login
                mHabbo = IonEnvironment.GetHabboHotel().GetAuthenticator().Login(sTicket);

                // Authenticator has forced unique login now
                this.CompleteLogin();
            }
            catch (IncorrectLoginException exLogin)
            {
                SendClientError(exLogin.Message);
            }
            catch (ModerationBanException exBan)
            {
                SendBanMessage(exBan.Message);
            }
        }

        private void CompleteLogin()
        {
            // Actually logged in?
            if (mHabbo != null)
            {
                // Send user rights
                mMessageHandler.GetResponse().Initialize(ResponseOpcodes.UserRights); // "@B"
                foreach (string sRight in IonEnvironment.GetHabboHotel().GetUserRights().GetRights(mHabbo.Role))
                {
                    mMessageHandler.GetResponse().AppendString(sRight);
                }
                mMessageHandler.SendResponse();

                // Login OK!
                mMessageHandler.GetResponse().Initialize(3); // "@C"
                mMessageHandler.SendResponse();

                // Register handlers
                mMessageHandler.UnRegisterPreLogin();
                mMessageHandler.RegisterUser();
                mMessageHandler.RegisterNavigator();
                mMessageHandler.RegisterCatalog();
            }
        }
        
        /// <summary>
        /// Tries to login this client on a Habbo account with a given username and password.
        /// </summary>
        /// <param name="sUsername">The username of the Habbo to attempt to login on.</param>
        /// <param name="sPassword">The login password of the Habbo username. Case sensitive.</param>
        public void Login(string sUsername, string sPassword)
        {
            try
            {
                // Try to login
                mHabbo = IonEnvironment.GetHabboHotel().GetAuthenticator().Login(sUsername, sPassword);
                
                // Authenticator has forced unique login now
                this.CompleteLogin();
            }
            catch (IncorrectLoginException exLogin)
            {
                SendClientError(exLogin.Message);
            }
            catch (ModerationBanException exBan)
            {
                SendBanMessage(exBan.Message);
            }
        }

        /// <summary>
        /// Attempts to save the Habbo DataObject of this session to the Database.
        /// </summary>
        public bool SaveUserObject()
        {
            if (mHabbo != null)
            {
                return IonEnvironment.GetHabboHotel().GetHabbos().UpdateHabbo(mHabbo);
            }

            return false;
        }

        /// <summary>
        /// Attempts to reload the Habbo DataObject of this session from the Database. If no DataObject is found anymore, the old one is kept.
        /// </summary>
        public bool ReloadUserObject()
        {
            if (mHabbo != null)
            {
                Habbo newObject = IonEnvironment.GetHabboHotel().GetHabbos().GetHabbo(mHabbo.ID);
                if (newObject != null)
                {
                    mHabbo = newObject;
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Reports a given error string to the client.
        /// </summary>
        /// <param name="sError">The error to report.</param>
        public void SendClientError(string sError)
        {
            ServerMessage message = new ServerMessage(ResponseOpcodes.GenericError);
            message.Append(sError);

            GetConnection().SendMessage(message);
        }
        /// <summary>
        /// Sends the 'You are banned' message to the client. The message holds a given ban reason.
        /// </summary>
        /// <param name="sText">The text to display in the ban message.</param>
        public void SendBanMessage(string sText)
        {
            ServerMessage message = new ServerMessage(ResponseOpcodes.UserBanned);
            message.Append(sText);

            GetConnection().SendMessage(message);
        }

        /// <summary>
        /// Sends a 'Notification from staff:' message. The message holds a given text.
        /// </summary>
        /// <param name="sText">The text to display in the message.</param>
        public void SendStaffMessage(string sText)
        {
            ServerMessage message = new ServerMessage(ResponseOpcodes.Mod);
            message.AppendString(sText);

            GetConnection().SendMessage(message);
        }

        /// <summary>
        /// Initializes the MessengerComponent for this client.
        /// </summary>
        public bool InitializeMessenger()
        {
            mMessenger = new MessengerComponent(this);
            mMessenger.ReloadBuddies();

            // Ohwell
            return true;
        }

        /// <summary>
        /// Adds activity points to this Habbo and notifies the client.
        /// </summary>
        /// <param name="amount">The amount of activity points to add.</param>
        public void AddActivityPoints(uint amount)
        {
            if (mHabbo != null)
            {
                // Add points
                mHabbo.ActivityPoints += amount;

                // Notify client
                ServerMessage notify = new ServerMessage(ResponseOpcodes.HabboActivityPointNotification);
                notify.AppendUInt32(mHabbo.ActivityPoints);
                notify.AppendUInt32(amount);
                this.GetConnection().SendMessage(notify);

                // Update user
                this.SaveUserObject();
            }
        }
        #endregion
    }
}
