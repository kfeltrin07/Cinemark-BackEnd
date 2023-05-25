using Filmovi_projekt.Controllers;
using Filmovi_projekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Filmovi_project_testing
{
    [TestClass]
    public class RatingsControllerTests
    {
        private DbContextOptions<RatingContext> _dbContextOptions;
        private RatingContext _dbContext;
        private RatingsController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RatingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new RatingContext(_dbContextOptions);
            _controller = new RatingsController(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetRating_ReturnsAllRatings()
        {
            // Arrange
            var ratings = new[]
            {
                new Rating { id_rating = 1, id_film = 1, rating = 4, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 1 },
                new Rating { id_rating = 2, id_film = 2, rating = 3, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 2 },
                new Rating { id_rating = 3, id_film = 3, rating = 5, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 3 }
            };

            await _dbContext.Ratings.AddRangeAsync(ratings);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetRating();

            // Assert
            var ratingResult = result.Value;
            Assert.IsNotNull(ratingResult);
            Assert.AreEqual(ratings.Length, ratingResult.Count());
        }

        [TestMethod]
        public async Task GetRating_WithValidId_ReturnsRating()
        {
            // Arrange
            var ratingId = 1;
            var rating = new Rating { id_rating = ratingId, id_film = 1, rating = 4, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 1 };

            await _dbContext.Ratings.AddAsync(rating);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetRating(ratingId);

            // Assert
            var ratingResult = result.Value;
            Assert.IsNotNull(ratingResult);
            Assert.AreEqual(ratingId, ratingResult.id_rating);
        }

        [TestMethod]
        public async Task GetRating_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var ratingId = 100;

            // Act
            var result = await _controller.GetRating(ratingId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PutRating_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var ratingId = 1;
            var updatedRating = new Rating { id_rating = ratingId, id_film = 1, rating = 5, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 1 };

            await _dbContext.Ratings.AddAsync(updatedRating);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.PutRating(ratingId, updatedRating);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task PutRating_WithNonMatchingId_ReturnsBadRequest()
        {
            // Arrange
            var ratingId = 1;
            var updatedRating = new Rating { id_rating = 2, id_film = 1, rating = 5, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 1 };

            // Act
            var result = await _controller.PutRating(ratingId, updatedRating);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task PutRating_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var ratingId = 100;
            var updatedRating = new Rating { id_rating = ratingId, id_film = 1, rating = 5, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 1 };

            // Act
            var result = await _controller.PutRating(ratingId, updatedRating);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostRating_WithValidRating_ReturnsCreatedAtAction()
        {
            // Arrange
            var rating = new Rating { id_rating = 1, id_film = 1, rating = 4, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 1 };

            // Act
            ActionResult<Rating> actionResult = await _controller.PostRating(rating);
            var createdAtActionResult = (CreatedAtActionResult)actionResult.Result;

            // Assert
            Assert.AreEqual("GetRating", createdAtActionResult.ActionName);
            Assert.AreEqual(rating.id_rating, createdAtActionResult.RouteValues["id"]);
        }

        [TestMethod]
        public async Task DeleteRating_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var ratingId = 1;
            var rating = new Rating { id_rating = ratingId, id_film = 1, rating = 4, change_date = DateTime.Now, insert_date = DateTime.Now, id_user = 1 };

            await _dbContext.Ratings.AddAsync(rating);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteRating(ratingId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteRating_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var ratingId = 100;

            // Act
            var result = await _controller.DeleteRating(ratingId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
