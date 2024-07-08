    $(document).ready(function () {
        $('#btnfiltrar').click(function (e) {
            e.preventDefault(); // Impede o comportamento padrão do link

            var somenteAtivos = $('#somenteAtivos').is(':checked');
            var nome = $('#txtnome').val();
            var documento = $('#txtdocumento').val();

            // Constrói a URL com os parâmetros usando template literal (ES6)
            var url = '/Clientes/FiltrarClientes?somenteAtivos=' + somenteAtivos
                + '&nome=' + encodeURIComponent(nome)
                + '&documento=' + encodeURIComponent(documento);

            // Realiza a requisição Ajax
            $.ajax({
                url: url,
                type: 'GET',
                success: function (data) {
                    $('#clientesTable').html(data);
                },
                error: function () {
                    alert('Erro ao buscar clientes.');
                }
            });
        });
    });