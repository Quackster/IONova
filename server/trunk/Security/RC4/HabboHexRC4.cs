using System;
using System.Text;

namespace Ion.Security.RC4
{
    public class HabboHexRC4
    {
        /* Class written by Nillus based on V8 class from Sulake
         * V7 key extracted by Mike
         * Temporarily 'init' fix by Mike
         * <3
         */

        #region Static fields
        private static readonly byte[] PRIVATE_KEY = { 44, 214, 122, 91, 114, 79, 16, 141, 115, 110, 207, 216, 238, 65, 59, 50, 186, 70, 128, 248, 107, 12, 33, 247, 66, 79, 53, 216, 74, 81, 145, 249, 179, 111, 233, 56, 49, 92, 123, 162, 26, 46, 182, 96, 208, 93, 114, 170, 255, 19, 164, 208, 79, 91, 241, 128, 158, 25, 252, 194, 217, 20, 22, 44, 1, 253, 45, 91, 113, 89, 203, 80, 34, 112, 99, 82, 243, 8, 90, 240, 39, 17, 230, 232, 80, 180, 173, 164, 112, 163, 217, 155, 170, 41, 187, 156, 213, 199, 176, 180, 180, 236, 167, 128, 31, 155, 210, 208, 55, 198, 5, 243, 27, 208, 78, 13, 142, 64, 80, 21, 18, 19, 175, 252, 126, 194, 11, 190, 99, 94, 184, 248, 167, 77, 45, 5, 141, 128, 72, 42, 45, 107, 88, 140, 147, 30, 248, 243, 208, 82, 137, 181, 69, 177, 128, 216, 25, 3, 239, 179, 160, 159, 129, 135, 23, 62, 192, 90, 91, 172, 119, 255, 135, 39, 78, 216, 12, 188, 45, 204, 93, 54, 30, 165, 129, 178, 151, 253, 92, 31, 196, 126, 4, 72, 182, 180, 216, 144, 78, 255, 185, 228, 134, 92, 103, 141, 2, 144, 123, 161, 101, 187, 145, 187, 171, 62, 21, 244, 17, 231, 203, 120, 176, 87, 150, 89, 244, 7, 29, 21, 235, 165, 86, 125, 184, 90, 232, 232, 145, 15, 198, 165, 103, 12, 245, 177, 151, 29, 45, 26, 184, 91, 20, 16, 231, 174, 237, 207, 165, 251, 114, 185, 245, 68, 82, 116, 216, 0, 203, 89, 234, 174, 100, 220, 60, 42, 60, 103, 17, 93, 208, 72, 242, 116, 148, 84, 230, 115, 56, 138, 134, 107, 199, 17, 73, 58, 75, 187, 200, 253, 141, 249, 246, 74, 201, 166, 194, 156, 72, 221, 20, 6, 91, 191, 243, 100, 3, 113, 79, 59, 175, 94, 112, 81, 69, 166, 145, 89, 163, 111, 180, 110, 146, 156, 43, 206, 248, 22, 188, 27, 123, 152, 65, 136, 212, 185, 83, 104, 162, 69, 21, 208, 116, 78, 193, 2, 179, 222, 109, 66, 75, 56, 46, 21, 105, 140, 236, 13, 78, 58, 30, 55, 114, 228, 96, 156, 89, 179, 116, 30, 63, 7, 52, 10, 182, 25, 87, 29, 166, 75, 64, 89, 30, 110, 40, 50, 121, 107, 44, 151, 246, 147, 131, 39, 105, 227, 58, 66, 56, 82, 107, 73, 91, 133, 210, 202, 174, 56, 108, 29, 117, 109, 128, 103, 237, 227, 13, 138, 177, 180, 146, 142, 82, 83, 115, 194, 148, 62, 74, 92, 154, 95, 194, 104, 216, 2, 166, 59, 150, 137, 164, 49, 189, 33, 236, 46, 82, 169, 73, 77, 177, 81, 67, 98, 181, 116, 49, 76, 97, 204, 227, 29, 203, 113, 110, 242, 255, 140, 46, 204, 144, 39, 234, 167, 30, 150, 110, 219, 138, 136, 88, 12, 179, 71, 23, 150, 233, 80, 217, 244, 248, 111, 65, 255, 69, 217, 55, 49, 43, 228, 225, 10, 123, 71, 41, 173, 7, 15, 194, 8, 87, 209, 75, 212, 179, 144, 151, 48, 134, 47, 109, 212, 8, 24, 66, 102, 198, 211, 35, 184, 154, 76, 147, 170, 90, 247, 53, 31, 164, 5, 189, 12, 208, 99, 185, 52, 74, 154, 137, 235, 112, 132, 5, 16, 65, 124, 87, 109, 83, 170, 37, 20, 88, 134, 2, 86, 218, 169, 222, 128, 202, 28, 87, 81, 154, 199, 124, 239, 130, 47, 88, 219, 61, 97, 18, 95, 81, 144, 123, 64, 49, 239, 24, 87, 134, 24, 102, 230, 169, 145, 83, 11, 126, 166, 230, 149, 31, 164, 94, 197, 27, 225, 35, 17, 24, 241, 140, 17, 42, 10, 40, 124, 217, 114, 116, 252, 232, 55, 77, 88, 75, 5, 48, 180, 220, 218, 124, 97, 177, 184, 192, 205, 59, 54, 89, 152, 79, 6, 64, 29, 167, 155, 62, 14, 197, 181, 66, 142, 153, 91, 230, 43, 96, 110, 122, 187, 235, 209, 190, 241, 128, 50, 23, 53, 114, 43, 111, 106, 99, 15, 232, 115, 101, 210, 234, 245, 238, 164, 56, 123, 94, 125, 223, 97, 210, 151, 91, 204, 4, 72, 140, 41, 143, 19, 93, 212, 153, 102, 182, 243, 102, 93, 214, 32, 68, 236, 146, 92, 168, 99, 46, 150, 249, 34, 177, 203, 105, 126, 129, 43, 156, 166, 3, 168, 43, 81, 183, 131, 168, 111, 131, 157, 155, 195, 195, 177, 47, 180, 82, 61, 225, 62, 150, 176, 212, 191, 129, 117, 98, 72, 173, 192, 36, 203, 15, 224, 254, 52, 127, 174, 231, 38, 213, 239, 120, 52, 178, 101, 97, 132, 130, 144, 152, 251, 226, 90, 18, 233, 74, 41, 88, 28, 17, 58, 177, 84, 226, 119, 241, 25, 192, 7, 157, 125, 170, 188, 191, 186, 75, 97, 225, 115, 184, 100, 168, 133, 0, 220, 95, 160, 242, 14, 185, 219, 214, 108, 157, 142, 32, 135, 69, 86, 64, 90, 236, 179, 137, 64, 128, 214, 63, 132, 152, 177, 167, 158, 8, 122, 139, 89, 115, 11, 27, 85, 94, 45, 12, 164, 18, 169, 213, 74, 196, 61, 55, 60, 238, 33, 77, 181, 88, 166, 61, 96, 152, 139, 209, 42, 223, 203, 149, 25, 93, 71, 132, 40, 77, 31, 187, 168, 88, 210, 106, 251, 181, 29, 15, 158, 194, 183, 176, 230, 91, 2, 124, 174, 86, 165, 57, 108, 191, 227, 106, 164, 159, 110, 35, 205, 248, 254, 105, 129, 25, 77, 6, 164, 93, 176, 192, 205, 26, 96, 109, 191, 35, 239, 46, 124, 53, 208, 221, 175, 169, 246, 68, 228, 158, 39, 221, 66, 234, 170, 154, 6, 192, 132, 25, 6, 168, 169, 26, 251, 183, 23, 204, 192, 34, 96, 126, 20, 183, 135, 20, 223, 115, 137, 254, 247, 13, 71, 7, 176, 162, 184, 184, 255, 128, 229, 236, 107, 42, 80, 68, 112, 127, 4, 57, 89, 26, 78, 251, 177, 21, 151, 224, 26, 227, 112, 78, 240, 11, 247, 87, 103 };
        private static readonly char[] HEXALPHABET_CHARS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        #endregion

