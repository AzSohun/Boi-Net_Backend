using AutoMapper;
using Boi.Net.Model;
using Boi.Net.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Boi.Net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        private readonly BookService _sevice;
        private readonly IMapper _mapper;

        public BookController(BookService service, IMapper mapper)
        {
            _sevice = service;
            _mapper = mapper;
        }


        // Get All Books
        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAll(
            [FromQuery] string search = "",
            [FromQuery] string genre = "",
            [FromQuery] string author = "",
            [FromQuery] string isbn = "",
            [FromQuery] bool isavailable = true,
            [FromQuery] bool asc = true,
            [FromQuery] int pagecount = 1,
            [FromQuery] int pagesize = 10)
        {

            var allBooks = await _sevice.GetAllBooks(search, genre, author, isbn, isavailable, asc, pagecount, pagesize);

            return Ok(allBooks);

        }


    }
}
