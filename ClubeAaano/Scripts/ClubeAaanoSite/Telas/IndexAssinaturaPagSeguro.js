// Busca e preenche a lista encontrados
function BuscarAssinaturas(nPagina) {

    PaginarPesquisa(0, nPagina, "BuscarAssinaturas");
    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/AssinaturaPagSeguro/ObterListaFiltradaSimplificadaPaginada",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltros(),
            Pagina: nPagina,
            NaoPaginarPesquisa: false,
            CampoOrdem: $("#optCampoOrdem").val(),
            NumeroItensPorPagina: 20
        }),
        traditional: true,
        success: function (retornoDto) {
            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao fazer a pesquisa de assinaturas. \n"
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
                    text: "Ocorreu um problema ao fazer a pesquisa de assinaturas. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                $("#lbTotResultados").html("<b>Erro na busca</b>");
                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.ListaEntidades.length == 0) {
                    $("#lbTotResultados").html("<b>Encontradas: 0</b>");
                    toastr.options.preventDuplicates = true;
                    toastr.info("Não foram encontradas assinaturas com os filtros preenchidos", "Pesquisa de assinaturas");
                } else {
                    for (var i = 0; i < retornoDto.ListaEntidades.length; i++) {

                        $("#tblResultados tbody").append("<tr>"
                            + "<td>" + retornoDto.ListaEntidades[i].CodigoSimplificado + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].NomeAssinate + "</td>"
                            + "<td>" + ConverterDataHoraJson(retornoDto.ListaEntidades[i].Criacao) + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].ReferenciaPlano + "</td>"
                            + "<td>" + FormatarTelefone(retornoDto.ListaEntidades[i].Telefone) + "</td>"
                            + "<td>" + retornoDto.ListaEntidades[i].Email + "</td>"
                            + "<td><a class='btn btn-sm btn-default' href='../AssinaturaPagSeguro/Visualizar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-eye'></i></a>"
                            + " <a class='btn btn-sm btn-primary' href='../AssinaturaPagSeguro/Editar/"
                            + retornoDto.ListaEntidades[i].Id + "'><i class='fa fa-pencil'></i></a>"
                            + "</td></tr>");
                    }
                }

                $("#lbTotResultados").html("<b>Encontradas: " + retornoDto.TotalItens + "</b>");
                EsconderCarregando("divCarregando");
                PaginarPesquisa(retornoDto.NumeroPaginas, nPagina, "BuscarAssinaturas");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de assinaturas. \n"
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
            key: "NOMEASSINANTE",
            value: $("#txtNome").val()
        });
    }

    if ($("#txtEmail").val() !== null && $("#txtEmail").val().length > 0 && $("#txtEmail").val() !== "") {
        listaFiltros.push({
            key: "EMAILASSINANTE",
            value: $("#txtEmail").val()
        });
    }

    if ($("#txtTelefone").val() !== null && $("#txtTelefone").val().length > 0 && $("#txtTelefone").val() !== "") {
        listaFiltros.push({
            key: "TELEFONEASSINANTE",
            value: $("#txtTelefone").val()
        });
    }

    if ($("#txtIdPlano").val() !== null && $("#txtIdPlano").val().length > 0 && $("#txtIdPlano").val() !== "") {
        listaFiltros.push({
            key: "IDPLANO",
            value: $("#txtIdPlano").val()
        });
    }

    if ($("#txtCodigo").val() !== null && $("#txtCodigo").val().length > 0 && $("#txtCodigo").val() !== "") {
        listaFiltros.push({
            key: "CODIGOSIMPLIFICADO",
            value: $("#txtCodigo").val()
        });
    }

    if ($("#txtCriacaoInicial").val() !== null && $("#txtCriacaoInicial").val().length > 0 && $("#txtCriacaoInicial").val() !== "") {
        listaFiltros.push({
            key: "DATACRIACAOINICIO",
            value: $("#txtCriacaoInicial").val()
        });
    }

    if ($("#txtCriacaoFinal").val() !== null && $("#txtCriacaoFinal").val().length > 0 && $("#txtCriacaoFinal").val() !== "") {
        listaFiltros.push({
            key: "DATACRIACAOFIM",
            value: $("#txtCriacaoFinal").val()
        });
    }

    if ($("#optStatus").val() !== null && $("#optStatus").val().length > 0 && $("#optStatus").val() !== "") {
        listaFiltros.push({
            key: "STATUS",
            value: $("#optStatus").val()
        });
    }

    if ($("#chClubeDesconto").is(':checked')) {
        listaFiltros.push({
            key: "CLUBEDESCONTO",
            value: "true"
        });
    }

    return listaFiltros;
}

// Ao selecionar um plano para filtrar
function SelecionarPlano(id, nome, codigo) {
    $("#txtIdPlano").val(id);
    $("#txtNomePlano").val(nome);
    $("#btnBuscarPlano").html("<i class='fa fa-eraser'></i> Limpar");

    $("#modalSelecionarPlano").modal("hide");
}

// Exporta os cadastros para Excel
function ExportarAssinaturas() {
    ExibirCarregando("divCarregando");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/AssinaturaPagSeguro/GerarExcelAssinaturas",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltros(),
            CampoOrdem: $("#optCampoOrdem").val()
        }),
        traditional: true,
        success: function (retornoDto) {

            swal({
                title: "Sucesso!",
                text: "Pesquisa exportada com sucesso! ",
                icon: "success",
                button: "Ok",
            });

            window.open('/AssinaturaPagSeguro/DownloadArquivo', '_blank');
            EsconderCarregando("divCarregando");
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer a pesquisa de assinaturas. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Envia um modelo de email para todas as assinaturas pesquisadas
function EnviarEmailIndexAssinaturas(id, assunto) {
    ExibirCarregando("divCarregandoModeloEmail");
    $(".enviarEmail").attr("disabled", "disabled");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/AssinaturaPagSeguro/EnviarEmailMassa",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            ListaFiltros: LerFiltros(),
            IdModeloEmail: id
        }),
        traditional: true,
        success: function (retornoDto) {
            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao enviar os emails. \n"
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
                    text: "Ocorreu um problema ao enviar os emails. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoModeloEmail");
            } else {
                swal({
                    title: "Emails processados",
                    text: retornoDto.Mensagem,
                    icon: "success",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoModeloEmail");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao enviar os emails. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregandoModeloEmail");
        }
    });
}