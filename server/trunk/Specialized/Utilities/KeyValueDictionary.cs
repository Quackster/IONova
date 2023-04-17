using System;
using System.Data;
using System.Collections.Generic;

using Ion.Storage;

namespace Ion.Specialized.Utilities
{
    /// <summary>
    /// A Dictionary(string, string) that can load text entries from a Database.
    /// </summary>
    public class KeyValueDictionary
    {
        #region Fields
        private readonly string mTableName;
        private readonly string mKeyName;
        private readonly string mValueName;
        private Dictionary<string, string> mDictionary;
        #endregion

        #region Properties
        public int Count
        {
            get { return mDictionary.Count; }
        }
        #endregion

        #region Constructor
        public KeyValueDictionary(string sTableName, string sKeyName, string sValueName)
        {
            mTableName = sTableName;
            mKeyName = sKeyName;
            mValueName = sValueName;
            mDictionary = new Dictionary<string, string>();
        }
        #endregion

        #region Methods
        public void Clear()
        {
            mDictionary.Clear();
        }
        public bool Contains(string sKey)
        {
            return mDictionary.ContainsKey(sKey);
        }
        public string GetValue(string sKey)
        {
            if (mDictionary.ContainsKey(sKey))
            {
                return mDictionary[sKey];
            }
            else
            {
                return sKey;
            }
        }

        public void Reload()
        {
            this.Clear();

            using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
            {
                foreach(DataRow row in dbClient.ReadDataTable("SELECT " + mKeyName + ", " + mValueName + " FROM " + mTableName + ";").Rows)
                {
                    string key = (string)row[0];
                    string value = (string)row[1];
                    if (!mDictionary.ContainsKey(key))
                    {
                        mDictionary.Add(key, value);
                    }
                }
            }
        }
        #endregion
    }
}
