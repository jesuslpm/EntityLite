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
using System.Reflection;
using System.Globalization;
using System.Threading.Tasks;

namespace inercya.EntityLite
{
    public interface IRepository
    {
        IQueryLite Query(Projection projection);
        IQueryLite Query(string projectionName);
        SaveResult Save(object entity);
        void Insert(object entity);
        bool Update(object entity);
        bool Update(object entity, params string[] fieldsToUpdate);
        bool Delete(object entity);
        Type EntityType { get; }
#pragma warning disable CA1716 // Identifiers should not match keywords
        object Get(Projection projection, object entityId, FetchMode fetchMode);
#pragma warning restore CA1716 // Identifiers should not match keywords
        object Get(string projectionName, object entityId, FetchMode fetchMode);
        object Get(Projection projection, object entityId, string[] fields);
        object Get(string projectionName, object entityId, string[] fields);
        Task<object> GetAsync(string projectionName, object entityId, string[] fields);
        Task<object> GetAsync(Projection projection, object entityId, string[] fields);
        Task<object> GetAsync(string projectionName, object entityId, FetchMode fetchMode);
        Task<object> GetAsync(Projection projection, object entityId, FetchMode fetchMode);
        Task<bool> DeleteAsync(object entity);
        Task<bool> UpdateAsync(object entity, params string[] fieldsToUpdate);
        Task<bool> UpdateAsync(object entity);
        Task InsertAsync(object entity);
        Task<SaveResult> SaveAsync(object entity);

    }

    public interface IRepository<TEntity> : IRepository 
    {
        SaveResult Save(TEntity entity);
        SaveCollectionResult<TEntity> Save(IEnumerable<TEntity> entities, IEnumerable<TEntity> currentEntities);
        void Insert(TEntity entity);
        bool Update(TEntity entity);
        bool Update(TEntity entity, params string[] fieldsToUpdate);
        bool Delete(TEntity entity);
        new IQueryLite<TEntity> Query(Projection projection);
        new IQueryLite<TEntity> Query(string projectionName);
		new TEntity Get(Projection projection, object entityId, FetchMode fetchMode);
		new TEntity Get(string projectionName, object entityId, FetchMode fetchMode);
        new TEntity Get(Projection projection, object entityId, string[] fields);
        new TEntity Get(string projectionName, object entityId, string[] fields);
        Task<SaveResult> SaveAsync(TEntity entity);
        Task InsertAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity, params string[] fieldsToUpdate);
        Task<bool> DeleteAsync(TEntity entity);
        new Task<TEntity> GetAsync(Projection projection, object entityId, FetchMode fetchMode);
        new Task<TEntity> GetAsync(string projectionName, object entityId, FetchMode fetchMode);
        new Task<TEntity> GetAsync(Projection projection, object entityId, string[] fields);
        new Task<TEntity> GetAsync(string projectionName, object entityId, string[] fields);

