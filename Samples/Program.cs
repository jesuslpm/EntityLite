using inercya.EntityLite;
using inercya.EntityLite.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Samples.Entities;
using System.Data;
using System.Reflection;
using System.Data.Common;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestOracleSeq();
            //ToPascalTests();
            //Program.RaiseProductPrice();
            Program.InsertUpdateDeleteProduct();
            //Program.ShowPagedProducts();
            //ShowOrderDetails();
        }

        static void TestOracleSeq()
        {
            using (var ds = new NorhtwindDataService("Northwind"))
            {
                var cmd = ds.Connection.CreateCommand();
                cmd.CommandText = @"
DECLARE
  id_seq_$var$ PLS_INTEGER;
BEGIN
  id_seq_$var$ := ID_SEQ.nextval;
  :id_seq_$param$ := id_seq_$var$;
END;";
                IDbDataParameter id = cmd.CreateParameter();
                id.DbType = DbType.Int32;
                id.Direction = ParameterDirection.Output;
                id.ParameterName = ":id_seq_$param$";
                cmd.Parameters.Add(id);
                ds.OpenConnection();
                cmd.ExecuteNonQuery();
                Console.WriteLine(id.Value);

            }
        }

        static void ToPascalTests()
        {
            Console.WriteLine("OrderDetails".ToPascalNamingConvention());
            Console.WriteLine("order_details".ToPascalNamingConvention());
            Console.WriteLine("ORDER_DETAILS".ToPascalNamingConvention());
            Console.WriteLine(string.Empty.ToPascalNamingConvention());

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
                    .Where(OrderDetailFields.OrderId, 10248)
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

        //static Func<IDataReader, DataRow, DbType> GetDbTypeFunc(string providerName, DbProviderFactory factory)
        //{
        //    if (providerName == "Npgsql")
        //    {
        //        Type datareaderType = Type.GetType("Npgsql.NpgsqlDataReader, " + factory.GetType().Assembly.FullName);
        //        MethodInfo getFieldDbTypeMehtodInfo = datareaderType.GetMethod("GetFieldDbType");
        //        Delegate.CreateDelegate()
        //        // GetFieldDbType;
        //    }
        //}

        static void InsertUpdateDeleteProduct()
        {
            using (var ds = new Entities.NorhtwindDataService("Northwind"))
            {
                //ds.DefaultSchemaName = "NORTHWIND";
                var p = new Entities.Product
                {
                    CategoryId = 2,
                    ProductName = "New Product",
                    QuantityPerUnit = "2",
                    ReorderLevel = 50,
                    SupplierId = 2,
                    UnitPrice = 10,
                    UnitsInStock = 1,
                    UnitsOnOrder = 0,
                    Discontinued = false
                };

                // inserts the new product
                ds.ProductRepository.Save(p);

                Console.WriteLine("Inserted product id:" + p.ProductId);

                p.ProductName = "Another Name";

                // updates the product
                ds.ProductRepository.Save(p);

                p = ds.ProductRepository.Get(Projection.Detailed, p.ProductId);

                Console.WriteLine("CategoryName:" + p.CategoryName);

                ds.ProductRepository.Delete(p.ProductId);


            }
        }
    }
}
