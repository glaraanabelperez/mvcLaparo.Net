using Abrazos.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Models;
using mvc.Laparoscopy.Persistence.Configurations;

namespace mvc.Laparoscopy.Persistence
{
    public  class ApplicationDbContext : LaparoDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Discount> Discount_ { get; set; } = null!;


        protected override void ModelConfig(ModelBuilder modelBuilder)
        {
            //modelBuilder.UseCollation("Modern_Spanish_CI_AS");
            modelBuilder.ApplyConfiguration(new ProductConfigurations());
            modelBuilder.ApplyConfiguration(new UserConfigurations());
            modelBuilder.ApplyConfiguration(new DiscountConfigurations());

        }
    }
}

