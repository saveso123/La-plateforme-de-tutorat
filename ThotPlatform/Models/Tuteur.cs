using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant un tuteur/enseignant
    /// </summary>
    public class Tuteur : Utilisateur
    {
        [StringLength(1000)]
        [Display(Name = "Biographie")]
        [DataType(DataType.MultilineText)]
        public string Biographie { get; set; }

        [StringLength(500)]
        [Display(Name = "Diplomes")]
        public string Diplomes { get; set; }

        [Display(Name = "Annees d'experience")]
        [Range(0, 50)]
        public int AnneesExperience { get; set; }

        [Display(Name = "Disponible pour rencontres physiques")]
        public bool DisponiblePhysique { get; set; }

        [Display(Name = "Tarif horaire rencontre physique")]
        [Range(0, 1000)]
        public decimal? TarifHorairePhysique { get; set; }

        [StringLength(500)]
        [Display(Name = "Adresse")]
        public string Adresse { get; set; }

        [StringLength(100)]
        [Display(Name = "Ville")]
        public string Ville { get; set; }

        [StringLength(10)]
        [Display(Name = "Code postal")]
        public string CodePostal { get; set; }

        [Display(Name = "Note moyenne")]
        [Range(0, 5)]
        public decimal NoteMoyenne { get; set; }

        [Display(Name = "Nombre d'evaluations")]
        public int NombreEvaluations { get; set; }

        [Display(Name = "Disponible actuellement")]
        public bool EstDisponible { get; set; }

        // Relations
        public virtual ICollection<TuteurDomaine> DomainesExpertise { get; set; }
        public virtual ICollection<Reponse> Reponses { get; set; }
        public virtual ICollection<SessionClavardage> SessionsClavardage { get; set; }
        public virtual ICollection<RencontrePhysique> RencontresPhysiques { get; set; }
        public virtual ICollection<Cours> Cours { get; set; }

        public Tuteur()
        {
            DomainesExpertise = new HashSet<TuteurDomaine>();
            Reponses = new HashSet<Reponse>();
            SessionsClavardage = new HashSet<SessionClavardage>();
            RencontresPhysiques = new HashSet<RencontrePhysique>();
            Cours = new HashSet<Cours>();
            EstDisponible = true;
            NoteMoyenne = 0;
            NombreEvaluations = 0;
        }
    }
}

