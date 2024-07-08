using System.Data.Entity;

namespace AppCadastroClientes.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }

    }
}