using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystemV2Tests.Mocks
{
    internal class TransactionMock : IDbContextTransaction, IDisposable, IAsyncDisposable
    {
        private bool _disposed = false;

        public Guid TransactionId => throw new NotImplementedException();

        public bool SupportsSavepoints => throw new NotImplementedException();

        public void Commit()
        {
            return;
        }

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free any other managed objects here.
                }

                // Free any unmanaged objects here.
                _disposed = true;
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose(false);
            return ValueTask.CompletedTask;
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public Task RollbackAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void CreateSavepoint(string name)
        {
            throw new NotImplementedException();
        }

        public Task CreateSavepointAsync(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void RollbackToSavepoint(string name)
        {
            throw new NotImplementedException();
        }

        public Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void ReleaseSavepoint(string name)
        {
            throw new NotImplementedException();
        }

        public Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
