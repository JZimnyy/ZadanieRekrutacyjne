namespace ZadanieRekrutacyjne.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TreeStructure5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trees", "Parent_TreeId", c => c.Int());
            CreateIndex("dbo.Trees", "Parent_TreeId");
            AddForeignKey("dbo.Trees", "Parent_TreeId", "dbo.Trees", "TreeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trees", "Parent_TreeId", "dbo.Trees");
            DropIndex("dbo.Trees", new[] { "Parent_TreeId" });
            DropColumn("dbo.Trees", "Parent_TreeId");
        }
    }
}
