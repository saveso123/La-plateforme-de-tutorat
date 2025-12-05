using System;
using ThotPlatform.Models;

namespace ThotPlatform.ViewModels
{
    /// <summary>
    /// ViewModel pour afficher les rencontres physiques sans probleme de proxy
    /// </summary>
    public class RencontrePhysiqueViewModel
    {
        public int RencontreId { get; set; }
        public DateTime DateHeure { get; set; }
        public decimal DureeHeures { get; set; }
        public string Lieu { get; set; }
        public string Description { get; set; }
        public decimal Tarif { get; set; }
        public bool TarifPreferentiel { get; set; }
        public StatutRencontre Statut { get; set; }
        public int? NoteEtudiant { get; set; }
        public string Commentaire { get; set; }
        public DateTime DateCreation { get; set; }

        // Donnees de l'etudiant
        public string EtudiantPrenom { get; set; }
        public string EtudiantNom { get; set; }
        public string EtudiantEmail { get; set; }

        // Donnees du tuteur
        public string TuteurPrenom { get; set; }
        public string TuteurNom { get; set; }
        public string TuteurEmail { get; set; }

        // Donnees du domaine
        public string DomaineNom { get; set; }

        // Propriete calculee
        public decimal CoutTotal => Tarif * DureeHeures;
    }
}
