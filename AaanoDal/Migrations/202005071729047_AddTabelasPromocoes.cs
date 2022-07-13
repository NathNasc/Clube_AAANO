namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTabelasPromocoes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Promocoes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        IdLojaParceira = c.Guid(nullable: false),
                        Resumo = c.String(nullable: false, maxLength: 150, unicode: false),
                        Detalhes = c.String(maxLength: 10000, unicode: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LojasParceiras", t => t.IdLojaParceira, cascadeDelete: true)
                .Index(t => t.Id, name: "ix_IdPromocao")
                .Index(t => t.IdLojaParceira);
            
            CreateTable(
                "dbo.PromocoesPlanos",
                c => new
                    {
                        IdPlanoPagSeguro = c.Guid(nullable: false),
                        IdPromocao = c.Guid(nullable: false),
                        Id = c.Guid(nullable: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => new { t.IdPlanoPagSeguro, t.IdPromocao })
                .ForeignKey("dbo.PlanosPagSeguro", t => t.IdPlanoPagSeguro, cascadeDelete: true)
                .ForeignKey("dbo.Promocoes", t => t.IdPromocao, cascadeDelete: true)
                .Index(t => t.IdPlanoPagSeguro, name: "ix_IdPlano")
                .Index(t => t.IdPromocao);
            
            CreateIndex("dbo.LojasParceiras", "Id", name: "ix_IdLojaParceira");
            CreateIndex("dbo.PermissoesUsuarios", new[] { "IdUsuario", "IdLojaParceira" }, name: "ix_UsuarioLoja");
            CreateIndex("dbo.PlanosPagSeguro", "Id", name: "ix_IdPlanoPagSeguro");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PromocoesPlanos", "IdPromocao", "dbo.Promocoes");
            DropForeignKey("dbo.PromocoesPlanos", "IdPlanoPagSeguro", "dbo.PlanosPagSeguro");
            DropForeignKey("dbo.Promocoes", "IdLojaParceira", "dbo.LojasParceiras");
            DropIndex("dbo.PromocoesPlanos", new[] { "IdPromocao" });
            DropIndex("dbo.PromocoesPlanos", "ix_IdPlano");
            DropIndex("dbo.Promocoes", new[] { "IdLojaParceira" });
            DropIndex("dbo.Promocoes", "ix_IdPromocao");
            DropIndex("dbo.PlanosPagSeguro", "ix_IdPlanoPagSeguro");
            DropIndex("dbo.PermissoesUsuarios", "ix_UsuarioLoja");
            DropIndex("dbo.LojasParceiras", "ix_IdLojaParceira");
            DropTable("dbo.PromocoesPlanos");
            DropTable("dbo.Promocoes");
            CreateIndex("dbo.PermissoesUsuarios", "IdUsuario");
            CreateIndex("dbo.PermissoesUsuarios", "IdLojaParceira");
        }
    }
}
