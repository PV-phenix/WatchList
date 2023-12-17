using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ListeFilms.Models
{
    public class ModeleVueFilm
    {
        [Key]
        public int IdFilm { get; set; }
        public string? Titre { get; set; }
        public string? Genre { get; set; }
        public int Annee { get; set; }
        [DisplayName("Présent dans ma liste")]
        public bool PresentDansListe { get; set; }  
        public bool Vu { get; set; }
        public int? Note { get; set; }
        public string? Commentaire { get; set; }
    }
}
