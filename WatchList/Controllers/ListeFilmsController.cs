using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WatchList.Data;
using WatchList.Models;

namespace WatchList.Controllers
{
    [Authorize]
    public class ListeFilmsController : Controller
    {
        private readonly ApplicationDbContext _contexte;
        private readonly UserManager<Utilisateur> _gestionnaire;

        public ListeFilmsController(ApplicationDbContext contexte, UserManager<Utilisateur> gestionnaire)
        {
            _contexte = contexte;
            _gestionnaire = gestionnaire;
        }

        private Task<Utilisateur> GetCurrentUserAsync() =>_gestionnaire.GetUserAsync(HttpContext.User);

        [HttpGet]
        public async Task<string?> RecupererIdUtilisateurCourant()
        {
            Utilisateur utilisateur = await GetCurrentUserAsync();
            return utilisateur?.Id;
        }


        public async Task<IActionResult> Index()
        {
            var id = await RecupererIdUtilisateurCourant();
            var filmsUtilisateur = _contexte.FilmsUtilisateur.Where(x => x.IdUtilisateur == id);
            var modele = filmsUtilisateur.Select(x => new ModeleVueFilm
            {
                IdFilm = x.IdFilm,
                Titre = x.Film.Titre,
                Annee = x.Film.Annee,
                Genre = x.Film.Genre,
                Producteur = x.Film.Producteur,
                Acteurs = x.Film.Acteurs,
                Vu = x.Vu,
                PresentDansListe = true,
                Note = x.Note,
                Commentaire = x.Commentaire

            }).ToList();

            return View(modele);
        }

        public async Task<JsonResult> AvezVousVu(int id, int val) 
        {

            var idUtilisateur = await RecupererIdUtilisateurCourant();
            var film = _contexte.FilmsUtilisateur.FirstOrDefault(x =>
                            x.Film.Id == id && x.IdUtilisateur == idUtilisateur);
            if (val == 1)
            {
                // s'il existe un enregistrement dans FilmsUtilisateur qui contient à la fois l'identifiant de l'utilisateur
                // et celui du film d, alors le film existe dans la liste de films et peut
                // être supprimé

                if (film != null)
                {
                    film.Vu = false;

                    val = 0;
                }
            }

            else
            {
                film.Vu = true;


                val = 1;
            }
            // nous pouvons maintenant enregistrer les changements dans la base de données
            await _contexte.SaveChangesAsync();
            // et renvoyer notre valeur de retour (-1, 0 ou 1) au script qui a appelé
            // cette méthode depuis la page Index

            return Json(val);
        }

        public async Task<IActionResult> AjoutNote(int id, int val)
        {
            var idUtilisateur = await RecupererIdUtilisateurCourant();
            var filmsUtilisateur = _contexte.FilmsUtilisateur.FirstOrDefault(x =>
                x.Film.Id == id && x.IdUtilisateur == idUtilisateur);

            filmsUtilisateur.Note = val;
            await _contexte.SaveChangesAsync();

            return View();
        }


    }
}
