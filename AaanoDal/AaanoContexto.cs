using AaanoDal.Restricoes.Base;
using AaanoDal.Restricoes.ClubeAaano;
using AaanoVo.Base;
using AaanoVo.ClubeAaano;
using MySql.Data.Entity;
using System;
using System.Data.Entity;

namespace AaanoDal
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class AaanoContexto : DbContext
    {
        private static string strConexao = "Server=localhost;port=3306;Database=bd;Uid=Nath;Pwd=pass;includesecurityasserts=true";

        public AaanoContexto() : base(strConexao)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<PromocaoVo> Promocao { get; set; }
        public DbSet<PromocaoPlanoVo> PromocaoLoja { get; set; }
        public DbSet<UsuarioVo> Usuario { get; set; }
        public DbSet<LojaParceiraVo> LojaParceira { get; set; }
        public DbSet<PermissaoUsuarioVo> PermissaoUsuario { get; set; }
        public DbSet<PlanoPagSeguroVo> PlanoPagSeguro { get; set; }
        public DbSet<AssinaturaPagSeguroVo> AssinaturaPagSeguro { get; set; }
        public DbSet<IntegracaoPagSeguroVo> IntegracaoPagSeguro { get; set; }
        public DbSet<ResgatePromocaoVo> ResgatePromocao { get; set; }
        public DbSet<BrindeVo> Brinde { get; set; }
        public DbSet<ModeloEmailVo> ModeloEmail { get; set; }
        public DbSet<EmailEnviadoVo> EmailEnviado { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);
            modelbuilder.Properties<string>().Configure(p => p.HasColumnType("varchar"));

            modelbuilder.Configurations.Add(new PermissaoUsuarioRestricoes());
            modelbuilder.Configurations.Add(new LojaParceiraRestricoes());
            modelbuilder.Configurations.Add(new PlanoPagSeguroRestricoes());
            modelbuilder.Configurations.Add(new UsuarioRestricoes());
            modelbuilder.Configurations.Add(new PromocaoRestricoes());
            modelbuilder.Configurations.Add(new PromocaoPlanoRestricoes());
            modelbuilder.Configurations.Add(new AssinaturaPagSeguroRestricoes());
            modelbuilder.Configurations.Add(new IntegracaoPagSeguroRestricoes());
            modelbuilder.Configurations.Add(new ResgatePromocaoRestricoes());
            modelbuilder.Configurations.Add(new BrindeRestricoes());
            modelbuilder.Configurations.Add(new ModeloEmailRestricoes());
            modelbuilder.Configurations.Add(new EmailEnviadoRestricoes());

            //DbConfiguration.SetConfiguration(new MySqlEFConfiguration());

        }

        /// <summary>
        /// Salva todas as mudanças feitas na transação
        /// </summary>
        public bool Salvar(ref string mensagemErro)
        {
            try
            {
                base.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.ToString().Contains("duplicada"))
                {
                    mensagemErro = "Esse cadastro já existe, não é possível incluir cadastros duplicados.";
                    return false;
                }
                if (ex.InnerException.ToString().Contains("REFERENCE"))
                {
                    mensagemErro = "Existem cadastros que estão utilizando este cadastro.";
                    return false;
                }
                else
                {
                    mensagemErro = "Erro ao salvar as alterações no banco de dados: " + ex.InnerException;
                    return false;
                }
            }
        }
    }
}