        #region Instance fields
        private byte i;
        private byte j;
        private byte[] mTable;
        #endregion

        #region Constructor
        public HabboHexRC4(string sPublicKey)
        {
            int iKeyHash = CalculateKeyHash(sPublicKey);
            Initialize(iKeyHash);
        }
        #endregion

        #region Methods
        public void Initialize(int iKeyHash)
        {
            // Reset indexes and table
            this.i = 0;
            this.j = 0;
            mTable = new byte[256];

            mTable[0] = 210;
            mTable[1] = 13;
            mTable[2] = 16;
            mTable[3] = 171;
            mTable[4] = 150;
            mTable[5] = 141;
            mTable[6] = 29;
            mTable[7] = 249;
            mTable[8] = 134;
            mTable[9] = 233;
            mTable[10] = 237;
            mTable[11] = 56;
            mTable[12] = 236;
            mTable[13] = 129;
            mTable[14] = 136;
            mTable[15] = 54;
            mTable[16] = 41;
            mTable[17] = 64;
            mTable[18] = 73;
            mTable[19] = 240;
            mTable[20] = 187;
            mTable[21] = 113;
            mTable[22] = 170;
            mTable[23] = 35;
            mTable[24] = 196;
            mTable[25] = 38;
            mTable[26] = 93;
            mTable[27] = 60;
            mTable[28] = 30;
            mTable[29] = 46;
            mTable[30] = 189;
            mTable[31] = 8;
            mTable[32] = 192;
            mTable[33] = 94;
            mTable[34] = 160;
            mTable[35] = 133;
            mTable[36] = 231;
            mTable[37] = 44;
            mTable[38] = 167;
            mTable[39] = 39;
            mTable[40] = 177;
            mTable[41] = 70;
            mTable[42] = 88;
            mTable[43] = 152;
            mTable[44] = 162;
            mTable[45] = 62;
            mTable[46] = 247;
            mTable[47] = 53;
            mTable[48] = 72;
            mTable[49] = 111;
            mTable[50] = 235;
            mTable[51] = 77;
            mTable[52] = 204;
            mTable[53] = 103;
            mTable[54] = 74;
            mTable[55] = 191;
            mTable[56] = 78;
            mTable[57] = 118;
            mTable[58] = 241;
            mTable[59] = 6;
            mTable[60] = 71;
            mTable[61] = 203;
            mTable[62] = 224;
            mTable[63] = 207;
            mTable[64] = 9;
            mTable[65] = 232;
            mTable[66] = 99;
            mTable[67] = 175;
            mTable[68] = 219;
            mTable[69] = 107;
            mTable[70] = 142;
            mTable[71] = 185;
            mTable[72] = 79;
            mTable[73] = 43;
            mTable[74] = 40;
            mTable[75] = 148;
            mTable[76] = 181;
            mTable[77] = 125;
            mTable[78] = 172;
            mTable[79] = 21;
            mTable[80] = 137;
            mTable[81] = 200;
            mTable[82] = 27;
            mTable[83] = 174;
            mTable[84] = 109;
            mTable[85] = 126;
            mTable[86] = 85;
            mTable[87] = 59;
            mTable[88] = 102;
            mTable[89] = 108;
            mTable[90] = 28;
            mTable[91] = 97;
            mTable[92] = 75;
            mTable[93] = 0;
            mTable[94] = 4;
            mTable[95] = 120;
            mTable[96] = 163;
            mTable[97] = 153;
            mTable[98] = 157;
            mTable[99] = 229;
            mTable[100] = 246;
            mTable[101] = 124;
            mTable[102] = 145;
            mTable[103] = 190;
            mTable[104] = 63;
            mTable[105] = 164;
            mTable[106] = 154;
            mTable[107] = 96;
            mTable[108] = 52;
            mTable[109] = 222;
            mTable[110] = 24;
            mTable[111] = 131;
            mTable[112] = 92;
            mTable[113] = 47;
            mTable[114] = 214;
            mTable[115] = 86;
            mTable[116] = 143;
            mTable[117] = 138;
            mTable[118] = 188;
            mTable[119] = 206;
            mTable[120] = 161;
            mTable[121] = 14;
            mTable[122] = 115;
            mTable[123] = 132;
            mTable[124] = 117;
            mTable[125] = 144;
            mTable[126] = 158;
            mTable[127] = 104;
            mTable[128] = 69;
            mTable[129] = 135;
            mTable[130] = 18;
            mTable[131] = 165;
            mTable[132] = 228;
            mTable[133] = 1;
            mTable[134] = 61;
            mTable[135] = 50;
            mTable[136] = 166;
            mTable[137] = 87;
            mTable[138] = 83;
            mTable[139] = 182;
            mTable[140] = 234;
            mTable[141] = 23;
            mTable[142] = 105;
            mTable[143] = 49;
            mTable[144] = 225;
            mTable[145] = 230;
            mTable[146] = 2;
            mTable[147] = 213;
            mTable[148] = 159;
            mTable[149] = 238;
            mTable[150] = 10;
            mTable[151] = 57;
            mTable[152] = 51;
            mTable[153] = 195;
            mTable[154] = 227;
            mTable[155] = 37;
            mTable[156] = 155;
            mTable[157] = 95;
            mTable[158] = 253;
            mTable[159] = 173;
            mTable[160] = 205;
            mTable[161] = 15;
            mTable[162] = 89;
            mTable[163] = 116;
            mTable[164] = 98;
            mTable[165] = 7;
            mTable[166] = 146;
            mTable[167] = 194;
            mTable[168] = 19;
            mTable[169] = 33;
            mTable[170] = 201;
            mTable[171] = 180;
            mTable[172] = 3;
            mTable[173] = 55;
            mTable[174] = 168;
            mTable[175] = 42;
            mTable[176] = 184;
            mTable[177] = 100;
            mTable[178] = 84;
            mTable[179] = 123;
            mTable[180] = 90;
            mTable[181] = 198;
            mTable[182] = 67;
            mTable[183] = 183;
            mTable[184] = 149;
            mTable[185] = 119;
            mTable[186] = 110;
            mTable[187] = 248;
            mTable[188] = 186;
            mTable[189] = 58;
            mTable[190] = 34;
            mTable[191] = 221;
            mTable[192] = 81;
            mTable[193] = 80;
            mTable[194] = 20;
            mTable[195] = 106;
            mTable[196] = 91;
            mTable[197] = 244;
            mTable[198] = 216;
            mTable[199] = 66;
            mTable[200] = 197;
            mTable[201] = 151;
            mTable[202] = 179;
            mTable[203] = 68;
            mTable[204] = 112;
            mTable[205] = 122;
            mTable[206] = 255;
            mTable[207] = 12;
            mTable[208] = 114;
            mTable[209] = 211;
            mTable[210] = 147;
            mTable[211] = 208;
            mTable[212] = 209;
            mTable[213] = 242;
            mTable[214] = 199;
            mTable[215] = 217;
            mTable[216] = 82;
            mTable[217] = 251;
            mTable[218] = 243;
            mTable[219] = 121;
            mTable[220] = 223;
            mTable[221] = 193;
            mTable[222] = 45;
            mTable[223] = 218;
            mTable[224] = 128;
            mTable[225] = 101;
            mTable[226] = 76;
            mTable[227] = 178;
            mTable[228] = 22;
            mTable[229] = 202;
            mTable[230] = 130;
            mTable[231] = 139;
            mTable[232] = 245;
            mTable[233] = 48;
            mTable[234] = 127;
            mTable[235] = 36;
            mTable[236] = 17;
            mTable[237] = 226;
            mTable[238] = 140;
            mTable[239] = 26;
            mTable[240] = 156;
            mTable[241] = 176;
            mTable[242] = 25;
            mTable[243] = 254;
            mTable[244] = 239;
            mTable[245] = 11;
            mTable[246] = 31;
            mTable[247] = 32;
            mTable[248] = 250;
            mTable[249] = 215;
            mTable[250] = 169;
            mTable[251] = 212;
            mTable[252] = 5;
            mTable[253] = 65;
            mTable[254] = 252;
            mTable[255] = 220;
        }
        public byte MixTable()
        {
            // Re-calculate table fields
            this.i = (byte)((this.i + 1) % 256);
            this.j = (byte)((j + mTable[i]) % 256);

            // Swap table fields
            byte bSwap = mTable[this.i];
            mTable[this.i] = mTable[this.j];
            mTable[this.j] = bSwap;

            return mTable[(mTable[this.i] + mTable[this.j]) % 256];
        }

