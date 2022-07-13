using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class ResgatePromocaoRestricoes : EntityTypeConfiguration<ResgatePromocaoVo>
    {
        public ResgatePromocaoRestricoes()
        {
            ToTable("ResgatesPromocoes");
            HasKey(p => new { p.Id });

            this.Property(p => p.IdAssinaturaPagSeguro)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_IdAssinatura")));

            this.Property(p => p.CodigoSimplificadoAssinatura)
            .HasMaxLength(100)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_CodigoAssinatura")));

            this.Property(p => p.NomeUsuarioResgate)
            .HasMaxLength(150);

            this.Property(p => p.Validade)
            .HasColumnType("Date");

            this.HasRequired(p => p.Promocao).WithMany().HasForeignKey(p => p.IdPromocao);
            this.HasRequired(p => p.AssinaturaPagSeguroVo).WithMany().HasForeignKey(p => p.IdAssinaturaPagSeguro);
        }
    }
}
