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

                if (!hits.Any()) yield break;

                foreach (var entity in hits)
                {
                    yield return entity;
                }
            }
        }

        public async IAsyncEnumerable<MyExampleEntity> AllWithPrefatchAsync()
        {
            const int pageSize = 10;

            var prefetchTask = _context.Entities
                .Take(pageSize)
                .ToListAsync();

            for (int i = 1; ; ++i)
            {
                var page = await prefetchTask;
                bool isLastpage = page.Count() < pageSize;

                if (!isLastpage)
                {
                    prefetchTask = _context.Entities
                        .Skip(pageSize * i)
                        .Take(pageSize)
                        .ToListAsync();
                }

                foreach (var entity in page)
                {
                    yield return entity;
                }

                if (isLastpage) yield break;
            }
        }
    }
}
