using System;
using ThotPlatform.Models;

namespace ThotPlatform.ViewModels
{
    /// <summary>
    /// ViewModel pour afficher les details d'un cours
    /// </summary>
    public class CoursDetailsViewModel
    {
        public int CoursId { get; set; }
        public string Nom { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DomaineNom { get; set; }
        public NiveauScolaire Niveau { get; set; }
        public int NombreModules { get; set; }
        public decimal DureeEstimeeHeures { get; set; }
        public string ImageCouverture { get; set; }
        public bool EstPublie { get; set; }
        public DateTime DateCreation { get; set; }
        public int NombreInscrits { get; set; }
        
        // Informations du tuteur
        public string TuteurNomComplet { get; set; }
        public string TuteurEmail { get; set; }
        public decimal TuteurNoteMoyenne { get; set; }
        
        // Pour l'etudiant
        public bool EstInscrit { get; set; }
    }
}

