namespace AaanoDal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrigirIdAssinanteBrindes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Brindes", "IdAssinaturaPagSeguro", "AssinaturasPagSeguro");
            DropIndex("Brindes", new[] { "IdAssinaturaPagSeguro" });
            AlterColumn("Brindes", "IdAssinaturaPagSeguro", c => c.Guid());
            CreateIndex("Brindes", "IdAssinaturaPagSeguro");
            AddForeignKey("Brindes", "IdAssinaturaPagSeguro", "AssinaturasPagSeguro", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Brindes", "IdAssinaturaPagSeguro", "AssinaturasPagSeguro");
            DropIndex("Brindes", new[] { "IdAssinaturaPagSeguro" });
            AlterColumn("Brindes", "IdAssinaturaPagSeguro", c => c.Guid(nullable: false));
            CreateIndex("Brindes", "IdAssinaturaPagSeguro");
            AddForeignKey("Brindes", "IdAssinaturaPagSeguro", "AssinaturasPagSeguro", "Id", cascadeDelete: true);
        }
    }
}
