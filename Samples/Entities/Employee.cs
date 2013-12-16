using inercya.EntityLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samples.Entities
{
    public partial class EmployeeRepository 
    {
        public IQueryLite<Employee> CreateGetEmployeeSubtreeQuery(int employeeId)
        {
            return new FunctionQueryLite<Employee>(this.DataService, "dbo.GetEmployeeSubTree", employeeId);
        }
    }
}
