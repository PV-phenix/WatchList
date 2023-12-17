namespace ListeFilms.Data
{
    public class Utilisateur : Microsoft.AspNetCore.Identity.IdentityUser
    {
        public Utilisateur() : base() { this.ListeFilms = new HashSet<FilmUtilisateur>(); }

        public int Id { get; set; }
        public string Prenom { get; set; }
        public virtual ICollection<FilmUtilisateur> ListeFilms { get; set; }
    }
}
