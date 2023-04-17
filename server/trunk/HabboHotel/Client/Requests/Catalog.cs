using System.Collections.Generic;

namespace Ion.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 101 - "Ae"
        /// </summary>
        private void GetCatalogIndex()
        {
            Response.Initialize(ResponseOpcodes.CatalogIndex);

            SendResponse();
        }

        /// <summary>
        /// 102 = "A?"
        /// </summary>
        private void GetCatalogPage()
        {
            uint pageID = Request.PopWireduint();
        }

        /// <summary>
        /// Registers handlers that have to do with the ingame Catalog.
        /// </summary>
        public void RegisterCatalog()
        {
            mRequestHandlers[101] = new RequestHandler(GetCatalogIndex);
            mRequestHandlers[102] = new RequestHandler(GetCatalogPage);
        }
    }
}
