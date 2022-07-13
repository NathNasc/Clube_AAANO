// Busca e preenche a lista de modelos encontrados
function BuscarModelos(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarModelos");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ModeloEmail/ObterListaFiltradaPaginada",
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
                    text: "Ocorreu um problema ao fazer a pesquisa de modelos. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de modelos. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas modelos com os filtros preenchidos", "Pesquisa de modelos");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Assunto + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../ModeloEmail/Visualizar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-primary' href='../ModeloEmail/Editar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../ModeloEmail/Excluir/"
                            + retornoDto.ListaEntidades[i].Id + "?Descricao="
                            + retornoDto.ListaEntidades[i].Assunto
                            + "'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarModelos");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de modelos. \n"
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

    if ($("#txtAssunto").val() !== null && $("#txtAssunto").val().length > 0 && $("#txtAssunto").val() !== "") {
        listaFiltros.push({
            key: "ASSUNTO",
            value: $("#txtAssunto").val()
        });
    }

    listaFiltros.push({
        key: "IGNORARFIXOS",
        value: ""
    });

    return listaFiltros;
}