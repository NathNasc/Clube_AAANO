using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class ModeloEmailRestricoes : EntityTypeConfiguration<ModeloEmailVo>
    {
        public ModeloEmailRestricoes()
        {
            ToTable("ModelosEmail");
            HasKey(p => new { p.Id });

            this.Property(p => p.Id)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_IdModeloEmail", 1)));

            this.Property(p => p.Assunto)
            .HasMaxLength(150)
            .IsRequired();

            this.Property(p => p.Corpo)
            .HasMaxLength(50000)
            .HasColumnType("TEXT");
        }
    }
}
