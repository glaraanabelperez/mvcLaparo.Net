using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace mvc.Laparoscopy.Persistence.Configurations
{
    public class DiscountConfigurations : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(e => e.Id);
            builder.ToTable("Discount");
            builder.Property(e => e.Id)
                .HasColumnType("int")
                .HasColumnName("id");
            builder.Property(e => e.Percentage)
              .HasColumnName("percentage");
            builder.Property(e => e.State)
               .HasColumnName("state");

            builder.HasMany(e => e.Products)
            .WithOne(e => e.Discount_)
            .HasForeignKey(e => e.DiscountId);

        }
    }
}
