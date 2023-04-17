using System;
using System.Data;
using System.Collections.Generic;

using Ion.Net.Messages;

namespace Ion.HabboHotel.Catalog
{
    public class CatalogPage : ISerializableObject
    {
        #region Fields
        private uint mID;
        private uint mParentID;
        private uint mTreeID;
        private bool mVisible;
        private bool mComingSoon;
        private string mName;
        private byte mIconColor;
        private byte mIconImage;

        private string mLayoutClass;
        private string mHeadlineImage;
        private string mTeaserImages;
        private string mText;
        private string mMoreDetails;
        private string mExtraData;
        #endregion

        #region Properties
        public uint ID
        {
            get { return mID; }
        }
        public uint ParentID
        {
            get { return mParentID; }
        }
        public uint TreeID
        {
            get { return mTreeID; }
        }
        public bool Visible
        {
            get { return mVisible; }
        }
        public bool ComingSoon
        {
            get { return mComingSoon; }
        }
        public string Name
        {
            get { return mName; }
        }
        public byte IconColor
        {
            get { return mIconColor; }
        }
        public byte IconImage
        {
            get { return mIconImage; }
        }

        public bool IsPage
        {
            get { return (mTreeID == 0); }
        }
        #endregion

        #region Constructor
        public CatalogPage()
        {

        }
        #endregion

        #region Methods
        public void Serialize(ServerMessage message)
        {
            // I got bored here, sorry guys
        }

        public static CatalogPage Parse(DataRow row)
        {
            try
            {
                CatalogPage page = new CatalogPage();
                page.mID = (uint)row["id"];
                page.mParentID = (uint)row["parentid"];
                page.mTreeID = (uint)row["treeid"];
                page.mVisible = ((byte)row["visible"] == 1);
                page.mComingSoon = ((byte)row["comingsoon"] == 1);
                page.mName = (string)row["name"];
                page.mIconColor = (byte)row["icon_color"];
                page.mIconImage = (byte)row["icon_image"];

                return page;
            }
            catch (Exception ex)
            {
                IonEnvironment.GetLog().WriteUnhandledExceptionError("CatalogPage.Parse", ex);
            }

            return null;
        }
        #endregion
    }
}
