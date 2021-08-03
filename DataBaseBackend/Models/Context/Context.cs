using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBaseBackend;

namespace DataBaseBackend
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<PrGroup> PrGroup { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<FillField> FillField { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<Gallery> Gallery { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<CommentReplay> CommentReplay { get; set; }
        public DbSet<Takhfif> Takhfif { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<BasketItem> BasketItem { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<MainSlider> MainSlider { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Message>()
            //  .HasOne(p => p.User).WithMany(d => d.Messages).HasForeignKey(a => a.userid).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Message>()
              .HasOne(p => p.User2).WithMany(d => d.Messages).HasForeignKey(a => a.user2id).OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<DataBaseBackend.ChangeInfo> ChangeInfo { get; set; }
    }
}
