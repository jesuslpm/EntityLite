using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace inercya.EntityLite.Extensions
{
	public static class LambdaExpressionExtensions
	{
		public static string GetMemberName(this LambdaExpression selector)
		{
			if (selector == null) throw new ArgumentNullException("selector");
			var memberExpression = selector.Body as MemberExpression;
			if (memberExpression == null)
			{
				var unaryExpression = selector.Body as UnaryExpression;
				if (unaryExpression != null)
				{
					memberExpression = unaryExpression.Operand as MemberExpression;
				}

			}
			if (memberExpression == null) throw new ArgumentException("selector must be a member expression", "selector");
			return memberExpression.Member.Name;
		}
	}
}
