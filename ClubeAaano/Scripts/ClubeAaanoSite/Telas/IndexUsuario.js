// Busca e preenche a lista de usuários encontrados
function BuscarUsuarios(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarUsuarios");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Usuario/ObterListaFiltradaPaginada",
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
                    text: "Ocorreu um problema ao fazer a pesquisa de usuários. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de usuários. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados usuários com os filtros preenchidos", "Pesquisa de usuários");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Nome + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].Email + "</td>"
                            + "<td>" + ((retornoDto.ListaEntidades[i].Administrador) ? "Sim" : "Não") + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../Usuario/Visualizar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-primary' href='../Usuario/Editar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + " <a class='btn btn-sm btn-info' href='../UsuarioPermissao/EditarPorUsuario/"
                            + retornoDto.ListaEntidades[i].Id + "?nomeUsuario=" + retornoDto.ListaEntidades[i].Nome
                            + "'><i class='fa fa-unlock-alt'></i></a> <a class='btn btn-sm btn-danger' href='../Usuario/Excluir/"
                            + retornoDto.ListaEntidades[i].Id + "?Descricao="
                            + retornoDto.ListaEntidades[i].Nome + " (" + retornoDto.ListaEntidades[i].Email
                            + ")'><i class='fa fa-trash'></i></a></td></tr>");
                    }
                }
                
                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarUsuarios");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de usuários. \n"
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

    if ($("#txtEmail").val() !== null && $("#txtEmail").val().length > 0 && $("#txtEmail").val() !== "") {
        listaFiltros.push({
            key: "EMAIL",
            value: $("#txtEmail").val()
        });
    }

    if ($("#optAdministrador").val() !== null && $("#optAdministrador").val().length > 0 && $("#optAdministrador").val() !== "") {
        listaFiltros.push({
            key: "ADMINISTRADOR",
            value: $("#optAdministrador").val()
        });
    }

    return listaFiltros;
}