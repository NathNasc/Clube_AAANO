namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTabelaModelosEmail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModelosEmail",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Assunto = c.String(nullable: false, maxLength: 150, unicode: false),
                        Corpo = c.String(maxLength: 50000, unicode: false, storeType: "TEXT"),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, name: "ix_IdModeloEmail", anonymousArguments: new { Type = "BTrees" });
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ModelosEmail", "ix_IdModeloEmail");
            DropTable("dbo.ModelosEmail");
        }
    }
}
