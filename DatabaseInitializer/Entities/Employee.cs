namespace DatabaseInitializer.Entities;

public class Employee
{
    public required int EmployeeId { get; set; }
    public required int PersonId { get; set; }
    public required int CompanyId { get; set; }
    public required string ContractType { get; set; }
    public required int Salary { get; set; }
}