using Xunit;
using Moq;
using Caker.Models;
using Caker.Repositories;
using Caker.Data;
using Microsoft.EntityFrameworkCore;

namespace Caker.Tests
{
    public class MessageRepositoryTests
    {
        [Fact]
        public async Task GetMessagesBetweenUsers_ShouldReturnMessages()
        {
            var options = new DbContextOptionsBuilder<CakerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new CakerDbContext(options);
            var repository = new MessageRepository(context);

            var message = new Message { Id = 1, FromId = 1, ToId = 2, Content = "Hello" };
            await repository.Create(message);

            var result = await repository.GetMessagesBetweenUsers(1, 2);

            Assert.Single(result);
            Assert.Equal("Hello", result.First().Content);
        }

    }
}
