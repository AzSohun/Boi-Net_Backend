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
        public async Task<List<Book>> GetAllBooks(
            string? searchTitle,
            string? filterGenre,
            string? filterAuthor,
            string? filterIsbn,
            bool? filterIsAvailable,
            bool asc,
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

            if ((bool)filterIsAvailable!)
            {
                query = _context.Books.Where(book => book.IsAvailable == filterIsAvailable);
            }

            if ((bool)!asc)
            {
                query = query.OrderDescending();
            }

            var books = await query.Skip((pageCount - 1) * pageSize).Take(pageSize).ToListAsync();

            return books;
        }


        // To Get Book By Id
        public async Task<Book?> GetBookById(int id)
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
        public async Task<Book?> UpdateBook(Book updatedBook)
        {
            await _context.SaveChangesAsync();

            return updatedBook;
        }


        // To Delete Book
        public async Task<string> DeleteBook(Book existingBook)
        {
            _context.Remove(existingBook);
            await _context.SaveChangesAsync();

            return "Book Has Been Deleted.";
        }
    }
}
