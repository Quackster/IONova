using System;
using System.Data;
using System.Collections.Generic;

using Ion.Storage;

namespace Ion.HabboHotel.Access
{
    /// <summary>
    /// Provides access to user rights for roles etc.
    /// </summary>
    public class UserRightManager
    {
        #region Fields
        private uint mMaxRoles;
        private Dictionary<byte, string[]> mRights;
        #endregion

        #region Constructors
        public UserRightManager(uint maxRoles)
        {
            mMaxRoles = maxRoles;
            mRights = new Dictionary<byte, string[]>();
        }
        #endregion

        #region Methods
        public void ReloadRights()
        {
            mRights.Clear();
            using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
            {
                // Search for all roles
                for (byte role = 0; role <= mMaxRoles; role++)
                {
                    // Get rights for roles (and roles inherited)
                    DataTable result = dbClient.ReadDataTable("SELECT userright FROM access_userrights WHERE role <= " + role.ToString() + ";");
                    if (result.Rows.Count > 0)
                    {
                        // Shove rights to a string array
                        string[] rights = new string[result.Rows.Count];
                        for (int position = 0; position < result.Rows.Count; position++)
                        {
                            rights[position] = (string)result.Rows[position]["userright"];
                        }

                        // Add rights for this role to the dictionary
                        mRights.Add(role, rights);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the rights for a given user role.
        /// </summary>
        /// <param name="role">The user role to get the rights for.</param>
        /// <returns>String array with all the rights for this role and underlying roles.</returns>
        public string[] GetRights(byte role)
        {
            if (mRights.ContainsKey(role))
            {
                return mRights[role];
            }
            else
            {
                return new string[] { };
            }
        }

        /// <summary>
        /// Determines if a given user role has access to a given user right.
        /// </summary>
        /// <param name="role">The user role to check.</param>
        /// <param name="right">The user right to check.</param>
        public bool RoleHasRight(byte role, string right)
        {
            string[] rights = this.GetRights(role);
            foreach (string roleRight in rights)
            {
                if (roleRight == right)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
