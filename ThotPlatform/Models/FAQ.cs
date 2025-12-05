using System;
using System.ComponentModel.DataAnnotations;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant une entree FAQ (archivage des questions/reponses)
    /// </summary>
    public class FAQ
    {
        [Key]
        public int FAQId { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Required]
        [Display(Name = "Reponse")]
        [DataType(DataType.MultilineText)]
        public string Reponse { get; set; }

        [Required]
        public int DomaineId { get; set; }

        [Display(Name = "Nombre de consultations")]
        public int NombreConsultations { get; set; }

        [Display(Name = "Utile")]
        public int NombreUtile { get; set; }

        [Display(Name = "Non utile")]
        public int NombreNonUtile { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Date de derniere mise a jour")]
        public DateTime? DateMiseAJour { get; set; }

        [Display(Name = "Archivee")]
        public bool EstArchivee { get; set; }

        // Relations
        public virtual Domaine Domaine { get; set; }

        public FAQ()
        {
            DateCreation = DateTime.Now;
            NombreConsultations = 0;
            NombreUtile = 0;
            NombreNonUtile = 0;
            EstArchivee = false;
        }
    }
}
