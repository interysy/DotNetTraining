using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagementSystemV2.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        IGenericRepository<T> Repository<T>() where T : class;

        Task<IDbContextTransaction> StartTransactionAsync();

        AuthorBookRepository AuthorBookRepository();

        RentalRepository RentalRepository();
    }
}
