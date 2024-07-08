document.addEventListener('DOMContentLoaded', function () {
    const documentoInput = document.getElementById('Documento');
    const tipoPF = document.getElementById('rdbPF');
    const tipoPJ = document.getElementById('rdbPJ');
    const telefoneInput = document.getElementById('Telefone');
    const documentoError = document.getElementById('documento-error');
    let timeoutId;

    if (documentoInput && tipoPF && tipoPJ && documentoError && telefoneInput) {
        // Função para formatar CPF ou CNPJ
        function formatarDocumento(value, tipo) {
            const maxLength = tipo === 'PF' ? 14 : 18;
            let formattedValue = value.replace(/\D/g, '').slice(0, maxLength);

            if (tipo === 'PF') {
                formattedValue = formattedValue.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
            } else if (tipo === 'PJ') {
                formattedValue = formattedValue.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
            } else if (tipo === 'Telefone') {
                formattedValue = formattedValue.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
            }
            return formattedValue;
        }

        // Função para validar CPF ou CNPJ
        function validarDocumento(value, tipo) {
            if (tipo === 'PF') {
                return validaCpf(value);
            } else if (tipo === 'PJ') {
                return validaCnpj(value);
            }
            return false;
        }

        // Função para validar CPF
        function validaCpf(cpf) {
            cpf = cpf.replace(/\D/g, '');
            if (cpf.length !== 11) return false;

            let sum = 0;
            let mod;
            for (let i = 1; i <= 9; i++) sum += parseInt(cpf.substring(i - 1, i)) * (11 - i);
            mod = (sum * 10) % 11;

            if ((mod === 10) || (mod === 11)) mod = 0;
            if (mod !== parseInt(cpf.substring(9, 10))) return false;

            sum = 0;
            for (let i = 1; i <= 10; i++) sum += parseInt(cpf.substring(i - 1, i)) * (12 - i);
            mod = (sum * 10) % 11;

            if ((mod === 10) || (mod === 11)) mod = 0;
            if (mod !== parseInt(cpf.substring(10, 11))) return false;

            return true;
        }

        // Função para validar CNPJ
        function validaCnpj(cnpj) {
            cnpj = cnpj.replace(/\D/g, '');
            if (cnpj.length !== 14) return false;

            let size = cnpj.length - 2;
            let numbers = cnpj.substring(0, size);
            let digits = cnpj.substring(size);
            let sum = 0;
            let pos = size - 7;

            for (let i = size; i >= 1; i--) {
                sum += numbers.charAt(size - i) * pos--;
                if (pos < 2) pos = 9;
            }

            let result = sum % 11 < 2 ? 0 : 11 - (sum % 11);
            if (result !== parseInt(digits.charAt(0))) return false;

            size = size + 1;
            numbers = cnpj.substring(0, size);
            sum = 0;
            pos = size - 7;

            for (let i = size; i >= 1; i--) {
                sum += numbers.charAt(size - i) * pos--;
                if (pos < 2) pos = 9;
            }

            result = sum % 11 < 2 ? 0 : 11 - (sum % 11);
            if (result !== parseInt(digits.charAt(1))) return false;

            return true;
        }

      

        // Evento de input para o campo documento
        documentoInput.addEventListener('input', function (e) {
            clearTimeout(timeoutId);
            timeoutId = setTimeout(function () {
                const tipo = tipoPF.checked ? 'PF' : tipoPJ.checked ? 'PJ' : null;
                if (!tipo) return;

                const valorAtual = e.target.value;
                const valorFormatado = formatarDocumento(valorAtual, tipo);

                // Corrige o valor formatado para respeitar o maxLength após a formatação
                if (valorFormatado.length > e.target.maxLength) {
                    e.target.value = valorFormatado.slice(0, e.target.maxLength);
                } else {
                    e.target.value = valorFormatado;
                }

                if (!validarDocumento(valorFormatado, tipo)) {
                    if (tipo === 'PF') {
                        documentoError.textContent = 'CPF inválido.';
                    } else if (tipo === 'PJ') {
                        documentoError.textContent = 'CNPJ inválido.';
                    }
                    documentoError.style.display = 'block';
                } else {
                    documentoError.style.display = 'none';
                }
            }, 500); // Aguarda meio segundo após o último input
        });

        // Evento de input para o campo telefone
        telefoneInput.addEventListener('input', function (e) {
            const valorAtual = e.target.value;
            const telefoneFormatado = valorAtual.replace(/\D/g, '').slice(0, 13); // Remove não-dígitos e limita a 13 caracteres

            // Exibe o valor formatado no campo de entrada
            e.target.value = formatarDocumento(telefoneFormatado, 'Telefone');
           
        });

        // Eventos de clique nos radio buttons
        tipoPF.addEventListener('click', function () {
            documentoInput.value = ''; // Limpa o campo Documento
            documentoInput.maxLength = 14; // Limita a 14 caracteres para Pessoa Física (CPF)
            $(documentoInput).unmask();
            $(documentoInput).mask('000.000.000-00', { reverse: true });
            documentoError.style.display = 'none';
        });

        tipoPJ.addEventListener('click', function () {
            documentoInput.value = ''; // Limpa o campo Documento
            documentoInput.maxLength = 18; // Limita a 18 caracteres para Pessoa Jurídica (CNPJ)
            $(documentoInput).unmask();
            $(documentoInput).mask('00.000.000/0000-00', { reverse: true });
            documentoError.style.display = 'none';
        });

        // Verifica o tipo inicialmente selecionado ao carregar a página
        if (tipoPF.checked) {
            documentoInput.maxLength = 14;
            $(documentoInput).mask('000.000.000-00', { reverse: true });
        } else if (tipoPJ.checked) {
            documentoInput.maxLength = 18;
            $(documentoInput).mask('00.000.000/0000-00', { reverse: true });
        }
    } else {
        console.error('Elementos não encontrados.');
    }
});
