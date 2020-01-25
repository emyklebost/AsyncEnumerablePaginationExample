using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AsyncEnumerablePagination
{
    public class MyExampleRepository
    {
        private readonly MyExampleDbContext _context;

        public MyExampleRepository(MyExampleDbContext context)
        {
            _context = context;
        }

        public async IAsyncEnumerable<MyExampleEntity> AllAsync()
        {
            const int pageSize = 10;

            for (int i = 0; ; ++i)
            {
                var hits = await _context.Entities
                    .Skip(pageSize * i)
                    .Take(pageSize)
                    .ToListAsync();

                if (!hits.Any()) break;

                foreach (var entity in hits)
                {
                    yield return entity;
                }
            }
        }
    }
}
