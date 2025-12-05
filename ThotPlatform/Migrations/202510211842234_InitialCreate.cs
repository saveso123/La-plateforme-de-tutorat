namespace ThotPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChoixReponses",
                c => new
                    {
                        ChoixId = c.Int(nullable: false, identity: true),
                        QuestionQuizId = c.Int(nullable: false),
                        Texte = c.String(nullable: false, maxLength: 500),
                        EstCorrect = c.Boolean(nullable: false),
                        Ordre = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChoixId)
                .ForeignKey("dbo.QuestionQuizs", t => t.QuestionQuizId, cascadeDelete: true)
                .Index(t => t.QuestionQuizId);
            
            CreateTable(
                "dbo.QuestionQuizs",
                c => new
                    {
                        QuestionQuizId = c.Int(nullable: false, identity: true),
                        QuizId = c.Int(nullable: false),
                        Texte = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                        Ordre = c.Int(nullable: false),
                        Image = c.String(maxLength: 500),
                        Explication = c.String(),
                    })
                .PrimaryKey(t => t.QuestionQuizId)
                .ForeignKey("dbo.Quizs", t => t.QuizId, cascadeDelete: true)
                .Index(t => t.QuizId);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        QuizId = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        Titre = c.String(nullable: false, maxLength: 200),
                        Description = c.String(),
                        DureeLimiteMinutes = c.Int(),
                        NotePassage = c.Int(nullable: false),
                        NombreTentativesAutorisees = c.Int(nullable: false),
                        OrdreAleatoire = c.Boolean(nullable: false),
                        AfficherReponses = c.Boolean(nullable: false),
                        EstPublie = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.QuizId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        CoursId = c.Int(nullable: false),
                        Titre = c.String(nullable: false, maxLength: 200),
                        Description = c.String(),
                        Ordre = c.Int(nullable: false),
                        DureeMinutes = c.Int(nullable: false),
                        VideoTheorique = c.String(maxLength: 500),
                        VideoDemonstrative = c.String(maxLength: 500),
                        ContenuTexte = c.String(),
                        EstPublie = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.Cours", t => t.CoursId, cascadeDelete: true)
                .Index(t => t.CoursId);
            
            CreateTable(
                "dbo.Cours",
                c => new
                    {
                        CoursId = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false, maxLength: 200),
                        Code = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false),
                        TuteurId = c.Int(nullable: false),
                        DomaineId = c.Int(nullable: false),
                        Niveau = c.Int(nullable: false),
                        NombreModules = c.Int(nullable: false),
                        DureeEstimeeHeures = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImageCouverture = c.String(maxLength: 500),
                        DateCreation = c.DateTime(nullable: false),
                        DateModification = c.DateTime(nullable: false),
                        EstPublie = c.Boolean(nullable: false),
                        NombreInscrits = c.Int(nullable: false),
                        NoteMoyenne = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CoursId)
                .ForeignKey("dbo.Utilisateurs", t => t.TuteurId)
                .ForeignKey("dbo.Domaines", t => t.DomaineId)
                .Index(t => t.Code, unique: true)
                .Index(t => t.TuteurId)
                .Index(t => t.DomaineId);
            
            CreateTable(
                "dbo.Domaines",
                c => new
                    {
                        DomaineId = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        Icone = c.String(maxLength: 50),
                        EstActif = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DomaineId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        EtudiantId = c.Int(nullable: false),
                        DomaineId = c.Int(nullable: false),
                        Titre = c.String(nullable: false, maxLength: 200),
                        Contenu = c.String(nullable: false),
                        DateCreation = c.DateTime(nullable: false),
                        FichierJoint = c.String(maxLength: 500),
                        Statut = c.Int(nullable: false),
                        EstPrioritaire = c.Boolean(nullable: false),
                        DateLimiteReponse = c.DateTime(nullable: false),
                        NombreVues = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.Utilisateurs", t => t.EtudiantId)
                .ForeignKey("dbo.Domaines", t => t.DomaineId)
                .Index(t => t.EtudiantId)
                .Index(t => t.DomaineId);
            
            CreateTable(
                "dbo.Utilisateurs",
                c => new
                    {
                        UtilisateurId = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false, maxLength: 100),
                        Prenom = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 255),
                        Username = c.String(nullable: false, maxLength: 50),
                        MotDePasse = c.String(nullable: false, maxLength: 255),
                        Telephone = c.String(maxLength: 20),
                        DateInscription = c.DateTime(nullable: false),
                        DerniereConnexion = c.DateTime(),
                        EstActif = c.Boolean(nullable: false),
                        PremierChangementMotDePasse = c.Boolean(nullable: false),
                        Niveau = c.Int(),
                        Etablissement = c.String(maxLength: 500),
                        EstAbonne = c.Boolean(),
                        DateDebutAbonnement = c.DateTime(),
                        DateFinAbonnement = c.DateTime(),
                        Adresse = c.String(maxLength: 500),
                        Ville = c.String(maxLength: 100),
                        CodePostal = c.String(maxLength: 10),
                        Biographie = c.String(maxLength: 1000),
                        Diplomes = c.String(maxLength: 500),
                        AnneesExperience = c.Int(),
                        DisponiblePhysique = c.Boolean(),
                        TarifHorairePhysique = c.Decimal(precision: 18, scale: 2),
                        Adresse1 = c.String(maxLength: 500),
                        Ville1 = c.String(maxLength: 100),
                        CodePostal1 = c.String(maxLength: 10),
                        NoteMoyenne = c.Decimal(precision: 18, scale: 2),
                        NombreEvaluations = c.Int(),
                        EstDisponible = c.Boolean(),
                        TypeUtilisateur = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UtilisateurId)
                .Index(t => t.Email, unique: true)
                .Index(t => t.Username, unique: true);
            
            CreateTable(
                "dbo.InscriptionCours",
                c => new
                    {
                        InscriptionId = c.Int(nullable: false, identity: true),
                        EtudiantId = c.Int(nullable: false),
                        CoursId = c.Int(nullable: false),
                        DateInscription = c.DateTime(nullable: false),
                        DateDerniereActivite = c.DateTime(),
                        ProgressionPourcentage = c.Int(nullable: false),
                        TempsTotalMinutes = c.Int(nullable: false),
                        EstComplete = c.Boolean(nullable: false),
                        DateCompletion = c.DateTime(),
                        NoteFinal = c.Int(),
                        EvaluationCours = c.Int(),
                        Commentaire = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.InscriptionId)
                .ForeignKey("dbo.Utilisateurs", t => t.EtudiantId)
                .ForeignKey("dbo.Cours", t => t.CoursId)
                .Index(t => t.EtudiantId)
                .Index(t => t.CoursId);
            
            CreateTable(
                "dbo.ProgressionModules",
                c => new
                    {
                        ProgressionId = c.Int(nullable: false, identity: true),
                        InscriptionId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        DateDebut = c.DateTime(),
                        DateCompletion = c.DateTime(),
                        TempsPasseMinutes = c.Int(nullable: false),
                        EstComplete = c.Boolean(nullable: false),
                        VideoTheoriqueVue = c.Boolean(nullable: false),
                        VideoDemonstrativeVue = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProgressionId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.InscriptionCours", t => t.InscriptionId, cascadeDelete: true)
                .Index(t => t.InscriptionId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.QuestionCours",
                c => new
                    {
                        QuestionCoursId = c.Int(nullable: false, identity: true),
                        InscriptionId = c.Int(nullable: false),
                        ModuleId = c.Int(),
                        RessourceId = c.Int(),
                        Contenu = c.String(nullable: false),
                        DateCreation = c.DateTime(nullable: false),
                        Reponse = c.String(),
                        DateReponse = c.DateTime(),
                        TuteurId = c.Int(),
                        Statut = c.Int(nullable: false),
                        Tuteur_UtilisateurId = c.Int(),
                    })
                .PrimaryKey(t => t.QuestionCoursId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.Utilisateurs", t => t.Tuteur_UtilisateurId)
                .ForeignKey("dbo.InscriptionCours", t => t.InscriptionId, cascadeDelete: true)
                .Index(t => t.InscriptionId)
                .Index(t => t.ModuleId)
                .Index(t => t.Tuteur_UtilisateurId);
            
            CreateTable(
                "dbo.TuteurDomaines",
                c => new
                    {
                        TuteurDomaineId = c.Int(nullable: false, identity: true),
                        TuteurId = c.Int(nullable: false),
                        DomaineId = c.Int(nullable: false),
                        NiveauExpertise = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TuteurDomaineId)
                .ForeignKey("dbo.Utilisateurs", t => t.TuteurId)
                .ForeignKey("dbo.Domaines", t => t.DomaineId)
                .Index(t => t.TuteurId)
                .Index(t => t.DomaineId);
            
            CreateTable(
                "dbo.RencontrePhysiques",
                c => new
                    {
                        RencontreId = c.Int(nullable: false, identity: true),
                        EtudiantId = c.Int(nullable: false),
                        TuteurId = c.Int(nullable: false),
                        DomaineId = c.Int(nullable: false),
                        DateHeure = c.DateTime(nullable: false),
                        DureeHeures = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Lieu = c.String(nullable: false, maxLength: 500),
                        Description = c.String(maxLength: 1000),
                        Statut = c.Int(nullable: false),
                        Tarif = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TarifPreferentiel = c.Boolean(nullable: false),
                        DateCreation = c.DateTime(nullable: false),
                        DateConfirmation = c.DateTime(),
                        NoteEtudiant = c.Int(),
                        Commentaire = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.RencontreId)
                .ForeignKey("dbo.Domaines", t => t.DomaineId)
                .ForeignKey("dbo.Utilisateurs", t => t.TuteurId)
                .ForeignKey("dbo.Utilisateurs", t => t.EtudiantId)
                .Index(t => t.EtudiantId)
                .Index(t => t.TuteurId)
                .Index(t => t.DomaineId);
            
            CreateTable(
                "dbo.Reponses",
                c => new
                    {
                        ReponseId = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        TuteurId = c.Int(nullable: false),
                        Contenu = c.String(nullable: false),
                        DateCreation = c.DateTime(nullable: false),
                        FichierJoint = c.String(maxLength: 500),
                        EstValidee = c.Boolean(nullable: false),
                        Note = c.Int(),
                        Commentaire = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.ReponseId)
                .ForeignKey("dbo.Utilisateurs", t => t.TuteurId)
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId)
                .Index(t => t.TuteurId);
            
            CreateTable(
                "dbo.SessionClavardages",
                c => new
                    {
                        SessionId = c.Int(nullable: false, identity: true),
                        EtudiantId = c.Int(nullable: false),
                        TuteurId = c.Int(nullable: false),
                        DomaineId = c.Int(nullable: false),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        DureeMinutes = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Statut = c.Int(nullable: false),
                        Cout = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoteEtudiant = c.Int(),
                        Commentaire = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.SessionId)
                .ForeignKey("dbo.Domaines", t => t.DomaineId)
                .ForeignKey("dbo.Utilisateurs", t => t.TuteurId)
                .ForeignKey("dbo.Utilisateurs", t => t.EtudiantId)
                .Index(t => t.EtudiantId)
                .Index(t => t.TuteurId)
                .Index(t => t.DomaineId);
            
            CreateTable(
                "dbo.MessageClavardages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        SessionId = c.Int(nullable: false),
                        UtilisateurId = c.Int(nullable: false),
                        Contenu = c.String(nullable: false),
                        DateEnvoi = c.DateTime(nullable: false),
                        EstLu = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.SessionClavardages", t => t.SessionId, cascadeDelete: true)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        EtudiantId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Montant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateTransaction = c.DateTime(nullable: false),
                        Statut = c.Int(nullable: false),
                        PayPalTransactionId = c.String(nullable: false, maxLength: 100),
                        PayPalPaymentId = c.String(maxLength: 100),
                        PayPalStatut = c.String(maxLength: 50),
                        Description = c.String(maxLength: 1000),
                        ItemId = c.Int(),
                        DateRemboursement = c.DateTime(),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Utilisateurs", t => t.EtudiantId)
                .Index(t => t.EtudiantId);
            
            CreateTable(
                "dbo.RessourceCours",
                c => new
                    {
                        RessourceId = c.Int(nullable: false, identity: true),
                        CoursId = c.Int(nullable: false),
                        Titre = c.String(nullable: false, maxLength: 200),
                        Type = c.Int(nullable: false),
                        CheminFichier = c.String(nullable: false, maxLength: 500),
                        TailleFichier = c.Long(nullable: false),
                        DateAjout = c.DateTime(nullable: false),
                        NombreTelechargements = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RessourceId)
                .ForeignKey("dbo.Cours", t => t.CoursId, cascadeDelete: true)
                .Index(t => t.CoursId);
            
            CreateTable(
                "dbo.RessourceModules",
                c => new
                    {
                        RessourceModuleId = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        Titre = c.String(nullable: false, maxLength: 200),
                        Type = c.Int(nullable: false),
                        CheminFichier = c.String(nullable: false, maxLength: 500),
                        DateAjout = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RessourceModuleId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.TentativeQuizs",
                c => new
                    {
                        TentativeId = c.Int(nullable: false, identity: true),
                        QuizId = c.Int(nullable: false),
                        EtudiantId = c.Int(nullable: false),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        NoteObtenue = c.Int(),
                        PointsObtenus = c.Int(),
                        PointsTotaux = c.Int(nullable: false),
                        NumeroTentative = c.Int(nullable: false),
                        EstCompletee = c.Boolean(nullable: false),
                        EstReussie = c.Boolean(nullable: false),
                        Etudiant_UtilisateurId = c.Int(),
                    })
                .PrimaryKey(t => t.TentativeId)
                .ForeignKey("dbo.Utilisateurs", t => t.Etudiant_UtilisateurId)
                .ForeignKey("dbo.Quizs", t => t.QuizId)
                .Index(t => t.QuizId)
                .Index(t => t.Etudiant_UtilisateurId);
            
            CreateTable(
                "dbo.ReponseQuizs",
                c => new
                    {
                        ReponseQuizId = c.Int(nullable: false, identity: true),
                        TentativeId = c.Int(nullable: false),
                        QuestionQuizId = c.Int(nullable: false),
                        ChoixId = c.Int(),
                        ReponseTexte = c.String(),
                        EstCorrecte = c.Boolean(nullable: false),
                        PointsObtenus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReponseQuizId)
                .ForeignKey("dbo.ChoixReponses", t => t.ChoixId)
                .ForeignKey("dbo.QuestionQuizs", t => t.QuestionQuizId)
                .ForeignKey("dbo.TentativeQuizs", t => t.TentativeId, cascadeDelete: true)
                .Index(t => t.TentativeId)
                .Index(t => t.QuestionQuizId)
                .Index(t => t.ChoixId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TentativeQuizs", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.ReponseQuizs", "TentativeId", "dbo.TentativeQuizs");
            DropForeignKey("dbo.ReponseQuizs", "QuestionQuizId", "dbo.QuestionQuizs");
            DropForeignKey("dbo.ReponseQuizs", "ChoixId", "dbo.ChoixReponses");
            DropForeignKey("dbo.TentativeQuizs", "Etudiant_UtilisateurId", "dbo.Utilisateurs");
            DropForeignKey("dbo.QuestionQuizs", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.RessourceModules", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Quizs", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.RessourceCours", "CoursId", "dbo.Cours");
            DropForeignKey("dbo.Modules", "CoursId", "dbo.Cours");
            DropForeignKey("dbo.InscriptionCours", "CoursId", "dbo.Cours");
            DropForeignKey("dbo.Cours", "DomaineId", "dbo.Domaines");
            DropForeignKey("dbo.TuteurDomaines", "DomaineId", "dbo.Domaines");
            DropForeignKey("dbo.Questions", "DomaineId", "dbo.Domaines");
            DropForeignKey("dbo.Reponses", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Transactions", "EtudiantId", "dbo.Utilisateurs");
            DropForeignKey("dbo.SessionClavardages", "EtudiantId", "dbo.Utilisateurs");
            DropForeignKey("dbo.RencontrePhysiques", "EtudiantId", "dbo.Utilisateurs");
            DropForeignKey("dbo.Questions", "EtudiantId", "dbo.Utilisateurs");
            DropForeignKey("dbo.InscriptionCours", "EtudiantId", "dbo.Utilisateurs");
            DropForeignKey("dbo.QuestionCours", "InscriptionId", "dbo.InscriptionCours");
            DropForeignKey("dbo.QuestionCours", "Tuteur_UtilisateurId", "dbo.Utilisateurs");
            DropForeignKey("dbo.SessionClavardages", "TuteurId", "dbo.Utilisateurs");
            DropForeignKey("dbo.MessageClavardages", "SessionId", "dbo.SessionClavardages");
            DropForeignKey("dbo.SessionClavardages", "DomaineId", "dbo.Domaines");
            DropForeignKey("dbo.Reponses", "TuteurId", "dbo.Utilisateurs");
            DropForeignKey("dbo.RencontrePhysiques", "TuteurId", "dbo.Utilisateurs");
            DropForeignKey("dbo.RencontrePhysiques", "DomaineId", "dbo.Domaines");
            DropForeignKey("dbo.TuteurDomaines", "TuteurId", "dbo.Utilisateurs");
            DropForeignKey("dbo.Cours", "TuteurId", "dbo.Utilisateurs");
            DropForeignKey("dbo.QuestionCours", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.ProgressionModules", "InscriptionId", "dbo.InscriptionCours");
            DropForeignKey("dbo.ProgressionModules", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.ChoixReponses", "QuestionQuizId", "dbo.QuestionQuizs");
            DropIndex("dbo.ReponseQuizs", new[] { "ChoixId" });
            DropIndex("dbo.ReponseQuizs", new[] { "QuestionQuizId" });
            DropIndex("dbo.ReponseQuizs", new[] { "TentativeId" });
            DropIndex("dbo.TentativeQuizs", new[] { "Etudiant_UtilisateurId" });
            DropIndex("dbo.TentativeQuizs", new[] { "QuizId" });
            DropIndex("dbo.RessourceModules", new[] { "ModuleId" });
            DropIndex("dbo.RessourceCours", new[] { "CoursId" });
            DropIndex("dbo.Transactions", new[] { "EtudiantId" });
            DropIndex("dbo.MessageClavardages", new[] { "SessionId" });
            DropIndex("dbo.SessionClavardages", new[] { "DomaineId" });
            DropIndex("dbo.SessionClavardages", new[] { "TuteurId" });
            DropIndex("dbo.SessionClavardages", new[] { "EtudiantId" });
            DropIndex("dbo.Reponses", new[] { "TuteurId" });
            DropIndex("dbo.Reponses", new[] { "QuestionId" });
            DropIndex("dbo.RencontrePhysiques", new[] { "DomaineId" });
            DropIndex("dbo.RencontrePhysiques", new[] { "TuteurId" });
            DropIndex("dbo.RencontrePhysiques", new[] { "EtudiantId" });
            DropIndex("dbo.TuteurDomaines", new[] { "DomaineId" });
            DropIndex("dbo.TuteurDomaines", new[] { "TuteurId" });
            DropIndex("dbo.QuestionCours", new[] { "Tuteur_UtilisateurId" });
            DropIndex("dbo.QuestionCours", new[] { "ModuleId" });
            DropIndex("dbo.QuestionCours", new[] { "InscriptionId" });
            DropIndex("dbo.ProgressionModules", new[] { "ModuleId" });
            DropIndex("dbo.ProgressionModules", new[] { "InscriptionId" });
            DropIndex("dbo.InscriptionCours", new[] { "CoursId" });
            DropIndex("dbo.InscriptionCours", new[] { "EtudiantId" });
            DropIndex("dbo.Utilisateurs", new[] { "Username" });
            DropIndex("dbo.Utilisateurs", new[] { "Email" });
            DropIndex("dbo.Questions", new[] { "DomaineId" });
            DropIndex("dbo.Questions", new[] { "EtudiantId" });
            DropIndex("dbo.Cours", new[] { "DomaineId" });
            DropIndex("dbo.Cours", new[] { "TuteurId" });
            DropIndex("dbo.Cours", new[] { "Code" });
            DropIndex("dbo.Modules", new[] { "CoursId" });
            DropIndex("dbo.Quizs", new[] { "ModuleId" });
            DropIndex("dbo.QuestionQuizs", new[] { "QuizId" });
            DropIndex("dbo.ChoixReponses", new[] { "QuestionQuizId" });
            DropTable("dbo.ReponseQuizs");
            DropTable("dbo.TentativeQuizs");
            DropTable("dbo.RessourceModules");
            DropTable("dbo.RessourceCours");
            DropTable("dbo.Transactions");
            DropTable("dbo.MessageClavardages");
            DropTable("dbo.SessionClavardages");
            DropTable("dbo.Reponses");
            DropTable("dbo.RencontrePhysiques");
            DropTable("dbo.TuteurDomaines");
            DropTable("dbo.QuestionCours");
            DropTable("dbo.ProgressionModules");
            DropTable("dbo.InscriptionCours");
            DropTable("dbo.Utilisateurs");
            DropTable("dbo.Questions");
            DropTable("dbo.Domaines");
            DropTable("dbo.Cours");
            DropTable("dbo.Modules");
            DropTable("dbo.Quizs");
            DropTable("dbo.QuestionQuizs");
            DropTable("dbo.ChoixReponses");
        }
    }
}

