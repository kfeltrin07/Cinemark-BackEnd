using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Filmovi_projekt.Controllers;
using Filmovi_projekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Filmovi_project_testing
{
    [TestClass]
    public class FilmsControllerTests
    {
        private DbContextOptions<FilmsContext> _options;

        [TestInitialize]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<FilmsContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var context = new FilmsContext(_options))
            {
                context.Database.EnsureDeleted();
            }
        }

        [TestMethod]
        public async Task GetFilmovi_ReturnsListOfFilms()
        {
            // Arrange
            using (var context = new FilmsContext(_options))
            {
                context.Films.Add(new Films { id_film = 1, title = "Film 1", director = "Director 1", main_actor = "Actor 1", release_date = DateTime.Now, summary = "Summary 1", picture_url = "PictureURL 1", video_url = "VideoURL 1", total_rating = 0, rating_count = 0 });
                context.Films.Add(new Films { id_film = 2, title = "Film 2", director = "Director 2", main_actor = "Actor 2", release_date = DateTime.Now, summary = "Summary 2", picture_url = "PictureURL 2", video_url = "VideoURL 2", total_rating = 0, rating_count = 0 });
                context.SaveChanges();
            }

            using (var context = new FilmsContext(_options))
            {
                var controller = new FilmsController(context);

                // Act
                var result = await controller.GetFilmovi();

                // Assert
                Assert.IsNotNull(result.Value);
                var films = result.Value.ToList();
                Assert.AreEqual(2, films.Count);
                Assert.AreEqual("Film 1", films[0].title);
                Assert.AreEqual("Film 2", films[1].title);
            }
        }

        [TestMethod]
        public async Task GetFilms_ReturnsFilmById()
        {
            // Arrange
            using (var context = new FilmsContext(_options))
            {
                context.Films.Add(new Films { id_film = 1, title = "Film 1", director = "Director 1", main_actor = "Actor 1", release_date = DateTime.Now, summary = "Summary 1", picture_url = "PictureURL 1", video_url = "VideoURL 1", total_rating = 0, rating_count = 0 });
                context.SaveChanges();
            }

            using (var context = new FilmsContext(_options))
            {
                var controller = new FilmsController(context);

                // Act
                var result = await controller.GetFilms(1);

                // Assert
                Assert.IsNotNull(result.Value);
                var film = result.Value;
                Assert.AreEqual("Film 1", film.title);
            }
        }

        [TestMethod]
        public async Task GetFilms_ReturnsNotFoundForInvalidId()
        {

            using (var context = new FilmsContext(_options))
            {
                var controller = new FilmsController(context);

                // Act
                var result = await controller.GetFilms(1);

                // Assert
                Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
            }
        }

        [TestMethod]
        public async Task PutFilms_ReturnsNoContentForSuccessfulUpdate()
        {
            // Arrange
            using (var context = new FilmsContext(_options))
            {
                context.Films.Add(new Films { id_film = 1, title = "Film 1", director = "Director 1", main_actor = "Actor 1", release_date = DateTime.Now, summary = "Summary 1", picture_url = "PictureURL 1", video_url = "VideoURL 1", total_rating = 0, rating_count = 0 });
                context.SaveChanges();
            }

            using (var context = new FilmsContext(_options))
            {
                var controller = new FilmsController(context);
                var updatedFilm = new Films { id_film = 1, title = "Updated Film", director = "Updated Director", main_actor = "Updated Actor", release_date = DateTime.Now, summary = "Updated Summary", picture_url = "Updated PictureURL", video_url = "Updated VideoURL", total_rating = 0, rating_count = 0 };

                // Act
                var result = await controller.PutFilms(1, updatedFilm);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
                var film = await context.Films.FindAsync(1);
                Assert.AreEqual("Updated Film", film.title);
                Assert.AreEqual("Updated Director", film.director);
                Assert.AreEqual("Updated Actor", film.main_actor);
                Assert.AreEqual("Updated Summary", film.summary);
                Assert.AreEqual("Updated PictureURL", film.picture_url);
                Assert.AreEqual("Updated VideoURL", film.video_url);
            }
        }

        [TestMethod]
        public async Task PutFilms_ReturnsBadRequestForMismatchedId()
        {
            // Arrange
            using (var context = new FilmsContext(_options))
            {
                context.Films.Add(new Films { id_film = 1, title = "Film 1", director = "Director 1", main_actor = "Actor 1", release_date = DateTime.Now, summary = "Summary 1", picture_url = "PictureURL 1", video_url = "VideoURL 1", total_rating = 0, rating_count = 0 });
                context.SaveChanges();
            }

            using (var context = new FilmsContext(_options))
            {
                var controller = new FilmsController(context);
                var updatedFilm = new Films { id_film = 2, title = "Updated Film", director = "Updated Director", main_actor = "Updated Actor", release_date = DateTime.Now, summary = "Updated Summary", picture_url = "Updated PictureURL", video_url = "Updated VideoURL", total_rating = 0, rating_count = 0 };

                // Act
                var result = await controller.PutFilms(1, updatedFilm);

                // Assert
                Assert.IsInstanceOfType(result, typeof(BadRequestResult));
                var film = await context.Films.FindAsync(1);
                Assert.AreEqual("Film 1", film.title);
                Assert.AreEqual("Director 1", film.director);
                Assert.AreEqual("Actor 1", film.main_actor);
                Assert.AreEqual("Summary 1", film.summary);
                Assert.AreEqual("PictureURL 1", film.picture_url);
                Assert.AreEqual("VideoURL 1", film.video_url);
            }
        }

        [TestMethod]
        public async Task PostFilms_ReturnsCreatedResponseAndAddsFilm()
        {
            // Arrange
            using (var context = new FilmsContext(_options))
            {
                // No films in the database
            }

            using (var context = new FilmsContext(_options))
            {
                var controller = new FilmsController(context);
                var newFilm = new Films { id_film = 1, title = "New Film", director = "New Director", main_actor = "New Actor", release_date = DateTime.Now, summary = "New Summary", picture_url = "New PictureURL", video_url = "New VideoURL", total_rating = 0, rating_count = 0 };

                // Act
                var result = await controller.PostFilms(newFilm);

                // Assert
                Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
                var createdResult = (CreatedAtActionResult)result.Result;
                Assert.AreEqual("GetFilms", createdResult.ActionName);
                Assert.AreEqual(1, createdResult.RouteValues["id"]);
                var film = await context.Films.FindAsync(1);
                Assert.AreEqual("New Film", film.title);
                Assert.AreEqual("New Director", film.director);
                Assert.AreEqual("New Actor", film.main_actor);
                Assert.AreEqual("New Summary", film.summary);
                Assert.AreEqual("New PictureURL", film.picture_url);
                Assert.AreEqual("New VideoURL", film.video_url);
            }
        }

        [TestMethod]
        public async Task DeleteFilms_ReturnsNoContentAndRemovesFilm()
        {
            // Arrange
            using (var context = new FilmsContext(_options))
            {
                context.Films.Add(new Films { id_film = 1, title = "Film 1", director = "Director 1", main_actor = "Actor 1", release_date = DateTime.Now, summary = "Summary 1", picture_url = "PictureURL 1", video_url = "VideoURL 1", total_rating = 0, rating_count = 0 });
                context.SaveChanges();
            }

            using (var context = new FilmsContext(_options))
            {
                var controller = new FilmsController(context);

                // Act
                var result = await controller.DeleteFilms(1);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
                var film = await context.Films.FindAsync(1);
                Assert.IsNull(film);
            }
        }

        [TestMethod]
        public async Task DeleteFilms_ReturnsNotFoundForInvalidId()
        {
            // Arrange
            using (var context = new FilmsContext(_options))
            {
                // No films in the database
            }

            using (var context = new FilmsContext(_options))
            {
                var controller = new FilmsController(context);

                // Act
                var result = await controller.DeleteFilms(1);

                // Assert
                Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            }
        }
    }
}
