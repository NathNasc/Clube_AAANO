using AaanoVo.ClubeAaano;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class UsuarioRestricoes : EntityTypeConfiguration<UsuarioVo>
    {
        public UsuarioRestricoes()
        {
            ToTable("Usuarios");
            HasKey(p => p.Id);

            this.Property(p => p.Nome)
            .HasMaxLength(150)
            .IsRequired();

            this.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(100);

            this.Property(p => p.Senha)
            .IsRequired()
            .HasMaxLength(50);
        }
    }
}
