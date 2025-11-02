using Bogus;
using DatabaseInitializer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatabaseInitializer.Data;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly Faker _faker;

    public DataSeeder(ApplicationDbContext context)
    {
        _context = context;
        _faker = new Faker("pl");
    }

    public async Task SeedAsync(int personCount = 50, int companyCount = 10)
    {
        // Check if data already exists
        if (await _context.People.AnyAsync())
        {
            Console.WriteLine("Database already contains data. Skipping seed.");
            return;
        }

        Console.WriteLine($"Seeding database with {personCount} people and {companyCount} companies...");

        // Step 1: Create People without relationships first
        var people = await CreatePeopleAsync(personCount);
        Console.WriteLine($"Created {people.Count} people");

        // Step 2: Add family relationships (Father, Mother, Spouse)
        await AddFamilyRelationshipsAsync(people);
        Console.WriteLine("Added family relationships");

        // Step 3: Create Companies (without Boss initially)
        var companies = await CreateCompaniesAsync(companyCount);
        Console.WriteLine($"Created {companies.Count} companies");

        // Step 4: Create Employees
        var employees = await CreateEmployeesAsync(people, companies);
        Console.WriteLine($"Created {employees.Count} employees");

        // Step 5: Assign Boss to Companies
        await AssignBossesToCompaniesAsync(companies, employees);
        Console.WriteLine("Assigned bosses to companies");

        Console.WriteLine("Database seeding completed successfully!");
    }

    private async Task<List<Entities.Person>> CreatePeopleAsync(int count)
    {
        var personFaker = new Faker<Entities.Person>("pl")
            .RuleFor(p => p.FirstName, (f, p) => f.Name.FirstName())
            .RuleFor(p => p.LastName, (f, p) => f.Name.LastName())
            .RuleFor(p => p.BirthDate, (f, p) => DateOnly.FromDateTime(f.Date.Between(new DateTime(1990, 1, 1), DateTime.Now)))
            .RuleFor(p => p.Gender, (f, p) => f.PickRandom(new[] { "Male", "Female" }))
            .RuleFor(p => p.FatherId, (f, p) => null)
            .RuleFor(p => p.MotherId, (f, p) => null)
            .RuleFor(p => p.SpouseId, (f, p) => null);

        var people = personFaker.Generate(count);
        
        await _context.People.AddRangeAsync(people);
        await _context.SaveChangesAsync();

        return people;
    }

    private async Task AddFamilyRelationshipsAsync(List<Entities.Person> people)
    {
        var random = new Random();
        
        // Add parent relationships - ensure everyone has at least one parent
        var potentialParents = people.Where(p => 
            DateOnly.FromDateTime(DateTime.Now).Year - p.BirthDate.Year > 25).ToList();
        
        var potentialChildren = people.Where(p => 
            DateOnly.FromDateTime(DateTime.Now).Year - p.BirthDate.Year < 40).ToList();

        var potentialFathers = potentialParents.Where(p => p.Gender == "Male").ToList();
        var potentialMothers = potentialParents.Where(p => p.Gender == "Female").ToList();

        foreach (var child in potentialChildren)
        {
            bool hasFather = false;
            bool hasMother = false;

            // Try to assign father
            if (potentialFathers.Any())
            {
                var father = potentialFathers
                    .Where(p => p.PersonId != child.PersonId)
                    .OrderBy(_ => random.Next())
                    .FirstOrDefault();
                
                if (father != null)
                {
                    child.FatherId = father.PersonId;
                    hasFather = true;
                }
            }

            // Try to assign mother
            if (potentialMothers.Any())
            {
                var mother = potentialMothers
                    .Where(p => p.PersonId != child.PersonId)
                    .OrderBy(_ => random.Next())
                    .FirstOrDefault();
                
                if (mother != null)
                {
                    child.MotherId = mother.PersonId;
                    hasMother = true;
                }
            }

            // Ensure at least one parent is assigned
            // If neither parent was assigned (shouldn't happen if lists aren't empty), 
            // force assignment of whichever is available
            if (!hasFather && !hasMother)
            {
                if (potentialFathers.Any())
                {
                    var father = potentialFathers.OrderBy(_ => random.Next()).First();
                    child.FatherId = father.PersonId;
                }
                else if (potentialMothers.Any())
                {
                    var mother = potentialMothers.OrderBy(_ => random.Next()).First();
                    child.MotherId = mother.PersonId;
                }
            }
        }

        // Add some spouse relationships
        var unmarried = people.Where(p => p.SpouseId == null).ToList();
        for (int i = 0; i < unmarried.Count - 1; i += 2)
        {
            if (random.Next(100) < 40) // 40% chance of being married
            {
                var person1 = unmarried[i];
                var person2 = unmarried[i + 1];
                
                person1.SpouseId = person2.PersonId;
                person2.SpouseId = person1.PersonId;
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task<List<Company>> CreateCompaniesAsync(int count)
    {
        var companyFaker = new Faker<Company>("pl")
            .RuleFor(c => c.Name, (f, c) => f.Company.CompanyName())
            .RuleFor(c => c.BossId, (f, c) => null); // Will be set later

        var companies = companyFaker.Generate(count);
        
        await _context.Companies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();

        return companies;
    }

    private async Task<List<Employee>> CreateEmployeesAsync(List<Entities.Person> people, List<Company> companies)
    {
        var random = new Random();
        var employees = new List<Employee>();
        var usedPersonIds = new HashSet<int>();

        var contractTypes = new[] { "Full-time", "Part-time", "Contract", "Temporary", "Internship" };

        // Create 2-10 employees per company
        foreach (var company in companies)
        {
            var employeeCount = random.Next(2, 11);
            var availablePeople = people
                .Where(p => !usedPersonIds.Contains(p.PersonId))
                .OrderBy(_ => random.Next())
                .Take(employeeCount)
                .ToList();

            foreach (var person in availablePeople)
            {
                var employee = new Employee
                {
                    EmployeeId = 0, // Will be auto-generated
                    PersonId = person.PersonId,
                    CompanyId = company.CompanyId,
                    ContractType = contractTypes[random.Next(contractTypes.Length)],
                    Salary = random.Next(3000, 15000)
                };

                employees.Add(employee);
                usedPersonIds.Add(person.PersonId);
            }
        }

        await _context.Employees.AddRangeAsync(employees);
        await _context.SaveChangesAsync();

        return employees;
    }

    private async Task AssignBossesToCompaniesAsync(List<Company> companies, List<Employee> employees)
    {
        var random = new Random();

        foreach (var company in companies)
        {
            var companyEmployees = employees
                .Where(e => e.CompanyId == company.CompanyId)
                .ToList();

            if (companyEmployees.Any())
            {
                // Pick a random employee as boss
                var boss = companyEmployees[random.Next(companyEmployees.Count)];
                company.BossId = boss.EmployeeId;
                
                // Give boss a higher salary
                boss.Salary = (int)(boss.Salary * 1.5);
            }
        }

        await _context.SaveChangesAsync();
    }
}
