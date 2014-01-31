using inercya.EntityLite.Templates;
using Samples.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Samples.Entities.Templates
{
public partial class RaiseProductPricesTemplate : ISqlTemplate
{
    public string DefaultSchema { get; set; }

    public string SchemaPrefix
    {
        get { return string.IsNullOrEmpty(DefaultSchema) ? string.Empty : DefaultSchema + "."; }
    }

    [DbParameter(DbType= DbType.Int32)]
    public int? CategoryId { get; set; }

    [DbParameter(DbType = DbType.Decimal, Precision=5, Scale=4)]
    public decimal Rate { get; set; }

}
}
