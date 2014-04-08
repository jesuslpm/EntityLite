
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
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="CategoryID", BaseColumnName ="CategoryID", BaseTableName = "Categories" )]
		public Int32 CategoryId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryName", BaseColumnName ="CategoryName", BaseTableName = "Categories" )]
		public String CategoryName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 2147483647, ColumnName ="Description", BaseColumnName ="Description", BaseTableName = "Categories" )]
		public String Description { get; set; }

		[DataMember]
		[SqlField(DbType.Binary, 2147483647, ColumnName ="Picture", BaseColumnName ="Picture", BaseTableName = "Categories" )]
		public Byte[] Picture { get; set; }


	}

	public partial class CategoryRepository : Repository<Category> 
	{
		public CategoryRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Category Get(string projectionName, System.Int32 categoryId)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryId, FetchMode.UseIdentityMap);
		}

		public Category Get(string projectionName, System.Int32 categoryId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryId, fetchMode);
		}

		public Category Get(Projection projection, System.Int32 categoryId)
		{
			return ((IRepository<Category>)this).Get(projection, categoryId, FetchMode.UseIdentityMap);
		}

		public Category Get(Projection projection, System.Int32 categoryId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).Get(projection, categoryId, fetchMode);
		}

		public Category Get(string projectionName, System.Int32 categoryId, params string[] fields)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryId, fields);
		}

		public Category Get(Projection projection, System.Int32 categoryId, params string[] fields)
		{
			return ((IRepository<Category>)this).Get(projection, categoryId, fields);
		}

		public void Delete(System.Int32 categoryId)
		{
			var entity = new Category { CategoryId = categoryId };
			this.Delete(entity);
		}
	}

	public static partial class CategoryFields
	{
		public const string CategoryId = "CategoryId";
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
		[SqlField(DbType.StringFixedLength, 5, IsKey=true, ColumnName ="CustomerID", BaseColumnName ="CustomerID", BaseTableName = "Customers" )]
		public String CustomerId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Customers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ContactName", BaseColumnName ="ContactName", BaseTableName = "Customers" )]
		public String ContactName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ContactTitle", BaseColumnName ="ContactTitle", BaseTableName = "Customers" )]
		public String ContactTitle { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Customers" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Customers" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Customers" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Customers" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Customers" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Customers" )]
		public String Phone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Fax", BaseColumnName ="Fax", BaseTableName = "Customers" )]
		public String Fax { get; set; }


	}

	public partial class CustomerRepository : Repository<Customer> 
	{
		public CustomerRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Customer Get(string projectionName, System.String customerId)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerId, FetchMode.UseIdentityMap);
		}

		public Customer Get(string projectionName, System.String customerId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerId, fetchMode);
		}

		public Customer Get(Projection projection, System.String customerId)
		{
			return ((IRepository<Customer>)this).Get(projection, customerId, FetchMode.UseIdentityMap);
		}

		public Customer Get(Projection projection, System.String customerId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projection, customerId, fetchMode);
		}

		public Customer Get(string projectionName, System.String customerId, params string[] fields)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerId, fields);
		}

		public Customer Get(Projection projection, System.String customerId, params string[] fields)
		{
			return ((IRepository<Customer>)this).Get(projection, customerId, fields);
		}

		public void Delete(System.String customerId)
		{
			var entity = new Customer { CustomerId = customerId };
			this.Delete(entity);
		}
	}

	public static partial class CustomerFields
	{
		public const string CustomerId = "CustomerId";
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
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="EmployeeID", BaseColumnName ="EmployeeID", BaseTableName = "Employees" )]
		public Int32 EmployeeId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="LastName", BaseColumnName ="LastName", BaseTableName = "Employees" )]
		public String LastName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="FirstName", BaseColumnName ="FirstName", BaseTableName = "Employees" )]
		public String FirstName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="Title", BaseColumnName ="Title", BaseTableName = "Employees" )]
		public String Title { get; set; }

		[DataMember]
		[SqlField(DbType.String, 25, ColumnName ="TitleOfCourtesy", BaseColumnName ="TitleOfCourtesy", BaseTableName = "Employees" )]
		public String TitleOfCourtesy { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="BirthDate", BaseColumnName ="BirthDate", BaseTableName = "Employees" )]
		public DateTime? BirthDate { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="HireDate", BaseColumnName ="HireDate", BaseTableName = "Employees" )]
		public DateTime? HireDate { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Employees" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Employees" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Employees" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Employees" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Employees" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="HomePhone", BaseColumnName ="HomePhone", BaseTableName = "Employees" )]
		public String HomePhone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 4, ColumnName ="Extension", BaseColumnName ="Extension", BaseTableName = "Employees" )]
		public String Extension { get; set; }

		[DataMember]
		[SqlField(DbType.Binary, 2147483647, ColumnName ="Photo", BaseColumnName ="Photo", BaseTableName = "Employees" )]
		public Byte[] Photo { get; set; }

		[DataMember]
		[SqlField(DbType.String, 1073741823, ColumnName ="Notes", BaseColumnName ="Notes", BaseTableName = "Employees" )]
		public String Notes { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="ReportsTo", BaseColumnName ="ReportsTo", BaseTableName = "Employees" )]
		public Int32? ReportsTo { get; set; }

		[DataMember]
		[SqlField(DbType.String, 255, ColumnName ="PhotoPath", BaseColumnName ="PhotoPath", BaseTableName = "Employees" )]
		public String PhotoPath { get; set; }


	}

	public partial class EmployeeRepository : Repository<Employee> 
	{
		public EmployeeRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Employee Get(string projectionName, System.Int32 employeeId)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeId, FetchMode.UseIdentityMap);
		}

		public Employee Get(string projectionName, System.Int32 employeeId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeId, fetchMode);
		}

		public Employee Get(Projection projection, System.Int32 employeeId)
		{
			return ((IRepository<Employee>)this).Get(projection, employeeId, FetchMode.UseIdentityMap);
		}

		public Employee Get(Projection projection, System.Int32 employeeId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projection, employeeId, fetchMode);
		}

		public Employee Get(string projectionName, System.Int32 employeeId, params string[] fields)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeId, fields);
		}

		public Employee Get(Projection projection, System.Int32 employeeId, params string[] fields)
		{
			return ((IRepository<Employee>)this).Get(projection, employeeId, fields);
		}

		public void Delete(System.Int32 employeeId)
		{
			var entity = new Employee { EmployeeId = employeeId };
			this.Delete(entity);
		}
	}

	public static partial class EmployeeFields
	{
		public const string EmployeeId = "EmployeeId";
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
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="OrderDetailID", BaseColumnName ="OrderDetailID", BaseTableName = "OrderDetails" )]
		public Int32 OrderDetailId { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, ColumnName ="OrderID", BaseColumnName ="OrderID", BaseTableName = "OrderDetails" )]
		public Int32 OrderId { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, ColumnName ="ProductID", BaseColumnName ="ProductID", BaseTableName = "OrderDetails" )]
		public Int32 ProductId { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 19, Scale=4, ColumnName ="UnitPrice", BaseColumnName ="UnitPrice", BaseTableName = "OrderDetails" )]
		public Decimal UnitPrice { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, ColumnName ="Quantity", BaseColumnName ="Quantity", BaseTableName = "OrderDetails" )]
		public Int16 Quantity { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 5, Scale=4, ColumnName ="Discount", BaseColumnName ="Discount", BaseTableName = "OrderDetails" )]
		public Decimal Discount { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 32, Scale=8, AllowNull = true, IsReadOnly = true, ColumnName ="SubTotal" )]
		public Decimal? SubTotal { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ProductName" )]
		public String ProductName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryName" )]
		public String CategoryName { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="OrderDate" )]
		public DateTime? OrderDate { get; set; }

		[DataMember]
		[SqlField(DbType.StringFixedLength, 5, ColumnName ="CustomerID" )]
		public String CustomerId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CustomerName" )]
		public String CustomerName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="Address" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="City" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Region" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="PostalCode" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Country" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Phone" )]
		public String Phone { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsInStock" )]
		public Int16? UnitsInStock { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsOnOrder" )]
		public Int16? UnitsOnOrder { get; set; }


	}

	public partial class OrderDetailRepository : Repository<OrderDetail> 
	{
		public OrderDetailRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public OrderDetail Get(string projectionName, System.Int32 orderDetailId)
		{
			return ((IRepository<OrderDetail>)this).Get(projectionName, orderDetailId, FetchMode.UseIdentityMap);
		}

		public OrderDetail Get(string projectionName, System.Int32 orderDetailId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<OrderDetail>)this).Get(projectionName, orderDetailId, fetchMode);
		}

		public OrderDetail Get(Projection projection, System.Int32 orderDetailId)
		{
			return ((IRepository<OrderDetail>)this).Get(projection, orderDetailId, FetchMode.UseIdentityMap);
		}

		public OrderDetail Get(Projection projection, System.Int32 orderDetailId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<OrderDetail>)this).Get(projection, orderDetailId, fetchMode);
		}

		public OrderDetail Get(string projectionName, System.Int32 orderDetailId, params string[] fields)
		{
			return ((IRepository<OrderDetail>)this).Get(projectionName, orderDetailId, fields);
		}

		public OrderDetail Get(Projection projection, System.Int32 orderDetailId, params string[] fields)
		{
			return ((IRepository<OrderDetail>)this).Get(projection, orderDetailId, fields);
		}

		public void Delete(System.Int32 orderDetailId)
		{
			var entity = new OrderDetail { OrderDetailId = orderDetailId };
			this.Delete(entity);
		}
	}

	public static partial class OrderDetailFields
	{
		public const string OrderDetailId = "OrderDetailId";
		public const string OrderId = "OrderId";
		public const string ProductId = "ProductId";
		public const string UnitPrice = "UnitPrice";
		public const string Quantity = "Quantity";
		public const string Discount = "Discount";
		public const string SubTotal = "SubTotal";
		public const string ProductName = "ProductName";
		public const string CategoryName = "CategoryName";
		public const string OrderDate = "OrderDate";
		public const string CustomerId = "CustomerId";
		public const string CustomerName = "CustomerName";
		public const string Address = "Address";
		public const string City = "City";
		public const string Region = "Region";
		public const string PostalCode = "PostalCode";
		public const string Country = "Country";
		public const string Phone = "Phone";
		public const string UnitsInStock = "UnitsInStock";
		public const string UnitsOnOrder = "UnitsOnOrder";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Orders")]
	public partial class Order
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="OrderID", BaseColumnName ="OrderID", BaseTableName = "Orders" )]
		public Int32 OrderId { get; set; }

		[DataMember]
		[SqlField(DbType.StringFixedLength, 5, ColumnName ="CustomerID", BaseColumnName ="CustomerID", BaseTableName = "Orders" )]
		public String CustomerId { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="EmployeeID", BaseColumnName ="EmployeeID", BaseTableName = "Orders" )]
		public Int32? EmployeeId { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="OrderDate", BaseColumnName ="OrderDate", BaseTableName = "Orders" )]
		public DateTime? OrderDate { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="RequiredDate", BaseColumnName ="RequiredDate", BaseTableName = "Orders" )]
		public DateTime? RequiredDate { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="ShippedDate", BaseColumnName ="ShippedDate", BaseTableName = "Orders" )]
		public DateTime? ShippedDate { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="ShipVia", BaseColumnName ="ShipVia", BaseTableName = "Orders" )]
		public Int32? ShipVia { get; set; }

		[DataMember]
		[SqlField(DbType.Currency, 8, Precision = 19, AllowNull = true, ColumnName ="Freight", BaseColumnName ="Freight", BaseTableName = "Orders" )]
		public Decimal? Freight { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ShipName", BaseColumnName ="ShipName", BaseTableName = "Orders" )]
		public String ShipName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="ShipAddress", BaseColumnName ="ShipAddress", BaseTableName = "Orders" )]
		public String ShipAddress { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="ShipCity", BaseColumnName ="ShipCity", BaseTableName = "Orders" )]
		public String ShipCity { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="ShipRegion", BaseColumnName ="ShipRegion", BaseTableName = "Orders" )]
		public String ShipRegion { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="ShipPostalCode", BaseColumnName ="ShipPostalCode", BaseTableName = "Orders" )]
		public String ShipPostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="ShipCountry", BaseColumnName ="ShipCountry", BaseTableName = "Orders" )]
		public String ShipCountry { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CustomerCompanyName" )]
		public String CustomerCompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="EmployeeFirstName" )]
		public String EmployeeFirstName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="EmployeeLastName" )]
		public String EmployeeLastName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ShipperCompanyName" )]
		public String ShipperCompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 38, Scale=8, AllowNull = true, ColumnName ="OrderTotal" )]
		public Decimal? OrderTotal { get; set; }

		[DataMember]
		[SqlField(DbType.Int64, 8, Precision = 19, AllowNull = true, ColumnName ="LineCount" )]
		public Int64? LineCount { get; set; }


	}

	public partial class OrderRepository : Repository<Order> 
	{
		public OrderRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Order Get(string projectionName, System.Int32 orderId)
		{
			return ((IRepository<Order>)this).Get(projectionName, orderId, FetchMode.UseIdentityMap);
		}

		public Order Get(string projectionName, System.Int32 orderId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Order>)this).Get(projectionName, orderId, fetchMode);
		}

		public Order Get(Projection projection, System.Int32 orderId)
		{
			return ((IRepository<Order>)this).Get(projection, orderId, FetchMode.UseIdentityMap);
		}

		public Order Get(Projection projection, System.Int32 orderId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Order>)this).Get(projection, orderId, fetchMode);
		}

		public Order Get(string projectionName, System.Int32 orderId, params string[] fields)
		{
			return ((IRepository<Order>)this).Get(projectionName, orderId, fields);
		}

		public Order Get(Projection projection, System.Int32 orderId, params string[] fields)
		{
			return ((IRepository<Order>)this).Get(projection, orderId, fields);
		}

		public void Delete(System.Int32 orderId)
		{
			var entity = new Order { OrderId = orderId };
			this.Delete(entity);
		}
	}

	public static partial class OrderFields
	{
		public const string OrderId = "OrderId";
		public const string CustomerId = "CustomerId";
		public const string EmployeeId = "EmployeeId";
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
		public const string CustomerCompanyName = "CustomerCompanyName";
		public const string EmployeeFirstName = "EmployeeFirstName";
		public const string EmployeeLastName = "EmployeeLastName";
		public const string ShipperCompanyName = "ShipperCompanyName";
		public const string OrderTotal = "OrderTotal";
		public const string LineCount = "LineCount";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Products")]
	public partial class Product
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="ProductID", BaseColumnName ="ProductID", BaseTableName = "Products" )]
		public Int32 ProductId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ProductName", BaseColumnName ="ProductName", BaseTableName = "Products" )]
		public String ProductName { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="SupplierID", BaseColumnName ="SupplierID", BaseTableName = "Products" )]
		public Int32? SupplierId { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="CategoryID", BaseColumnName ="CategoryID", BaseTableName = "Products" )]
		public Int32? CategoryId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="QuantityPerUnit", BaseColumnName ="QuantityPerUnit", BaseTableName = "Products" )]
		public String QuantityPerUnit { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 19, Scale=4, ColumnName ="UnitPrice", BaseColumnName ="UnitPrice", BaseTableName = "Products" )]
		public Decimal UnitPrice { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsInStock", BaseColumnName ="UnitsInStock", BaseTableName = "Products" )]
		public Int16? UnitsInStock { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsOnOrder", BaseColumnName ="UnitsOnOrder", BaseTableName = "Products" )]
		public Int16? UnitsOnOrder { get; set; }

		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="ReorderLevel", BaseColumnName ="ReorderLevel", BaseTableName = "Products" )]
		public Int16? ReorderLevel { get; set; }

		[DataMember]
		[SqlField(DbType.Boolean, 1, ColumnName ="Discontinued", BaseColumnName ="Discontinued", BaseTableName = "Products" )]
		public Boolean Discontinued { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryName" )]
		public String CategoryName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="SupplierName" )]
		public String SupplierName { get; set; }


	}

	public partial class ProductRepository : Repository<Product> 
	{
		public ProductRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Product Get(string projectionName, System.Int32 productId)
		{
			return ((IRepository<Product>)this).Get(projectionName, productId, FetchMode.UseIdentityMap);
		}

		public Product Get(string projectionName, System.Int32 productId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).Get(projectionName, productId, fetchMode);
		}

		public Product Get(Projection projection, System.Int32 productId)
		{
			return ((IRepository<Product>)this).Get(projection, productId, FetchMode.UseIdentityMap);
		}

		public Product Get(Projection projection, System.Int32 productId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).Get(projection, productId, fetchMode);
		}

		public Product Get(string projectionName, System.Int32 productId, params string[] fields)
		{
			return ((IRepository<Product>)this).Get(projectionName, productId, fields);
		}

		public Product Get(Projection projection, System.Int32 productId, params string[] fields)
		{
			return ((IRepository<Product>)this).Get(projection, productId, fields);
		}

		public void Delete(System.Int32 productId)
		{
			var entity = new Product { ProductId = productId };
			this.Delete(entity);
		}

		public void RaiseProductPrices(Decimal? rate)
		{
            var executor = new StoredProcedureExecutor(this.DataService, true)
            {
                GetCommandFunc = () =>
                {
                    var proc =  Samples.Entities.StoredProcedures.CreateRaiseProductPricesProcedure(this.DataService.Connection, this.DataService.EntityLiteProvider.ParameterPrefix);
					proc.Parameters[this.DataService.EntityLiteProvider.ParameterPrefix + "rate"].Value = rate == null ? (object) DBNull.Value : rate.Value;
                    return proc;
                }
            };

			executor.ExecuteNonQuery();
		}
	}

	public static partial class ProductFields
	{
		public const string ProductId = "ProductId";
		public const string ProductName = "ProductName";
		public const string SupplierId = "SupplierId";
		public const string CategoryId = "CategoryId";
		public const string QuantityPerUnit = "QuantityPerUnit";
		public const string UnitPrice = "UnitPrice";
		public const string UnitsInStock = "UnitsInStock";
		public const string UnitsOnOrder = "UnitsOnOrder";
		public const string ReorderLevel = "ReorderLevel";
		public const string Discontinued = "Discontinued";
		public const string CategoryName = "CategoryName";
		public const string SupplierName = "SupplierName";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Shippers")]
	public partial class Shipper
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="ShipperID", BaseColumnName ="ShipperID", BaseTableName = "Shippers" )]
		public Int32 ShipperId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Shippers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Shippers" )]
		public String Phone { get; set; }


	}

	public partial class ShipperRepository : Repository<Shipper> 
	{
		public ShipperRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Shipper Get(string projectionName, System.Int32 shipperId)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperId, FetchMode.UseIdentityMap);
		}

		public Shipper Get(string projectionName, System.Int32 shipperId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperId, fetchMode);
		}

		public Shipper Get(Projection projection, System.Int32 shipperId)
		{
			return ((IRepository<Shipper>)this).Get(projection, shipperId, FetchMode.UseIdentityMap);
		}

		public Shipper Get(Projection projection, System.Int32 shipperId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).Get(projection, shipperId, fetchMode);
		}

		public Shipper Get(string projectionName, System.Int32 shipperId, params string[] fields)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperId, fields);
		}

		public Shipper Get(Projection projection, System.Int32 shipperId, params string[] fields)
		{
			return ((IRepository<Shipper>)this).Get(projection, shipperId, fields);
		}

		public void Delete(System.Int32 shipperId)
		{
			var entity = new Shipper { ShipperId = shipperId };
			this.Delete(entity);
		}
	}

	public static partial class ShipperFields
	{
		public const string ShipperId = "ShipperId";
		public const string CompanyName = "CompanyName";
		public const string Phone = "Phone";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Suppliers")]
	public partial class Supplier
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="SupplierID", BaseColumnName ="SupplierID", BaseTableName = "Suppliers" )]
		public Int32 SupplierId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Suppliers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ContactName", BaseColumnName ="ContactName", BaseTableName = "Suppliers" )]
		public String ContactName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ContactTitle", BaseColumnName ="ContactTitle", BaseTableName = "Suppliers" )]
		public String ContactTitle { get; set; }

		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Suppliers" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Suppliers" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Suppliers" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Suppliers" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Suppliers" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Suppliers" )]
		public String Phone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Fax", BaseColumnName ="Fax", BaseTableName = "Suppliers" )]
		public String Fax { get; set; }

		[DataMember]
		[SqlField(DbType.String, 1073741823, ColumnName ="HomePage", BaseColumnName ="HomePage", BaseTableName = "Suppliers" )]
		public String HomePage { get; set; }


	}

	public partial class SupplierRepository : Repository<Supplier> 
	{
		public SupplierRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Supplier Get(string projectionName, System.Int32 supplierId)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierId, FetchMode.UseIdentityMap);
		}

		public Supplier Get(string projectionName, System.Int32 supplierId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierId, fetchMode);
		}

		public Supplier Get(Projection projection, System.Int32 supplierId)
		{
			return ((IRepository<Supplier>)this).Get(projection, supplierId, FetchMode.UseIdentityMap);
		}

		public Supplier Get(Projection projection, System.Int32 supplierId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).Get(projection, supplierId, fetchMode);
		}

		public Supplier Get(string projectionName, System.Int32 supplierId, params string[] fields)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierId, fields);
		}

		public Supplier Get(Projection projection, System.Int32 supplierId, params string[] fields)
		{
			return ((IRepository<Supplier>)this).Get(projection, supplierId, fields);
		}

		public void Delete(System.Int32 supplierId)
		{
			var entity = new Supplier { SupplierId = supplierId };
			this.Delete(entity);
		}
	}

	public static partial class SupplierFields
	{
		public const string SupplierId = "SupplierId";
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

	[Serializable]
	[DataContract]
	[SqlEntity()]
	public partial class ProductSale
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="CategoryID" )]
		public Int32? CategoryId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryName" )]
		public String CategoryName { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, ColumnName ="ProductID" )]
		public Int32 ProductId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ProductName" )]
		public String ProductName { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="Year" )]
		public Int32? Year { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="Quarter" )]
		public Int32? Quarter { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 38, Scale=8, AllowNull = true, ColumnName ="Sales" )]
		public Decimal? Sales { get; set; }


	}

	public partial class ProductSaleRepository : Repository<ProductSale> 
	{
		public ProductSaleRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

	}

	public static partial class ProductSaleFields
	{
		public const string CategoryId = "CategoryId";
		public const string CategoryName = "CategoryName";
		public const string ProductId = "ProductId";
		public const string ProductName = "ProductName";
		public const string Year = "Year";
		public const string Quarter = "Quarter";
		public const string Sales = "Sales";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Items")]
	public partial class Item
	{
		[DataMember]
		[SqlField(DbType.Guid, 16, IsKey=true, ColumnName ="ItemGuid", BaseColumnName ="ItemGuid", BaseTableName = "Items" )]
		public Guid ItemGuid { get; set; }

		[DataMember]
		[SqlField(DbType.AnsiString, 50, ColumnName ="Value", BaseColumnName ="Value", BaseTableName = "Items" )]
		public String Value { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="CreatedDate", BaseColumnName ="CreatedDate", BaseTableName = "Items" )]
		public DateTime? CreatedDate { get; set; }


	}

	public partial class ItemRepository : Repository<Item> 
	{
		public ItemRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Item Get(string projectionName, System.Guid itemGuid)
		{
			return ((IRepository<Item>)this).Get(projectionName, itemGuid, FetchMode.UseIdentityMap);
		}

		public Item Get(string projectionName, System.Guid itemGuid, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Item>)this).Get(projectionName, itemGuid, fetchMode);
		}

		public Item Get(Projection projection, System.Guid itemGuid)
		{
			return ((IRepository<Item>)this).Get(projection, itemGuid, FetchMode.UseIdentityMap);
		}

		public Item Get(Projection projection, System.Guid itemGuid, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Item>)this).Get(projection, itemGuid, fetchMode);
		}

		public Item Get(string projectionName, System.Guid itemGuid, params string[] fields)
		{
			return ((IRepository<Item>)this).Get(projectionName, itemGuid, fields);
		}

		public Item Get(Projection projection, System.Guid itemGuid, params string[] fields)
		{
			return ((IRepository<Item>)this).Get(projection, itemGuid, fields);
		}

		public void Delete(System.Guid itemGuid)
		{
			var entity = new Item { ItemGuid = itemGuid };
			this.Delete(entity);
		}
	}

	public static partial class ItemFields
	{
		public const string ItemGuid = "ItemGuid";
		public const string Value = "Value";
		public const string CreatedDate = "CreatedDate";
	}

}

