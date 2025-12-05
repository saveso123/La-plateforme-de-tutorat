namespace ThotPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddFAQAndChatFeatures : DbMigration
    {
        public override void Up()
        {
            // Ajouter les colonnes manquantes a la table Tuteur
            AddColumn("dbo.Utilisateurs", "EstDisponible", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Utilisateurs", "HeureDebut", c => c.Time(precision: 7));
            AddColumn("dbo.Utilisateurs", "HeureFin", c => c.Time(precision: 7));
            AddColumn("dbo.Utilisateurs", "TarifHoraire", c => c.Decimal(precision: 10, scale: 2));
            AddColumn("dbo.Utilisateurs", "JoursDisponibles", c => c.String(maxLength: 50));

            // Creer la table FAQ
            CreateTable(
                "dbo.FAQs",
                c => new
                {
                    FAQId = c.Int(nullable: false, identity: true),
                    Question = c.String(nullable: false, maxLength: 500),
                    Reponse = c.String(nullable: false),
                    DomaineId = c.Int(nullable: false),
                    NombreConsultations = c.Int(nullable: false, defaultValue: 0),
                    NombreUtile = c.Int(nullable: false, defaultValue: 0),
                    NombreNonUtile = c.Int(nullable: false, defaultValue: 0),
                    DateCreation = c.DateTime(nullable: false),
                    DateMiseAJour = c.DateTime(),
                    EstArchivee = c.Boolean(nullable: false, defaultValue: false),
                })
                .PrimaryKey(t => t.FAQId)
                .ForeignKey("dbo.Domaines", t => t.DomaineId)
                .Index(t => t.DomaineId);

            // Ajouter les colonnes manquantes aux sessions de clavardage
            AddColumn("dbo.SessionsClavardage", "Cout", c => c.Decimal(precision: 10, scale: 2, nullable: false, defaultValue: 0));
            AddColumn("dbo.SessionsClavardage", "NoteEtudiant", c => c.Int());
            AddColumn("dbo.SessionsClavardage", "Commentaire", c => c.String(maxLength: 500));

            // Ajouter les colonnes manquantes aux rencontres physiques
            AddColumn("dbo.RencontresPhysiques", "TarifPreferentiel", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.RencontresPhysiques", "DateConfirmation", c => c.DateTime());
            AddColumn("dbo.RencontresPhysiques", "NoteEtudiant", c => c.Int());
            AddColumn("dbo.RencontresPhysiques", "Commentaire", c => c.String(maxLength: 500));
        }

        public override void Down()
        {
            // Supprimer les colonnes ajoutees a Tuteur
            DropColumn("dbo.Utilisateurs", "JoursDisponibles");
            DropColumn("dbo.Utilisateurs", "TarifHoraire");
            DropColumn("dbo.Utilisateurs", "HeureFin");
            DropColumn("dbo.Utilisateurs", "HeureDebut");
            DropColumn("dbo.Utilisateurs", "EstDisponible");

            // Supprimer la table FAQ
            DropForeignKey("dbo.FAQs", "DomaineId", "dbo.Domaines");
            DropIndex("dbo.FAQs", new[] { "DomaineId" });
            DropTable("dbo.FAQs");

            // Supprimer les colonnes ajoutees aux sessions
            DropColumn("dbo.SessionsClavardage", "Commentaire");
            DropColumn("dbo.SessionsClavardage", "NoteEtudiant");
            DropColumn("dbo.SessionsClavardage", "Cout");

            // Supprimer les colonnes ajoutees aux rencontres
            DropColumn("dbo.RencontresPhysiques", "Commentaire");
            DropColumn("dbo.RencontresPhysiques", "NoteEtudiant");
            DropColumn("dbo.RencontresPhysiques", "DateConfirmation");
            DropColumn("dbo.RencontresPhysiques", "TarifPreferentiel");
        }
    }
}
