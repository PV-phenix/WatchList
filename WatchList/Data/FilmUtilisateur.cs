namespace WatchList.Data
{
    public class FilmUtilisateur
    {
        public string? IdUtilisateur { get;set; }
        public int IdFilm { get; set; }
        public bool Vu { get; set; }
        public int Note { get; set; }
        public string? Commentaire { get; set; }
        public int FilmId { get; set; }

        public virtual Utilisateur? User { get; set; }
        public virtual Film? Film { get; set; }
     }
}