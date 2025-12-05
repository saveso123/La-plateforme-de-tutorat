namespace ThotPlatform.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddMessagesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        ExpediteurId = c.Int(nullable: false, defaultValue: 0),
                        DestinatireId = c.Int(nullable: false, defaultValue: 0),
                        Sujet = c.String(nullable: false, maxLength: 200),
                        Contenu = c.String(nullable: false),
                        DateEnvoi = c.DateTime(nullable: false),
                        EstLu = c.Boolean(nullable: false),
                        DateLecture = c.DateTime(),
                        ConversationId = c.Int(),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Utilisateurs", t => t.ExpediteurId)
                .ForeignKey("dbo.Utilisateurs", t => t.DestinatireId)
                .Index(t => t.ExpediteurId)
                .Index(t => t.DestinatireId);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "DestinatireId", "dbo.Utilisateurs");
            DropForeignKey("dbo.Messages", "ExpediteurId", "dbo.Utilisateurs");
            DropIndex("dbo.Messages", new[] { "DestinatireId" });
            DropIndex("dbo.Messages", new[] { "ExpediteurId" });
            DropTable("dbo.Messages");
        }
    }
}

