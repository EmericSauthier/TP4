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
using NuGet.Packaging.Signing;

namespace ApiFilm.Controllers.Tests
{
    [TestClass()]
    public class UtilisateursControllerTests
    {
        private FilmRatingsDBContext _context;
        private UtilisateursController _controller;

        public UtilisateursControllerTests()
        {
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>().UseNpgsql("Server=localhost;port=5432;Database=FilmRatingsDB; uid=postgres; password=postgres;");
            _context = new FilmRatingsDBContext(builder.Options);
            _controller = new UtilisateursController(_context);
        }

        [TestMethod()]
        public void UtilisateursControllerTest_OK()
        {
            Assert.IsNotNull(_controller, "Le controlleur est null.");
        }

        [TestMethod()]
        public void GetUtilisateursTest_OK()
        {
            var expected = _context.Utilisateurs.ToList();

            var results = _controller.GetUtilisateurs().Result.Value;

            CollectionAssert.AreEqual(expected, results.ToList(), "Pas les mêmes listes");
        }

        [TestMethod()]
        public void GetUtilisateurByIdTest_OK()
        {
            Utilisateur expected = _context.Utilisateurs.Where(u => u.UtilisateurId == 1).First();

            var result = _controller.GetUtilisateurById(1).Result.Value;

            Assert.AreEqual(expected, result, "Pas les mêmes utilisateurs");
        }

        [TestMethod()]
        public void GetUtilisateurByIdTest_NONOK()
        {
            var result = _controller.GetUtilisateurById(0).Result.Result;

            Assert.AreEqual(StatusCodes.Status404NotFound, ((NotFoundResult)result).StatusCode, "Pas de code 404");
        }

        [TestMethod()]
        public void GetUtilisateurByEmailTest_OK()
        {
            Utilisateur expected = _context.Utilisateurs.Where(u => u.UtilisateurId == 1).First();

            var result = _controller.GetUtilisateurByEmail(expected.Mail).Result.Value;

            Assert.AreEqual(expected, result, "Pas les mêmes utilisateurs");
        }

        [TestMethod()]
        public void GetUtilisateurByEmailTest_NONOK()
        {
            var result = _controller.GetUtilisateurByEmail("rrichings1@aver.com").Result.Result;

            Assert.AreEqual(StatusCodes.Status404NotFound, ((NotFoundResult)result).StatusCode, "Pas de code 404");
        }

        [TestMethod()]
        public void PutUtilisateurTest_OK()
        {
            Utilisateur expected = _context.Utilisateurs.Where(u => u.UtilisateurId == 1).First();
            expected.Pwd = "Password1234€";

            var result = _controller.PutUtilisateur(1, expected).Result;
            Utilisateur resultUser = _controller.GetUtilisateurById(1).Result.Value;

            Assert.AreEqual(StatusCodes.Status204NoContent, ((NoContentResult)result).StatusCode, "Pas de code 204");
            Assert.AreEqual(expected, resultUser, "Pas les mêmes utilisateurs");
        }

        [TestMethod]
        public void PostUtilisateurTest_OK()
        {
            // Arrange
            // On s'arrange pour que le mail soit unique en concaténant un random ou un timestamp
            Utilisateur userAtester = new Utilisateur()
            {
                Nom = "MACHIN",
                Prenom = "Luc",
                Mobile = "0606070809",
                Mail = "machin" + DateTime.Now.ToString() + "@gmail.com",
                Pwd = "Toto1234!",
                Rue = "Chemin de Bellevue",
                CodePostal = "74940",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };
            // Act
            var result = _controller.PostUtilisateur(userAtester).Result; // .Result pour appeler la méthode async de manière synchrone, afin d'attendre l’ajout

            // Assert
            Utilisateur? userRecupere = _context.Utilisateurs.Where(u => u.Mail.ToUpper() == userAtester.Mail.ToUpper()).FirstOrDefault(); // On récupère l'utilisateur créé directement dans la BD grace à son mail unique
            // On ne connait pas l'ID de l’utilisateur envoyé car numéro automatique.
            // Du coup, on récupère l'ID de celui récupéré et on compare ensuite les 2 users
            userAtester.UtilisateurId = userRecupere.UtilisateurId;

            Assert.AreEqual(userRecupere, userAtester, "Utilisateurs pas identiques");
        }

        [TestMethod()]
        public void DeleteUtilisateurTest()
        {
            //Assert.Fail();
        }
    }
}