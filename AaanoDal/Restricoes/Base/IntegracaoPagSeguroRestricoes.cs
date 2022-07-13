using AaanoVo.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.Base
{
    public class IntegracaoPagSeguroRestricoes : EntityTypeConfiguration<IntegracaoPagSeguroVo>
    {
        public IntegracaoPagSeguroRestricoes()
        {
            ToTable("IntegracaoPagSeguro");
            HasKey(p => new { p.Id });

            this.Property(p => p.ChaveRecurso)
            .HasMaxLength(100)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_Chave")));
        }
    }
}
