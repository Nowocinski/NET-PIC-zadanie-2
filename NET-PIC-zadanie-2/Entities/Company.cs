namespace NET_PIC_zadanie_2.Entities;

public class Company
{
    public required int CompanyId { get; set; }
    public required string Name { get; set; }
    public required int BossId { get; set; }
}