// Busca e preenche a lista de lojas encontrados
function BuscarEmailsEnviados(nPagina) {
    PaginarPesquisa(0, nPagina, "BuscarEmailsEnviados");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/EmailEnviado/ObterListaFiltradaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltros(),
            Pagina: nPagina,
            NaoPaginarPesquisa: false,
            CampoOrdem: $("#optCampoOrdemEmail").val()
        }),
        traditional: true,
        success: function (retornoDto) {

            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de emails enviados. \n"
                        + "\n Mensagem de retorno: " + retornoDto.Error,
                    icon: "warning",
                    button: "Ok"
                });

                $("#lbTotResultados").html("<b>Erro na busca</b>");
                EsconderCarregando("divCarregando");
                return;
            }

            if (!retornoDto.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de emails enviados. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                $("#lbTotResultados").html("<b>Erro na busca</b>");
                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    $("#lbTotResultados").html("<b>Encontrados: 0</b>");
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontrados emails com os filtros preenchidos", "Pesquisa de emails");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        var sucesso = "<i class='fa fa-";
                        if (retornoDto.ListaEntidades[i].SucessoEnvio) {
                            sucesso += "check-circle'><i>";
                        } else {
                            sucesso +="warning'><i>"
                        }

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + ConverterDataHoraJson(retornoDto.ListaEntidades[i].DataInclusao) + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].Assunto + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].NomeAssinante + "</td>"
                            + "<td>" + sucesso + "</td></tr>");
                    }
                }

                $("#lbTotResultados").html("<b>Encontrados: " + retornoDto.TotalItens + "</b>");
                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarEmailsEnviados");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de emails enviados. \n"
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

    if ($("#txtEnvioInicial").val() !== null && $("#txtEnvioInicial").val().length > 0 && $("#txtEnvioInicial").val() !== "") {
        listaFiltros.push({
            key: "DATAENVIOINICIAL",
            value: $("#txtEnvioInicial").val()
        });
    }

    if ($("#txtEnvioFinal").val() !== null && $("#txtEnvioFinal").val().length > 0 && $("#txtEnvioFinal").val() !== "") {
        listaFiltros.push({
            key: "DATAENVIOFINAL",
            value: $("#txtEnvioFinal").val()
        });
    }

    if ($("#txtIdAssinatura").val() !== null && $("#txtIdAssinatura").val().length > 0 && $("#txtIdAssinatura").val() !== "") {
        listaFiltros.push({
            key: "IDASSINATURAPAGSEGURO",
            value: $("#txtIdAssinatura").val()
        });
    }

    if ($("#txtIdModelo").val() !== null && $("#txtIdModelo").val().length > 0 && $("#txtIdModelo").val() !== "") {
        listaFiltros.push({
            key: "IDMODELOEMAIL",
            value: $("#txtIdModelo").val()
        });
    }

    if ($("#optSucesso").val() !== null && $("#optSucesso").val().length > 0 && $("#optSucesso").val() !== "") {
        listaFiltros.push({
            key: "SUCESSO",
            value: $("#optSucesso").val()
        });
    }

    return listaFiltros;
}

// Ao selecionar um modelo de email para filtrar
function SelecionarModelo(id, assunto) {
    $("#txtIdModelo").val(id);
    $("#txtModelo").val(assunto);
    $("#btnBuscarModelo").html("<i class='fa fa-eraser'></i> Limpar");
    $("#modalSelecionarModeloEmail").modal("hide");
}

// Ao selecionar uma promoção para filtrar
function SelecionarAssinatura(codigo, nome, id) {
    $("#txtIdAssinatura").val(id);
    $("#txtNomeAssinante").val(nome);
    $("#btnBuscarAssinatura").html("<i class='fa fa-eraser'></i> Limpar");
    $("#modalSelecionarAssinatura").modal("hide");
}
