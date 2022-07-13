// Busca e preenche a lista de promoções encontrados
function BuscarPromocoesPorCodigoAssinatura() {

    ExibirCarregando("divCarregando");
    $("#tblResultados tbody").empty();
    $("#tblHistorico tbody").empty();
    $("#txtNomeAssinante").val("");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ResgatePromocao/ObterPromocoesPorCodigoAssinatura",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            Codigo: $("#txtCodigoSimplificado").val(),
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

            if (!retornoDto.Retorno && retornoDto.Mensagem !== "Assinatura não encontrada.") {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao fazer a pesquisa de promoções. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {

                if (retornoDto.AssinaturaPagSeguroDto == null) {
                    swal({
                        title: "Assinatura não encontrada!",
                        text: "Confira o código da assinatura ou utilize a pesquisa avançada.",
                        icon: "warning",
                        button: "Ok",
                    });

                } else {

                    $("#txtNomeAssinante").val(retornoDto.AssinaturaPagSeguroDto.NomeAssinate);

                    // Preencher disponíveis
                    if (retornoDto.ListaPromocoesDisponiveis.length <= 0) {
                        $("#tblResultados tbody").append("<tr><td>Não há promoções disponíveis</td></tr>");
                    } else {
                        for (var i = 0; i < retornoDto.ListaPromocoesDisponiveis.length; i++) {

                            $("#tblResultados tbody").append("<tr>"
                                + "<td>" + retornoDto.ListaPromocoesDisponiveis[i].ResumoPromocao + "</td>"
                                + "<td>" + ConverterDataJson(retornoDto.ListaPromocoesDisponiveis[i].Validade) + "</td>"
                                + "<td>" + retornoDto.ListaPromocoesDisponiveis[i].NomeLojaParceira + "</td>"
                                + "<td><button class='btn btn-sm btn-primary' id='btnResgate" + retornoDto.ListaPromocoesDisponiveis[i].IdPromocao
                                + "' title='Resgatar' onclick='ResgatarPromocao(\""
                                + retornoDto.ListaPromocoesDisponiveis[i].IdPromocao + "\", \""
                                + retornoDto.ListaPromocoesDisponiveis[i].IdAssinaturaPagSeguro + "\", \""
                                + retornoDto.ListaPromocoesDisponiveis[i].CodigoSimplificadoAssinatura + "\", \""
                                + ConverterDataJson(retornoDto.ListaPromocoesDisponiveis[i].Validade) + "\")'><i class='fa fa-cart-plus'></i></button>"
                                + "</td></tr>");
                        }
                    }

                    // Preencher ultimos resgates
                    if (retornoDto.ListaUltimosResgates.length <= 0) {
                        $("#tblHistorico tbody").append("<tr><td>Não foram registrados resgates</td></tr>");
                    } else {
                        for (var j = 0; j < retornoDto.ListaUltimosResgates.length; j++) {

                            $("#tblHistorico tbody").append("<tr>"
                                + "<td>" + retornoDto.ListaUltimosResgates[j].ResumoPromocao + "</td>"
                                + "<td>" + ConverterDataHoraJson(retornoDto.ListaUltimosResgates[j].Resgate) + "</td>"
                                + "<td>" + retornoDto.ListaUltimosResgates[j].NomeLojaParceira + "</td>"
                                + "<td>" + retornoDto.ListaUltimosResgates[j].NomeUsuarioResgate + "</td>"
                                + "</td></tr>");
                        }
                    }
                }

                EsconderCarregando("divCarregando");
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

// Resgata uma promoção
function ResgatarPromocao(idPromocao, idAssinatura, codigoAssinatura, validade) {
    ExibirCarregando("divCarregandoResgate");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/ResgatePromocao/ResgatarPromocao",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            IdPromocao: idPromocao,
            IdAssinaturaPagSeguro: idAssinatura,
            CodigoSimplificadoAssinatura: codigoAssinatura,
            Validade: validade
        }),
        traditional: true,
        success: function (retornoDto) {

            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao resgatar a promoção. \n"
                        + "\n Mensagem de retorno: " + retornoDto.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregandoResgate");
                return;
            }

            if (!retornoDto.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao resgatar a promoção. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregandoResgate");
            } else {

                //Swal de confirmação
                swal({
                    title: "Resgate realizado com sucesso!",
                    text: "Deseja limpar a tela e resgate?",
                    icon: "success",
                    buttons: ["Sim", "Não"],
                    dangerMode: true,
                }).then(function (willDelete) {
                    if (willDelete) {
                        BuscarPromocoesPorCodigoAssinatura();
                    } else {
                        $("#tblResultados tbody").empty();
                        $("#tblHistorico tbody").empty();
                        $("#txtNomeAssinante").val("");
                        $("#txtCodigoSimplificado").val("");
                        $("#txtCodigoSimplificado").focus();
                    }
                });
                EsconderCarregando("divCarregandoResgate");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao fazer o resgate. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregandoResgate");
        }
    });
}

// Seleção simples que preenche código e nome do assinante
function SelecionarAssinatura(codSimplificado, nome, id) {
    $("#txtCodigoSimplificado").val(codSimplificado);
    $("#txtNomeAssinante").val(nome);
    BuscarPromocoesPorCodigoAssinatura();

    $("#modalSelecionarAssinatura").modal("hide");
}

// Fecha a câmera e a modal
function FecharLeituraQrCode() {
    $("#btnPararScanner").click();
    $("#modalLerQrCode").modal("hide");
}