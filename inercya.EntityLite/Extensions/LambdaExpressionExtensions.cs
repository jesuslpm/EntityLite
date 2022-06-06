/*
Copyright 2014 i-nercya intelligent software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
			if (selector == null) throw new ArgumentNullException(nameof(selector));
			var memberExpression = selector.Body as MemberExpression;
			if (memberExpression == null)
			{
				var unaryExpression = selector.Body as UnaryExpression;
				if (unaryExpression != null)
				{
					memberExpression = unaryExpression.Operand as MemberExpression;
				}

			}
			if (memberExpression == null) throw new ArgumentException("selector must be a member expression", nameof(selector));
			return memberExpression.Member.Name;
		}
	}
}
