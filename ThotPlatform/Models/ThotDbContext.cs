using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ThotPlatform.Models
{
    /// <summary>
    /// Contexte de base de donnees Entity Framework pour la plateforme Thot
    /// </summary>
    public class ThotDbContext : DbContext
    {
        public ThotDbContext() : base("ThotDbContext")
        {
            // Configuration pour activer les migrations automatiques
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ThotDbContext, Migrations.Configuration>());
            
            // Desactiver uniquement le lazy loading (garder les proxies pour Include())
            this.Configuration.LazyLoadingEnabled = false;
        }

        // DbSets pour les utilisateurs
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Tuteur> Tuteurs { get; set; }

        // DbSets pour le systeme de tutorat
        public DbSet<Domaine> Domaines { get; set; }
        public DbSet<TuteurDomaine> TuteurDomaines { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Reponse> Reponses { get; set; }
        public DbSet<SessionClavardage> SessionsClavardage { get; set; }
        public DbSet<MessageClavardage> MessagesClavardage { get; set; }
        public DbSet<RencontrePhysique> RencontresPhysiques { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // DbSets pour le systeme E-learning
        public DbSet<Cours> Cours { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<RessourceCours> RessourcesCours { get; set; }
        public DbSet<RessourceModule> RessourcesModules { get; set; }
        public DbSet<InscriptionCours> InscriptionsCours { get; set; }
        public DbSet<ProgressionModule> ProgressionsModules { get; set; }
        public DbSet<QuestionCours> QuestionsCours { get; set; }

        // DbSets pour les quiz
        public DbSet<Quiz> Quizs { get; set; }
        public DbSet<QuestionQuiz> QuestionsQuiz { get; set; }
        public DbSet<ChoixReponse> ChoixReponses { get; set; }
        public DbSet<TentativeQuiz> TentativesQuiz { get; set; }
        public DbSet<ReponseQuiz> ReponsesQuiz { get; set; }

        // DbSets pour la messagerie
        public DbSet<Message> Messages { get; set; }

        // DbSets pour la FAQ
        public DbSet<FAQ> FAQs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Desactiver la suppression en cascade par defaut
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // Configuration de l'heritage TPH (Table Per Hierarchy) pour Utilisateur
            modelBuilder.Entity<Utilisateur>()
                .Map<Etudiant>(m => m.Requires("TypeUtilisateur").HasValue("Etudiant"))
                .Map<Tuteur>(m => m.Requires("TypeUtilisateur").HasValue("Tuteur"));

            // Configuration des relations Etudiant
            modelBuilder.Entity<Etudiant>()
                .HasMany(e => e.Questions)
                .WithRequired(q => q.Etudiant)
                .HasForeignKey(q => q.EtudiantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Etudiant>()
                .HasMany(e => e.SessionsClavardage)
                .WithRequired(s => s.Etudiant)
                .HasForeignKey(s => s.EtudiantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Etudiant>()
                .HasMany(e => e.RencontresPhysiques)
                .WithRequired(r => r.Etudiant)
                .HasForeignKey(r => r.EtudiantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Etudiant>()
                .HasMany(e => e.Transactions)
                .WithRequired(t => t.Etudiant)
                .HasForeignKey(t => t.EtudiantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Etudiant>()
                .HasMany(e => e.InscriptionsCours)
                .WithRequired(i => i.Etudiant)
                .HasForeignKey(i => i.EtudiantId)
                .WillCascadeOnDelete(false);

            // Configuration des relations Tuteur
            modelBuilder.Entity<Tuteur>()
                .HasMany(t => t.DomainesExpertise)
                .WithRequired(td => td.Tuteur)
                .HasForeignKey(td => td.TuteurId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tuteur>()
                .HasMany(t => t.Reponses)
                .WithRequired(r => r.Tuteur)
                .HasForeignKey(r => r.TuteurId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tuteur>()
                .HasMany(t => t.SessionsClavardage)
                .WithRequired(s => s.Tuteur)
                .HasForeignKey(s => s.TuteurId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tuteur>()
                .HasMany(t => t.RencontresPhysiques)
                .WithRequired(r => r.Tuteur)
                .HasForeignKey(r => r.TuteurId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tuteur>()
                .HasMany(t => t.Cours)
                .WithRequired(c => c.Tuteur)
                .HasForeignKey(c => c.TuteurId)
                .WillCascadeOnDelete(false);

            // Configuration des relations Domaine
            modelBuilder.Entity<Domaine>()
                .HasMany(d => d.TuteursDomaines)
                .WithRequired(td => td.Domaine)
                .HasForeignKey(td => td.DomaineId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Domaine>()
                .HasMany(d => d.Questions)
                .WithRequired(q => q.Domaine)
                .HasForeignKey(q => q.DomaineId)
                .WillCascadeOnDelete(false);

            // Configuration des relations FAQ
            modelBuilder.Entity<Domaine>()
                .HasMany(d => d.FAQs)
                .WithRequired(f => f.Domaine)
                .HasForeignKey(f => f.DomaineId)
                .WillCascadeOnDelete(false);

            // Configuration des relations Question-Reponse
            modelBuilder.Entity<Question>()
                .HasMany(q => q.Reponses)
                .WithRequired(r => r.Question)
                .HasForeignKey(r => r.QuestionId)
                .WillCascadeOnDelete(true);

            // Configuration des relations SessionClavardage-MessageClavardage
            modelBuilder.Entity<SessionClavardage>()
                .HasMany(s => s.Messages)
                .WithRequired(m => m.Session)
                .HasForeignKey(m => m.SessionId)
                .WillCascadeOnDelete(true);

            // Configuration des relations Cours
            modelBuilder.Entity<Cours>()
                .HasMany(c => c.Modules)
                .WithRequired(m => m.Cours)
                .HasForeignKey(m => m.CoursId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Cours>()
                .HasMany(c => c.Ressources)
                .WithRequired(r => r.Cours)
                .HasForeignKey(r => r.CoursId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Cours>()
                .HasMany(c => c.Inscriptions)
                .WithRequired(i => i.Cours)
                .HasForeignKey(i => i.CoursId)
                .WillCascadeOnDelete(false);

            // Configuration des relations Module
            modelBuilder.Entity<Module>()
                .HasMany(m => m.Ressources)
                .WithRequired(r => r.Module)
                .HasForeignKey(r => r.ModuleId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Module>()
                .HasMany(m => m.Quiz)
                .WithRequired(q => q.Module)
                .HasForeignKey(q => q.ModuleId)
                .WillCascadeOnDelete(true);

            // Configuration des relations InscriptionCours
            modelBuilder.Entity<InscriptionCours>()
                .HasMany(i => i.ProgressionsModules)
                .WithRequired(p => p.Inscription)
                .HasForeignKey(p => p.InscriptionId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<InscriptionCours>()
                .HasMany(i => i.QuestionsCours)
                .WithRequired(q => q.Inscription)
                .HasForeignKey(q => q.InscriptionId)
                .WillCascadeOnDelete(true);

            // Configuration des relations Quiz
            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithRequired(qq => qq.Quiz)
                .HasForeignKey(qq => qq.QuizId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Tentatives)
                .WithRequired(t => t.Quiz)
                .HasForeignKey(t => t.QuizId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<QuestionQuiz>()
                .HasMany(q => q.Choix)
                .WithRequired(c => c.Question)
                .HasForeignKey(c => c.QuestionQuizId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<TentativeQuiz>()
                .HasMany(t => t.Reponses)
                .WithRequired(r => r.Tentative)
                .HasForeignKey(r => r.TentativeId)
                .WillCascadeOnDelete(true);

            // Index uniques
            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Cours>()
                .HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}

