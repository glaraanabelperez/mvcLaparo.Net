using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace mvc.Laparoscopy.Persistence.Configurations
{
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
 
            builder.HasKey(e => e.Name);
            builder.ToTable("Category");
            builder.Property(e => e.Name)
                .HasColumnName("Name");
           

            builder.HasMany(d => d.Products)
            .WithOne(e => e.Category_)
            .HasForeignKey(d=> d.CategoryNameId);
        }
    }
}
