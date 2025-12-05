namespace ThotPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutIdentifiantUniqueEtLangue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Utilisateurs", "IdentifiantUnique", c => c.String(maxLength: 20));
            AddColumn("dbo.Utilisateurs", "LanguePreferee", c => c.String(maxLength: 10, defaultValue: "fr"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Utilisateurs", "LanguePreferee");
            DropColumn("dbo.Utilisateurs", "IdentifiantUnique");
        }
    }
}

