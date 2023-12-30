using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WatchList.Data;
using WatchList.Models;

namespace WatchList.Controllers
{
    [Authorize]  //cet mention indique que toute la classe doit être autorisée en accès par la
                 //connexion ou login sinon la rediriger vers la page de login

    public class FilmsController : Controller
    {
        private readonly ApplicationDbContext _contexte;
        private readonly UserManager<Utilisateur> _gestionnaire;
        static int val = -1;

        public FilmsController(ApplicationDbContext contexte, UserManager<Utilisateur> gestionnaire)
        {
            _contexte = contexte;
            _gestionnaire = gestionnaire;

        }

        [HttpGet]
        public async Task<string> RecupererIdUtilisateurCourant()
        {
            Utilisateur utilisateur = await GetCurrentUserAsync();
            return utilisateur?.Id;
        }

        private Task<Utilisateur> GetCurrentUserAsync() => _gestionnaire.GetUserAsync(HttpContext.User);

        // GET: Films
        public async Task<IActionResult> Index(string sortOrder, string? searchString,string SelectFilter)
        {

            var idUtilisateur = await RecupererIdUtilisateurCourant();
            var modele = await _contexte.Films.Select(x =>
                    new ModeleVueFilm
                    {
                            IdFilm = x.Id,
                            Titre = x.Titre,
                            Annee = x.Annee,
                            Genre = x.Genre,
                            Producteur = x.Producteur,
                            Acteurs = x.Acteurs
                    }).ToListAsync();
            foreach (var item in modele)
            {
                var m = await _contexte.FilmsUtilisateur.FirstOrDefaultAsync(x =>
                           x.IdUtilisateur == idUtilisateur && x.IdFilm == item.IdFilm);
                if (m != null)
                {
                    item.PresentDansListe = true;
                    item.Note = m.Note;
                    item.Vu = m.Vu;
                }
            }
            if (_contexte.Films == null)
            {
                return Problem("Entity set '_contexte.Films'  is null.");
            }

            //var movies = from m in _contexte.Films
            //             select m;

            if (!String.IsNullOrEmpty(searchString))
            {

                var model = SelectFilter switch
                {
                    "Titre" => await _contexte.Films.Where(s => s.Titre.Contains(searchString)).ToListAsync(),
                    "Année" => await _contexte.Films.Where(s => s.Annee.ToString().Contains(searchString)).ToListAsync(),
                    "Genre" => await _contexte.Films.Where(s => s.Genre.Contains(searchString)).ToListAsync(),
                    "Producteur" => await _contexte.Films.Where(s => s.Producteur.Contains(searchString)).ToListAsync(),
                    "Acteur" => await _contexte.Films.Where(s => s.Acteurs.Contains(searchString)).ToListAsync(),
                    _ => await _contexte.Films.Where(s => s.Titre.Contains(searchString)).ToListAsync()
                };
                
                
                
                //var model = await _contexte.Films.Where(s => s.Titre.Contains(searchString)).ToListAsync();

                ObservableCollection<ModeleVueFilm> Searchfilms = new ObservableCollection<ModeleVueFilm>();


                foreach (Film f in model)
                {
                    Searchfilms.Add(new ModeleVueFilm { IdFilm = f.Id, Titre = f.Titre, Annee = f.Annee, Genre = f.Genre, Producteur = f.Producteur, Acteurs = f.Acteurs });
                }


                return View(Searchfilms);
             }

            ViewData["TitreSortParm"] = String.IsNullOrEmpty(sortOrder) ? "titre_desc" : "";
            ViewData["AnneeSortParm"] = sortOrder == "Annee" ? "annee_desc" : "Annee";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["ProductSortParm"] = sortOrder == "Producteur" ? "product_desc" : "Producteur";
            ViewData["ActSortParm"] = sortOrder == "Acteurs" ? "act_desc" : "Acteurs";
            ViewData["FiltreActuel"] = searchString;

            var films = from f in _contexte.Films
                           select f;
            if (!String.IsNullOrEmpty(searchString))
            {
                films = films.Where(f => f.Titre.Contains(searchString)
                                       ||f.Annee.ToString().Contains(searchString)
                                       ||f.Genre.ToString().Contains(searchString)
                                       ||f.Producteur.Contains(searchString)
                                       ||f.Acteurs.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(sortOrder))
            {
                switch (sortOrder)
                {
                    case "titre_desc":
                        films = films.OrderByDescending(f => f.Titre);
                        break;
                    case "Annee":
                        films = films.OrderBy(f => f.Annee);
                        break;
                    case "annee_desc":
                        films = films.OrderByDescending(f => f.Annee);
                        break;
                      case "Genre":
                        films = films.OrderBy(f => f.Genre);
                        break;
                    case "genre_desc":
                        films = films.OrderByDescending(f => f.Genre);
                        break;
                    case "Producteur":
                        films = films.OrderBy(f => f.Producteur);
                        break;
                    case "product_desc":
                        films = films.OrderByDescending(f => f.Producteur);
                        break;
                    case "Acteurs":
                        films = films.OrderBy(f => f.Acteurs);
                        break;
                    case "act_desc":
                        films = films.OrderByDescending(f => f.Acteurs);
                        break;
                    default:
                        films = films.OrderBy(f => f.Titre);
                        break;
                }
                ObservableCollection<ModeleVueFilm> Films = new ObservableCollection<ModeleVueFilm>();
                foreach (Film f in films)
                {
                    Films.Add(new ModeleVueFilm { IdFilm = f.Id, Titre = f.Titre, Annee = f.Annee, Genre = f.Genre, Producteur = f.Producteur, Acteurs = f.Acteurs });
                }
                return View(Films);
            } else
           
            return View(modele);

        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _contexte.Films == null)
            {
                return NotFound();
            }

            var film = await _contexte.Films
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Annee,Genre,Producteur,Acteurs")] Film film)
        {
            if (ModelState.IsValid)
            {
                _contexte.Add(film);
                await _contexte.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _contexte.Films == null)
            {
                return NotFound();
            }

            var film = await _contexte.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Annee,Genre,Producteur,Acteurs")] Film film)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _contexte.Update(film);
                    await _contexte.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _contexte.Films == null)
            {
                return NotFound();
            }

            var film = await _contexte.Films
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_contexte.Films == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Films'  is null.");
            }
            var film = await _contexte.Films.FindAsync(id);
            if (film != null)
            {
                _contexte.Films.Remove(film);
            }
            
