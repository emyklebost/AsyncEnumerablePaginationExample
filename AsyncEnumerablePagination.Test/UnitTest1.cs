using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AsyncEnumerablePagination.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            const int numberOfEntities = 1000;
            using var ctx = CreateTestDatabaseWithData(numberOfEntities);
            var sut = new MyExampleRepository(ctx);

            var list = new List<MyExampleEntity>();

            await foreach (var entity in sut.AllAsync())
            {
                list.Add(entity);
            }

            Assert.Equal(numberOfEntities, list.Count);
        }

        private static MyExampleDbContext CreateTestDatabaseWithData(int numberOfEntities)
        {
            var options = new DbContextOptionsBuilder<MyExampleDbContext>()
                .UseInMemoryDatabase(databaseName: "test-database")
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