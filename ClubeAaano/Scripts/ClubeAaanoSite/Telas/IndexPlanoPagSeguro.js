// Busca e preenche a lista de planos encontrados
function BuscarPlanos(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarPlanos");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/PlanoPagSeguro/ObterListaFiltradaPaginada",
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
                    toastr.info("Não foram encontrados planos com os filtros preenchidos", "Pesquisa de planos");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Id + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].Nome + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].CodigoSimplificado + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../PlanoPagSeguro/Visualizar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-primary' href='../PlanoPagSeguro/Editar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../PlanoPagSeguro/Excluir/"
                            + retornoDto.ListaEntidades[i].Id + "?Descricao="
                            + retornoDto.ListaEntidades[i].Nome + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarPlanos");
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
function LerFiltros() {
    var listaFiltros = [];

    if ($("#txtNome").val() !== null && $("#txtNome").val().length > 0 && $("#txtNome").val() !== "") {
        listaFiltros.push({
            key: "NOME",
            value: $("#txtNome").val()
        });
    }

    if ($("#txtIdPlano").val() !== null && $("#txtIdPlano").val().length > 0 && $("#txtIdPlano").val() !== "") {
        listaFiltros.push({
            key: "ID",
            value: $("#txtIdPlano").val()
        });
    }

    return listaFiltros;
}