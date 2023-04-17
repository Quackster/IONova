using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ion.Scripting
{
    public class RoflPlugin : IonPlugin
    {
        public RoflPlugin()
        {
            mName = "Ion Rofl Plugin";
            mDescription = "This plugin rofls because it's the first plugin for Ion";
            mAuthor = "Nillus";
            mVersion = new Version(1, 0); // 1.0
        }

        #region Methods
        public override void Load()
        {

        }
        public override void Unload()
        {
           
        }
        #endregion
    }
}
