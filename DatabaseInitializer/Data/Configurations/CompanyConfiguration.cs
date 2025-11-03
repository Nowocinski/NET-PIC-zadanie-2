using DatabaseInitializer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseInitializer.Data.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");
        
        builder.HasKey(c => c.CompanyId);
        
        builder.Property(c => c.CompanyId)
            .HasColumnName("company_id")
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.BossId).IsRequired(false);
        
        // Relationships
        builder.HasOne(c => c.Boss)
            .WithMany()
            .HasForeignKey(c => c.BossId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        builder.HasMany(c => c.Employees)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(c => c.Name);
    }
}