using System.ComponentModel.DataAnnotations;

namespace ListeFilms.Data
{
    public class Film
    {
        [Key]
        public int Id { get; set; }
        public string Titre { get; set; }
        public int Annee { get; set; }
        public string Genre { get; set; }
    }
}
