using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;

namespace inercya.EntityLite
{
    public class EntityMetadata
    {
        public string BaseTableName { get; set; }
        
        public string SchemaName { get; set; }
 
        public string AutoIncrementFieldName { get; set; }

        private ICollection<String> _primaryKeyProperties;
        public ICollection<String> PrimaryKeyPropertyNames
        {
            get { return _primaryKeyProperties; }
        }

        private IDictionary<string, PropertyMetadata> _properties;
        public IDictionary<string, PropertyMetadata> Properties
        {
            get { return _properties; }
        }

		private IDictionary<string, PropertyMetadata> _updatableProperties;
		public IDictionary<string, PropertyMetadata> UpdatableProperties
		{
			get { return _updatableProperties; }
		}

        public EntityMetadata()
        {
            _primaryKeyProperties = new Collection<string>();
            _properties = new Dictionary<string, PropertyMetadata>(StringComparer.InvariantCultureIgnoreCase);
			_updatableProperties = new Dictionary<string, PropertyMetadata>(StringComparer.InvariantCultureIgnoreCase);
        }

		private static CacheLite<Type, EntityMetadata> metadataCache = new CacheLite<Type, EntityMetadata>(CreateEntityMetadata);

		public static EntityMetadata GetEntityMetadata(Type entityType)
		{
			return metadataCache.GetItem(entityType);
		}

        public string GetFullTableName(string defaultSchemaName)
        {
            string schemaName = string.IsNullOrEmpty(this.SchemaName) ? defaultSchemaName : this.SchemaName;
            return string.IsNullOrEmpty(schemaName) ? this.BaseTableName : schemaName + "." + this.BaseTableName;
        }

		private static EntityMetadata CreateEntityMetadata(Type entityType)
		{
			object[] attributes = entityType.GetCustomAttributes(typeof(SqlEntityAttribute), false);
			if (attributes.Length == 0) return null;
			SqlEntityAttribute sqlMetadata = (SqlEntityAttribute)attributes[0];

			EntityMetadata entityMetadata = new EntityMetadata();
			entityMetadata.BaseTableName = sqlMetadata.BaseTableName;
			entityMetadata.SchemaName = sqlMetadata.SchemaName;

			PropertyInfo[] properties = entityType.GetProperties();
			foreach (PropertyInfo pi in properties)
			{
				PropertyMetadata propertyMetadata = new PropertyMetadata();
				propertyMetadata.PropertyInfo = pi;
				entityMetadata.Properties.Add(pi.Name, propertyMetadata);

				attributes = pi.GetCustomAttributes(typeof(SqlFieldAttribute), false);
				if (attributes.Length > 0)
				{
					SqlFieldAttribute field = (SqlFieldAttribute)attributes[0];
					propertyMetadata.SqlField = field;
					if (field.IsKey && !string.IsNullOrEmpty(field.BaseTableName) && field.BaseTableName.Equals(entityMetadata.BaseTableName, StringComparison.InvariantCultureIgnoreCase))
					{
						// propiedad con la misma columna base
						string conflictingPrimaryKeyPropertyName = entityMetadata.PrimaryKeyPropertyNames
							.FirstOrDefault(pn => entityMetadata.Properties[pn].SqlField.BaseColumnName == field.BaseColumnName);

						if (conflictingPrimaryKeyPropertyName == null)
						{
							entityMetadata.PrimaryKeyPropertyNames.Add(pi.Name);
						}
						else if (field.BaseColumnName == pi.Name)
						{
							entityMetadata.PrimaryKeyPropertyNames.Remove(conflictingPrimaryKeyPropertyName);
							entityMetadata.PrimaryKeyPropertyNames.Add(pi.Name);
						}						
					}

					if (field.IsAutoincrement == false && field.IsReadOnly == false && string.IsNullOrEmpty(entityMetadata.BaseTableName) == false
						&& string.Equals(entityMetadata.BaseTableName, field.BaseTableName, StringComparison.InvariantCultureIgnoreCase))
					{
						string conflictingPropertyName = entityMetadata.UpdatableProperties
							.FirstOrDefault(kv => kv.Value.SqlField != null && kv.Value.SqlField.BaseColumnName == field.BaseColumnName).Key;

						if (conflictingPropertyName == null)
						{
							entityMetadata.UpdatableProperties.Add(pi.Name, propertyMetadata);
						}
						else if (field.BaseColumnName == pi.Name)
						{
							entityMetadata.UpdatableProperties.Remove(conflictingPropertyName);
							entityMetadata.UpdatableProperties.Add(pi.Name, propertyMetadata);
						}
					}

					if (field.IsAutoincrement && !string.IsNullOrEmpty(field.BaseTableName) && field.BaseTableName.Equals(entityMetadata.BaseTableName, StringComparison.InvariantCultureIgnoreCase))
					{
						if (string.IsNullOrEmpty(entityMetadata.AutoIncrementFieldName) || field.BaseColumnName == pi.Name)
						{
							entityMetadata.AutoIncrementFieldName = pi.Name;
						}
					}
				}
				if (pi.GetCustomAttributes(typeof(LocalizedFieldAttribute), false).Length > 0)
				{
					propertyMetadata.IsLocalizedFiled = true;
				}
			}
			return entityMetadata;
		}
    }

    public class PropertyMetadata
    {
        public SqlFieldAttribute SqlField;
        public PropertyInfo PropertyInfo;
		public bool IsLocalizedFiled;
    }
}
