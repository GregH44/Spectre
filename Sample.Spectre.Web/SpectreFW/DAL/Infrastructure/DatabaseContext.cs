using Microsoft.EntityFrameworkCore;

namespace SpectreFW.DAL.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
    }
}
