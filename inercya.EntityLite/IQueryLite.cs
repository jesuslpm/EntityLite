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
using System.Collections;
using inercya.EntityLite.Builders;
using inercya.EntityLite.Templates;
using System.Data;
using System.Threading.Tasks;

namespace inercya.EntityLite
{
    public interface IQueryLite
    {
        ICollection<ConditionLite> Filter { get; set; }
        ICollection<SortDescriptor> Sort { get; set; }
		ICollection<string> Options { get; set; }
		IEnumerable ToEnumerable();
		IEnumerable ToEnumerable(int fromIndex, int toIndex);
        IList ToList();
        IList ToList(int fromIndex, int toIndex);
		object FirstOrDefault();
        int GetCount();
        bool Any();
        Type EntityType { get; set; }
        IList<string> FieldList { get; set; }
        IQueryBuilder QueryBuilder { get; set; }
        DataService DataService { get; set; }
        DataTable Pivot(params PivotTransform[] pivotTransforms);
        DataTable Pivot(Comparison<PivotedColumn> pivotedColumnComparison, params PivotTransform[] pivotTransforms);
        int CommandTimeout { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "this is not a Keyword in c# and we need to be backward compatible")]
        Alias Alias { get; set; }
        int Delete();

        int SelectInto(string destinationTableName);
        int InsertInto(string destinationTableName, params string[] columnNames);
        int InsertInto<T>(string destinationTableName, params string[] propertyNames);
        int InsertIntoBaseTable(params string[] propertyNames);
        int InsertIntoBaseTable<T>(params string[] propertyNames);

        Task<IEnumerable> ToEnumerableAsync();
        Task<IEnumerable> ToEnumerableAsync(int fromIndex, int toIndex);
        Task<IList> ToListAsync();
        Task<IList> ToListAsync(int fromIndex, int toIndex);
        Task<object> FirstOrDefaultAsync();
        Task<int> GetCountAsync();
        Task<bool> AnyAsync();
        Task<int> DeleteAsync();
        Task<int> SelectIntoAsync(string destinationTableName);
        Task<int> InsertIntoAsync(string destinationTableName, params string[] columnNames);
        Task<int> InsertIntoAsync<T>(string destinationTableName, params string[] propertyNames);
        Task<int> InsertIntoBaseTableAsync(params string[] propertyNames);
        Task<int> InsertIntoBaseTableAsync<T>(params string[] propertyNames);

    }

    public interface IQueryLite<TEntity> : IQueryLite
    {
		new TEntity FirstOrDefault();
        new IEnumerable<TEntity> ToEnumerable();
        new IEnumerable<TEntity> ToEnumerable(int fromIndex, int toIndex);
        new List<TEntity> ToList();
        new List<TEntity> ToList(int fromIndex, int toIndex);
        new Task<TEntity> FirstOrDefaultAsync();
        new Task<IEnumerable<TEntity>> ToEnumerableAsync();
        new Task<IEnumerable<TEntity>> ToEnumerableAsync(int fromIndex, int toIndex);
        new Task<List<TEntity>> ToListAsync();
        new Task<List<TEntity>> ToListAsync(int fromIndex, int toIndex);
    }

    public interface IProjectedQueryLite : IQueryLite
    {
        string ProjectionName { get; set; }
    }

    public interface ITableOrViewQueryLite: IQueryLite
    {
        string FullyQualifiedTableOrViewName { get; set; }
    }

    public interface IParameterizedQueryLite : IQueryLite
    {
        object[] ParameterValues { get; set; }
    }

    public interface ITableValuedFunctionQueryLite : IParameterizedQueryLite
    {
        string FunctionName { get; set; }
    }

    public interface ITemplatedQueryLite : IQueryLite
    {
        ISqlTemplate Template { get; set; }
    }
}
