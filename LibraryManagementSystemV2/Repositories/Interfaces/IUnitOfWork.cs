using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagementSystemV2.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        IGenericRepository<T> Repository<T>() where T : class;
        Task<IDbContextTransaction> StartTransactionAsync();
        IAuthorBookRepository AuthorBookRepository();
        RentalRepository RentalRepository();
    }
}
