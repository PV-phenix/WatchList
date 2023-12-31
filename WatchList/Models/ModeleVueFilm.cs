﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WatchList.Models
{
    public class ModeleVueFilm
    {
        [Key]
        public int IdFilm { get; set; }
        public string Titre { get; set; }
        public int Annee { get; set; }
        public string? Genre { get; set; }
        public string? Producteur {  get; set; }
        public string? Acteurs {  get; set; }
        [DisplayName("Présent dans ma liste")]
        public bool PresentDansListe { get; set; }  
        public bool Vu { get; set; }
        public int? Note { get; set; }
        public string? Commentaire { get; set; }
    }
}
