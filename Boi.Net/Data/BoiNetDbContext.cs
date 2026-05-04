using Boi.Net.Model;
using Microsoft.EntityFrameworkCore;

namespace Boi.Net.Data
{
    public class BoiNetDbContext: DbContext
    {

        public BoiNetDbContext(DbContextOptions<BoiNetDbContext> options): base(options)
        {
            
        }

        public DbSet<Book> Books { get; set; }

    }
}
