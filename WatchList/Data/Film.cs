using System.ComponentModel.DataAnnotations;

namespace WatchList.Data
{
    public class Film
    {
        [Key]
        public int Id { get; set; }
        public string Titre { get; set; }
        public int Annee { get; set; }





    }
}
