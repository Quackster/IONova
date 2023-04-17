using System;
using System.Collections.Generic;

using Ion.Net.Messages;

namespace Ion.HabboHotel.Client.Utilities
{
    /// <summary>
    /// Decodes a ClientMessage whose body consists out of structured user properties.
    /// </summary>
    public class UserPropertiesDecoder
    {
        #region Fields
        private readonly Dictionary<int, string> mProperties;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a user property with a given ID. If there is no property with that ID decoded, null is returned.
        /// </summary>
        /// <param name="propID">The ID of the property to get.</param>
        public string this[int propID]
        {
            get
            {
                if (mProperties.ContainsKey(propID))
                    return mProperties[propID];

                return null;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Decodes a ClientMessage body to user properties and puts them in the constructed object.
        /// </summary>
        /// <param name="message">The ClientMessage to decode the body with user properties of.</param>
        public UserPropertiesDecoder(ClientMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            mProperties = new Dictionary<int, string>();
            while (message.remainingContent > 0)
            {
                int propID = message.PopInt32();

                if (propID == 10) // Spam me yes/no property
                {
                    // Weird exception on protocol due to Base64 boolean
                    // Skip 7 bytes and ignore this property

                    message.Advance(7);
                    continue;
                }

                string propVal = message.PopFixedString();
                mProperties.Add(propID, propVal);
            }
        }
        #endregion
    }
}
