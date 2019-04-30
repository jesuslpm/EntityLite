
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.ComponentModel;
using inercya.EntityLite;	
using inercya.EntityLite.Extensions;	

namespace Samples.Entities
{
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="COUNTRY")]
	public partial class Country
	{
		private String _countryName;
		[DataMember]
		[SqlField(DbType.String, 15, IsKey=true, ColumnName ="COUNTRY", BaseColumnName ="COUNTRY", BaseTableName = "COUNTRY" )]		
		public String CountryName 
		{ 
		    get { return _countryName; } 
			set 
			{
			    _countryName = value;
			}
        }

		private String _currency;
		[DataMember]
		[SqlField(DbType.String, 10, ColumnName ="CURRENCY", BaseColumnName ="CURRENCY", BaseTableName = "COUNTRY" )]		
		public String Currency 
		{ 
		    get { return _currency; } 
			set 
			{
			    _currency = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"COUNTRY\", \"CURRENCY\"";

	}

	public partial class CountryRepository : Repository<Country> 
	{
		public CountryRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Country Get(string projectionName, String countryName)
		{
			return ((IRepository<Country>)this).Get(projectionName, countryName, FetchMode.UseIdentityMap);
		}

		public Country Get(string projectionName, String countryName, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Country>)this).Get(projectionName, countryName, fetchMode);
		}

		public Country Get(Projection projection, String countryName)
		{
			return ((IRepository<Country>)this).Get(projection, countryName, FetchMode.UseIdentityMap);
		}

		public Country Get(Projection projection, String countryName, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Country>)this).Get(projection, countryName, fetchMode);
		}

		public Country Get(string projectionName, String countryName, params string[] fields)
		{
			return ((IRepository<Country>)this).Get(projectionName, countryName, fields);
		}

		public Country Get(Projection projection, String countryName, params string[] fields)
		{
			return ((IRepository<Country>)this).Get(projection, countryName, fields);
		}

		public bool Delete(String countryName)
		{
			var entity = new Country { CountryName = countryName };
			return this.Delete(entity);
		}

			}
	[Obsolete("Use nameof instead")]
	public static partial class CountryFields
	{
		public const string CountryName = "CountryName";
		public const string Currency = "Currency";
	}

	public static partial class CountryProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="JOB")]
	public partial class Job
	{
		private String _jobCode;
		[DataMember]
		[SqlField(DbType.String, 5, IsKey=true, ColumnName ="JOB_CODE", BaseColumnName ="JOB_CODE", BaseTableName = "JOB" )]		
		public String JobCode 
		{ 
		    get { return _jobCode; } 
			set 
			{
			    _jobCode = value;
			}
        }

		private Int16 _jobGrade;
		[DataMember]
		[SqlField(DbType.Int16, 2, IsKey=true, ColumnName ="JOB_GRADE", BaseColumnName ="JOB_GRADE", BaseTableName = "JOB" )]		
		public Int16 JobGrade 
		{ 
		    get { return _jobGrade; } 
			set 
			{
			    _jobGrade = value;
			}
        }

		private String _jobCountry;
		[DataMember]
		[SqlField(DbType.String, 15, IsKey=true, ColumnName ="JOB_COUNTRY", BaseColumnName ="JOB_COUNTRY", BaseTableName = "JOB" )]		
		public String JobCountry 
		{ 
		    get { return _jobCountry; } 
			set 
			{
			    _jobCountry = value;
			}
        }

		private String _jobTitle;
		[DataMember]
		[SqlField(DbType.String, 25, ColumnName ="JOB_TITLE", BaseColumnName ="JOB_TITLE", BaseTableName = "JOB" )]		
		public String JobTitle 
		{ 
		    get { return _jobTitle; } 
			set 
			{
			    _jobTitle = value;
			}
        }

		private Decimal _minSalary;
		[DataMember]
		[SqlField(DbType.Decimal, 8, Precision = 10, Scale=2, ColumnName ="MIN_SALARY", BaseColumnName ="MIN_SALARY", BaseTableName = "JOB" )]		
		public Decimal MinSalary 
		{ 
		    get { return _minSalary; } 
			set 
			{
			    _minSalary = value;
			}
        }

		private Decimal _maxSalary;
		[DataMember]
		[SqlField(DbType.Decimal, 8, Precision = 10, Scale=2, ColumnName ="MAX_SALARY", BaseColumnName ="MAX_SALARY", BaseTableName = "JOB" )]		
		public Decimal MaxSalary 
		{ 
		    get { return _maxSalary; } 
			set 
			{
			    _maxSalary = value;
			}
        }

		private String _jobRequirement;
		[DataMember]
		[SqlField(DbType.String, 2147483647, ColumnName ="JOB_REQUIREMENT", BaseColumnName ="JOB_REQUIREMENT", BaseTableName = "JOB" )]		
		public String JobRequirement 
		{ 
		    get { return _jobRequirement; } 
			set 
			{
			    _jobRequirement = value;
			}
        }

		private Array _languageReq;
		[DataMember]
		[SqlField(DbType.Binary, 8, ColumnName ="LANGUAGE_REQ", BaseColumnName ="LANGUAGE_REQ", BaseTableName = "JOB" )]		
		public Array LanguageReq 
		{ 
		    get { return _languageReq; } 
			set 
			{
			    _languageReq = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"JOB_CODE\", \"JOB_GRADE\", \"JOB_COUNTRY\", \"JOB_TITLE\", \"MIN_SALARY\", \"MAX_SALARY\", \"JOB_REQUIREMENT\", \"LANGUAGE_REQ\"";

	}

	public partial class JobRepository : Repository<Job> 
	{
		public JobRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class JobFields
	{
		public const string JobCode = "JobCode";
		public const string JobGrade = "JobGrade";
		public const string JobCountry = "JobCountry";
		public const string JobTitle = "JobTitle";
		public const string MinSalary = "MinSalary";
		public const string MaxSalary = "MaxSalary";
		public const string JobRequirement = "JobRequirement";
		public const string LanguageReq = "LanguageReq";
	}

	public static partial class JobProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="DEPARTMENT")]
	public partial class Department
	{
		private String _deptNo;
		[DataMember]
		[SqlField(DbType.String, 3, IsKey=true, ColumnName ="DEPT_NO", BaseColumnName ="DEPT_NO", BaseTableName = "DEPARTMENT" )]		
		public String DeptNo 
		{ 
		    get { return _deptNo; } 
			set 
			{
			    _deptNo = value;
			}
        }

		private String _departmentName;
		[DataMember]
		[SqlField(DbType.String, 25, ColumnName ="DEPARTMENT", BaseColumnName ="DEPARTMENT", BaseTableName = "DEPARTMENT" )]		
		public String DepartmentName 
		{ 
		    get { return _departmentName; } 
			set 
			{
			    _departmentName = value;
			}
        }

		private String _headDept;
		[DataMember]
		[SqlField(DbType.String, 3, ColumnName ="HEAD_DEPT", BaseColumnName ="HEAD_DEPT", BaseTableName = "DEPARTMENT" )]		
		public String HeadDept 
		{ 
		    get { return _headDept; } 
			set 
			{
			    _headDept = value;
			}
        }

		private Int16? _mngrNo;
		[DataMember]
		[SqlField(DbType.Int16, 2, AllowNull = true, ColumnName ="MNGR_NO", BaseColumnName ="MNGR_NO", BaseTableName = "DEPARTMENT" )]		
		public Int16? MngrNo 
		{ 
		    get { return _mngrNo; } 
			set 
			{
			    _mngrNo = value;
			}
        }

		private Decimal? _budget;
		[DataMember]
		[SqlField(DbType.Decimal, 8, Precision = 12, Scale=2, AllowNull = true, ColumnName ="BUDGET", BaseColumnName ="BUDGET", BaseTableName = "DEPARTMENT" )]		
		public Decimal? Budget 
		{ 
		    get { return _budget; } 
			set 
			{
			    _budget = value;
			}
        }

		private String _location;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="LOCATION", BaseColumnName ="LOCATION", BaseTableName = "DEPARTMENT" )]		
		public String Location 
		{ 
		    get { return _location; } 
			set 
			{
			    _location = value;
			}
        }

		private String _phoneNo;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="PHONE_NO", BaseColumnName ="PHONE_NO", BaseTableName = "DEPARTMENT" )]		
		public String PhoneNo 
		{ 
		    get { return _phoneNo; } 
			set 
			{
			    _phoneNo = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"DEPT_NO\", \"DEPARTMENT\", \"HEAD_DEPT\", \"MNGR_NO\", \"BUDGET\", \"LOCATION\", \"PHONE_NO\"";

	}

	public partial class DepartmentRepository : Repository<Department> 
	{
		public DepartmentRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Department Get(string projectionName, String deptNo)
		{
			return ((IRepository<Department>)this).Get(projectionName, deptNo, FetchMode.UseIdentityMap);
		}

		public Department Get(string projectionName, String deptNo, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Department>)this).Get(projectionName, deptNo, fetchMode);
		}

		public Department Get(Projection projection, String deptNo)
		{
			return ((IRepository<Department>)this).Get(projection, deptNo, FetchMode.UseIdentityMap);
		}

		public Department Get(Projection projection, String deptNo, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Department>)this).Get(projection, deptNo, fetchMode);
		}

		public Department Get(string projectionName, String deptNo, params string[] fields)
		{
			return ((IRepository<Department>)this).Get(projectionName, deptNo, fields);
		}

		public Department Get(Projection projection, String deptNo, params string[] fields)
		{
			return ((IRepository<Department>)this).Get(projection, deptNo, fields);
		}

		public bool Delete(String deptNo)
		{
			var entity = new Department { DeptNo = deptNo };
			return this.Delete(entity);
		}

		
		public IList<Department> OrgChart()
		{
            var executor = new StoredProcedureExecutor(this.DataService, true)
            {
                GetCommandFunc = () =>
                {
                    var proc =  Samples.Entities.StoredProcedures.CreateOrgChartProcedure(this.DataService.Connection, this.DataService.EntityLiteProvider.ParameterPrefix, this.DataService.EntityLiteProvider.DefaultSchema);
                    return proc;
                }
            };

			
			var result = executor.ToList<Department>();	
			return result;
		}
	}
	[Obsolete("Use nameof instead")]
	public static partial class DepartmentFields
	{
		public const string DeptNo = "DeptNo";
		public const string DepartmentName = "DepartmentName";
		public const string HeadDept = "HeadDept";
		public const string MngrNo = "MngrNo";
		public const string Budget = "Budget";
		public const string Location = "Location";
		public const string PhoneNo = "PhoneNo";
	}

	public static partial class DepartmentProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="EMPLOYEE")]
	public partial class Employee
	{
		private Int16 _empNo;
		[DataMember]
		[SqlField(DbType.Int16, 2, IsKey=true, SequenceName = "EMP_NO_GEN", ColumnName ="EMP_NO", BaseColumnName ="EMP_NO", BaseTableName = "EMPLOYEE" )]		
		public Int16 EmpNo 
		{ 
		    get { return _empNo; } 
			set 
			{
			    _empNo = value;
			}
        }

		private String _firstName;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="FIRST_NAME", BaseColumnName ="FIRST_NAME", BaseTableName = "EMPLOYEE" )]		
		public String FirstName 
		{ 
		    get { return _firstName; } 
			set 
			{
			    _firstName = value;
			}
        }

		private String _lastName;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="LAST_NAME", BaseColumnName ="LAST_NAME", BaseTableName = "EMPLOYEE" )]		
		public String LastName 
		{ 
		    get { return _lastName; } 
			set 
			{
			    _lastName = value;
			}
        }

		private String _phoneExt;
		[DataMember]
		[SqlField(DbType.String, 4, ColumnName ="PHONE_EXT", BaseColumnName ="PHONE_EXT", BaseTableName = "EMPLOYEE" )]		
		public String PhoneExt 
		{ 
		    get { return _phoneExt; } 
			set 
			{
			    _phoneExt = value;
			}
        }

		private DateTime _hireDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, ColumnName ="HIRE_DATE", BaseColumnName ="HIRE_DATE", BaseTableName = "EMPLOYEE" )]		
		public DateTime HireDate 
		{ 
		    get { return _hireDate; } 
			set 
			{
			    _hireDate = value;
			}
        }

		private String _deptNo;
		[DataMember]
		[SqlField(DbType.String, 3, ColumnName ="DEPT_NO", BaseColumnName ="DEPT_NO", BaseTableName = "EMPLOYEE" )]		
		public String DeptNo 
		{ 
		    get { return _deptNo; } 
			set 
			{
			    _deptNo = value;
			}
        }

		private String _jobCode;
		[DataMember]
		[SqlField(DbType.String, 5, ColumnName ="JOB_CODE", BaseColumnName ="JOB_CODE", BaseTableName = "EMPLOYEE" )]		
		public String JobCode 
		{ 
		    get { return _jobCode; } 
			set 
			{
			    _jobCode = value;
			}
        }

		private Int16 _jobGrade;
		[DataMember]
		[SqlField(DbType.Int16, 2, ColumnName ="JOB_GRADE", BaseColumnName ="JOB_GRADE", BaseTableName = "EMPLOYEE" )]		
		public Int16 JobGrade 
		{ 
		    get { return _jobGrade; } 
			set 
			{
			    _jobGrade = value;
			}
        }

		private String _jobCountry;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="JOB_COUNTRY", BaseColumnName ="JOB_COUNTRY", BaseTableName = "EMPLOYEE" )]		
		public String JobCountry 
		{ 
		    get { return _jobCountry; } 
			set 
			{
			    _jobCountry = value;
			}
        }

		private Decimal _salary;
		[DataMember]
		[SqlField(DbType.Decimal, 8, Precision = 10, Scale=2, ColumnName ="SALARY", BaseColumnName ="SALARY", BaseTableName = "EMPLOYEE" )]		
		public Decimal Salary 
		{ 
		    get { return _salary; } 
			set 
			{
			    _salary = value;
			}
        }

		private String _fullName;
		[DataMember]
		[SqlField(DbType.String, 37, IsReadOnly = true, ColumnName ="FULL_NAME", BaseColumnName ="FULL_NAME", BaseTableName = "EMPLOYEE" )]		
		public String FullName 
		{ 
		    get { return _fullName; } 
			set 
			{
			    _fullName = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"EMP_NO\", \"FIRST_NAME\", \"LAST_NAME\", \"PHONE_EXT\", \"HIRE_DATE\", \"DEPT_NO\", \"JOB_CODE\", \"JOB_GRADE\", \"JOB_COUNTRY\", \"SALARY\", \"FULL_NAME\"";

	}

	public partial class EmployeeRepository : Repository<Employee> 
	{
		public EmployeeRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Employee Get(string projectionName, Int16 empNo)
		{
			return ((IRepository<Employee>)this).Get(projectionName, empNo, FetchMode.UseIdentityMap);
		}

		public Employee Get(string projectionName, Int16 empNo, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projectionName, empNo, fetchMode);
		}

		public Employee Get(Projection projection, Int16 empNo)
		{
			return ((IRepository<Employee>)this).Get(projection, empNo, FetchMode.UseIdentityMap);
		}

		public Employee Get(Projection projection, Int16 empNo, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Employee>)this).Get(projection, empNo, fetchMode);
		}

		public Employee Get(string projectionName, Int16 empNo, params string[] fields)
		{
			return ((IRepository<Employee>)this).Get(projectionName, empNo, fields);
		}

		public Employee Get(Projection projection, Int16 empNo, params string[] fields)
		{
			return ((IRepository<Employee>)this).Get(projection, empNo, fields);
		}

		public bool Delete(Int16 empNo)
		{
			var entity = new Employee { EmpNo = empNo };
			return this.Delete(entity);
		}

			}
	[Obsolete("Use nameof instead")]
	public static partial class EmployeeFields
	{
		public const string EmpNo = "EmpNo";
		public const string FirstName = "FirstName";
		public const string LastName = "LastName";
		public const string PhoneExt = "PhoneExt";
		public const string HireDate = "HireDate";
		public const string DeptNo = "DeptNo";
		public const string JobCode = "JobCode";
		public const string JobGrade = "JobGrade";
		public const string JobCountry = "JobCountry";
		public const string Salary = "Salary";
		public const string FullName = "FullName";
	}

	public static partial class EmployeeProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="SALES")]
	public partial class Sale
	{
		private String _poNumber;
		[DataMember]
		[SqlField(DbType.String, 8, IsKey=true, ColumnName ="PO_NUMBER", BaseColumnName ="PO_NUMBER", BaseTableName = "SALES" )]		
		public String PoNumber 
		{ 
		    get { return _poNumber; } 
			set 
			{
			    _poNumber = value;
			}
        }

		private Int32 _custNo;
		[DataMember]
		[SqlField(DbType.Int32, 4, ColumnName ="CUST_NO", BaseColumnName ="CUST_NO", BaseTableName = "SALES" )]		
		public Int32 CustNo 
		{ 
		    get { return _custNo; } 
			set 
			{
			    _custNo = value;
			}
        }

		private Int16? _salesRep;
		[DataMember]
		[SqlField(DbType.Int16, 2, AllowNull = true, ColumnName ="SALES_REP", BaseColumnName ="SALES_REP", BaseTableName = "SALES" )]		
		public Int16? SalesRep 
		{ 
		    get { return _salesRep; } 
			set 
			{
			    _salesRep = value;
			}
        }

		private String _orderStatus;
		[DataMember]
		[SqlField(DbType.String, 7, ColumnName ="ORDER_STATUS", BaseColumnName ="ORDER_STATUS", BaseTableName = "SALES" )]		
		public String OrderStatus 
		{ 
		    get { return _orderStatus; } 
			set 
			{
			    _orderStatus = value;
			}
        }

		private DateTime _orderDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, ColumnName ="ORDER_DATE", BaseColumnName ="ORDER_DATE", BaseTableName = "SALES" )]		
		public DateTime OrderDate 
		{ 
		    get { return _orderDate; } 
			set 
			{
			    _orderDate = value;
			}
        }

		private DateTime? _shipDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, AllowNull = true, ColumnName ="SHIP_DATE", BaseColumnName ="SHIP_DATE", BaseTableName = "SALES" )]		
		public DateTime? ShipDate 
		{ 
		    get { return _shipDate; } 
			set 
			{
			    _shipDate = value;
			}
        }

		private DateTime? _dateNeeded;
		[DataMember]
		[SqlField(DbType.DateTime, 8, AllowNull = true, ColumnName ="DATE_NEEDED", BaseColumnName ="DATE_NEEDED", BaseTableName = "SALES" )]		
		public DateTime? DateNeeded 
		{ 
		    get { return _dateNeeded; } 
			set 
			{
			    _dateNeeded = value;
			}
        }

		private String _paid;
		[DataMember]
		[SqlField(DbType.String, 1, ColumnName ="PAID", BaseColumnName ="PAID", BaseTableName = "SALES" )]		
		public String Paid 
		{ 
		    get { return _paid; } 
			set 
			{
			    _paid = value;
			}
        }

		private Int32 _qtyOrdered;
		[DataMember]
		[SqlField(DbType.Int32, 4, ColumnName ="QTY_ORDERED", BaseColumnName ="QTY_ORDERED", BaseTableName = "SALES" )]		
		public Int32 QtyOrdered 
		{ 
		    get { return _qtyOrdered; } 
			set 
			{
			    _qtyOrdered = value;
			}
        }

		private Decimal _totalValue;
		[DataMember]
		[SqlField(DbType.Decimal, 4, Precision = 9, Scale=2, ColumnName ="TOTAL_VALUE", BaseColumnName ="TOTAL_VALUE", BaseTableName = "SALES" )]		
		public Decimal TotalValue 
		{ 
		    get { return _totalValue; } 
			set 
			{
			    _totalValue = value;
			}
        }

		private Single _discount;
		[DataMember]
		[SqlField(DbType.Single, 4, ColumnName ="DISCOUNT", BaseColumnName ="DISCOUNT", BaseTableName = "SALES" )]		
		public Single Discount 
		{ 
		    get { return _discount; } 
			set 
			{
			    _discount = value;
			}
        }

		private String _itemType;
		[DataMember]
		[SqlField(DbType.String, 12, ColumnName ="ITEM_TYPE", BaseColumnName ="ITEM_TYPE", BaseTableName = "SALES" )]		
		public String ItemType 
		{ 
		    get { return _itemType; } 
			set 
			{
			    _itemType = value;
			}
        }

		private Decimal? _aged;
		[DataMember]
		[SqlField(DbType.Decimal, 8, Precision = 8, Scale=9, AllowNull = true, IsReadOnly = true, ColumnName ="AGED", BaseColumnName ="AGED", BaseTableName = "SALES" )]		
		public Decimal? Aged 
		{ 
		    get { return _aged; } 
			set 
			{
			    _aged = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"PO_NUMBER\", \"CUST_NO\", \"SALES_REP\", \"ORDER_STATUS\", \"ORDER_DATE\", \"SHIP_DATE\", \"DATE_NEEDED\", \"PAID\", \"QTY_ORDERED\", \"TOTAL_VALUE\", \"DISCOUNT\", \"ITEM_TYPE\", \"AGED\"";

	}

	public partial class SaleRepository : Repository<Sale> 
	{
		public SaleRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Sale Get(string projectionName, String poNumber)
		{
			return ((IRepository<Sale>)this).Get(projectionName, poNumber, FetchMode.UseIdentityMap);
		}

		public Sale Get(string projectionName, String poNumber, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Sale>)this).Get(projectionName, poNumber, fetchMode);
		}

		public Sale Get(Projection projection, String poNumber)
		{
			return ((IRepository<Sale>)this).Get(projection, poNumber, FetchMode.UseIdentityMap);
		}

		public Sale Get(Projection projection, String poNumber, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Sale>)this).Get(projection, poNumber, fetchMode);
		}

		public Sale Get(string projectionName, String poNumber, params string[] fields)
		{
			return ((IRepository<Sale>)this).Get(projectionName, poNumber, fields);
		}

		public Sale Get(Projection projection, String poNumber, params string[] fields)
		{
			return ((IRepository<Sale>)this).Get(projection, poNumber, fields);
		}

		public bool Delete(String poNumber)
		{
			var entity = new Sale { PoNumber = poNumber };
			return this.Delete(entity);
		}

			}
	[Obsolete("Use nameof instead")]
	public static partial class SaleFields
	{
		public const string PoNumber = "PoNumber";
		public const string CustNo = "CustNo";
		public const string SalesRep = "SalesRep";
		public const string OrderStatus = "OrderStatus";
		public const string OrderDate = "OrderDate";
		public const string ShipDate = "ShipDate";
		public const string DateNeeded = "DateNeeded";
		public const string Paid = "Paid";
		public const string QtyOrdered = "QtyOrdered";
		public const string TotalValue = "TotalValue";
		public const string Discount = "Discount";
		public const string ItemType = "ItemType";
		public const string Aged = "Aged";
	}

	public static partial class SaleProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity()]
	public partial class Phone
	{
		private Int16? _empNo;
		[DataMember]
		[SqlField(DbType.Int16, 2, AllowNull = true, ColumnName ="EMP_NO" )]		
		public Int16? EmpNo 
		{ 
		    get { return _empNo; } 
			set 
			{
			    _empNo = value;
			}
        }

		private String _firstName;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="FIRST_NAME" )]		
		public String FirstName 
		{ 
		    get { return _firstName; } 
			set 
			{
			    _firstName = value;
			}
        }

		private String _lastName;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="LAST_NAME" )]		
		public String LastName 
		{ 
		    get { return _lastName; } 
			set 
			{
			    _lastName = value;
			}
        }

		private String _phoneExt;
		[DataMember]
		[SqlField(DbType.String, 4, ColumnName ="PHONE_EXT" )]		
		public String PhoneExt 
		{ 
		    get { return _phoneExt; } 
			set 
			{
			    _phoneExt = value;
			}
        }

		private String _location;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="LOCATION" )]		
		public String Location 
		{ 
		    get { return _location; } 
			set 
			{
			    _location = value;
			}
        }

		private String _phoneNo;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="PHONE_NO" )]		
		public String PhoneNo 
		{ 
		    get { return _phoneNo; } 
			set 
			{
			    _phoneNo = value;
			}
        }

		public const string ListProjectionColumnList = "\"EMP_NO\", \"FIRST_NAME\", \"LAST_NAME\", \"PHONE_EXT\", \"LOCATION\", \"PHONE_NO\"";

	}

	public partial class PhoneRepository : Repository<Phone> 
	{
		public PhoneRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class PhoneFields
	{
		public const string EmpNo = "EmpNo";
		public const string FirstName = "FirstName";
		public const string LastName = "LastName";
		public const string PhoneExt = "PhoneExt";
		public const string Location = "Location";
		public const string PhoneNo = "PhoneNo";
	}

	public static partial class PhoneProjections
	{
		public const string List = "List";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="PROJECT")]
	public partial class Project
	{
		private String _projId;
		[DataMember]
		[SqlField(DbType.String, 5, IsKey=true, ColumnName ="PROJ_ID", BaseColumnName ="PROJ_ID", BaseTableName = "PROJECT" )]		
		public String ProjId 
		{ 
		    get { return _projId; } 
			set 
			{
			    _projId = value;
			}
        }

		private String _projName;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="PROJ_NAME", BaseColumnName ="PROJ_NAME", BaseTableName = "PROJECT" )]		
		public String ProjName 
		{ 
		    get { return _projName; } 
			set 
			{
			    _projName = value;
			}
        }

		private String _projDesc;
		[DataMember]
		[SqlField(DbType.String, 2147483647, ColumnName ="PROJ_DESC", BaseColumnName ="PROJ_DESC", BaseTableName = "PROJECT" )]		
		public String ProjDesc 
		{ 
		    get { return _projDesc; } 
			set 
			{
			    _projDesc = value;
			}
        }

		private Int16? _teamLeader;
		[DataMember]
		[SqlField(DbType.Int16, 2, AllowNull = true, ColumnName ="TEAM_LEADER", BaseColumnName ="TEAM_LEADER", BaseTableName = "PROJECT" )]		
		public Int16? TeamLeader 
		{ 
		    get { return _teamLeader; } 
			set 
			{
			    _teamLeader = value;
			}
        }

		private String _product;
		[DataMember]
		[SqlField(DbType.String, 12, ColumnName ="PRODUCT", BaseColumnName ="PRODUCT", BaseTableName = "PROJECT" )]		
		public String Product 
		{ 
		    get { return _product; } 
			set 
			{
			    _product = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"PROJ_ID\", \"PROJ_NAME\", \"PROJ_DESC\", \"TEAM_LEADER\", \"PRODUCT\"";

	}

	public partial class ProjectRepository : Repository<Project> 
	{
		public ProjectRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Project Get(string projectionName, String projId)
		{
			return ((IRepository<Project>)this).Get(projectionName, projId, FetchMode.UseIdentityMap);
		}

		public Project Get(string projectionName, String projId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Project>)this).Get(projectionName, projId, fetchMode);
		}

		public Project Get(Projection projection, String projId)
		{
			return ((IRepository<Project>)this).Get(projection, projId, FetchMode.UseIdentityMap);
		}

		public Project Get(Projection projection, String projId, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Project>)this).Get(projection, projId, fetchMode);
		}

		public Project Get(string projectionName, String projId, params string[] fields)
		{
			return ((IRepository<Project>)this).Get(projectionName, projId, fields);
		}

		public Project Get(Projection projection, String projId, params string[] fields)
		{
			return ((IRepository<Project>)this).Get(projection, projId, fields);
		}

		public bool Delete(String projId)
		{
			var entity = new Project { ProjId = projId };
			return this.Delete(entity);
		}

			}
	[Obsolete("Use nameof instead")]
	public static partial class ProjectFields
	{
		public const string ProjId = "ProjId";
		public const string ProjName = "ProjName";
		public const string ProjDesc = "ProjDesc";
		public const string TeamLeader = "TeamLeader";
		public const string Product = "Product";
	}

	public static partial class ProjectProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="EMPLOYEE_PROJECT")]
	public partial class EmployeeProject
	{
		private Int16 _empNo;
		[DataMember]
		[SqlField(DbType.Int16, 2, IsKey=true, ColumnName ="EMP_NO", BaseColumnName ="EMP_NO", BaseTableName = "EMPLOYEE_PROJECT" )]		
		public Int16 EmpNo 
		{ 
		    get { return _empNo; } 
			set 
			{
			    _empNo = value;
			}
        }

		private String _projId;
		[DataMember]
		[SqlField(DbType.String, 5, IsKey=true, ColumnName ="PROJ_ID", BaseColumnName ="PROJ_ID", BaseTableName = "EMPLOYEE_PROJECT" )]		
		public String ProjId 
		{ 
		    get { return _projId; } 
			set 
			{
			    _projId = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"EMP_NO\", \"PROJ_ID\"";

	}

	public partial class EmployeeProjectRepository : Repository<EmployeeProject> 
	{
		public EmployeeProjectRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class EmployeeProjectFields
	{
		public const string EmpNo = "EmpNo";
		public const string ProjId = "ProjId";
	}

	public static partial class EmployeeProjectProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="PROJ_DEPT_BUDGET")]
	public partial class ProjDeptBudget
	{
		private Int32 _fiscalYear;
		[DataMember]
		[SqlField(DbType.Int32, 4, IsKey=true, ColumnName ="FISCAL_YEAR", BaseColumnName ="FISCAL_YEAR", BaseTableName = "PROJ_DEPT_BUDGET" )]		
		public Int32 FiscalYear 
		{ 
		    get { return _fiscalYear; } 
			set 
			{
			    _fiscalYear = value;
			}
        }

		private String _projId;
		[DataMember]
		[SqlField(DbType.String, 5, IsKey=true, ColumnName ="PROJ_ID", BaseColumnName ="PROJ_ID", BaseTableName = "PROJ_DEPT_BUDGET" )]		
		public String ProjId 
		{ 
		    get { return _projId; } 
			set 
			{
			    _projId = value;
			}
        }

		private String _deptNo;
		[DataMember]
		[SqlField(DbType.String, 3, IsKey=true, ColumnName ="DEPT_NO", BaseColumnName ="DEPT_NO", BaseTableName = "PROJ_DEPT_BUDGET" )]		
		public String DeptNo 
		{ 
		    get { return _deptNo; } 
			set 
			{
			    _deptNo = value;
			}
        }

		private Array _quartHeadCnt;
		[DataMember]
		[SqlField(DbType.Binary, 8, ColumnName ="QUART_HEAD_CNT", BaseColumnName ="QUART_HEAD_CNT", BaseTableName = "PROJ_DEPT_BUDGET" )]		
		public Array QuartHeadCnt 
		{ 
		    get { return _quartHeadCnt; } 
			set 
			{
			    _quartHeadCnt = value;
			}
        }

		private Decimal? _projectedBudget;
		[DataMember]
		[SqlField(DbType.Decimal, 8, Precision = 12, Scale=2, AllowNull = true, ColumnName ="PROJECTED_BUDGET", BaseColumnName ="PROJECTED_BUDGET", BaseTableName = "PROJ_DEPT_BUDGET" )]		
		public Decimal? ProjectedBudget 
		{ 
		    get { return _projectedBudget; } 
			set 
			{
			    _projectedBudget = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"FISCAL_YEAR\", \"PROJ_ID\", \"DEPT_NO\", \"QUART_HEAD_CNT\", \"PROJECTED_BUDGET\"";

	}

	public partial class ProjDeptBudgetRepository : Repository<ProjDeptBudget> 
	{
		public ProjDeptBudgetRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class ProjDeptBudgetFields
	{
		public const string FiscalYear = "FiscalYear";
		public const string ProjId = "ProjId";
		public const string DeptNo = "DeptNo";
		public const string QuartHeadCnt = "QuartHeadCnt";
		public const string ProjectedBudget = "ProjectedBudget";
	}

	public static partial class ProjDeptBudgetProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="SALARY_HISTORY")]
	public partial class SalaryHistoryItem
	{
		private Int16 _empNo;
		[DataMember]
		[SqlField(DbType.Int16, 2, IsKey=true, ColumnName ="EMP_NO", BaseColumnName ="EMP_NO", BaseTableName = "SALARY_HISTORY" )]		
		public Int16 EmpNo 
		{ 
		    get { return _empNo; } 
			set 
			{
			    _empNo = value;
			}
        }

		private DateTime _changeDate;
		[DataMember]
		[SqlField(DbType.DateTime, 8, IsKey=true, ColumnName ="CHANGE_DATE", BaseColumnName ="CHANGE_DATE", BaseTableName = "SALARY_HISTORY" )]		
		public DateTime ChangeDate 
		{ 
		    get { return _changeDate; } 
			set 
			{
			    _changeDate = value;
			}
        }

		private String _updaterId;
		[DataMember]
		[SqlField(DbType.String, 20, IsKey=true, ColumnName ="UPDATER_ID", BaseColumnName ="UPDATER_ID", BaseTableName = "SALARY_HISTORY" )]		
		public String UpdaterId 
		{ 
		    get { return _updaterId; } 
			set 
			{
			    _updaterId = value;
			}
        }

		private Decimal _oldSalary;
		[DataMember]
		[SqlField(DbType.Decimal, 8, Precision = 10, Scale=2, ColumnName ="OLD_SALARY", BaseColumnName ="OLD_SALARY", BaseTableName = "SALARY_HISTORY" )]		
		public Decimal OldSalary 
		{ 
		    get { return _oldSalary; } 
			set 
			{
			    _oldSalary = value;
			}
        }

		private Double _percentChange;
		[DataMember]
		[SqlField(DbType.Double, 8, ColumnName ="PERCENT_CHANGE", BaseColumnName ="PERCENT_CHANGE", BaseTableName = "SALARY_HISTORY" )]		
		public Double PercentChange 
		{ 
		    get { return _percentChange; } 
			set 
			{
			    _percentChange = value;
			}
        }

		private Double? _newSalary;
		[DataMember]
		[SqlField(DbType.Double, 8, AllowNull = true, IsReadOnly = true, ColumnName ="NEW_SALARY", BaseColumnName ="NEW_SALARY", BaseTableName = "SALARY_HISTORY" )]		
		public Double? NewSalary 
		{ 
		    get { return _newSalary; } 
			set 
			{
			    _newSalary = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"EMP_NO\", \"CHANGE_DATE\", \"UPDATER_ID\", \"OLD_SALARY\", \"PERCENT_CHANGE\", \"NEW_SALARY\"";

	}

	public partial class SalaryHistoryItemRepository : Repository<SalaryHistoryItem> 
	{
		public SalaryHistoryItemRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

	}
	[Obsolete("Use nameof instead")]
	public static partial class SalaryHistoryItemFields
	{
		public const string EmpNo = "EmpNo";
		public const string ChangeDate = "ChangeDate";
		public const string UpdaterId = "UpdaterId";
		public const string OldSalary = "OldSalary";
		public const string PercentChange = "PercentChange";
		public const string NewSalary = "NewSalary";
	}

	public static partial class SalaryHistoryItemProjections
	{
		public const string BaseTable = "BaseTable";
	}
	[Serializable]
	[DataContract]
	[SqlEntity(BaseTableName="CUSTOMER")]
	public partial class Customer
	{
		private Int32 _custNo;
		[DataMember]
		[SqlField(DbType.Int32, 4, IsKey=true, SequenceName = "CUST_NO_GEN", ColumnName ="CUST_NO", BaseColumnName ="CUST_NO", BaseTableName = "CUSTOMER" )]		
		public Int32 CustNo 
		{ 
		    get { return _custNo; } 
			set 
			{
			    _custNo = value;
			}
        }

		private String _customerName;
		[DataMember]
		[SqlField(DbType.String, 25, ColumnName ="CUSTOMER", BaseColumnName ="CUSTOMER", BaseTableName = "CUSTOMER" )]		
		public String CustomerName 
		{ 
		    get { return _customerName; } 
			set 
			{
			    _customerName = value;
			}
        }

		private String _contactFirst;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="CONTACT_FIRST", BaseColumnName ="CONTACT_FIRST", BaseTableName = "CUSTOMER" )]		
		public String ContactFirst 
		{ 
		    get { return _contactFirst; } 
			set 
			{
			    _contactFirst = value;
			}
        }

		private String _contactLast;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="CONTACT_LAST", BaseColumnName ="CONTACT_LAST", BaseTableName = "CUSTOMER" )]		
		public String ContactLast 
		{ 
		    get { return _contactLast; } 
			set 
			{
			    _contactLast = value;
			}
        }

		private String _phoneNo;
		[DataMember]
		[SqlField(DbType.String, 20, ColumnName ="PHONE_NO", BaseColumnName ="PHONE_NO", BaseTableName = "CUSTOMER" )]		
		public String PhoneNo 
		{ 
		    get { return _phoneNo; } 
			set 
			{
			    _phoneNo = value;
			}
        }

		private String _addressLine1;
		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ADDRESS_LINE1", BaseColumnName ="ADDRESS_LINE1", BaseTableName = "CUSTOMER" )]		
		public String AddressLine1 
		{ 
		    get { return _addressLine1; } 
			set 
			{
			    _addressLine1 = value;
			}
        }

		private String _addressLine2;
		[DataMember]
		[SqlField(DbType.String, 30, ColumnName ="ADDRESS_LINE2", BaseColumnName ="ADDRESS_LINE2", BaseTableName = "CUSTOMER" )]		
		public String AddressLine2 
		{ 
		    get { return _addressLine2; } 
			set 
			{
			    _addressLine2 = value;
			}
        }

		private String _city;
		[DataMember]
		[SqlField(DbType.String, 25, ColumnName ="CITY", BaseColumnName ="CITY", BaseTableName = "CUSTOMER" )]		
		public String City 
		{ 
		    get { return _city; } 
			set 
			{
			    _city = value;
			}
        }

		private String _stateProvince;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="STATE_PROVINCE", BaseColumnName ="STATE_PROVINCE", BaseTableName = "CUSTOMER" )]		
		public String StateProvince 
		{ 
		    get { return _stateProvince; } 
			set 
			{
			    _stateProvince = value;
			}
        }

		private String _country;
		[DataMember]
		[SqlField(DbType.String, 15, ColumnName ="COUNTRY", BaseColumnName ="COUNTRY", BaseTableName = "CUSTOMER" )]		
		public String Country 
		{ 
		    get { return _country; } 
			set 
			{
			    _country = value;
			}
        }

		private String _postalCode;
		[DataMember]
		[SqlField(DbType.String, 12, ColumnName ="POSTAL_CODE", BaseColumnName ="POSTAL_CODE", BaseTableName = "CUSTOMER" )]		
		public String PostalCode 
		{ 
		    get { return _postalCode; } 
			set 
			{
			    _postalCode = value;
			}
        }

		private String _onHold;
		[DataMember]
		[SqlField(DbType.String, 1, ColumnName ="ON_HOLD", BaseColumnName ="ON_HOLD", BaseTableName = "CUSTOMER" )]		
		public String OnHold 
		{ 
		    get { return _onHold; } 
			set 
			{
			    _onHold = value;
			}
        }

		public const string BaseTableProjectionColumnList = "\"CUST_NO\", \"CUSTOMER\", \"CONTACT_FIRST\", \"CONTACT_LAST\", \"PHONE_NO\", \"ADDRESS_LINE1\", \"ADDRESS_LINE2\", \"CITY\", \"STATE_PROVINCE\", \"COUNTRY\", \"POSTAL_CODE\", \"ON_HOLD\"";

	}

	public partial class CustomerRepository : Repository<Customer> 
	{
		public CustomerRepository(DataService DataService) : base(DataService)
		{
		}

		public new EmployeesDataService  DataService  
		{
			get { return (EmployeesDataService) base.DataService; }
			set { base.DataService = value; }
		}

		public Customer Get(string projectionName, Int32 custNo)
		{
			return ((IRepository<Customer>)this).Get(projectionName, custNo, FetchMode.UseIdentityMap);
		}

		public Customer Get(string projectionName, Int32 custNo, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projectionName, custNo, fetchMode);
		}

		public Customer Get(Projection projection, Int32 custNo)
		{
			return ((IRepository<Customer>)this).Get(projection, custNo, FetchMode.UseIdentityMap);
		}

		public Customer Get(Projection projection, Int32 custNo, FetchMode fetchMode = FetchMode.UseIdentityMap)
		{
			return ((IRepository<Customer>)this).Get(projection, custNo, fetchMode);
		}

		public Customer Get(string projectionName, Int32 custNo, params string[] fields)
		{
			return ((IRepository<Customer>)this).Get(projectionName, custNo, fields);
		}

		public Customer Get(Projection projection, Int32 custNo, params string[] fields)
		{
			return ((IRepository<Customer>)this).Get(projection, custNo, fields);
		}

		public bool Delete(Int32 custNo)
		{
			var entity = new Customer { CustNo = custNo };
			return this.Delete(entity);
		}

			}
	[Obsolete("Use nameof instead")]
	public static partial class CustomerFields
	{
		public const string CustNo = "CustNo";
		public const string CustomerName = "CustomerName";
		public const string ContactFirst = "ContactFirst";
		public const string ContactLast = "ContactLast";
		public const string PhoneNo = "PhoneNo";
		public const string AddressLine1 = "AddressLine1";
		public const string AddressLine2 = "AddressLine2";
		public const string City = "City";
		public const string StateProvince = "StateProvince";
		public const string Country = "Country";
		public const string PostalCode = "PostalCode";
		public const string OnHold = "OnHold";
	}

	public static partial class CustomerProjections
	{
		public const string BaseTable = "BaseTable";
	}
}

