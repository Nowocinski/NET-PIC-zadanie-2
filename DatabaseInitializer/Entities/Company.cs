namespace DatabaseInitializer.Entities;

public class Company
{
    public required int CompanyId { get; set; }
    public required string Name { get; set; }
    public required int BossId { get; set; }
}