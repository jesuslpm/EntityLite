using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inercya.EntityLite.Extensions;

namespace inercya.EntityLite
{
    public interface IRepository
    {
        IQueryLite Query(Projection projection);
        IQueryLite Query(string projectionName);
        void Save(object entity);

        void Insert(object entity);

        void Update(object entity);
        void Delete(object entity);
        Type EntityType { get; }

		object Get(Projection projection, object entityId, FetchMode fetchMode = FetchMode.UseIdentityMap);
		object Get(string projectionName, object entityId, FetchMode fetchMode = FetchMode.UseIdentityMap);
        object Get(Projection projection, object entityId, string[] fields);
        object Get(string projectionName, object entityId, string[] fields);

    }

    public interface IRepository<TEntity> : IRepository 
    {
        void Save(TEntity entity);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        new IQueryLite<TEntity> Query(Projection projection);
        new IQueryLite<TEntity> Query(string projectionName);

		new TEntity Get(Projection projection, object entityId, FetchMode fetchMode = FetchMode.UseIdentityMap);
		new TEntity Get(string projectionName, object entityId, FetchMode fetchMode = FetchMode.UseIdentityMap);
        new TEntity Get(Projection projection, object entityId, string[] fields);
        new TEntity Get(string projectionName, object entityId, string[] fields);
    }


    public class Repository<TEntity> : IRepository<TEntity> where TEntity: class, new()
    {
		public DataService DataService { get;  set; }

        public Repository(DataService dataService)
        {
            this.DataService = dataService;
        }

        #region IRepository<TEntity> Members

        public virtual void Save(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            Type entityType = entity.GetType();
            object id = entity.GetId();
            var idTypeCode = Type.GetTypeCode(id.GetType());
            bool isNew = false;
            if (idTypeCode == TypeCode.Int16 || idTypeCode == TypeCode.Int32 || idTypeCode == TypeCode.Int64)
            {
                long longId = Convert.ToInt64(id);
                if (longId <= 0) isNew = true;
            }
            else if (id is Guid)
            {
                if (object.Equals(id, Guid.Empty))
                {
                    isNew = true;
                }
            }
            else
            {
                throw new NotSupportedException("Primary key type not supported for save");
            }
           
            if (isNew) this.Insert(entity);
            else this.Update(entity);
        }

        public virtual void Insert(TEntity entity)
        {
            this.DataService.Insert(entity);
        }

        public virtual void Update(TEntity entity)
        {
            this.DataService.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            this.DataService.Delete(entity);
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

        #region IRepository Members

        IQueryLite IRepository.Query(Projection projection)
        {
            return this.Query(projection);
        }

        IQueryLite IRepository.Query(string projectionName)
        {
            return this.Query(projectionName);
        }

        void IRepository.Save(object entity)
        {
            this.Save((TEntity)entity);
        }

        void IRepository.Delete(object entity)
        {
            this.Delete((TEntity)entity);
        }

        void IRepository.Insert(object entity)
        {
            this.Insert((TEntity)entity);
        }

        void IRepository.Update(object entity)
        {
            this.Update((TEntity)entity);
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

        #endregion
    }
}
