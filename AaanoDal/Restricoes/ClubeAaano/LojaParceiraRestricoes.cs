using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class LojaParceiraRestricoes : EntityTypeConfiguration<LojaParceiraVo>
    {
        public LojaParceiraRestricoes()
        {
            ToTable("LojasParceiras");
            HasKey(p => p.Id);

            this.Property(p => p.Id)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_IdLojaParceira", 1)));

            this.Property(p => p.Nome)
            .HasMaxLength(150)
            .IsRequired();

            this.Property(p => p.Endereco)
            .HasMaxLength(200);

            this.Property(p => p.Telefone)
            .IsRequired()
            .HasMaxLength(15);
        }
    }
}
