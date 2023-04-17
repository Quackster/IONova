using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ion.Scripting
{
    /// <summary>
    /// Represents a stub for Ion plugins. This class cannot be instantiated.
    /// </summary>
    public abstract class IonPlugin
    {
        #region Fields
        protected string mName;
        protected string mDescription;
        protected string mAuthor;
        protected Version mVersion;
        protected IonPluginHost mHost;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of this plugin.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }
        /// <summary>
        /// Gets the description of this plugin.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
        }
        /// <summary>
        /// Gets the author of this plugin.
        /// </summary>
        public string Author
        {
            get { return mAuthor; }
        }
        /// <summary>
        /// Gets the System.Version object representing the version of this plugin.
        /// </summary>
        public Version Version
        {
            get { return mVersion; }
        }
        /// <summary>
        /// Gets or sets the IonPluginHost of this plugin. The plugin will call back to the host during runtime.
        /// </summary>
        public IonPluginHost Host
        {
            get { return mHost; }
            set { mHost = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method is called when the plugin is loaded by the host.
        /// </summary>
        public abstract void Load();
        /// <summary>
        /// This method is called when the plugin is unloaded by the host.
        /// </summary>
        public abstract void Unload();
        #endregion
    }
}
