// Ao selecionar uma promoção para filtrar
function SelecionarPlano(id, nome, codigo) {
    var linha = document.getElementById("tblPlanos").rows.length - 1;
    $("#tblPlanos tbody").append("<tr id='" + id + "'>"
        + "<td><input type='hidden' id='ListaPlanos_" + linha.toString() +
        "__Nome' name='ListaPlanos[" + linha.toString()
        + "].Nome' value='" + nome
        + "' />" + nome + "</td>"

        + "<td><input type='hidden' id='ListaPlanos_" + linha.toString() +
        "__Id' name='ListaPlanos[" + linha.toString()
        + "].Id' value='" + id + "' />"

        + "<input type='hidden' id='ListaPlanos_" + linha.toString() +
        "__Excluir' name='ListaPlanos[" + linha.toString()
        + "].Excluir' value='false' />"

        + "<input type='hidden' id='ListaPlanos_" + linha.toString() +
        "__CodigoSimplificado' name='ListaPlanos[" + linha.toString()
        + "].CodigoSimplificado' value='" + codigo + "' />"

        + "<button class='btn btn-sm btn-danger' type='button' "
        + "onclick='RemoverPlano(" + linha + ", \"" + id
        + "\")'><i class='fa fa-trash'></i></button></td></tr>");

    MostrarMensagemRetorno("INCLUIDO", "O plano");
}

// Esconde a linha e marca como excluido
function RemoverPlano(indice, id) {
    $("#ListaPlanos_" + indice.toString() + "__Excluir").val(true);
    $("#" + id).attr("hidden", "hidden");
}


