using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class PromocaoRestricoes : EntityTypeConfiguration<PromocaoVo>
    {
        public PromocaoRestricoes()
        {
            ToTable("Promocoes");
            HasKey(p => new { p.Id });

            this.Property(p => p.Id)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_IdPromocao", 1)));

            this.Property(p => p.Resumo)
            .HasMaxLength(150)
            .IsRequired();

            this.Property(p => p.Detalhes)
            .HasMaxLength(10000);

            this.HasRequired(p => p.LojaParceira).WithMany().HasForeignKey(p => p.IdLojaParceira);
        }
    }
}
