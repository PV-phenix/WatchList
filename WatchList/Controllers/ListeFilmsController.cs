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
        //public IActionResult Index()
        //{
        //    return View();
        //}

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
                Vu = x.Vu,
                PresentDansListe = true,
                Note = x.Note,
                Commentaire = x.Commentaire

            }).ToList();

            return View(modele);
        }



    }
}
