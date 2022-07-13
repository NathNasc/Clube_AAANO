// -------> Retorna o endereço do site
function RetornarEndereco() {
    var enderecoServico = window.location.protocol + "//" + window.location.host;
    return enderecoServico;
}

// -------> Mostra o toast com a mensagem de erro ou sucesso
function MostrarMensagemRetorno(retornoMensagem, inicioMensagem) {
    toastr.options = {
        "showDuration": "400",
        "hideDuration": "600",
        "timeOut": "3000",
        "positionClass": "toast-top-right",
        "preventDuplicates": "true"
    }

    switch (retornoMensagem) {
        case "INCLUIDO":
            toastr.success(inicioMensagem + " foi incluído com sucesso!", "Tudo certo!");
            break;

        case "ALTERADO":
            toastr.success(inicioMensagem + " foi alterado com sucesso!", "Tudo certo!");
            break;

        case "EXCLUIDO":
            toastr.success(inicioMensagem + " foi excluído com sucesso!", "Tudo certo!");
            break;

        case "ESTORNADO":
            toastr.success(inicioMensagem + " foi estornado com sucesso!", "Tudo certo!");
            break;

        default:
            break;
    }
}

// -------> Exibe a div de carregando
function ExibirCarregando(nomeDiv) {
    $("#" + nomeDiv).attr("class", "overlay");
    $("#" + nomeDiv).show();
    $("#" + nomeDiv).focus();
}

// -------> Esconde a div de carregando
function EsconderCarregando(nomeDiv) {
    $("#" + nomeDiv).removeAttr("class");
    $("#" + nomeDiv).hide();

}

// -------> Monta a paginação de uma pesquisa com 5 botões que correm de acordo com a pg selecionada
function PaginarPesquisa(totalPaginas, paginaSelecionado, metodoPesquisa, divPaginas, paginacao) {
    divPaginas = (divPaginas == null || divPaginas == "" || divPaginas === undefined) ? "divPaginas" : divPaginas;
    paginacao = (paginacao == null || paginacao == "" || paginacao === undefined) ? "paginacao" : paginacao;

    //Remover as paginas da pesquisa anterior e adicionar uma nova paginação
    $("#" + divPaginas + " ul").remove();
    $("#" + divPaginas).append('<ul class="pagination pagination-md no-margin pull-right" id="' + paginacao + '"></ul>');

    if (totalPaginas > 0) {
        //Adicionar botão para voltar para a 1ª pg, se não for ela a selecionadas
        if (paginaSelecionado !== 1) {
            $("#" + paginacao).append('<li><a href="#" onclick="' + metodoPesquisa + '(1)">«</a>');
        }

        //Se tiver até 5 paginas (número máximo de botões)
        if (totalPaginas <= 5) {

            //Percorrer as páginas e adiconar os botões para todas
            for (var i = 1; i <= totalPaginas; i++) {
                if (i === paginaSelecionado) {
                    $('#' + paginacao).append('<li><a href="#">' + i + '</a></li>');
                }
                else {
                    $('#' + paginacao).append('<li><a href="#" onclick="' + metodoPesquisa + '(' + i + ')">' + i + '</a></li>');
                }
            }

        } else { //Se tiver mais de 5

            //Até a página 3
            if (paginaSelecionado <= 3) {
                //sequencia normal até a 5ª pagina
                for (var i = 1; i <= 5; i++) {
                    if (i === paginaSelecionado) {
                        $('#' + paginacao).append('<li><a href="#">' + i + '</a></li>');
                    }
                    else {
                        $('#' + paginacao).append('<li><a href="#" onclick="' + metodoPesquisa + '(' + i + ')">' + i + '</a></li>');
                    }
                }

            } else { //Maior que a pg 3

                //Se tiver mais que 2 paginas restantes depois da selecionada
                var restante = totalPaginas - paginaSelecionado;
                if (restante >= 2) {

                    //Mostrar a selecionada no meio, duas anteriores e próximas duas
                    for (var i = -2; i <= 2; i++) {
                        if (paginaSelecionado - i === paginaSelecionado) {
                            $('#' + paginacao).append('<li><a href="#">' + paginaSelecionado + '</a></li>');
                        }
                        else {
                            $('#' + paginacao).append('<li><a href="#" onclick="' + metodoPesquisa + '('
                                + (paginaSelecionado + i) + ')">' + (paginaSelecionado + i) + '</a></li>');
                        }
                    }

                    //Se tiver menos que 2 pg
                } else {
                    //Mostrar as 5 últimas pgs
                    for (var i = totalPaginas - 4; i <= totalPaginas; i++) {
                        if (i === paginaSelecionado) {
                            $('#' + paginacao).append('<li><a href="#">' + i + '</a></li>');
                        }
                        else {
                            $('#' + paginacao).append('<li><a href="#" onclick="' + metodoPesquisa + '('
                                + i + ')">' + i + '</a></li>');
                        }
                    }
                }
            }
        }

        //Adicionar botão para ir para a última pg, se não for ela a selecionadas
        if (paginaSelecionado != totalPaginas && totalPaginas !== 0) {
            $("#" + paginacao).append('<li><a href="#" onclick="' + metodoPesquisa + '(' + totalPaginas + ')">»</a>');
        }
    }
}

