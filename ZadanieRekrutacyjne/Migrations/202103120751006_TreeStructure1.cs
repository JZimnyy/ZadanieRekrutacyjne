namespace ZadanieRekrutacyjne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TreeStructure1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Trees", "Tree_TreeId", "dbo.Trees");
            DropIndex("dbo.Trees", new[] { "Tree_TreeId" });
            DropColumn("dbo.Trees", "Tree_TreeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Trees", "Tree_TreeId", c => c.Int());
            CreateIndex("dbo.Trees", "Tree_TreeId");
            AddForeignKey("dbo.Trees", "Tree_TreeId", "dbo.Trees", "TreeId");
        }
    }
}
