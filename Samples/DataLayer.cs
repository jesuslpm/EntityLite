
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
// using Microsoft.SqlServer.Types;
using System.Runtime.Serialization;
using System.ComponentModel;
using inercya.EntityLite;	
using inercya.EntityLite.Extensions;	

namespace Samples.Entities
{
	[Serializable]
	[DataContract]
    [TypeScript] 
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
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="CategoryID", BaseColumnName ="CategoryID", BaseTableName = "Categories" )]		public Int32 CategoryId 
		{ 
		    get { return _categoryId; } 
			set 
			{
			    _categoryId = value;
				NotifyPropertyChange("CategoryId");
			}
        }

		private String _categoryNameLang1;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryNameLang1", BaseColumnName ="CategoryNameLang1", BaseTableName = "Categories" )]		public String CategoryNameLang1 
		{ 
		    get { return _categoryNameLang1; } 
			set 
			{
			    _categoryNameLang1 = value;
				NotifyPropertyChange("CategoryNameLang1");
			}
        }

		private String _description;
		[DataMember]
		[SqlField(DbType.String, 2147483647, ColumnName ="Description", BaseColumnName ="Description", BaseTableName = "Categories" )]		public String Description 
		{ 
		    get { return _description; } 
			set 
			{
			    _description = value;
				NotifyPropertyChange("Description");
			}
        }

		private Byte[] _picture;
		[DataMember]
		[SqlField(DbType.Binary, 2147483647, ColumnName ="Picture", BaseColumnName ="Picture", BaseTableName = "Categories" )]		public Byte[] Picture 
		{ 
		    get { return _picture; } 
			set 
			{
			    _picture = value;
				NotifyPropertyChange("Picture");
			}
        }

		private String _categoryNameLang2;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryNameLang2", BaseColumnName ="CategoryNameLang2", BaseTableName = "Categories" )]		public String CategoryNameLang2 
		{ 
		    get { return _categoryNameLang2; } 
			set 
			{
			    _categoryNameLang2 = value;
				NotifyPropertyChange("CategoryNameLang2");
			}
        }

		[LocalizedField]
		[DataMember]
		public string CategoryName 
		{ 
			get
			{
				return CurrentLanguageService.GetLocalizedValue(this, "CategoryName");
			} 
		}

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

		public bool Delete(System.Int32 categoryId)
		{
			var entity = new Category { CategoryId = categoryId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<Category> GetAsync(string projectionName, System.Int32 categoryId)
		{
			return ((IRepository<Category>)this).GetAsync(projectionName, categoryId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Category> GetAsync(string projectionName, System.Int32 categoryId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).GetAsync(projectionName, categoryId, fetchMode);
		}

		public System.Threading.Tasks.Task<Category> GetAsync(Projection projection, System.Int32 categoryId)
		{
			return ((IRepository<Category>)this).GetAsync(projection, categoryId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Category> GetAsync(Projection projection, System.Int32 categoryId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Category>)this).GetAsync(projection, categoryId, fetchMode);
		}

		public System.Threading.Tasks.Task<Category> GetAsync(string projectionName, System.Int32 categoryId, params string[] fields)
		{
			return ((IRepository<Category>)this).GetAsync(projectionName, categoryId, fields);
		}

		public System.Threading.Tasks.Task<Category> GetAsync(Projection projection, System.Int32 categoryId, params string[] fields)
		{
			return ((IRepository<Category>)this).GetAsync(projection, categoryId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 categoryId)
		{
			var entity = new Category { CategoryId = categoryId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class CategoryFields
	{
		public const string CategoryId = "CategoryId";
		public const string CategoryNameLang1 = "CategoryNameLang1";
		public const string Description = "Description";
		public const string Picture = "Picture";
		public const string CategoryNameLang2 = "CategoryNameLang2";
		public const string CategoryName = "CategoryName";
	}

	[Serializable]
	[DataContract]
    [TypeScript] 
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
		
		private String _customerId;
		[DataMember]
		[SqlField(DbType.StringFixedLength, 5, IsKey=true, ColumnName ="CustomerID", BaseColumnName ="CustomerID", BaseTableName = "Customers" )]		public String CustomerId 
		{ 
		    get { return _customerId; } 
			set 
			{
			    _customerId = value;
				NotifyPropertyChange("CustomerId");
			}
        }

		private String _companyName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Customers" )]		public String CompanyName 
		{ 
		    get { return _companyName; } 
			set 
			{
			    _companyName = value;
				NotifyPropertyChange("CompanyName");
			}
        }

		private String _contactName;
		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ContactName", BaseColumnName ="ContactName", BaseTableName = "Customers" )]		public String ContactName 
		{ 
		    get { return _contactName; } 
			set 
			{
			    _contactName = value;
				NotifyPropertyChange("ContactName");
			}
        }

		private String _contactTitle;
		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ContactTitle", BaseColumnName ="ContactTitle", BaseTableName = "Customers" )]		public String ContactTitle 
		{ 
		    get { return _contactTitle; } 
			set 
			{
			    _contactTitle = value;
				NotifyPropertyChange("ContactTitle");
			}
        }

		private String _address;
		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Customers" )]		public String Address 
		{ 
		    get { return _address; } 
			set 
			{
			    _address = value;
				NotifyPropertyChange("Address");
			}
        }

		private String _city;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Customers" )]		public String City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
				NotifyPropertyChange("City");
			}
        }

		private String _region;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Customers" )]		public String Region 
		{ 
		    get { return _region; } 
			set 
			{
			    _region = value;
				NotifyPropertyChange("Region");
			}
        }

		private String _postalCode;
		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Customers" )]		public String PostalCode 
		{ 
		    get { return _postalCode; } 
			set 
			{
			    _postalCode = value;
				NotifyPropertyChange("PostalCode");
			}
        }

		private String _country;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Customers" )]		public String Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
				NotifyPropertyChange("Country");
			}
        }

		private String _phone;
		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Customers" )]		public String Phone 
		{ 
		    get { return _phone; } 
			set 
			{
			    _phone = value;
				NotifyPropertyChange("Phone");
			}
        }

		private String _fax;
		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Fax", BaseColumnName ="Fax", BaseTableName = "Customers" )]		public String Fax 
		{ 
		    get { return _fax; } 
			set 
			{
			    _fax = value;
				NotifyPropertyChange("Fax");
			}
        }


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

		public bool Delete(System.String customerId)
		{
			var entity = new Customer { CustomerId = customerId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<Customer> GetAsync(string projectionName, System.String customerId)
		{
			return ((IRepository<Customer>)this).GetAsync(projectionName, customerId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Customer> GetAsync(string projectionName, System.String customerId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).GetAsync(projectionName, customerId, fetchMode);
		}

		public System.Threading.Tasks.Task<Customer> GetAsync(Projection projection, System.String customerId)
		{
			return ((IRepository<Customer>)this).GetAsync(projection, customerId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Customer> GetAsync(Projection projection, System.String customerId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).GetAsync(projection, customerId, fetchMode);
		}

		public System.Threading.Tasks.Task<Customer> GetAsync(string projectionName, System.String customerId, params string[] fields)
		{
			return ((IRepository<Customer>)this).GetAsync(projectionName, customerId, fields);
		}

		public System.Threading.Tasks.Task<Customer> GetAsync(Projection projection, System.String customerId, params string[] fields)
		{
			return ((IRepository<Customer>)this).GetAsync(projection, customerId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.String customerId)
		{
			var entity = new Customer { CustomerId = customerId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
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
    [TypeScript] 
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
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="EmployeeID", BaseColumnName ="EmployeeID", BaseTableName = "Employees" )]		public Int32 EmployeeId 
		{ 
		    get { return _employeeId; } 
			set 
			{
			    _employeeId = value;
				NotifyPropertyChange("EmployeeId");
			}
        }

		private String _lastName;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="LastName", BaseColumnName ="LastName", BaseTableName = "Employees" )]		public String LastName 
		{ 
		    get { return _lastName; } 
			set 
			{
			    _lastName = value;
				NotifyPropertyChange("LastName");
			}
        }

		private String _firstName;
		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="FirstName", BaseColumnName ="FirstName", BaseTableName = "Employees" )]		public String FirstName 
		{ 
		    get { return _firstName; } 
			set 
			{
			    _firstName = value;
				NotifyPropertyChange("FirstName");
			}
        }

		private String _title;
		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="Title", BaseColumnName ="Title", BaseTableName = "Employees" )]		public String Title 
		{ 
		    get { return _title; } 
			set 
			{
			    _title = value;
				NotifyPropertyChange("Title");
			}
        }

		private String _titleOfCourtesy;
		[DataMember]
		[SqlField(DbType.String, 25, ColumnName ="TitleOfCourtesy", BaseColumnName ="TitleOfCourtesy", BaseTableName = "Employees" )]		public String TitleOfCourtesy 
		{ 
		    get { return _titleOfCourtesy; } 
			set 
			{
			    _titleOfCourtesy = value;
				NotifyPropertyChange("TitleOfCourtesy");
			}
        }

		private DateTime? _birthDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="BirthDate", BaseColumnName ="BirthDate", BaseTableName = "Employees" )]		public DateTime? BirthDate 
		{ 
		    get { return _birthDate; } 
			set 
			{
			    _birthDate = value;
				NotifyPropertyChange("BirthDate");
			}
        }

		private DateTime? _hireDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="HireDate", BaseColumnName ="HireDate", BaseTableName = "Employees" )]		public DateTime? HireDate 
		{ 
		    get { return _hireDate; } 
			set 
			{
			    _hireDate = value;
				NotifyPropertyChange("HireDate");
			}
        }

		private String _address;
		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Employees" )]		public String Address 
		{ 
		    get { return _address; } 
			set 
			{
			    _address = value;
				NotifyPropertyChange("Address");
			}
        }

		private String _city;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Employees" )]		public String City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
				NotifyPropertyChange("City");
			}
        }

		private String _region;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Employees" )]		public String Region 
		{ 
		    get { return _region; } 
			set 
			{
			    _region = value;
				NotifyPropertyChange("Region");
			}
        }

		private String _postalCode;
		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Employees" )]		public String PostalCode 
		{ 
		    get { return _postalCode; } 
			set 
			{
			    _postalCode = value;
				NotifyPropertyChange("PostalCode");
			}
        }

		private String _country;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Employees" )]		public String Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
				NotifyPropertyChange("Country");
			}
        }

		private String _homePhone;
		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="HomePhone", BaseColumnName ="HomePhone", BaseTableName = "Employees" )]		public String HomePhone 
		{ 
		    get { return _homePhone; } 
			set 
			{
			    _homePhone = value;
				NotifyPropertyChange("HomePhone");
			}
        }

		private String _extension;
		[DataMember]
		[SqlField(DbType.String, 4, ColumnName ="Extension", BaseColumnName ="Extension", BaseTableName = "Employees" )]		public String Extension 
		{ 
		    get { return _extension; } 
			set 
			{
			    _extension = value;
				NotifyPropertyChange("Extension");
			}
        }

		private Byte[] _photo;
		[DataMember]
		[SqlField(DbType.Binary, 2147483647, ColumnName ="Photo", BaseColumnName ="Photo", BaseTableName = "Employees" )]		public Byte[] Photo 
		{ 
		    get { return _photo; } 
			set 
			{
			    _photo = value;
				NotifyPropertyChange("Photo");
			}
        }

		private String _notes;
		[DataMember]
		[SqlField(DbType.String, 1073741823, ColumnName ="Notes", BaseColumnName ="Notes", BaseTableName = "Employees" )]		public String Notes 
		{ 
		    get { return _notes; } 
			set 
			{
			    _notes = value;
				NotifyPropertyChange("Notes");
			}
        }

		private Int32? _reportsTo;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="ReportsTo", BaseColumnName ="ReportsTo", BaseTableName = "Employees" )]		public Int32? ReportsTo 
		{ 
		    get { return _reportsTo; } 
			set 
			{
			    _reportsTo = value;
				NotifyPropertyChange("ReportsTo");
			}
        }

		private String _photoPath;
		[DataMember]
		[SqlField(DbType.String, 255, ColumnName ="PhotoPath", BaseColumnName ="PhotoPath", BaseTableName = "Employees" )]		public String PhotoPath 
		{ 
		    get { return _photoPath; } 
			set 
			{
			    _photoPath = value;
				NotifyPropertyChange("PhotoPath");
			}
        }


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

		public bool Delete(System.Int32 employeeId)
		{
			var entity = new Employee { EmployeeId = employeeId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<Employee> GetAsync(string projectionName, System.Int32 employeeId)
		{
			return ((IRepository<Employee>)this).GetAsync(projectionName, employeeId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Employee> GetAsync(string projectionName, System.Int32 employeeId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).GetAsync(projectionName, employeeId, fetchMode);
		}

		public System.Threading.Tasks.Task<Employee> GetAsync(Projection projection, System.Int32 employeeId)
		{
			return ((IRepository<Employee>)this).GetAsync(projection, employeeId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Employee> GetAsync(Projection projection, System.Int32 employeeId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).GetAsync(projection, employeeId, fetchMode);
		}

		public System.Threading.Tasks.Task<Employee> GetAsync(string projectionName, System.Int32 employeeId, params string[] fields)
		{
			return ((IRepository<Employee>)this).GetAsync(projectionName, employeeId, fields);
		}

		public System.Threading.Tasks.Task<Employee> GetAsync(Projection projection, System.Int32 employeeId, params string[] fields)
		{
			return ((IRepository<Employee>)this).GetAsync(projection, employeeId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 employeeId)
		{
			var entity = new Employee { EmployeeId = employeeId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
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
    [TypeScript] 
	[SqlEntity(BaseTableName="OrderDetails")]
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
		
		private Int32 _orderDetailId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="OrderDetailID", BaseColumnName ="OrderDetailID", BaseTableName = "OrderDetails" )]		public Int32 OrderDetailId 
		{ 
		    get { return _orderDetailId; } 
			set 
			{
			    _orderDetailId = value;
				NotifyPropertyChange("OrderDetailId");
			}
        }

		private Int32 _orderId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, ColumnName ="OrderID", BaseColumnName ="OrderID", BaseTableName = "OrderDetails" )]		public Int32 OrderId 
		{ 
		    get { return _orderId; } 
			set 
			{
			    _orderId = value;
				NotifyPropertyChange("OrderId");
			}
        }

		private Int32 _productId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, ColumnName ="ProductID", BaseColumnName ="ProductID", BaseTableName = "OrderDetails" )]		public Int32 ProductId 
		{ 
		    get { return _productId; } 
			set 
			{
			    _productId = value;
				NotifyPropertyChange("ProductId");
			}
        }

		private Decimal _unitPrice;
		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 19, Scale=4, ColumnName ="UnitPrice", BaseColumnName ="UnitPrice", BaseTableName = "OrderDetails" )]		public Decimal UnitPrice 
		{ 
		    get { return _unitPrice; } 
			set 
			{
			    _unitPrice = value;
				NotifyPropertyChange("UnitPrice");
			}
        }

		private Int16 _quantity;
		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, ColumnName ="Quantity", BaseColumnName ="Quantity", BaseTableName = "OrderDetails" )]		public Int16 Quantity 
		{ 
		    get { return _quantity; } 
			set 
			{
			    _quantity = value;
				NotifyPropertyChange("Quantity");
			}
        }

		private Decimal _discount;
		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 5, Scale=4, ColumnName ="Discount", BaseColumnName ="Discount", BaseTableName = "OrderDetails" )]		public Decimal Discount 
		{ 
		    get { return _discount; } 
			set 
			{
			    _discount = value;
				NotifyPropertyChange("Discount");
			}
        }

		private Decimal? _subTotal;
		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 32, Scale=8, AllowNull = true, IsReadOnly = true, ColumnName ="SubTotal" )]		public Decimal? SubTotal 
		{ 
		    get { return _subTotal; } 
			set 
			{
			    _subTotal = value;
				NotifyPropertyChange("SubTotal");
			}
        }

		private String _productName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ProductName" )]		public String ProductName 
		{ 
		    get { return _productName; } 
			set 
			{
			    _productName = value;
				NotifyPropertyChange("ProductName");
			}
        }

		private String _categoryNameLang1;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryNameLang1" )]		public String CategoryNameLang1 
		{ 
		    get { return _categoryNameLang1; } 
			set 
			{
			    _categoryNameLang1 = value;
				NotifyPropertyChange("CategoryNameLang1");
			}
        }

		private String _categoryNameLang2;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryNameLang2" )]		public String CategoryNameLang2 
		{ 
		    get { return _categoryNameLang2; } 
			set 
			{
			    _categoryNameLang2 = value;
				NotifyPropertyChange("CategoryNameLang2");
			}
        }

		private DateTime? _orderDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="OrderDate" )]		public DateTime? OrderDate 
		{ 
		    get { return _orderDate; } 
			set 
			{
			    _orderDate = value;
				NotifyPropertyChange("OrderDate");
			}
        }

		private String _customerId;
		[DataMember]
		[SqlField(DbType.StringFixedLength, 5, ColumnName ="CustomerID" )]		public String CustomerId 
		{ 
		    get { return _customerId; } 
			set 
			{
			    _customerId = value;
				NotifyPropertyChange("CustomerId");
			}
        }

		private String _customerName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CustomerName" )]		public String CustomerName 
		{ 
		    get { return _customerName; } 
			set 
			{
			    _customerName = value;
				NotifyPropertyChange("CustomerName");
			}
        }

		private String _address;
		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="Address" )]		public String Address 
		{ 
		    get { return _address; } 
			set 
			{
			    _address = value;
				NotifyPropertyChange("Address");
			}
        }

		private String _city;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="City" )]		public String City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
				NotifyPropertyChange("City");
			}
        }

		private String _region;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Region" )]		public String Region 
		{ 
		    get { return _region; } 
			set 
			{
			    _region = value;
				NotifyPropertyChange("Region");
			}
        }

		private String _postalCode;
		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="PostalCode" )]		public String PostalCode 
		{ 
		    get { return _postalCode; } 
			set 
			{
			    _postalCode = value;
				NotifyPropertyChange("PostalCode");
			}
        }

		private String _country;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Country" )]		public String Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
				NotifyPropertyChange("Country");
			}
        }

		private String _phone;
		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Phone" )]		public String Phone 
		{ 
		    get { return _phone; } 
			set 
			{
			    _phone = value;
				NotifyPropertyChange("Phone");
			}
        }

		private Int16? _unitsInStock;
		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsInStock" )]		public Int16? UnitsInStock 
		{ 
		    get { return _unitsInStock; } 
			set 
			{
			    _unitsInStock = value;
				NotifyPropertyChange("UnitsInStock");
			}
        }

		private Int16? _unitsOnOrder;
		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsOnOrder" )]		public Int16? UnitsOnOrder 
		{ 
		    get { return _unitsOnOrder; } 
			set 
			{
			    _unitsOnOrder = value;
				NotifyPropertyChange("UnitsOnOrder");
			}
        }

		[LocalizedField]
		[DataMember]
		public string CategoryName 
		{ 
			get
			{
				return CurrentLanguageService.GetLocalizedValue(this, "CategoryName");
			} 
		}

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

		public bool Delete(System.Int32 orderDetailId)
		{
			var entity = new OrderDetail { OrderDetailId = orderDetailId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<OrderDetail> GetAsync(string projectionName, System.Int32 orderDetailId)
		{
			return ((IRepository<OrderDetail>)this).GetAsync(projectionName, orderDetailId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<OrderDetail> GetAsync(string projectionName, System.Int32 orderDetailId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<OrderDetail>)this).GetAsync(projectionName, orderDetailId, fetchMode);
		}

		public System.Threading.Tasks.Task<OrderDetail> GetAsync(Projection projection, System.Int32 orderDetailId)
		{
			return ((IRepository<OrderDetail>)this).GetAsync(projection, orderDetailId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<OrderDetail> GetAsync(Projection projection, System.Int32 orderDetailId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<OrderDetail>)this).GetAsync(projection, orderDetailId, fetchMode);
		}

		public System.Threading.Tasks.Task<OrderDetail> GetAsync(string projectionName, System.Int32 orderDetailId, params string[] fields)
		{
			return ((IRepository<OrderDetail>)this).GetAsync(projectionName, orderDetailId, fields);
		}

		public System.Threading.Tasks.Task<OrderDetail> GetAsync(Projection projection, System.Int32 orderDetailId, params string[] fields)
		{
			return ((IRepository<OrderDetail>)this).GetAsync(projection, orderDetailId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 orderDetailId)
		{
			var entity = new OrderDetail { OrderDetailId = orderDetailId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
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
		public const string CategoryNameLang1 = "CategoryNameLang1";
		public const string CategoryNameLang2 = "CategoryNameLang2";
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
		public const string CategoryName = "CategoryName";
	}

	[Serializable]
	[DataContract]
    [TypeScript] 
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
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="OrderID", BaseColumnName ="OrderID", BaseTableName = "Orders" )]		public Int32 OrderId 
		{ 
		    get { return _orderId; } 
			set 
			{
			    _orderId = value;
				NotifyPropertyChange("OrderId");
			}
        }

		private String _customerId;
		[DataMember]
		[SqlField(DbType.StringFixedLength, 5, ColumnName ="CustomerID", BaseColumnName ="CustomerID", BaseTableName = "Orders" )]		public String CustomerId 
		{ 
		    get { return _customerId; } 
			set 
			{
			    _customerId = value;
				NotifyPropertyChange("CustomerId");
			}
        }

		private Int32? _employeeId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="EmployeeID", BaseColumnName ="EmployeeID", BaseTableName = "Orders" )]		public Int32? EmployeeId 
		{ 
		    get { return _employeeId; } 
			set 
			{
			    _employeeId = value;
				NotifyPropertyChange("EmployeeId");
			}
        }

		private DateTime? _orderDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="OrderDate", BaseColumnName ="OrderDate", BaseTableName = "Orders" )]		public DateTime? OrderDate 
		{ 
		    get { return _orderDate; } 
			set 
			{
			    _orderDate = value;
				NotifyPropertyChange("OrderDate");
			}
        }

		private DateTime? _requiredDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="RequiredDate", BaseColumnName ="RequiredDate", BaseTableName = "Orders" )]		public DateTime? RequiredDate 
		{ 
		    get { return _requiredDate; } 
			set 
			{
			    _requiredDate = value;
				NotifyPropertyChange("RequiredDate");
			}
        }

		private DateTime? _shippedDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, Precision = 23, Scale=3, AllowNull = true, ColumnName ="ShippedDate", BaseColumnName ="ShippedDate", BaseTableName = "Orders" )]		public DateTime? ShippedDate 
		{ 
		    get { return _shippedDate; } 
			set 
			{
			    _shippedDate = value;
				NotifyPropertyChange("ShippedDate");
			}
        }

		private Int32? _shipVia;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="ShipVia", BaseColumnName ="ShipVia", BaseTableName = "Orders" )]		public Int32? ShipVia 
		{ 
		    get { return _shipVia; } 
			set 
			{
			    _shipVia = value;
				NotifyPropertyChange("ShipVia");
			}
        }

		private Decimal? _freight;
		[DataMember]
		[SqlField(DbType.Currency, 8, Precision = 19, AllowNull = true, ColumnName ="Freight", BaseColumnName ="Freight", BaseTableName = "Orders" )]		public Decimal? Freight 
		{ 
		    get { return _freight; } 
			set 
			{
			    _freight = value;
				NotifyPropertyChange("Freight");
			}
        }

		private String _shipName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ShipName", BaseColumnName ="ShipName", BaseTableName = "Orders" )]		public String ShipName 
		{ 
		    get { return _shipName; } 
			set 
			{
			    _shipName = value;
				NotifyPropertyChange("ShipName");
			}
        }

		private String _shipAddress;
		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="ShipAddress", BaseColumnName ="ShipAddress", BaseTableName = "Orders" )]		public String ShipAddress 
		{ 
		    get { return _shipAddress; } 
			set 
			{
			    _shipAddress = value;
				NotifyPropertyChange("ShipAddress");
			}
        }

		private String _shipCity;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="ShipCity", BaseColumnName ="ShipCity", BaseTableName = "Orders" )]		public String ShipCity 
		{ 
		    get { return _shipCity; } 
			set 
			{
			    _shipCity = value;
				NotifyPropertyChange("ShipCity");
			}
        }

		private String _shipRegion;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="ShipRegion", BaseColumnName ="ShipRegion", BaseTableName = "Orders" )]		public String ShipRegion 
		{ 
		    get { return _shipRegion; } 
			set 
			{
			    _shipRegion = value;
				NotifyPropertyChange("ShipRegion");
			}
        }

		private String _shipPostalCode;
		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="ShipPostalCode", BaseColumnName ="ShipPostalCode", BaseTableName = "Orders" )]		public String ShipPostalCode 
		{ 
		    get { return _shipPostalCode; } 
			set 
			{
			    _shipPostalCode = value;
				NotifyPropertyChange("ShipPostalCode");
			}
        }

		private String _shipCountry;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="ShipCountry", BaseColumnName ="ShipCountry", BaseTableName = "Orders" )]		public String ShipCountry 
		{ 
		    get { return _shipCountry; } 
			set 
			{
			    _shipCountry = value;
				NotifyPropertyChange("ShipCountry");
			}
        }

		private String _customerCompanyName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CustomerCompanyName" )]		public String CustomerCompanyName 
		{ 
		    get { return _customerCompanyName; } 
			set 
			{
			    _customerCompanyName = value;
				NotifyPropertyChange("CustomerCompanyName");
			}
        }

		private String _employeeFirstName;
		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="EmployeeFirstName" )]		public String EmployeeFirstName 
		{ 
		    get { return _employeeFirstName; } 
			set 
			{
			    _employeeFirstName = value;
				NotifyPropertyChange("EmployeeFirstName");
			}
        }

		private String _employeeLastName;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="EmployeeLastName" )]		public String EmployeeLastName 
		{ 
		    get { return _employeeLastName; } 
			set 
			{
			    _employeeLastName = value;
				NotifyPropertyChange("EmployeeLastName");
			}
        }

		private String _shipperCompanyName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ShipperCompanyName" )]		public String ShipperCompanyName 
		{ 
		    get { return _shipperCompanyName; } 
			set 
			{
			    _shipperCompanyName = value;
				NotifyPropertyChange("ShipperCompanyName");
			}
        }

		private Decimal? _orderTotal;
		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 38, Scale=8, AllowNull = true, ColumnName ="OrderTotal" )]		public Decimal? OrderTotal 
		{ 
		    get { return _orderTotal; } 
			set 
			{
			    _orderTotal = value;
				NotifyPropertyChange("OrderTotal");
			}
        }

		private Int64? _lineCount;
		[DataMember]
		[SqlField(DbType.Int64, 8, Precision = 19, AllowNull = true, ColumnName ="LineCount" )]		public Int64? LineCount 
		{ 
		    get { return _lineCount; } 
			set 
			{
			    _lineCount = value;
				NotifyPropertyChange("LineCount");
			}
        }


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

		public bool Delete(System.Int32 orderId)
		{
			var entity = new Order { OrderId = orderId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<Order> GetAsync(string projectionName, System.Int32 orderId)
		{
			return ((IRepository<Order>)this).GetAsync(projectionName, orderId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Order> GetAsync(string projectionName, System.Int32 orderId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Order>)this).GetAsync(projectionName, orderId, fetchMode);
		}

		public System.Threading.Tasks.Task<Order> GetAsync(Projection projection, System.Int32 orderId)
		{
			return ((IRepository<Order>)this).GetAsync(projection, orderId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Order> GetAsync(Projection projection, System.Int32 orderId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Order>)this).GetAsync(projection, orderId, fetchMode);
		}

		public System.Threading.Tasks.Task<Order> GetAsync(string projectionName, System.Int32 orderId, params string[] fields)
		{
			return ((IRepository<Order>)this).GetAsync(projectionName, orderId, fields);
		}

		public System.Threading.Tasks.Task<Order> GetAsync(Projection projection, System.Int32 orderId, params string[] fields)
		{
			return ((IRepository<Order>)this).GetAsync(projection, orderId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 orderId)
		{
			var entity = new Order { OrderId = orderId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
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
    [TypeScript] 
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
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="ProductID", BaseColumnName ="ProductID", BaseTableName = "Products" )]		public Int32 ProductId 
		{ 
		    get { return _productId; } 
			set 
			{
			    _productId = value;
				NotifyPropertyChange("ProductId");
			}
        }

		private String _productName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ProductName", BaseColumnName ="ProductName", BaseTableName = "Products" )]		public String ProductName 
		{ 
		    get { return _productName; } 
			set 
			{
			    _productName = value;
				NotifyPropertyChange("ProductName");
			}
        }

		private Int32? _supplierId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="SupplierID", BaseColumnName ="SupplierID", BaseTableName = "Products" )]		public Int32? SupplierId 
		{ 
		    get { return _supplierId; } 
			set 
			{
			    _supplierId = value;
				NotifyPropertyChange("SupplierId");
			}
        }

		private Int32? _categoryId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="CategoryID", BaseColumnName ="CategoryID", BaseTableName = "Products" )]		public Int32? CategoryId 
		{ 
		    get { return _categoryId; } 
			set 
			{
			    _categoryId = value;
				NotifyPropertyChange("CategoryId");
			}
        }

		private String _quantityPerUnit;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="QuantityPerUnit", BaseColumnName ="QuantityPerUnit", BaseTableName = "Products" )]		public String QuantityPerUnit 
		{ 
		    get { return _quantityPerUnit; } 
			set 
			{
			    _quantityPerUnit = value;
				NotifyPropertyChange("QuantityPerUnit");
			}
        }

		private Decimal _unitPrice;
		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 19, Scale=4, ColumnName ="UnitPrice", BaseColumnName ="UnitPrice", BaseTableName = "Products" )]		public Decimal UnitPrice 
		{ 
		    get { return _unitPrice; } 
			set 
			{
			    _unitPrice = value;
				NotifyPropertyChange("UnitPrice");
			}
        }

		private Int16? _unitsInStock;
		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsInStock", BaseColumnName ="UnitsInStock", BaseTableName = "Products" )]		public Int16? UnitsInStock 
		{ 
		    get { return _unitsInStock; } 
			set 
			{
			    _unitsInStock = value;
				NotifyPropertyChange("UnitsInStock");
			}
        }

		private Int16? _unitsOnOrder;
		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="UnitsOnOrder", BaseColumnName ="UnitsOnOrder", BaseTableName = "Products" )]		public Int16? UnitsOnOrder 
		{ 
		    get { return _unitsOnOrder; } 
			set 
			{
			    _unitsOnOrder = value;
				NotifyPropertyChange("UnitsOnOrder");
			}
        }

		private Int16? _reorderLevel;
		[DataMember]
		[SqlField(DbType.Int16, 2, Precision = 5, AllowNull = true, ColumnName ="ReorderLevel", BaseColumnName ="ReorderLevel", BaseTableName = "Products" )]		public Int16? ReorderLevel 
		{ 
		    get { return _reorderLevel; } 
			set 
			{
			    _reorderLevel = value;
				NotifyPropertyChange("ReorderLevel");
			}
        }

		private Boolean _discontinued;
		[DataMember]
		[SqlField(DbType.Boolean, 1, ColumnName ="Discontinued", BaseColumnName ="Discontinued", BaseTableName = "Products" )]		public Boolean Discontinued 
		{ 
		    get { return _discontinued; } 
			set 
			{
			    _discontinued = value;
				NotifyPropertyChange("Discontinued");
			}
        }

		private Int32 _entityRowVersion;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, ColumnName ="EntityRowVersion", BaseColumnName ="EntityRowVersion", BaseTableName = "Products" )]		public Int32 EntityRowVersion 
		{ 
		    get { return _entityRowVersion; } 
			set 
			{
			    _entityRowVersion = value;
				NotifyPropertyChange("EntityRowVersion");
			}
        }

		private String _categoryNameLang1;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryNameLang1" )]		public String CategoryNameLang1 
		{ 
		    get { return _categoryNameLang1; } 
			set 
			{
			    _categoryNameLang1 = value;
				NotifyPropertyChange("CategoryNameLang1");
			}
        }

		private String _categoryNameLang2;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryNameLang2" )]		public String CategoryNameLang2 
		{ 
		    get { return _categoryNameLang2; } 
			set 
			{
			    _categoryNameLang2 = value;
				NotifyPropertyChange("CategoryNameLang2");
			}
        }

		private String _supplierName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="SupplierName" )]		public String SupplierName 
		{ 
		    get { return _supplierName; } 
			set 
			{
			    _supplierName = value;
				NotifyPropertyChange("SupplierName");
			}
        }

		[LocalizedField]
		[DataMember]
		public string CategoryName 
		{ 
			get
			{
				return CurrentLanguageService.GetLocalizedValue(this, "CategoryName");
			} 
		}

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

		public bool Delete(System.Int32 productId)
		{
			var entity = new Product { ProductId = productId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<Product> GetAsync(string projectionName, System.Int32 productId)
		{
			return ((IRepository<Product>)this).GetAsync(projectionName, productId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Product> GetAsync(string projectionName, System.Int32 productId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).GetAsync(projectionName, productId, fetchMode);
		}

		public System.Threading.Tasks.Task<Product> GetAsync(Projection projection, System.Int32 productId)
		{
			return ((IRepository<Product>)this).GetAsync(projection, productId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Product> GetAsync(Projection projection, System.Int32 productId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Product>)this).GetAsync(projection, productId, fetchMode);
		}

		public System.Threading.Tasks.Task<Product> GetAsync(string projectionName, System.Int32 productId, params string[] fields)
		{
			return ((IRepository<Product>)this).GetAsync(projectionName, productId, fields);
		}

		public System.Threading.Tasks.Task<Product> GetAsync(Projection projection, System.Int32 productId, params string[] fields)
		{
			return ((IRepository<Product>)this).GetAsync(projection, productId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 productId)
		{
			var entity = new Product { ProductId = productId };
			return this.DeleteAsync(entity);
		}


		public void RaiseProductPrices(Decimal? rate)
		{
            var executor = new StoredProcedureExecutor(this.DataService, true)
            {
                CommandTimeout = 10,
                GetCommandFunc = () =>
                {
                    var proc =  Samples.Entities.StoredProcedures.CreateRaiseProductPricesProcedure(this.DataService.Connection, this.DataService.EntityLiteProvider.ParameterPrefix);
					proc.Parameters[this.DataService.EntityLiteProvider.ParameterPrefix + "rate"].Value = rate == null ? (object) DBNull.Value : rate.Value;
                    return proc;
                }
            };

			executor.ExecuteNonQuery();
		}

		public async System.Threading.Tasks.Task RaiseProductPricesAsync(Decimal? rate)
		{
            var executor = new StoredProcedureExecutor(this.DataService, true)
            {
                CommandTimeout = 10,
                GetCommandFunc = () =>
                {
                    var proc =  Samples.Entities.StoredProcedures.CreateRaiseProductPricesProcedure(this.DataService.Connection, this.DataService.EntityLiteProvider.ParameterPrefix);
					proc.Parameters[this.DataService.EntityLiteProvider.ParameterPrefix + "rate"].Value = rate == null ? (object) DBNull.Value : rate.Value;
                    return proc;
                }
            };

			await executor.ExecuteNonQueryAsync().ConfigureAwait(false);
		}
	}
	[Obsolete("Use nameof instead")]
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
		public const string EntityRowVersion = "EntityRowVersion";
		public const string CategoryNameLang1 = "CategoryNameLang1";
		public const string CategoryNameLang2 = "CategoryNameLang2";
		public const string SupplierName = "SupplierName";
		public const string CategoryName = "CategoryName";
	}

	[Serializable]
	[DataContract]
    [TypeScript] 
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
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="ShipperID", BaseColumnName ="ShipperID", BaseTableName = "Shippers" )]		public Int32 ShipperId 
		{ 
		    get { return _shipperId; } 
			set 
			{
			    _shipperId = value;
				NotifyPropertyChange("ShipperId");
			}
        }

		private String _companyName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Shippers" )]		public String CompanyName 
		{ 
		    get { return _companyName; } 
			set 
			{
			    _companyName = value;
				NotifyPropertyChange("CompanyName");
			}
        }

		private String _phone;
		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Shippers" )]		public String Phone 
		{ 
		    get { return _phone; } 
			set 
			{
			    _phone = value;
				NotifyPropertyChange("Phone");
			}
        }


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

		public bool Delete(System.Int32 shipperId)
		{
			var entity = new Shipper { ShipperId = shipperId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<Shipper> GetAsync(string projectionName, System.Int32 shipperId)
		{
			return ((IRepository<Shipper>)this).GetAsync(projectionName, shipperId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Shipper> GetAsync(string projectionName, System.Int32 shipperId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).GetAsync(projectionName, shipperId, fetchMode);
		}

		public System.Threading.Tasks.Task<Shipper> GetAsync(Projection projection, System.Int32 shipperId)
		{
			return ((IRepository<Shipper>)this).GetAsync(projection, shipperId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Shipper> GetAsync(Projection projection, System.Int32 shipperId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Shipper>)this).GetAsync(projection, shipperId, fetchMode);
		}

		public System.Threading.Tasks.Task<Shipper> GetAsync(string projectionName, System.Int32 shipperId, params string[] fields)
		{
			return ((IRepository<Shipper>)this).GetAsync(projectionName, shipperId, fields);
		}

		public System.Threading.Tasks.Task<Shipper> GetAsync(Projection projection, System.Int32 shipperId, params string[] fields)
		{
			return ((IRepository<Shipper>)this).GetAsync(projection, shipperId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 shipperId)
		{
			var entity = new Shipper { ShipperId = shipperId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class ShipperFields
	{
		public const string ShipperId = "ShipperId";
		public const string CompanyName = "CompanyName";
		public const string Phone = "Phone";
	}

	[Serializable]
	[DataContract]
    [TypeScript] 
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
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="SupplierID", BaseColumnName ="SupplierID", BaseTableName = "Suppliers" )]		public Int32 SupplierId 
		{ 
		    get { return _supplierId; } 
			set 
			{
			    _supplierId = value;
				NotifyPropertyChange("SupplierId");
			}
        }

		private String _companyName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="CompanyName", BaseColumnName ="CompanyName", BaseTableName = "Suppliers" )]		public String CompanyName 
		{ 
		    get { return _companyName; } 
			set 
			{
			    _companyName = value;
				NotifyPropertyChange("CompanyName");
			}
        }

		private String _contactName;
		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ContactName", BaseColumnName ="ContactName", BaseTableName = "Suppliers" )]		public String ContactName 
		{ 
		    get { return _contactName; } 
			set 
			{
			    _contactName = value;
				NotifyPropertyChange("ContactName");
			}
        }

		private String _contactTitle;
		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ContactTitle", BaseColumnName ="ContactTitle", BaseTableName = "Suppliers" )]		public String ContactTitle 
		{ 
		    get { return _contactTitle; } 
			set 
			{
			    _contactTitle = value;
				NotifyPropertyChange("ContactTitle");
			}
        }

		private String _address;
		[DataMember]
		[SqlField(DbType.String, 60, ColumnName ="Address", BaseColumnName ="Address", BaseTableName = "Suppliers" )]		public String Address 
		{ 
		    get { return _address; } 
			set 
			{
			    _address = value;
				NotifyPropertyChange("Address");
			}
        }

		private String _city;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="City", BaseColumnName ="City", BaseTableName = "Suppliers" )]		public String City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
				NotifyPropertyChange("City");
			}
        }

		private String _region;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Region", BaseColumnName ="Region", BaseTableName = "Suppliers" )]		public String Region 
		{ 
		    get { return _region; } 
			set 
			{
			    _region = value;
				NotifyPropertyChange("Region");
			}
        }

		private String _postalCode;
		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="PostalCode", BaseColumnName ="PostalCode", BaseTableName = "Suppliers" )]		public String PostalCode 
		{ 
		    get { return _postalCode; } 
			set 
			{
			    _postalCode = value;
				NotifyPropertyChange("PostalCode");
			}
        }

		private String _country;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="Country", BaseColumnName ="Country", BaseTableName = "Suppliers" )]		public String Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
				NotifyPropertyChange("Country");
			}
        }

		private String _phone;
		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Phone", BaseColumnName ="Phone", BaseTableName = "Suppliers" )]		public String Phone 
		{ 
		    get { return _phone; } 
			set 
			{
			    _phone = value;
				NotifyPropertyChange("Phone");
			}
        }

		private String _fax;
		[DataMember]
		[SqlField(DbType.String, 24, ColumnName ="Fax", BaseColumnName ="Fax", BaseTableName = "Suppliers" )]		public String Fax 
		{ 
		    get { return _fax; } 
			set 
			{
			    _fax = value;
				NotifyPropertyChange("Fax");
			}
        }

		private String _homePage;
		[DataMember]
		[SqlField(DbType.String, 1073741823, ColumnName ="HomePage", BaseColumnName ="HomePage", BaseTableName = "Suppliers" )]		public String HomePage 
		{ 
		    get { return _homePage; } 
			set 
			{
			    _homePage = value;
				NotifyPropertyChange("HomePage");
			}
        }


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

		public bool Delete(System.Int32 supplierId)
		{
			var entity = new Supplier { SupplierId = supplierId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<Supplier> GetAsync(string projectionName, System.Int32 supplierId)
		{
			return ((IRepository<Supplier>)this).GetAsync(projectionName, supplierId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Supplier> GetAsync(string projectionName, System.Int32 supplierId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).GetAsync(projectionName, supplierId, fetchMode);
		}

		public System.Threading.Tasks.Task<Supplier> GetAsync(Projection projection, System.Int32 supplierId)
		{
			return ((IRepository<Supplier>)this).GetAsync(projection, supplierId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Supplier> GetAsync(Projection projection, System.Int32 supplierId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Supplier>)this).GetAsync(projection, supplierId, fetchMode);
		}

		public System.Threading.Tasks.Task<Supplier> GetAsync(string projectionName, System.Int32 supplierId, params string[] fields)
		{
			return ((IRepository<Supplier>)this).GetAsync(projectionName, supplierId, fields);
		}

		public System.Threading.Tasks.Task<Supplier> GetAsync(Projection projection, System.Int32 supplierId, params string[] fields)
		{
			return ((IRepository<Supplier>)this).GetAsync(projection, supplierId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 supplierId)
		{
			var entity = new Supplier { SupplierId = supplierId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
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
    [TypeScript] 
	[SqlEntity(BaseTableName="my_table")]
	public partial class MyEntity : INotifyPropertyChanged
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
		
		private Int32 _entityId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, ColumnName ="entity_id", BaseColumnName ="entity_id", BaseTableName = "my_table" )]		public Int32 EntityId 
		{ 
		    get { return _entityId; } 
			set 
			{
			    _entityId = value;
				NotifyPropertyChange("EntityId");
			}
        }

		private String _value;
		[DataMember]
		[SqlField(DbType.String, 128, ColumnName ="value", BaseColumnName ="value", BaseTableName = "my_table" )]		public String Value 
		{ 
		    get { return _value; } 
			set 
			{
			    _value = value;
				NotifyPropertyChange("Value");
			}
        }


	}

	public partial class MyEntityRepository : Repository<MyEntity> 
	{
		public MyEntityRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public MyEntity Get(string projectionName, System.Int32 entityId)
		{
			return ((IRepository<MyEntity>)this).Get(projectionName, entityId, FetchMode.UseIdentityMap);
		}

		public MyEntity Get(string projectionName, System.Int32 entityId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<MyEntity>)this).Get(projectionName, entityId, fetchMode);
		}

		public MyEntity Get(Projection projection, System.Int32 entityId)
		{
			return ((IRepository<MyEntity>)this).Get(projection, entityId, FetchMode.UseIdentityMap);
		}

		public MyEntity Get(Projection projection, System.Int32 entityId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<MyEntity>)this).Get(projection, entityId, fetchMode);
		}

		public MyEntity Get(string projectionName, System.Int32 entityId, params string[] fields)
		{
			return ((IRepository<MyEntity>)this).Get(projectionName, entityId, fields);
		}

		public MyEntity Get(Projection projection, System.Int32 entityId, params string[] fields)
		{
			return ((IRepository<MyEntity>)this).Get(projection, entityId, fields);
		}

		public bool Delete(System.Int32 entityId)
		{
			var entity = new MyEntity { EntityId = entityId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<MyEntity> GetAsync(string projectionName, System.Int32 entityId)
		{
			return ((IRepository<MyEntity>)this).GetAsync(projectionName, entityId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<MyEntity> GetAsync(string projectionName, System.Int32 entityId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<MyEntity>)this).GetAsync(projectionName, entityId, fetchMode);
		}

		public System.Threading.Tasks.Task<MyEntity> GetAsync(Projection projection, System.Int32 entityId)
		{
			return ((IRepository<MyEntity>)this).GetAsync(projection, entityId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<MyEntity> GetAsync(Projection projection, System.Int32 entityId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<MyEntity>)this).GetAsync(projection, entityId, fetchMode);
		}

		public System.Threading.Tasks.Task<MyEntity> GetAsync(string projectionName, System.Int32 entityId, params string[] fields)
		{
			return ((IRepository<MyEntity>)this).GetAsync(projectionName, entityId, fields);
		}

		public System.Threading.Tasks.Task<MyEntity> GetAsync(Projection projection, System.Int32 entityId, params string[] fields)
		{
			return ((IRepository<MyEntity>)this).GetAsync(projection, entityId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 entityId)
		{
			var entity = new MyEntity { EntityId = entityId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class MyEntityFields
	{
		public const string EntityId = "EntityId";
		public const string Value = "Value";
	}

	[Serializable]
	[DataContract]
    [TypeScript] 
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
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="CategoryID" )]		public Int32? CategoryId 
		{ 
		    get { return _categoryId; } 
			set 
			{
			    _categoryId = value;
				NotifyPropertyChange("CategoryId");
			}
        }

		private String _categoryNameLang1;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryNameLang1" )]		public String CategoryNameLang1 
		{ 
		    get { return _categoryNameLang1; } 
			set 
			{
			    _categoryNameLang1 = value;
				NotifyPropertyChange("CategoryNameLang1");
			}
        }

		private String _categoryNameLang2;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CategoryNameLang2" )]		public String CategoryNameLang2 
		{ 
		    get { return _categoryNameLang2; } 
			set 
			{
			    _categoryNameLang2 = value;
				NotifyPropertyChange("CategoryNameLang2");
			}
        }

		private Int32 _productId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, ColumnName ="ProductID" )]		public Int32 ProductId 
		{ 
		    get { return _productId; } 
			set 
			{
			    _productId = value;
				NotifyPropertyChange("ProductId");
			}
        }

		private String _productName;
		[DataMember]
		[SqlField(DbType.String, 40, ColumnName ="ProductName" )]		public String ProductName 
		{ 
		    get { return _productName; } 
			set 
			{
			    _productName = value;
				NotifyPropertyChange("ProductName");
			}
        }

		private Int32? _year;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="Year" )]		public Int32? Year 
		{ 
		    get { return _year; } 
			set 
			{
			    _year = value;
				NotifyPropertyChange("Year");
			}
        }

		private Int32? _quarter;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="Quarter" )]		public Int32? Quarter 
		{ 
		    get { return _quarter; } 
			set 
			{
			    _quarter = value;
				NotifyPropertyChange("Quarter");
			}
        }

		private Decimal? _sales;
		[DataMember]
		[SqlField(DbType.Decimal, 17, Precision = 38, Scale=8, AllowNull = true, ColumnName ="Sales" )]		public Decimal? Sales 
		{ 
		    get { return _sales; } 
			set 
			{
			    _sales = value;
				NotifyPropertyChange("Sales");
			}
        }

		private Int32? _orderCount;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, AllowNull = true, ColumnName ="OrderCount" )]		public Int32? OrderCount 
		{ 
		    get { return _orderCount; } 
			set 
			{
			    _orderCount = value;
				NotifyPropertyChange("OrderCount");
			}
        }

		[LocalizedField]
		[DataMember]
		public string CategoryName 
		{ 
			get
			{
				return CurrentLanguageService.GetLocalizedValue(this, "CategoryName");
			} 
		}

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
	[Obsolete("Use nameof instead")]
	public static partial class ProductSaleFields
	{
		public const string CategoryId = "CategoryId";
		public const string CategoryNameLang1 = "CategoryNameLang1";
		public const string CategoryNameLang2 = "CategoryNameLang2";
		public const string ProductId = "ProductId";
		public const string ProductName = "ProductName";
		public const string Year = "Year";
		public const string Quarter = "Quarter";
		public const string Sales = "Sales";
		public const string OrderCount = "OrderCount";
		public const string CategoryName = "CategoryName";
	}

	[Serializable]
	[DataContract]
    [TypeScript] 
	[SqlEntity(BaseTableName="Items")]
	public partial class Item : INotifyPropertyChanged
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
		
		private Int32 _itemId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, IsAutoincrement=true, IsReadOnly = true, ColumnName ="ItemId", BaseColumnName ="ItemId", BaseTableName = "Items" )]		public Int32 ItemId 
		{ 
		    get { return _itemId; } 
			set 
			{
			    _itemId = value;
				NotifyPropertyChange("ItemId");
			}
        }

		private String _field1;
		[DataMember]
		[SqlField(DbType.String, 50, ColumnName ="Field1", BaseColumnName ="Field1", BaseTableName = "Items" )]		public String Field1 
		{ 
		    get { return _field1; } 
			set 
			{
			    _field1 = value;
				NotifyPropertyChange("Field1");
			}
        }

		private String _field2;
		[DataMember]
		[SqlField(DbType.String, 50, ColumnName ="Field2", BaseColumnName ="Field2", BaseTableName = "Items" )]		public String Field2 
		{ 
		    get { return _field2; } 
			set 
			{
			    _field2 = value;
				NotifyPropertyChange("Field2");
			}
        }

		private String _field3;
		[DataMember]
		[SqlField(DbType.String, 50, ColumnName ="Field3", BaseColumnName ="Field3", BaseTableName = "Items" )]		public String Field3 
		{ 
		    get { return _field3; } 
			set 
			{
			    _field3 = value;
				NotifyPropertyChange("Field3");
			}
        }

		private String _field4;
		[DataMember]
		[SqlField(DbType.String, 50, ColumnName ="Field4", BaseColumnName ="Field4", BaseTableName = "Items" )]		public String Field4 
		{ 
		    get { return _field4; } 
			set 
			{
			    _field4 = value;
				NotifyPropertyChange("Field4");
			}
        }

		private Int64 _dbChangeNumber;
		[DataMember]
		[SqlField(DbType.Int64, 8, Precision = 19, ColumnName ="DbChangeNumber", BaseColumnName ="DbChangeNumber", BaseTableName = "Items" )]		public Int64 DbChangeNumber 
		{ 
		    get { return _dbChangeNumber; } 
			set 
			{
			    _dbChangeNumber = value;
				NotifyPropertyChange("DbChangeNumber");
			}
        }


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

		public Item Get(string projectionName, System.Int32 itemId)
		{
			return ((IRepository<Item>)this).Get(projectionName, itemId, FetchMode.UseIdentityMap);
		}

		public Item Get(string projectionName, System.Int32 itemId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Item>)this).Get(projectionName, itemId, fetchMode);
		}

		public Item Get(Projection projection, System.Int32 itemId)
		{
			return ((IRepository<Item>)this).Get(projection, itemId, FetchMode.UseIdentityMap);
		}

		public Item Get(Projection projection, System.Int32 itemId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Item>)this).Get(projection, itemId, fetchMode);
		}

		public Item Get(string projectionName, System.Int32 itemId, params string[] fields)
		{
			return ((IRepository<Item>)this).Get(projectionName, itemId, fields);
		}

		public Item Get(Projection projection, System.Int32 itemId, params string[] fields)
		{
			return ((IRepository<Item>)this).Get(projection, itemId, fields);
		}

		public bool Delete(System.Int32 itemId)
		{
			var entity = new Item { ItemId = itemId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<Item> GetAsync(string projectionName, System.Int32 itemId)
		{
			return ((IRepository<Item>)this).GetAsync(projectionName, itemId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Item> GetAsync(string projectionName, System.Int32 itemId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Item>)this).GetAsync(projectionName, itemId, fetchMode);
		}

		public System.Threading.Tasks.Task<Item> GetAsync(Projection projection, System.Int32 itemId)
		{
			return ((IRepository<Item>)this).GetAsync(projection, itemId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<Item> GetAsync(Projection projection, System.Int32 itemId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Item>)this).GetAsync(projection, itemId, fetchMode);
		}

		public System.Threading.Tasks.Task<Item> GetAsync(string projectionName, System.Int32 itemId, params string[] fields)
		{
			return ((IRepository<Item>)this).GetAsync(projectionName, itemId, fields);
		}

		public System.Threading.Tasks.Task<Item> GetAsync(Projection projection, System.Int32 itemId, params string[] fields)
		{
			return ((IRepository<Item>)this).GetAsync(projection, itemId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 itemId)
		{
			var entity = new Item { ItemId = itemId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class ItemFields
	{
		public const string ItemId = "ItemId";
		public const string Field1 = "Field1";
		public const string Field2 = "Field2";
		public const string Field3 = "Field3";
		public const string Field4 = "Field4";
		public const string DbChangeNumber = "DbChangeNumber";
	}

	[Serializable]
	[DataContract]
    [TypeScript] 
	[SqlEntity(BaseTableName="Metadata")]
	public partial class MetadataItem : INotifyPropertyChanged
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
		
		private Int32 _metadataId;
		[DataMember]
		[SqlField(DbType.Int32, 4, Precision = 10, IsKey=true, ColumnName ="MetadataId", BaseColumnName ="MetadataId", BaseTableName = "Metadata" )]		public Int32 MetadataId 
		{ 
		    get { return _metadataId; } 
			set 
			{
			    _metadataId = value;
				NotifyPropertyChange("MetadataId");
			}
        }

		private Newtonsoft.Json.Linq.JToken _data;
		[DataMember]
		[SqlField(DbType.AnsiString, 8000, ColumnName ="DataJson", BaseColumnName ="DataJson", BaseTableName = "Metadata" )]		public Newtonsoft.Json.Linq.JToken Data 
		{ 
		    get { return _data; } 
			set 
			{
			    _data = value;
				NotifyPropertyChange("Data");
			}
        }


	}

	public partial class MetadataItemRepository : Repository<MetadataItem> 
	{
		public MetadataItemRepository(DataService DataService) : base(DataService)
		{
		}

		public new NorthwindDataService  DataService  
		{
			get { return (NorthwindDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public MetadataItem Get(string projectionName, System.Int32 metadataId)
		{
			return ((IRepository<MetadataItem>)this).Get(projectionName, metadataId, FetchMode.UseIdentityMap);
		}

		public MetadataItem Get(string projectionName, System.Int32 metadataId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<MetadataItem>)this).Get(projectionName, metadataId, fetchMode);
		}

		public MetadataItem Get(Projection projection, System.Int32 metadataId)
		{
			return ((IRepository<MetadataItem>)this).Get(projection, metadataId, FetchMode.UseIdentityMap);
		}

		public MetadataItem Get(Projection projection, System.Int32 metadataId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<MetadataItem>)this).Get(projection, metadataId, fetchMode);
		}

		public MetadataItem Get(string projectionName, System.Int32 metadataId, params string[] fields)
		{
			return ((IRepository<MetadataItem>)this).Get(projectionName, metadataId, fields);
		}

		public MetadataItem Get(Projection projection, System.Int32 metadataId, params string[] fields)
		{
			return ((IRepository<MetadataItem>)this).Get(projection, metadataId, fields);
		}

		public bool Delete(System.Int32 metadataId)
		{
			var entity = new MetadataItem { MetadataId = metadataId };
			return this.Delete(entity);
		}
		// asyncrhonous methods

		public System.Threading.Tasks.Task<MetadataItem> GetAsync(string projectionName, System.Int32 metadataId)
		{
			return ((IRepository<MetadataItem>)this).GetAsync(projectionName, metadataId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<MetadataItem> GetAsync(string projectionName, System.Int32 metadataId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<MetadataItem>)this).GetAsync(projectionName, metadataId, fetchMode);
		}

		public System.Threading.Tasks.Task<MetadataItem> GetAsync(Projection projection, System.Int32 metadataId)
		{
			return ((IRepository<MetadataItem>)this).GetAsync(projection, metadataId, FetchMode.UseIdentityMap);
		}

		public System.Threading.Tasks.Task<MetadataItem> GetAsync(Projection projection, System.Int32 metadataId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<MetadataItem>)this).GetAsync(projection, metadataId, fetchMode);
		}

		public System.Threading.Tasks.Task<MetadataItem> GetAsync(string projectionName, System.Int32 metadataId, params string[] fields)
		{
			return ((IRepository<MetadataItem>)this).GetAsync(projectionName, metadataId, fields);
		}

		public System.Threading.Tasks.Task<MetadataItem> GetAsync(Projection projection, System.Int32 metadataId, params string[] fields)
		{
			return ((IRepository<MetadataItem>)this).GetAsync(projection, metadataId, fields);
		}

		public System.Threading.Tasks.Task<bool> DeleteAsync(System.Int32 metadataId)
		{
			var entity = new MetadataItem { MetadataId = metadataId };
			return this.DeleteAsync(entity);
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class MetadataItemFields
	{
		public const string MetadataId = "MetadataId";
		public const string Data = "Data";
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

		private Samples.Entities.MyEntityRepository _MyEntityRepository;
		public Samples.Entities.MyEntityRepository MyEntityRepository
		{
			get 
			{
				if ( _MyEntityRepository == null)
				{
					_MyEntityRepository = new Samples.Entities.MyEntityRepository(this);
				}
				return _MyEntityRepository;
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

		private Samples.Entities.MetadataItemRepository _MetadataItemRepository;
		public Samples.Entities.MetadataItemRepository MetadataItemRepository
		{
			get 
			{
				if ( _MetadataItemRepository == null)
				{
					_MetadataItemRepository = new Samples.Entities.MetadataItemRepository(this);
				}
				return _MetadataItemRepository;
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
namespace Samples.Entities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public partial class TypeScriptAttribute : Attribute
    {
        public static IEnumerable<Type> GetClasses()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            foreach (var t in asm.GetTypes())
            {
                var attrs = t.GetCustomAttributes(typeof(TypeScriptAttribute), false);
                if (attrs != null && attrs.Length > 0) yield return t;
            }
        }
    }
}
