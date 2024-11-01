﻿using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace LibraryManagementSystemV2.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly LibraryManagementContext _context;
        protected readonly DbSet<T> _dbSet; 
        public GenericRepository(LibraryManagementContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<T?> GetByIdAsync(long id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }


        public async Task DeleteByIdAsync(long id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);

            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
                await SaveAsync();
            }
        }
        public async Task<T?> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetAllAsync(bool tracked = true)
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? where = null, bool tracked = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (where != null)
            {
                query = query.Where(where);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void SetEntryStateToModified(T entry)
        {
             _context.Entry(entry).State = EntityState.Modified;
        }

        public void DeleteRange(IEnumerable<T> toRemove)
        {
            _context.RemoveRange(toRemove);
        }

        public bool Exists(Expression<Func<T, bool>> expression) { 
            return _dbSet.Any<T>(expression);
        }

        public async Task ExplicitlyLoad<TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> navigationPropertyPath) where TProperty : class
        {
            var entry = _context.Entry(entity);
            await entry.Collection(navigationPropertyPath).LoadAsync();
        }


    }
}
