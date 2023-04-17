using System;
using System.Text;
using System.Collections.Generic;

using Ion.Specialized.Encoding;

namespace Ion.Net.Messages
{
    public class ServerMessage : IHabboMessage
    {
        #region Fields
        /// <summary>
        /// The ID of this message as an unsigned 32 bit integer.
        /// </summary>
        private uint mID;
        /// <summary>
        /// The content of this message as a System.Collections.Generic.List(byte).
        /// </summary>
        private List<byte> mContent;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ID of this message as an unsigned 32 bit integer.
        /// </summary>
        public uint ID
        {
            get { return mID; }
        }
        /// <summary>
        /// Gets theheader of this message, by Base64 encoding the message ID to a 3 byte string.
        /// </summary>
        public string Header
        {
            get
            {
                return IonEnvironment.GetDefaultTextEncoding().GetString(Base64Encoding.Encodeuint(mID, 2));
            }
        }
        /// <summary>
        /// Gets the length of the content in this message.
        /// </summary>
        public int contentLength
        {
            get { return mContent.Count; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a weak ServerMessage, which is not useable until Initialize() is called.
        /// </summary>
        public ServerMessage()
        {
            // Requires a call to Initialize before usage
        }
        /// <summary>
        /// Constructs a ServerMessage object with a given ID and no content.
        /// </summary>
        /// <param name="ID">The ID of this message as an unsigned 32 bit integer.</param>
        public ServerMessage(uint ID)
        {
            Initialize(ID);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clears all the content in the message and sets the message ID.
        /// </summary>
        /// <param name="ID">The ID of this message as an unsigned 32 bit integer.</param>
        public void Initialize(uint ID)
        {
            mID = ID;
            mContent = new List<byte>();
        }
        /// <summary>
        /// Clears the message content.
        /// </summary>
        public void Clear()
        {
            mContent.Clear();
        }
        /// <summary>
        /// Returns the total content of this message as a string.
        /// </summary>
        /// <returns>String</returns>
        public string GetContentString()
        {
            return IonEnvironment.GetDefaultTextEncoding().GetString(mContent.ToArray());
        }

        /// <summary>
        /// Appends a single byte to the message content.
        /// </summary>
        /// <param name="b">The byte to append.</param>
        public void Append(byte b)
        {
            mContent.Add(b);
        }
        /// <summary>
        /// Appends a byte array to the message content.
        /// </summary>
        /// <param name="bzData">The byte array to append.</param>
        public void Append(byte[] bzData)
        {
            if (bzData != null && bzData.Length > 0)
                mContent.AddRange(bzData);
        }
        /// <summary>
        /// Encodes a string with the environment's default text encoding and appends it to the message content.
        /// </summary>
        /// <param name="s">The string to append.</param>
        public void Append(string s)
        {
            Append(s, null);
        }
        /// <summary>
        /// Encodes a string with a given text encoding and appends it to the message content.
        /// </summary>
        /// <param name="s">The string to append.</param>
        /// <param name="pEncoding">A System.Text.Encoding to use for encoding the string.</param>
        public void Append(string s, Encoding pEncoding)
        {
            if (s != null && s.Length > 0)
            {
                if (pEncoding == null)
                    pEncoding = IonEnvironment.GetDefaultTextEncoding();

                Append(pEncoding.GetBytes(s));
            }
        }
        /// <summary>
        /// Appends a 32 bit integer in it's string representation to the message content.
        /// </summary>
        /// <param name="i">The 32 bit integer to append.</param>
        public void Append(Int32 i)
        {
            Append(i.ToString(), Encoding.ASCII);
        }
        /// <summary>
        /// Appends a 32 bit unsigned integer in it's string representation to the message content.
        /// </summary>
        /// <param name="i">The 32 bit unsigned integer to append.</param>
        public void Append(uint i)
        {
            Append((Int32)i);
        }

        /// <summary>
        /// Appends a wire encoded boolean to the message content.
        /// </summary>
        /// <param name="b">The boolean to encode and append.</param>
        public void AppendBoolean(bool b)
        {
            if (b == true)
                mContent.Add(WireEncoding.POSITIVE);
            else
                mContent.Add(WireEncoding.NEGATIVE);
        }
        /// <summary>
        /// Appends a wire encoded 32 bit integer to the message content.
        /// </summary>
        /// <param name="i">The 32 bit integer to encode and append.</param>
        public void AppendInt32(Int32 i)
        {
            Append(WireEncoding.EncodeInt32(i));
        }
        /// <summary>
        /// Appends a wire encoded 32 bit unsigned integer to the message content.
        /// </summary>
        /// <param name="i">The 32 bit unsigned integer to encode and append.</param>
        /// <seealso>AppendInt32</seealso>
        public void AppendUInt32(uint i)
        {
            AppendInt32((Int32)i);
        }

        /// <summary>
        /// Appends a string with the default string breaker byte to the message content.
        /// </summary>
        /// <param name="s">The string to append.</param>
        public void AppendString(string s)
        {
            AppendString(s, 2);
        }
        /// <summary>
        /// Appends a string with a given string breaker byte to the message content.
        /// </summary>
        /// <param name="s">The string to append.</param>
        /// <param name="Breaker">The byte used to mark the end of the string.</param>
        public void AppendString(string s, byte Breaker)
        {
            Append(s); // Append string with default encoding
            Append(Breaker); // Append breaker
        }

        public void AppendObject(ISerializableObject obj)
        {
            obj.Serialize(this);
        }

        public byte[] GetBytes()
        {
            byte[] Data = new byte[contentLength + 2 + 1];
            
            byte[] Header = Base64Encoding.Encodeuint(mID, 2);
            Data[0] = Header[0];
            Data[1] = Header[1];

            for (int i = 0; i < this.contentLength; i++)
            {
                Data[i + 2] = mContent[i]; 
            }

            Data[Data.Length - 1] = 1;

            return Data;
        }
        #endregion
    }
}
