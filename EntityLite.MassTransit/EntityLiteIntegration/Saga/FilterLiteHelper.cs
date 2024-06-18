using inercya.EntityLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.EntityLiteIntegration.Saga
{
    internal class FilterLiteHelper
    {
        public static ICollection<ConditionLite> GetFilterFromExpression<TSaga>(Expression<Func<TSaga, bool>> expression)
        {
            List<(string, object?)> propertiesAndValues = SqlExpressionVisitor.CreateFromExpression(expression);
            var filter = new List<ConditionLite>();
            foreach (var (propertyName, value) in propertiesAndValues)
            {
                if (value == null)
                {
                    filter.Where(propertyName, OperatorLite.IsNull);
                }
                else
                {
                    filter.Where(propertyName, OperatorLite.Equals, value);
                }
            }
            return filter;
        }
    }
}
