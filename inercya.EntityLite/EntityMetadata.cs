﻿/*
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
using System.Reflection;
using System.Collections.ObjectModel;

namespace inercya.EntityLite
{
    public class EntityMetadata
    {
        public Type EntityType { get; private set; }

        public IPropertyGetterDictionary Getters { get; private set; }

        public IPropertySetterDictionary Setters { get; private set; }

        public string BaseTableName { get; set; }
        
        public string SchemaName { get; set; }
 
        public string AutoIncrementFieldName { get; set; }

        public string SequenceFieldName { get; set; }

        public string SequenceName { get; set; }

        public bool IsAuditable { get; set; }
        public string FieldPrefix { get; set; }

        public string AutogeneratedFieldName
        {
            get
            {
                string autoGeneratedFieldName = this.SequenceFieldName;
                if (this.AutoIncrementFieldName != null) autoGeneratedFieldName = this.AutoIncrementFieldName;
                return autoGeneratedFieldName;
            }
        }

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


        public string PrimaryKeyPropertyName { get; private set; }


        public bool IsPrimaryKeyGuid { get; private set; }

        public Type PrimaryKeyType { get; private set; }

        public Dictionary<string, PropertyMetadata> ColumnToPropertyMap { get; private set; } = new Dictionary<string, PropertyMetadata>();

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

        private string _fullTableName;

        public string GetFullTableName(string defaultSchemaName, string startQuote, string endQuote)
        {
            if (_fullTableName == null)
            {
                if (string.IsNullOrEmpty(this.BaseTableName)) return null;
                string schemaName = string.IsNullOrEmpty(this.SchemaName) ? defaultSchemaName : this.SchemaName;
                _fullTableName = string.IsNullOrEmpty(schemaName) ? startQuote + this.BaseTableName + endQuote : startQuote + schemaName + endQuote + "." + startQuote + this.BaseTableName + endQuote;
            }
            return _fullTableName;
        }

        public string GetFullSequenceName(string defaultSchemaName)
        {
            
            if (string.IsNullOrEmpty(this.SequenceName)) return null;
            if (this.SequenceName.IndexOf('.') >= 0) return this.SequenceName;
            string schemaName = string.IsNullOrEmpty(this.SchemaName) ? defaultSchemaName : this.SchemaName;
            return string.IsNullOrEmpty(schemaName) ? this.SequenceName : schemaName + "." + this.SequenceName;
        }

		private static EntityMetadata CreateEntityMetadata(Type entityType)
		{
            

			object[] attributes = entityType.GetCustomAttributes(typeof(SqlEntityAttribute), false);
			if (attributes.Length == 0) return null;
            EntityMetadata entityMetadata = new EntityMetadata()
            {
                EntityType = entityType,
                Getters = PropertyHelper.GetPropertyGetters(entityType),
                Setters = PropertyHelper.GetPropertySetters(entityType)
            };
			SqlEntityAttribute sqlMetadata = (SqlEntityAttribute)attributes[0];

			
			entityMetadata.BaseTableName = sqlMetadata.BaseTableName;
			entityMetadata.SchemaName = sqlMetadata.SchemaName;
            entityMetadata.IsAuditable = sqlMetadata.IsAuditable;
            entityMetadata.FieldPrefix = sqlMetadata.FieldPrefix ?? string.Empty;

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
                    entityMetadata.ColumnToPropertyMap.Add(field.ColumnName, propertyMetadata);
					if (field.IsKey && !string.IsNullOrEmpty(field.BaseTableName) && field.BaseTableName.Equals(entityMetadata.BaseTableName, StringComparison.OrdinalIgnoreCase))
					{
						// propiedad con la misma columna base
						string conflictingPrimaryKeyPropertyName = entityMetadata.PrimaryKeyPropertyNames
							.FirstOrDefault(pn => entityMetadata.Properties[pn].SqlField.BaseColumnName == field.BaseColumnName);

						if (conflictingPrimaryKeyPropertyName == null)
						{
							entityMetadata.PrimaryKeyPropertyNames.Add(pi.Name);
						}
						else if (string.Equals(field.BaseColumnName, field.ColumnName, StringComparison.OrdinalIgnoreCase))
						{
							entityMetadata.PrimaryKeyPropertyNames.Remove(conflictingPrimaryKeyPropertyName);
							entityMetadata.PrimaryKeyPropertyNames.Add(pi.Name);
						}						
					}

					if (field.IsAutoincrement == false && field.IsReadOnly == false && string.IsNullOrEmpty(entityMetadata.BaseTableName) == false
						&& string.Equals(entityMetadata.BaseTableName, field.BaseTableName, StringComparison.OrdinalIgnoreCase))
					{
						string conflictingPropertyName = entityMetadata.UpdatableProperties
							.FirstOrDefault(kv => kv.Value.SqlField != null && kv.Value.SqlField.BaseColumnName == field.BaseColumnName).Key;

						if (conflictingPropertyName == null)
						{
							entityMetadata.UpdatableProperties.Add(pi.Name, propertyMetadata);
						}
						else if (string.Equals(field.BaseColumnName, field.ColumnName, StringComparison.OrdinalIgnoreCase))
						{
							entityMetadata.UpdatableProperties.Remove(conflictingPropertyName);
							entityMetadata.UpdatableProperties.Add(pi.Name, propertyMetadata);
						}
					}

					if (field.IsAutoincrement && !string.IsNullOrEmpty(field.BaseTableName) && field.BaseTableName.Equals(entityMetadata.BaseTableName, StringComparison.OrdinalIgnoreCase))
					{
						if (string.IsNullOrEmpty(entityMetadata.AutoIncrementFieldName) || string.Equals(field.BaseColumnName, field.ColumnName, StringComparison.OrdinalIgnoreCase))
						{
							entityMetadata.AutoIncrementFieldName = pi.Name;
						}
					}

                    if (!string.IsNullOrEmpty(field.SequenceName) && field.BaseTableName.Equals(entityMetadata.BaseTableName, StringComparison.OrdinalIgnoreCase))
                    { 
                        if (string.IsNullOrEmpty(entityMetadata.SequenceName) || string.Equals(field.BaseTableName, field.ColumnName, StringComparison.OrdinalIgnoreCase))
                        {
                            entityMetadata.SequenceFieldName = pi.Name;
                            entityMetadata.SequenceName = field.SequenceName;
                        }
                    }
				}
				if (pi.GetCustomAttributes(typeof(LocalizedFieldAttribute), false).Length > 0)
				{
					propertyMetadata.IsLocalizedFiled = true;
				}
			}

            if (entityMetadata.PrimaryKeyPropertyNames.Count == 1)
            {
                entityMetadata.PrimaryKeyPropertyName = entityMetadata.PrimaryKeyPropertyNames.FirstOrDefault();
            }

            if (entityMetadata.PrimaryKeyPropertyName != null)
            {
                entityMetadata.PrimaryKeyType = entityMetadata.Properties[entityMetadata.PrimaryKeyPropertyName].PropertyInfo.PropertyType;
                entityMetadata.IsPrimaryKeyGuid = entityMetadata.PrimaryKeyType == typeof(Guid);
            }
			return entityMetadata;
		}
    }

    public class PropertyMetadata
    {
        public SqlFieldAttribute SqlField { get; internal set; }
        public PropertyInfo PropertyInfo { get; internal set; }
        public bool IsLocalizedFiled { get; internal set; }
    }
}
