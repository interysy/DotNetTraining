using LibraryManagementSystemV2.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace LibraryManagementSystemV2.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryManagementContext _context;

        public UnitOfWork(LibraryManagementContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        } 

        public async Task<IDbContextTransaction> StartTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public IAuthorBookRepository AuthorBookRepository()
        {
            return new AuthorBookRepository(_context);

        }

        public RentalRepository RentalRepository()
        {
            return new RentalRepository(_context);

        }
    }
}
