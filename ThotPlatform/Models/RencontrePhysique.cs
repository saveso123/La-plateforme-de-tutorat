using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant une rencontre physique entre un etudiant et un tuteur
    /// </summary>
    public class RencontrePhysique
    {
        [Key]
        public int RencontreId { get; set; }

        [Required]
        public int EtudiantId { get; set; }

        [Required]
        public int TuteurId { get; set; }

        [Required]
        public int DomaineId { get; set; }

        [Required]
        [Display(Name = "Date et heure")]
        public DateTime DateHeure { get; set; }

        [Display(Name = "Duree (heures)")]
        [Range(0.5, 8)]
        public decimal DureeHeures { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Lieu de rencontre")]
        public string Lieu { get; set; }

        [StringLength(1000)]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Statut")]
        public StatutRencontre Statut { get; set; }

        [Display(Name = "Tarif")]
        public decimal Tarif { get; set; }

        [Display(Name = "Abonne (tarif preferentiel)")]
        public bool TarifPreferentiel { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Date de confirmation")]
        public DateTime? DateConfirmation { get; set; }

        [Display(Name = "Note de l'etudiant")]
        [Range(1, 5)]
        public int? NoteEtudiant { get; set; }

        [StringLength(500)]
        [Display(Name = "Commentaire")]
        public string Commentaire { get; set; }

        // Relations
        public virtual Etudiant Etudiant { get; set; }
        public virtual Tuteur Tuteur { get; set; }
        public virtual Domaine Domaine { get; set; }

        public RencontrePhysique()
        {
            DateCreation = DateTime.Now;
            Statut = StatutRencontre.EnAttente;
            DureeHeures = 1;
        }

        [NotMapped]
        [Display(Name = "Cout total")]
        public decimal CoutTotal => Tarif * DureeHeures;
    }

    /// <summary>
    /// Enumeration des statuts de rencontre
    /// </summary>
    public enum StatutRencontre
    {
        [Display(Name = "En attente de confirmation")]
        EnAttente = 1,

        [Display(Name = "Confirmee")]
        Confirmee = 2,

        [Display(Name = "En cours")]
        EnCours = 3,

        [Display(Name = "Terminee")]
        Terminee = 4,

        [Display(Name = "Annulee")]
        Annulee = 5
    }
}

