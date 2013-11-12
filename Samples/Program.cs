using inercya.EntityLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Samples.Entities;
using System.Data;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.RaiseProductPrice();
            //Program.InsertUpdateDeleteProduct();
            //Program.ShowPagedProducts();
            //ShowOrderDetails();
        }

        static byte[] GetSalt()
        {
            var rng = System.Security.Cryptography.RNGCryptoServiceProvider.Create();
            byte[] salt = new byte[32];
            rng.GetBytes(salt);
            return salt;
        }

        static void RaiseProductPrice()
        {
            using (var ds = new Entities.NorhtwindDataService("Northwind"))
            {
                ds.ProductRepository.RaiseProductPrices(-0.11m);
            }
        }

        static void ShowPagedProducts()
        {
            using (var ds = new Entities.NorhtwindDataService("Northwind"))
            {
                const int PageSize = 10;
                var query = ds.ProductRepository.Query(Projection.Detailed)
                    .Fields(ProductFields.CategoryName, ProductFields.ProductName)
                    .OrderBy(ProductFields.CategoryName, ProductFields.ProductName);

                var productCount = query.GetCount();

                var fromRowIndex = 0;
                var toRowIndex = PageSize - 1;
                while (fromRowIndex < productCount)
                {
                    foreach (var product in query.ToEnumerable(fromRowIndex, toRowIndex))
                    {
                        Console.WriteLine("{0}\t{1}", product.CategoryName, product.ProductName);
                    }
                    Console.WriteLine("Press enter to view the next product page ...");
                    Console.ReadLine();
                    fromRowIndex = toRowIndex + 1;
                    toRowIndex += PageSize;
                }
            }
        }

        static void ShowOrderDetails()
        {
            using (var ds = new Entities.NorhtwindDataService("Northwind"))
            {
                var orderDetails = ds.OrderDetailRepository.Query(Projection.Detailed)
                    .Where(OrderDetailFields.OrderID, 10248)
                    .ToList();

            }

        }

        static void P()
        {
            using (var ds = new Entities.NorhtwindDataService("Northwind"))
            {
                ds.OpenConnection();
                using (var cmd = ds.Connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM OrderDetail_Detailed LIMIT 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();
                    }
                }
            }
        }

        static byte[] GetSecureHash(string password, byte[] salt)
        {
            var rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
            return rfc.GetBytes(64);
        }

        static void InsertUpdateDeleteProduct()
        {
            using (var ds = new Entities.NorhtwindDataService("Northwind"))
            {
                var p = new Entities.Product
                {
                    CategoryID = 1,
                    ProductName = "New Product",
                    QuantityPerUnit = "2",
                    ReorderLevel = 50,
                    SupplierID = 2,
                    UnitPrice = 10,
                    UnitsInStock = 1,
                    UnitsOnOrder = 0,
                    Discontinued = false
                };

                // inserts the new product
                ds.ProductRepository.Save(p);

                Console.WriteLine("Inserted product id:" + p.ProductID);

                p.ProductName = "Another Name";

                // updates the product
                ds.ProductRepository.Save(p);

                p = ds.ProductRepository.Get(Projection.Detailed, p.ProductID);

                Console.WriteLine("CategoryName:" + p.CategoryName);

                ds.ProductRepository.Delete(p.ProductID);


            }
        }
    }
}
