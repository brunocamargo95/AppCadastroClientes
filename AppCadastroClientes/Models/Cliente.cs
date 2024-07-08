using System;
using System.ComponentModel.DataAnnotations;

namespace AppCadastroClientes.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Tipo { get; set; }

        [Required]
        public string Documento { get; set; }

        public DateTime DataCadastro { get; set; }

        [Required]
        public string Telefone { get; set; }

        public bool IsDeleted { get; set; }
    }
}