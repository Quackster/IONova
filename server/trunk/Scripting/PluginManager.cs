using System;
using System.IO;
using System.Runtime;
using System.Reflection;
using System.Collections.Generic;

namespace Ion.Scripting
{
    public class PluginManager
    {
        #region Fields
        private List<IonPlugin> mPlugins = new List<IonPlugin>();
        #endregion

        #region Methods
        /// <summary>
        /// Unloads the current plugins and clears the plugin collection.
        /// </summary>
        public void Clear()
        {
            lock (mPlugins)
            {
                foreach (IonPlugin pPlugin in mPlugins)
                {
                    pPlugin.Unload();
                }

                mPlugins.Clear();
            }
        }

        public List<IonPlugin> GetPlugins(string sDirectory)
        {
            List<IonPlugin> pPlugins = new List<IonPlugin>();
            if (Directory.Exists(sDirectory))
            {
                string[] szFiles = Directory.GetFiles(sDirectory, "*.dll");
                for (int x = 0; x < szFiles.Length; x++)
                {
                    IonPlugin pPlugin = this.InstantiatePluginFromAssembly(szFiles[x]);
                    if (pPlugin != null)
                        pPlugins.Add(pPlugin);
                }
            }

            return pPlugins;
        }

        /// <summary>
        /// Tries to instantiate and return an IonPlugin descendant by loading a .NET assembly file at a given path. Null is returned if the target file does not exist, is not a .NET assembly file or is not a IonPlugin descendant.
        /// </summary>
        /// <param name="sAssemblyPath">The full path to the assembly file to load.</param>
        private IonPlugin InstantiatePluginFromAssembly(string sAssemblyPath)
        {
            try
            {
                Assembly pAssembly = Assembly.Load(AssemblyName.GetAssemblyName(sAssemblyPath));
                IonPlugin pPlugin = (IonPlugin)Activator.CreateInstance(pAssembly.GetType());

                return pPlugin;
            }
            catch { return null; } // Either file not found, no CLI header or no IonPlugin deriver
        }
        #endregion
    }
}
