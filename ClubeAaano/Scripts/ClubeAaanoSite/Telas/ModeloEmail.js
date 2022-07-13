var tagNome = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: 'Nome assinante',
        tooltip: 'Substituído pelo nome do assinante',
        click: function () {
            context.invoke('editor.insertText', '||NOMEASSINANTE||');
        }
    });

    return button.render();
}

var tagCarteirinha = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: 'Link para carteirinha',
        tooltip: 'Substituído pelo link de informações',
        click: function () {
            context.invoke('createLink', {
                text: "clicando aqui",
                url: '||URLCARTEIRINHA||',
                isNewWindow: true
            });
        }
    });

    return button.render();
}

var tagUltimoPagamento = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: 'Último pagamento',
        tooltip: 'Substituído pela data do último pagamento',
        click: function () {
            context.invoke('editor.insertText', '||ULTIMOPAGAMENTO||');
        }
    });

    return button.render();
}

var tagEmailAaano = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: 'Email da AAANO',
        tooltip: 'Substituído pelo endereço de email da AAANO',
        click: function () {
            context.invoke('editor.insertText', '||EMAILAAANO||');
        }
    });

    return button.render();
}

var tagFacebookAaano = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: 'Facebook da AAANO',
        tooltip: 'Link para facebook da AAANO',
        click: function () {
            context.invoke('createLink', {
                text: "Facebook (@aaanovaodessa)",
                url: 'https://www.facebook.com/aaanovaodessa',
                isNewWindow: true
            });
        }
    });

    return button.render();
}

var tagInstagramAaano = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: 'Instagram da AAANO',
        tooltip: 'Link para Instagram da AAANO',
        click: function () {
            context.invoke('createLink', {
                text: "Instagram (@aaanovaodessa)",
                url: 'https://www.instagram.com/aaanovaodessa/',
                isNewWindow: true
            });
        }
    });

    return button.render();
}

var tagSiteAaano = function (context) {
    var ui = $.summernote.ui;

    var button = ui.button({
        contents: 'Site da AAANO',
        tooltip: 'Link para site da AAANO',
        click: function () {
            context.invoke('createLink', {
                text: "Nosso site",
                url: 'http://www.aaano.com.br/',
                isNewWindow: true
            });
        }
    });

    return button.render();
}




