
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
// using Microsoft.SqlServer.Types;

using System.ComponentModel;
using inercya.EntityLite;	
using inercya.EntityLite.Extensions;	

namespace Samples.Entities
{
	[SqlEntity(BaseTableName="Categories")]
	public partial class Category : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _categoryId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="CategoryID", BaseColumnName ="CategoryID", BaseTableName = "Categories" )]		
		public Int32 CategoryId 
		{ 
		    get { return _categoryId; } 
			set 
			{
			    _categoryId = value;
				NotifyPropertyChange("CategoryId");
			}
        }

		private String? _categoryName;
		[SqlField(DbType.String, 15, ColumnName ="CategoryName", BaseColumnName ="CategoryName", BaseTableName = "Categories" )]		
		public String? CategoryName 
		{ 
		    get { return _categoryName; } 
			set 
			{
			    _categoryName = value;
				NotifyPropertyChange("CategoryName");
			}
        }

		private String? _description;
		[SqlField(DbType.String, 1073741823, ColumnName ="Description", BaseColumnName ="Description", BaseTableName = "Categories" )]		
		public String? Description 
		{ 
		    get { return _description; } 
			set 
			{
			    _description = value;
				NotifyPropertyChange("Description");
			}
        }

		private Byte[]? _picture;
		[SqlField(DbType.Binary, 2147483647, ColumnName ="Picture", BaseColumnName ="Picture", BaseTableName = "Categories" )]		
		public Byte[]? Picture 
		{ 
		    get { return _picture; } 
			set 
			{
			    _picture = value;
				NotifyPropertyChange("Picture");
			}
        }

		public const string BaseTableProjectionColumnList = "[CategoryID], [CategoryName], [Description], [Picture]";

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

		public Category Get(string projectionName, Int32 categoryId)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryId, FetchMode.UseIdentityMap);
		}

		public Category Get(string projectionName, Int32 categoryId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryId, fetchMode);
		}

		public Category Get(Projection projection, Int32 categoryId)
		{
			return ((IRepository<Category>)this).Get(projection, categoryId, FetchMode.UseIdentityMap);
		}

		public Category Get(Projection projection, Int32 categoryId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).Get(projection, categoryId, fetchMode);
		}

		public Category Get(string projectionName, Int32 categoryId, params string[] fields)
		{
			return ((IRepository<Category>)this).Get(projectionName, categoryId, fields);
		}

		public Category Get(Projection projection, Int32 categoryId, params string[] fields)
		{
			return ((IRepository<Category>)this).Get(projection, categoryId, fields);
		}

		public bool Delete(Int32 categoryId)
		{
			var entity = new Category { CategoryId = categoryId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
	public static partial class CategoryFields
	{
		public const string CategoryId = "CategoryId";
		public const string CategoryName = "CategoryName";
		public const string Description = "Description";
		public const string Picture = "Picture";
	}

	public static partial class CategoryProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[SqlEntity(BaseTableName="Customers")]
	public partial class Customer : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private String? _customerId;
		[SqlField(DbType.StringFixedLength, 5, IsKey=true, ColumnName ="CustomerID", BaseColumnName ="CustomerID", BaseTableName = "Customers" )]		
		public String? CustomerId 
		{ 
		    get { return _customerId; } 
			set 
			{
			    _customerId = value;
				NotifyPropertyChange("CustomerId");
			}
        }

		private String? _companyName;
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Customers" )]		
		public String? CompanyName 
		{ 
		    get { return _companyName; } 
			set 
			{
			    _companyName = value;
				NotifyPropertyChange("CompanyName");
			}
        }

		private String? _contactName;
		[SqlField(DbType.String, 30, ColumnName ="ContactName", BaseColumnName ="ContactName", BaseTableName = "Customers" )]		
		public String? ContactName 
		{ 
		    get { return _contactName; } 
			set 
			{
			    _contactName = value;
				NotifyPropertyChange("ContactName");
			}
        }

		private String? _contactTitle;
		[SqlField(DbType.String, 30, ColumnName ="ContactTitle", BaseColumnName ="ContactTitle", BaseTableName = "Customers" )]		
		public String? ContactTitle 
		{ 
		    get { return _contactTitle; } 
			set 
			{
			    _contactTitle = value;
				NotifyPropertyChange("ContactTitle");
			}
        }

		private String? _address;
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Customers" )]		
		public String? Address 
		{ 
		    get { return _address; } 
			set 
			{
			    _address = value;
				NotifyPropertyChange("Address");
			}
        }

		private String? _city;
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Customers" )]		
		public String? City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
				NotifyPropertyChange("City");
			}
        }

		private String? _region;
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Customers" )]		
		public String? Region 
		{ 
		    get { return _region; } 
			set 
			{
			    _region = value;
				NotifyPropertyChange("Region");
			}
        }

		private String? _postalCode;
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Customers" )]		
		public String? PostalCode 
		{ 
		    get { return _postalCode; } 
			set 
			{
			    _postalCode = value;
				NotifyPropertyChange("PostalCode");
			}
        }

		private String? _country;
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Customers" )]		
		public String? Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
				NotifyPropertyChange("Country");
			}
        }

		private String? _phone;
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Customers" )]		
		public String? Phone 
		{ 
		    get { return _phone; } 
			set 
			{
			    _phone = value;
				NotifyPropertyChange("Phone");
			}
        }

		private String? _fax;
		[SqlField(DbType.String, 24, ColumnName ="Fax", BaseColumnName ="Fax", BaseTableName = "Customers" )]		
		public String? Fax 
		{ 
		    get { return _fax; } 
			set 
			{
			    _fax = value;
				NotifyPropertyChange("Fax");
			}
        }

		public const string BaseTableProjectionColumnList = "[CustomerID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Country], [Phone], [Fax]";

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

		public Customer Get(string projectionName, String? customerId)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerId, FetchMode.UseIdentityMap);
		}

		public Customer Get(string projectionName, String? customerId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerId, fetchMode);
		}

		public Customer Get(Projection projection, String? customerId)
		{
			return ((IRepository<Customer>)this).Get(projection, customerId, FetchMode.UseIdentityMap);
		}

		public Customer Get(Projection projection, String? customerId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projection, customerId, fetchMode);
		}

		public Customer Get(string projectionName, String? customerId, params string[] fields)
		{
			return ((IRepository<Customer>)this).Get(projectionName, customerId, fields);
		}

		public Customer Get(Projection projection, String? customerId, params string[] fields)
		{
			return ((IRepository<Customer>)this).Get(projection, customerId, fields);
		}

		public bool Delete(String? customerId)
		{
			var entity = new Customer { CustomerId = customerId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
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

	public static partial class CustomerProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[SqlEntity(BaseTableName="Employees")]
	public partial class Employee : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _employeeId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="EmployeeID", BaseColumnName ="EmployeeID", BaseTableName = "Employees" )]		
		public Int32 EmployeeId 
		{ 
		    get { return _employeeId; } 
			set 
			{
			    _employeeId = value;
				NotifyPropertyChange("EmployeeId");
			}
        }

		private String? _lastName;
		[SqlField(DbType.String, 20, ColumnName ="LastName", BaseColumnName ="LastName", BaseTableName = "Employees" )]		
		public String? LastName 
		{ 
		    get { return _lastName; } 
			set 
			{
			    _lastName = value;
				NotifyPropertyChange("LastName");
			}
        }

		private String? _firstName;
		[SqlField(DbType.String, 10, ColumnName ="FirstName", BaseColumnName ="FirstName", BaseTableName = "Employees" )]		
		public String? FirstName 
		{ 
		    get { return _firstName; } 
			set 
			{
			    _firstName = value;
				NotifyPropertyChange("FirstName");
			}
        }

		private String? _title;
		[SqlField(DbType.String, 30, ColumnName ="Title", BaseColumnName ="Title", BaseTableName = "Employees" )]		
		public String? Title 
		{ 
		    get { return _title; } 
			set 
			{
			    _title = value;
				NotifyPropertyChange("Title");
			}
        }

		private String? _titleOfCourtesy;
		[SqlField(DbType.String, 25, ColumnName ="TitleOfCourtesy", BaseColumnName ="TitleOfCourtesy", BaseTableName = "Employees" )]		
		public String? TitleOfCourtesy 
		{ 
		    get { return _titleOfCourtesy; } 
			set 
			{
			    _titleOfCourtesy = value;
				NotifyPropertyChange("TitleOfCourtesy");
			}
        }

		private DateTime? _birthDate;
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="BirthDate", BaseColumnName ="BirthDate", BaseTableName = "Employees" )]		
		public DateTime? BirthDate 
		{ 
		    get { return _birthDate; } 
			set 
			{
			    _birthDate = value;
				NotifyPropertyChange("BirthDate");
			}
        }

		private DateTime? _hireDate;
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="HireDate", BaseColumnName ="HireDate", BaseTableName = "Employees" )]		
		public DateTime? HireDate 
		{ 
		    get { return _hireDate; } 
			set 
			{
			    _hireDate = value;
				NotifyPropertyChange("HireDate");
			}
        }

		private String? _address;
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Employees" )]		
		public String? Address 
		{ 
		    get { return _address; } 
			set 
			{
			    _address = value;
				NotifyPropertyChange("Address");
			}
        }

		private String? _city;
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Employees" )]		
		public String? City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
				NotifyPropertyChange("City");
			}
        }

		private String? _region;
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Employees" )]		
		public String? Region 
		{ 
		    get { return _region; } 
			set 
			{
			    _region = value;
				NotifyPropertyChange("Region");
			}
        }

		private String? _postalCode;
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Employees" )]		
		public String? PostalCode 
		{ 
		    get { return _postalCode; } 
			set 
			{
			    _postalCode = value;
				NotifyPropertyChange("PostalCode");
			}
        }

		private String? _country;
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Employees" )]		
		public String? Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
				NotifyPropertyChange("Country");
			}
        }

		private String? _homePhone;
		[SqlField(DbType.String, 24, ColumnName ="HomePhone", BaseColumnName ="HomePhone", BaseTableName = "Employees" )]		
		public String? HomePhone 
		{ 
		    get { return _homePhone; } 
			set 
			{
			    _homePhone = value;
				NotifyPropertyChange("HomePhone");
			}
        }

		private String? _extension;
		[SqlField(DbType.String, 4, ColumnName ="Extension", BaseColumnName ="Extension", BaseTableName = "Employees" )]		
		public String? Extension 
		{ 
		    get { return _extension; } 
			set 
			{
			    _extension = value;
				NotifyPropertyChange("Extension");
			}
        }

		private Byte[]? _photo;
		[SqlField(DbType.Binary, 2147483647, ColumnName ="Photo", BaseColumnName ="Photo", BaseTableName = "Employees" )]		
		public Byte[]? Photo 
		{ 
		    get { return _photo; } 
			set 
			{
			    _photo = value;
				NotifyPropertyChange("Photo");
			}
        }

		private String? _notes;
		[SqlField(DbType.String, 1073741823, ColumnName ="Notes", BaseColumnName ="Notes", BaseTableName = "Employees" )]		
		public String? Notes 
		{ 
		    get { return _notes; } 
			set 
			{
			    _notes = value;
				NotifyPropertyChange("Notes");
			}
        }

		private Int32? _reportsTo;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="ReportsTo", BaseColumnName ="ReportsTo", BaseTableName = "Employees" )]		
		public Int32? ReportsTo 
		{ 
		    get { return _reportsTo; } 
			set 
			{
			    _reportsTo = value;
				NotifyPropertyChange("ReportsTo");
			}
        }

		private String? _photoPath;
		[SqlField(DbType.String, 255, ColumnName ="PhotoPath", BaseColumnName ="PhotoPath", BaseTableName = "Employees" )]		
		public String? PhotoPath 
		{ 
		    get { return _photoPath; } 
			set 
			{
			    _photoPath = value;
				NotifyPropertyChange("PhotoPath");
			}
        }

		public const string BaseTableProjectionColumnList = "[EmployeeID], [LastName], [FirstName], [Title], [TitleOfCourtesy], [BirthDate], [HireDate], [Address], [City], [Region], [PostalCode], [Country], [HomePhone], [Extension], [Photo], [Notes], [ReportsTo], [PhotoPath]";

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

		public Employee Get(string projectionName, Int32 employeeId)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeId, FetchMode.UseIdentityMap);
		}

		public Employee Get(string projectionName, Int32 employeeId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeId, fetchMode);
		}

		public Employee Get(Projection projection, Int32 employeeId)
		{
			return ((IRepository<Employee>)this).Get(projection, employeeId, FetchMode.UseIdentityMap);
		}

		public Employee Get(Projection projection, Int32 employeeId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projection, employeeId, fetchMode);
		}

		public Employee Get(string projectionName, Int32 employeeId, params string[] fields)
		{
			return ((IRepository<Employee>)this).Get(projectionName, employeeId, fields);
		}

		public Employee Get(Projection projection, Int32 employeeId, params string[] fields)
		{
			return ((IRepository<Employee>)this).Get(projection, employeeId, fields);
		}

		public bool Delete(Int32 employeeId)
		{
			var entity = new Employee { EmployeeId = employeeId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
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

	public static partial class EmployeeProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[SqlEntity(BaseTableName="Order Details")]
	public partial class OrderDetail : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _orderId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, ColumnName ="OrderID", BaseColumnName ="OrderID", BaseTableName = "Order Details" )]		
		public Int32 OrderId 
		{ 
		    get { return _orderId; } 
			set 
			{
			    _orderId = value;
				NotifyPropertyChange("OrderId");
			}
        }

		private Int32 _productId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, ColumnName ="ProductID", BaseColumnName ="ProductID", BaseTableName = "Order Details" )]		
		public Int32 ProductId 
		{ 
		    get { return _productId; } 
			set 
			{
			    _productId = value;
				NotifyPropertyChange("ProductId");
			}
        }

		private Decimal _unitPrice;
		[SqlField(DbType.Currency, 8, Precision = 19, ColumnName ="UnitPrice", BaseColumnName ="UnitPrice", BaseTableName = "Order Details" )]		
		public Decimal UnitPrice 
		{ 
		    get { return _unitPrice; } 
			set 
			{
			    _unitPrice = value;
				NotifyPropertyChange("UnitPrice");
			}
        }

		private Int16 _quantity;
		[SqlField(DbType.Int16, 2, Precision = 5, ColumnName ="Quantity", BaseColumnName ="Quantity", BaseTableName = "Order Details" )]		
		public Int16 Quantity 
		{ 
		    get { return _quantity; } 
			set 
			{
			    _quantity = value;
				NotifyPropertyChange("Quantity");
			}
        }

		private Single _discount;
		[SqlField(DbType.Single, 4, Precision = 7, ColumnName ="Discount", BaseColumnName ="Discount", BaseTableName = "Order Details" )]		
		public Single Discount 
		{ 
		    get { return _discount; } 
			set 
			{
			    _discount = value;
				NotifyPropertyChange("Discount");
			}
        }

		private String? _productName;
		[SqlField(DbType.String, 40, ColumnName ="ProductName" )]		
		public String? ProductName 
		{ 
		    get { return _productName; } 
			set 
			{
			    _productName = value;
				NotifyPropertyChange("ProductName");
			}
        }

		private String? _categoryName;
		[SqlField(DbType.String, 15, ColumnName ="CategoryName" )]		
		public String? CategoryName 
		{ 
		    get { return _categoryName; } 
			set 
			{
			    _categoryName = value;
				NotifyPropertyChange("CategoryName");
			}
        }

		private Decimal? _subTotal;
		[SqlField(DbType.Currency, 8, Precision = 19, AllowNull = true, IsReadOnly = true, ColumnName ="SubTotal" )]		
		public Decimal? SubTotal 
		{ 
		    get { return _subTotal; } 
			set 
			{
			    _subTotal = value;
				NotifyPropertyChange("SubTotal");
			}
        }

		private String? _city;
		[SqlField(DbType.String, 15, ColumnName ="City" )]		
		public String? City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
				NotifyPropertyChange("City");
			}
        }

		private String? _country;
		[SqlField(DbType.String, 15, ColumnName ="Country" )]		
		public String? Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
				NotifyPropertyChange("Country");
			}
        }

		private String? _customerId;
		[SqlField(DbType.StringFixedLength, 5, ColumnName ="CustomerID" )]		
		public String? CustomerId 
		{ 
		    get { return _customerId; } 
			set 
			{
			    _customerId = value;
				NotifyPropertyChange("CustomerId");
			}
        }

		private String? _customerName;
		[SqlField(DbType.String, 40, ColumnName ="CustomerName" )]		
		public String? CustomerName 
		{ 
		    get { return _customerName; } 
			set 
			{
			    _customerName = value;
				NotifyPropertyChange("CustomerName");
			}
        }

		private String? _region;
		[SqlField(DbType.String, 15, ColumnName ="Region" )]		
		public String? Region 
		{ 
		    get { return _region; } 
			set 
			{
			    _region = value;
				NotifyPropertyChange("Region");
			}
        }

		private DateTime? _orderDate;
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="OrderDate" )]		
		public DateTime? OrderDate 
		{ 
		    get { return _orderDate; } 
			set 
			{
			    _orderDate = value;
				NotifyPropertyChange("OrderDate");
			}
        }

		public const string BaseTableProjectionColumnList = "[OrderID], [ProductID], [UnitPrice], [Quantity], [Discount]";
		public const string DetailedProjectionColumnList = "[OrderID], [ProductID], [UnitPrice], [Quantity], [Discount], [ProductName], [CategoryName], [SubTotal]";
		public const string ExtendedProjectionColumnList = "[OrderID], [ProductID], [UnitPrice], [Quantity], [Discount], [ProductName], [CategoryName], [City], [Country], [CustomerID], [CustomerName], [Region], [OrderDate]";

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

	}
	// [Obsolete("Use nameof instead")]
	public static partial class OrderDetailFields
	{
		public const string OrderId = "OrderId";
		public const string ProductId = "ProductId";
		public const string UnitPrice = "UnitPrice";
		public const string Quantity = "Quantity";
		public const string Discount = "Discount";
		public const string ProductName = "ProductName";
		public const string CategoryName = "CategoryName";
		public const string SubTotal = "SubTotal";
		public const string City = "City";
		public const string Country = "Country";
		public const string CustomerId = "CustomerId";
		public const string CustomerName = "CustomerName";
		public const string Region = "Region";
		public const string OrderDate = "OrderDate";
	}

	public static partial class OrderDetailProjections
	{
		public const string BaseTable = "BaseTable";
		public const string Detailed = "Detailed";
		public const string Extended = "Extended";
	}
	[SqlEntity(BaseTableName="OrderDetailsCopy")]
	public partial class OrderDetailCopy : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _orderId;
		[SqlField(DbType.Int32, 4, Precision = 10, ColumnName ="OrderID", BaseColumnName ="OrderID", BaseTableName = "OrderDetailsCopy" )]		
		public Int32 OrderId 
		{ 
		    get { return _orderId; } 
			set 
			{
			    _orderId = value;
				NotifyPropertyChange("OrderId");
			}
        }

		private Int32 _productId;
		[SqlField(DbType.Int32, 4, Precision = 10, ColumnName ="ProductID", BaseColumnName ="ProductID", BaseTableName = "OrderDetailsCopy" )]		
		public Int32 ProductId 
		{ 
		    get { return _productId; } 
			set 
			{
			    _productId = value;
				NotifyPropertyChange("ProductId");
			}
        }

		private Decimal _unitPrice;
		[SqlField(DbType.Currency, 8, Precision = 19, ColumnName ="UnitPrice", BaseColumnName ="UnitPrice", BaseTableName = "OrderDetailsCopy" )]		
		public Decimal UnitPrice 
		{ 
		    get { return _unitPrice; } 
			set 
			{
			    _unitPrice = value;
				NotifyPropertyChange("UnitPrice");
			}
        }

		private Int16 _quantity;
		[SqlField(DbType.Int16, 2, Precision = 5, ColumnName ="Quantity", BaseColumnName ="Quantity", BaseTableName = "OrderDetailsCopy" )]		
		public Int16 Quantity 
		{ 
		    get { return _quantity; } 
			set 
			{
			    _quantity = value;
				NotifyPropertyChange("Quantity");
			}
        }

		private Single _discount;
		[SqlField(DbType.Single, 4, Precision = 7, ColumnName ="Discount", BaseColumnName ="Discount", BaseTableName = "OrderDetailsCopy" )]		
		public Single Discount 
		{ 
		    get { return _discount; } 
			set 
			{
			    _discount = value;
				NotifyPropertyChange("Discount");
			}
        }

		public const string BaseTableProjectionColumnList = "[OrderID], [ProductID], [UnitPrice], [Quantity], [Discount]";

	}

	public partial class OrderDetailCopyRepository : Repository<OrderDetailCopy> 
	{
		public OrderDetailCopyRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

	}
	// [Obsolete("Use nameof instead")]
	public static partial class OrderDetailCopyFields
	{
		public const string OrderId = "OrderId";
		public const string ProductId = "ProductId";
		public const string UnitPrice = "UnitPrice";
		public const string Quantity = "Quantity";
		public const string Discount = "Discount";
	}

	public static partial class OrderDetailCopyProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[SqlEntity(BaseTableName="Orders")]
	public partial class Order : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _orderId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="OrderID", BaseColumnName ="OrderID", BaseTableName = "Orders" )]		
		public Int32 OrderId 
		{ 
		    get { return _orderId; } 
			set 
			{
			    _orderId = value;
				NotifyPropertyChange("OrderId");
			}
        }

		private String? _customerId;
		[SqlField(DbType.StringFixedLength, 5, ColumnName ="CustomerID", BaseColumnName ="CustomerID", BaseTableName = "Orders" )]		
		public String? CustomerId 
		{ 
		    get { return _customerId; } 
			set 
			{
			    _customerId = value;
				NotifyPropertyChange("CustomerId");
			}
        }

		private Int32? _employeeId;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="EmployeeID", BaseColumnName ="EmployeeID", BaseTableName = "Orders" )]		
		public Int32? EmployeeId 
		{ 
		    get { return _employeeId; } 
			set 
			{
			    _employeeId = value;
				NotifyPropertyChange("EmployeeId");
			}
        }

		private DateTime? _orderDate;
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="OrderDate", BaseColumnName ="OrderDate", BaseTableName = "Orders" )]		
		public DateTime? OrderDate 
		{ 
		    get { return _orderDate; } 
			set 
			{
			    _orderDate = value;
				NotifyPropertyChange("OrderDate");
			}
        }

		private DateTime? _requiredDate;
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="RequiredDate", BaseColumnName ="RequiredDate", BaseTableName = "Orders" )]		
		public DateTime? RequiredDate 
		{ 
		    get { return _requiredDate; } 
			set 
			{
			    _requiredDate = value;
				NotifyPropertyChange("RequiredDate");
			}
        }

		private DateTime? _shippedDate;
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="ShippedDate", BaseColumnName ="ShippedDate", BaseTableName = "Orders" )]		
		public DateTime? ShippedDate 
		{ 
		    get { return _shippedDate; } 
			set 
			{
			    _shippedDate = value;
				NotifyPropertyChange("ShippedDate");
			}
        }

		private Int32? _shipVia;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="ShipVia", BaseColumnName ="ShipVia", BaseTableName = "Orders" )]		
		public Int32? ShipVia 
		{ 
		    get { return _shipVia; } 
			set 
			{
			    _shipVia = value;
				NotifyPropertyChange("ShipVia");
			}
        }

		private Decimal? _freight;
		[SqlField(DbType.Currency, 8, Precision = 19, AllowNull = true, ColumnName ="Freight", BaseColumnName ="Freight", BaseTableName = "Orders" )]		
		public Decimal? Freight 
		{ 
		    get { return _freight; } 
			set 
			{
			    _freight = value;
				NotifyPropertyChange("Freight");
			}
        }

		private String? _shipName;
		[SqlField(DbType.String, 40, ColumnName ="ShipName", BaseColumnName ="ShipName", BaseTableName = "Orders" )]		
		public String? ShipName 
		{ 
		    get { return _shipName; } 
			set 
			{
			    _shipName = value;
				NotifyPropertyChange("ShipName");
			}
        }

		private String? _shipAddress;
		[SqlField(DbType.String, 60, ColumnName ="ShipAddress", BaseColumnName ="ShipAddress", BaseTableName = "Orders" )]		
		public String? ShipAddress 
		{ 
		    get { return _shipAddress; } 
			set 
			{
			    _shipAddress = value;
				NotifyPropertyChange("ShipAddress");
			}
        }

		private String? _shipCity;
		[SqlField(DbType.String, 15, ColumnName ="ShipCity", BaseColumnName ="ShipCity", BaseTableName = "Orders" )]		
		public String? ShipCity 
		{ 
		    get { return _shipCity; } 
			set 
			{
			    _shipCity = value;
				NotifyPropertyChange("ShipCity");
			}
        }

		private String? _shipRegion;
		[SqlField(DbType.String, 15, ColumnName ="ShipRegion", BaseColumnName ="ShipRegion", BaseTableName = "Orders" )]		
		public String? ShipRegion 
		{ 
		    get { return _shipRegion; } 
			set 
			{
			    _shipRegion = value;
				NotifyPropertyChange("ShipRegion");
			}
        }

		private String? _shipPostalCode;
		[SqlField(DbType.String, 10, ColumnName ="ShipPostalCode", BaseColumnName ="ShipPostalCode", BaseTableName = "Orders" )]		
		public String? ShipPostalCode 
		{ 
		    get { return _shipPostalCode; } 
			set 
			{
			    _shipPostalCode = value;
				NotifyPropertyChange("ShipPostalCode");
			}
        }

		private String? _shipCountry;
		[SqlField(DbType.String, 15, ColumnName ="ShipCountry", BaseColumnName ="ShipCountry", BaseTableName = "Orders" )]		
		public String? ShipCountry 
		{ 
		    get { return _shipCountry; } 
			set 
			{
			    _shipCountry = value;
				NotifyPropertyChange("ShipCountry");
			}
        }

		private String? _customerCompanyName;
		[SqlField(DbType.String, 40, ColumnName ="CustomerCompanyName" )]		
		public String? CustomerCompanyName 
		{ 
		    get { return _customerCompanyName; } 
			set 
			{
			    _customerCompanyName = value;
				NotifyPropertyChange("CustomerCompanyName");
			}
        }

		private String? _employeeFirstName;
		[SqlField(DbType.String, 10, ColumnName ="EmployeeFirstName" )]		
		public String? EmployeeFirstName 
		{ 
		    get { return _employeeFirstName; } 
			set 
			{
			    _employeeFirstName = value;
				NotifyPropertyChange("EmployeeFirstName");
			}
        }

		private String? _employeeLastName;
		[SqlField(DbType.String, 20, ColumnName ="EmployeeLastName" )]		
		public String? EmployeeLastName 
		{ 
		    get { return _employeeLastName; } 
			set 
			{
			    _employeeLastName = value;
				NotifyPropertyChange("EmployeeLastName");
			}
        }

		private String? _shipperCompanyName;
		[SqlField(DbType.String, 40, ColumnName ="ShipperCompanyName" )]		
		public String? ShipperCompanyName 
		{ 
		    get { return _shipperCompanyName; } 
			set 
			{
			    _shipperCompanyName = value;
				NotifyPropertyChange("ShipperCompanyName");
			}
        }

		private Double? _orderTotal;
		[SqlField(DbType.Double, 8, Precision = 15, AllowNull = true, ColumnName ="OrderTotal" )]		
		public Double? OrderTotal 
		{ 
		    get { return _orderTotal; } 
			set 
			{
			    _orderTotal = value;
				NotifyPropertyChange("OrderTotal");
			}
        }

		private Int64? _lineCount;
		[SqlField(DbType.Int64, 8, Precision = 19, AllowNull = true, ColumnName ="LineCount" )]		
		public Int64? LineCount 
		{ 
		    get { return _lineCount; } 
			set 
			{
			    _lineCount = value;
				NotifyPropertyChange("LineCount");
			}
        }

		public const string BaseTableProjectionColumnList = "[OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode], [ShipCountry]";
		public const string ExtendedProjectionColumnList = "[OrderID], [CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight], [ShipName], [ShipAddress], [ShipCity], [ShipRegion], [ShipPostalCode], [ShipCountry], [CustomerCompanyName], [EmployeeFirstName], [EmployeeLastName], [ShipperCompanyName], [OrderTotal], [LineCount]";

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

		public Order Get(string projectionName, Int32 orderId)
		{
			return ((IRepository<Order>)this).Get(projectionName, orderId, FetchMode.UseIdentityMap);
		}

		public Order Get(string projectionName, Int32 orderId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Order>)this).Get(projectionName, orderId, fetchMode);
		}

		public Order Get(Projection projection, Int32 orderId)
		{
			return ((IRepository<Order>)this).Get(projection, orderId, FetchMode.UseIdentityMap);
		}

		public Order Get(Projection projection, Int32 orderId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Order>)this).Get(projection, orderId, fetchMode);
		}

		public Order Get(string projectionName, Int32 orderId, params string[] fields)
		{
			return ((IRepository<Order>)this).Get(projectionName, orderId, fields);
		}

		public Order Get(Projection projection, Int32 orderId, params string[] fields)
		{
			return ((IRepository<Order>)this).Get(projection, orderId, fields);
		}

		public bool Delete(Int32 orderId)
		{
			var entity = new Order { OrderId = orderId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
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

	public static partial class OrderProjections
	{
		public const string BaseTable = "BaseTable";
		public const string Extended = "Extended";
	}
	[SqlEntity(BaseTableName="Products")]
	public partial class Product : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _productId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="ProductID", BaseColumnName ="ProductID", BaseTableName = "Products" )]		
		public Int32 ProductId 
		{ 
		    get { return _productId; } 
			set 
			{
			    _productId = value;
				NotifyPropertyChange("ProductId");
			}
        }

		private String? _productName;
		[SqlField(DbType.String, 40, ColumnName ="ProductName", BaseColumnName ="ProductName", BaseTableName = "Products" )]		
		public String? ProductName 
		{ 
		    get { return _productName; } 
			set 
			{
			    _productName = value;
				NotifyPropertyChange("ProductName");
			}
        }

		private Int32? _supplierId;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="SupplierID", BaseColumnName ="SupplierID", BaseTableName = "Products" )]		
		public Int32? SupplierId 
		{ 
		    get { return _supplierId; } 
			set 
			{
			    _supplierId = value;
				NotifyPropertyChange("SupplierId");
			}
        }

		private Int32? _categoryId;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="CategoryID", BaseColumnName ="CategoryID", BaseTableName = "Products" )]		
		public Int32? CategoryId 
		{ 
		    get { return _categoryId; } 
			set 
			{
			    _categoryId = value;
				NotifyPropertyChange("CategoryId");
			}
        }

		private String? _quantityPerUnit;
		[SqlField(DbType.String, 20, ColumnName ="QuantityPerUnit", BaseColumnName ="QuantityPerUnit", BaseTableName = "Products" )]		
		public String? QuantityPerUnit 
		{ 
		    get { return _quantityPerUnit; } 
			set 
			{
			    _quantityPerUnit = value;
				NotifyPropertyChange("QuantityPerUnit");
			}
        }

		private Decimal? _unitPrice;
		[SqlField(DbType.Currency, 8, Precision = 19, AllowNull = true, ColumnName ="UnitPrice", BaseColumnName ="UnitPrice", BaseTableName = "Products" )]		
		public Decimal? UnitPrice 
		{ 
		    get { return _unitPrice; } 
			set 
			{
			    _unitPrice = value;
				NotifyPropertyChange("UnitPrice");
			}
        }

		private Int16? _unitsInStock;
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsInStock", BaseColumnName ="UnitsInStock", BaseTableName = "Products" )]		
		public Int16? UnitsInStock 
		{ 
		    get { return _unitsInStock; } 
			set 
			{
			    _unitsInStock = value;
				NotifyPropertyChange("UnitsInStock");
			}
        }

		private Int16? _unitsOnOrder;
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsOnOrder", BaseColumnName ="UnitsOnOrder", BaseTableName = "Products" )]		
		public Int16? UnitsOnOrder 
		{ 
		    get { return _unitsOnOrder; } 
			set 
			{
			    _unitsOnOrder = value;
				NotifyPropertyChange("UnitsOnOrder");
			}
        }

		private Int16? _reorderLevel;
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="ReorderLevel", BaseColumnName ="ReorderLevel", BaseTableName = "Products" )]		
		public Int16? ReorderLevel 
		{ 
		    get { return _reorderLevel; } 
			set 
			{
			    _reorderLevel = value;
				NotifyPropertyChange("ReorderLevel");
			}
        }

		private Boolean _discontinued;
		[SqlField(DbType.Boolean, 1, ColumnName ="Discontinued", BaseColumnName ="Discontinued", BaseTableName = "Products" )]		
		public Boolean Discontinued 
		{ 
		    get { return _discontinued; } 
			set 
			{
			    _discontinued = value;
				NotifyPropertyChange("Discontinued");
			}
        }

		private String? _categoryName;
		[SqlField(DbType.String, 15, ColumnName ="CategoryName" )]		
		public String? CategoryName 
		{ 
		    get { return _categoryName; } 
			set 
			{
			    _categoryName = value;
				NotifyPropertyChange("CategoryName");
			}
        }

		private String? _supplierName;
		[SqlField(DbType.String, 40, ColumnName ="SupplierName" )]		
		public String? SupplierName 
		{ 
		    get { return _supplierName; } 
			set 
			{
			    _supplierName = value;
				NotifyPropertyChange("SupplierName");
			}
        }

		public const string BaseTableProjectionColumnList = "[ProductID], [ProductName], [SupplierID], [CategoryID], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]";
		public const string DetailedProjectionColumnList = "[ProductID], [ProductName], [SupplierID], [CategoryID], [QuantityPerUnit], [UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued], [CategoryName], [SupplierName]";

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

		public Product Get(string projectionName, Int32 productId)
		{
			return ((IRepository<Product>)this).Get(projectionName, productId, FetchMode.UseIdentityMap);
		}

		public Product Get(string projectionName, Int32 productId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).Get(projectionName, productId, fetchMode);
		}

		public Product Get(Projection projection, Int32 productId)
		{
			return ((IRepository<Product>)this).Get(projection, productId, FetchMode.UseIdentityMap);
		}

		public Product Get(Projection projection, Int32 productId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).Get(projection, productId, fetchMode);
		}

		public Product Get(string projectionName, Int32 productId, params string[] fields)
		{
			return ((IRepository<Product>)this).Get(projectionName, productId, fields);
		}

		public Product Get(Projection projection, Int32 productId, params string[] fields)
		{
			return ((IRepository<Product>)this).Get(projection, productId, fields);
		}

		public bool Delete(Int32 productId)
		{
			var entity = new Product { ProductId = productId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
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

	public static partial class ProductProjections
	{
		public const string BaseTable = "BaseTable";
		public const string Detailed = "Detailed";
	}
	[SqlEntity(BaseTableName="Shippers")]
	public partial class Shipper : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _shipperId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="ShipperID", BaseColumnName ="ShipperID", BaseTableName = "Shippers" )]		
		public Int32 ShipperId 
		{ 
		    get { return _shipperId; } 
			set 
			{
			    _shipperId = value;
				NotifyPropertyChange("ShipperId");
			}
        }

		private String? _companyName;
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Shippers" )]		
		public String? CompanyName 
		{ 
		    get { return _companyName; } 
			set 
			{
			    _companyName = value;
				NotifyPropertyChange("CompanyName");
			}
        }

		private String? _phone;
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Shippers" )]		
		public String? Phone 
		{ 
		    get { return _phone; } 
			set 
			{
			    _phone = value;
				NotifyPropertyChange("Phone");
			}
        }

		public const string BaseTableProjectionColumnList = "[ShipperID], [CompanyName], [Phone]";

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

		public Shipper Get(string projectionName, Int32 shipperId)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperId, FetchMode.UseIdentityMap);
		}

		public Shipper Get(string projectionName, Int32 shipperId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperId, fetchMode);
		}

		public Shipper Get(Projection projection, Int32 shipperId)
		{
			return ((IRepository<Shipper>)this).Get(projection, shipperId, FetchMode.UseIdentityMap);
		}

		public Shipper Get(Projection projection, Int32 shipperId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).Get(projection, shipperId, fetchMode);
		}

		public Shipper Get(string projectionName, Int32 shipperId, params string[] fields)
		{
			return ((IRepository<Shipper>)this).Get(projectionName, shipperId, fields);
		}

		public Shipper Get(Projection projection, Int32 shipperId, params string[] fields)
		{
			return ((IRepository<Shipper>)this).Get(projection, shipperId, fields);
		}

		public bool Delete(Int32 shipperId)
		{
			var entity = new Shipper { ShipperId = shipperId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
	public static partial class ShipperFields
	{
		public const string ShipperId = "ShipperId";
		public const string CompanyName = "CompanyName";
		public const string Phone = "Phone";
	}

	public static partial class ShipperProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[SqlEntity(BaseTableName="Suppliers")]
	public partial class Supplier : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _supplierId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="SupplierID", BaseColumnName ="SupplierID", BaseTableName = "Suppliers" )]		
		public Int32 SupplierId 
		{ 
		    get { return _supplierId; } 
			set 
			{
			    _supplierId = value;
				NotifyPropertyChange("SupplierId");
			}
        }

		private String? _companyName;
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Suppliers" )]		
		public String? CompanyName 
		{ 
		    get { return _companyName; } 
			set 
			{
			    _companyName = value;
				NotifyPropertyChange("CompanyName");
			}
        }

		private String? _contactName;
		[SqlField(DbType.String, 30, ColumnName ="ContactName", BaseColumnName ="ContactName", BaseTableName = "Suppliers" )]		
		public String? ContactName 
		{ 
		    get { return _contactName; } 
			set 
			{
			    _contactName = value;
				NotifyPropertyChange("ContactName");
			}
        }

		private String? _contactTitle;
		[SqlField(DbType.String, 30, ColumnName ="ContactTitle", BaseColumnName ="ContactTitle", BaseTableName = "Suppliers" )]		
		public String? ContactTitle 
		{ 
		    get { return _contactTitle; } 
			set 
			{
			    _contactTitle = value;
				NotifyPropertyChange("ContactTitle");
			}
        }

		private String? _address;
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Suppliers" )]		
		public String? Address 
		{ 
		    get { return _address; } 
			set 
			{
			    _address = value;
				NotifyPropertyChange("Address");
			}
        }

		private String? _city;
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Suppliers" )]		
		public String? City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
				NotifyPropertyChange("City");
			}
        }

		private String? _region;
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Suppliers" )]		
		public String? Region 
		{ 
		    get { return _region; } 
			set 
			{
			    _region = value;
				NotifyPropertyChange("Region");
			}
        }

		private String? _postalCode;
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Suppliers" )]		
		public String? PostalCode 
		{ 
		    get { return _postalCode; } 
			set 
			{
			    _postalCode = value;
				NotifyPropertyChange("PostalCode");
			}
        }

		private String? _country;
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Suppliers" )]		
		public String? Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
				NotifyPropertyChange("Country");
			}
        }

		private String? _phone;
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Suppliers" )]		
		public String? Phone 
		{ 
		    get { return _phone; } 
			set 
			{
			    _phone = value;
				NotifyPropertyChange("Phone");
			}
        }

		private String? _fax;
		[SqlField(DbType.String, 24, ColumnName ="Fax", BaseColumnName ="Fax", BaseTableName = "Suppliers" )]		
		public String? Fax 
		{ 
		    get { return _fax; } 
			set 
			{
			    _fax = value;
				NotifyPropertyChange("Fax");
			}
        }

		private String? _homePage;
		[SqlField(DbType.String, 1073741823, ColumnName ="HomePage", BaseColumnName ="HomePage", BaseTableName = "Suppliers" )]		
		public String? HomePage 
		{ 
		    get { return _homePage; } 
			set 
			{
			    _homePage = value;
				NotifyPropertyChange("HomePage");
			}
        }

		public const string BaseTableProjectionColumnList = "[SupplierID], [CompanyName], [ContactName], [ContactTitle], [Address], [City], [Region], [PostalCode], [Country], [Phone], [Fax], [HomePage]";

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

		public Supplier Get(string projectionName, Int32 supplierId)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierId, FetchMode.UseIdentityMap);
		}

		public Supplier Get(string projectionName, Int32 supplierId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierId, fetchMode);
		}

		public Supplier Get(Projection projection, Int32 supplierId)
		{
			return ((IRepository<Supplier>)this).Get(projection, supplierId, FetchMode.UseIdentityMap);
		}

		public Supplier Get(Projection projection, Int32 supplierId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).Get(projection, supplierId, fetchMode);
		}

		public Supplier Get(string projectionName, Int32 supplierId, params string[] fields)
		{
			return ((IRepository<Supplier>)this).Get(projectionName, supplierId, fields);
		}

		public Supplier Get(Projection projection, Int32 supplierId, params string[] fields)
		{
			return ((IRepository<Supplier>)this).Get(projection, supplierId, fields);
		}

		public bool Delete(Int32 supplierId)
		{
			var entity = new Supplier { SupplierId = supplierId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
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

	public static partial class SupplierProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[SqlEntity()]
	public partial class ProductSale : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32? _categoryId;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="CategoryID" )]		
		public Int32? CategoryId 
		{ 
		    get { return _categoryId; } 
			set 
			{
			    _categoryId = value;
				NotifyPropertyChange("CategoryId");
			}
        }

		private String? _categoryName;
		[SqlField(DbType.String, 15, ColumnName ="CategoryName" )]		
		public String? CategoryName 
		{ 
		    get { return _categoryName; } 
			set 
			{
			    _categoryName = value;
				NotifyPropertyChange("CategoryName");
			}
        }

		private Int32 _productId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, ColumnName ="ProductID" )]		
		public Int32 ProductId 
		{ 
		    get { return _productId; } 
			set 
			{
			    _productId = value;
				NotifyPropertyChange("ProductId");
			}
        }

		private String? _productName;
		[SqlField(DbType.String, 40, ColumnName ="ProductName" )]		
		public String? ProductName 
		{ 
		    get { return _productName; } 
			set 
			{
			    _productName = value;
				NotifyPropertyChange("ProductName");
			}
        }

		private Int32? _year;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="Year" )]		
		public Int32? Year 
		{ 
		    get { return _year; } 
			set 
			{
			    _year = value;
				NotifyPropertyChange("Year");
			}
        }

		private Int32? _quarter;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="Quarter" )]		
		public Int32? Quarter 
		{ 
		    get { return _quarter; } 
			set 
			{
			    _quarter = value;
				NotifyPropertyChange("Quarter");
			}
        }

		private Double? _sales;
		[SqlField(DbType.Double, 8, Precision = 15, AllowNull = true, ColumnName ="Sales" )]		
		public Double? Sales 
		{ 
		    get { return _sales; } 
			set 
			{
			    _sales = value;
				NotifyPropertyChange("Sales");
			}
        }

		private Int32? _orders;
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="Orders" )]		
		public Int32? Orders 
		{ 
		    get { return _orders; } 
			set 
			{
			    _orders = value;
				NotifyPropertyChange("Orders");
			}
        }

		public const string QuarterProjectionColumnList = "[CategoryID], [CategoryName], [ProductID], [ProductName], [Year], [Quarter], [Sales]";
		public const string YearProjectionColumnList = "[ProductID], [ProductName], [Year], [Sales], [Orders]";

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
	// [Obsolete("Use nameof instead")]
	public static partial class ProductSaleFields
	{
		public const string CategoryId = "CategoryId";
		public const string CategoryName = "CategoryName";
		public const string ProductId = "ProductId";
		public const string ProductName = "ProductName";
		public const string Year = "Year";
		public const string Quarter = "Quarter";
		public const string Sales = "Sales";
		public const string Orders = "Orders";
	}

	public static partial class ProductSaleProjections
	{
		public const string Quarter = "Quarter";
		public const string Year = "Year";
	}
	[SqlEntity(BaseTableName="JsonItems")]
	public partial class JsonItem : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _jsonItemId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="JsonItemId", BaseColumnName ="JsonItemId", BaseTableName = "JsonItems" )]		
		public Int32 JsonItemId 
		{ 
		    get { return _jsonItemId; } 
			set 
			{
			    _jsonItemId = value;
				NotifyPropertyChange("JsonItemId");
			}
        }

		private System.Text.Json.Nodes.JsonNode? _data;
		[SqlField(DbType.String, 2147483647, ColumnName ="DataJson", BaseColumnName ="DataJson", BaseTableName = "JsonItems" )]		
		public System.Text.Json.Nodes.JsonNode? Data 
		{ 
		    get { return _data; } 
			set 
			{
			    _data = value;
				NotifyPropertyChange("Data");
			}
        }

		public const string BaseTableProjectionColumnList = "[JsonItemId], [DataJson]";

	}

	public partial class JsonItemRepository : Repository<JsonItem> 
	{
		public JsonItemRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public JsonItem Get(string projectionName, Int32 jsonItemId)
		{
			return ((IRepository<JsonItem>)this).Get(projectionName, jsonItemId, FetchMode.UseIdentityMap);
		}

		public JsonItem Get(string projectionName, Int32 jsonItemId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<JsonItem>)this).Get(projectionName, jsonItemId, fetchMode);
		}

		public JsonItem Get(Projection projection, Int32 jsonItemId)
		{
			return ((IRepository<JsonItem>)this).Get(projection, jsonItemId, FetchMode.UseIdentityMap);
		}

		public JsonItem Get(Projection projection, Int32 jsonItemId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<JsonItem>)this).Get(projection, jsonItemId, fetchMode);
		}

		public JsonItem Get(string projectionName, Int32 jsonItemId, params string[] fields)
		{
			return ((IRepository<JsonItem>)this).Get(projectionName, jsonItemId, fields);
		}

		public JsonItem Get(Projection projection, Int32 jsonItemId, params string[] fields)
		{
			return ((IRepository<JsonItem>)this).Get(projection, jsonItemId, fields);
		}

		public bool Delete(Int32 jsonItemId)
		{
			var entity = new JsonItem { JsonItemId = jsonItemId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
	public static partial class JsonItemFields
	{
		public const string JsonItemId = "JsonItemId";
		public const string Data = "Data";
	}

	public static partial class JsonItemProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[SqlEntity(BaseTableName="DateItems")]
	public partial class DateItem : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }				
		
		private Int32 _dateItemId;
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="DateItemId", BaseColumnName ="DateItemId", BaseTableName = "DateItems" )]		
		public Int32 DateItemId 
		{ 
		    get { return _dateItemId; } 
			set 
			{
			    _dateItemId = value;
				NotifyPropertyChange("DateItemId");
			}
        }

		private DateTime? _utcDateTime;
		[System.Text.Json.Serialization.JsonConverter(typeof(inercya.System.Text.Json.Converters.UtcDateTimeJsonConverter))]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="UtcDateTime", BaseColumnName ="UtcDateTime", BaseTableName = "DateItems" )]		
		public DateTime? UtcDateTime 
		{ 
		    get { return _utcDateTime; } 
			set 
			{
			    _utcDateTime = value;
				NotifyPropertyChange("UtcDateTime");
			}
        }

		private DateTime? _itemDateTime;
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="ItemDateTime", BaseColumnName ="ItemDateTime", BaseTableName = "DateItems" )]		
		public DateTime? ItemDateTime 
		{ 
		    get { return _itemDateTime; } 
			set 
			{
			    _itemDateTime = value;
				NotifyPropertyChange("ItemDateTime");
			}
        }

		private DateTime? _itemDate;
		[System.Text.Json.Serialization.JsonConverter(typeof(inercya.System.Text.Json.Converters.RoundDateJsonConverter))]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="ItemDate", BaseColumnName ="ItemDate", BaseTableName = "DateItems" )]		
		public DateTime? ItemDate 
		{ 
		    get { return _itemDate; } 
			set 
			{
			    _itemDate = value;
				NotifyPropertyChange("ItemDate");
			}
        }

		public const string BaseTableProjectionColumnList = "[DateItemId], [UtcDateTime], [ItemDateTime], [ItemDate]";

	}

	public partial class DateItemRepository : Repository<DateItem> 
	{
		public DateItemRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public DateItem Get(string projectionName, Int32 dateItemId)
		{
			return ((IRepository<DateItem>)this).Get(projectionName, dateItemId, FetchMode.UseIdentityMap);
		}

		public DateItem Get(string projectionName, Int32 dateItemId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<DateItem>)this).Get(projectionName, dateItemId, fetchMode);
		}

		public DateItem Get(Projection projection, Int32 dateItemId)
		{
			return ((IRepository<DateItem>)this).Get(projection, dateItemId, FetchMode.UseIdentityMap);
		}

		public DateItem Get(Projection projection, Int32 dateItemId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<DateItem>)this).Get(projection, dateItemId, fetchMode);
		}

		public DateItem Get(string projectionName, Int32 dateItemId, params string[] fields)
		{
			return ((IRepository<DateItem>)this).Get(projectionName, dateItemId, fields);
		}

		public DateItem Get(Projection projection, Int32 dateItemId, params string[] fields)
		{
			return ((IRepository<DateItem>)this).Get(projection, dateItemId, fields);
		}

		public bool Delete(Int32 dateItemId)
		{
			var entity = new DateItem { DateItemId = dateItemId };
			return this.Delete(entity);
		}

			}
	// [Obsolete("Use nameof instead")]
	public static partial class DateItemFields
	{
		public const string DateItemId = "DateItemId";
		public const string UtcDateTime = "UtcDateTime";
		public const string ItemDateTime = "ItemDateTime";
		public const string ItemDate = "ItemDate";
	}

	public static partial class DateItemProjections
	{
		public const string BaseTable = "BaseTable";
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

        public NorthwindDataService() : base()
        {
			Init();
        }

        public NorthwindDataService(string connectionString) : base(System.Data.SqlClient.SqlClientFactory.Instance, connectionString)
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

		private Samples.Entities.OrderDetailCopyRepository _OrderDetailCopyRepository;
		public Samples.Entities.OrderDetailCopyRepository OrderDetailCopyRepository
		{
			get 
			{
				if ( _OrderDetailCopyRepository == null)
				{
					_OrderDetailCopyRepository = new Samples.Entities.OrderDetailCopyRepository(this);
				}
				return _OrderDetailCopyRepository;
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

		private Samples.Entities.JsonItemRepository _JsonItemRepository;
		public Samples.Entities.JsonItemRepository JsonItemRepository
		{
			get 
			{
				if ( _JsonItemRepository == null)
				{
					_JsonItemRepository = new Samples.Entities.JsonItemRepository(this);
				}
				return _JsonItemRepository;
			}
		}

		private Samples.Entities.DateItemRepository _DateItemRepository;
		public Samples.Entities.DateItemRepository DateItemRepository
		{
			get 
			{
				if ( _DateItemRepository == null)
				{
					_DateItemRepository = new Samples.Entities.DateItemRepository(this);
				}
				return _DateItemRepository;
			}
		}
	}
}
