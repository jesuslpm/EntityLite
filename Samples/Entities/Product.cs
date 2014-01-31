using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inercya.EntityLite;
using Samples.Entities.Templates;

namespace Samples.Entities
{
    public partial class ProductRepository
    {
        public int RaiseProductPrices(int? categoryId, decimal rate)
        {
            var template = new RaiseProductPricesTemplate
            {
                CategoryId = categoryId,
                DefaultSchema = this.DataService.EntityLiteProvider.DefaultSchema,
                Rate = rate
            };
            var cmd = new TemplatedCommand(this.DataService, template);
            return cmd.ExecuteNonQuery();
        }
    }
}
