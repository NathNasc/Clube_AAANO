// Busca e preenche a lista de brindes encontrados
function BuscarBrindes(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarBrindes");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Brinde/ObterListaFiltradaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltros(),
            CampoOrdem: $("#optCampoOrdem").val(),
            Pagina: nPagina,
            NaoPaginarPesquisa: false
        }),
        traditional: true,
        success: function (retornoDto) {

            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de brindes. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de brindes. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas brindes com os filtros preenchidos", "Pesquisa de brindes");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        var entregue = "Não";
                        if (retornoDto.ListaEntidades[i].Entregue) {
                            entregue = "Sim";
                        }

                        var sorteio = "";
                        if (retornoDto.ListaEntidades[i].NomeAssinante.length <= 0 || retornoDto.ListaEntidades[i].NomeAssinante == null) {
                            sorteio = " <a class='btn btn-sm btn-info' tittle='Sortear' href='../Brinde/SortearExistente/"
                                + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-trophy'></i></a>"
                        }

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Descricao + "</td>"
                            + "<td>" + ConverterDataJson(retornoDto.ListaEntidades[i].Sorteio) + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].NomeAssinante + "</td>"
                            + "<td>" + entregue + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../Brinde/Visualizar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-primary' href='../Brinde/Editar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../Brinde/Excluir/"
                            + retornoDto.ListaEntidades[i].Id + "?Descricao="
                            + retornoDto.ListaEntidades[i].Descricao + (retornoDto.ListaEntidades[i].Sorteio == null ? "" : " (" + ConverterDataJson(retornoDto.ListaEntidades[i].Sorteio) + ")")
                            + "'><i class='fa fa-trash'></i></a>" + sorteio
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarBrindes");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de brindes. \n"
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

    if ($("#txtSorteioInicial").val() !== null && $("#txtSorteioInicial").val().length > 0 && $("#txtSorteioInicial").val() !== "") {
        listaFiltros.push({
            key: "SORTEIOINICIO",
            value: $("#txtSorteioInicial").val()
        });
    }

    if ($("#txtSorteioFinal").val() !== null && $("#txtSorteioFinal").val().length > 0 && $("#txtSorteioFinal").val() !== "") {
        listaFiltros.push({
            key: "SORTEIOFIM",
            value: $("#txtSorteioFinal").val()
        });
    }

    if ($("#txtIdAssinatura").val() !== null && $("#txtIdAssinatura").val().length > 0 && $("#txtIdAssinatura").val() !== "") {
        listaFiltros.push({
            key: "IDASSINANTE",
            value: $("#txtIdAssinatura").val()
        });
    }

    if ($("#txtDescricao").val() !== null && $("#txtDescricao").val().length > 0 && $("#txtDescricao").val() !== "") {
        listaFiltros.push({
            key: "DESCRICAO",
            value: $("#txtDescricao").val()
        });
    }

    if ($("#chEntregue").is(':checked')) {
        listaFiltros.push({
            key: "APENASPENDENTESDEENTREGA",
            value: "true"
        });
    }

    return listaFiltros;
}

// Ao selecionar uma promoção para filtrar
function SelecionarAssinatura(codigo, nome, id) {
    $("#txtIdAssinatura").val(id);
    $("#txtNomeAssinante").val(nome);
    $("#btnBuscarAssinatura").html("<i class='fa fa-eraser'></i> Limpar");
    $("#modalSelecionarAssinatura").modal("hide");
}
