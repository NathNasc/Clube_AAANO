using AaanoBll.Base;
using AaanoBll.ClubeAaano;
using AaanoDto.ClubeAaano;
using AaanoDto.Requisicoes;
using AaanoDto.Retornos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Aaano
{
    [TestClass]
    public class UsuarioTestes
    {
        /// <summary>
        /// Retorna um usuario para teste
        /// </summary>
        /// <returns></returns>
        public UsuarioDto RetornarNovoUsuario()
        {
            UsuarioDto usuarioDto = new UsuarioDto()
            {
                Nome = "Usuario teste " + DateTime.Now.ToString(),
                Id = Guid.NewGuid(),
                Email = $"testes{DateTime.Now.Millisecond}@testes{DateTime.Now.Second}.com.br",
                Senha = "147852369",
            };

            return usuarioDto;
        }

        /// <summary>
        /// Testa a inclusão, obtenção, alteração e exclusão de um usuario
        /// </summary>
        [TestMethod]
        public void CrudUsuario()
        {
            RequisicaoEntidadeDto<UsuarioDto> requisicaoDto = new RequisicaoEntidadeDto<UsuarioDto>()
            {
                EntidadeDto = RetornarNovoUsuario()
            };

            string senhaCrip = "";
            UtilitarioBll.CriptografarSenha(requisicaoDto.EntidadeDto.Senha, ref senhaCrip);

            requisicaoDto.EntidadeDto.Senha = senhaCrip;

            Assert.IsTrue(Utilitario.RetornarAutenticacaoRequisicaoPreenchida(requisicaoDto));
            UsuarioBll usuarioBll = new UsuarioBll(true);

            // Incluir
            RetornoDto retornoDto = new RetornoDto();
            usuarioBll.Incluir(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Editar
            requisicaoDto.EntidadeDto.Nome = "Usuario atualizado " + DateTime.Now;
            usuarioBll.Editar(requisicaoDto, ref retornoDto);
            Assert.AreEqual(true, retornoDto.Retorno);

            // Obter
            RetornoObterDto<UsuarioDto> retornoObterDto = new RetornoObterDto<UsuarioDto>();
            RequisicaoObterDto requisicaoObterDto = new RequisicaoObterDto()
            {
                Id = requisicaoDto.EntidadeDto.Id,
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao
            };

            usuarioBll.Obter(requisicaoObterDto, ref retornoObterDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Obter lista
            RequisicaoObterListaDto requisicaoObterListaDto = new RequisicaoObterListaDto()
            {
                CampoOrdem = "NOME",
                IdUsuario = requisicaoDto.IdUsuario,
                Identificacao = requisicaoDto.Identificacao,
                NumeroItensPorPagina = 2,
                Pagina = 1
            };

            RetornoObterListaDto<UsuarioDto> retornoObterListaDto = new RetornoObterListaDto<UsuarioDto>();
            usuarioBll.ObterListaFiltrada(requisicaoObterListaDto, ref retornoObterListaDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);

            // Excluir
            usuarioBll.Excluir(requisicaoObterDto, ref retornoDto);
            Assert.AreEqual(true, retornoObterDto.Retorno);
        }
    }
}
