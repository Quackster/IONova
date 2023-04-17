using System;

namespace Ion.Storage
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string sMessage) : base(sMessage) { }
    }
}
