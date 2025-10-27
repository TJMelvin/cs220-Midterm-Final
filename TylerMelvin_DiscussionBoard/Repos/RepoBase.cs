using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TylerMelvin_DiscussionBoard.Data;

namespace TylerMelvin_DiscussionBoard.Repos
{
    public class RepoBase<T> : IRepo<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public RepoBase(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DbSet<T> All() => _context.Set<T>();

        public IQueryable<T> Search(Expression<Func<T, bool>> predicate)
             => _context.Set<T>().Where(predicate);

        public void Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Set<T>().Update(entity);
        }

        public void Remove (T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Set<T>().Remove(entity);
        }

        public int SaveChanges() => _context.SaveChanges();

        void IRepo<T>.SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
