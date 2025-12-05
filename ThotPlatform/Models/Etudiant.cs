using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant un etudiant
    /// </summary>
    public class Etudiant : Utilisateur
    {
        [Required]
        [Display(Name = "Niveau scolaire")]
        public NiveauScolaire Niveau { get; set; }

        [StringLength(500)]
        [Display(Name = "Etablissement")]
        public string Etablissement { get; set; }

        [Display(Name = "Est abonne")]
        public bool EstAbonne { get; set; }

        [Display(Name = "Date debut abonnement")]
        public DateTime? DateDebutAbonnement { get; set; }

        [Display(Name = "Date fin abonnement")]
        public DateTime? DateFinAbonnement { get; set; }

        [StringLength(500)]
        [Display(Name = "Adresse")]
        public string Adresse { get; set; }

        [StringLength(100)]
        [Display(Name = "Ville")]
        public string Ville { get; set; }

        [StringLength(10)]
        [Display(Name = "Code postal")]
        public string CodePostal { get; set; }

        // Relations
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<SessionClavardage> SessionsClavardage { get; set; }
        public virtual ICollection<RencontrePhysique> RencontresPhysiques { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<InscriptionCours> InscriptionsCours { get; set; }

        public Etudiant()
        {
            Questions = new HashSet<Question>();
            SessionsClavardage = new HashSet<SessionClavardage>();
            RencontresPhysiques = new HashSet<RencontrePhysique>();
            Transactions = new HashSet<Transaction>();
            InscriptionsCours = new HashSet<InscriptionCours>();
            EstAbonne = false;
        }

        [NotMapped]
        [Display(Name = "Abonnement actif")]
        public bool AbonnementActif
        {
            get
            {
                if (!EstAbonne || !DateFinAbonnement.HasValue)
                    return false;
                
                // Verifier que la date fin est dans le futur
                return DateFinAbonnement.Value > DateTime.Now;
            }
        }

        /// <summary>
        /// Methode pour verifier si l'abonnement est actif a une date donnee
        /// Utile pour les requetes LINQ qui ne peuvent pas utiliser DateTime.Now
        /// </summary>
        public bool IsSubscribedAt(DateTime date)
        {
            return EstAbonne && 
                   DateFinAbonnement.HasValue && 
                   DateFinAbonnement.Value > date;
        }
    }

    /// <summary>
    /// Enumeration des niveaux scolaires
    /// </summary>
    public enum NiveauScolaire
    {
        [Display(Name = "Primaire")]
        Primaire = 1,
        
        [Display(Name = "Secondaire")]
        Secondaire = 2,
        
        [Display(Name = "Collegial")]
        Collegial = 3
    }
}

