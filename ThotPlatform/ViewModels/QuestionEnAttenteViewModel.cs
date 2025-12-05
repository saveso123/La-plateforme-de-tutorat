using System;
using ThotPlatform.Models;

namespace ThotPlatform.ViewModels
{
    /// <summary>
    /// ViewModel pour afficher les questions en attente
    /// </summary>
    public class QuestionEnAttenteViewModel
    {
        public int QuestionId { get; set; }
        public string Titre { get; set; }
        public string Contenu { get; set; }
        public DateTime DateCreation { get; set; }
        public string FichierJoint { get; set; }
        public bool EstPrioritaire { get; set; }
        public StatutQuestion Statut { get; set; }
        
        // Informations de l'etudiant
        public string EtudiantNomComplet { get; set; }
        public string EtudiantNiveau { get; set; }
        
        // Informations du domaine
        public string DomaineNom { get; set; }
    }
}

