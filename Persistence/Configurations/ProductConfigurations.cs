using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace mvc.Laparoscopy.Persistence.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
 
            builder.HasKey(e => e.Id);
            builder.ToTable("Product");
            builder.Property(e => e.Id)
                .HasColumnType("int")
                .HasColumnName("id");
           
            builder.Property(e => e.Name)
            .HasColumnName("name");
            builder.Property(e => e.Description)
              .HasColumnName("description");

            builder.Property(e => e.image)
            .HasColumnName("image");
            builder.Property(e => e.Price)
               .HasColumnName("price");
            builder.Property(e => e.TotalPrice)
                .HasColumnName("totalPrice");
            builder.Property(e => e.DateInit)
              .HasColumnName("dateInit");
            builder.Property(e => e.State)
              .HasColumnName("state");
            builder.Property(e => e.Fauvorite)
              .HasColumnName("fauvorite");
            builder.Property(e => e.DiscountId)
               .HasColumnName("discountId");

            builder.Property(e => e.Codigo)
                .HasColumnName("codigo");

            builder.Property(e => e.CategoryNameId)
                .HasColumnName("CategoryNameId");



            builder.HasOne(e => e.Category_)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.CategoryNameId);

            builder.HasOne(e => e.Discount_)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.DiscountId);
        }
    }
}
