using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Models;


namespace KEN.Interfaces.Iservices
{
    public interface IServiceBase<TEntity> where TEntity : class
    {
        TEntity GetById(int id);
        IQueryable<TEntity> GetAll();

        bool Add(TEntity entity);
        bool Delete(int id);
        ResponseViewModel Update(TEntity entity);
        ResponseViewModel BatchTransaction(TEntity entitity, BatchOperation operation);
    }
}