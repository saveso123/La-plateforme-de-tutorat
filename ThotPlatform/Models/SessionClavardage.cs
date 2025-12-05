using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant une session de clavardage entre un etudiant et un tuteur
    /// </summary>
    public class SessionClavardage
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        public int EtudiantId { get; set; }

        [Required]
        public int TuteurId { get; set; }

        [Required]
        public int DomaineId { get; set; }

        [Display(Name = "Date de debut")]
        public DateTime DateDebut { get; set; }

        [Display(Name = "Date de fin")]
        public DateTime? DateFin { get; set; }

        [Display(Name = "Duree (minutes)")]
        public int DureeMinutes { get; set; }

        [Display(Name = "Type de session")]
        public TypeSession Type { get; set; }

        [Display(Name = "Statut")]
        public StatutSession Statut { get; set; }

        [Display(Name = "Cout")]
        public decimal Cout { get; set; }

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
        public virtual ICollection<MessageClavardage> Messages { get; set; }

        public SessionClavardage()
        {
            DateDebut = DateTime.Now;
            Statut = StatutSession.EnAttente;
            Messages = new HashSet<MessageClavardage>();
            DureeMinutes = 60; // 1 heure par defaut
        }

        [NotMapped]
        [Display(Name = "Duree reelle")]
        public TimeSpan? DureeReelle
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
    /// Modele representant un message dans une session de clavardage
    /// </summary>
    public class MessageClavardage
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public int UtilisateurId { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Contenu { get; set; }

        [Display(Name = "Date d'envoi")]
        public DateTime DateEnvoi { get; set; }

        [Display(Name = "Lu")]
        public bool EstLu { get; set; }

        // Relations
        public virtual SessionClavardage Session { get; set; }

        public MessageClavardage()
        {
            DateEnvoi = DateTime.Now;
            EstLu = false;
        }
    }

    /// <summary>
    /// Enumeration des types de session
    /// </summary>
    public enum TypeSession
    {
        [Display(Name = "Session normale (abonne)")]
        Normale = 1,

        [Display(Name = "Session immediate (prioritaire)")]
        Immediate = 2
    }

    /// <summary>
    /// Enumeration des statuts de session
    /// </summary>
    public enum StatutSession
    {
        [Display(Name = "En attente")]
        EnAttente = 1,

        [Display(Name = "En cours")]
        EnCours = 2,

        [Display(Name = "Terminee")]
        Terminee = 3,

        [Display(Name = "Annulee")]
        Annulee = 4
    }
}

