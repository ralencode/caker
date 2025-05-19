using Xunit;
using Moq;
using Caker.Models;
using Caker.Repositories;
using Caker.Data;
using Microsoft.EntityFrameworkCore;

namespace Caker.Tests
{
    public class OrderRepositoryTests
    {
        [Fact]
        public async Task GetByCustomer_ShouldReturnOrders()
        {
            var options = new DbContextOptionsBuilder<CakerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new CakerDbContext(options);
            var repository = new OrderRepository(context);

            var order = new Order { Id = 1, CustomerId = 5, CakeId = 1 };
            await repository.Create(order);

            var result = await repository.GetByCustomer(5);

            Assert.Single(result);
            Assert.Equal(1, result.First().CakeId);
        }

    }
}
