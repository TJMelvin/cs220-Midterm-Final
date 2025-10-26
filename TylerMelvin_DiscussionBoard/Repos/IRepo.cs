using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TylerMelvin_DiscussionBoard.Repos
{
    public interface IRepo<T> where T : class
    {
        IQueryable<T> All();
        IQueryable<T> Search(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void SaveChanges();
    }
}