        public byte[] Encipher(byte[] Data)
        {
            byte[] Result = new byte[Data.Length * 2];

            for (int a = 0, b = 0; a < Data.Length; a++, b += 2)
            {
                byte k = MixTable();
                int c = Data[a] & 0xff ^ k;
                if (c > 0)
                {
                    Result[b] = (byte)HEXALPHABET_CHARS[c >> 4 & 0xf];
                    Result[b + 1] = (byte)HEXALPHABET_CHARS[c & 0xf];
                }
            }

            return Result;
        }
        public string Encipher(string sData)
        {
            StringBuilder sbResult = new StringBuilder(sData.Length * 2);
            for (int a = 0; a < sData.Length; a++)
            {
                byte k = MixTable();
                int c = sData[a] & 0xff ^ k;
                if (c > 0)
                {
                    sbResult.Append(HEXALPHABET_CHARS[c >> 4 & 0xf]);
                    sbResult.Append(HEXALPHABET_CHARS[c & 0xf]);
               }
                else
                {
                    sbResult.Append("00");
                }
            }

            return sbResult.ToString();
        }

        public byte[] Decipher(byte[] Data, int Length)
        {
            if (Length % 2 != 0)
                throw new HabboRc4Exception("Invalid input data, input data is not hexadecimal.");

            byte[] Result = new byte[Length / 2];
            for (int a = 0, b = 0; a < Length; a += 2, b++)
            {
                byte c = ConvertTwoHexBytesToByte((byte)Data[a], (byte)Data[a + 1]);

                Result[b] = (byte)(c ^ MixTable());
            }

            return Result;
        }
        public string Decipher(string sData)
        {
            if (sData.Length % 2 != 0)
                throw new HabboRc4Exception("Invalid input data, input data is not hexadecimal.");

            StringBuilder sbResult = new StringBuilder(sData.Length);
            for (int a = 0, b = 0; a < sData.Length; a += 2, b++)
            {
                byte c = ConvertTwoHexBytesToByte((byte)sData[a], (byte)sData[a + 1]);

                sbResult.Append((char)(c ^ MixTable()));
            }

            return sbResult.ToString();
        }

