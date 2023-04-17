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


        #endregion

        #region Properties

        #endregion

        #region Constructor

        public DatabaseManager(string sServer, uint Port, string sUser, string sPassword, string sDatabase, uint minPoolSize, uint maxPoolSize)
        {

        }


        #endregion
    }
}