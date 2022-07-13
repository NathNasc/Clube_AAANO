// Ao selecionar uma promoção para filtrar
function SelecionarLojaParceira(id, nome) {
    $("#NomeLojaParceira").val(nome);
    $("#IdLojaParceira").val(id);

    $("#modalSelecionarLoja").modal("hide");
}


