using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using inercya.EntityLite.Extensions;

namespace inercya.EntityLite
{
    public interface IRepository
    {
        IQueryLite CreateQuery(Projection projection);
        IQueryLite CreateQuery(string projectionName);
        void Save(object entity);
        void Delete(object entity);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        Type EntityType { get; }

		object Get(Projection projection, object entityId, FetchMode fetchMode = FetchMode.UseIdentityMap);
		object Get(string projectionName, object entityId, FetchMode fetchMode = FetchMode.UseIdentityMap);
        object Get(Projection projection, object entityId, string[] fields);
        object Get(string projectionName, object entityId, string[] fields);

    }

    public interface IRepository<TEntity> : IRepository 
    {
        void Save(TEntity entity);
        void Delete(TEntity entity);
        new IQueryLite<TEntity> CreateQuery(Projection projection);
        new IQueryLite<TEntity> CreateQuery(string projectionName);

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
                if (longId == 0) isNew = true;
            }
            else if (id is Guid)
            {
                if (object.Equals(id, Guid.Empty)) isNew = true;
            }
            else
            {
                throw new NotSupportedException("Primary key type not supported for save");
            }
           
            if (isNew) this.DataService.Insert(entity);
            else this.DataService.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            this.DataService.Delete(entity);
        }

        public IQueryLite<TEntity> CreateQuery(Projection projection)
        {
            return new QueryLite<TEntity>(projection, this.DataService);
        }

        public IQueryLite<TEntity> CreateQuery(string projectionName)
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
				ContextLite.SessionIdentityMap != null && 
				(entity = ContextLite.SessionIdentityMap.Get<TEntity>(projectionName, entityId)) != null)
			{
				return entity;
			}
			string primaryKeyFieldName = typeof(TEntity).GetPrimaryKeyFieldName();
			entity = this.CreateQuery(projectionName).Get(primaryKeyFieldName, entityId);
			if (entity != null && ContextLite.SessionIdentityMap != null)
			{
				ContextLite.SessionIdentityMap.Put(projectionName, entity);
			}
			return entity;
		}

        TEntity IRepository<TEntity>.Get(string projectionName, object entityId, string[] fields)
        {
            TEntity entity = null;
            if (ContextLite.SessionIdentityMap != null &&
                (entity = ContextLite.SessionIdentityMap.Get<TEntity>(projectionName, entityId)) != null)
            {
                return entity;
            }
            string primaryKeyFieldName = typeof(TEntity).GetPrimaryKeyFieldName();
            return this.CreateQuery(projectionName).Fields(fields).Get(primaryKeyFieldName, entityId);
        }

        TEntity IRepository<TEntity>.Get(Projection projection, object entityId, string[] fields)
        {
            return ((IRepository<TEntity>)this).Get(projection.GetProjectionName(), entityId, fields);
        }

        #endregion

        #region IRepository Members

        IQueryLite IRepository.CreateQuery(Projection projection)
        {
            return this.CreateQuery(projection);
        }

        IQueryLite IRepository.CreateQuery(string projectionName)
        {
            return this.CreateQuery(projectionName);
        }

        void IRepository.Save(object entity)
        {
            this.Save((TEntity)entity);
        }

        void IRepository.Delete(object entity)
        {
            this.Delete((TEntity)entity);
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

        public void BeginTransaction()
        {
            this.DataService.BeginTransaction();
        }

        public void CommitTransaction()
        {
			this.DataService.Commit();
        }

        public void RollbackTransaction()
        {
            this.DataService.Rollback();
        }

        public Type EntityType
        {
            get { return typeof(TEntity); }
        }

        #endregion
    }
}
