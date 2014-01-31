using inercya.EntityLite;
using Samples.Entities.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samples.Entities
{
public partial class ProductSaleRepository
{

    public IQueryLite<ProductSale> TemplatedQuery(string grouping, int? employeeId)
    {
        var template = new SalesQueryTemplate
        {
            EmployeeId = employeeId,
            Grouping = grouping
        };
        return new TemplatedQueryLite<ProductSale>(this.DataService, template);
    }
}
}
