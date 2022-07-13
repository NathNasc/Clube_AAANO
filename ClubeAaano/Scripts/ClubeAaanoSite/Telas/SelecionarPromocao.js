var metodoSelecaoPromocao = "SelecionarPromocao";

// Busca e preenche a lista encontrados
function BuscarPromocoesParaSelecao(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarPromocoesSelecao", "divPaginasSelecaoPromocao", "paginasPromocao");
    ExibirCarregando("divCarregando");
    $("#tblSelecionarPromocao tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Promocao/ObterListaFiltradaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltrosPromocoesSelecao(),
            Pagina: nPagina,
            NaoPaginarPesquisa: false,
            CampoOrdem: "RESUMO",
            NumeroItensPorPagina: 5
        }),
        traditional: true,
        success: function (retornoDto) {
            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de promoções. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de promoções. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas promoções com os filtros preenchidos", "Pesquisa de promoções");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        $("#tblSelecionarPromocao tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Resumo + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].NomeLojaParceira + "</td>"
                            + "<td><button class='btn btn-sm btn-default' onclick='" + metodoSelecaoPromocao + "(\""
                            + retornoDto.ListaEntidades[i].Id + "\", \"" +
                            retornoDto.ListaEntidades[i].Resumo + "\", \"" +
                            +retornoDto.ListaEntidades[i].NomeLojaParceira + "\")' type='button><i class='fa fa-check'></i></button>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarPromocoesParaSelecao", "divPaginasSelecaoPromocao", "paginasPromocao");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de promoções. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Lê os filtros preenchidos na tela
function LerFiltrosPromocoesSelecao() {
    var listaFiltros = [];

    if ($("#txtResumoPromocaoSelecao").val() !== null && $("#txtResumoPromocaoSelecao").val().length > 0 && $("#txtResumoPromocaoSelecao").val() !== "") {
        listaFiltros.push({
            key: "RESUMO",
            value: $("#txtResumoPromocaoSelecao").val()
        });
    }

    return listaFiltros;
}

// Faz a pesquisa inicial e abre a modal
function ExibirModelPesquisaPromocao() {
    $("#txtResumoPromocaoSelecao").val("");
    setTimeout(function () { $('input[id="txtResumoPromocaoSelecao"]').focus() }, 500);

    BuscarPromocoesParaSelecao(1);
    $("#modalSelecionarPromocao").modal({ backdrop: 'static', keyboard: false });
}