        Task<SaveCollectionResult<TEntity>> SaveAsync(IEnumerable<TEntity> entities, IEnumerable<TEntity> currentEntities);
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity: class
    {
		public DataService DataService { get;  set; }


        private static EntityMetadata _entityMetadata;

        public static EntityMetadata EntityMetadata 
        {
            get
            {
                if (_entityMetadata == null)
                {
                    _entityMetadata = EntityMetadata.GetEntityMetadata(typeof(TEntity));
                }
                return _entityMetadata;
            }
        }

        private static IPropertyGetterDictionary _getters;
        public static IPropertyGetterDictionary Getters
        {
            get 
            {
                if (_getters == null)
                {
                    _getters = PropertyHelper.GetPropertyGetters(typeof(TEntity));
                }
                return _getters;
            }
        }

        private static IPropertySetterDictionary _setters;
        public static IPropertySetterDictionary Setters
        {
            get
            {
                if (_setters == null)
                {
                    _setters = PropertyHelper.GetPropertySetters(typeof(TEntity));
                }
                return _setters;
            }
        }


        private static volatile bool isSaveCollectionFunctionInitialized;
        private static object saveCollectionFunctionSyncObject = new object();
        static Func<Repository<TEntity>, IEnumerable<TEntity>, IEnumerable<TEntity>, SaveCollectionResult<TEntity>> _saveCollectionFunction;
        static Func<Repository<TEntity>, IEnumerable<TEntity>, IEnumerable<TEntity>, SaveCollectionResult<TEntity>> SaveCollectionFunction
        {
            get
            {
                if (isSaveCollectionFunctionInitialized == false)
                {
                    lock (saveCollectionFunctionSyncObject)
                    {
#pragma warning disable CA1508 // This waring is incorrect, isSaveCollectionFunctionInitialized will not always be false, the next line assigns it to true
                        if (isSaveCollectionFunctionInitialized == false)
#pragma warning restore CA1508 // Avoid dead conditional code 
                        {
                            isSaveCollectionFunctionInitialized = true;
                            var mi = typeof(Repository<TEntity>).GetMethod("SaveCollection", BindingFlags.NonPublic | BindingFlags.Static);
                            mi = mi.MakeGenericMethod(EntityMetadata.PrimaryKeyType);
                            _saveCollectionFunction = (Func<Repository<TEntity>, IEnumerable<TEntity>, IEnumerable<TEntity>, SaveCollectionResult<TEntity>>)Delegate.CreateDelegate(typeof(Func<Repository<TEntity>, IEnumerable<TEntity>, IEnumerable<TEntity>, SaveCollectionResult<TEntity>>), mi);
                        }
                    }
                }
                return _saveCollectionFunction;
            }
        }

        private static volatile bool isSaveCollectionAsyncFunctionInitialized;
        private static object saveCollectionAsyncFunctionSyncObject = new object();
        static Func<Repository<TEntity>, IEnumerable<TEntity>, IEnumerable<TEntity>, Task<SaveCollectionResult<TEntity>>> _saveCollectionAsyncFunction;
        static Func<Repository<TEntity>, IEnumerable<TEntity>, IEnumerable<TEntity>, Task<SaveCollectionResult<TEntity>>> SaveCollectionAsyncFunction
        {
            get
            {
                if (isSaveCollectionAsyncFunctionInitialized == false)
                {
                    lock (saveCollectionAsyncFunctionSyncObject)
                    {
#pragma warning disable CA1508 // This waring is incorrect, isSaveCollectionAsyncFunctionInitialized will not always be false, the next line assigns it to true
                        if (isSaveCollectionAsyncFunctionInitialized == false)
#pragma warning restore CA1508 // Avoid dead conditional code 
                        {
                            isSaveCollectionAsyncFunctionInitialized = true;
                            var mi = typeof(Repository<TEntity>).GetMethod("SaveCollectionAsync", BindingFlags.NonPublic | BindingFlags.Static);
                            mi = mi.MakeGenericMethod(EntityMetadata.PrimaryKeyType);
                            _saveCollectionAsyncFunction = (Func<Repository<TEntity>, IEnumerable<TEntity>, IEnumerable<TEntity>, Task<SaveCollectionResult<TEntity>>>)Delegate.CreateDelegate(typeof(Func<Repository<TEntity>, IEnumerable<TEntity>, IEnumerable<TEntity>, Task<SaveCollectionResult<TEntity>>>), mi);
                        }
                    }
                }
                return _saveCollectionAsyncFunction;
            }
        }

        public Repository(DataService dataService)
        {
            this.DataService = dataService;
        }

#region IRepository<TEntity> Members

        public virtual SaveResult Save(TEntity entity)
        {
            bool isNew = IsNew(entity);

            if (isNew)
            {
                this.Insert(entity);
                return SaveResult.Inserted;
            }

            if (this.Update(entity))
            {
                return SaveResult.Updated;
            }
            return SaveResult.NotModified;
        }

        public virtual SaveResult Save(TEntity entity, params string[] fieldsToUpdate)
        {
            bool isNew = IsNew(entity);

            if (isNew)
            {
                this.Insert(entity);
                return SaveResult.Inserted;
            }

            if (this.Update(entity, fieldsToUpdate))
            {
                return SaveResult.Updated;
            }
            return SaveResult.NotModified;
        }

        private static SaveCollectionResult<TEntity> SaveCollection<TKey>(Repository<TEntity> repository, IEnumerable<TEntity> entities, IEnumerable<TEntity> currentEntities)
        {
            var getId = typeof(TEntity).GetIdGetter();
            var currentEntityById = currentEntities.ToDictionary(x => (TKey)getId(x));
            var entitySet = new HashSet<TKey>(entities.Select(x => (TKey)getId(x)));

            var result = new SaveCollectionResult<TEntity>();

            repository.DataService.BeginTransaction();
            try
            {
                result.Deleted = currentEntities.Where(current => getId(current) != null && !entitySet.Contains((TKey)getId(current))).ToList();
                foreach (var deletedEntity in result.Deleted)
                {
                    repository.Delete(deletedEntity);
                }

                result.Updated = new List<TEntity>();
                foreach (var entity in entities)
                {
                    object id = getId(entity);
                    if (id != null && currentEntityById.TryGetValue((TKey)id, out var current) && entity.IsModifiedFrom(current))
                    {
                        repository.Update(entity);
                        result.Updated.Add(entity);
                    }
                }

                result.Inserted = entities.Where(entity => getId(entity) == null || !currentEntityById.ContainsKey((TKey)getId(entity))).ToList();
                foreach (var newEntity in result.Inserted)
                {
                    repository.Insert(newEntity);
                }

                repository.DataService.Commit();
                return result;
            }
            catch
            {
                if (repository.DataService.IsActiveTransaction) repository.DataService.Rollback();
                throw;
            }
        }


        public SaveCollectionResult<TEntity> Save(IEnumerable<TEntity> entities, IEnumerable<TEntity> currentEntities)
        {
            if (SaveCollectionFunction == null)
            {
                throw new NotSupportedException($"Entity {typeof(TEntity).Name} has no single primary key");
            }
            return SaveCollectionFunction(this, entities, currentEntities);
        }

        public static bool IsNew(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Type entityType = entity.GetType();
            var metadata = entityType.GetEntityMetadata();
            if (metadata == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} cannot be saved because it is not an entity, it has no SqlEntity attribute", entityType.Name));
            }
            if (string.IsNullOrEmpty(metadata.BaseTableName))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} cannot be saved because it has no base table", entityType.Name));
            }
            if (metadata.PrimaryKeyPropertyNames.Count == 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} cannot be saved, because is has no primary key", entityType.Name));
            }
            if (metadata.PrimaryKeyPropertyNames.Count > 1)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,"{0} cannot be saved, because its primary key is multiple, use insert or update instead", entityType.Name));
            }
            string primaryKeyFieldName = metadata.PrimaryKeyPropertyNames.FirstOrDefault();

            object id = entity.GetPropertyValue(primaryKeyFieldName);

            bool isNew = false;
            if (id is Guid)
            {
                if (object.Equals(id, Guid.Empty))
                {
                    isNew = true;
                }
            }
            else
            {
                // No podemos lanzar error porque aunque el campo no sea autogenerado por la base de datos
                // podría ser generado en el dataservice.
                //if (string.IsNullOrEmpty(metadata.AutogeneratedFieldName))
                //{
                //    throw new ArgumentException(string.Format("{0} cannot be saved, because {1} is not an autogenerated field, use insert and update instead", entityType.Name, primaryKeyFieldName));
                //}
                var idTypeCode = Type.GetTypeCode(id.GetType());
                if (idTypeCode == TypeCode.Int16 || idTypeCode == TypeCode.Int32 || idTypeCode == TypeCode.Int64 || idTypeCode == TypeCode.Decimal)
                {
                    decimal decimalId = Convert.ToDecimal(id, CultureInfo.InvariantCulture);
                    if (decimalId == 0m) isNew = true;
                }
                else
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} cannot be saved, because the type of {1}  is not supported, use insert or update instead", entityType.Name, primaryKeyFieldName));
                }
            }
            return isNew;
        }

        public virtual async Task<SaveResult> SaveAsync(TEntity entity)
        {
            bool isNew = IsNew(entity);
            if (isNew)
            {
                await this.InsertAsync(entity).ConfigureAwait(false);
                return SaveResult.Inserted;
            }

            if (await this.UpdateAsync(entity).ConfigureAwait(false))
            {
                return SaveResult.Updated;
            }
            return SaveResult.NotModified;
        }


        public Task<SaveCollectionResult<TEntity>> SaveAsync(IEnumerable<TEntity> entities, IEnumerable<TEntity> currentEntities)
        {
            if (SaveCollectionAsyncFunction == null)
            {
                throw new NotSupportedException($"Entity {typeof(TEntity).Name} has no single primary key");
            }
            return SaveCollectionAsyncFunction(this, entities, currentEntities);
        }


        private static async Task<SaveCollectionResult<TEntity>> SaveCollectionAsync<TKey>(Repository<TEntity> repository, IEnumerable<TEntity> entities, IEnumerable<TEntity> currentEntities)
        {
            var getId = typeof(TEntity).GetIdGetter();
            var currentEntityById = currentEntities.ToDictionary(x => (TKey)getId(x));
            var entitySet = new HashSet<TKey>(entities.Select(x => (TKey)getId(x)));

            var result = new SaveCollectionResult<TEntity>();

            repository.DataService.BeginTransaction();
            try
            {
                result.Deleted = currentEntities.Where(current => getId(current) != null && !entitySet.Contains((TKey)getId(current))).ToList();
                foreach (var deletedEntity in result.Deleted)
                {
                    await repository.DeleteAsync(deletedEntity).ConfigureAwait(false);
                }

                result.Updated = new List<TEntity>();
                foreach (var entity in entities)
                {
                    object id = getId(entity);
                    if (id != null && currentEntityById.TryGetValue((TKey)id, out var current) && entity.IsModifiedFrom(current))
                    {
                        await repository.UpdateAsync(entity).ConfigureAwait(false);
                        result.Updated.Add(entity);
                    }
                }

                result.Inserted = entities.Where(entity => getId(entity) == null || !currentEntityById.ContainsKey((TKey)getId(entity))).ToList();
                foreach (var newEntity in result.Inserted)
                {
                    await repository.InsertAsync(newEntity).ConfigureAwait(false);
                }

                repository.DataService.Commit();
                return result;
            }
            catch
            {
                if (repository.DataService.IsActiveTransaction) repository.DataService.Rollback();
                throw;
            }
        }

 



        public virtual async Task<SaveResult> SaveAsync(TEntity entity, params string[] fieldsToUpdate)
        {
            bool isNew = IsNew(entity);
            if (isNew)
            {
                await this.InsertAsync(entity).ConfigureAwait(false);
                return SaveResult.Inserted;
            }

            if (await this.UpdateAsync(entity, fieldsToUpdate).ConfigureAwait(false))
            {
                return SaveResult.Updated;
            }
            return SaveResult.NotModified;
        }

        public virtual void Insert(TEntity entity)
        {
            this.DataService.Insert(entity, EntityMetadata);
        }

        public virtual Task InsertAsync(TEntity entity)
        {
            return this.DataService.InsertAsync(entity, EntityMetadata);
        }

        public bool Update(TEntity entity)
        {
            return this.Update(entity, this.DataService.GetValidatedForUpdateSortedFields(entity));
        }

        protected virtual bool Update(TEntity entity, List<string> sortedFields)
        {
            return this.DataService.Update(entity, sortedFields);
        }

        public bool Update(TEntity entity, params string[] fieldsToUpdate)
        {
            if (fieldsToUpdate != null && fieldsToUpdate.Length == 0) return false;
            return this.Update(entity, this.DataService.GetValidatedForUpdateSortedFields(entity, fieldsToUpdate));
        }

        public Task<bool> UpdateAsync(TEntity entity)
        {
            return this.UpdateAsync(entity, this.DataService.GetValidatedForUpdateSortedFields(entity));
        }

        protected virtual Task<bool> UpdateAsync(TEntity entity, List<string> sortedFields)
        {
            return this.DataService.UpdateAsync(entity, sortedFields);
        }

        public Task<bool> UpdateAsync(TEntity entity, params string[] fieldsToUpdate)
        {
            if (fieldsToUpdate != null && fieldsToUpdate.Length == 0) return Task.FromResult(false);
            return this.UpdateAsync(entity, this.DataService.GetValidatedForUpdateSortedFields(entity, fieldsToUpdate));
        }

        public virtual bool Delete(TEntity entity)
        {
            return this.DataService.Delete(entity);
        }

        public virtual Task<bool> DeleteAsync(TEntity entity)
        {
            return this.DataService.DeleteAsync(entity);
        }

        public IQueryLite<TEntity> Query(Projection projection)
        {
            return new QueryLite<TEntity>(projection, this.DataService);
        }

        public IQueryLite<TEntity> Query(string projectionName)
        {
            return new QueryLite<TEntity>(projectionName, this.DataService);
        }

		TEntity IRepository<TEntity>.Get(Projection projection, object entityId, FetchMode fetchMode)
		{
			return ((IRepository<TEntity>)this).Get(projection.GetProjectionName(), entityId, fetchMode);
		}

		TEntity IRepository<TEntity>.Get(string projectionName, object entityId, FetchMode fetchMode)
		{
			TEntity entity = null;
			if (fetchMode == FetchMode.UseIdentityMap &&
				(entity = this.DataService.IdentityMap.Get<TEntity>(projectionName, entityId)) != null)
			{
				return entity;
			}
			string primaryKeyFieldName = typeof(TEntity).GetPrimaryKeyFieldName();
			entity = this.Query(projectionName).Get(primaryKeyFieldName, entityId);
			if (entity != null)
			{
				this.DataService.IdentityMap.Put(projectionName, entity);
			}
			return entity;
		}

        TEntity IRepository<TEntity>.Get(string projectionName, object entityId, string[] fields)
        {
            TEntity entity = null;
            if ((entity = DataService.IdentityMap.Get<TEntity>(projectionName, entityId)) != null)
            {
                return entity;
            }
            string primaryKeyFieldName = typeof(TEntity).GetPrimaryKeyFieldName();
            return this.Query(projectionName).Fields(fields).Get(primaryKeyFieldName, entityId);
        }

        TEntity IRepository<TEntity>.Get(Projection projection, object entityId, string[] fields)
        {
            return ((IRepository<TEntity>)this).Get(projection.GetProjectionName(), entityId, fields);
        }

