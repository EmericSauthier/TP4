using ApiFilm.Models.EntityFramework;
using ApiFilm.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFilm.Models.DataManager
{
    public class UtilisateurManager : IDataRepository<Utilisateur>
    {
        readonly FilmRatingsDBContext? filmsDbContext;
        public UtilisateurManager() { }
        public UtilisateurManager(FilmRatingsDBContext context)
        {
            filmsDbContext = context;
        }

        public ActionResult<IEnumerable<Utilisateur>> GetAll()
        {
            return filmsDbContext.Utilisateurs.ToList();
        }
        public ActionResult<Utilisateur> GetById(int id)
        {
            return filmsDbContext.Utilisateurs.FirstOrDefault(u => u.UtilisateurId == id);
        }
        public ActionResult<Utilisateur> GetByString(string mail)
        {
            return filmsDbContext.Utilisateurs.FirstOrDefault(u => u.Mail.ToUpper() == mail.ToUpper());
        }
        public void Add(Utilisateur entity)
        {
            filmsDbContext.Utilisateurs.Add(entity);
            filmsDbContext.SaveChanges();
        }
        public void Update(Utilisateur utilisateur, Utilisateur entity)
        {
            filmsDbContext.Entry(utilisateur).State = EntityState.Modified;
            utilisateur.UtilisateurId = entity.UtilisateurId;
            utilisateur.Nom = entity.Nom;
            utilisateur.Prenom = entity.Prenom;
            utilisateur.Mail = entity.Mail;
            utilisateur.Rue = entity.Rue;
            utilisateur.CodePostal = entity.CodePostal;
            utilisateur.Ville = entity.Ville;
            utilisateur.Pays = entity.Pays;
            utilisateur.Latitude = entity.Latitude;
            utilisateur.Longitude = entity.Longitude;
            utilisateur.Pwd = entity.Pwd;
            utilisateur.Mobile = entity.Mobile;
            utilisateur.NotesUtilisateur = entity.NotesUtilisateur;
            filmsDbContext.SaveChanges();
        }
        public void Delete(Utilisateur utilisateur)
        {
            filmsDbContext.Utilisateurs.Remove(utilisateur);
            filmsDbContext.SaveChanges();
        }
    }
}
