using System;
using System.Reflection;

namespace Ion.Scripting
{
    /// <summary>
    /// Provides a host for Ion plugins, exposing access to types and methods in the Ion plugin host and it's parents.
    /// </summary>
    public class IonPluginHost : IIonPluginHost
    {
        /// <summary>
        /// Tries to find a System.Type by doing a case-sensitive search for a full type name including namespaces. If the type is found it is returned. Null is returned if an exception is thrown.
        /// </summary>
        /// <param name="sTypeName">The full, case-sensitive type name string of the type to get.</param>
        /// <returns>Type</returns>
        public Type GetIonEnvironmentType(string sTypeName)
        {
            try { return Type.GetType(sTypeName); }
            catch { return null; }
        }
        /// <summary>
        /// Tries to find System.Reflection.MethodInfo of a given method in a given type, by doing a search in the type. If the method is found, the MethodInfo is returned. Null is returned if an exception is thrown.
        /// </summary>
        /// <param name="pType">The System.Type instance of the type to search in.</param>
        /// <param name="sMethodName">The method name of the wanted method as a string.</param>
        /// <returns>MethodInfo</returns>
        public MethodInfo GetIonEnvironmentMethod(Type pType, string sMethodName)
        {
            try { return pType.GetMethod(sMethodName); }
            catch { return null; }
        }
    }

    public interface IIonPluginHost
    {
        #region Methods
        Type GetIonEnvironmentType(string sType);
        MethodInfo GetIonEnvironmentMethod(Type pType, string sMethodName);
        #endregion
    }
}
