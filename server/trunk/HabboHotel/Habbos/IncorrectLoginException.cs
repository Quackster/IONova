using System;

namespace Ion.HabboHotel.Habbos
{
    public class IncorrectLoginException : Exception
    {
        public IncorrectLoginException(string sMessage) : base(sMessage) { }
    }
}
