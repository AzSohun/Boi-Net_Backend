using AutoMapper;
using Boi.Net.Data;
using Boi.Net.Model;
using Microsoft.EntityFrameworkCore;

namespace Boi.Net.Services
{
    public class BookService
    {

        private readonly BoiNetDbContext _context;
        private readonly IPhotoService _photoService;

        public BookService(BoiNetDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }


        // To Get All The Books
        public async Task<List<Book>> GetAllBooks(
            string? searchTitle,
            string? filterGenre,
            string? filterAuthor,
            string? filterIsbn,
            bool? filterIsAvailable,
            string sortBy,
            bool asc,
            int pageCount = 1,
            int pageSize = 10)
        {

            // Using AsNoTracking Can Reduced 40% of Memory Using. 
            var query = _context.Books.AsNoTracking().AsQueryable();


            // Search
            if (!string.IsNullOrWhiteSpace(searchTitle))
            {
                query = query.Where(book => book.Title.Contains(searchTitle));
            }


            // Filter
            if (!string.IsNullOrWhiteSpace(filterGenre))
            {
                query = query.Where(book => book.Genre == filterGenre);
            }

            if (!string.IsNullOrWhiteSpace(filterAuthor))
            {
                query = query.Where(book => book.Author == filterAuthor);
            }

            if (!string.IsNullOrWhiteSpace(filterIsbn))
            {
                query = query.Where(book => book.ISBN == filterIsbn);
            }


            // For Hide the Stock Out Product
            //if (filterIsAvailable.HasValue)
            //{
            //    query = query.Where(book => book.IsAvailable == filterIsAvailable.Value);
            //}


            // Sorting Logic
            query = sortBy.ToLower() switch
            {
                "title" => asc ? query.OrderBy(book => book.Title) : query.OrderByDescending(book => book.Title),
                "price" => asc ? query.OrderBy(book => book.Price) : query.OrderByDescending(book => book.Price),
                _ => asc ? query.OrderBy(book => book.CreatedAt) : query.OrderByDescending(book => book.CreatedAt)
            };
            // query = asc ? query.OrderBy(b => b.CreatedAt) : query.OrderByDescending(book => book.CreatedAt);


            var books = await query.Skip((pageCount - 1) * pageSize).Take(pageSize).ToListAsync();

            return books;
        }


        // To Get Book By Id
        public async Task<Book?> GetBookById(int id)
        {

            var book = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);

            return book;

        }

        // To Get Book By ISBN
        public async Task<Book?> GetBookByISBN(string ISBN)
        {

            var book = await _context.Books.FirstOrDefaultAsync(book => book.ISBN == ISBN);

            return book;

        }


        // To Create Book
        public async Task CreateBook(Book newBook, IFormFile? imageFile)
        {

            if(imageFile != null)
            {

                var uploadImageUrl = await _photoService.AddPhotoAsync(imageFile);

                if(uploadImageUrl.Error != null)
                {
                    throw new Exception(uploadImageUrl.Error.Message);
                }

                newBook.CoverPhoto = uploadImageUrl.SecureUrl.ToString();
                newBook.CoverPublicId = uploadImageUrl.PublicId;

            }

            await _context.Books.AddAsync(newBook);
            await _context.SaveChangesAsync();
        }


        // To Update Book
        public async Task UpdateBook(Book book, IFormFile? imageFile)
        {

            if(imageFile != null)
            {

                if (!string.IsNullOrWhiteSpace(book.CoverPublicId))
                {
                    await _photoService.DeletePhotoAsync(book.CoverPublicId);
                }

                var uploadImageUrl = await _photoService.AddPhotoAsync(imageFile);

                if (uploadImageUrl.Error != null)
                {
                    throw new Exception(uploadImageUrl.Error.Message);
                }

                book.CoverPhoto = uploadImageUrl.SecureUrl.ToString();
                book.CoverPublicId = uploadImageUrl.PublicId;

            }


            await _context.SaveChangesAsync();
        }


        // To Delete Book
        public async Task DeleteBook(Book existingBook)
        {
            _context.Remove(existingBook);
            await _context.SaveChangesAsync();

        }
    }
}
