using Filmovi_projekt.Controllers;
using Filmovi_projekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmovi_project_testing
{
    [TestClass]
    public class BookmarkControllerTests
    {
        private DbContextOptions<BookmarkContext> _dbContextOptions;
        private BookmarkController _controller;
        private BookmarkContext _dbContext;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BookmarkContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase")
               .Options;
            _dbContext = new BookmarkContext(_dbContextOptions);
            _controller = new BookmarkController(_dbContext);

        }
        
        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetBookmarks_ReturnsListOfBookmarks()
        {
            // Arrange
            var bookmarks = new List<Bookmark>
            {
                new Bookmark { id_Bookmark = 1, id_user = 1, id_film = 1 },
                new Bookmark { id_Bookmark = 2, id_user = 1, id_film = 2 },
                new Bookmark { id_Bookmark = 3, id_user = 2, id_film = 1 }
            };

            _dbContext.Bookmark.AddRange(bookmarks);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.GetBookmarks();

            // Assert
            Assert.IsNotNull(result.Value);
            var resultList = result.Value.ToList();
            Assert.AreEqual(3, resultList.Count);
            Assert.AreEqual(1, resultList[0].id_Bookmark);
            Assert.AreEqual(2, resultList[1].id_Bookmark);
            Assert.AreEqual(3, resultList[2].id_Bookmark);

        }

        [TestMethod]
        public async Task GetBookmark_ExistingId_ReturnsBookmark()
        {
            // Arrange
            var bookmark = new Bookmark { id_Bookmark = 1, id_user = 1, id_film = 1 };
            _dbContext.Bookmark.Add(bookmark);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.GetBookmark(1);

            // Assert
            Assert.IsInstanceOfType(result.Value, typeof(Bookmark));
            var actionResult = (ActionResult<Bookmark>)result.Value;
            Assert.AreEqual(bookmark, actionResult.Value);
        }

        [TestMethod]
        public async Task GetBookmark_NonExistingId_ReturnsNotFoundResult()
        {
            // Act
            var result = await _controller.GetBookmark(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PutBookmark_InvalidId_ReturnsBadRequestResult()
        {
            // Arrange
            var bookmark = new Bookmark { id_Bookmark = 1, id_user = 1, id_film = 1 };
            _dbContext.Bookmark.Add(bookmark);
            _dbContext.SaveChanges();

            var updatedBookmark = new Bookmark { id_Bookmark = 2, id_user = 2, id_film = 2 };

            // Act
            var result = await _controller.PutBookmark(1, updatedBookmark);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task PostBookmark_ValidBookmark_ReturnsCreatedResult()
        {
            // Arrange
            var bookmark = new Bookmark { id_Bookmark = 1, id_user = 1, id_film = 1 };

            // Act
            var result = await _controller.PostBookmark(bookmark);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = (CreatedAtActionResult)result.Result;
            Assert.AreEqual("GetBookmark", createdResult.ActionName);
            Assert.AreEqual(bookmark, createdResult.Value);
        }

        [TestMethod]
        public async Task DeleteBookmark_ExistingId_ReturnsNoContentResult()
        {
            // Arrange
            var bookmark = new Bookmark { id_Bookmark = 1, id_user = 1, id_film = 1 };
            _dbContext.Bookmark.Add(bookmark);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.DeleteBookmark(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            Assert.IsNull(_dbContext.Bookmark.Find(1));
        }

        [TestMethod]
        public async Task DeleteBookmark_NonExistingId_ReturnsNotFoundResult()
        {
            // Act
            var result = await _controller.DeleteBookmark(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public async Task ByUser_ValidBookmark_ReturnsBookmark()
        {
            // Arrange
            var bookmark = new Bookmark { id_Bookmark = 1, id_user = 1, id_film = 1 };
            _dbContext.Bookmark.Add(bookmark);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.ByUser(bookmark);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<Bookmark>));
            var actionResult = (ActionResult<Bookmark>)result.Value;
            Assert.AreEqual(bookmark, actionResult.Value);
        }

        [TestMethod]
        public async Task ByUser_InvalidBookmark_ReturnsNotFoundResult()
        {
            // Arrange
            var bookmark = new Bookmark { id_user = 1, id_film = 1 };

            // Act
            var result = await _controller.ByUser(bookmark);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Check_ExistingBookmark_ReturnsOkResult()
        {
            // Arrange
            var bookmark = new Bookmark { id_user = 1, id_film = 1 };
            _dbContext.Bookmark.Add(bookmark);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.Check(bookmark);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(true, okResult.Value);
        }

        [TestMethod]
        public async Task Check_NonExistingBookmark_ReturnsNotFoundResult()
        {
            // Arrange
            var bookmark = new Bookmark { id_user = 1, id_film = 1 };

            // Act
            var result = await _controller.Check(bookmark);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.AreEqual(false, notFoundResult.Value);
        }

        [TestMethod]
        public async Task DeleteBookmarksOfUser_ExistingUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var bookmarks = new List<Bookmark>
            {
                new Bookmark { id_Bookmark = 1, id_user = userId, id_film = 1 },
                new Bookmark { id_Bookmark = 2, id_user = userId, id_film = 2 },
                new Bookmark { id_Bookmark = 3, id_user = 2, id_film = 1 }
            };

            _dbContext.Bookmark.AddRange(bookmarks);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.DeleteBookmarksOfUser(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual("Sucess", okResult.Value);
            Assert.IsFalse(_dbContext.Bookmark.Any(b => b.id_user == userId));
        }

        [TestMethod]
        public async Task DeleteBookmarksOfUser_NonExistingUserId_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 100;

            // Act
            var result = await _controller.DeleteBookmarksOfUser(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
