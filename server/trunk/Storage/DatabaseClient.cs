using System;
using System.Data;

using MySql.Data.MySqlClient;

namespace Ion.Storage
{
    /// <summary>
    /// Represents a client of a database,
    /// </summary>
    public class DatabaseClient : IDisposable
    {
        #region Fields
        private uint mHandle;
        private DateTime mLastActivity;

        private DatabaseManager mManager;
        private MySqlConnection mConnection;
        private MySqlCommand mCommand;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the handle of this database client.
        /// </summary>
        public uint Handle
        {
            get { return mHandle; }
        }
        /// <summary>
        /// Gets whether this database client is anonymous and does not recycle in the database manager.
        /// </summary>
        public bool Anonymous
        {
            get { return (mHandle == 0); }
        }
        /// <summary>
        /// Gets the DateTime object representing the date and time this client has been used for the last time.
        /// </summary>
        public DateTime LastActivity
        {
            get { return mLastActivity; }
        }
        /// <summary>
        /// Gets the amount of seconds that this client has been inactive.
        /// </summary>
        public int Inactivity
        {
            get { return (int)(DateTime.Now - mLastActivity).TotalSeconds; }
        }
        /// <summary>
        /// Gets the state of the connection instance.
        /// </summary>
        public ConnectionState State
        {
            get { return (mConnection != null) ? mConnection.State : ConnectionState.Broken; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructs a new database client with a given handle to a given database proxy.
        /// </summary>
        /// <param name="Handle">The identifier of this database client as an unsigned 32 bit integer.</param>
        /// <param name="pManager">The instance of the DatabaseManager that manages the database proxy of this database client.</param>
        public DatabaseClient(uint Handle, DatabaseManager pManager)
        {
            if (pManager == null)
                throw new ArgumentNullException("pManager");

            mHandle = Handle;
            mManager = pManager;

            mConnection = new MySqlConnection(mManager.CreateConnectionString());
            mCommand = mConnection.CreateCommand();

            UpdateLastActivity();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Attempts to open the database connection.
        /// </summary>
        public void Connect()
        {
            if (mConnection == null)
                throw new DatabaseException("Connection instance of database client " + mHandle + " holds no value.");
            if (mConnection.State != ConnectionState.Closed)
                throw new DatabaseException("Connection instance of database client " + mHandle + " requires to be closed before it can open again.");

            try
            {
                mConnection.Open();
            }
            catch (MySqlException mex)
            {
                throw new DatabaseException("Failed to open connection for database client " + mHandle + ", exception message: " + mex.Message);
            }
        }
        /// <summary>
        /// Attempts to close the database connection.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                mConnection.Close();
            }
            catch { } 
        }
        /// <summary>
        /// Closes the database connection (if open) and disposes all resources.
        /// </summary>
        public void Destroy()
        {
            Disconnect();

            mConnection.Dispose();
            mConnection = null;

            mCommand.Dispose();
            mCommand = null;

            mManager = null;
        }
        /// <summary>
        /// Updates the last activity timestamp to the current date and time.
        /// </summary>
        public void UpdateLastActivity()
        {
            mLastActivity = DateTime.Now;
        }

        /// <summary>
        /// Returns the DatabaseManager of this database client.
        /// </summary>
        public DatabaseManager GetManager()
        {
            return mManager;
        }

        public void AddParamWithValue(string sParam, object val)
        {
            mCommand.Parameters.AddWithValue(sParam, val);
        }
        public void ExecuteQuery(string sQuery)
        {
            mCommand.CommandText = sQuery;
            mCommand.ExecuteScalar();
            mCommand.CommandText = null;
        }

        public DataSet ReadDataSet(string sQuery)
        {
            DataSet pDataSet = new DataSet();
            mCommand.CommandText = sQuery;

            using (MySqlDataAdapter pAdapter = new MySqlDataAdapter(mCommand))
            {
                pAdapter.Fill(pDataSet);
            }
            mCommand.CommandText = null;

            return pDataSet;
        }
        public DataTable ReadDataTable(string sQuery)
        {
            DataTable pDataTable = new DataTable();
            mCommand.CommandText = sQuery;

            using (MySqlDataAdapter pAdapter = new MySqlDataAdapter(mCommand))
            {
                pAdapter.Fill(pDataTable);
            }
            mCommand.CommandText = null;

            return pDataTable;
        }
        public DataRow ReadDataRow(string sQuery)
        {
            DataTable pDataTable = ReadDataTable(sQuery);
            if (pDataTable != null && pDataTable.Rows.Count > 0)
                return pDataTable.Rows[0];

            return null;
        }
        public String ReadString(string sQuery)
        {
            mCommand.CommandText = sQuery;
            String result = mCommand.ExecuteScalar().ToString();
            mCommand.CommandText = null;

            return result;
        }
        public Int32 ReadInt32(string sQuery)
        {
            mCommand.CommandText = sQuery;
            Int32 result = (Int32)mCommand.ExecuteScalar();
            mCommand.CommandText = null;

            return result;
        }
        public UInt32 ReadUInt32(string sQuery)
        {
            mCommand.CommandText = sQuery;
            UInt32 result = (UInt32)mCommand.ExecuteScalar();
            mCommand.CommandText = null;

            return result;
        }

        #region IDisposable members
        /// <summary>
        /// Returns the DatabaseClient to the DatabaseManager, where the connection will stay alive for 30 seconds of inactivity.
        /// </summary>
        public void Dispose()
        {
            if (this.Anonymous == false) // No disposing for this client yet! Return to the manager!
            {
                // Reset this!
                mCommand.CommandText = null;
                mCommand.Parameters.Clear();

                mManager.ReleaseClient(mHandle);
            }
            else // Anonymous client, dispose this right away!
            {
                Destroy();
            }
        }
        #endregion
        #endregion
    }
}
