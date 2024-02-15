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
        private FilmRatingsDBContext _context;

        public UtilisateursControllerTests()
        {
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>().UseNpgsql("Server=localhost;port=5432;Database=FilmRatingsDB; uid=postgres; password=postgres;");
            _context = new FilmRatingsDBContext(builder.Options);
        }

        [TestMethod()]
        public void UtilisateursControllerTest_OK()
        {
            UtilisateursController controller = new UtilisateursController(_context);

            Assert.IsNotNull(controller, "Le controlleur est null.");
        }

        [TestMethod()]
        public void GetUtilisateursTest_OK()
        {
            UtilisateursController controller = new UtilisateursController(_context);
            var expected = _context.Utilisateurs.ToList();

            var results = controller.GetUtilisateurs().Result.Value;

            CollectionAssert.AreEqual(expected, results.ToList(), "Pas les mêmes listes");
        }

        [TestMethod()]
        public void GetUtilisateurByIdTest_OK()
        {
            UtilisateursController controller = new UtilisateursController(_context);
            Utilisateur expected = _context.Utilisateurs.Where(u => u.UtilisateurId == 1).First();

            var result = controller.GetUtilisateurById(1).Result.Value;

            Assert.AreEqual(expected, result, "Pas les mêmes utilisateurs");
        }

        [TestMethod()]
        public void GetUtilisateurByIdTest_NONOK()
        {
            UtilisateursController controller = new UtilisateursController(_context);

            var result = controller.GetUtilisateurById(0).Result.Result;

            Assert.AreEqual(StatusCodes.Status404NotFound, ((NotFoundResult)result).StatusCode, "Pas de code 404");
        }

        [TestMethod()]
        public void GetUtilisateurByEmailTest_OK()
        {
            UtilisateursController controller = new UtilisateursController(_context);
            Utilisateur expected = _context.Utilisateurs.Where(u => u.UtilisateurId == 1).First();

            var result = controller.GetUtilisateurByEmail(expected.Mail).Result.Value;

            Assert.AreEqual(expected, result, "Pas les mêmes utilisateurs");
        }

        [TestMethod()]
        public void GetUtilisateurByEmailTest_NONOK()
        {
            UtilisateursController controller = new UtilisateursController(_context);

            var result = controller.GetUtilisateurByEmail("a@a.fr").Status;

            //Assert.AreEqual(StatusCodes.Status404NotFound, ((NotFoundResult)result).StatusCode, "Pas de code 404");
        }

        [TestMethod()]
        public void PutUtilisateurTest_OK()
        {
            UtilisateursController controller = new UtilisateursController(_context);
            Utilisateur expected = _context.Utilisateurs.Where(u => u.UtilisateurId == 1).First();
            expected.Pwd = "Password1234€";

            var result = controller.PutUtilisateur(1, expected).Result;
            Utilisateur resultUser = controller.GetUtilisateurById(1).Result.Value;

            Assert.AreEqual(StatusCodes.Status204NoContent, ((NoContentResult)result).StatusCode, "Pas de code 204");
            Assert.AreEqual(expected, resultUser, "Pas les mêmes utilisateurs");
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