using System;
using System.Text;
using System.Data;

using Ion.Storage;
using Ion.Net.Messages;

namespace Ion.HabboHotel.Habbos
{
    /// <summary>
    /// Represents a service user's account and avatar in the account and holds the information about the account.
    /// </summary>
    public class Habbo : ISerializableObject
    {
        #region Fields
        // Account
        private uint mID;
        private string mUsername;
        private string mPassword;
        private byte mRole;
        private DateTime mSignedUp;

        // Personal
        private string mEmail;
        private string mDateOfBirth;

        // Avatar
        private string mMotto;
        private string mFigure;
        private char mGender;

        // Valueables
        private uint mCoins;
        private uint mFilms;
        private uint mGameTickets;
        private uint mActivityPoints;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ID of this Habbo as a 32 bit unsigned integer.
        /// </summary>
        public uint ID
        {
            get { return mID; }
        }
        /// <summary>
        /// Gets or sets the username of this Habbo as a string. The username is shown to other Habbo's in the service, and is also used to login to the service.
        /// </summary>
        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }
        /// <summary>
        /// Gets or sets the password of this Habbo as a string. (plaintext) The password is used in combination with the username when logging in to the service.
        /// </summary>
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
        /// <summary>
        /// Gets the user role of this Habbo as a byte.
        /// </summary>
        public byte Role
        {
            get { return mRole; }
        }
        public DateTime SignedUp
        {
            get { return mSignedUp; }
            set { mSignedUp = value; }
        }

        public string Email
        {
            get { return mEmail; }
            set { mEmail = value; }
        }
        public string DateOfBirth
        {
            get { return mDateOfBirth; }
            set { mDateOfBirth = value; }
        }

        public string Motto
        {
            get { return mMotto; }
            set
            {
                // TODO: swear word filtering?
                mMotto = value;
            }
        }

        public string Figure
        {
            get { return mFigure; }
            set { mFigure = value; }
        }

        public char Gender
        {
            get { return mGender; }
            set { mGender = (value == 'M' || value == 'F') ? value : 'M'; }
        }

        public uint Coins
        {
            get { return mCoins; }
            set { mCoins = value; }
        }

        public uint Films
        {
            get { return mFilms; }
            set { mFilms = value; }
        }

        public uint GameTickets
        {
            get { return mGameTickets; }
            set { mGameTickets = value; }
        }

        public uint ActivityPoints
        {
            get { return mActivityPoints; }
            set { mActivityPoints = value; }
        }
        #endregion

        #region Methods
        public void Serialize(ServerMessage message)
        {
            message.AppendString(mID.ToString());
            message.AppendString(mUsername);
            message.AppendString(mFigure);
            message.AppendString(mGender.ToString());
            message.AppendString(mMotto.ToString());
            message.AppendBoolean(false);
            message.AppendString("");
            message.AppendBoolean(false);
            message.AppendBoolean(false);
            message.AppendBoolean(false);
            message.AppendBoolean(false);
        }

        public string ToOldProtocolString()
        {
            return "name=" + mUsername + Convert.ToChar(13) +
                      "figure=" + mFigure + Convert.ToChar(13) +
                      "sex=" + mGender.ToString() + Convert.ToChar(13) +
                      "customData=" + mMotto + Convert.ToChar(13) +
                      "ph_tickets=" + mGameTickets + Convert.ToChar(13) +
                      "photo_film=" + mFilms + Convert.ToChar(13) +
                      "ph_figure=" + "" + Convert.ToChar(13) +
                      "directMail=0" + Convert.ToChar(13);
        }


        #endregion
    }
}
