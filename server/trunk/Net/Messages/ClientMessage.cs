using System;
using System.Text;

using Ion.Specialized.Encoding;

namespace Ion.Net.Messages
{
    /// <summary>
    /// Represents a Habbo client > server protocol structured message, providing methods to identify and 'read' the message.
    /// </summary>
    public class ClientMessage : IHabboMessage
    {
        #region Fields
        /// <summary>
        /// The ID of this message as an unsigned 32 bit integer.
        /// </summary>
        private readonly uint mID;
        /// <summary>
        /// The content of this message as a byte array.
        /// </summary>
        private readonly byte[] mContent;
        /// <summary>
        /// The current index in the content array, used when reading the message.
        /// </summary>
        private int mContentCursor;
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
        /// Gets theheader of this message, by Base64 encoding the message ID to a 2 byte string.
        /// </summary>
        public string Header
        {
            get
            {
                return IonEnvironment.GetDefaultTextEncoding().GetString(Base64Encoding.Encodeuint(mID, 2));
            }
        }
        /// <summary>
        /// Gets the method name the client intends to invoke on the server with this message.
        /// </summary>
        public string MethodName
        {
            get
            {
                return string.Format("TODO:GETMETHOD{0}", mID);
            }
        }
        /// <summary>
        /// Gets the length of the content in this message.
        /// </summary>
        public int contentLength
        {
            get { return mContent.Length; }
        }
        /// <summary>
        /// Gets the amount of unread content bytes.
        /// </summary>
        public int remainingContent
        {
            get { return (mContent.Length - mContentCursor); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a ClientMessage object for a given message ID and a given content byte array.
        /// </summary>
        /// <param name="ID">The ID of the message as an unsigned 32 bit integer.</param>
        /// <param name="bzContent">The content as a byte array. If null is supplied, an empty byte array will be created.</param>
        public ClientMessage(uint ID, byte[] bzContent)
        {
            if (bzContent == null)
                bzContent = new byte[0];

            mID = ID;
            mContent = bzContent;
            mContentCursor = 0;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Resets the client message to it's state when it was constructed by resetting the content reader cursor. This allows to re-read read data.
        /// </summary>
        public void Reset()
        {
            mContentCursor = 0;
        }
        /// <summary>
        /// Advances the content cursor by a given amount of bytes.
        /// </summary>
        /// <param name="n">The amount of bytes to 'skip'.</param>
        public void Advance(int n)
        {
            mContentCursor += n;
        }
        /// <summary>
        /// Returns the total content of this message as a string.
        /// </summary>
        /// <returns>String</returns>
        public string GetContentString()
        {
            return IonEnvironment.GetDefaultTextEncoding().GetString(mContent);
        }

        /// <summary>
        /// Reads a given amount of bytes from the remaining message content and returns it in a byte array. The reader cursor is incremented during reading.
        /// </summary>
        /// <param name="numBytes">The amount of bytes to read, advance and return. If there is less remaining data than this value, all remaining data will be read.</param>
        /// <returns>byte[]</returns>
        private byte[] ReadBytes(int numBytes)
        {
            if (numBytes > this.remainingContent)
                numBytes = this.remainingContent;

            byte[] bzData = new byte[numBytes];
            for (int x = 0; x < numBytes; x++)
            {
                bzData[x] = mContent[mContentCursor++];
            }

            return bzData;
        }
        /// <summary>
        /// Reads a given amount of bytes from the remaining message content and returns it in a byte array. The reader cursor does not increment during reading.
        /// </summary>
        /// <param name="numBytes">The amount of bytes to read, advance and return. If there is less remaining data than this value, all remaining data will be read.</param>
        /// <returns>byte[]</returns>
        private byte[] ReadBytesFreezeCursor(int numBytes)
        {
            if (numBytes > this.remainingContent)
                numBytes = this.remainingContent;

            byte[] bzData = new byte[numBytes];
            for (int x = 0, y = mContentCursor; x < numBytes; x++, y++)
            {
                bzData[x] = mContent[y];
            }

            return bzData;
        }
        /// <summary>
        /// Reads a length-prefixed (Base64) value from the message and returns it as a byte array.
        /// </summary>
        /// <returns>byte[]</returns>
        private byte[] ReadFixedValue()
        {
            Int32 Length = Base64Encoding.DecodeInt32(this.ReadBytes(2));
            return this.ReadBytes(Length);
        }

        /// <summary>
        /// Reads a Base64 boolean and returns it. False is returned if there is no remaining content.
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean PopBase64Boolean()
        {
            return (this.remainingContent > 0 && mContent[mContentCursor++] == Base64Encoding.POSITIVE);
        }

        public Int32 PopInt32()
        {
            return Base64Encoding.DecodeInt32(this.ReadBytes(2));
        }
        public UInt32 PopUInt32()
        {
            return (UInt32)PopInt32();
        }

        /// <summary>
        /// Reads a length prefixed string from the message content and encodes it with a given System.Text.Encoding.
        /// </summary>
        /// <param name="pEncoding">The System.Text.Encoding to encode the string with.</param>
        /// <returns>String</returns>
        public String PopFixedString(Encoding pEncoding)
        {
            if (pEncoding == null)
                pEncoding = IonEnvironment.GetDefaultTextEncoding();

            return pEncoding.GetString(this.ReadFixedValue());
        }
        /// <summary>
        /// Reads a length prefixed string from the message content and encodes it with the Ion environment default text encoding.
        /// </summary>
        /// <returns>String</returns>
        public String PopFixedString()
        {
            Encoding pEncoding = IonEnvironment.GetDefaultTextEncoding();
            return PopFixedString(pEncoding);
        }

        /// <summary>
        /// Reads a length prefixed string 32 bit integer from the message content and tries to parse it to integer. No exceptions are thrown if parsing fails.
        /// </summary>
        /// <returns>Int32</returns>
        public Int32 PopFixedInt32()
        {
            Int32 i;
            String s = PopFixedString(Encoding.ASCII);
            Int32.TryParse(s, out i);

            return i;
        }
        /// <summary>
        /// Reads a length prefixed string 32 bit unsigned integer from the message content and tries to parse it to integer. No exceptions are thrown if parsing fails.
        /// </summary>
        /// <returns>Int32</returns>
        /// <seealso>PopFixedInt32</seealso>
        public uint PopFixedUInt32()
        {
            return (uint)PopFixedInt32();
        }

        /// <summary>
        /// Reads a wire format boolean and returns it. False is returned if there is no remaining content.
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean PopWiredBoolean()
        {
            return (this.remainingContent > 0 && mContent[mContentCursor++] == WireEncoding.POSITIVE);
        }
        /// <summary>
        /// Reads the next wire encoded 32 bit integer from the message content and advances the reader cursor.
        /// </summary>
        /// <returns>Int32</returns>
        public Int32 PopWiredInt32()
        {
            if (this.remainingContent == 0)
                return 0;

            byte[] bzData = this.ReadBytesFreezeCursor(WireEncoding.MAX_INTEGER_BYTE_AMOUNT);
            Int32 totalBytes = 0;
            Int32 i = WireEncoding.DecodeInt32(bzData, out totalBytes);
            mContentCursor += totalBytes;

            return i;
        }
        /// <summary>
        /// Reads the next wire encoded unsigned 32 bit integer from the message content and advances the reader cursor.
        /// </summary>
        /// <returns>Int32</returns>
        /// <see>PopWiredInt32()</see>
        public uint PopWireduint()
        {
            return (uint)PopWiredInt32();
        }

        public override string ToString()
        {
            return this.Header + GetContentString();
        }
        #endregion
    }
}
