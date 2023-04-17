using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ion.Specialized.Encoding
{
    public static class WireEncoding
    {
        #region Fields
        public static byte NEGATIVE = 72; // 'H'
        public static byte POSITIVE = 73; // 'I'
        public static int MAX_INTEGER_BYTE_AMOUNT = 6;
        #endregion

        #region Methods
        public static byte[] EncodeInt32(Int32 i)
        {
            byte[] wf = new byte[WireEncoding.MAX_INTEGER_BYTE_AMOUNT];
            int pos = 0;
            int numBytes = 1;
            int startPos = pos;
            int negativeMask = i >= 0 ? 0 : 4;
            i = Math.Abs(i);
            wf[pos++] = (byte)(64 + (i & 3));
            for (i >>= 2; i != 0; i >>= WireEncoding.MAX_INTEGER_BYTE_AMOUNT)
            {
                numBytes++;
                wf[pos++] = (byte)(64 + (i & 0x3f));
            }
            wf[startPos] = (byte)(wf[startPos] | numBytes << 3 | negativeMask);

            // Skip the null bytes in the result
            byte[] bzData = new byte[numBytes];
            for (int x = 0; x < numBytes; x++)
            {
                bzData[x] = wf[x];
            }

            return bzData;
        }
        public static Int32 DecodeInt32(byte[] bzData, out Int32 totalBytes)
        {
            int pos = 0;
            int v = 0;
            bool negative = (bzData[pos] & 4) == 4;
            totalBytes = bzData[pos] >> 3 & 7;
            v = bzData[pos] & 3;
            pos++;
            int shiftAmount = 2;
            for (int b = 1; b < totalBytes; b++)
            {
                v |= (bzData[pos] & 0x3f) << shiftAmount;
                shiftAmount = 2 + 6 * b;
                pos++;
            }

            if (negative == true)
                v *= -1;

            return v;
        }
        #endregion
    }
}