// -------> Retorna o texto do CNPJ com pontuação
function FormatarCnpj(cnpj) {
    if (cnpj !== "" && cnpj != null && cnpj.length > 0) {

        cnpj = cnpj.replace(/\D/g, '')
            .replace(/^(\d{2})(\d{3})?(\d{3})?(\d{4})?(\d{2})?/, "$1.$2.$3/$4-$5");

        return cnpj;
    } else {
        return "";
    }
}

// -------> Retorna o texto do CEP com pontuação
function FormatarCep(cep) {
    if (cep !== "" && cep != null && cep.length > 0) {

        cep = cep.replace(/\D/g, '')
            .replace(/^(\d{5})(\d{3})/, "$1-$2");

        return cep;
    } else {
        return "";
    }
}

// -------> Retorna o texto do CNPJ com pontuação
function FormatarCpf(cpf) {
    if (cpf !== "" && cpf != null && cpf.length > 0) {

        cpf = cpf.replace(/\D/g, '')
            .replace(/^(\d{3})(\d{3})?(\d{3})?(\d{2})/, "$1.$2.$3-$4");

        return cpf;
    } else {
        return "";
    }
}

// -------> Retorna o texto do Telefone com pontuação
function FormatarTelefone(telefone) {
    if (telefone !== "" && telefone != null && telefone.length > 0) {

        if (telefone.length == 10) {
            telefoneLimpo = telefone.replace(/\[0-9]*/g, '');
            telefone = telefoneLimpo[0] + telefoneLimpo[1] + ") ";
            telefone += telefoneLimpo[2] + telefoneLimpo[3] + telefoneLimpo[4] + telefoneLimpo[5] + "-";
            telefone += telefoneLimpo[6] + telefoneLimpo[7] + telefoneLimpo[8] + telefoneLimpo[9];
        } else {
            telefoneLimpo = telefone.replace(/\[0-9]*/g, '');
            telefone = telefoneLimpo[0] + telefoneLimpo[1] + ") ";
            telefone += telefoneLimpo[2] + telefoneLimpo[3] + telefoneLimpo[4] + telefoneLimpo[5] + telefoneLimpo[6] + "-";
            telefone += telefoneLimpo[7] + telefoneLimpo[8] + telefoneLimpo[9] + telefoneLimpo[10];
        }

        return "(" + telefone;
    } else {
        return "";
    }
}

// -------> Converte uma data e hora vinda do JSON
function ConverterDataHoraJson(dataJson) {
    if (dataJson != "" && dataJson != null) {

        var dataJson = new Date(parseInt(dataJson.replace("/Date(", "").replace(")/", ""), 10));

        var dataConvertida = Padl(dataJson.getDate().toString(), 2, '0');
        dataConvertida += "/" + Padl((dataJson.getMonth() + 1).toString(), 2, '0') + "/" + dataJson.getFullYear();
        dataConvertida += " " + dataJson.toString().substring(16, 24);

        return dataConvertida;
    } else {

        return "/ /    : : ";
    }
}

// -------> Converte uma data e hora vinda do JSON
function ConverterDataJson(dataJson) {

    if (dataJson != "" && dataJson != null) {

        var dataJson = new Date(parseInt(dataJson.replace("/Date(", "").replace(")/", ""), 10));
        var dataConvertida = Padl(dataJson.getDate().toString(), 2, '0');
        dataConvertida += "/" + Padl((dataJson.getMonth() + 1).toString(), 2, '0') + "/" + dataJson.getFullYear();

        return dataConvertida;
    } else {

        return "  /   / ";
    }
}

// --------> Completa com o caracter passado a esquerda
function Padl(texto, comprimento, caracter) {
    var comprimentoAtual = texto.length;
    var diferenca = comprimento - comprimentoAtual;
    var textoEsquerda = "";
    if (diferenca > 0) {
        for (var i = 0; i < diferenca; i++) {
            textoEsquerda = textoEsquerda + caracter;
        }
    }
    var resultado = textoEsquerda + texto;
    return resultado;
}

// --------> Fixa o DDD do telefone
$(".Telefone").focusin(function () {
    if ($(".Telefone").val().length <= 0) {
        $(".Telefone").val("(19)");
    }
});

// --------> Remove o DDD do telefone quando não foi preenchido
$(".Telefone").focusout(function () {
    var teste = $(".Telefone").val();
    if ($(".Telefone").val() == "(19)") {
        $(".Telefone").val("");
    }
});

// --------> Lé os filtros iniciais e preenche na tela
function LerFiltroIniciais(filtrosIniciais, callBack) {
    if (filtrosIniciais == null) {
        callBack(1);
        return;
    }

    // Preencher os filtros
    Object.keys(filtrosIniciais).forEach(function (key) {
        $('#' + key).val(filtrosIniciais[key]);
    });

    callBack(1);
}

