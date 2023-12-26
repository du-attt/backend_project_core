using backend_project_core.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace backend_project_core.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        #region DbSet
        //Khởi tạo các table
        public DbSet<Users> Users { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Wishlists> Wishlists { get; set; }
        public DbSet<WishlishDetails> WishlishDetails { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetail { get; set; }


        #endregion
        //Thiết kế các logic liên kết trong table
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users");
                entity.HasIndex(e => e.email).IsUnique(); // Email là duy nhất
            });
            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(e => e._id).HasColumnName("_id");
                entity.Property(e => e.rating).HasPrecision(3,1);
                entity.Property(e => e.price).HasPrecision(10,2);
                entity.Property(e => e.newPrice).HasPrecision(10,2);
            });
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.Property(e => e._id).HasColumnName("_id");
            });
            modelBuilder.Entity<Carts>(entity =>
            {
                entity.ToTable("Carts");

                entity.HasOne(e => e.Users)
                .WithMany(e => e.Carts)
                .HasForeignKey(e => e.idUser)
                .HasConstraintName("FK_Cart_User");

            });
            modelBuilder.Entity<CartDetails>(entity =>
            {
                entity.ToTable("CartDetails");

                entity.HasOne(e => e.Carts)
                .WithMany(e => e.CartDetails)
                .HasForeignKey(e => e.idCart)
                .HasConstraintName("FK_CartDetails_Cart");

                entity.HasOne(e => e.Products)
                .WithMany(e => e.CartDetails)
                .HasForeignKey(e => e._idProduct)
                .HasConstraintName("FK_CartDetails_Product");

            });
            modelBuilder.Entity<Wishlists>(entity =>
            {
                entity.ToTable("Wishlists");
                entity.HasOne(e => e.Users)
               .WithMany(e => e.Wishlists)
               .HasForeignKey(e => e.idUser)
               .HasConstraintName("FK_Wishlish_User");

            });
            modelBuilder.Entity<WishlishDetails>(entity =>
            {
                entity.ToTable("WishlishDetails");

                entity.HasOne(e => e.Wishlists)
                .WithMany(e => e.WishlishDetails)
                .HasForeignKey(e => e.idWishlish)
                .HasConstraintName("FK_WishlishDetails_Cart");

                entity.HasOne(e => e.Products)
                .WithMany(e => e.WishlishDetails)
                .HasForeignKey(e => e._idProduct)
                .HasConstraintName("FK_WishlishDetails_Product");

            });
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoice");
                entity.HasOne(e => e.Users)
               .WithMany(e => e.Invoices)
               .HasForeignKey(e => e.idUser)
               .HasConstraintName("FK_Invoice_User");

            });
            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.ToTable("InvoiceDetail");

                entity.HasOne(e => e.Invoice)
                .WithMany(e => e.InvoiceDetails)
                .HasForeignKey(e => e.idInvoice)
                .HasConstraintName("FK_InvoiceDetail_Invoice");

                entity.HasOne(e => e.Products)
                .WithMany(e => e.InvoiceDetails)
                .HasForeignKey(e => e._idProduct)
                .HasConstraintName("FK_InvoiceDetails_Product");

            });

        }
    }
}