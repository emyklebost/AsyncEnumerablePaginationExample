using Microsoft.EntityFrameworkCore;

namespace AsyncEnumerablePagination
{
    public class MyExampleDbContext : DbContext
    {
        public MyExampleDbContext(DbContextOptions<MyExampleDbContext> options)
            : base(options)
        {}

        public DbSet<MyExampleEntity> Entities { get; set; }
    }
}
