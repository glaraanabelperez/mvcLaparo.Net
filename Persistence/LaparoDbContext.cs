using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Abrazos.Persistence.Database
{
    public abstract class LaparoDbContext : DbContext
    {
        public LaparoDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            base.OnModelCreating(modelBuilder);
            ModelConfig(modelBuilder);

        }

        protected abstract void ModelConfig(ModelBuilder modelBuilder);
    }
}