            await _contexte.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
          return (_contexte.Films?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet]
        public async Task<JsonResult> AjouterSupprimer(int id,int val)
        {
                
            var idUtilisateur = await RecupererIdUtilisateurCourant();
            if (val == 1)
            {
                // s'il existe un enregistrement dans FilmsUtilisateur qui contient à la fois l'identifiant de l'utilisateur
                // et celui du film, alors le film existe dans la liste de films et peut
                // être supprimé
                var film = _contexte.FilmsUtilisateur.FirstOrDefault(x =>
                        x.IdFilm == id && x.IdUtilisateur == idUtilisateur);
                if (film != null)
                {
                    _contexte.FilmsUtilisateur.Remove(film);
                    val = 0;
                }

            }
            else
            {
                // le film n'est pas dans la liste de films, nous devons donc
                // créer un nouvel objet FilmUtilisateur et l'ajouter à la base de données.

                //var originalMovie = (from m in _contexte.FilmsUtilisateur.ToList()

                //                     where m.IdFilm == id

                //                     select m).First();
                _contexte.FilmsUtilisateur.Add(
                   new FilmUtilisateur
                   {
                       IdUtilisateur = idUtilisateur,
                       IdFilm = id,
                       Vu = false,
                       Note = 0,
                       FilmId = _contexte.Films.Find(id).Id
                   });
                
                val = 1;
            }
            // nous pouvons maintenant enregistrer les changements dans la base de données
            await _contexte.SaveChangesAsync();
            // et renvoyer notre valeur de retour (-1, 0 ou 1) au script qui a appelé
            // cette méthode depuis la page Index
            
            return Json(val);
        }

        //public async Task<IActionResult> Index(string searchString,bool notUsed )
        //{
        //    if (_contexte.Films == null)
        //    {
        //        return Problem("Entity set '_contexte.Films'  is null.");
        //    }

        //    var movies = from m in _contexte.Films
        //                 select m;

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        movies = movies.Where(s => s.Titre!.Contains(searchString));
        //    }

        //    return View(await movies.ToListAsync());
        //}

    }
}