namespace Samples.Entities
{
	public partial class EmployeesDataService : DataService
	{
		partial void OnCreated();

		private void Init()
		{
			EntityNameToEntityViewTransform = TextTransform.None;
			OnCreated();
		}

        public EmployeesDataService() : base("Employees")
        {
			Init();
        }

        public EmployeesDataService(string connectionStringName) : base(connectionStringName)
        {
			Init();
        }

        public EmployeesDataService(string connectionString, string providerName) : base(connectionString, providerName)
        {
			Init();
        }

		private Samples.Entities.CountryRepository _CountryRepository;
		public Samples.Entities.CountryRepository CountryRepository
		{
			get 
			{
				if ( _CountryRepository == null)
				{
					_CountryRepository = new Samples.Entities.CountryRepository(this);
				}
				return _CountryRepository;
			}
		}

		private Samples.Entities.JobRepository _JobRepository;
		public Samples.Entities.JobRepository JobRepository
		{
			get 
			{
				if ( _JobRepository == null)
				{
					_JobRepository = new Samples.Entities.JobRepository(this);
				}
				return _JobRepository;
			}
		}

		private Samples.Entities.DepartmentRepository _DepartmentRepository;
		public Samples.Entities.DepartmentRepository DepartmentRepository
		{
			get 
			{
				if ( _DepartmentRepository == null)
				{
					_DepartmentRepository = new Samples.Entities.DepartmentRepository(this);
				}
				return _DepartmentRepository;
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

		private Samples.Entities.SaleRepository _SaleRepository;
		public Samples.Entities.SaleRepository SaleRepository
		{
			get 
			{
				if ( _SaleRepository == null)
				{
					_SaleRepository = new Samples.Entities.SaleRepository(this);
				}
				return _SaleRepository;
			}
		}

		private Samples.Entities.PhoneRepository _PhoneRepository;
		public Samples.Entities.PhoneRepository PhoneRepository
		{
			get 
			{
				if ( _PhoneRepository == null)
				{
					_PhoneRepository = new Samples.Entities.PhoneRepository(this);
				}
				return _PhoneRepository;
			}
		}

		private Samples.Entities.ProjectRepository _ProjectRepository;
		public Samples.Entities.ProjectRepository ProjectRepository
		{
			get 
			{
				if ( _ProjectRepository == null)
				{
					_ProjectRepository = new Samples.Entities.ProjectRepository(this);
				}
				return _ProjectRepository;
			}
		}

		private Samples.Entities.EmployeeProjectRepository _EmployeeProjectRepository;
		public Samples.Entities.EmployeeProjectRepository EmployeeProjectRepository
		{
			get 
			{
				if ( _EmployeeProjectRepository == null)
				{
					_EmployeeProjectRepository = new Samples.Entities.EmployeeProjectRepository(this);
				}
				return _EmployeeProjectRepository;
			}
		}

		private Samples.Entities.ProjDeptBudgetRepository _ProjDeptBudgetRepository;
		public Samples.Entities.ProjDeptBudgetRepository ProjDeptBudgetRepository
		{
			get 
			{
				if ( _ProjDeptBudgetRepository == null)
				{
					_ProjDeptBudgetRepository = new Samples.Entities.ProjDeptBudgetRepository(this);
				}
				return _ProjDeptBudgetRepository;
			}
		}

		private Samples.Entities.SalaryHistoryItemRepository _SalaryHistoryItemRepository;
		public Samples.Entities.SalaryHistoryItemRepository SalaryHistoryItemRepository
		{
			get 
			{
				if ( _SalaryHistoryItemRepository == null)
				{
					_SalaryHistoryItemRepository = new Samples.Entities.SalaryHistoryItemRepository(this);
				}
				return _SalaryHistoryItemRepository;
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
	}
}
namespace Samples.Entities
{
	public static partial class StoredProcedures
	{
		public static DbCommand CreateOrgChartProcedure(DbConnection connection, string parameterPrefix, string schema = "")
		{
			var cmd = connection.CreateCommand();
			cmd.CommandText = string.IsNullOrEmpty(schema) ? "ORG_CHART" : schema + "." + "ORG_CHART";
			cmd.CommandType = CommandType.StoredProcedure;
			IDbDataParameter p = null;

			return cmd;
		}

	}
}
