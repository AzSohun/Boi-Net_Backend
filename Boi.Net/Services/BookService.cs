using AutoMapper;
using Boi.Net.Data;
using Boi.Net.Model;
using Microsoft.EntityFrameworkCore;

namespace Boi.Net.Services
{
    public class BookService
    {

        private readonly BoiNetDbContext _context;

        public BookService(BoiNetDbContext context)
        {
            _context = context;
        }


        // To Get All The Books
        public async Task<List<Book>> GetAll(
            string? searchTitle, 
            string? filterGenre, 
            string? filterAuthor, 
            string? filterIsbn,
            decimal? filerPrice,
            bool? filterIsAvailable,
            DateOnly? filterPublishDate,
            int pageCount = 1,
            int pageSize = 10)
        {

            var query = _context.Books.AsQueryable();


            // Search
            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                query = _context.Books.Where(book => book.Title.Contains(searchTitle));
            }


            // Filter
            if (!string.IsNullOrWhiteSpace(filterGenre))
            {
                query = _context.Books.Where(book => book.Genre == filterGenre);
            }

            if (!string.IsNullOrWhiteSpace(filterAuthor))
            {
                query = _context.Books.Where(book => book.Author == filterAuthor);
            }

            if (!string.IsNullOrWhiteSpace(filterIsbn))
            {
                query = _context.Books.Where(book => book.ISBN == filterIsbn);
            }

            if (filerPrice < 0)
            {
                query = _context.Books.Where(book => book.Price == filerPrice);
            }

            if ((bool)filterIsAvailable!)
            {
                query = _context.Books.Where(book => book.IsAvailable == filterIsAvailable); 
            }


            var books = await query.Skip((pageCount - 1) * pageSize).Take(pageSize).ToListAsync();

            return books;
        }


        // To Get Book By Id
        public async Task<Book?> GetById(int id)
        {

            return await _context.Books.FirstOrDefaultAsync(book => book.Id == id);

        }

        // To Get Book By ISBN
        public async Task<Book?> GetBookByISBN(string ISBN)
        {

            return await _context.Books.FirstOrDefaultAsync(book => book.ISBN == ISBN);

        }


        // To Create Book
        public async Task CreateBook(Book book)
        {

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

        }


        // To Update Book
        public async Task<Book?> UpdateBook(int id, Book updatedBook)
        {

            var existingBook = _context.Books.FirstOrDefaultAsync(book => book.Id == id);

            if(existingBook != null) {

                await _context.SaveChangesAsync();

                return updatedBook;
            
            }

            return null;  
        }


        // To Delete Book
        public async Task<string> DeleteBook(int id)
        {
            var existingBook = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);

            if(existingBook != null)
            {
                _context.Remove(existingBook);
                await _context.SaveChangesAsync();

                return "Book Has Been Deleted.";
            }

            return "Book Not Found.";
        }
    }
}
