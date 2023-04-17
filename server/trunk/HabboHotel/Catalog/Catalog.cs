using System;
using System.Data;
using System.Collections.Generic;

using Ion.Storage;

namespace Ion.HabboHotel.Catalog
{
    public class Catalog
    {
        #region Fields
        private List<CatalogPage> mPages;
        private List<CatalogProduct> mProducts;
        #endregion

        #region Constructors
        public Catalog()
        {
            mPages = new List<CatalogPage>();
            mProducts = new List<CatalogProduct>();
        }
        #endregion

        #region Methods
        public void ReloadPages()
        {
            lock (this)
            {
                mPages.Clear();

                using (DatabaseClient dbClient = IonEnvironment.GetDatabase().GetClient())
                {
                    CatalogPage index = CatalogPage.Parse(dbClient.ReadDataRow("SELECT * FROM catalog_pages WHERE parentid = 0 LIMIT 1;"));
                    if(index != null)
                    {
                        // Index = first page!
                        mPages.Add(index);

                        // Fetchs trees and inner pages on index
                        foreach (DataRow childRow in dbClient.ReadDataTable("SELECT * FROM catalog_pages WHERE parentid = " + index.TreeID.ToString() + " ORDER BY orderid ASC;").Rows)
                        {
                            CatalogPage child = CatalogPage.Parse(childRow);
                            if (child != null)
                            {
                                // Add child to index
                                mPages.Add(child);

                                // Is this child a parent?
                                if (child.TreeID > 0)
                                {
                                    // Add children to this parent
                                    foreach (DataRow pageRow in dbClient.ReadDataTable("SELECT * FROM catalog_pages WHERE parentid = " + child.TreeID.ToString() + " AND treeid = 0 ORDER BY orderid ASC;").Rows)
                                    {
                                        CatalogPage page = CatalogPage.Parse(pageRow);
                                        if (page != null)
                                        {
                                            mPages.Add(page);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ReloadProducts()
        {
            lock (this)
            {
                mProducts.Clear();
            }
        }

        public CatalogPage GetPage(uint pageID)
        {
            foreach (CatalogPage page in mPages)
            {
                if (page.ID == pageID)
                {
                    return page;
                }
            }

            return null;
        }
        public CatalogProduct GetProduct(uint productID)
        {
            foreach (CatalogProduct product in mProducts)
            {

            }

            return null;
        }
        public List<CatalogProduct> GetProducts(uint pageID)
        {
            List<CatalogProduct> products = new List<CatalogProduct>();
            foreach (CatalogProduct product in mProducts)
            {
                if (false)
                {
                    products.Add(product);
                }
            }

            return products;
        }

        public List<CatalogPage> GetPages()
        {
            return mPages;
        }
        public List<CatalogProduct> GetProducts()
        {
            return mProducts;
        }
        #endregion
    }
}
