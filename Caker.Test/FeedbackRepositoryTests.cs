using Xunit;
using Moq;
using Caker.Models;
using Caker.Repositories;
using Caker.Data;
using Microsoft.EntityFrameworkCore;

namespace Caker.Tests
{
    public class FeedbackRepositoryTests
    {
        [Fact]
        public async Task GetByConfectioner_ShouldReturnFeedbacks()
        {
            var options = new DbContextOptionsBuilder<CakerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new CakerDbContext(options);
            var repository = new FeedbackRepository(context);

            var feedback = new Feedback { Id = 1, ConfectionerId = 7, Text = "Great Cake!" };
            await repository.Create(feedback);

            var result = await repository.GetByConfectioner(7);

            Assert.Single(result);
            Assert.Equal("Great Cake!", result.First().Text);
        }

    }
}
