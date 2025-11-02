using DatabaseInitializer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseInitializer.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employee");
        
        builder.HasKey(e => e.EmployeeId);
        
        builder.Property(e => e.EmployeeId)
            .HasColumnName("employee_id")
            .ValueGeneratedOnAdd();
        
        builder.Property(e => e.ContractType).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Salary).IsRequired();
    }
}