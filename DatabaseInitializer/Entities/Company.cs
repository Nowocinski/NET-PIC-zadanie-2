namespace DatabaseInitializer.Entities;

public class Company
{
    public required int CompanyId { get; set; }
    public required string Name { get; set; }
    public required int BossId { get; set; }
    
    // Navigation properties
    public virtual Employee Boss { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}