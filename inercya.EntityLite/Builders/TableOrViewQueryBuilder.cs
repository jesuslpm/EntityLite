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
using inercya.EntityLite.Extensions;
using System.Data.Common;
using System.Reflection;

namespace inercya.EntityLite.Builders
{
    public class TableOrViewQueryBuilder : AbstractQueryBuilder
    {

        public TableOrViewQueryBuilder(IQueryLite queryLite) : base(queryLite)
        {
        }


        public string FullyQualifiedTableOrViewName
        {
            get
            {
                return ((ITableOrViewQueryLite)this.QueryLite).FullyQualifiedTableOrViewName;
            }
        }

        public override string GetColumnList()
        {
            if (QueryLite.FieldList == null || QueryLite.FieldList.Count == 0)
            {
                return "*";
            }
            return base.GetColumnList();
        }

        public override string GetFromClauseContent(DbCommand selectCommand, ref int paramIndex, int indentation)
        {
            string tableOrViewName = this.FullyQualifiedTableOrViewName;
            string startQuote =  this.QueryLite.DataService.EntityLiteProvider.StartQuote;
            string endQuote =  this.QueryLite.DataService.EntityLiteProvider.EndQuote;
            string quotedObjectName = null;
            if (tableOrViewName.StartsWith(startQuote) && tableOrViewName.EndsWith(endQuote))
            {
                quotedObjectName = tableOrViewName;
            }
            else
            {
                var tokens = tableOrViewName.Split('.');
                quotedObjectName = string.Join(".", tokens.Select(x => startQuote + x + endQuote).ToArray());
            }

            string fromContent = quotedObjectName;
            if (this.QueryLite.Alias != null) fromContent += " " + this.QueryLite.Alias.Name;
            return fromContent;
        }
    }
}
