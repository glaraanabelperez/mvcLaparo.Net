using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace mvc.Laparoscopy.Persistence.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);
            builder.ToTable("User");
            builder.Property(e => e.Id)
                .HasColumnType("int")
                .HasColumnName("Id");
            builder.Property(e => e.Email)
              .HasColumnName("Email");
            builder.Property(e => e.Pass)
              .HasColumnName("Pass");
           
        }
    }
}
