using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant un quiz associe a un module
    /// </summary>
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Titre du quiz")]
        public string Titre { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Duree limite (minutes)")]
        public int? DureeLimiteMinutes { get; set; }

        [Display(Name = "Note de passage (%)")]
        [Range(0, 100)]
        public int NotePassage { get; set; }

        [Display(Name = "Nombre de tentatives autorisees")]
        public int NombreTentativesAutorisees { get; set; }

        [Display(Name = "Ordre aleatoire des questions")]
        public bool OrdreAleatoire { get; set; }

        [Display(Name = "Afficher les reponses apres")]
        public bool AfficherReponses { get; set; }

        [Display(Name = "Publie")]
        public bool EstPublie { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        // Relations
        public virtual Module Module { get; set; }
        public virtual ICollection<QuestionQuiz> Questions { get; set; }
        public virtual ICollection<TentativeQuiz> Tentatives { get; set; }

        public Quiz()
        {
            NotePassage = 60;
            NombreTentativesAutorisees = 3;
            OrdreAleatoire = false;
            AfficherReponses = true;
            EstPublie = false;
            DateCreation = DateTime.Now;
            Questions = new HashSet<QuestionQuiz>();
            Tentatives = new HashSet<TentativeQuiz>();
        }
    }

    /// <summary>
    /// Modele representant une question de quiz
    /// </summary>
    public class QuestionQuiz
    {
        [Key]
        public int QuestionQuizId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        [Display(Name = "Question")]
        [DataType(DataType.MultilineText)]
        public string Texte { get; set; }

        [Display(Name = "Type de question")]
        public TypeQuestionQuiz Type { get; set; }

        [Display(Name = "Points")]
        [Range(1, 100)]
        public int Points { get; set; }

        [Display(Name = "Ordre")]
        public int Ordre { get; set; }

        [StringLength(500)]
        [Display(Name = "Image")]
        public string Image { get; set; }

        [Display(Name = "Explication")]
        [DataType(DataType.MultilineText)]
        public string Explication { get; set; }

        // Relations
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<ChoixReponse> Choix { get; set; }

        public QuestionQuiz()
        {
            Points = 1;
            Type = TypeQuestionQuiz.ChoixMultiple;
            Choix = new HashSet<ChoixReponse>();
        }
    }

    /// <summary>
    /// Modele representant un choix de reponse pour une question de quiz
    /// </summary>
    public class ChoixReponse
    {
        [Key]
        public int ChoixId { get; set; }

        [Required]
        public int QuestionQuizId { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Texte du choix")]
        public string Texte { get; set; }

        [Display(Name = "Est la bonne reponse")]
        public bool EstCorrect { get; set; }

        [Display(Name = "Ordre")]
        public int Ordre { get; set; }

        // Relations
        public virtual QuestionQuiz Question { get; set; }
    }

    /// <summary>
    /// Modele representant une tentative de quiz par un etudiant
    /// </summary>
    public class TentativeQuiz
    {
        [Key]
        public int TentativeId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int EtudiantId { get; set; }

        [Display(Name = "Date de debut")]
        public DateTime DateDebut { get; set; }

        [Display(Name = "Date de fin")]
        public DateTime? DateFin { get; set; }

        [Display(Name = "Note obtenue (%)")]
        [Range(0, 100)]
        public int? NoteObtenue { get; set; }

        [Display(Name = "Points obtenus")]
        public int? PointsObtenus { get; set; }

        [Display(Name = "Points totaux")]
        public int PointsTotaux { get; set; }

        [Display(Name = "Numero de tentative")]
        public int NumeroTentative { get; set; }

        [Display(Name = "Completee")]
        public bool EstCompletee { get; set; }

        [Display(Name = "Reussie")]
        public bool EstReussie { get; set; }

        [Display(Name = "Corrigee")]
        public bool EstCorrigee { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        // Relations
        public virtual Quiz Quiz { get; set; }
        public virtual Etudiant Etudiant { get; set; }
        public virtual ICollection<ReponseQuiz> Reponses { get; set; }

        public TentativeQuiz()
        {
            DateDebut = DateTime.Now;
            DateCreation = DateTime.Now;
            EstCompletee = false;
            EstReussie = false;
            EstCorrigee = false;
            Reponses = new HashSet<ReponseQuiz>();
        }

        [NotMapped]
        [Display(Name = "Duree")]
        public TimeSpan? Duree
        {
            get
            {
                if (DateFin.HasValue)
                    return DateFin.Value - DateDebut;
                return null;
            }
        }
    }

    /// <summary>
    /// Modele representant la reponse d'un etudiant a une question de quiz
    /// </summary>
    public class ReponseQuiz
    {
        [Key]
        public int ReponseQuizId { get; set; }

        [Required]
        public int TentativeId { get; set; }

        [Required]
        public int QuestionQuizId { get; set; }

        public int? ChoixId { get; set; }

        [Display(Name = "Reponse texte")]
        public string ReponseTexte { get; set; }

        [Display(Name = "Est correcte")]
        public bool EstCorrecte { get; set; }

        [Display(Name = "Points obtenus")]
        public int PointsObtenus { get; set; }

        [Display(Name = "Points accordes (correction tuteur)")]
        public int? PointsAccordes { get; set; }

        // Relations
        public virtual TentativeQuiz Tentative { get; set; }
        public virtual QuestionQuiz Question { get; set; }
        public virtual ChoixReponse Choix { get; set; }
    }

    /// <summary>
    /// Enumeration des types de questions de quiz
    /// </summary>
    public enum TypeQuestionQuiz
    {
        [Display(Name = "Choix multiple")]
        ChoixMultiple = 1,

        [Display(Name = "Vrai ou Faux")]
        VraiFaux = 2,

        [Display(Name = "Reponse courte")]
        ReponseCourte = 3,

        [Display(Name = "Reponse longue")]
        ReponseLongue = 4
    }
}

