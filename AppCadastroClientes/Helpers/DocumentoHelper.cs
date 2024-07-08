using System;
using System.Text.RegularExpressions;

namespace AppCadastroClientes.Helpers
{

    public static class DocumentoHelper
    {
        // Método para formatar CPF ou CNPJ
        public static string FormatDocumento(string value, string tipo)
        {
            string formattedValue = Regex.Replace(value, @"\D", "");

            if (tipo == "PF" && formattedValue.Length <= 11)
            {
                return Convert.ToUInt64(formattedValue).ToString(@"000\.000\.000\-00");
            }
            else if (tipo == "PJ" && formattedValue.Length <= 14)
            {
                return Convert.ToUInt64(formattedValue).ToString(@"00\.000\.000\/0000\-00");
            }
            return value;
        }

        // Método para validar CPF
        public static bool ValidarCpf(string cpf)
        {
            cpf = Regex.Replace(cpf, @"\D", "");
            if (cpf.Length != 11) return false;

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        // Método para validar CNPJ
        public static bool ValidarCnpj(string cnpj)
        {
            cnpj = Regex.Replace(cnpj, @"\D", "");
            if (cnpj.Length != 14) return false;

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            int resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }
    }

}