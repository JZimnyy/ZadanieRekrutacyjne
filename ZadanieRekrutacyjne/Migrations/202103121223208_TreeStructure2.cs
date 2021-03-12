namespace ZadanieRekrutacyjne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TreeStructure2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Trees", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Trees", "Name", c => c.String());
        }
    }
}
