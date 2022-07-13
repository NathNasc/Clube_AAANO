using AaanoVo.ClubeAaano;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace AaanoDal.Restricoes.ClubeAaano
{
    public class PermissaoUsuarioRestricoes : EntityTypeConfiguration<PermissaoUsuarioVo>
    {
        public PermissaoUsuarioRestricoes()
        {
            ToTable("PermissoesUsuarios");
            HasKey(p => new { p.IdLojaParceira, p.IdUsuario });

            this.Property(p => p.IdUsuario)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_UsuarioLoja", 1)));

            this.Property(p => p.IdLojaParceira)
            .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("ix_UsuarioLoja", 2)));

            this.HasRequired(p => p.Usuario).WithMany().HasForeignKey(p => p.IdUsuario);
            this.HasRequired(p => p.LojaParceira).WithMany().HasForeignKey(p => p.IdLojaParceira);

        }
    }
}
