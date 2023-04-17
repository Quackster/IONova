using System.Collections.Generic;
using System.IO;

namespace Ion.Configuration
{
    public class ConfigurationModule
    {
        #region Fields
        private Dictionary<string, string> mContent = new Dictionary<string, string>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the configuration value as string for a given key.
        /// </summary>
        /// <param name="sKey">The key (string) to get or set the value string of.</param>
        public string this[string sKey]
        {
            get
            {
                return this.GetValue(sKey);
            }
            set
            {
                this.SetValue(sKey, value);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the configuration value string of a given key. A blank string is returned if the configuration key is not defined.
        /// </summary>
        /// <param name="sKey">The configuration key as a string.</param>
        /// <returns>The configuration value of a given key as a string.</returns>
        public string GetValue(string sKey)
        {
            if (!mContent.ContainsKey(sKey))
                return "";

            return mContent[sKey];
        }
        public void SetValue(string sKey, string sValue)
        {
            if(mContent.ContainsKey(sKey))
                mContent[sKey] = sValue;
            else
                mContent.Add(sKey, sValue);
        }
        public bool TryParseInt32(string sField, out int i)
        {
            bool Success = int.TryParse(this[sField], out i);
            if (!Success)
                IonEnvironment.GetLog().WriteConfigurationParseError(sField);

            return Success;
        }
        public bool TryParseUInt32(string sField, out uint i)
        {
            bool Success = uint.TryParse(this[sField], out i);
            if (!Success)
                IonEnvironment.GetLog().WriteConfigurationParseError(sField);

            return Success;
        }
        public int TryParseInt32(string sField)
        {
            int i = 0; TryParseInt32(sField, out i);
            return i;
        }
        public uint TryParseUInt32(string sField)
        {
            uint i = 0; TryParseUInt32(sField, out i);
            return i;
        }
        
        /// <summary>
        /// Loads a ConfigurationModule from a file at a given path.
        /// </summary>
        /// <param name="sPath">The path of the configuration file to load.</param>
        /// <returns>ConfigurationModule holding the loaded configurations.</returns>
        public static ConfigurationModule LoadFromFile(string sPath)
        {
            if (!File.Exists(sPath))
                throw new FileNotFoundException("File at path \"" + sPath + "\" does not exist.");

            ConfigurationModule pConfig = new ConfigurationModule();
            using (StreamReader pReader = new StreamReader(sPath))
            {
                string sLine = null;
                while ((sLine = pReader.ReadLine()) != null)
                {
                    if (sLine.Length == 0 || sLine[0] == '#')
                        continue; // This line is empty/a comment

                    int indexOfDelimiter = sLine.IndexOf('=');
                    if (indexOfDelimiter != -1)
                    {
                        string sKey = sLine.Substring(0, indexOfDelimiter);
                        if (!pConfig.mContent.ContainsKey(sKey))
                        {
                            string sValue = sLine.Substring(indexOfDelimiter + 1);
                            pConfig.mContent.Add(sKey, sValue);
                        }
                    }
                }
                pReader.Close();
            }

            return pConfig;
        }
        #endregion
    }
}
