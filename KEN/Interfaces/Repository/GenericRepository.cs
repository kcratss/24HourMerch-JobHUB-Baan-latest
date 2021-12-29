using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using KEN.Interfaces.Repository;
using KEN_DataAccess;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using KEN.AppCode;

namespace KEN.Interfaces.Repository
{

    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal KENNEWEntities _context;
        internal DbSet<TEntity> _dbSet;

        public GenericRepository(KENNEWEntities context)
        {
            this._context = context;
            this._dbSet = context.Set<TEntity>();
        }
        public virtual IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            var j = _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual bool Save()
        {
            try
            {
                return _context.SaveChanges() > 0;
            }

            catch (DbEntityValidationException e)
            {
                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format(
                        "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
                        Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime())), eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format(
                            "- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                //System.IO.File.AppendAllLines(@"c:\temp\errors.txt", outputLines);
                throw new ApplicationException(string.Join(",", outputLines));
            }
            catch (DbUpdateException e)
            {
                var outputLines = new List<string>();
                if (e.InnerException.InnerException.Message.Contains("UNIQUE KEY"))
                {
                    outputLines.Add("Already exists! : You cannot add  duplicate information.");
                }
                else if (e.InnerException.InnerException.Message.Contains("DELETE statement"))
                {
                    outputLines.Add("Failed! : You cannot Delete this record because it is used in another process.");
                }
                else
                {
                    outputLines.Add("Please handle this Internal Error !  : " + e.InnerException.InnerException.Message);
                }
                throw new HttpException(string.Join(",", outputLines));

            }
        }
    }
}