#endregion

        Task<TEntity> IRepository<TEntity>.GetAsync(Projection projection, object entityId, FetchMode fetchMode)
        {
            return ((IRepository<TEntity>)this).GetAsync(projection.GetProjectionName(), entityId, fetchMode);
        }

        async Task<TEntity> IRepository<TEntity>.GetAsync(string projectionName, object entityId, FetchMode fetchMode)
        {
            TEntity entity = null;
            if (fetchMode == FetchMode.UseIdentityMap &&
                (entity = this.DataService.IdentityMap.Get<TEntity>(projectionName, entityId)) != null)
            {
                return await Task.FromResult(entity).ConfigureAwait(false);
            }
            string primaryKeyFieldName = typeof(TEntity).GetPrimaryKeyFieldName();
            entity = await this.Query(projectionName).GetAsync(primaryKeyFieldName, entityId).ConfigureAwait(false);
            if (entity != null)
            {
                this.DataService.IdentityMap.Put(projectionName, entity);
            }
            return entity;
        }

        async Task<TEntity> IRepository<TEntity>.GetAsync(string projectionName, object entityId, string[] fields)
        {
            TEntity entity = null;
            if ((entity = DataService.IdentityMap.Get<TEntity>(projectionName, entityId)) != null)
            {
                return await Task.FromResult(entity).ConfigureAwait(false);
            }
            string primaryKeyFieldName = typeof(TEntity).GetPrimaryKeyFieldName();
            return await this.Query(projectionName).Fields(fields).GetAsync(primaryKeyFieldName, entityId).ConfigureAwait(false);
        }


        Task<TEntity> IRepository<TEntity>.GetAsync(Projection projection, object entityId, string[] fields)
        {
            return ((IRepository<TEntity>)this).GetAsync(projection.GetProjectionName(), entityId, fields);
        }

