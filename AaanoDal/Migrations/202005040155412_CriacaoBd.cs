namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoBd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LojasParceiras",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nome = c.String(nullable: false, maxLength: 150, unicode: false),
                        Endereco = c.String(maxLength: 200, unicode: false),
                        Telefone = c.String(nullable: false, maxLength: 15, unicode: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PermissoesUsuarios",
                c => new
                    {
                        IdLojaParceira = c.Guid(nullable: false),
                        IdUsuario = c.Guid(nullable: false),
                        Id = c.Guid(nullable: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => new { t.IdLojaParceira, t.IdUsuario })
                .ForeignKey("dbo.LojasParceiras", t => t.IdLojaParceira, cascadeDelete: true)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdLojaParceira)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nome = c.String(nullable: false, maxLength: 150, unicode: false),
                        Email = c.String(nullable: false, maxLength: 100, unicode: false),
                        Senha = c.String(nullable: false, maxLength: 50, unicode: false),
                        Administrador = c.Boolean(nullable: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlanosPagSeguro",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nome = c.String(nullable: false, maxLength: 100, unicode: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PermissoesUsuarios", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.PermissoesUsuarios", "IdLojaParceira", "dbo.LojasParceiras");
            DropIndex("dbo.PermissoesUsuarios", new[] { "IdUsuario" });
            DropIndex("dbo.PermissoesUsuarios", new[] { "IdLojaParceira" });
            DropTable("dbo.PlanosPagSeguro");
            DropTable("dbo.Usuarios");
            DropTable("dbo.PermissoesUsuarios");
            DropTable("dbo.LojasParceiras");
        }
    }
}
