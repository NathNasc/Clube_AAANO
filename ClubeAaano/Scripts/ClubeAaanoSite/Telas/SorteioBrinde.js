var assinaturas = [];

// Busca e preenche a lista de brindes encontrados
function ObterAssinaturasAtivas() {

    ExibirCarregando("divCarregando");

    var filtros = [];
    filtros.push({
        key: "STATUS",
        value: "2"
    });

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/AssinaturaPagSeguro/ObterListaFiltradaSimplificadaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: filtros,
            CampoOrdem: "NOMEASSINANTE",
            Pagina: 0,
            NaoPaginarPesquisa: true
        }),
        traditional: true,
        success: function (retornoDto) {

            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao obter as assinaturas. \n"
                        + "\n Mensagem de retorno: " + retornoDto.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregando");
                return;
            }

            if (!retornoDto.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao obter as assinaturas. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {
                assinaturas = retornoDto.ListaEntidades;
                EsconderCarregando("divCarregando");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao obter as assinaturas. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Iniciar o sorteio dos nomes
var randTimer;
function Sortear() {
    var i = 0;
    randTimer = setInterval(function () {
        if (i >= 50) {
            clearInterval(randTimer);
            $("#NomeAssinante").val($("#txtNomeSorteio").html());
            $("#btnGravar").removeAttr("Disabled");
        } else {
            var indice = Math.floor(Math.random() * assinaturas.length)
            $("#txtNomeSorteio").html(assinaturas[indice].NomeAssinate);
            $("#IdAssinaturaPagSeguro").val(assinaturas[indice].Id);
            i++;
        }
    }, 50);
}
