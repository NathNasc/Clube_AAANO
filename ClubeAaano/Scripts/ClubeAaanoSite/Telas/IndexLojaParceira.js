// Busca e preenche a lista de lojas encontrados
function BuscarLojas(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarLojas");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/LojaParceira/ObterListaFiltradaPaginada",
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

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Nome + "</td>"
                            + "<td>" + FormatarTelefone(retornoDto.ListaEntidades[i].Telefone) + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].Endereco + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../LojaParceira/Visualizar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-primary' href='../LojaParceira/Editar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-danger' href='../LojaParceira/Excluir/"
                            + retornoDto.ListaEntidades[i].Id + "?Descricao="
                            + retornoDto.ListaEntidades[i].Nome + " (" + retornoDto.ListaEntidades[i].Telefone
                            + ")'><i class='fa fa-trash'></i></a>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarLojas");
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
function LerFiltros() {
    var listaFiltros = [];

    if ($("#txtNome").val() !== null && $("#txtNome").val().length > 0 && $("#txtNome").val() !== "") {
        listaFiltros.push({
            key: "NOME",
            value: $("#txtNome").val()
        });
    }

    if ($("#txtTelefone").val() !== null && $("#txtTelefone").val().length > 0 && $("#txtTelefone").val() !== "") {
        listaFiltros.push({
            key: "TELEFONE",
            value: $("#txtTelefone").val()
        });
    }

    if ($("#txtEndereco").val() !== null && $("#txtEndereco").val().length > 0 && $("#txtEndereco").val() !== "") {
        listaFiltros.push({
            key: "ENDERECO",
            value: $("#txtEndereco").val()
        });
    }

    return listaFiltros;
}