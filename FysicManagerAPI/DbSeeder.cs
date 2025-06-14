using FysicManagerAPI.Data;
using FysicManagerAPI.Models;

namespace FysicManagerAPI;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (!db.Practices.Any())
        {
            db.Practices.AddRange(new List<Practice>
            {
                new() { Name = "Fysio One", Address = "Main St 1", PostalCode = "1000AA", City = "Amsterdam", Country = "Netherlands", PhoneNumber = "+31111111111", Email = "one@fysio.com", Website = "https://fysioone.com", Color = "#FF0000" },
                new() { Name = "Fysio Two", Address = "Second St 2", PostalCode = "2000BB", City = "Rotterdam", Country = "Netherlands", PhoneNumber = "+31222222222", Email = "two@fysio.com", Website = "https://fysiotwo.com", Color = "#00FF00" }
            });
        }
        if (!db.Patients.Any())
        {
            db.Patients.AddRange(new List<Patient>
            {
                new() { FirstName = "John", LastName = "Doe", Initials = "J.D.", DateOfBirth = new DateTime(1990,1,1), Email = "john.doe@email.com", PhoneNumber = "0612345678", Address = "Patient St 1", PostalCode = "1234AB", City = "Amsterdam", Country = "Netherlands" },
                new() { FirstName = "Jane", LastName = "Smith", Initials = "J.S.", DateOfBirth = new DateTime(1985,5,20), Email = "jane.smith@email.com", PhoneNumber = "0687654321", Address = "Patient St 2", PostalCode = "4321BA", City = "Rotterdam", Country = "Netherlands" }
            });
        }
        db.SaveChanges();
    }
}
