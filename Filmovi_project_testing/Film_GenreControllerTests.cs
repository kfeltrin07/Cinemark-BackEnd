using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Filmovi_projekt.Controllers;
using Filmovi_projekt.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Filmovi_project_testing
{
    [TestClass]
    public class Film_GenreControllerTests
    {
        private DbContextOptions<Film_GenreContext> _dbContextOptions;
        private Film_GenreContext _dbContext;
        private Film_GenreController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _dbContextOptions = new DbContextOptionsBuilder<Film_GenreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new Film_GenreContext(_dbContextOptions);
            _controller = new Film_GenreController(_dbContext);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [TestMethod]
        public async Task GetFilm_Genre_ReturnsListOfFilm_Genre()
        {
            // Arrange
            _dbContext.Film_Genre.AddRange(
                new Film_Genre { id_field = 1, id_genre = 1, id_film = 1 },
                new Film_Genre { id_field = 2, id_genre = 2, id_film = 2 }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetFilm_Genre();

            // Assert
            Assert.IsNotNull(result.Value);
            var film_Genres = result.Value.ToList();
            Assert.AreEqual(2, film_Genres.Count);
            Assert.AreEqual(1, film_Genres[0].id_field);
            Assert.AreEqual(2, film_Genres[1].id_field);
        }

        [TestMethod]
        public async Task GetFilm_Genre_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetFilm_Genre(1);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetFilm_Genre_WithValidId_ReturnsFilm_Genre()
        {
            // Arrange
            var film_Genre = new Film_Genre { id_field = 1, id_genre = 1, id_film = 1 };
            _dbContext.Film_Genre.Add(film_Genre);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetFilm_Genre(1);

            // Assert
            Assert.IsNotNull(result.Value);
            var returnedFilm_Genre = result.Value;
            Assert.AreEqual(film_Genre.id_field, returnedFilm_Genre.id_field);
            Assert.AreEqual(film_Genre.id_genre, returnedFilm_Genre.id_genre);
            Assert.AreEqual(film_Genre.id_film, returnedFilm_Genre.id_film);
        }

        [TestMethod]
        public async Task PutFilm_Genre_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var film_Genre = new Film_Genre { id_field = 1, id_genre = 1, id_film = 1 };

            // Act
            var result = await _controller.PutFilm_Genre(2, film_Genre);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task PutFilm_Genre_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var film_Genre = new Film_Genre { id_field = 1, id_genre = 1, id_film = 1 };
            _dbContext.Film_Genre.Add(film_Genre);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.PutFilm_Genre(1, film_Genre);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task PostFilm_Genre_WithNullInput_ReturnsProblem()
        {
            // Act
            var result = await _controller.PostFilm_Genre(null);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var objectResult = (ObjectResult)result.Result;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task PostFilm_Genre_WithValidInput_ReturnsCreatedResponse()
        {
            // Arrange
            var film_Genre = new Film_Genre { id_field = 1, id_genre = 1, id_film = 1 };

            // Act
            var result = await _controller.PostFilm_Genre(film_Genre);

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdAtActionResult = (CreatedAtActionResult)result.Result;
            Assert.AreEqual("GetFilm_Genre", createdAtActionResult.ActionName);
        }

        [TestMethod]
        public async Task DeleteFilm_Genre_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteFilm_Genre(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteFilm_Genre_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var film_Genre = new Film_Genre { id_field = 1, id_genre = 1, id_film = 1 };
            _dbContext.Film_Genre.Add(film_Genre);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteFilm_Genre(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}

