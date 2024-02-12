using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApiFilm.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiFilm.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ApiFilm.Controllers.Tests
{
    [TestClass()]
    public class UtilisateursControllerTests
    {
        private UtilisateursController _controller;

        public UtilisateursControllerTests()
        {
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>().UseNpgsql("Server=localhost;port=5432;Database=FilmRatingsDB; uid=postgres; password=postgres;");
            FilmRatingsDBContext context = new FilmRatingsDBContext(builder.Options);
            _controller = new UtilisateursController(context);
        }

        [TestMethod()]
        public void UtilisateursControllerTest_OK()
        {
            Assert.IsNotNull(_controller, "Le controlleur est null.");
        }

        [TestMethod()]
        public void GetUtilisateursTest_OK()
        {
            var results = _controller.GetUtilisateurs().Result;

            Assert.IsNotNull(results.Value, "Pas de résultats.");
            Assert.AreEqual(12, results.Value.Count(), "Listes non égales");
        }

        [TestMethod()]
        public void GetUtilisateurByIdTest_OK()
        {
            var result = _controller.GetUtilisateurById(1).Result;

        }

        [TestMethod()]
        public void GetUtilisateurByIdTest_NONOK()
        {
            var result = _controller.GetUtilisateurById(0).Result.Result;

            Assert.AreEqual(StatusCodes.Status404NotFound, ((NotFoundResult)result).StatusCode, "Pas de code 404");
        }

        [TestMethod()]
        public void GetUtilisateurByEmailTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void PutUtilisateurTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void PostUtilisateurTest()
        {
            //Assert.Fail();
        }

        [TestMethod()]
        public void DeleteUtilisateurTest()
        {
            //Assert.Fail();
        }
    }
}