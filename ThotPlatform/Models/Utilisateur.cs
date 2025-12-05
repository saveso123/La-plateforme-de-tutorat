using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Classe de base pour tous les utilisateurs de la plateforme
    /// </summary>
    public abstract class Utilisateur
    {
        [Key]
        public int UtilisateurId { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(100)]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prenom est requis")]
        [StringLength(100)]
        [Display(Name = "Prenom")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [StringLength(255)]
        [Index(IsUnique = true)]
        [Display(Name = "Adresse courriel")]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Identifiant")]
        public string Username { get; set; }

        [StringLength(20)]
        [Display(Name = "Identifiant unique")]
        public string IdentifiantUnique { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; }

        [StringLength(10)]
        [Display(Name = "Langue preferee")]
        public string LanguePreferee { get; set; }

        [StringLength(20)]
        [Phone]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Display(Name = "Date d'inscription")]
        public DateTime DateInscription { get; set; }

        [Display(Name = "Derniere connexion")]
        public DateTime? DerniereConnexion { get; set; }

        [Display(Name = "Actif")]
        public bool EstActif { get; set; }

        [Display(Name = "Premier changement de mot de passe")]
        public bool PremierChangementMotDePasse { get; set; }

        [NotMapped]
        [Display(Name = "Nom complet")]
        public string NomComplet => $"{Prenom} {Nom}";

        protected Utilisateur()
        {
            DateInscription = DateTime.Now;
            EstActif = true;
            PremierChangementMotDePasse = false;
            LanguePreferee = "fr"; // Francais par defaut
        }
    }
}

