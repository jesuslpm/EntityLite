using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samples.Entities;
using inercya.EntityLite;

using FirebirdSql.Data.FirebirdClient;

namespace FirebirdFx
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ds = new EmployeesDataService())
            {
                var chart = ds.DepartmentRepository.OrgChart();
                //var customers = ds.CustomerRepository
                //    .Query(CustomerProjections.BaseTable)
                //    .WithTimeout(300)
                //    .OrderBy(nameof(Customer.CustNo))
                //    .ToList(10, 19);

                //var emp = new Employee
                //{
                //    DeptNo = "180",
                //    FirstName = "Jesús",
                //    HireDate = DateTime.Today,
                //    JobCode = "Eng",
                //    JobCountry = "USA",
                //    JobGrade = 3,
                //    LastName = "López",
                //    PhoneExt = "123",
                //    Salary = 72550
                //};

                //ds.EmployeeRepository.Insert(emp);

                //var employees = ds.EmployeeRepository.Query(EmployeeProjections.BaseTable).ToList();

                //var result = ds.CustomerRepository.CommandTemplate();

            
            }

            //Anonimous_Block_With_Output_Parameters();

            
        }

        static void Anonimous_Block_With_Output_Parameters()
        {
            var commandText = @"
EXECUTE BLOCK (input_param INTEGER = @InputParam)
RETURNS (output_param1 INTEGER, output_param2 INTEGER)
AS
BEGIN
    output_param1 = input_param;
    output_param2 = 2 * input_param;
    suspend;
END
";
            using (var cn = new FbConnection(@"Data Source=localhost;Database=C:\Program Files\Firebird\Firebird_2_5\examples\empbuild\EMPLOYEE.FDB;User=SYSDBA;Password=masterkey"))
            using (var cmd = new FbCommand(commandText, cn))
            {
                var inputParam = cmd.Parameters.Add("@InputParam", FbDbType.Integer);
                inputParam.Value = 1;
                cn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.Dispose();
                    if (reader.Read())
                    {
                        for(int fieldIndex = 0; fieldIndex < reader.FieldCount;fieldIndex++)
                        {
                            Console.WriteLine($"{reader.GetName(fieldIndex)}: {reader.GetValue(fieldIndex)}");
                        }
                    }
                }
            }
        }
    }
}
