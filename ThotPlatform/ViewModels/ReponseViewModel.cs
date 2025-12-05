namespace ThotPlatform.ViewModels
{
    public class ReponseViewModel
    {
        public int ReponseId { get; set; }
        public string Contenu { get; set; }
        public System.DateTime DateCreation { get; set; }
        public bool EstValidee { get; set; }
        public int? Note { get; set; }
        public string QuestionTitre { get; set; }
        public string QuestionContenu { get; set; }
        public System.DateTime QuestionDate { get; set; }
        public string EtudiantNom { get; set; }
    }
}

