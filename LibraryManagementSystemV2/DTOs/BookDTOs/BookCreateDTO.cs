﻿using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace LibraryManagementSystemV2.DTOs.NewFolder1
{
    public class BookCreateDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required float Price { get; set; }
        public required int Quantity { get; set; } 
          
        public required ICollection<long> AuthorIDs { get; set; } 

        public ICollection<AuthorCreateDTO>? NewAuthors { get; set; }

        public static Book BookCreateDTOToBook(BookCreateDTO bookDTO) { 

            var book = new Book
            {
                Name = bookDTO.Name,
                Description = bookDTO.Description,
                Price = bookDTO.Price,
                Quantity = bookDTO.Quantity
            };

            return book;
        }

    }
}