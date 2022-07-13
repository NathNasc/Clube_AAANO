namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TabelaBrindes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brindes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 150, unicode: false),
                        IdAssinaturaPagSeguro = c.Guid(nullable: false),
                        Sorteio = c.DateTime(storeType: "date"),
                        Entregue = c.Boolean(nullable: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssinaturasPagSeguro", t => t.IdAssinaturaPagSeguro, cascadeDelete: true)
                .Index(t => t.IdAssinaturaPagSeguro);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Brindes", "IdAssinaturaPagSeguro", "dbo.AssinaturasPagSeguro");
            DropIndex("dbo.Brindes", new[] { "IdAssinaturaPagSeguro" });
            DropTable("dbo.Brindes");
        }
    }
}
