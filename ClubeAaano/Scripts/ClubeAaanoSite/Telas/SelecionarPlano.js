var metodoSelecaoPlano = "SelecionarPlano";

// Busca e preenche a lista encontrados
function BuscarPlanosParaSelecao(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarPlanosSelecao", "divPaginasSelecaoPlano", "paginasPlano");
    ExibirCarregando("divCarregando");
    $("#tblSelecionarPlano tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/PlanoPagSeguro/ObterListaFiltradaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltrosPlanosSelecao(),
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
                    text: "Ocorreu um problema ao fazer a pesquisa de planos. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de planos. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas planos com os filtros preenchidos", "Pesquisa de planos");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        $("#tblSelecionarPlano tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Nome + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].CodigoSimplificado + "</td>"
                            + "<td><button class='btn btn-sm btn-default' onclick='" + metodoSelecaoPlano + "(\""
                            + retornoDto.ListaEntidades[i].Id + "\", \""
                            + retornoDto.ListaEntidades[i].Nome + "\", \""
                            + retornoDto.ListaEntidades[i].CodigoSimplificado + "\")' type='button'><i class='fa fa-check'></i></button>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarPlanosParaSelecao", "divPaginasSelecaoPlano", "paginasPlano");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de planos. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Lê os filtros preenchidos na tela
function LerFiltrosPlanosSelecao() {
    var listaFiltros = [];

    if ($("#txtNomePlanoSelecao").val() !== null && $("#txtNomePlanoSelecao").val().length > 0 && $("#txtNomePlanoSelecao").val() !== "") {
        listaFiltros.push({
            key: "NOME",
            value: $("#txtNomePlanoSelecao").val()
        });
    }

    return listaFiltros;
}

// Faz a pesquisa inicial e abre a modal
function ExibirModelPesquisaPlano() {
    $("#txtNomePlanoSelecao").val("");
    setTimeout(function () { $('input[id="txtNomePlanoSelecao"]').focus() }, 500);

    BuscarPlanosParaSelecao(1);
    $("#modalSelecionarPlano").modal({ backdrop: 'static', keyboard: false });
}
