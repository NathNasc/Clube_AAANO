namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTabelaEmailEnviado : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailsEnviados",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SucessoEnvio = c.Boolean(nullable: false),
                        IdModeloEmail = c.Guid(nullable: false),
                        IdAssinaturaPagSeguro = c.Guid(nullable: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssinaturasPagSeguro", t => t.IdAssinaturaPagSeguro, cascadeDelete: true)
                .ForeignKey("dbo.ModelosEmail", t => t.IdModeloEmail, cascadeDelete: true)
                .Index(t => t.IdModeloEmail)
                .Index(t => t.IdAssinaturaPagSeguro, name: "ix_IdAssinaturaEmail");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailsEnviados", "IdModeloEmail", "dbo.ModelosEmail");
            DropForeignKey("dbo.EmailsEnviados", "IdAssinaturaPagSeguro", "dbo.AssinaturasPagSeguro");
            DropIndex("dbo.EmailsEnviados", "ix_IdAssinaturaEmail");
            DropIndex("dbo.EmailsEnviados", new[] { "IdModeloEmail" });
            DropTable("dbo.EmailsEnviados");
        }
    }
}
