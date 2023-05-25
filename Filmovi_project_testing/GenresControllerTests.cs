using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Filmovi_projekt.Controllers;
using Filmovi_projekt.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Filmovi_project_testing
{
    [TestClass]
    public class GenresControllerTests
    {
        private DbContextOptions<GenreContext> _options;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<GenreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var context = new GenreContext(_options))
            {
                context.Database.EnsureDeleted();
            }
        }

        [TestMethod]
        public async Task GetGenres_ReturnsListOfGenres()
        {
            // Arrange
            using (var context = new GenreContext(_options))
            {
                context.Genre.Add(new Genre { id_genre = 1, name = "Genre 1", description = "Description 1" });
                context.Genre.Add(new Genre { id_genre = 2, name = "Genre 2", description = "Description 2" });
                context.SaveChanges();
            }

            using (var context = new GenreContext(_options))
            {

                var controller = new GenresController(context);

                // Act
                var result = await controller.GetGenres();

                // Assert
                Assert.IsNotNull(result.Value);
                var genres = result.Value.ToList();
                Assert.AreEqual(2, genres.Count);
                Assert.AreEqual("Genre 1", genres[0].name);
                Assert.AreEqual("Genre 2", genres[1].name);
            }
        }

        [TestMethod]
        public async Task GetGenre_ReturnsCorrectGenre()
        {
            // Arrange
            using (var context = new GenreContext(_options))
            {
                context.Genre.Add(new Genre { id_genre = 1, name = "Genre 1", description = "Description 1" });
                context.Genre.Add(new Genre { id_genre = 2, name = "Genre 2", description = "Description 2" });
                context.SaveChanges();
            }

            using (var context = new GenreContext(_options))
            {
                var controller = new GenresController(context);

                // Act
                var result = await controller.GetGenre(1);

                // Assert
                Assert.IsNotNull(result.Value);
                var genre = result.Value;
                Assert.AreEqual("Genre 1", genre.name);
                Assert.AreEqual("Description 1", genre.description);
            }
        }

        [TestMethod]
        public async Task GetGenre_ReturnsNotFoundForInvalidId()
        {
            // Arrange
            using (var context = new GenreContext(_options))
            {
                context.Genre.Add(new Genre { id_genre = 1, name = "Genre 1", description = "Description 1" });
                context.SaveChanges();
            }

            using (var context = new GenreContext(_options))
            {
                var controller = new GenresController(context);

                // Act
                var result = await controller.GetGenre(2);

                // Assert
                Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
            }
        }

        [TestMethod]
        public async Task PutGenre_ReturnsNoContentForSuccessfulUpdate()
        {
            // Arrange
            using (var context = new GenreContext(_options))
            {
                context.Genre.Add(new Genre { id_genre = 1, name = "Genre 1", description = "Description 1" });
                context.SaveChanges();
            }

            using (var context = new GenreContext(_options))
            {
                var controller = new GenresController(context);

                // Act
                var genre = new Genre { id_genre = 1, name = "Updated Genre", description = "Updated Description" };
                var result = await controller.PutGenre(1, genre);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
                var updatedGenre = await context.Genre.FindAsync(1);
                Assert.AreEqual("Updated Genre", updatedGenre.name);
                Assert.AreEqual("Updated Description", updatedGenre.description);
            }
        }

        [TestMethod]
        public async Task PutGenre_ReturnsBadRequestForInvalidId()
        {
            // Arrange
            using (var context = new GenreContext(_options))
            {
                context.Genre.Add(new Genre { id_genre = 1, name = "Genre 1", description = "Description 1" });
                context.SaveChanges();
            }

            using (var context = new GenreContext(_options))
            {
                var controller = new GenresController(context);

                // Act
                var genre = new Genre { id_genre = 2, name = "Updated Genre", description = "Updated Description" };
                var result = await controller.PutGenre(2, genre);

                // Assert
                Assert.IsTrue(result is NotFoundResult);
            }
        }

        [TestMethod]
        public async Task PostGenre_ReturnsCreatedResponse()
        {
            // Arrange
            using (var context = new GenreContext(_options))
            {
                context.Genre.Add(new Genre { id_genre = 1, name = "Genre 1", description = "Description 1" });
                context.SaveChanges();
            }

            using (var context = new GenreContext(_options))
            {
                var controller = new GenresController(context);

                // Act
                var genre = new Genre { id_genre = 2, name = "Genre 2", description = "Description 2" };
                var result = await controller.PostGenre(genre);

                // Assert
                Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
                var createdGenre = await context.Genre.FindAsync(2);
                Assert.AreEqual("Genre 2", createdGenre.name);
                Assert.AreEqual("Description 2", createdGenre.description);
            }
        }

        [TestMethod]
        public async Task DeleteGenre_ReturnsNoContentForSuccessfulDeletion()
        {
            // Arrange
            using (var context = new GenreContext(_options))
            {
                context.Genre.Add(new Genre { id_genre = 1, name = "Genre 1", description = "Description 1" });
                context.SaveChanges();
            }

            using (var context = new GenreContext(_options))
            {
                var controller = new GenresController(context);

                // Act
                var result = await controller.DeleteGenre(1);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
                var deletedGenre = await context.Genre.FindAsync(1);
                Assert.IsNull(deletedGenre);
            }
        }

        [TestMethod]
        public async Task DeleteGenre_ReturnsNotFoundForInvalidId()
        {
            // Arrange
            using (var context = new GenreContext(_options))
            {
                context.Genre.Add(new Genre { id_genre = 1, name = "Genre 1", description = "Description 1" });
                context.SaveChanges();
            }

            using (var context = new GenreContext(_options))
            {
                var controller = new GenresController(context);

                // Act
                var result = await controller.DeleteGenre(2);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }
        }
    }
}
