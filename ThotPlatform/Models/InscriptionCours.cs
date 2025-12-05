using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant l'inscription d'un etudiant a un cours
    /// </summary>
    public class InscriptionCours
    {
        [Key]
        public int InscriptionId { get; set; }

        [Required]
        public int EtudiantId { get; set; }

        [Required]
        public int CoursId { get; set; }

        [Display(Name = "Date d'inscription")]
        public DateTime DateInscription { get; set; }

        [Display(Name = "Date de derniere activite")]
        public DateTime? DateDerniereActivite { get; set; }

        [Display(Name = "Progression (%)")]
        [Range(0, 100)]
        public int ProgressionPourcentage { get; set; }

        [Display(Name = "Temps total (minutes)")]
        public int TempsTotalMinutes { get; set; }

        [Display(Name = "Cours complete")]
        public bool EstComplete { get; set; }

        [Display(Name = "Date de completion")]
        public DateTime? DateCompletion { get; set; }

        [Display(Name = "Note finale")]
        [Range(0, 100)]
        public int? NoteFinal { get; set; }

        [Display(Name = "Evaluation du cours")]
        [Range(1, 5)]
        public int? EvaluationCours { get; set; }

        [StringLength(500)]
        [Display(Name = "Commentaire")]
        public string Commentaire { get; set; }

        // Relations
        public virtual Etudiant Etudiant { get; set; }
        public virtual Cours Cours { get; set; }
        public virtual ICollection<ProgressionModule> ProgressionsModules { get; set; }
        public virtual ICollection<QuestionCours> QuestionsCours { get; set; }

        public InscriptionCours()
        {
            DateInscription = DateTime.Now;
            ProgressionPourcentage = 0;
            TempsTotalMinutes = 0;
            EstComplete = false;
            ProgressionsModules = new HashSet<ProgressionModule>();
            QuestionsCours = new HashSet<QuestionCours>();
        }
    }

    /// <summary>
    /// Modele representant la progression d'un etudiant dans un module
    /// </summary>
    public class ProgressionModule
    {
        [Key]
        public int ProgressionId { get; set; }

        [Required]
        public int InscriptionId { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [Display(Name = "Date de debut")]
        public DateTime? DateDebut { get; set; }

        [Display(Name = "Date de completion")]
        public DateTime? DateCompletion { get; set; }

        [Display(Name = "Temps passe (minutes)")]
        public int TempsPasseMinutes { get; set; }

        [Display(Name = "Module complete")]
        public bool EstComplete { get; set; }

        [Display(Name = "Video theorique vue")]
        public bool VideoTheoriqueVue { get; set; }

        [Display(Name = "Video demonstrative vue")]
        public bool VideoDemonstrativeVue { get; set; }

        // Relations
        public virtual InscriptionCours Inscription { get; set; }
        public virtual Module Module { get; set; }

        public ProgressionModule()
        {
            EstComplete = false;
            VideoTheoriqueVue = false;
            VideoDemonstrativeVue = false;
            TempsPasseMinutes = 0;
        }
    }

    /// <summary>
    /// Modele representant une question posee par un etudiant sur une ressource de cours
    /// </summary>
    public class QuestionCours
    {
        [Key]
        public int QuestionCoursId { get; set; }

        [Required]
        public int InscriptionId { get; set; }

        public int? ModuleId { get; set; }

        public int? RessourceId { get; set; }

        [Required]
        [Display(Name = "Question")]
        [DataType(DataType.MultilineText)]
        public string Contenu { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Reponse")]
        [DataType(DataType.MultilineText)]
        public string Reponse { get; set; }

        [Display(Name = "Date de reponse")]
        public DateTime? DateReponse { get; set; }

        public int? TuteurId { get; set; }

        [Display(Name = "Statut")]
        public StatutQuestionCours Statut { get; set; }

        // Relations
        public virtual InscriptionCours Inscription { get; set; }
        public virtual Module Module { get; set; }
        public virtual Tuteur Tuteur { get; set; }

        public QuestionCours()
        {
            DateCreation = DateTime.Now;
            Statut = StatutQuestionCours.EnAttente;
        }
    }

    /// <summary>
    /// Enumeration des statuts de question de cours
    /// </summary>
    public enum StatutQuestionCours
    {
        [Display(Name = "En attente")]
        EnAttente = 1,

        [Display(Name = "Repondue")]
        Repondue = 2,

        [Display(Name = "Fermee")]
        Fermee = 3
    }
}

