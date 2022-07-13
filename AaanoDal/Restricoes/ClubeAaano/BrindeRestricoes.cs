using AaanoVo.ClubeAaano;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class BrindeRestricoes : EntityTypeConfiguration<BrindeVo>
    {
        public BrindeRestricoes()
        {
            ToTable("Brindes");
            HasKey(p => new { p.Id });

            this.Property(p => p.Id);

            this.Property(p => p.Descricao)
            .HasMaxLength(150)
            .IsRequired();

            this.Property(p => p.Sorteio)
            .HasColumnType("Date");

            this.HasOptional(p => p.Assinante).WithMany().HasForeignKey(p => p.IdAssinaturaPagSeguro);
        }
    }
}
