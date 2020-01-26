using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async IAsyncEnumerable<MyExampleEntity> AllWithPrefatchAsync()
        {
            const int pageSize = 10;

            var prefetchTask = _context.Entities
                .Take(pageSize)
                .ToListAsync();

            for (int i = 1; ; ++i)
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var task = _context.Entities
                    .Skip(pageSize * i)
                    .Take(pageSize)
                    .ToListAsync(cancellationTokenSource.Token);

                var hits = await prefetchTask;

                if (hits.Any())
                {
                    prefetchTask = task;
                }
                else
                {
                    cancellationTokenSource.Cancel();
                    break;
                }

                foreach (var entity in hits)
                {
                    yield return entity;
                }
            }
        }
    }
}
