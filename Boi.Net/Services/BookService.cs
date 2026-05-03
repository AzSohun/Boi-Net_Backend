using Boi.Net.Data;

namespace Boi.Net.Services
{
    public class BookService
    {


        private readonly BoiNetDbContext _context;

        public BookService(BoiNetDbContext context)
        {

            _context = context;
            
        }

    }
}
