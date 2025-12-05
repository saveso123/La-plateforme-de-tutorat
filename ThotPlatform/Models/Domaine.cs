using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant un domaine d'expertise (Mathematiques, Physique, etc.)
    /// </summary>
    public class Domaine
    {
        [Key]
        public int DomaineId { get; set; }

        [Required(ErrorMessage = "Le nom du domaine est requis")]
        [StringLength(100)]
        [Display(Name = "Nom du domaine")]
        public string Nom { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Icone")]
        public string Icone { get; set; }

        [Display(Name = "Actif")]
        public bool EstActif { get; set; }

        // Relations
        public virtual ICollection<TuteurDomaine> TuteursDomaines { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<FAQ> FAQs { get; set; }

        public Domaine()
        {
            TuteursDomaines = new HashSet<TuteurDomaine>();
            Questions = new HashSet<Question>();
            FAQs = new HashSet<FAQ>();
            EstActif = true;
        }
    }

    /// <summary>
    /// Table de liaison Many-to-Many entre Tuteur et Domaine
    /// </summary>
    public class TuteurDomaine
    {
        [Key]
        public int TuteurDomaineId { get; set; }

        [Required]
        public int TuteurId { get; set; }

        [Required]
        public int DomaineId { get; set; }

        [Display(Name = "Niveau d'expertise")]
        public NiveauExpertise NiveauExpertise { get; set; }

        // Relations
        public virtual Tuteur Tuteur { get; set; }
        public virtual Domaine Domaine { get; set; }
    }

    /// <summary>
    /// Enumeration des niveaux d'expertise
    /// </summary>
    public enum NiveauExpertise
    {
        [Display(Name = "Debutant")]
        Debutant = 1,

        [Display(Name = "Intermediaire")]
        Intermediaire = 2,

        [Display(Name = "Avance")]
        Avance = 3,

        [Display(Name = "Expert")]
        Expert = 4
    }
}

