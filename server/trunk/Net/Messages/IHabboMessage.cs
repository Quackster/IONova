using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ion.Net.Messages
{
    /// <summary>
    /// Represents a Habbo client > server protocol structured message, providing methods to identify and 'read' the message.
    /// </summary>
    public interface IHabboMessage
    {
        #region Properties
        uint ID { get; }
        String Header { get; }
        Int32 contentLength { get; }
        #endregion

        #region Methods
        string GetContentString();
        #endregion
    }
}
