var metodoSelecaoModeloEmail = "EnviarEmailIndexAssinaturas";

// Busca e preenche a lista encontrados
function BuscarModelosEmailParaSelecao(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarModeloEmailsSelecao", "divPaginasSelecaoModeloEmail", "paginasModeloEmail");
    ExibirCarregando("divCarregandoModeloEmail");
    $("#tblSelecionarModeloEmail tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ModeloEmail/ObterListaFiltradaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltrosModeloEmailSelecao(),
            Pagina: nPagina,
            NaoPaginarPesquisa: false,
            CampoOrdem: "ASSUNTO",
            NumeroItensPorPagina: 5
        }),
        traditional: true,
        success: function (retornoDto) {
            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a modelos. \n"
                        + "\n Mensagem de retorno: " + retornoDto.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregandoModeloEmail");
                return;
            }

            if (!retornoDto.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a modelos. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoModeloEmail");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados modelos com os filtros preenchidos", "Pesquisa de emails");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        var icone = "<i class='fa fa-send'></i>";
                        if (metodoSelecaoModeloEmail != "EnviarEmailIndexAssinaturas") {
                            icone = "<i class='fa fa-check'></i>";
                            $("#txtLabelEnvio").hide();
                        }

                        $("#tblSelecionarModeloEmail tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].Assunto + "</td>"
                            + "<td><button class='btn btn-sm btn-default enviarEmail' onclick='" + metodoSelecaoModeloEmail + "(\""
                            + retornoDto.ListaEntidades[i].Id + "\", \""
                            + retornoDto.ListaEntidades[i].Assunto + "\", \""
                            + "\")' type='button'>" + icone + "</button>"
                            + "</td></tr>");
                    }
                }

                EsconderCarregando("divCarregandoModeloEmail");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarModeloEmailsParaSelecao", "divPaginasSelecaoModeloEmail", "paginasModeloEmail");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a modelos. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregandoModeloEmail");
        }
    });
}

// Lê os filtros preenchidos na tela
function LerFiltrosModeloEmailSelecao() {
    var listaFiltros = [];

    if ($("#txtAssuntoModeloEmailSelecao").val() !== null && $("#txtAssuntoModeloEmailSelecao").val().length > 0 && $("#txtAssuntoModeloEmailSelecao").val() !== "") {
        listaFiltros.push({
            key: "ASSUNTO",
            value: $("#txtAssuntoModeloEmailSelecao").val()
        });
    }

    return listaFiltros;
}

// Faz a pesquisa inicial e abre a modal
function ExibirModelPesquisaModeloEmail() {
    $("#txtAssuntoModeloEmailSelecao").val("");
    setTimeout(function () { $('input[id="txtAssuntoModeloEmailSelecao"]').focus() }, 500);

    BuscarModelosEmailParaSelecao(1);
    $("#modalSelecionarModeloEmail").modal({ backdrop: 'static', keyboard: false });
}
