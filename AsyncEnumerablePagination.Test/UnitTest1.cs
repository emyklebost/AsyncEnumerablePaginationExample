using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AsyncEnumerablePagination.Test
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(333)]
        public async Task AllAsyncTest(int numberOfEntities)
        {
            using var ctx = CreateTestDatabaseWithData(numberOfEntities);
            var sut = new MyExampleRepository(ctx);

            var list = new List<MyExampleEntity>();

            await foreach (var entity in sut.AllAsync())
            {
                list.Add(entity);
            }

            Assert.Equal(numberOfEntities, list.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(333)]
        public async Task AllWithPrefatchAsyncTest(int numberOfEntities)
        {
            using var ctx = CreateTestDatabaseWithData(numberOfEntities);
            var sut = new MyExampleRepository(ctx);

            var list = new List<MyExampleEntity>();

            await foreach (var entity in sut.AllWithPrefatchAsync())
            {
                list.Add(entity);
            }

            Assert.Equal(numberOfEntities, list.Count);
        }

        private static MyExampleDbContext CreateTestDatabaseWithData(int numberOfEntities)
        {
            var options = new DbContextOptionsBuilder<MyExampleDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var ctx = new MyExampleDbContext(options);

            for (int i = 1; i <= numberOfEntities; ++i)
            {
                var entity = new MyExampleEntity { Value = $"testvalue{i}" };
                ctx.Add(entity);
            }

            ctx.SaveChanges();

            return ctx;
        }
    }
}
