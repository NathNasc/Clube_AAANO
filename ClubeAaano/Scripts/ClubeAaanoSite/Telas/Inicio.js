function ObterInformacoesDashboard() {
    ExibirCarregando("divCarregando");

    $.ajax({
        type: "POST",
        url: RetornarEndereco() + "/Usuario/ObterInformacoesDashboard",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: "",
        traditional: true,
        success: function (dados) {

            if (dados.Error != undefined) {
                swal({
                    title: "Ops...",
                    html: true,
                    text: "Ocorreu um problema ao obter as informações. \n"
                        + "\n Mensagem de retorno: " + dados.Error,
                    icon: "warning",
                    button: "Ok"
                });

                EsconderCarregando("divCarregando");
                return;
            }

            if (!dados.Retorno) {
                swal({
                    title: "Ops...",
                    text: "Ocorreu um problema ao obter as informações. \n"
                        + "Mensagem de retorno: " + dados.Mensagem,
                    icon: "warning",
                    button: "Ok",
                });

                EsconderCarregando("divCarregando");
            } else {
                $("#labNovasAssinaturas").html(dados.QuantidadeNovasAssinaturas);
                $("#labCuponsResgatados").html(dados.QuantidadeResgates);
                $("#labPagamentosPendentes").html(dados.QuantidadeAssinaturasPagamentoPendente);
                $("#labAssinaturasCanceladas").html(dados.QuantidadeAssinaturasCanceladas);
                $("#labAssinaturasAtivas").html(dados.QuantidadeAssinaturasAtivas);
                $("#labLojasParceiras").html(dados.QuantidadeLojas);
                $("#labBrindesNaoEnviados").html(dados.QuantidadeBrindesNaoEnviados);
                $("#labTotalCancelamento").html(dados.QuantidadeTotalCancelamento);

                $('.connectedSortable').sortable({
                    placeholder: 'sort-highlight',
                    connectWith: '.connectedSortable',
                    handle: '.box-header, .nav-tabs',
                    forcePlaceholderSize: true,
                    zIndex: 999999
                });

                $('.connectedSortable .box-header, .connectedSortable .nav-tabs-custom').css('cursor', 'move');

                $('#chat-box').slimScroll({
                    height: '250px'
                });

                var area = new Morris.Area({
                    element: 'assinaturasLinha',
                    resize: true,
                    data: dados.ListaAssinaturasPorMes,
                    xkey: 'Mes',
                    ykeys: ['Quantidade'],
                    labels: ['Assinaturas'],
                    xlabels: 'Mes',
                    lineColors: ['#a0d0e0'],
                    hideHover: 'auto',
                    parseTime: false
                });

                var line = new Morris.Line({
                    element: 'resgatesLinha',
                    resize: true,
                    data: dados.ListaResgatesPorMes,
                    xkey: 'Mes',
                    ykeys: ['Quantidade'],
                    labels: ['Resgates'],
                    lineColors: ['#efefef'],
                    lineWidth: 2,
                    hideHover: 'auto',
                    gridTextColor: '#fff',
                    gridStrokeWidth: 0.4,
                    pointSize: 4,
                    pointStrokeColors: ['#efefef'],
                    gridLineColor: '#efefef',
                    gridTextFamily: 'Open Sans',
                    gridTextSize: 15,
                    parseTime: false
                });

                // Donut Chart
                var donut = new Morris.Donut({
                    element: 'assinaturasPizza',
                    resize: true,
                    colors: ['#00008B', '#008000', '#FFA500', '#D8BFD8', '#FF0000'],
                    data: [
                        { label: 'Patinha (R$15,00)', value: dados.PercentualAssinaturas15 },
                        { label: 'Filhote (R$30,00)', value: dados.PercentualAssinaturas30 },
                        { label: 'Adote (R$50,00)', value: dados.PercentualAssinaturas50 },
                        { label: 'Castre (R$100,00)', value: dados.PercentualAssinaturas100 },
                        { label: 'Resgate (R$200,00)', value: dados.PercentualAssinaturas200 }
                    ],
                    formatter: function (x) { return x + "%" },
                    hideHover: 'auto'
                });

                // Fix for charts under tabs
                $('.box ul.nav a').on('shown.bs.tab', function () {
                    area.redraw();
                    donut.redraw();
                    line.redraw();
                });
            }

            EsconderCarregando("divCarregando");
        },
        error: function (request, status, error) {
            swal({
                title: "Ops...",
                text: "Ocorreu um problema ao obter as informações. \n"
                    + "Se o problema continuar, entre em contato com o suporte. \n"
                    + "Mensagem de retorno: \n" + error,
                icon: "warning",
                button: "Ok",
            });

            EsconderCarregando("divCarregando");
        }
    });
}
