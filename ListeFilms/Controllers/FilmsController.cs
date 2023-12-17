using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ListeFilms.Data;
using ListeFilms.Models;

namespace ListeFilms.Controllers
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
        public async Task<IActionResult> Index()
        {

            var idUtilisateur = await RecupererIdUtilisateurCourant();
            var modele = await _contexte.Films.Select(x =>
                    new ModeleVueFilm
                    {
                        IdFilm = x.Id,
                        Titre = x.Titre,
                        Annee = x.Annee,
                        Genre = x.Genre,
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

            return View(modele);
            //return _contexte.Films != null ?
            //              View(await _contexte.Films.ToListAsync()) :
            //              Problem("Entity set 'ApplicationDbContext.Films'  is null.");
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
        public async Task<IActionResult> Create([Bind("Id,Titre,Annee,Genre")] Film film)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Annee,Genre")] Film film)
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

    }
}
