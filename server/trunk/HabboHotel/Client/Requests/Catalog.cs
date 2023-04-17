using System;
using System.Collections.Generic;

using Ion.HabboHotel.Catalog;

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

            // Add all visible pages to the index response
            List<CatalogPage> pages = IonEnvironment.GetHabboHotel().GetCatalog().GetPages();
            foreach (CatalogPage page in pages)
            {
                Response.AppendBoolean(page.Visible);
                Response.AppendInt32((int)page.IconColor);
                Response.AppendInt32((int)page.IconImage);
                Response.AppendUInt32(page.ID);
                Response.AppendString(page.Name);
                Response.AppendBoolean(page.ComingSoon);
                Response.AppendUInt32(page.TreeID);
            }

            SendResponse();
        }

        /// <summary>
        /// 102 = "A?"
        /// </summary>
        private void GetCatalogPage()
        {
            uint pageID = Request.PopWireduint();

            Response.Initialize(ResponseOpcodes.CatalogPage);
            CatalogPage page = IonEnvironment.GetHabboHotel().GetCatalog().GetPage(pageID);
            if (page != null)
            {
                Response.AppendObject(page);

                List<CatalogProduct> products = IonEnvironment.GetHabboHotel().GetCatalog().GetProducts(pageID);
                foreach (CatalogProduct product in products)
                {
                    Response.AppendObject(product);
                }
            }
            SendResponse();
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
