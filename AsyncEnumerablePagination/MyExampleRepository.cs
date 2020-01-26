using System;
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

        public async IAsyncEnumerable<MyExampleEntity> AllWithPrefatchAsync()
        {
            const int pageSize = 10;

            double totalCount = await _context.Entities.CountAsync();
            int numberOfPages = (int)Math.Ceiling(totalCount / pageSize);

            if (numberOfPages == 0) yield break;

            var prefetchTask = _context.Entities
                .Take(pageSize)
                .ToListAsync();

            if (numberOfPages == 1)
            {
                var page = await prefetchTask;

                foreach (var entity in page)
                {
                    yield return entity;
                }

                yield break;
            }

            for (int i = 1; i <= numberOfPages; ++i)
            {
                var task = _context.Entities
                    .Skip(pageSize * i)
                    .Take(pageSize)
                    .ToListAsync();

                var page = await prefetchTask;
                prefetchTask = task;

                foreach (var entity in page)
                {
                    yield return entity;
                }
            }
        }

        public async IAsyncEnumerable<MyExampleEntity> AllWithPrefatchUnknownTotalSizeAsync()
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
