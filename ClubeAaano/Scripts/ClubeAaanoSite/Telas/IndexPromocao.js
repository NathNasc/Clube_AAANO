// Busca e preenche a lista de promoções encontrados
function BuscarPromocoes(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarPromocoes");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Promocao/ObterListaFiltradaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltros(),
            Pagina: nPagina,
            NaoPaginarPesquisa: false
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

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Resumo + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].NomeLojaParceira + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../Promocao/Visualizar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-primary' href='../Promocao/Editar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='../PromocaoPlano/EditarPorPromocao/"
                            + retornoDto.ListaEntidades[i].Id + "?resumoPromocao=" + retornoDto.ListaEntidades[i].Resumo
                            + "'><i class='fa fa-check-square'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../Promocao/Excluir/"
                            + retornoDto.ListaEntidades[i].Id + "?Descricao="
                            + retornoDto.ListaEntidades[i].Resumo + " da loja "
                            + retornoDto.ListaEntidades[i].NomeLojaParceira
                            + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarPromocoes");
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
function LerFiltros() {
    var listaFiltros = [];

    if ($("#txtResumo").val() !== null && $("#txtResumo").val().length > 0 && $("#txtResumo").val() !== "") {
        listaFiltros.push({
            key: "RESUMO",
            value: $("#txtResumo").val()
        });
    }

    if ($("#txtDetalhes").val() !== null && $("#txtDetalhes").val().length > 0 && $("#txtDetalhes").val() !== "") {
        listaFiltros.push({
            key: "DETALHES",
            value: $("#txtDetalhes").val()
        });
    }

    return listaFiltros;
}