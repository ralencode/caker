using Xunit;
using Moq;
using Caker.Models;
using Caker.Repositories;
using Caker.Data;
using Microsoft.EntityFrameworkCore;

namespace Caker.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task GetById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CakerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new CakerDbContext(options);
            var repository = new UserRepository(context);

            var user = new User { Id = 1, PhoneNumber = "12345" };
            await repository.Create(user);

            // Act
            var result = await repository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("12345", result.PhoneNumber);
        }
    }
}
