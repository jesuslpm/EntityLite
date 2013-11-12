

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
	[SqlEntity(BaseTableName="Categories")]
	public partial class Category
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, IsKey=true, IsAutoincrement=true, IsReadOnly = true, BaseColumnName = "CategoryID", BaseTableName="Categories" )]
		public Int32 CategoryID { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "CategoryName", BaseTableName="Categories" )]
		public String CategoryName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 2147483647, 255, 255, BaseColumnName = "Description", BaseTableName="Categories" )]
		public String Description { get; set; }

		[DataMember]
		[SqlField(DbType.Binary, 2147483647, 255, 255, BaseColumnName = "Picture", BaseTableName="Categories" )]
		public Byte[] Picture { get; set; }


	}

	public partial class CategoryRepository : Repository<Category> 
	{
		public CategoryRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Category Get(string projectionName, System.Int32 categoryID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryID, fetchMode);
		}

		public Category Get(Projection projection, System.Int32 categoryID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).Get(projection, categoryID, fetchMode);
		}

		public Category Get(string projectionName, System.Int32 categoryID, params string[] fields)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryID, fields);
		}

		public Category Get(Projection projection, System.Int32 categoryID, params string[] fields)
		{
			return ((IRepository<Category>)this).Get(projection, categoryID, fields);
		}

		public void Delete(System.Int32 categoryID)
		{
			var entity = new Category { CategoryID = categoryID };
			this.Delete(entity);
		}
	}

	public static partial class CategoryFields
	{
		public const string CategoryID = "CategoryID";
		public const string CategoryName = "CategoryName";
		public const string Description = "Description";
		public const string Picture = "Picture";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Customers")]
	public partial class Customer
	{
		[DataMember]
		[SqlField(DbType.StringFixedLength, 5, 255, 255, IsKey=true, BaseColumnName = "CustomerID", BaseTableName="Customers" )]
		public String CustomerID { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, 255, 255, BaseColumnName = "CompanyName", BaseTableName="Customers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, 255, 255, BaseColumnName = "ContactName", BaseTableName="Customers" )]
		public String ContactName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, 255, 255, BaseColumnName = "ContactTitle", BaseTableName="Customers" )]
		public String ContactTitle { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, 255, 255, BaseColumnName = "Address", BaseTableName="Customers" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "City", BaseTableName="Customers" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "Region", BaseTableName="Customers" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, 255, 255, BaseColumnName = "PostalCode", BaseTableName="Customers" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "Country", BaseTableName="Customers" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, 255, 255, BaseColumnName = "Phone", BaseTableName="Customers" )]
		public String Phone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, 255, 255, BaseColumnName = "Fax", BaseTableName="Customers" )]
		public String Fax { get; set; }


	}

	public partial class CustomerRepository : Repository<Customer> 
	{
		public CustomerRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Customer Get(string projectionName, System.String customerID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerID, fetchMode);
		}

		public Customer Get(Projection projection, System.String customerID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projection, customerID, fetchMode);
		}

		public Customer Get(string projectionName, System.String customerID, params string[] fields)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerID, fields);
		}

		public Customer Get(Projection projection, System.String customerID, params string[] fields)
		{
			return ((IRepository<Customer>)this).Get(projection, customerID, fields);
		}

		public void Delete(System.String customerID)
		{
			var entity = new Customer { CustomerID = customerID };
			this.Delete(entity);
		}
	}

	public static partial class CustomerFields
	{
		public const string CustomerID = "CustomerID";
		public const string CompanyName = "CompanyName";
		public const string ContactName = "ContactName";
		public const string ContactTitle = "ContactTitle";
		public const string Address = "Address";
		public const string City = "City";
		public const string Region = "Region";
		public const string PostalCode = "PostalCode";
		public const string Country = "Country";
		public const string Phone = "Phone";
		public const string Fax = "Fax";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Employees")]
	public partial class Employee
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, IsKey=true, IsAutoincrement=true, IsReadOnly = true, BaseColumnName = "EmployeeID", BaseTableName="Employees" )]
		public Int32 EmployeeID { get; set; }

		[DataMember]
		[SqlField(DbType.String, 20, 255, 255, BaseColumnName = "LastName", BaseTableName="Employees" )]
		public String LastName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, 255, 255, BaseColumnName = "FirstName", BaseTableName="Employees" )]
		public String FirstName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, 255, 255, BaseColumnName = "Title", BaseTableName="Employees" )]
		public String Title { get; set; }

		[DataMember]
		[SqlField(DbType.String, 25, 255, 255, BaseColumnName = "TitleOfCourtesy", BaseTableName="Employees" )]
		public String TitleOfCourtesy { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, 23, 3, AllowNull = true, BaseColumnName = "BirthDate", BaseTableName="Employees" )]
		public DateTime? BirthDate { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, 23, 3, AllowNull = true, BaseColumnName = "HireDate", BaseTableName="Employees" )]
		public DateTime? HireDate { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, 255, 255, BaseColumnName = "Address", BaseTableName="Employees" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "City", BaseTableName="Employees" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "Region", BaseTableName="Employees" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, 255, 255, BaseColumnName = "PostalCode", BaseTableName="Employees" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "Country", BaseTableName="Employees" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, 255, 255, BaseColumnName = "HomePhone", BaseTableName="Employees" )]
		public String HomePhone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 4, 255, 255, BaseColumnName = "Extension", BaseTableName="Employees" )]
		public String Extension { get; set; }

		[DataMember]
		[SqlField(DbType.Binary, 2147483647, 255, 255, BaseColumnName = "Photo", BaseTableName="Employees" )]
		public Byte[] Photo { get; set; }

		[DataMember]
		[SqlField(DbType.String, 1073741823, 255, 255, BaseColumnName = "Notes", BaseTableName="Employees" )]
		public String Notes { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, AllowNull = true, BaseColumnName = "ReportsTo", BaseTableName="Employees" )]
		public Int32? ReportsTo { get; set; }

		[DataMember]
		[SqlField(DbType.String, 255, 255, 255, BaseColumnName = "PhotoPath", BaseTableName="Employees" )]
		public String PhotoPath { get; set; }


	}

	public partial class EmployeeRepository : Repository<Employee> 
	{
		public EmployeeRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Employee Get(string projectionName, System.Int32 employeeID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeID, fetchMode);
		}

		public Employee Get(Projection projection, System.Int32 employeeID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projection, employeeID, fetchMode);
		}

		public Employee Get(string projectionName, System.Int32 employeeID, params string[] fields)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeID, fields);
		}

		public Employee Get(Projection projection, System.Int32 employeeID, params string[] fields)
		{
			return ((IRepository<Employee>)this).Get(projection, employeeID, fields);
		}

		public void Delete(System.Int32 employeeID)
		{
			var entity = new Employee { EmployeeID = employeeID };
			this.Delete(entity);
		}
	}

	public static partial class EmployeeFields
	{
		public const string EmployeeID = "EmployeeID";
		public const string LastName = "LastName";
		public const string FirstName = "FirstName";
		public const string Title = "Title";
		public const string TitleOfCourtesy = "TitleOfCourtesy";
		public const string BirthDate = "BirthDate";
		public const string HireDate = "HireDate";
		public const string Address = "Address";
		public const string City = "City";
		public const string Region = "Region";
		public const string PostalCode = "PostalCode";
		public const string Country = "Country";
		public const string HomePhone = "HomePhone";
		public const string Extension = "Extension";
		public const string Photo = "Photo";
		public const string Notes = "Notes";
		public const string ReportsTo = "ReportsTo";
		public const string PhotoPath = "PhotoPath";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="OrderDetails")]
	public partial class OrderDetail
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, IsKey=true, IsAutoincrement=true, IsReadOnly = true, BaseColumnName = "OrderDetailID", BaseTableName="OrderDetails" )]
		public Int32 OrderDetailID { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, BaseColumnName = "OrderID", BaseTableName="OrderDetails" )]
		public Int32 OrderID { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, BaseColumnName = "ProductID", BaseTableName="OrderDetails" )]
		public Int32 ProductID { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, 19, 4, BaseColumnName = "UnitPrice", BaseTableName="OrderDetails" )]
		public Decimal UnitPrice { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, 5, 255, BaseColumnName = "Quantity", BaseTableName="OrderDetails" )]
		public Int16 Quantity { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, 5, 4, BaseColumnName = "Discount", BaseTableName="OrderDetails" )]
		public Decimal Discount { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, 32, 8, AllowNull = true, IsReadOnly = true, BaseColumnName = "SubTotal", BaseTableName="" )]
		public Decimal? SubTotal { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, 255, 255, BaseColumnName = "ProductName", BaseTableName="Products" )]
		public String ProductName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "CategoryName", BaseTableName="Categories" )]
		public String CategoryName { get; set; }


	}

	public partial class OrderDetailRepository : Repository<OrderDetail> 
	{
		public OrderDetailRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public OrderDetail Get(string projectionName, System.Int32 orderDetailID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<OrderDetail>)this).Get(projectionName, orderDetailID, fetchMode);
		}

		public OrderDetail Get(Projection projection, System.Int32 orderDetailID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<OrderDetail>)this).Get(projection, orderDetailID, fetchMode);
		}

		public OrderDetail Get(string projectionName, System.Int32 orderDetailID, params string[] fields)
		{
			return ((IRepository<OrderDetail>)this).Get(projectionName, orderDetailID, fields);
		}

		public OrderDetail Get(Projection projection, System.Int32 orderDetailID, params string[] fields)
		{
			return ((IRepository<OrderDetail>)this).Get(projection, orderDetailID, fields);
		}

		public void Delete(System.Int32 orderDetailID)
		{
			var entity = new OrderDetail { OrderDetailID = orderDetailID };
			this.Delete(entity);
		}
	}

	public static partial class OrderDetailFields
	{
		public const string OrderDetailID = "OrderDetailID";
		public const string OrderID = "OrderID";
		public const string ProductID = "ProductID";
		public const string UnitPrice = "UnitPrice";
		public const string Quantity = "Quantity";
		public const string Discount = "Discount";
		public const string SubTotal = "SubTotal";
		public const string ProductName = "ProductName";
		public const string CategoryName = "CategoryName";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Orders")]
	public partial class Orders
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, IsKey=true, IsAutoincrement=true, IsReadOnly = true, BaseColumnName = "OrderID", BaseTableName="Orders" )]
		public Int32 OrderID { get; set; }

		[DataMember]
		[SqlField(DbType.StringFixedLength, 5, 255, 255, BaseColumnName = "CustomerID", BaseTableName="Orders" )]
		public String CustomerID { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, AllowNull = true, BaseColumnName = "EmployeeID", BaseTableName="Orders" )]
		public Int32? EmployeeID { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, 23, 3, AllowNull = true, BaseColumnName = "OrderDate", BaseTableName="Orders" )]
		public DateTime? OrderDate { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, 23, 3, AllowNull = true, BaseColumnName = "RequiredDate", BaseTableName="Orders" )]
		public DateTime? RequiredDate { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, 23, 3, AllowNull = true, BaseColumnName = "ShippedDate", BaseTableName="Orders" )]
		public DateTime? ShippedDate { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, AllowNull = true, BaseColumnName = "ShipVia", BaseTableName="Orders" )]
		public Int32? ShipVia { get; set; }

		[DataMember]
		[SqlField(DbType.Currency, 8, 19, 255, AllowNull = true, BaseColumnName = "Freight", BaseTableName="Orders" )]
		public Decimal? Freight { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, 255, 255, BaseColumnName = "ShipName", BaseTableName="Orders" )]
		public String ShipName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, 255, 255, BaseColumnName = "ShipAddress", BaseTableName="Orders" )]
		public String ShipAddress { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "ShipCity", BaseTableName="Orders" )]
		public String ShipCity { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "ShipRegion", BaseTableName="Orders" )]
		public String ShipRegion { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, 255, 255, BaseColumnName = "ShipPostalCode", BaseTableName="Orders" )]
		public String ShipPostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "ShipCountry", BaseTableName="Orders" )]
		public String ShipCountry { get; set; }


	}

	public partial class OrdersRepository : Repository<Orders> 
	{
		public OrdersRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Orders Get(string projectionName, System.Int32 orderID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Orders>)this).Get(projectionName, orderID, fetchMode);
		}

		public Orders Get(Projection projection, System.Int32 orderID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Orders>)this).Get(projection, orderID, fetchMode);
		}

		public Orders Get(string projectionName, System.Int32 orderID, params string[] fields)
		{
			return ((IRepository<Orders>)this).Get(projectionName, orderID, fields);
		}

		public Orders Get(Projection projection, System.Int32 orderID, params string[] fields)
		{
			return ((IRepository<Orders>)this).Get(projection, orderID, fields);
		}

		public void Delete(System.Int32 orderID)
		{
			var entity = new Orders { OrderID = orderID };
			this.Delete(entity);
		}
	}

	public static partial class OrdersFields
	{
		public const string OrderID = "OrderID";
		public const string CustomerID = "CustomerID";
		public const string EmployeeID = "EmployeeID";
		public const string OrderDate = "OrderDate";
		public const string RequiredDate = "RequiredDate";
		public const string ShippedDate = "ShippedDate";
		public const string ShipVia = "ShipVia";
		public const string Freight = "Freight";
		public const string ShipName = "ShipName";
		public const string ShipAddress = "ShipAddress";
		public const string ShipCity = "ShipCity";
		public const string ShipRegion = "ShipRegion";
		public const string ShipPostalCode = "ShipPostalCode";
		public const string ShipCountry = "ShipCountry";
	}

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
		[SqlField(DbType.Decimal, 17, 19, 4, BaseColumnName = "UnitPrice", BaseTableName="Products" )]
		public Decimal UnitPrice { get; set; }

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
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "CategoryName", BaseTableName="Categories" )]
		public String CategoryName { get; set; }


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

		public void RaiseProductPrices(Decimal? rate)
		{
			using (var proc = Samples.Entities.StoredProcedures.CreateRaiseProductPricesProcedure(this.DataService.Connection, this.DataService.ParameterPrefix))
			{
				this.DataService.OpenConnection();
				if (this.DataService.IsActiveTransaction) proc.Transaction = this.DataService.Transaction;
				proc.Parameters[this.DataService.ParameterPrefix + "rate"].Value = rate == null ? (object) DBNull.Value : rate.Value;
				proc.ExecuteNonQuery();
			}
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
		public const string CategoryName = "CategoryName";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Shippers")]
	public partial class Shipper
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, IsKey=true, IsAutoincrement=true, IsReadOnly = true, BaseColumnName = "ShipperID", BaseTableName="Shippers" )]
		public Int32 ShipperID { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, 255, 255, BaseColumnName = "CompanyName", BaseTableName="Shippers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, 255, 255, BaseColumnName = "Phone", BaseTableName="Shippers" )]
		public String Phone { get; set; }


	}

	public partial class ShipperRepository : Repository<Shipper> 
	{
		public ShipperRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Shipper Get(string projectionName, System.Int32 shipperID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperID, fetchMode);
		}

		public Shipper Get(Projection projection, System.Int32 shipperID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).Get(projection, shipperID, fetchMode);
		}

		public Shipper Get(string projectionName, System.Int32 shipperID, params string[] fields)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperID, fields);
		}

		public Shipper Get(Projection projection, System.Int32 shipperID, params string[] fields)
		{
			return ((IRepository<Shipper>)this).Get(projection, shipperID, fields);
		}

		public void Delete(System.Int32 shipperID)
		{
			var entity = new Shipper { ShipperID = shipperID };
			this.Delete(entity);
		}
	}

	public static partial class ShipperFields
	{
		public const string ShipperID = "ShipperID";
		public const string CompanyName = "CompanyName";
		public const string Phone = "Phone";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Suppliers")]
	public partial class Supplier
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, 10, 255, IsKey=true, IsAutoincrement=true, IsReadOnly = true, BaseColumnName = "SupplierID", BaseTableName="Suppliers" )]
		public Int32 SupplierID { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, 255, 255, BaseColumnName = "CompanyName", BaseTableName="Suppliers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, 255, 255, BaseColumnName = "ContactName", BaseTableName="Suppliers" )]
		public String ContactName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, 255, 255, BaseColumnName = "ContactTitle", BaseTableName="Suppliers" )]
		public String ContactTitle { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, 255, 255, BaseColumnName = "Address", BaseTableName="Suppliers" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "City", BaseTableName="Suppliers" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "Region", BaseTableName="Suppliers" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, 255, 255, BaseColumnName = "PostalCode", BaseTableName="Suppliers" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, 255, 255, BaseColumnName = "Country", BaseTableName="Suppliers" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, 255, 255, BaseColumnName = "Phone", BaseTableName="Suppliers" )]
		public String Phone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, 255, 255, BaseColumnName = "Fax", BaseTableName="Suppliers" )]
		public String Fax { get; set; }

		[DataMember]
		[SqlField(DbType.String, 1073741823, 255, 255, BaseColumnName = "HomePage", BaseTableName="Suppliers" )]
		public String HomePage { get; set; }


	}

	public partial class SupplierRepository : Repository<Supplier> 
	{
		public SupplierRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Supplier Get(string projectionName, System.Int32 supplierID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierID, fetchMode);
		}

		public Supplier Get(Projection projection, System.Int32 supplierID, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).Get(projection, supplierID, fetchMode);
		}

		public Supplier Get(string projectionName, System.Int32 supplierID, params string[] fields)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierID, fields);
		}

		public Supplier Get(Projection projection, System.Int32 supplierID, params string[] fields)
		{
			return ((IRepository<Supplier>)this).Get(projection, supplierID, fields);
		}

		public void Delete(System.Int32 supplierID)
		{
			var entity = new Supplier { SupplierID = supplierID };
			this.Delete(entity);
		}
	}

	public static partial class SupplierFields
	{
		public const string SupplierID = "SupplierID";
		public const string CompanyName = "CompanyName";
		public const string ContactName = "ContactName";
		public const string ContactTitle = "ContactTitle";
		public const string Address = "Address";
		public const string City = "City";
		public const string Region = "Region";
		public const string PostalCode = "PostalCode";
		public const string Country = "Country";
		public const string Phone = "Phone";
		public const string Fax = "Fax";
		public const string HomePage = "HomePage";
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

		private Samples.Entities.CategoryRepository _CategoryRepository;
		public Samples.Entities.CategoryRepository CategoryRepository
		{
			get 
			{
				if ( _CategoryRepository == null)
				{
					_CategoryRepository = new Samples.Entities.CategoryRepository(this);
				}
				return _CategoryRepository;
			}
		}

		private Samples.Entities.CustomerRepository _CustomerRepository;
		public Samples.Entities.CustomerRepository CustomerRepository
		{
			get 
			{
				if ( _CustomerRepository == null)
				{
					_CustomerRepository = new Samples.Entities.CustomerRepository(this);
				}
				return _CustomerRepository;
			}
		}

		private Samples.Entities.EmployeeRepository _EmployeeRepository;
		public Samples.Entities.EmployeeRepository EmployeeRepository
		{
			get 
			{
				if ( _EmployeeRepository == null)
				{
					_EmployeeRepository = new Samples.Entities.EmployeeRepository(this);
				}
				return _EmployeeRepository;
			}
		}

		private Samples.Entities.OrderDetailRepository _OrderDetailRepository;
		public Samples.Entities.OrderDetailRepository OrderDetailRepository
		{
			get 
			{
				if ( _OrderDetailRepository == null)
				{
					_OrderDetailRepository = new Samples.Entities.OrderDetailRepository(this);
				}
				return _OrderDetailRepository;
			}
		}

		private Samples.Entities.OrdersRepository _OrdersRepository;
		public Samples.Entities.OrdersRepository OrdersRepository
		{
			get 
			{
				if ( _OrdersRepository == null)
				{
					_OrdersRepository = new Samples.Entities.OrdersRepository(this);
				}
				return _OrdersRepository;
			}
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

		private Samples.Entities.ShipperRepository _ShipperRepository;
		public Samples.Entities.ShipperRepository ShipperRepository
		{
			get 
			{
				if ( _ShipperRepository == null)
				{
					_ShipperRepository = new Samples.Entities.ShipperRepository(this);
				}
				return _ShipperRepository;
			}
		}

		private Samples.Entities.SupplierRepository _SupplierRepository;
		public Samples.Entities.SupplierRepository SupplierRepository
		{
			get 
			{
				if ( _SupplierRepository == null)
				{
					_SupplierRepository = new Samples.Entities.SupplierRepository(this);
				}
				return _SupplierRepository;
			}
		}
	}
}
namespace Samples.Entities
{
	public static partial class StoredProcedures
	{
		public static DbCommand CreateRaiseProductPricesProcedure(DbConnection connection, string parameterPrefix)
		{
			var cmd = connection.CreateCommand();
			cmd.CommandText = "RaiseProductPrices";
			cmd.CommandType = CommandType.StoredProcedure;
			IDbDataParameter p = null;

			p = cmd.CreateParameter();
			p.ParameterName = parameterPrefix + "RETURN_VALUE";
			p.DbType = DbType.Int32;
			p.Direction = ParameterDirection.ReturnValue;
			p.SourceColumn = "RETURN_VALUE";
			cmd.Parameters.Add(p);

			p = cmd.CreateParameter();
			p.ParameterName = parameterPrefix + "rate";
			p.DbType = DbType.Decimal;
			p.Direction = ParameterDirection.Input;
			p.Precision = 5;
			p.Scale = 4;
			p.SourceColumn = "rate";
			cmd.Parameters.Add(p);

			return cmd;
		}

	}
}