#region IRepository Members

        IQueryLite IRepository.Query(Projection projection)
        {
            return this.Query(projection);
        }

        IQueryLite IRepository.Query(string projectionName)
        {
            return this.Query(projectionName);
        }

        SaveResult IRepository.Save(object entity)
        {
            return this.Save((TEntity)entity);
        }

        Task<SaveResult> IRepository.SaveAsync(object entity)
        {
            return this.SaveAsync((TEntity)entity);
        }

        bool IRepository.Update(object entity, string[] fieldsToUpdate)
        {
            return this.Update((TEntity)entity, fieldsToUpdate);
        }

        Task<bool> IRepository.UpdateAsync(object entity, string[] fieldsToUpdate)
        {
            return this.UpdateAsync((TEntity)entity, fieldsToUpdate);
        }

        bool IRepository.Delete(object entity)
        {
            return this.Delete((TEntity)entity);
        }

        Task<bool> IRepository.DeleteAsync(object entity)
        {
            return this.DeleteAsync((TEntity)entity);
        }


        void IRepository.Insert(object entity)
        {
            this.Insert((TEntity)entity);
        }

        Task IRepository.InsertAsync(object entity)
        {
            return this.InsertAsync((TEntity)entity);
        }


        bool IRepository.Update(object entity)
        {
            return this.Update((TEntity)entity);
        }

        Task<bool> IRepository.UpdateAsync(object entity)
        {
            return this.UpdateAsync((TEntity)entity);
        }

        object IRepository.Get(Projection projection, object entityId, FetchMode fetchMode)
		{
			return ((IRepository<TEntity>)this).Get(projection, entityId, fetchMode);
		}

		object IRepository.Get(string proyectionName, object entityId, FetchMode fetchMode)
		{
			return ((IRepository<TEntity>)this).Get(proyectionName, entityId, fetchMode);
		}

        object IRepository.Get(Projection projection, object entityId, string[] fields)
        {
            return ((IRepository<TEntity>)this).Get(projection, entityId, fields);
        }

        object IRepository.Get(string proyectionName, object entityId, string[] fields)
        {
            return ((IRepository<TEntity>)this).Get(proyectionName, entityId, fields);
        }       

        public Type EntityType
        {
            get { return typeof(TEntity); }
        }

        async Task<object> IRepository.GetAsync(Projection projection, object entityId, FetchMode fetchMode)
        {
            return await ((IRepository<TEntity>)this).GetAsync(projection, entityId, fetchMode).ConfigureAwait(false);
        }

        async Task<object> IRepository.GetAsync(string proyectionName, object entityId, FetchMode fetchMode)
        {
            return await ((IRepository<TEntity>)this).GetAsync(proyectionName, entityId, fetchMode).ConfigureAwait(false);
        }

        async Task<object> IRepository.GetAsync(Projection projection, object entityId, string[] fields)
        {
            return await ((IRepository<TEntity>)this).GetAsync(projection, entityId, fields).ConfigureAwait(false);
        }

        async Task<object> IRepository.GetAsync(string proyectionName, object entityId, string[] fields)
        {
            return await ((IRepository<TEntity>)this).GetAsync(proyectionName, entityId, fields).ConfigureAwait(false);
        }

#endregion
    }


}
