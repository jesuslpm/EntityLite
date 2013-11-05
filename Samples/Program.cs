using inercya.EntityLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //InsertUpdateDeleteProduct();
            InsertUpdateDeleteUser();
            
        }

        static byte[] GetSalt()
        {
            var rng = System.Security.Cryptography.RNGCryptoServiceProvider.Create();
            byte[] salt = new byte[32];
            rng.GetBytes(salt);
            return salt;
        }

        static byte[] GetSecureHash(string password, byte[] salt)
        {
            var rfc = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
            return rfc.GetBytes(64);
        }

        static void InsertUpdateDeleteUser()
        {
            using (var ds = new Entities.NorhtwindDataService("Northwind"))
            {
                ds.CurrentUserId = 1;
                var salt = GetSalt();
                var u = new Entities.Security.User
                {
                    EmailAddress = "jesuslpm@hotmail.com",
                    IsActive = true,
                    LoginName = "jesuslpm",
                    UserName = "Jesús López",
                    UserPasswordHash = GetSecureHash("€ntity£it€1", salt),
                    UserPassworkdSalt = salt
                };

                ds.SecurityUserRepository.Save(u);
                Console.WriteLine("Inserted UserId:" + u.UserId);

                u.UserName = "Jesús López Méndez";
                ds.SecurityUserRepository.Save(u);

                u = ds.SecurityUserRepository.Get(Projection.BaseTable, u.UserId);

                Console.WriteLine("User Name: " + u.UserName);

                ds.SecurityUserRepository.Delete(u.UserId);
            }
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
                    UnitsOnOrder = 0
                };

                // inserts the new product
                ds.ProductRepository.Save(p);

                Console.WriteLine("Inserted product id:" + p.ProductID);

                p.ProductName = "Another Name";

                // updates the product
                ds.ProductRepository.Save(p);

                p = ds.ProductRepository.Get(inercya.EntityLite.Projection.Detailed, p.ProductID);

                Console.WriteLine("CategoryName:" + p.CategoryName);

                ds.ProductRepository.Delete(p.ProductID);

            }
        }
    }
}
