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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para a entidade Chat
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User1)
                .WithMany()
                .HasForeignKey(c => c.user_one)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.user_two)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração para a entidade User e Chat
            modelBuilder.Entity<User>()
                .HasMany(u => u.Chats)
                .WithOne()
                .HasForeignKey(c => c.user_one)
                .OnDelete(DeleteBehavior.Cascade); // Delete em cascata do lado do usuário

            // Configuração para outras entidades relacionadas ao usuário...
            modelBuilder.Entity<User>()
                .HasMany(u => u.Favoritos)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.LikesDeslikes)
                .WithOne(ld => ld.User)
                .HasForeignKey(ld => ld.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Pedidos)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.User_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ProdutosCarrinho)
                .WithOne(pc => pc.User)
                .HasForeignKey(pc => pc.User_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ProdutosPedido)
                .WithOne(pp => pp.User)
                .HasForeignKey(pp => pp.User_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Publicacoes)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.User_id)
                .OnDelete(DeleteBehavior.Cascade);

            // Repita esse padrão para outras entidades conforme necessário

            // Restrições para as outras entidades que não são do tipo User
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User1)
                .WithMany(u => u.ChatsAsUser1)
                .HasForeignKey(c => c.user_one)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User2)
                .WithMany(u => u.ChatsAsUser2)
                .HasForeignKey(c => c.user_two)
                .OnDelete(DeleteBehavior.Cascade);



            // Adicione configurações semelhantes para outras entidades conforme necessário
        }
    }

}