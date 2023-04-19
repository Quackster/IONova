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
        #region Properties

        public string Server { get; set; }
        public uint Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public uint MinPoolSize { get; set; }
        public uint MaxPoolSize { get; set; }

        #endregion

        #region Constructor

        public DatabaseManager(string server, uint port, string user, string password, string database, uint minPoolSize, uint maxPoolSize)
        {
            Server = server;
            Port = port;
            User = user;
            Password = password;
            Database = database;
            MinPoolSize = minPoolSize;
            MaxPoolSize = maxPoolSize;
        }

        #endregion

        #region Methods

        public DatabaseContext GetContext()
        {
            return new DatabaseContext(Server, Port, User, Password, Database, MinPoolSize, MaxPoolSize);
        }

        #endregion
    }
}