        #region Static methods
        public static string GeneratePublicKeyString()
        {
            int keyLength = new Random(DateTime.Now.Millisecond).Next(52, 64);
            Random v = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + keyLength);
            StringBuilder sb = new StringBuilder(keyLength);

            for (int i = 0; i < keyLength; i++)
            {
                int j = 0;
                if (v.Next(0, 2) == 1)
                    j = v.Next(97, 123);
                else
                    j = v.Next(48, 58);
                sb.Append((char)j);
            }

            return sb.ToString();
        }
        public static int CalculateKeyHash(string sPublicKey)
        {
            int iHash = 0;
            string sTable = sPublicKey.Substring(0, sPublicKey.Length / 2);
            string sKey = sPublicKey.Substring(sPublicKey.Length / 2);

            for (int i = 0; i < sTable.Length; i++)
            {
                int iIndex = sTable.IndexOf(sKey[i]);
                if (iIndex % 2 == 0)
                    iIndex *= 2;
                if (i % 3 == 0)
                    iIndex *= 3;
                if (iIndex < 0)
                    iIndex = sTable.Length % 2;

                iHash += iIndex;
                iHash ^= iIndex << (i % 3) * 8;
            }

            return iHash;
        }

        private static byte ConvertTwoHexBytesToByte(byte A, byte B)
        {
            int C = 0; // The output value
            int D = 0; // Counter used for determining hex value

            while (D < HEXALPHABET_CHARS.Length)
            {
                if (HEXALPHABET_CHARS[D] == (A & 0xff))
                {
                    C = (D << 4);
                    break;
                }
                D++;
            }

            D = 0;
            while (D < HEXALPHABET_CHARS.Length)
            {
                if (HEXALPHABET_CHARS[D] == (B & 0xff))
                {
                    C += D;
                    break;
                }
                D++;
            }

            return (byte)C;
        }
        #endregion
        #endregion
    }

    public class HabboRc4Exception : Exception
    {
        #region Constructor
        public HabboRc4Exception(string sMessage) : base(sMessage) { }
        #endregion
    }
}
