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
using System.Reflection;
using System.Text.RegularExpressions;
using ApiFilm.Models.DataManager;
using ApiFilm.Models.Repository;
using Moq;

namespace ApiFilm.Controllers.Tests
{
    [TestClass()]
    public class UtilisateursControllerTests
    {
        private FilmRatingsDBContext _context;
        private UtilisateursController _controller;
        private IDataRepository<Utilisateur> _dataRepository;

        public UtilisateursControllerTests()
        {
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>().UseNpgsql("Server=localhost;port=5432;Database=FilmRatingsDB; uid=postgres; password=postgres;");
            _context = new FilmRatingsDBContext(builder.Options);
            _dataRepository = new UtilisateurManager(_context);
            _controller = new UtilisateursController(_dataRepository);
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
            var result = _controller.GetUtilisateurById(0).Result;

            Assert.IsNull(result.Value);
        }

        [TestMethod()]
        public void GetUtilisateurByEmailTest_OK()
        {
            Utilisateur expected = _context.Utilisateurs.Where(u => u.UtilisateurId == 1).First();

            var result = _controller.GetUtilisateurByEmail(expected.Mail).Result.Value;

            Assert.AreEqual(expected, result, "Pas les mêmes utilisateurs");
        }

        //[TestMethod()]
        //public void GetUtilisateurByEmailTest_NONOK()
        //{
        //    var result = _controller.GetUtilisateurByEmail("rrichings1@aver.com").Result.Result;

        //    Assert.AreEqual(StatusCodes.Status404NotFound, ((NotFoundResult)result).StatusCode, "Pas de code 404");
        //}

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
                Mail = "machin" + DateTime.UtcNow.ToString() + "@gmail.com",
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

