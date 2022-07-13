namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TabelaIntegracaoPagSeguro : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IntegracaoPagSeguro",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ChaveRecurso = c.String(maxLength: 100, unicode: false),
                        UltimaSincronizacao = c.DateTime(nullable: false, precision: 0),
                        DataInclusao = c.DateTime(nullable: false, precision: 0),
                        DataAlteracao = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ChaveRecurso, name: "ix_Chave");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.IntegracaoPagSeguro", "ix_Chave");
            DropTable("dbo.IntegracaoPagSeguro");
        }
    }
}
