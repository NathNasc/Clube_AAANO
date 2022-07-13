namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TabelaResgates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResgatesPromocoes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IdPromocao = c.Guid(nullable: false),
                        IdAssinaturaPagSeguro = c.Guid(nullable: false),
                        CodigoSimplificadoAssinatura = c.String(maxLength: 1000, unicode: false),
                        Resgate = c.DateTime(precision: 0),
                        Validade = c.DateTime(nullable: false, storeType: "date"),
                        NomeUsuarioResgate = c.String(maxLength: 150, unicode: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssinaturasPagSeguro", t => t.IdAssinaturaPagSeguro, cascadeDelete: true)
                .ForeignKey("dbo.Promocoes", t => t.IdPromocao, cascadeDelete: true)
                .Index(t => t.IdPromocao)
                .Index(t => t.IdAssinaturaPagSeguro, name: "ix_IdAssinatura")
                .Index(t => t.CodigoSimplificadoAssinatura, name: "ix_CodigoAssinatura");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResgatesPromocoes", "IdPromocao", "dbo.Promocoes");
            DropForeignKey("dbo.ResgatesPromocoes", "IdAssinaturaPagSeguro", "dbo.AssinaturasPagSeguro");
            DropIndex("dbo.ResgatesPromocoes", "ix_CodigoAssinatura");
            DropIndex("dbo.ResgatesPromocoes", "ix_IdAssinatura");
            DropIndex("dbo.ResgatesPromocoes", new[] { "IdPromocao" });
            DropTable("dbo.ResgatesPromocoes");
        }
    }
}
