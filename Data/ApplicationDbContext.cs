using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogYonetimPaneli.Models;

namespace BlogYonetimPaneli.Data
{
    // Uygulamanın veritabanı bağlamı (DbContext). EF Core bu sınıf
    // üzerinden veritabanına sorgu gönderir ve değişiklik kaydeder.
    // IdentityDbContext'ten türediği için AspNetUsers, AspNetRoles gibi
    // Identity tabloları da otomatik olarak buraya dahil olur.
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Her DbSet, veritabanındaki bir tabloyu temsil eder.
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }

        // Model ilişkilerinin ve davranışlarının EF Core'a tarif edildiği yer.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity tablolarının kurulumu için gerekli

            // Post - Category arasındaki bire-çok ilişki açıkça tanımlanıyor.
            // Bir Category silinmeye çalışıldığında, ona bağlı Post kayıtları
            // varsa silme işlemi engellenir (DeleteBehavior.Restrict).
            builder.Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
