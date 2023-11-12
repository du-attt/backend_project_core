using backend_project_core.Data;
using Microsoft.EntityFrameworkCore;

namespace backend_project_core.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.; initial Catalog=project; User Id=sa; password=hoimanchi03; TrustServerCertificate = True");
        }

        //Cách kết nối linh hoạt hơn 
        //public DbSet<Users> Users { get; set; }
        //#endregion

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
           
        //    modelBuilder.Entity<Users>(entity => {
        //        entity.HasIndex(e => e.UserName).IsUnique();
        //        entity.Property(e => e.HoTen).IsRequired().HasMaxLength(150);
        //        entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
        //    });
        //}
    }
}