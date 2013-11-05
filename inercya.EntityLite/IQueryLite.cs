using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using inercya.EntityLite.Builders;

namespace inercya.EntityLite
{
    public interface IQueryLite
    {
        ICollection<SimpleConditionLite> Filter { get; set; }
        ICollection<SortDescriptor> Sort { get; set; }
		ICollection<string> Options { get; set; }
		IEnumerable ToEnumerable();
		IEnumerable ToEnumerable(int fromIndex, int toIndex);
        IList ToList();
        IList ToList(int fromIndex, int toIndex);
		object FirstOrDefault();
        int GetCount();
        Type EntityType { get; set; }
        string FieldList { get; set; }
        Func<IQueryBuilder> CreateQueryBuilder { get; set; }
        DataService DataService { get; set; }
    }

    public interface IQueryLite<TEntity> : IQueryLite
    {
		new TEntity FirstOrDefault();
		new IEnumerable<TEntity> ToEnumerable();
		new IEnumerable<TEntity> ToEnumerable(int fromIndex, int toIndex);
        new IList<TEntity> ToList();
        new IList<TEntity> ToList(int fromIndex, int toIndex);
    }

    public interface IProjectedQueryLite : IQueryLite
    {
        string ProjectionName { get; set; }
    }

    public interface IParameterizedQueryLite : IQueryLite
    {
        object[] ParameterValues { get; set; }
    }

    public interface ITableValuedFunctionQueryLite : IParameterizedQueryLite
    {
        string FunctionName { get; set; }
    }
}
