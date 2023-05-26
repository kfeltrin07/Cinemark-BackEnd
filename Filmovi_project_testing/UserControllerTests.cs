using Filmovi_projekt.Controllers;
using Filmovi_projekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filmovi_project_testing
{
    [TestClass]
    public class UserControllerTests
    {
        private DbContextOptions<UserContext> _dbContextOptions;
        private UserContext _dbContext;
        private UsersController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _dbContextOptions = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new UserContext(_dbContextOptions);
            _controller = new UsersController(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetLogins_ReturnsAllLogins()
        {
            // Arrange

            // Act
            var result = await _controller.Getlogins();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value.Count());
        }

        [TestMethod]
        public async Task GetLogin_WithValidId_ReturnsLogin()
        {
            // Arrange
            var login = new User { id_user = 1, username = "testuser", password = "password", email = "testuser@example.com" };
            await _dbContext.Users.AddAsync(login);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetLogin(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(login, result.Value);
        }

        [TestMethod]
        public async Task GetLogin_WithInvalidId_ReturnsNotFound()
        {
            // Arrange

            // Act
            var result = await _controller.GetLogin(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Result is NotFoundResult);
        }

        [TestMethod]
        public async Task PutLogin_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var login = new User { id_user = 1, username = "testuser", password = "password", email = "testuser@example.com" };
            await _dbContext.Users.AddAsync(login);
            await _dbContext.SaveChangesAsync();

            var updatedLogin = new User { id_user = 2, username = "updateduser", password = "newpassword", email = "updateduser@example.com" };

            // Act
            var result = await _controller.PutLogin(1, updatedLogin);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is BadRequestResult);
        }

        [TestMethod]
        public async Task PostLogin_WithValidLogin_ReturnsCreatedAtAction()
        {
            // Arrange
            var login = new User { id_user = 1, username = "testuser", password = "password", email = "testuser@example.com" };

            // Act
            var result = await _controller.PostLogin(login);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ActionResult<User>);
        }

        [TestMethod]
        public async Task DeleteLogin_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var login = new User { id_user = 1, username = "testuser", password = "password", email = "testuser@example.com" };
            await _dbContext.Users.AddAsync(login);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteLogin(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is NoContentResult);
        }

        [TestMethod]
        public async Task DeleteLogin_WithInvalidId_ReturnsNotFound()
        {
            // Arrange

            // Act
            var result = await _controller.DeleteLogin(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is NotFoundResult);
        }

    }
}
