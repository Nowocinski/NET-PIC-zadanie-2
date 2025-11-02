using DatabaseInitializer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseInitializer.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Person");
        
        builder.HasKey(p => p.PersonId);
        
        builder.Property(c => c.PersonId)
            .HasColumnName("person_id")
            .ValueGeneratedOnAdd();
        
        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(50);
        builder.Property(p => p.BirthDate).IsRequired();
        builder.Property(p => p.Gender).IsRequired().HasMaxLength(10);
        builder.Property(p => p.FatherId).IsRequired(false);
        builder.Property(p => p.MotherId).IsRequired(false);
        builder.Property(p => p.SpouseId).IsRequired(false);
        
        // Relationships
        builder.HasOne(p => p.Father)
            .WithMany()
            .HasForeignKey(p => p.FatherId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Mother)
            .WithMany()
            .HasForeignKey(p => p.MotherId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Spouse)
            .WithMany()
            .HasForeignKey(p => p.SpouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}