        [TestMethod]
        public void PostUtilisateurTest_NONOK_Mobile()
        {
            // Arrange
            // On s'arrange pour que le mail soit unique en concaténant un random ou un timestamp
            Utilisateur utilisateur = new Utilisateur()
            {
                Nom = "MACHIN",
                Prenom = "Luc",
                Mobile = "1",
                Mail = "machin" + DateTime.Now.ToString() + "@gmail.com",
                Pwd = "Toto1234!",
                Rue = "Chemin de Bellevue",
                CodePostal = "74940",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };

            string PhoneRegex = @"^0[0-9]{9}$";
            Regex regex = new Regex(PhoneRegex);

            if (!regex.IsMatch(utilisateur.Mobile))
            {
                _controller.ModelState.AddModelError("Mobile", "Le n° de mobile doit contenir 10 chiffres"); //On met le même message que dans la classe Utilisateur.
            }

            var result = _controller.PostUtilisateur(utilisateur).Result;

            Assert.IsNull(result.Value, "Est pas null");
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)result.Result).StatusCode, "Pas de code 400");
        }

        [TestMethod]
        public void PostUtilisateurTest_NONOK_CodePostal()
        {
            // Arrange
            // On s'arrange pour que le mail soit unique en concaténant un random ou un timestamp
            Utilisateur utilisateur = new Utilisateur()
            {
                Nom = "MACHIN",
                Prenom = "Luc",
                Mobile = "0607080901",
                Mail = "machin" + DateTime.Now.ToString() + "@gmail.com",
                Pwd = "Toto1234!",
                Rue = "Chemin de Bellevue",
                CodePostal = "a",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };

            string cpRegex = @"^[0-9]{5}$";
            Regex regex = new Regex(cpRegex);

            if (!regex.IsMatch(utilisateur.CodePostal))
            {
                _controller.ModelState.AddModelError("Code Postal", "Le code postal doit contenir 5 chiffres."); //On met le même message que dans la classe Utilisateur.
            }

            var result = _controller.PostUtilisateur(utilisateur).Result;

            Assert.IsNull(result.Value, "Est pas null");
            Assert.AreEqual(StatusCodes.Status400BadRequest, ((BadRequestObjectResult)result.Result).StatusCode, "Pas de code 400");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteUtilisateurTest_OK()
        {
            Utilisateur user = new Utilisateur()
            {
                Nom = "MACHIN",
                Prenom = "Luc",
                Mobile = "0606070809",
                Mail = "machin" + DateTime.UtcNow.ToString() + "@gmail.com",
                Pwd = "Toto1234!",
                Rue = "Chemin de Bellevue",
                CodePostal = "74940",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };

            _context.Utilisateurs.Add(user);
            _context.SaveChanges();

            int id = _context.Utilisateurs.Where(u => u.Mail == user.Mail).First().UtilisateurId;

            var resultDelete = _controller.DeleteUtilisateur(id).Result;

            Assert.AreEqual(StatusCodes.Status204NoContent, ((NoContentResult)resultDelete).StatusCode, "Pas de code 204");
            _context.Utilisateurs.Where(u => u.UtilisateurId == id).First();
        }

        [TestMethod]
        public void Postutilisateur_ModelValidated_CreationOK_AvecMoq()
        {
            // Arrange
            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            var userController = new UtilisateursController(mockRepository.Object);
            Utilisateur user = new Utilisateur
            {
                Nom = "POISSON",
                Prenom = "Pascal",
                Mobile = "1",
                Mail = "poisson@gmail.com",
                Pwd = "Toto12345678!",
                Rue = "Chemin de Bellevue",
                CodePostal = "74940",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };
            // Act
            var actionResult = userController.PostUtilisateur(user).Result;
            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<Utilisateur>), "Pas un ActionResult<Utilisateur>");
            Assert.IsInstanceOfType(actionResult.Result, typeof(CreatedAtActionResult), "Pas un CreatedAtActionResult");
            var result = actionResult.Result as CreatedAtActionResult;
            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un Utilisateur");
            user.UtilisateurId = ((Utilisateur)result.Value).UtilisateurId;
            Assert.AreEqual(user, (Utilisateur)result.Value, "Utilisateurs pas identiques");
        }

        [TestMethod]
        public void GetUtilisateurById_ExistingIdPassed_ReturnsRightItem_AvecMoq()
        {
            // Arrange
            Utilisateur user = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F
            };

            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            mockRepository.Setup(x => x.GetByIdAsync(1).Result).Returns(user);
            var userController = new UtilisateursController(mockRepository.Object);

            // Act
            var actionResult = userController.GetUtilisateurById(1).Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Value);
            Assert.AreEqual(user, actionResult.Value as Utilisateur);
        }

        [TestMethod]
        public void GetUtilisateurById_UnknownIdPassed_ReturnsNotFoundResult_AvecMoq()
        {
            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            var userController = new UtilisateursController(mockRepository.Object);
            // Act
            var actionResult = userController.GetUtilisateurById(0).Result;
            // Assert
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));

        }

        [TestMethod]
        public void GetUtilisateurByEmail_OK_AvecMoq()
        {
            // Arrange
            Utilisateur user = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F
            };

            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            mockRepository.Setup(x => x.GetByStringAsync("clilleymd@last.fm").Result).Returns(user);
            var userController = new UtilisateursController(mockRepository.Object);

            // Act
            var actionResult = userController.GetUtilisateurByEmail("clilleymd@last.fm").Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.Value);
            Assert.AreEqual(user, actionResult.Value as Utilisateur);
        }

        [TestMethod]
        public void GetUtilisateurByEmail_NONOK_AvecMoq()
        {
            // Arrange
            Utilisateur user = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F
            };

            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            mockRepository.Setup(x => x.GetByStringAsync("clilleymd@last.fm").Result).Returns(user);
            var userController = new UtilisateursController(mockRepository.Object);

            // Act
            var actionResult = userController.GetUtilisateurByEmail("aifbafuba@last.fm").Result;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNull(actionResult.Value, "Est pas null");
            Assert.AreEqual(StatusCodes.Status404NotFound, ((NotFoundResult)actionResult.Result).StatusCode, "Pas de code 404");
        }

        [TestMethod]
        public void DeleteUtilisateurTest_AvecMoq()
        {
            // Arrange
            Utilisateur user = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F
            };
            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            mockRepository.Setup(x => x.GetByIdAsync(1).Result).Returns(user);
            var userController = new UtilisateursController(mockRepository.Object);
            // Act
            var actionResult = userController.DeleteUtilisateur(1).Result;
            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Pas un NoContentResult"); // Test du type de retour
        }

        [TestMethod]
        public void PutUtilisateurTest_AvecMoq()
        {
            // Arrange
            Utilisateur user = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Calida",
                Prenom = "Lilley",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F
            };
            Utilisateur userModif = new Utilisateur
            {
                UtilisateurId = 1,
                Nom = "Test",
                Prenom = "Update",
                Mobile = "0653930778",
                Mail = "clilleymd@last.fm",
                Pwd = "Toto12345678!",
                Rue = "Impasse des bergeronnettes",
                CodePostal = "74200",
                Ville = "Allinges",
                Pays = "France",
                Latitude = 46.344795F,
                Longitude = 6.4885845F
            };

            var mockRepository = new Mock<IDataRepository<Utilisateur>>();
            mockRepository.Setup(x => x.GetByIdAsync(1).Result).Returns(user);
            var userController = new UtilisateursController(mockRepository.Object);

            // Act
            var actionResult = userController.PutUtilisateur(1, userModif).Result;

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult), "Pas un NoContentResult"); // Test du type de retour
        }
    }
}