using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class AssinaturaPagSeguroRestricoes : EntityTypeConfiguration<AssinaturaPagSeguroVo>
    {
        public AssinaturaPagSeguroRestricoes()
        {
            ToTable("AssinaturasPagSeguro");
            HasKey(p => p.Id);

            this.Property(p => p.Id)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_IdLojaParceira", 1)));

            this.Property(p => p.Bairro)
            .HasMaxLength(60);

            this.Property(p => p.Cep)
            .HasMaxLength(11);

            this.Property(p => p.Cidade)
            .HasMaxLength(60);

            this.Property(p => p.Complemento)
            .HasMaxLength(40);

            this.Property(p => p.Email)
            .HasMaxLength(60);

            this.Property(p => p.Estado)
            .HasMaxLength(2);

            this.Property(p => p.Logradouro)
            .HasMaxLength(80);

            this.Property(p => p.NomeAssinate)
            .HasMaxLength(50);

            this.Property(p => p.Numero)
            .HasMaxLength(20);

            this.Property(p => p.Pais)
            .HasMaxLength(10);

            this.Property(p => p.Telefone)
            .HasMaxLength(11);
        }
    }
}
