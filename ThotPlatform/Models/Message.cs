using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant un message entre un etudiant et un tuteur
    /// </summary>
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int ExpediteurId { get; set; }

        [Required]
        public int DestinatireId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Sujet")]
        public string Sujet { get; set; }

        [Required]
        [Display(Name = "Contenu")]
        [DataType(DataType.MultilineText)]
        public string Contenu { get; set; }

        [Display(Name = "Date d'envoi")]
        public DateTime DateEnvoi { get; set; }

        [Display(Name = "Lu")]
        public bool EstLu { get; set; }

        [Display(Name = "Date de lecture")]
        public DateTime? DateLecture { get; set; }

        public int? ConversationId { get; set; }

        // Relations
        [ForeignKey("ExpediteurId")]
        public virtual Utilisateur Expediteur { get; set; }

        [ForeignKey("DestinatireId")]
        public virtual Utilisateur Destinataire { get; set; }

        public Message()
        {
            DateEnvoi = DateTime.Now;
            EstLu = false;
        }
    }
}

