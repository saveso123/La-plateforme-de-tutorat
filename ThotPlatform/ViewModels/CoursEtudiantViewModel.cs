using System.Collections.Generic;
using System.Linq;
using ThotPlatform.Models;

namespace ThotPlatform.ViewModels
{
    /// <summary>
    /// ViewModel pour l'affichage d'un cours cote etudiant
    /// </summary>
    public class CoursEtudiantViewModel
    {
        public Cours Cours { get; set; }
        public List<Module> Modules { get; set; }
        public Module ModuleActuel { get; set; }
        public List<ProgressionModule> Progressions { get; set; }
        public List<TentativeQuiz> TentativesQuiz { get; set; }
        public InscriptionCours Inscription { get; set; }

        public int ProgressionGlobale
        {
            get
            {
                if (Modules == null || Modules.Count == 0)
                    return 0;

                var modulesCompletes = Progressions?.Count(p => p.EstComplete) ?? 0;
                return (int)((modulesCompletes * 100.0) / Modules.Count);
            }
        }

        public CoursEtudiantViewModel()
        {
            Modules = new List<Module>();
            Progressions = new List<ProgressionModule>();
            TentativesQuiz = new List<TentativeQuiz>();
        }
    }
}

