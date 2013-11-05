

using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.SqlServer.Types;
using System.Runtime.Serialization;
using inercya.EntityLite;	
using inercya.EntityLite.Extensions;		

namespace Samples.Entities
{
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Products")]
	public partial class Product
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, IsKey=true, IsAutoincrement=true, IsReadOnly = true, BaseColumnName = "ProductID", BaseTableName="Products" )]
		public Int32 ProductID { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, 255, 255, BaseColumnName = "ProductName", BaseTableName="Products" )]
		public String ProductName { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, AllowNull = true, BaseColumnName = "SupplierID", BaseTableName="Products" )]
		public Int32? SupplierID { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, AllowNull = true, BaseColumnName = "CategoryID", BaseTableName="Products" )]
		public Int32? CategoryID { get; set; }

		[DataMember]
		[SqlField(DbType.String, 20, 255, 255, BaseColumnName = "QuantityPerUnit", BaseTableName="Products" )]
		public String QuantityPerUnit { get; set; }

		[DataMember]
		[SqlField(DbType.Currency, 8, 19, 255, AllowNull = true, BaseColumnName = "UnitPrice", BaseTableName="Products" )]
		public Decimal? UnitPrice { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, 5, 255, AllowNull = true, BaseColumnName = "UnitsInStock", BaseTableName="Products" )]
		public Int16? UnitsInStock { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, 5, 255, AllowNull = true, BaseColumnName = "UnitsOnOrder", BaseTableName="Products" )]
		public Int16? UnitsOnOrder { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, 5, 255, AllowNull = true, BaseColumnName = "ReorderLevel", BaseTableName="Products" )]
		public Int16? ReorderLevel { get; set; }

		[DataMember]
		[SqlField(DbType.Boolean, 1, 255, 255, BaseColumnName = "Discontinued", BaseTableName="Products" )]
		public Boolean Discontinued { get; set; }

		[DataMember]
		[SqlField(DbType.Currency, 8, 19, 255, AllowNull = true, BaseColumnName = "Total", BaseTableName="Products" )]
		public Decimal? Total { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "CategoryName", BaseTableName="Categories" )]
		public String CategoryName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, 255, 255, BaseColumnName = "CompanyName", BaseTableName="Suppliers" )]
		public String SupplierName { get; set; }


	}

	public partial class ProductRepository : Repository<Product> 
	{
		public ProductRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Product Get(string projectionName, System.Int32 productID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).Get(projectionName, productID, fetchMode);
		}

		public Product Get(Projection projection, System.Int32 productID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).Get(projection, productID, fetchMode);
		}

		public Product Get(string projectionName, System.Int32 productID, params string[] fields)
		{
			return ((IRepository<Product>)this).Get(projectionName, productID, fields);
		}

		public Product Get(Projection projection, System.Int32 productID, params string[] fields)
		{
			return ((IRepository<Product>)this).Get(projection, productID, fields);
		}

		public void Delete(System.Int32 productID)
		{
			var entity = new Product { ProductID = productID };
			this.Delete(entity);
		}
	}

	public static partial class ProductFields
	{
		public const string ProductID = "ProductID";
		public const string ProductName = "ProductName";
		public const string SupplierID = "SupplierID";
		public const string CategoryID = "CategoryID";
		public const string QuantityPerUnit = "QuantityPerUnit";
		public const string UnitPrice = "UnitPrice";
		public const string UnitsInStock = "UnitsInStock";
		public const string UnitsOnOrder = "UnitsOnOrder";
		public const string ReorderLevel = "ReorderLevel";
		public const string Discontinued = "Discontinued";
		public const string Total = "Total";
		public const string CategoryName = "CategoryName";
		public const string SupplierName = "SupplierName";
	}

}

