using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class PlanoPagSeguroRestricoes : EntityTypeConfiguration<PlanoPagSeguroVo>
    {
        public PlanoPagSeguroRestricoes()
        {
            ToTable("PlanosPagSeguro");
            HasKey(p => p.Id);

            this.Property(p=>p.Id)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_IdPlanoPagSeguro", 1)));

            this.Property(p => p.Nome)
            .HasMaxLength(100)
            .IsRequired();

            this.Property(p => p.CodigoSimplificado)
            .HasMaxLength(40)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_CodigoPlano", 1) { IsUnique = true }))
            .IsRequired();
        }
    }
}
