using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant un cours E-learning
    /// </summary>
    public class Cours
    {
        [Key]
        public int CoursId { get; set; }

        [Required(ErrorMessage = "Le nom du cours est requis")]
        [StringLength(200)]
        [Display(Name = "Nom du cours")]
        public string Nom { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Code du cours")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        public int TuteurId { get; set; }

        [Required]
        public int DomaineId { get; set; }

        [Display(Name = "Niveau scolaire")]
        public NiveauScolaire Niveau { get; set; }

        [Display(Name = "Nombre de modules")]
        [Range(1, 100)]
        public int NombreModules { get; set; }

        [Display(Name = "Duree estimee (heures)")]
        public decimal DureeEstimeeHeures { get; set; }

        [StringLength(500)]
        [Display(Name = "Image de couverture")]
        public string ImageCouverture { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Date de derniere modification")]
        public DateTime DateModification { get; set; }

        [Display(Name = "Publie")]
        public bool EstPublie { get; set; }

        [Display(Name = "Nombre d'inscrits")]
        public int NombreInscrits { get; set; }

        [Display(Name = "Note moyenne")]
        [Range(0, 5)]
        public decimal NoteMoyenne { get; set; }

        // Relations
        public virtual Tuteur Tuteur { get; set; }
        public virtual Domaine Domaine { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
        public virtual ICollection<InscriptionCours> Inscriptions { get; set; }
        public virtual ICollection<RessourceCours> Ressources { get; set; }

        public Cours()
        {
            DateCreation = DateTime.Now;
            DateModification = DateTime.Now;
            EstPublie = false;
            NombreInscrits = 0;
            NoteMoyenne = 0;
            Modules = new HashSet<Module>();
            Inscriptions = new HashSet<InscriptionCours>();
            Ressources = new HashSet<RessourceCours>();
        }
    }

    /// <summary>
    /// Modele representant un module d'un cours
    /// </summary>
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        public int CoursId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Titre du module")]
        public string Titre { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Ordre")]
        public int Ordre { get; set; }

        [Display(Name = "Duree (minutes)")]
        public int DureeMinutes { get; set; }

        [StringLength(500)]
        [Display(Name = "Video theorique")]
        public string VideoTheorique { get; set; }

        [StringLength(500)]
        [Display(Name = "Video demonstrative")]
        public string VideoDemonstrative { get; set; }

        [Display(Name = "Contenu texte")]
        [DataType(DataType.Html)]
        public string ContenuTexte { get; set; }

        [Display(Name = "Publie")]
        public bool EstPublie { get; set; }

        [Display(Name = "Date de creation")]
        public DateTime DateCreation { get; set; }

        // Relations
        public virtual Cours Cours { get; set; }
        public virtual ICollection<RessourceModule> Ressources { get; set; }
        public virtual ICollection<Quiz> Quiz { get; set; }

        public Module()
        {
            EstPublie = false;
            DateCreation = DateTime.Now;
            Ressources = new HashSet<RessourceModule>();
            Quiz = new HashSet<Quiz>();
        }
    }

    /// <summary>
    /// Modele representant une ressource d'un cours (notes, exercices, etc.)
    /// </summary>
    public class RessourceCours
    {
        [Key]
        public int RessourceId { get; set; }

        [Required]
        public int CoursId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Titre")]
        public string Titre { get; set; }

        [Display(Name = "Type de ressource")]
        public TypeRessource Type { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Chemin du fichier")]
        public string CheminFichier { get; set; }

        [Display(Name = "Taille (octets)")]
        public long TailleFichier { get; set; }

        [Display(Name = "Date d'ajout")]
        public DateTime DateAjout { get; set; }

        [Display(Name = "Nombre de telechargements")]
        public int NombreTelechargements { get; set; }

        // Relations
        public virtual Cours Cours { get; set; }

        public RessourceCours()
        {
            DateAjout = DateTime.Now;
            NombreTelechargements = 0;
        }
    }

    /// <summary>
    /// Modele representant une ressource d'un module
    /// </summary>
    public class RessourceModule
    {
        [Key]
        public int RessourceModuleId { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Titre")]
        public string Titre { get; set; }

        [Display(Name = "Type de ressource")]
        public TypeRessource Type { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Chemin du fichier")]
        public string CheminFichier { get; set; }

        [Display(Name = "Date d'ajout")]
        public DateTime DateAjout { get; set; }

        // Relations
        public virtual Module Module { get; set; }

        public RessourceModule()
        {
            DateAjout = DateTime.Now;
        }
    }

    /// <summary>
    /// Enumeration des types de ressources
    /// </summary>
    public enum TypeRessource
    {
        [Display(Name = "Notes de cours")]
        NotesCours = 1,

        [Display(Name = "Exercices")]
        Exercices = 2,

        [Display(Name = "Solutionnaire")]
        Solutionnaire = 3,

        [Display(Name = "Laboratoire")]
        Laboratoire = 4,

        [Display(Name = "Document PDF")]
        DocumentPDF = 5,

        [Display(Name = "Presentation")]
        Presentation = 6,

        [Display(Name = "Video")]
        Video = 7,

        [Display(Name = "Autre")]
        Autre = 8
    }
}

