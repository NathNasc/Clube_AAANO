using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class PromocaoPlanoRestricoes : EntityTypeConfiguration<PromocaoPlanoVo>
    {
        public PromocaoPlanoRestricoes()
        {
            ToTable("PromocoesPlanos");
            HasKey(p => new { p.IdPlanoPagSeguro, p.IdPromocao });

            this.Property(p => p.IdPlanoPagSeguro)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_IdPlano")));

            this.HasRequired(p => p.Promocao).WithMany().HasForeignKey(p => p.IdPromocao);
            this.HasRequired(p => p.PlanoPagSeguro).WithMany().HasForeignKey(p => p.IdPlanoPagSeguro);
        }
    }
}
