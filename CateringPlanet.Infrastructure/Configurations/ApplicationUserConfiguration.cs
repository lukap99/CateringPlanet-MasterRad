using CateringPlanet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CateringPlanet.Infrastructure.Configurations;

/// <summary>EF Core configuration for the Identity <c>ApplicationUser</c> (AspNetUsers table).</summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Custom columns are kept snake_case to match database naming convention
        builder.Property(u => u.FirstName)
            .HasMaxLength(20)
            .HasColumnName("first_name");
        builder.Property(u => u.LastName)
            .HasMaxLength(20)
            .HasColumnName("last_name");
        builder.Property(u => u.Points)
            .HasColumnName("points");
    }
}
