namespace MassTransit.EntityLiteIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;
    using inercya.EntityLite;


    public class SqlLockStatementProvider :
        ILockStatementProvider
    {
        protected static readonly ConcurrentDictionary<Type, SchemaTableColumnTrio> TableNames = new ConcurrentDictionary<Type, SchemaTableColumnTrio>();
        readonly bool _enableSchemaCaching;
        readonly ILockStatementFormatter _formatter;


        public SqlLockStatementProvider(ILockStatementFormatter formatter, bool enableSchemaCaching = true)
        {
            _formatter = formatter;
            _enableSchemaCaching = enableSchemaCaching;
        }

        public virtual string GetRowLockStatement<T>(DataService ds)
            where T : class
        {
            return FormatLockStatement<T>(ds, nameof(ISaga.CorrelationId));
        }

        public virtual string GetRowLockStatement<T>(DataService ds, params string[] propertyNames)
            where T : class
        {
            return FormatLockStatement<T>(ds, propertyNames);
        }

        public virtual string GetOutboxStatement(DataService context)
        {
            var schemaTableTrio = GetSchemaAndTableNameAndColumnName(context, typeof(OutboxState), nameof(OutboxState.Created));

            var sb = new StringBuilder(128);
            _formatter.CreateOutboxStatement(sb, schemaTableTrio.Schema, schemaTableTrio.Table, schemaTableTrio.ColumnNames[0]);

            return sb.ToString();
        }

        public IQueryLite<OutboxState> GetOutboxQuery(DataService ds)
        {
            return new TemplatedQueryLite<OutboxState>(ds, new OutboxLockQuery());
        }

        public IQueryLite<InboxState> GetInboxQuery(DataService ds)
        {
            return new TemplatedQueryLite<InboxState>(ds, new InboxLockQuery());
        }

        public IQueryLite<T> GetLockQuery<T>(DataService ds) where T : class
        {
            return new TemplatedQueryLite<T>(ds, new LockQuery<T>(ds));
        }

        string FormatLockStatement<T>(DataService ds, params string[] propertyNames)
            where T : class
        {
            var schemaTableTrio = GetSchemaAndTableNameAndColumnName(ds, typeof(T), propertyNames);

            var sb = new StringBuilder(128);
            _formatter.Create(sb, schemaTableTrio.Schema, schemaTableTrio.Table);

            for (var i = 0; i < propertyNames.Length; i++)
                _formatter.AppendColumn(sb, i, schemaTableTrio.ColumnNames[i]);

            _formatter.Complete(sb);

            return sb.ToString();
        }

        SchemaTableColumnTrio GetSchemaAndTableNameAndColumnName(DataService ds, Type type, params string[] propertyNames)
        {
            if (TableNames.TryGetValue(type, out var result) && _enableSchemaCaching)
                return result;

            var metadata = EntityMetadata.GetEntityMetadata(type);
            if (metadata == null)
            { 
                throw new InvalidOperationException($"Entity metadata not found: {TypeCache.GetShortName(type)}"); 
            }

            var schema = metadata.SchemaName;
            var tableName = metadata.BaseTableName;

            var columnNames = new List<string>();

            for (var i = 0; i < propertyNames.Length; i++)
            {
                if (metadata.Properties.TryGetValue(propertyNames[i], out var property) == false)
                {
                    throw new InvalidOperationException($"Property not found: {TypeCache.GetShortName(type)}.{propertyNames[i]}");
                }
                columnNames.Add(property.SqlField.ColumnName);
            }
            result = new SchemaTableColumnTrio(schema ?? ds.EntityLiteProvider.DefaultSchema, tableName, columnNames.ToArray());

            if (_enableSchemaCaching)
                TableNames.TryAdd(type, result);

            return result;
        }


        protected readonly struct SchemaTableColumnTrio
        {
            public SchemaTableColumnTrio(string schema, string table, string[] columnNames)
            {
                Schema = schema;
                Table = table;
                ColumnNames = columnNames;
            }

            public readonly string Schema;
            public readonly string Table;
            public readonly string[] ColumnNames;
        }
    }
}
