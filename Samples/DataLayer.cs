

using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
		[SqlField(DbType.Int32, 4, IsKey=true, IsAutoincrement=true, ColumnName ="category_id", BaseColumnName ="category_id", BaseTableName = "categories" )]
		public Int32 CategoryId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="category_name", BaseColumnName ="category_name", BaseTableName = "categories" )]
		public String CategoryName { get; set; }

		[DataMember]
		[SqlField(DbType.String, -1, ColumnName ="description", BaseColumnName ="description", BaseTableName = "categories" )]
		public String Description { get; set; }

		[DataMember]
		[SqlField(DbType.Binary, -1, ColumnName ="picture", BaseColumnName ="picture", BaseTableName = "categories" )]
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

		public Category Get(string projectionName, System.Int32 categoryId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryId, fetchMode);
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
		[SqlField(DbType.String, 32, IsKey=true, ColumnName ="customer_id", BaseColumnName ="customer_id", BaseTableName = "customers" )]
		public String CustomerId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="company_name", BaseColumnName ="company_name", BaseTableName = "customers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="contact_name", BaseColumnName ="contact_name", BaseTableName = "customers" )]
		public String ContactName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="contact_title", BaseColumnName ="contact_title", BaseTableName = "customers" )]
		public String ContactTitle { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="address", BaseColumnName ="address", BaseTableName = "customers" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="city", BaseColumnName ="city", BaseTableName = "customers" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="region", BaseColumnName ="region", BaseTableName = "customers" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="postal_code", BaseColumnName ="postal_code", BaseTableName = "customers" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="country", BaseColumnName ="country", BaseTableName = "customers" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="phone", BaseColumnName ="phone", BaseTableName = "customers" )]
		public String Phone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="fax", BaseColumnName ="fax", BaseTableName = "customers" )]
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

		public Customer Get(string projectionName, System.String customerId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerId, fetchMode);
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
		[SqlField(DbType.Int32, 4, IsKey=true, IsAutoincrement=true, ColumnName ="employee_id", BaseColumnName ="employee_id", BaseTableName = "employees" )]
		public Int32 EmployeeId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="last_name", BaseColumnName ="last_name", BaseTableName = "employees" )]
		public String LastName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="first_name", BaseColumnName ="first_name", BaseTableName = "employees" )]
		public String FirstName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="title", BaseColumnName ="title", BaseTableName = "employees" )]
		public String Title { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="title_of_courtesy", BaseColumnName ="title_of_courtesy", BaseTableName = "employees" )]
		public String TitleOfCourtesy { get; set; }

		[DataMember]
		[SqlField(DbType.Date, 4, AllowNull = true, ColumnName ="birth_date", BaseColumnName ="birth_date", BaseTableName = "employees" )]
		public DateTime? BirthDate { get; set; }

		[DataMember]
		[SqlField(DbType.Date, 4, AllowNull = true, ColumnName ="hire_date", BaseColumnName ="hire_date", BaseTableName = "employees" )]
		public DateTime? HireDate { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="address", BaseColumnName ="address", BaseTableName = "employees" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="city", BaseColumnName ="city", BaseTableName = "employees" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="region", BaseColumnName ="region", BaseTableName = "employees" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="postal_code", BaseColumnName ="postal_code", BaseTableName = "employees" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="country", BaseColumnName ="country", BaseTableName = "employees" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="home_phone", BaseColumnName ="home_phone", BaseTableName = "employees" )]
		public String HomePhone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 16, ColumnName ="extension", BaseColumnName ="extension", BaseTableName = "employees" )]
		public String Extension { get; set; }

		[DataMember]
		[SqlField(DbType.String, -1, ColumnName ="notes", BaseColumnName ="notes", BaseTableName = "employees" )]
		public String Notes { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="reports_to", BaseColumnName ="reports_to", BaseTableName = "employees" )]
		public Int32? ReportsTo { get; set; }

		[DataMember]
		[SqlField(DbType.String, 512, ColumnName ="photo_path", BaseColumnName ="photo_path", BaseTableName = "employees" )]
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

		public Employee Get(string projectionName, System.Int32 employeeId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeId, fetchMode);
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
		public const string Notes = "Notes";
		public const string ReportsTo = "ReportsTo";
		public const string PhotoPath = "PhotoPath";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Order_Details")]
	public partial class OrderDetail
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, IsKey=true, IsAutoincrement=true, ColumnName ="order_detail_id", BaseColumnName ="order_detail_id", BaseTableName = "order_details" )]
		public Int32 OrderDetailId { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, ColumnName ="order_id", BaseColumnName ="order_id", BaseTableName = "order_details" )]
		public Int32 OrderId { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, ColumnName ="product_id", BaseColumnName ="product_id", BaseTableName = "order_details" )]
		public Int32 ProductId { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, -1, Precision = 19, Scale=4, ColumnName ="unit_price", BaseColumnName ="unit_price", BaseTableName = "order_details" )]
		public Decimal UnitPrice { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, ColumnName ="quantity", BaseColumnName ="quantity", BaseTableName = "order_details" )]
		public Int32 Quantity { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, -1, Precision = 5, Scale=4, ColumnName ="discount", BaseColumnName ="discount", BaseTableName = "order_details" )]
		public Decimal Discount { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, -1, AllowNull = true, ColumnName ="subtotal" )]
		public Decimal? Subtotal { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="product_name" )]
		public String ProductName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="category_name" )]
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

		public OrderDetail Get(string projectionName, System.Int32 orderDetailId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<OrderDetail>)this).Get(projectionName, orderDetailId, fetchMode);
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
		public const string Subtotal = "Subtotal";
		public const string ProductName = "ProductName";
		public const string CategoryName = "CategoryName";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Orders")]
	public partial class Order
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, IsKey=true, IsAutoincrement=true, ColumnName ="order_id", BaseColumnName ="order_id", BaseTableName = "orders" )]
		public Int32 OrderId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 32, ColumnName ="customer_id", BaseColumnName ="customer_id", BaseTableName = "orders" )]
		public String CustomerId { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="employee_id", BaseColumnName ="employee_id", BaseTableName = "orders" )]
		public Int32? EmployeeId { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, AllowNull = true, ColumnName ="order_date", BaseColumnName ="order_date", BaseTableName = "orders" )]
		public DateTime? OrderDate { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, AllowNull = true, ColumnName ="required_date", BaseColumnName ="required_date", BaseTableName = "orders" )]
		public DateTime? RequiredDate { get; set; }

		[DataMember]
		[SqlField(DbType.DateTime, 8, AllowNull = true, ColumnName ="shipped_date", BaseColumnName ="shipped_date", BaseTableName = "orders" )]
		public DateTime? ShippedDate { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="ship_via", BaseColumnName ="ship_via", BaseTableName = "orders" )]
		public Int32? ShipVia { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, -1, Precision = 19, Scale=4, AllowNull = true, ColumnName ="freight", BaseColumnName ="freight", BaseTableName = "orders" )]
		public Decimal? Freight { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="ship_name", BaseColumnName ="ship_name", BaseTableName = "orders" )]
		public String ShipName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="ship_address", BaseColumnName ="ship_address", BaseTableName = "orders" )]
		public String ShipAddress { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="ship_city", BaseColumnName ="ship_city", BaseTableName = "orders" )]
		public String ShipCity { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="ship_region", BaseColumnName ="ship_region", BaseTableName = "orders" )]
		public String ShipRegion { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="ship_postal_code", BaseColumnName ="ship_postal_code", BaseTableName = "orders" )]
		public String ShipPostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="ship_country", BaseColumnName ="ship_country", BaseTableName = "orders" )]
		public String ShipCountry { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, -1, Precision = 19, Scale=4, AllowNull = true, ColumnName ="order_total" )]
		public Decimal? OrderTotal { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="line_count" )]
		public Int32? LineCount { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="customercompany_name" )]
		public String CustomercompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="employee_first_name" )]
		public String EmployeeFirstName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="employee_last_name" )]
		public String EmployeeLastName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="shipper_company_name" )]
		public String ShipperCompanyName { get; set; }


	}

	public partial class OrderRepository : Repository<Order> 
	{
		public OrderRepository(DataService DataService) : base(DataService)
		{
		}

		public new DataService DataService  
		{
			get { return (NorhtwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Order Get(string projectionName, System.Int32 orderId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Order>)this).Get(projectionName, orderId, fetchMode);
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
		public const string OrderTotal = "OrderTotal";
		public const string LineCount = "LineCount";
		public const string CustomercompanyName = "CustomercompanyName";
		public const string EmployeeFirstName = "EmployeeFirstName";
		public const string EmployeeLastName = "EmployeeLastName";
		public const string ShipperCompanyName = "ShipperCompanyName";
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Products")]
	public partial class Product
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, IsKey=true, IsAutoincrement=true, ColumnName ="product_id", BaseColumnName ="product_id", BaseTableName = "products" )]
		public Int32 ProductId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="product_name", BaseColumnName ="product_name", BaseTableName = "products" )]
		public String ProductName { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="supplier_id", BaseColumnName ="supplier_id", BaseTableName = "products" )]
		public Int32? SupplierId { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="category_id", BaseColumnName ="category_id", BaseTableName = "products" )]
		public Int32? CategoryId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="quantity_per_unit", BaseColumnName ="quantity_per_unit", BaseTableName = "products" )]
		public String QuantityPerUnit { get; set; }

		[DataMember]
		[SqlField(DbType.Decimal, -1, Precision = 19, Scale=4, AllowNull = true, ColumnName ="unit_price", BaseColumnName ="unit_price", BaseTableName = "products" )]
		public Decimal? UnitPrice { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="units_in_stock", BaseColumnName ="units_in_stock", BaseTableName = "products" )]
		public Int32? UnitsInStock { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="units_on_order", BaseColumnName ="units_on_order", BaseTableName = "products" )]
		public Int32? UnitsOnOrder { get; set; }

		[DataMember]
		[SqlField(DbType.Int32, 4, AllowNull = true, ColumnName ="reorder_level", BaseColumnName ="reorder_level", BaseTableName = "products" )]
		public Int32? ReorderLevel { get; set; }

		[DataMember]
		[SqlField(DbType.Boolean, 1, ColumnName ="discontinued", BaseColumnName ="discontinued", BaseTableName = "products" )]
		public Boolean Discontinued { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="category_name" )]
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

		public Product Get(string projectionName, System.Int32 productId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).Get(projectionName, productId, fetchMode);
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
	}

	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="Shippers")]
	public partial class Shipper
	{
		[DataMember]
		[SqlField(DbType.Int32, 4, IsKey=true, IsAutoincrement=true, ColumnName ="shipper_id", BaseColumnName ="shipper_id", BaseTableName = "shippers" )]
		public Int32 ShipperId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="company_name", BaseColumnName ="company_name", BaseTableName = "shippers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 32, ColumnName ="phone", BaseColumnName ="phone", BaseTableName = "shippers" )]
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

		public Shipper Get(string projectionName, System.Int32 shipperId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperId, fetchMode);
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
		[SqlField(DbType.Int32, 4, IsKey=true, IsAutoincrement=true, ColumnName ="supplier_id", BaseColumnName ="supplier_id", BaseTableName = "suppliers" )]
		public Int32 SupplierId { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="company_name", BaseColumnName ="company_name", BaseTableName = "suppliers" )]
		public String CompanyName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="contact_name", BaseColumnName ="contact_name", BaseTableName = "suppliers" )]
		public String ContactName { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="contact_title", BaseColumnName ="contact_title", BaseTableName = "suppliers" )]
		public String ContactTitle { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="address", BaseColumnName ="address", BaseTableName = "suppliers" )]
		public String Address { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="city", BaseColumnName ="city", BaseTableName = "suppliers" )]
		public String City { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="region", BaseColumnName ="region", BaseTableName = "suppliers" )]
		public String Region { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="postal_code", BaseColumnName ="postal_code", BaseTableName = "suppliers" )]
		public String PostalCode { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="country", BaseColumnName ="country", BaseTableName = "suppliers" )]
		public String Country { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="phone", BaseColumnName ="phone", BaseTableName = "suppliers" )]
		public String Phone { get; set; }

		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="fax", BaseColumnName ="fax", BaseTableName = "suppliers" )]
		public String Fax { get; set; }

		[DataMember]
		[SqlField(DbType.String, 512, ColumnName ="home_page", BaseColumnName ="home_page", BaseTableName = "suppliers" )]
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

		public Supplier Get(string projectionName, System.Int32 supplierId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierId, fetchMode);
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
	}
}
namespace Samples.Entities
{
	public static partial class StoredProcedures
	{
		public static DbCommand CreateRaiseProductPricesProcedure(DbConnection connection, string parameterPrefix)
		{
			var cmd = connection.CreateCommand();
			cmd.CommandText = "raise_product_prices";
			cmd.CommandType = CommandType.StoredProcedure;
			IDbDataParameter p = null;

			p = cmd.CreateParameter();
			p.ParameterName = parameterPrefix + "rate";
			p.DbType = DbType.Decimal;
			p.Direction = ParameterDirection.Input;
			p.SourceColumn = "rate";
			cmd.Parameters.Add(p);

			return cmd;
		}

	}
}