namespace Samples.Entities
{
	public partial class NorthwindDataService : DataService
	{
		partial void OnCreated();

		private void Init()
		{
			EntityNameToEntityViewTransform = TextTransform.None;
			EntityLiteProvider.DefaultSchema = "dbo";
			OnCreated();
		}

        public NorthwindDataService() : base("Northwind")
        {
			Init();
        }

        public NorthwindDataService(string connectionStringName) : base(connectionStringName)
        {
			Init();
        }

        public NorthwindDataService(string connectionString, string providerName) : base(connectionString, providerName)
        {
			Init();
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

		private Samples.Entities.OrderRepository _OrderRepository;
		public Samples.Entities.OrderRepository OrderRepository
		{
			get 
			{
				if ( _OrderRepository == null)
				{
					_OrderRepository = new Samples.Entities.OrderRepository(this);
				}
				return _OrderRepository;
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

		private Samples.Entities.ProductSaleRepository _ProductSaleRepository;
		public Samples.Entities.ProductSaleRepository ProductSaleRepository
		{
			get 
			{
				if ( _ProductSaleRepository == null)
				{
					_ProductSaleRepository = new Samples.Entities.ProductSaleRepository(this);
				}
				return _ProductSaleRepository;
			}
		}

		private Samples.Entities.ItemRepository _ItemRepository;
		public Samples.Entities.ItemRepository ItemRepository
		{
			get 
			{
				if ( _ItemRepository == null)
				{
					_ItemRepository = new Samples.Entities.ItemRepository(this);
				}
				return _ItemRepository;
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
