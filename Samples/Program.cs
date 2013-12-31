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
using System.Threading;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //UpdateCategories();
            //Localization();
            //ShowOrderDetails();
            //ToUnderscoreTest();
            //TestOracleSeq();
            //ToPascalTests();
            //RaiseProductPrices();
            InsertUpdateDeleteProduct();
            //ShowPagedProducts();
            //ShowSomeProducts();
            //ShowQuesoCabralesOrders();
            //ShowLondonAndewFullerSubtree();
            //HandCraftedSql();
            //ShowProductSales();
            //RaiseProductPrices2();
        }

        private static void RaiseProductPrices2()
        {
            using (var ds = new NorthwindDataService("Northwind"))
            {
                ds.ProductRepository.RaiseProductPrices(1, 0.10m);
            }
        }

        private static void ToPascalTests()
        {
            Console.WriteLine("DetailedOrderLines".ToPascalNamingConvention());
            Console.WriteLine("detailed Order lines".ToPascalNamingConvention());
            Console.WriteLine("detailed_order lines".ToPascalNamingConvention());
            Console.WriteLine("DetailedOrder Lines".ToPascalNamingConvention());
            Console.WriteLine("DETAILED_ORDER_LINES".ToPascalNamingConvention());
            Console.WriteLine("DetailedOrderLines".ToUnderscoreLowerCaseNamingConvention());
            Console.WriteLine("detailed Order lines".ToUnderscoreLowerCaseNamingConvention());
            Console.WriteLine("detailed_order lines".ToUnderscoreLowerCaseNamingConvention());
            Console.WriteLine("DetailedOrder Lines".ToUnderscoreLowerCaseNamingConvention());
            Console.WriteLine("DETAILED_ORDER_LINES".ToUnderscoreLowerCaseNamingConvention());
            Console.WriteLine("CategoryID".ToPascalNamingConvention());
            Console.WriteLine("CATEGORY_ID".ToPascalNamingConvention());
        }

        static void CreateServeralQueries()
        {
            using (var ds = new NorthwindDataService("Northwind"))
            {
                // this query is based on the dbo.Categories table
                IQueryLite<Category> query1 = ds.CategoryRepository.Query(Projection.BaseTable);

                // this query is based on the dbo.Product_Detailed view
                IQueryLite<Product> query2 = ds.ProductRepository.Query(Projection.Detailed);

                // this query is based on the dbo.ProductSale_Quarter view
                IQueryLite<ProductSale> query3 = ds.ProductSaleRepository.Query("Quarter");
            }
        }

        static void QueryByPrimaryKey()
        {
            // "Norhtwind" is the application configuration file connection string name
            using (var ds = new NorthwindDataService("Northwind"))
            {
                // reaads a category from the database by CategoryId
                // SELECT * FROM dbo.Categories WHERE CategoryId = @P0
                Category c = ds.CategoryRepository.Get(Projection.BaseTable, 1);

                // Loads the product with ProductId = 2 from the database
                // SELECT CategoryName, ProductName FROM Product_Detailed WHERE ProductId = @P0
                Product p = ds.ProductRepository.Get(Projection.Detailed, 2, ProductFields.CategoryName, ProductFields.ProductName);
            }
        }

        static void SubFilter()
        {
            using (var ds = new NorthwindDataService("Northwind"))
            {

                var subFilter = new FilterLite<Product>()
                            .Where(ProductFields.SupplierId, 1) 
                            .Or(ProductFields.SupplierId, OperatorLite.IsNull);

                // SELECT * FROM dbo.Products WHERE CategoryId = 1 AND (SupplierId = 1 OR SupplierId = 2)
                IList<Product> products = ds.ProductRepository.Query(Projection.BaseTable)
                                .Where(ProductFields.CategoryId, 1)
                                .And(subFilter)
                                .ToList();

            }
        }

        static void ShowSomeProducts()
        {
            using (var ds = new NorthwindDataService("Northwind"))
            {
               IEnumerable<Product> products = ds.ProductRepository.Query(Projection.Detailed)
                   .Fields(ProductFields.CategoryName, ProductFields.ProductName)
                   .Where(ProductFields.Discontinued, false)
                   .And(ProductFields.SupplierId, OperatorLite.In, new int[] {2, 3})
                   .And(ProductFields.UnitsInStock, OperatorLite.Greater, 0)
                   .OrderBy(ProductFields.CategoryName, ProductFields.ProductName)
                   .ToEnumerable();

                foreach (Product p in products)
                {
                    Console.WriteLine("CategoryName: {0}, ProductName: {1}", p.CategoryName, p.ProductName);
                }
            }
        }

        static void Localization()
        {
            using (var ds = new NorthwindDataService("Northwind"))
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                var c1 = ds.CategoryRepository.Query(Projection.BaseTable)
                            .Where(CategoryFields.CategoryName, "Beverages")
                            .FirstOrDefault();

                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Console.WriteLine("CategoryId: {0}, CategoryName: {1}", c1.CategoryId, c1.CategoryName);
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
                Console.WriteLine("CategoryId: {0}, CategoryName: {1}", c1.CategoryId, c1.CategoryName);

                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
                var c2 = ds.CategoryRepository.Query(Projection.BaseTable)
                            .Where(CategoryFields.CategoryName, "Bebidas")
                            .FirstOrDefault();

                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Console.WriteLine("CategoryId: {0}, CategoryName: {1}", c1.CategoryId, c1.CategoryName);
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
                Console.WriteLine("CategoryId: {0}, CategoryName: {1}", c1.CategoryId, c1.CategoryName);
            }
        }

        static void UpdateCategories()
        {
            try
            {
                using (var ds = new NorthwindDataService("Northwind"))
                {
                    var c1 = ds.CategoryRepository.Get(Projection.BaseTable, 1, FetchMode.DirectDatabaseAccess);
                    var c2 = ds.CategoryRepository.Get(Projection.BaseTable, 1, FetchMode.DirectDatabaseAccess);

                    //c1.CategoryNameLang1 = "Beverages";
                    ds.CategoryRepository.Save(c1);
                    ds.CategoryRepository.Save(c2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        static byte[] GetSalt()
        {
            var rng = System.Security.Cryptography.RNGCryptoServiceProvider.Create();
            byte[] salt = new byte[32];
            rng.GetBytes(salt);
            return salt;
        }

        static void RaiseProductPrices()
        {
            using (var ds = new Entities.NorthwindDataService("Northwind"))
            {
                ds.ProductRepository.RaiseProductPrices(0.10m);
            }
        }

        static void ShowPagedProducts()
        {
            using (var ds = new Entities.NorthwindDataService("Northwind"))
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
            for (int i = 0; i < 1000; i++)
            {
                using (var ds = new Entities.NorthwindDataService("Northwind"))
                {

                    var orderDetails = ds.OrderDetailRepository.Query(Projection.Detailed)
                        //.Where(OrderDetailFields.OrderId, 10248)
                        .ToEnumerable();

                    foreach (var od in orderDetails)
                    {
                        var categoryName = od.CategoryName;
                        var productNmae = od.ProductName;
                        var quantity = od.Quantity;
                        var subtotal = od.SubTotal;
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


        static void ShowQuesoCabralesOrders()
        {
            using (var ds = new NorthwindDataService("Northwind"))
            {
                IQueryLite<OrderDetail> orderDetailSubQuery = ds.OrderDetailRepository.Query(Projection.BaseTable)
                    .Fields(FieldsOption.None, OrderDetailFields.OrderId)
                    .Where(OrderDetailFields.ProductId, 11);

                // SELECT OrderId, OrderDate, CustomerId
                // FROM dbo.Orders
                // WHERE OrderId IN (
                //       SELECT OrderId
                //       FROM dbo.OrderDetails
                //       WHERE ProductId = 11
                //    )
                IQueryLite<Order> orderQuery = ds.OrderRepository.Query(Projection.BaseTable)
                    .Fields(OrderFields.OrderId, OrderFields.OrderDate, OrderFields.CustomerId)
                    .Where(OrderFields.OrderId, OperatorLite.In, orderDetailSubQuery);

                foreach(var order in orderQuery.ToEnumerable())
                {
                    Console.WriteLine("OrderId {0}, OrderDate {1}, CustomerId {2}", 
                        order.OrderId, order.OrderDate, order.CustomerId);
                }
            }
        }

        static void ShowLondonAndewFullerSubtree()
        {
            using (var ds = new NorthwindDataService("Northwind"))
            {
                // Andrew Fuller EmployeeId is 2
                // SELECT FirstName, LastName
                // FROM GetEmployeeSubTree(2)
                // WHERE City = 'London'
                // ORDER BY FirstName, LastName
                IQueryLite<Employee> query = new FunctionQueryLite<Employee>(ds, "dbo.GetEmployeeSubTree", 2)
                    .Fields(EmployeeFields.FirstName, EmployeeFields.LastName)
                    .Where(EmployeeFields.City, "London")
                    .OrderBy(EmployeeFields.FirstName, EmployeeFields.LastName);

                foreach(var emp in query.ToEnumerable())
                {
                    Console.WriteLine("FirstName: {0}, LastName: {1}", emp.FirstName, emp.LastName);
                }
            }
        }

        static void HandCraftedSql()
        {
            string handCraftedSqlString = "SELECT ShipperID, CompanyName FROM dbo.Shippers";
            using (var ds = new NorthwindDataService("Northwind"))
            {
                using (var cmd = ds.Connection.CreateCommand())
                {
                    cmd.CommandText = handCraftedSqlString;
                    ds.OpenConnection();
                    IEnumerable<Shipper> shippers = cmd.ExecuteReader().ToEnumerable<Shipper>();
                    foreach (var shipper in shippers)
                    {
                        Console.WriteLine("ShipperId: {0}, CompanyName: {1}", shipper.ShipperId, shipper.CompanyName);
                    }
                }
            }
        }

        static void ShowProductSales()
        {
            using (var ds = new NorthwindDataService("Northwind"))
            {
                var salesQuery = ds.ProductSaleRepository
                    .TemplatedQuery("Product", 2)
                    .Where(ProductSaleFields.Year, 1997)
                    .OrderBy(ProductSaleFields.CategoryName, ProductSaleFields.ProductName)
                    .OrderBy(ProductSaleFields.Year, ProductSaleFields.Quarter);

                foreach(var s in salesQuery.ToEnumerable(0, 9))
                {
                    Console.WriteLine("{0}, {1}, {2}, {3}, {4}", 
                        s.CategoryName, s.ProductName, s.Year, s.Quarter, s.Sales);
                }
            }
        }

        static void InsertUpdateDeleteProduct()
        {
            using (var ds = new Entities.NorthwindDataService("Northwind"))
            {
                //ds.DefaultSchemaName = "NORTHWIND";
                ds.BeginTransaction();
                var p = new Entities.Product
                {
                    CategoryId = 2,
                    ProductName = "New Product",
                    QuantityPerUnit = "2",
                    ReorderLevel = 50,
                    SupplierId = 2,
                    UnitPrice = 10,
                    UnitsInStock = 1,
                    UnitsOnOrder = 0
                  
                };

                // inserts the new product
                ds.ProductRepository.Save(p);

                Console.WriteLine("Inserted product id:" + p.ProductId);

                p.ProductName = "Another Name";

                // updates the product
                ds.ProductRepository.Save(p);

                p = ds.ProductRepository.Get(Projection.Detailed, p.ProductId);

                Console.WriteLine("CategoryName:" + p.CategoryName);

                p = ds.ProductRepository.Get(Projection.BaseTable, p.ProductId);

                ds.ProductRepository.Delete(p.ProductId);

                ds.Commit();
            }
        }
    }
}
