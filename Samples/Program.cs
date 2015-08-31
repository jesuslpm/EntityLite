/*
Copyright 2014 i-nercya intelligent software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using inercya.EntityLite.SqliteProfiler;
using inercya.EntityLite.Collections;
using System.ComponentModel;

namespace Samples
{
    class Program
    {

        static NorthwindDataService ds;
        static Profiler profiler;

        static void Main(string[] args)
        {
            //for (int i =0; i < 100; i++) TestQueue();
            profiler = new inercya.EntityLite.SqliteProfiler.Profiler(
                AppDomain.CurrentDomain.BaseDirectory, 
                ProfileFileFrecuency.Daily,
                true
            );
            ProfilerLite.Current = profiler;
            profiler.StartProfiling();
            using (ds = new NorthwindDataService())
            {
                //SingleTest(50000, InsertSingleItemEntityLite);
                //SequenceTest();
                //QueryByPrimaryKey();
                //ShowSomeProducts();
                //ShowOrderDetails();
                //ShowQuesoCabralesOrders();
                //ShowPagedProducts();
                //ShowLondonAndewFullerSubtree();
                //SearchOrderDetails();
                //ShowProductSales();
                //RaiseProductPrices();
                //InsertUpdateDeleteProduct();
                //RaiseProductPrices2();
                //HandCraftedSql();
                //Localization();
                //WillFail();
                //BuggyShowQuesoCabralesOrders();

                //Pivot();

                ShowAllEmployeesThatSoldSpecifiedProducts();
            }
            profiler.StopProfiling();
            Console.WriteLine("Press enter to exit ...");
            Console.ReadLine();
           
        }

        static void ShowAllEmployeesThatSoldSpecifiedProducts()
        {

            var query = ds.EmployeeRepository.ThatSoldAllSpecifiedProductsQuery(Enumerable.Range(1, 6))
                .Fields(EmployeeFields.EmployeeId, EmployeeFields.FirstName, EmployeeFields.LastName)
                .OrderBy(EmployeeFields.FirstName, EmployeeFields.LastName);



            var any = query.Any();

            foreach (var e in query.ToEnumerable())
            {
                Console.WriteLine("{0}: {1} {2}", e.EmployeeId, e.FirstName, e.LastName);
            }
        }

        static void InsertMultipleItems(int itemCount)
        {
            ds.BeginTransaction();
            for (int i = 1; i < itemCount; i++)
            {
                var item = new Entities.Item
                {
                    Field1 = "Field 1." + i.ToString(),
                    Field2 = "Field 2." + i.ToString(),
                    Field3 = "Field 3." + i.ToString(),
                    Field4 = "Field 4." + i.ToString()
                };
                ds.ItemRepository.Insert(item);
            }
            ds.Commit();
        }

        static void InsertSingleItemEntityLite(int i)
        {
            using (var ds = new NorthwindDataService())
            {
                var item = new Entities.Item
                {
                    Field1 = "Field 1." + i.ToString(),
                    Field2 = "Field 2." + i.ToString(),
                    Field3 = "Field 3." + i.ToString(),
                    Field4 = "Field 4." + i.ToString()
                };
                ds.ItemRepository.Insert(item);
            }
        }

        static void MultipleTest(int itemCount, Action<int> testAction)
        {

            Console.WriteLine(testAction.Method.Name);
            testAction(2); // warm up
            ds.ItemRepository.Truncate();
            var watch = Stopwatch.StartNew();
            testAction(itemCount);
            watch.Stop();
            Console.WriteLine((int)watch.Elapsed.TotalMilliseconds);
        }

        static void SingleTest(int itemCount, Action<int> testAction)
        {
            ds.ItemRepository.Truncate();
            Console.WriteLine(testAction.Method.Name);
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < itemCount; i++)
            {
                testAction(i);
            }
            watch.Stop();
            Console.WriteLine((int)watch.Elapsed.TotalMilliseconds);
        }

        private static void SequenceTest()
        {
            for (int i = 0; i < 20; i++)
            {
                var e = new MyEntity { Value = "hola secuencias" };
                ds.MyEntityRepository.Save(e);
                Console.WriteLine(e.EntityId);
            }

        }

        private static void WillFail()
        {
            try
            {
                var od = new OrderDetail
                {

                    OrderId = 1,
                    ProductId = 11,
                    UnitPrice = 14,
                    Quantity = 12

                };
                ds.OrderDetailRepository.Save(od);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private static void Pivot()
        {
            DataTable pivotedSales = ds.ProductSaleRepository.Query("Year")
                .OrderBy(ProductSaleFields.ProductName)
                .Pivot(
                    (c1, c2) =>
                    {
                        int yearComp = ((int)c1.PivotColumnValue).CompareTo(c2.PivotColumnValue);
                        if (yearComp != 0) return yearComp;
                        return c1.PivotTransformIndex.CompareTo(c2.PivotTransformIndex);
                    },
                    new PivotTransform(ProductSaleFields.Year, ProductSaleFields.Sales, x => "Y" + x.ToString() + "Sales"),
                    new PivotTransform(ProductSaleFields.Year, ProductSaleFields.OrderCount, x => "Y" + x.ToString() + "OrderCount")
                );
        }

        //private static void EmployeeSubTree_RefCursor()
        //{
        //    using (var ds = new NorthwindDataService())
        //    {
        //        object cSubTree;
        //        ds.EmployeeRepository.EmployeeSubtree(2, out cSubTree);
        //        IDataReader reader = ((dynamic)cSubTree).GetDataReader();
                
        //        foreach(var employee in reader.ToList<Employee>())
        //        {
        //            Console.WriteLine("{0}, {1}", employee.FirstName, employee.LastName);
        //        }
        //    }
        //}

        private static void SearchOrderDetails()
        {
            Console.WriteLine("\nSearchOrderDetails\n");
            var criteria = new OrderDetailSearchCriteria
            {
                ProductName = "C",
                CustomerId = "AROUT"
            };

            var orderDetails = ds.OrderDetailRepository.SearchQuery(criteria)
                .Fields(OrderDetailFields.OrderId, OrderDetailFields.OrderDate, OrderDetailFields.ProductName, OrderDetailFields.SubTotal)
                .OrderByDesc(OrderDetailFields.OrderDate)
                .OrderBy(OrderDetailFields.ProductName)
                .ToList(0, 9);

            foreach (var od in orderDetails)
            {
                Console.WriteLine("{0}, {1},  {2}, {3}", od.OrderId, od.OrderDate, od.ProductName, od.SubTotal);
            }
        }


        private static void RaiseProductPrices2()
        {
            Console.WriteLine("\nRaiseProductPrices2\n");
            ds.ProductRepository.RaiseProductPrices(1, 0.10m);
            Console.WriteLine("Product prices raised");
        }

        private static void NamingConventionTests()
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
            Console.WriteLine("\nCreateServeralQueries\n");

                // this query is based on the dbo.Categories table
                IQueryLite<Category> query1 = ds.CategoryRepository.Query(Projection.BaseTable);
                Console.WriteLine("Query created");

                // this query is based on the dbo.Product_Detailed view
                IQueryLite<Product> query2 = ds.ProductRepository.Query(Projection.Detailed);
                Console.WriteLine("Query created");

                // this query is based on the dbo.ProductSale_Quarter view
                IQueryLite<ProductSale> query3 = ds.ProductSaleRepository.Query("Quarter");
                Console.WriteLine("Query created");
        }

        static void QueryByPrimaryKey()
        {
            Console.WriteLine("\nQueryByPrimaryKey\n");

            // reaads a category from the database by CategoryId
            // SELECT * FROM dbo.Categories WHERE CategoryId = @P0
            Category c = ds.CategoryRepository.Get(Projection.BaseTable, 1);
            Console.WriteLine("Category {0}, {1}", c.CategoryId, c.CategoryName);

            // Loads the product with ProductId = 2 from the database
            // SELECT CategoryName, ProductName FROM Product_Detailed WHERE ProductId = @P0
            Product p = ds.ProductRepository.Get(Projection.Detailed, 2, ProductFields.CategoryName, ProductFields.ProductName);
            Console.WriteLine("{0}, {1}", p.CategoryName, p.ProductName);
        }

        static void SubFilter()
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

        static void ShowSomeProducts()
        {
            Console.WriteLine("\nShowSomeProducts\n");
            IEnumerable<Product> products = ds.ProductRepository.Query(Projection.Detailed)
                .Fields(ProductFields.CategoryName, ProductFields.ProductName)
                .Where(ProductFields.Discontinued, false)
                .And(ProductFields.SupplierId, OperatorLite.In, new int[] { 2, 3 })
                .And(ProductFields.UnitsInStock, OperatorLite.Greater, 0)
                .OrderBy(ProductFields.CategoryName, ProductFields.ProductName)
                .ToEnumerable();

            foreach (Product p in products)
            {
                Console.WriteLine("CategoryName: {0}, ProductName: {1}", p.CategoryName, p.ProductName);
            }
        }

        static void Localization()
        {
            Console.WriteLine("\nLocalization\n");

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            // the filter must work, it shoud use WHERE CategoryNameLang1 = 'Beverages', because Lang1 is the current language;
            // CategoryName is a magic field, it doesn't exist in the database, its is replaced by CategoryNameLang1 or CategoryNameLang2 
            // depending on the current culture
            var c1 = ds.CategoryRepository.Query(Projection.BaseTable)
                        .Where(CategoryFields.CategoryName, "Beverages")
                        .FirstOrDefault();


            // CategoryName is a magic field, it doesn't exist in the database, it returns either CategoryNameLang1 or CategoryNameLang2
            // depending on the current culture.
            // It should show: Beverages, Beverages, Bebidas
            Console.WriteLine("CurrentLanguage {0}, Lang1: {1}, Lang2: {2}", c1.CategoryName, c1.CategoryNameLang1, c1.CategoryNameLang2);

            // changing the current culture should change the ouptput
            // it should show: Bebidas, Beverages, Bebidas
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
            Console.WriteLine("CurrentLanguage {0}, Lang1: {1}, Lang2: {2}", c1.CategoryName, c1.CategoryNameLang1, c1.CategoryNameLang2);


            // it shoud use WHERE CategoryNameLang2 = 'Bebidas' because Lang2 is the current language
            c1 = ds.CategoryRepository.Query(Projection.BaseTable)
                        .Where(CategoryFields.CategoryName, "Bebidas")
                        .FirstOrDefault();

            // it should show: Bebidas, Beverages, Bebidas
            Console.WriteLine("CurrentLanguage {0}, Lang1: {1}, Lang2: {2}", c1.CategoryName, c1.CategoryNameLang1, c1.CategoryNameLang2);


            // changing the current culture should change the ouptput
            // it should show: Beverages, Beverages, Bebidas
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Console.WriteLine("CurrentLanguage {0}, Lang1: {1}, Lang2: {2}", c1.CategoryName, c1.CategoryNameLang1, c1.CategoryNameLang2);

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
            Console.WriteLine("\nRaiseProductPrices\n");
            ds.ProductRepository.RaiseProductPrices(0.10m);
            Console.WriteLine("Product prices raised");
        }

        static void ShowPagedProducts()
        {
            Console.WriteLine("\nShowPagedProducts\n");
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

        static void ShowOrderDetails()
        {
            var orderDetails = ds.OrderDetailRepository.Query(Projection.Detailed)
                .Where(OrderDetailFields.OrderId, 10248)
                .ToEnumerable();

            foreach (var od in orderDetails)
            {

                Console.WriteLine("{0}, {1}, {2}, {3}", od.CategoryName, od.ProductName, od.Quantity, od.SubTotal);
            }
        }

        static byte[] GetSecureHash(string password, byte[] salt)
        {
            var rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
            return rfc.GetBytes(64);
        }


        static void ShowQuesoCabralesOrders()
        {
            Console.WriteLine("\nShowQuesoCabralesOrders\n");

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

            foreach (var order in orderQuery.ToEnumerable())
            {
                Console.WriteLine("OrderId {0}, OrderDate {1}, CustomerId {2}",
                    order.OrderId, order.OrderDate, order.CustomerId);
            }
        }

        static void BuggyShowQuesoCabralesOrders()
        {
            Console.WriteLine("\nBuggyShowQuesoCabralesOrders\n");

            try
            {

                IQueryLite<OrderDetail> orderDetailSubQuery = ds.OrderDetailRepository.Query(Projection.BaseTable)
                    .Fields(OrderDetailFields.OrderId)
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

                foreach (var order in orderQuery.ToEnumerable())
                {
                    Console.WriteLine("OrderId {0}, OrderDate {1}, CustomerId {2}",
                        order.OrderId, order.OrderDate, order.CustomerId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ShowLondonAndewFullerSubtree()
        {
            Console.WriteLine("\nShowLondonAndewFullerSubtree\n");

            // Andrew Fuller EmployeeId is 2
            // SELECT FirstName, LastName
            // FROM GetEmployeeSubTree(2)
            // WHERE City = 'London'
            // ORDER BY FirstName, LastName
            IQueryLite<Employee> query = new FunctionQueryLite<Employee>(ds, "dbo.GetEmployeeSubTree", 2)
                .Fields(EmployeeFields.FirstName, EmployeeFields.LastName)
                .Where(EmployeeFields.City, "London")
                .OrderBy(EmployeeFields.FirstName, EmployeeFields.LastName);

            foreach (var emp in query.ToEnumerable())
            {
                Console.WriteLine("FirstName: {0}, LastName: {1}", emp.FirstName, emp.LastName);
            }
        }


        static void HandCraftedSql()
        {
            Console.WriteLine("\nHandCraftedSql\n");
            string handCraftedSqlString = "SELECT ShipperID, CompanyName FROM dbo.Shippers";

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

        static void ShowProductSales()
        {
            Console.WriteLine("\nShowProductSales\n");

            var salesQuery = ds.ProductSaleRepository
                .TemplatedQuery("Product", 2)
                .Where(ProductSaleFields.Year, 1997)
                .OrderBy(ProductSaleFields.CategoryName, ProductSaleFields.ProductName)
                .OrderBy(ProductSaleFields.Year, ProductSaleFields.Quarter);

            foreach (var s in salesQuery.ToEnumerable(0, 9))
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                    s.CategoryName, s.ProductName, s.Year, s.Quarter, s.Sales);
            }
        }

        static void InsertUpdateDeleteProduct()
        {
            Console.WriteLine("\nInsertUpdateDeleteProduct\n");

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
            var saveResult = ds.ProductRepository.Save(p);
            Console.WriteLine("Inserted product id:" + p.ProductId);

            p.ProductName = "Another Name";
            // updates the product
            var result = ds.ProductRepository.Save(p);
            Console.WriteLine("Product name changed");

            // Retrieves the product from the database and shows the product category name
            p = ds.ProductRepository.Get(Projection.Detailed, p.ProductId);
            Console.WriteLine("CategoryName:" + p.CategoryName);

            // deletes the product
            ds.ProductRepository.Delete(p.ProductId);
            Console.WriteLine("Product deleted");

            ds.Commit();
        }

        static volatile bool isRunning = true;

        static void TestQueue()
        {

            Thread[] threads = new Thread[8];
            var queue = new SafeQueue<int>();
            var signal = new AutoResetEvent(false);

            HashSet<int> integers = new HashSet<int>();

            isRunning = true;

            Thread queueReader = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    signal.WaitOne();
                    int n;
                    int count = 0;
                    while (queue.Dequeue(out n))
                    {
                        integers.Add(n);
                        count++;
                    }
                    //Console.WriteLine(count);
                    if (!isRunning) return;
                }
            }));

            queueReader.Start();

            Action<int> enqueue = (s) =>
            {
                for (int x = 0; x < 100000; x++)
                {
                    queue.Enqueue(s + x);
                    signal.Set();
                    if (x % 100 == 0) Thread.Yield();
                }
            };

            int start = 0;
            for (int i = 0; i < 8; i++)
            {

                threads[i] = new Thread(new ParameterizedThreadStart((state) => enqueue((int)state)));
                threads[i].Start(start);
                start += 100000;
            }

            for (int i = 0; i < 8; i++)
            {
                threads[i].Join();
            }
            Thread.Yield();
            isRunning = false;
            signal.Set();

            queueReader.Join();

            bool failed = false;
            for (int i = 0; i < 800000; i++)
            {
                if (!integers.Contains(i))
                {
                    //Console.WriteLine("{0} failed", i);
                    failed = true;
                }
            }
            if (failed) Console.WriteLine("Test failed");
        }
    }

}
