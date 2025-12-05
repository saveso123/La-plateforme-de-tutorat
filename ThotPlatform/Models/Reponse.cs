using System;
using System.ComponentModel.DataAnnotations;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant une reponse a une question
    /// </summary>
    public class Reponse
    {
        [Key]
        public int ReponseId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int TuteurId { get; set; }

        [Required(ErrorMessage = "Le contenu de la reponse est requis")]
        [Display(Name = "Reponse")]
        [DataType(DataType.MultilineText)]
        public string Contenu { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Fichier joint")]
        [StringLength(500)]
        public string FichierJoint { get; set; }

        [Display(Name = "Validee par l'etudiant")]
        public bool EstValidee { get; set; }

        [Display(Name = "Note")]
        [Range(1, 5)]
        public int? Note { get; set; }

        [StringLength(500)]
        [Display(Name = "Commentaire")]
        public string Commentaire { get; set; }

        // Relations
        public virtual Question Question { get; set; }
        public virtual Tuteur Tuteur { get; set; }

        public Reponse()
        {
            DateCreation = DateTime.Now;
            EstValidee = false;
        }
    }
}

