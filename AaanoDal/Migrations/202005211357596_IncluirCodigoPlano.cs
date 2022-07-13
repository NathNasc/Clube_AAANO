namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class IncluirCodigoPlano : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlanosPagSeguro", "CodigoSimplificado", c => c.String(nullable: false, maxLength: 40, unicode: false));
            CreateIndex("dbo.PlanosPagSeguro", "CodigoSimplificado", unique: true, name: "ix_CodigoPlano");
        }

        public override void Down()
        {
            DropIndex("dbo.PlanosPagSeguro", "ix_CodigoPlano");
            DropColumn("dbo.PlanosPagSeguro", "CodigoSimplificado");
        }
    }
}
