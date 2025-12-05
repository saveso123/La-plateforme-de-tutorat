using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Modele representant une transaction financiere
    /// </summary>
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int EtudiantId { get; set; }

        [Required]
        [Display(Name = "Type de transaction")]
        public TypeTransaction Type { get; set; }

        [Required]
        [Display(Name = "Montant")]
        [Range(0.01, 10000)]
        public decimal Montant { get; set; }

        [Display(Name = "Date de transaction")]
        public DateTime DateTransaction { get; set; }

        [Display(Name = "Statut")]
        public StatutTransaction Statut { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "ID PayPal")]
        public string PayPalTransactionId { get; set; }

        [StringLength(100)]
        [Display(Name = "ID Paiement PayPal")]
        public string PayPalPaymentId { get; set; }

        [StringLength(50)]
        [Display(Name = "Statut PayPal")]
        public string PayPalStatut { get; set; }

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        // ID de l'element associe (Question, Session, Rencontre, Abonnement)
        public int? ItemId { get; set; }

        [Display(Name = "Date de remboursement")]
        public DateTime? DateRemboursement { get; set; }

        // Relations
        public virtual Etudiant Etudiant { get; set; }

        public Transaction()
        {
            DateTransaction = DateTime.Now;
            Statut = StatutTransaction.EnAttente;
        }
    }

    /// <summary>
    /// Enumeration des types de transaction
    /// </summary>
    public enum TypeTransaction
    {
        [Display(Name = "Abonnement mensuel")]
        AbonnementMensuel = 1,

        [Display(Name = "Question ponctuelle")]
        QuestionPonctuelle = 2,

        [Display(Name = "Session immediate")]
        SessionImmediate = 3,

        [Display(Name = "Rencontre physique")]
        RencontrePhysique = 4,

        [Display(Name = "Remboursement")]
        Remboursement = 5
    }

    /// <summary>
    /// Enumeration des statuts de transaction
    /// </summary>
    public enum StatutTransaction
    {
        [Display(Name = "En attente")]
        EnAttente = 1,

        [Display(Name = "Approuvee")]
        Approuvee = 2,

        [Display(Name = "Completee")]
        Completee = 3,

        [Display(Name = "Echouee")]
        Echouee = 4,

        [Display(Name = "Remboursee")]
        Remboursee = 5,

        [Display(Name = "Annulee")]
        Annulee = 6
    }
}

