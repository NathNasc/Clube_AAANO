// Ao selecionar uma promoção para filtrar
function SelecionarLojaParceira(id, nome) {
    var linha = document.getElementById("tblLojas").rows.length - 1;
    $("#tblLojas tbody").append("<tr id='" + id + "'>"
        + "<td><input type='hidden' id='ListaPermissoes_" + linha.toString() +
        "__NomeLoja' name='ListaPermissoes[" + linha.toString()
        + "].NomeLoja' value='" + nome
        + "' />" + nome + "</td>"

        + "<td><input type='hidden' id='ListaPermissoes_" + linha.toString() +
        "__IdLojaParceira' name='ListaPermissoes[" + linha.toString()
        + "].IdLojaParceira' value='" + id + "' />"

        + "<input type='hidden' id='ListaPermissoes_" + linha.toString() +
        "__Excluir' name='ListaPermissoes[" + linha.toString()
        + "].Excluir' value='false' />"

        + "<button class='btn btn-sm btn-danger' type='button' "
        + "onclick='RemoverLoja(" + linha + ", \"" + id
        + "\")'><i class='fa fa-trash'></i></button></td></tr>");

    MostrarMensagemRetorno("INCLUIDO", "A loja");
}

// Esconde a linha e marca como excluido
function RemoverLoja(indice, id) {
    $("#ListaPermissoes_" + indice.toString() + "__Excluir").val(true);
    $("#" + id).attr("hidden", "hidden");
}


