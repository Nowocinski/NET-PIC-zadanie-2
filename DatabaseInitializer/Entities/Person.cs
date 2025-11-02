namespace DatabaseInitializer.Entities;

public class Person
{
    public required int PersonId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOnly Birth { get; set; }
    public required string Gender { get; set; }
    public int? FatherId { get; set; }
    public int? MotherId { get; set; }
    public int? SpouseId { get; set; }
    
    // Navigation properties
    public virtual Person? Father { get; set; }
    public virtual Person? Mother { get; set; }
    public virtual Person? Spouse { get; set; }
}