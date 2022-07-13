// Sincroniza as assinaturas do pagseguro
function SincronizarAssinaturas() {
    ExibirCarregando("divCarregando");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/AssinaturaPagSeguro/SincronizarAssinaturas",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        traditional: true,
        success: function (retornoDto) {
            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao sincronizar as assinaturas. \n"
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
                    text: "Ocorreu um problema ao sincronizar as assinaturas. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {
                swal({
                    title: "Novos contratos sincronizados!",
                    text: "Os novos contratos foram obtidos do PagSeguro",
                    icon: "success",
                    button: "Ok",
                });
                
                EsconderCarregando("divCarregando");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao sincronizar as assinaturas. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}

// Sincroniza as assinaturas do pagseguro
function ValidarAssinatura(idAssinatura) {
    ExibirCarregando("divCarregando");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/AssinaturaPagSeguro/ValidarAssinatura",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            id: idAssinatura,
        }),
        traditional: true,
        success: function (retornoDto) {
            if (retornoDto.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao validar a assinatura. \n"
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
                    text: "Ocorreu um problema ao validar a assinatura. \n"
                        + "Mensagem: " + retornoDto.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {
                swal({
                    title: "Assinatura validada!",
                    text: "O status da assinatura foi atualizado com sucesso",
                    icon: "success",
                    button: "Ok",
                });

                $("#Status").val(retornoDto.Entidade.Status);

                EsconderCarregando("divCarregando");
            }
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao validar a assinatura. \n"
                    + "Mensagem: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}