using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class EmailEnviadoRestricoes : EntityTypeConfiguration<EmailEnviadoVo>
    {
        public EmailEnviadoRestricoes()
        {
            ToTable("EmailsEnviados");
            HasKey(p => new { p.Id });

            this.Property(p => p.IdAssinaturaPagSeguro)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_IdAssinaturaEmail")));

            this.HasRequired(p => p.ModeloEmail).WithMany().HasForeignKey(p => p.IdModeloEmail);
            this.HasRequired(p => p.Assinatura).WithMany().HasForeignKey(p => p.IdAssinaturaPagSeguro);
        }
    }
}
