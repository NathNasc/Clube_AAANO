var metodoSelecaoAssinatura = "SelecionarAssinatura";

// Busca e preenche a lista encontrados
function BuscarAssinaturasParaSelecao(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarAssinaturasSelecao", "divPaginasSelecaoAssinante", "paginasSelecaoAssinante");
    ExibirCarregando("divCarregando");
    $("#tblSelecionarAssinatura tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/AssinaturaPagSeguro/ObterListaFiltradaSimplificadaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltrosAssinaturasSelecao(),
            Pagina: nPagina,
            NaoPaginarPesquisa: false,
            CampoOrdem: "NOME",
            NumeroItensPorPagina: 5
        }),
        traditional: true,
        success: function (retornoDto) {
            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de assinaturas. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de assinaturas. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas assinaturas com os filtros preenchidos", "Pesquisa de assinaturas");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        $("#tblSelecionarAssinatura tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].NomeAssinate + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].ReferenciaPlano + "</td>"
                            + "<td>" + FormatarTelefone(retornoDto.ListaEntidades[i].Telefone) + "</td>"
                            + "<td><button class='btn btn-sm btn-default' onclick='" + metodoSelecaoAssinatura + "(\""
                            + retornoDto.ListaEntidades[i].CodigoSimplificado + "\", \""
                            + retornoDto.ListaEntidades[i].NomeAssinate + "\", \""
                            + retornoDto.ListaEntidades[i].Id + "\")' type='button'><i class='fa fa-check'></i></button>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarAssinaturasParaSelecao", "divPaginasSelecaoAssinante", "paginasSelecaoAssinante");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de assinaturas. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Lê os filtros preenchidos na tela
function LerFiltrosAssinaturasSelecao() {
    var listaFiltros = [];

    if ($("#txtNomeAssinanteSelecao").val() !== null && $("#txtNomeAssinanteSelecao").val().length > 0 && $("#txtNomeAssinanteSelecao").val() !== "") {
        listaFiltros.push({
            key: "NOMEASSINANTE",
            value: $("#txtNomeAssinanteSelecao").val()
        });
    }

    if ($("#txtTelefoneAssinanteSelecao").val() !== null && $("#txtTelefoneAssinanteSelecao").val().length > 0 && $("#txtTelefoneAssinanteSelecao").val() !== "") {
        listaFiltros.push({
            key: "TELEFONEASSINANTE",
            value: $("#txtTelefoneAssinanteSelecao").val()
        });
    }

    return listaFiltros;
}

// Faz a pesquisa inicial e abre a modal
function ExibirModelPesquisaAssinatura() {
    $("#txtTelefoneAssinanteSelecao").val("");
    $("#txtNomeAssinanteSelecao").val("");
    setTimeout(function () { $('input[id="txtNomeAssinanteSelecao"]').focus() }, 500);

    BuscarAssinaturasParaSelecao(1);
    $("#modalSelecionarAssinatura").modal({ backdrop: 'static', keyboard: false });
}