namespace Samples.Entities.Security
{
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Users", SchemaName="Security")]
	public partial class User
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, IsKey=true, IsAutoincrement=true, IsReadOnly = true, BaseColumnName = "UserId", BaseTableName="Users" )]
		public Int32 UserId { get; set; }

		[DataMember]
		[SqlField(DbType.AnsiString, 256, 255, 255, BaseColumnName = "LoginName", BaseTableName="Users" )]
		public String LoginName { get; set; }

		[DataMember]
		[SqlField(DbType.AnsiString, 256, 255, 255, BaseColumnName = "UserName", BaseTableName="Users" )]
		public String UserName { get; set; }

		[DataMember]
		[SqlField(DbType.AnsiString, 256, 255, 255, BaseColumnName = "EmailAddress", BaseTableName="Users" )]
		public String EmailAddress { get; set; }

		[DataMember]
		[SqlField(DbType.Boolean, 1, 255, 255, BaseColumnName = "IsActive", BaseTableName="Users" )]
		public Boolean IsActive { get; set; }

		[DataMember]
		[SqlField(DbType.Binary, 64, 255, 255, BaseColumnName = "UserPasswordHash", BaseTableName="Users" )]
		public Byte[] UserPasswordHash { get; set; }

		[DataMember]
		[SqlField(DbType.Binary, 32, 255, 255, BaseColumnName = "UserPassworkdSalt", BaseTableName="Users" )]
		public Byte[] UserPassworkdSalt { get; set; }

		[DataMember]
		[SqlField(DbType.DateTimeOffset, 10, 255, 7, BaseColumnName = "CreatedDate", BaseTableName="Users" )]
		public DateTimeOffset CreatedDate { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, BaseColumnName = "CreatedBy", BaseTableName="Users" )]
		public Int32 CreatedBy { get; set; }

		[DataMember]
		[SqlField(DbType.DateTimeOffset, 10, 255, 7, BaseColumnName = "ModifiedDate", BaseTableName="Users" )]
		public DateTimeOffset ModifiedDate { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, BaseColumnName = "ModifiedBy", BaseTableName="Users" )]
		public Int32 ModifiedBy { get; set; }


	}

	public partial class UserRepository : Repository<User> 
	{
		public UserRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public User Get(string projectionName, System.Int32 userId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<User>)this).Get(projectionName, userId, fetchMode);
		}

		public User Get(Projection projection, System.Int32 userId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<User>)this).Get(projection, userId, fetchMode);
		}

		public User Get(string projectionName, System.Int32 userId, params string[] fields)
		{
			return ((IRepository<User>)this).Get(projectionName, userId, fields);
		}

		public User Get(Projection projection, System.Int32 userId, params string[] fields)
		{
			return ((IRepository<User>)this).Get(projection, userId, fields);
		}

		public void Delete(System.Int32 userId)
		{
			var entity = new User { UserId = userId };
			this.Delete(entity);
		}
	}

	public static partial class UserFields
	{
		public const string UserId = "UserId";
		public const string LoginName = "LoginName";
		public const string UserName = "UserName";
		public const string EmailAddress = "EmailAddress";
		public const string IsActive = "IsActive";
		public const string UserPasswordHash = "UserPasswordHash";
		public const string UserPassworkdSalt = "UserPassworkdSalt";
		public const string CreatedDate = "CreatedDate";
		public const string CreatedBy = "CreatedBy";
		public const string ModifiedDate = "ModifiedDate";
		public const string ModifiedBy = "ModifiedBy";
	}

}

namespace Samples.Entities
{
	public partial class NorhtwindDataService : DataService
	{
        public NorhtwindDataService(string connectionStringName) : base(connectionStringName)
        {
        }

        public NorhtwindDataService(string connectionString, string providerName) : base(connectionString, providerName)
        {
        }

		private Samples.Entities.ProductRepository _ProductRepository;
		public Samples.Entities.ProductRepository ProductRepository
		{
			get 
			{
				if ( _ProductRepository == null)
				{
					_ProductRepository = new Samples.Entities.ProductRepository(this);
				}
				return _ProductRepository;
			}
		}

		private Samples.Entities.Security.UserRepository _SecurityUserRepository;
		public Samples.Entities.Security.UserRepository SecurityUserRepository
		{
			get 
			{
				if ( _SecurityUserRepository == null)
				{
					_SecurityUserRepository = new Samples.Entities.Security.UserRepository(this);
				}
				return _SecurityUserRepository;
			}
		}
	}
}
