using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Filmovi_projekt.Controllers;
using Filmovi_projekt.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Filmovi_project_testing
{
    [TestClass]
    public class CommentsControllerTests
    {
        private DbContextOptions<CommentsContext> _dbContextOptions;
        private CommentsContext _dbContext;
        private CommentsController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _dbContextOptions = new DbContextOptionsBuilder<CommentsContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .EnableSensitiveDataLogging() 
                .Options;
            _dbContext = new CommentsContext(_dbContextOptions);
            _controller = new CommentsController(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetComments_ReturnsListOfComments()
        {
            // Arrange
            _dbContext.Comments.Add(new Comments { id_comment = 1, comment = "Test Comment 1" });
            _dbContext.Comments.Add(new Comments { id_comment = 2, comment = "Test Comment 2" });
            _dbContext.SaveChanges();

            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.GetComments();

            // Assert
            Assert.IsNotNull(result.Value);
            var comments = result.Value.ToList();
            Assert.AreEqual(2, comments.Count);
            Assert.AreEqual("Test Comment 1", comments[0].comment);
            Assert.AreEqual("Test Comment 2", comments[1].comment);
        }

        [TestMethod]
        public async Task GetComments_ReturnsNotFound_WhenNoCommentsExist()
        {
            // Arrange
            _dbContext.Comments.RemoveRange(_dbContext.Comments);
            await _dbContext.SaveChangesAsync();

            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.GetComments();

            // Assert
            Assert.IsNull(result.Result);
        }

        [TestMethod]
        public async Task GetComments_ReturnsComment_WhenCommentExists()
        {
            // Arrange
            _dbContext.Comments.Add(new Comments { id_comment = 1, comment = "Test Comment 1" });
            _dbContext.SaveChanges();

            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.GetComments(1);

            // Assert
            Assert.IsNotNull(result.Value);
            var comment = result.Value;
            Assert.AreEqual(1, comment.id_comment);
            Assert.AreEqual("Test Comment 1", comment.comment);
        }

        [TestMethod]
        public async Task GetComments_ReturnsNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.GetComments(1);

            // Assert
            Assert.IsNull(result.Value);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PutComments_ReturnsNoContent_WhenCommentIsUpdated()
        {
            // Arrange
            _dbContext.Comments.Add(new Comments { id_comment = 1, comment = "Test Comment" });
            _dbContext.SaveChanges();

            var _controller = new CommentsController(_dbContext);

            // Act
            var commentToUpdate = await _dbContext.Comments.FindAsync(1);
            commentToUpdate.comment = "Updated Comment";

            var result = await _controller.PutComments(1, commentToUpdate);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var updatedComment = await _dbContext.Comments.FindAsync(1);
            Assert.AreEqual("Updated Comment", updatedComment.comment);
        }


        [TestMethod]
        public async Task PutComments_ReturnsBadRequest_WhenCommentIdDoesNotMatch()
        {
            // Arrange
            _dbContext.Comments.Add(new Comments { id_comment = 1, comment = "Test Comment" });
            _dbContext.SaveChanges();

            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.PutComments(2, new Comments { id_comment = 1, comment = "Updated Comment" });

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));

            var comment = await _dbContext.Comments.FindAsync(1);
            Assert.AreEqual("Test Comment", comment.comment);
        }

        [TestMethod]
        public async Task PutComments_ReturnsNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.PutComments(1, new Comments { id_comment = 1, comment = "Updated Comment" });

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostComments_ReturnsCreatedAtAction_WhenCommentIsCreated()
        {
            // Arrange
            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.PostComments(new Comments { id_comment = 1, comment = "New Comment" });

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdAtActionResult = (CreatedAtActionResult)result.Result;
            Assert.AreEqual("GetComments", createdAtActionResult.ActionName);
            var comment = (Comments)createdAtActionResult.Value;
            Assert.AreEqual(1, comment.id_comment);
            Assert.AreEqual("New Comment", comment.comment);
        }

        [TestMethod]
        public async Task DeleteComments_ReturnsNoContent_WhenCommentIsDeleted()
        {
            // Arrange
            _dbContext.Comments.Add(new Comments { id_comment = 1, comment = "Test Comment" });
            _dbContext.SaveChanges();

            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.DeleteComments(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var comment = await _dbContext.Comments.FindAsync(1);
            Assert.IsNull(comment);
        }

        [TestMethod]
        public async Task DeleteComments_ReturnsNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.DeleteComments(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteCommentsOfUser_ReturnsOk_WhenCommentsAreDeleted()
        {
            // Arrange
            var _controller = new CommentsController(_dbContext);

            // Act
            var result = await _controller.DeleteCommentsOfUser(1);

            // Assert
            Assert.AreEqual("Sucess", (result as OkObjectResult)?.Value);
        }

    }
}
