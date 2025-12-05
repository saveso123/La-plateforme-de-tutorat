using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant une question posee par un etudiant
    /// </summary>
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public int EtudiantId { get; set; }

        [Required]
        public int DomaineId { get; set; }

        [Required(ErrorMessage = "Le titre est requis")]
        [StringLength(200)]
        [Display(Name = "Titre")]
        public string Titre { get; set; }

        [Required(ErrorMessage = "Le contenu de la question est requis")]
        [Display(Name = "Question")]
        [DataType(DataType.MultilineText)]
        public string Contenu { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Fichier joint")]
        [StringLength(500)]
        public string FichierJoint { get; set; }

        [Display(Name = "Statut")]
        public StatutQuestion Statut { get; set; }

        [Display(Name = "Prioritaire")]
        public bool EstPrioritaire { get; set; }

        [Display(Name = "Date limite de reponse")]
        public DateTime DateLimiteReponse { get; set; }

        [Display(Name = "Nombre de vues")]
        public int NombreVues { get; set; }

        // Relations
        public virtual Etudiant Etudiant { get; set; }
        public virtual Domaine Domaine { get; set; }
        public virtual ICollection<Reponse> Reponses { get; set; }

        public Question()
        {
            DateCreation = DateTime.Now;
            Statut = StatutQuestion.EnAttente;
            NombreVues = 0;
            Reponses = new HashSet<Reponse>();
            
            // Garantie de reponse dans les 2 heures
            DateLimiteReponse = DateTime.Now.AddHours(2);
        }
    }

    /// <summary>
    /// Enumeration des statuts de question
    /// </summary>
    public enum StatutQuestion
    {
        [Display(Name = "En attente")]
        EnAttente = 1,

        [Display(Name = "En cours de traitement")]
        EnCours = 2,

        [Display(Name = "Repondue")]
        Repondue = 3,

        [Display(Name = "Fermee")]
        Fermee = 4,

        [Display(Name = "Resolue (archivee)")]
        Resolue = 5
    }
}

