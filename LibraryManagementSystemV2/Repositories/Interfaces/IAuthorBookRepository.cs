﻿using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.Repositories.Interfaces
{
    public interface IAuthorBookRepository
    {
        public abstract Task<IEnumerable<AuthorBook>> GetBookAuthors(long bookId);
    }
}
