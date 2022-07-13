var metodoSelecaoLojaParceira = "SelecionarLojaParceira";

// Busca e preenche a lista encontrados
function BuscarLojasParceirasParaSelecao(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarLojasSelecao", "divPaginasSelecaoLoja", "paginasLoja");
    ExibirCarregando("divCarregando");
    $("#tblSelecionarLoja tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/LojaParceira/ObterListaFiltradaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltrosLojasParceirasSelecao(),
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
                    text: "Ocorreu um problema ao fazer a pesquisa de lojas. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de lojas. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas lojas com os filtros preenchidos", "Pesquisa de lojas");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        $("#tblSelecionarLoja tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Nome + "</td>"
                            + "<td>" + FormatarTelefone(retornoDto.ListaEntidades[i].Telefone) + "</td>"
                            + "<td><button class='btn btn-sm btn-default' onclick='" + metodoSelecaoLojaParceira + "(\""
                            + retornoDto.ListaEntidades[i].Id + "\", \""
                            + retornoDto.ListaEntidades[i].Nome + "\")' type='button'><i class='fa fa-check'></i></button>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarLojasParaSelecao", "divPaginasSelecaoLoja", "paginasLoja");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de lojas. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Lê os filtros preenchidos na tela
function LerFiltrosLojasParceirasSelecao() {
    var listaFiltros = [];

    if ($("#txtNomeLojaSelecao").val() !== null && $("#txtNomeLojaSelecao").val().length > 0 && $("#txtNomeLojaSelecao").val() !== "") {
        listaFiltros.push({
            key: "NOME",
            value: $("#txtNomeLojaSelecao").val()
        });
    }

    if ($("#txtTelefoneLojaSelecao").val() !== null && $("#txtTelefoneLojaSelecao").val().length > 0 && $("#txtTelefoneLojaSelecao").val() !== "") {
        listaFiltros.push({
            key: "TELEFONE",
            value: $("#txtTelefoneLojaSelecao").val()
        });
    }

    return listaFiltros;
}

// Faz a pesquisa inicial e abre a modal
function ExibirModelPesquisaLojaParceira() {
    $("#txtTelefoneLojaSelecao").val("");
    $("#txtNomeLojaSelecao").val("");
    setTimeout(function () { $('input[id="txtNomeLojaSelecao"]').focus() }, 800);

    BuscarLojasParceirasParaSelecao(1);
    $("#modalSelecionarLoja").modal({ backdrop: 'static', keyboard: false });
}

