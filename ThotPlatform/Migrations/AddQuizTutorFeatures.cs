namespace ThotPlatform.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuizTutorFeatures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TentativesQuiz", "EstCorrigee", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.TentativesQuiz", "DateCreation", c => c.DateTime(nullable: false, defaultValue: DateTime.Now));
            AddColumn("dbo.ReponsesQuiz", "PointsAccordes", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReponsesQuiz", "PointsAccordes");
            DropColumn("dbo.TentativesQuiz", "DateCreation");
            DropColumn("dbo.TentativesQuiz", "EstCorrigee");
        }
    }
}

