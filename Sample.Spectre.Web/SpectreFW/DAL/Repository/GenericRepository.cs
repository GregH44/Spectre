using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpectreFW.DAL.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected DbContext context = null;
        protected DbSet<TEntity> dbSet = null;

        public GenericRepository(DbContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        public virtual void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual void Delete(object keyValue)
        {
            var entity = Find(keyValue);

            if (entity != null)
                Delete(entity);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> entities = dbSet.Where(predicate);
            if (entities.Any())
                Delete(entities);
        }

        public virtual void Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Any(predicate);
        }

        public virtual Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.AnyAsync(predicate);
        }

        public virtual TEntity Get(object keyValue)
        {
            return Find(keyValue);
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.AsNoTracking().FirstOrDefault();
        }

        public virtual async Task<IEnumerable<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? skip = default(int?), int? take = default(int?), params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).AsNoTracking().ToListAsync();
            }
            else
            {
                return await query.AsNoTracking().ToListAsync();
            }
        }

        public virtual int Save()
        {
            return context.SaveChanges();
        }

        public virtual Task<int> SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        public virtual void Update(TEntity entity)
        {
            dbSet.Update(entity);
        }

        public virtual void Update(IEnumerable<TEntity> entities)
        {
            dbSet.UpdateRange(entities);
        }

        private TEntity Find(object keyValue)
        {
            var entity = default(TEntity);
            var primaryKeyProperty = typeof(TEntity).GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).Any(a => a.GetType() == typeof(KeyAttribute)));

            if (primaryKeyProperty != null && primaryKeyProperty.PropertyType != keyValue.GetType())
                entity = dbSet.Find(Convert.ChangeType(keyValue, primaryKeyProperty.PropertyType));
            else
                entity = dbSet.Find(keyValue);

            return entity;
        }

        private Task<TEntity> FindAsync(object keyValue)
        {
            var entity = default(Task<TEntity>);
            var primaryKeyProperty = typeof(TEntity).GetProperties().FirstOrDefault(prop => prop.GetCustomAttributes(false).Any(a => a.GetType() == typeof(KeyAttribute)));

            if (primaryKeyProperty != null && primaryKeyProperty.PropertyType != keyValue.GetType())
                entity = dbSet.FindAsync(Convert.ChangeType(keyValue, primaryKeyProperty.PropertyType));
            else
                entity = dbSet.FindAsync(keyValue);

            return entity;
        }
    }
}
