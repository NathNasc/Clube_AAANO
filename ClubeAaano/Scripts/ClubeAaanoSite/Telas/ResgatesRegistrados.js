// Busca e preenche a lista de lojas encontrados
function BuscarResgatesRegistrados(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarResgatesRegistrados");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ResgatePromocao/ObterListaFiltradaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltros(),
            Pagina: nPagina,
            NaoPaginarPesquisa: false,
            CampoOrdem: $("#optCampoOrdem").val()
        }),
        traditional: true,
        success: function (retornoDto) {

            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de resgates. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de resgates. \n"
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
                    toastr.info("Não foram encontrados resgates com os filtros preenchidos", "Pesquisa de resgates");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        /* Resg. | Resumo | Loja | Validade | Assinante | Opções
                         */
                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + ConverterDataJson(retornoDto.ListaEntidades[i].Validade) + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].ResumoPromocao + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].NomeLojaParceira + "</td>"
                            + "<td>" + ConverterDataJson(retornoDto.ListaEntidades[i].Resgate) + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].NomeAssinante + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../ResgatePromocao/Visualizar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + "</td></tr>");
                    }
                }

                $("#lbTotResultados").html("<b>Encontrados: " + retornoDto.TotalItens + "</b>");
                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarResgatesRegistrados");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de resgates. \n"
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

    if ($("#txtValidadeInicial").val() !== null && $("#txtValidadeInicial").val().length > 0 && $("#txtValidadeInicial").val() !== "") {
        listaFiltros.push({
            key: "INICIOVALIDADE",
            value: $("#txtValidadeInicial").val()
        });
    }

    if ($("#txtValidadeFinal").val() !== null && $("#txtValidadeFinal").val().length > 0 && $("#txtValidadeFinal").val() !== "") {
        listaFiltros.push({
            key: "FIMVALIDADE",
            value: $("#txtValidadeFinal").val()
        });
    }

    if ($("#txtResgateInicial").val() !== null && $("#txtResgateInicial").val().length > 0 && $("#txtResgateInicial").val() !== "") {
        listaFiltros.push({
            key: "INICIORESGATE",
            value: $("#txtResgateInicial").val()
        });
    }

    if ($("#txtResgateFinal").val() !== null && $("#txtResgateFinal").val().length > 0 && $("#txtResgateFinal").val() !== "") {
        listaFiltros.push({
            key: "FIMRESGATE",
            value: $("#txtResgateFinal").val()
        });
    }

    if ($("#txtIdAssinatura").val() !== null && $("#txtIdAssinatura").val().length > 0 && $("#txtIdAssinatura").val() !== "") {
        listaFiltros.push({
            key: "IDASSINATURAPAGUESEGURO",
            value: $("#txtIdAssinatura").val()
        });
    }

    if ($("#txtIdPromocao").val() !== null && $("#txtIdPromocao").val().length > 0 && $("#txtIdPromocao").val() !== "") {
        listaFiltros.push({
            key: "IDPROMOCAO",
            value: $("#txtIdPromocao").val()
        });
    }

    if ($("#txtIdLojaParceira").val() !== null && $("#txtIdLojaParceira").val().length > 0 && $("#txtIdLojaParceira").val() !== "") {
        listaFiltros.push({
            key: "IDLOJA",
            value: $("#txtIdLojaParceira").val()
        });
    }

    return listaFiltros;
}

// Ao selecionar uma promoção para filtrar
function SelecionarPromocao(id, resumo, loja) {
    $("#txtIdPromocao").val(id);
    $("#txtResumoPromocao").val(resumo + " - " + loja);
    $("#btnBuscarPromocao").html("<i class='fa fa-eraser'></i> Limpar");
    $("#modalSelecionarPromocao").modal("hide");
}

// Ao selecionar uma promoção para filtrar
function SelecionarLojaParceira(id, nome) {
    $("#txtIdLojaParceira").val(id);
    $("#txtNomeLojaParceira").val(nome);
    $("#btnBuscarLojaParceira").html("<i class='fa fa-eraser'></i> Limpar");
    $("#modalSelecionarLoja").modal("hide");
}

// Ao selecionar uma promoção para filtrar
function SelecionarAssinatura(codigo, nome, id) {
    $("#txtIdAssinatura").val(id);
    $("#txtNomeAssinante").val(nome);
    $("#btnBuscarAssinatura").html("<i class='fa fa-eraser'></i> Limpar");
    $("#modalSelecionarAssinatura").modal("hide");
}
