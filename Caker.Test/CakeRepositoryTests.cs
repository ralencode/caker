using Xunit;
using Moq;
using Caker.Models;
using Caker.Repositories;
using Caker.Data;
using Microsoft.EntityFrameworkCore;

namespace Caker.Tests
{
    public class CakeRepositoryTests
    {
        [Fact]
        public async Task GetByConfectioner_ShouldReturnCakes_WhenCakesExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CakerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new CakerDbContext(options);
            var repository = new CakeRepository(context);

            var cake1 = new Cake { Id = 1, ConfectionerId = 10, Name = "Cake1" };
            var cake2 = new Cake { Id = 2, ConfectionerId = 10, Name = "Cake2" };
            await repository.Create(cake1);
            await repository.Create(cake2);

            // Act
            var result = await repository.GetByConfectioner(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Cake1");
            Assert.Contains(result, c => c.Name == "Cake2");
        }
    }
}
