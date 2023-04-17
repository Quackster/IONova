using Ion.HabboHotel.Client;
using Ion.HabboHotel.Habbos;

namespace Ion.HabboHotel
{
    /// <summary>
    /// Represents a multiuser, virtual hotel where users can create avatars, to chat with other users in spaces set up as 'rooms'. Users have the ability to create their own 'guestroom' as well, and they are able to decorate it with virtual furniture.
    /// </summary>
    public class HabboHotel
    {
        #region Fields
        private uint mVersion;

        // Modules
        private GameClientManager mClientManager = null;
        private HabboManager mHabboManager = null;
        private HabboAuthenticator mAuthenticator = null;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the version of the game client as a 32 bit unsigned integer.
        /// </summary>
        public uint Version
        {
            get { return mVersion; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a Habbo Hotel environment and tries to initialize it.
        /// </summary>
        public HabboHotel()
        {
            // Try to parse version
            IonEnvironment.Configuration.TryParseUInt32("projects.habbo.client.version", out mVersion);
            
            // Initialize HabboHotel project modules
            mClientManager = new GameClientManager();
            mHabboManager = new HabboManager();
            mAuthenticator = new HabboAuthenticator();

            // Start connection checker for clients
            mClientManager.StartConnectionChecker();

            // Print that we are done!
            IonEnvironment.GetLog().WriteLine(string.Format("Initialized project 'Habbo Hotel' for version {0}.", mVersion));
        }
        #endregion

        #region Methods
        public void Destroy()
        {
            // Clear clients
            if (GetClients() != null)
            {
                GetClients().Clear();
                GetClients().StopConnectionChecker();
            }

            IonEnvironment.GetLog().WriteLine(string.Format("Destroyed project 'Habbo Hotel' for version {0}.", mVersion));
        }

        /// <summary>
        /// Returns the game client manager.
        /// </summary>
        public GameClientManager GetClients()
        {
            return mClientManager;
        }
        /// <summary>
        /// Returns the Habbo manager.
        /// </summary>
        public HabboManager GetHabbos()
        {
            return mHabboManager;
        }
        /// <summary>
        /// Returns the Habbo authenticator.
        /// </summary>
        public HabboAuthenticator GetAuthenticator()
        {
            return mAuthenticator;
        }

        #endregion
    }
}
