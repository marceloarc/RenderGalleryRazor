using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RenderGalleyRazor.Models
{
    public partial class DatabaseContext : IdentityDbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            this.Database.EnsureCreated();

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Art> Arts { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Favoritos> Favoritos { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ProdutoCarrinho> Produtos { get; set; }
        public DbSet<Publicacao> Publicacoes { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<LikesDeslikes> LikeDeslikes { get; set; }

        public DbSet<Pedido> Pedidos { get; set; }

        public DbSet<ProdutoPedido> ProdutosPedido { get; set; }

        public DbSet<Planos> Planos { get; set; }
        public DbSet<Vantagens> Vantagens { get; set; }
    }

}