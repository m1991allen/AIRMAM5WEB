using AIRMAM5.DBEntity.DBEntity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.DBEntity.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        protected IDbSet<TEntity> dbSet { get; set; }

        public DbContext _context
        {
            get;
            set;
        }

        public GenericRepository()
            : this(new AIRMAM5DBEntities())
        {
        }

        public GenericRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this._context = context;

            this.dbSet = this._context.Set<TEntity>();
        }

        public GenericRepository(ObjectContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this._context = new DbContext(context, true);
        }

        /// <summary>
        /// 新增資料到指定實體
        /// </summary>
        /// <param name="instance">資料實體</param>
        /// <exception cref="System.ArgumentNullException">資料實體</exception>
        public void Create(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this._context.Set<TEntity>().Add(instance);
                this.SaveChanges();
            }
        }

        /// <summary>
        /// 新增多筆資料到指定實體 CreateMultiple
        /// </summary>
        /// <param name="insertList"></param>
        public void CreateRange(List<TEntity> insertList)
        {
            if (insertList.Count() < 1)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this._context.Set<TEntity>().AddRange(insertList);
                this.SaveChanges();
            }
        }

        /// <summary>
        /// 更新資料到指定實體
        /// </summary>
        /// <param name="instance">資料實體</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Update(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this._context.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }

        /// <summary>
        /// 更新多筆資料到指定實體 UpdateMultiple
        /// </summary>
        /// <param name="updateList">資料實體</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UpdateMultiple(List<TEntity> updateList)
        {
            foreach (var instance in updateList)
            {
                this._context.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
            this.SaveChanges();
        }

        /// <summary>
        /// 由資料實體刪除資料
        /// </summary>
        /// <param name="instance">資料實體</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Delete(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this._context.Entry(instance).State = EntityState.Deleted;
                this.SaveChanges();
            }
        }

        /// <summary>
        /// 由資料實體刪除多筆資料 DeleteMultiple
        /// </summary>
        /// <param name="delList"></param>
        public void RemoveRange(List<TEntity> delList)
        {
            this._context.Set<TEntity>().RemoveRange(delList);
            this.SaveChanges();
        }

        /// <summary>
        /// 取得資料
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return this._context.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 根據條件尋找 TEntity
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return this.dbSet.Where(predicate);
        }

        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll()
        {
            return this._context.Set<TEntity>().AsQueryable();
        }

        /// <summary>
        /// 儲存
        /// </summary>
        public void SaveChanges()
        {
            this._context.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._context != null)
                {
                    this._context.Dispose();
                    this._context = null;
                }
            }
        }
    }
}
