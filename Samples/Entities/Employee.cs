using inercya.EntityLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samples.Entities
{
    public partial class EmployeeRepository 
    {
        public IQueryLite<Employee> EmployeeSubtreeQuery(int employeeId)
        {
            return new FunctionQueryLite<Employee>(this.DataService, "dbo.GetEmployeeSubTree", employeeId);
        }

        public IQueryLite<Employee> ThatSoldAllSpecifiedProductsQuery(IEnumerable<int> productIds)
        {
            var template = new Templates.EmployeesThatSoldAllSpecifiedProductsQueryTemplate
            {
                ProductIds = productIds
            };

            return new TemplatedQueryLite<Employee>(this.DataService, template);
        }
    }
}
