using backend_project_core.Data;
using Microsoft.EntityFrameworkCore;

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


        #endregion
        //Thiết kế các logic liên kết trong table
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>(entity =>
            {
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
        }
    }
}