namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTabelaAssinaturas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssinaturasPagSeguro",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Criacao = c.DateTime(nullable: false, precision: 0),
                        CodigoSimplificado = c.String(maxLength: 1000, unicode: false),
                        Status = c.Int(nullable: false),
                        UltimoEnventoRegistrado = c.DateTime(nullable: false, precision: 0),
                        IdPlano = c.Guid(nullable: false),
                        ReferenciaPlano = c.String(maxLength: 1000, unicode: false),
                        Email = c.String(maxLength: 60, unicode: false),
                        NomeAssinate = c.String(maxLength: 50, unicode: false),
                        Telefone = c.String(maxLength: 11, unicode: false),
                        Estado = c.String(maxLength: 2, unicode: false),
                        Numero = c.String(maxLength: 20, unicode: false),
                        Pais = c.String(maxLength: 10, unicode: false),
                        Complemento = c.String(maxLength: 40, unicode: false),
                        Bairro = c.String(maxLength: 60, unicode: false),
                        Cidade = c.String(maxLength: 60, unicode: false),
                        Cep = c.String(maxLength: 11, unicode: false),
                        Logradouro = c.String(maxLength: 80, unicode: false),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Id, name: "ix_IdLojaParceira");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.AssinaturasPagSeguro", "ix_IdLojaParceira");
            DropTable("dbo.AssinaturasPagSeguro");
        }
    }
}
