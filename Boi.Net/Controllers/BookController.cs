using AutoMapper;
using Boi.Net.DTOs.BookDTOs;
using Boi.Net.Model;
using Boi.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boi.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        private readonly BookService _service;
        private readonly IMapper _mapper;

        public BookController(BookService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }


        // Get All Books
        [HttpGet("all-books")]
        public async Task<ActionResult<List<Book>>> GetAll(
            [FromQuery] string search = "",
            [FromQuery] string genre = "",
            [FromQuery] string author = "",
            [FromQuery] string isbn = "",
            [FromQuery] bool isavailable = true,
            [FromQuery] string sortBy = "date",
            [FromQuery] bool asc = true,
            [FromQuery] int pagecount = 1,
            [FromQuery] int pagesize = 10)
        {

            var allBooks = await _service.GetAllBooks(search, genre, author, isbn, isavailable, sortBy, asc, pagecount, pagesize);

            return Ok(allBooks);

        }


        
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetById(int id)
        {

            var book = await _service.GetBookById(id);

            if(book == null)
            {
                return NotFound("Book Not Found");
            }

            return Ok(book);

        }


        [HttpGet("isbn/{isbn}")]
        public async Task<ActionResult<Book>> GetByIsbn(string isbn)
        {

            var book = await _service.GetBookByISBN(isbn);

            if(book == null)
            {
                return NotFound("Book Not Found");
            }

            return Ok(book);

        }


        [Authorize]
        [HttpGet("my-wishlist")]
        public async Task<ActionResult<Book>> MyWishlist()
        {

            return Ok(new { Message="Here is My Wishlist" });

        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("add-book")]
        public async Task<ActionResult> Create([FromForm] CreateBookDto createBookDto)
        {

            var book = await _service.GetBookByISBN(createBookDto.ISBN!);

            Console.WriteLine("Book: ", book);

            if(book != null)
            {
                return BadRequest("Book already exists.");
            }

            var newBook = _mapper.Map<Book>(createBookDto);

            await _service.CreateBook(newBook, createBookDto.ImageFile);


            return Ok(newBook);

        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] UpdateBookDto bookUpdate)
        {

            var existingBook = await _service.GetBookById(id);

            if(existingBook == null)
            {
                return NotFound("Book Not Found.");
            }

            var updatedBook = _mapper.Map(bookUpdate, existingBook);

            await _service.UpdateBook(updatedBook, bookUpdate.ImageFile);

            return Ok(updatedBook);
            
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {

            var book = await _service.GetBookById(id);

            if(book == null)
            {
                return NotFound("Book Not Found");
            }

            await _service.DeleteBook(book);

            return Ok("Book Has Been Deleted");

        }


    }
}
