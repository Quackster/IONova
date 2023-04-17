using System;

namespace Ion.HabboHotel.Habbos
{
    public class ModerationBanException : Exception
    {
        public ModerationBanException(string sReason) : base(sReason) { }
    }
}
