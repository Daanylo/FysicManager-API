using FysicManagerAPI.Data;
using FysicManagerAPI.Models;
using FysicManagerAPI.Models.DTOs;

namespace FysicManagerAPI;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (!db.Specializations.Any())
        {
            db.Specializations.AddRange(new List<Specialization>
            {
                new() { Name = "Nek", Description = "Gespecialiseerd in nekklachten" },
                new() { Name = "Schouder", Description = "Gespecialiseerd in schouderklachten" },
                new() { Name = "Rug", Description = "Gespecialiseerd in rugklachten" },
                new() { Name = "Heup", Description = "Gespecialiseerd in heupklachten" },
                new() { Name = "Knie", Description = "Gespecialiseerd in knieklachten" },
                new() { Name = "Enkel", Description = "Gespecialiseerd in enkelklachten" },
                new() { Name = "Bekkentherapie", Description = "Gespecialiseerd in bekkentherapie" },
            });
        }
        if (!db.AppointmentTypes.Any())
        {
            db.AppointmentTypes.AddRange(new List<AppointmentType>
            {
                new() { Name = "1000 - behandeling fysiotherapie", Description = "Standaard fysiotherapie behandeling in praktijk", Color = "#FF5733" },
                new() { Name = "1864 - intake en onderzoek fysiotherapie", Description = "Intake en onderzoek voor fysiotherapie", Color = "#33FF57" },
                new() { Name = "1600 - behandeling bekkentherapie", Description = "Bekkentherapie behandeling in praktijk", Color = "#3357FF" },
                new() { Name = "1001 - behandeling fysiotherapie incl. aan huis toeslag", Description = "Fysiotherapie behandeling aan huis met toeslag", Color = "#FF33A1" },
            });
            db.SaveChanges();
        }
        if (!db.Practices.Any())
        {
            db.Practices.AddRange(new List<Practice>
            {
                new() { Name = "Fysio One", Address = "Main St 1", PostalCode = "1000AA", City = "Amsterdam", Country = "Netherlands", PhoneNumber = "+31111111111", Email = "one@fysio.com", Website = "https://fysioone.com", Color = "#FF0000" },
                new() { Name = "Fysio Two", Address = "Second St 2", PostalCode = "2000BB", City = "Rotterdam", Country = "Netherlands", PhoneNumber = "+31222222222", Email = "two@fysio.com", Website = "https://fysiotwo.com", Color = "#00FF00" }
            });
            db.SaveChanges();
        }
        if (!db.Patients.Any())
        {
            db.Patients.AddRange(new List<Patient>
            {
                new() { FirstName = "John", LastName = "Doe", Initials = "J.D.", DateOfBirth = new DateTime(1990,1,1), Email = "john.doe@email.com", PhoneNumber = "0612345678", Address = "Patient St 1", PostalCode = "1234AB", City = "Amsterdam", Country = "Netherlands", BSN = "123456789" },
                new() { FirstName = "Jane", LastName = "Smith", Initials = "J.S.", DateOfBirth = new DateTime(1985,5,20), Email = "jane.smith@email.com", PhoneNumber = "0687654321", Address = "Patient St 2", PostalCode = "4321BA", City = "Rotterdam", Country = "Netherlands", BSN = "987654321" }
            });
            db.SaveChanges();
        }
        if (!db.Therapists.Any())
        {
            var specNek = db.Specializations.FirstOrDefault(s => s.Name == "Nek");
            var specSchouder = db.Specializations.FirstOrDefault(s => s.Name == "Schouder");
            var practice1 = db.Practices.FirstOrDefault(p => p.Name == "Fysio One");
            var practice2 = db.Practices.FirstOrDefault(p => p.Name == "Fysio Two");
            db.Therapists.AddRange(new List<Therapist>
            {
                new() { Name = "Alice Johnson",
                        Specializations = specNek != null ? [specNek] : [],
                        PhoneNumber = "0612345678", Email = "alicejohnson@email.com",
                        Practices = practice1 != null ? [practice1] : [] },
                new() { Name = "Bob Brown",
                        Specializations = specSchouder != null ? [specSchouder] : [],
                        PhoneNumber = "0687654321", Email = "bobbrown@email.com",
                        Practices = practice2 != null ? [practice2] : [] }
            });
            db.SaveChanges();
        }
        if (!db.Appointments.Any())
        {
            var patient1 = db.Patients.FirstOrDefault(p => p.FirstName == "John");
            var patient2 = db.Patients.FirstOrDefault(p => p.FirstName == "Jane");

            var practice1 = db.Practices.FirstOrDefault(p => p.Name == "Fysio One");
            var practice2 = db.Practices.FirstOrDefault(p => p.Name == "Fysio Two");

            var therapist1 = db.Therapists.FirstOrDefault(t => t.Name == "Alice Johnson");
            var therapist2 = db.Therapists.FirstOrDefault(t => t.Name == "Bob Brown");

            var appointmentType1 = db.AppointmentTypes.FirstOrDefault(a => a.Name.StartsWith("1000"));
            var appointmentType2 = db.AppointmentTypes.FirstOrDefault(a => a.Name.StartsWith("1864"));

            if (patient1 == null || patient2 == null || practice1 == null || practice2 == null || therapist1 == null || therapist2 == null)
            {
                throw new InvalidOperationException("Not all required entities are available for seeding appointments.");
            }
            db.Appointments.AddRange(new List<Appointment>
            {
                new()
                {
                    Description = "Nekklachten",
                    Patient = patient1,
                    Practice = practice1,
                    Therapist = therapist1,
                    AppointmentType = appointmentType1 ?? new AppointmentType { Name = "1000 - behandeling fysiotherapie", Description = "Standaard fysiotherapie behandeling in praktijk" },
                    Time = new DateTime(2025, 6, 14, 12, 00, 00),
                    Duration = 25,
                    Notes = ""
                },
                new()
                {
                    Description = "Schouderklachten",
                    Patient = patient2,
                    Practice = practice2,
                    Therapist = therapist2,
                    AppointmentType = appointmentType2 ?? new AppointmentType { Name = "1864 - intake en onderzoek fysiotherapie", Description = "Intake en onderzoek voor fysiotherapie" },
                    Time = new DateTime(2025, 6, 14, 13, 00, 00),
                    Duration = 25,
                    Notes = ""
                }
            });
        }
        
        if (!db.Workshifts.Any())
        {
            var practice1 = db.Practices.FirstOrDefault(p => p.Name == "Fysio One");
            var practice2 = db.Practices.FirstOrDefault(p => p.Name == "Fysio Two");

            var therapist1 = db.Therapists.FirstOrDefault(t => t.Name == "Alice Johnson");
            var therapist2 = db.Therapists.FirstOrDefault(t => t.Name == "Bob Brown");

            if (practice1 == null || practice2 == null || therapist1 == null || therapist2 == null)
            {
                throw new InvalidOperationException("Not all required entities are available for seeding workshifts.");
            }

            var workshifts = new List<Workshift>();

            // Create workshifts for June 16-18, 2025
            for (int day = 16; day <= 18; day++)
            {
                var date = new DateTime(2025, 6, day);
                
                // Alice Johnson (Therapist 1) - Monday to Wednesday
                // Morning shift: 8:00 - 12:00
                workshifts.Add(new Workshift
                {
                    StartTime = new DateTime(2025, 6, day, 8, 0, 0),
                    EndTime = new DateTime(2025, 6, day, 12, 0, 0),
                    Therapist = therapist1,
                    Practice = practice1
                });
                
                // Afternoon shift: 13:00 - 17:00
                workshifts.Add(new Workshift
                {
                    StartTime = new DateTime(2025, 6, day, 13, 0, 0),
                    EndTime = new DateTime(2025, 6, day, 17, 0, 0),
                    Therapist = therapist1,
                    Practice = practice1
                });

                // Bob Brown (Therapist 2) - Monday to Wednesday
                // Morning shift: 9:00 - 13:00
                workshifts.Add(new Workshift
                {
                    StartTime = new DateTime(2025, 6, day, 9, 0, 0),
                    EndTime = new DateTime(2025, 6, day, 13, 0, 0),
                    Therapist = therapist2,
                    Practice = practice2
                });
                
                // Afternoon shift: 14:00 - 18:00
                workshifts.Add(new Workshift
                {
                    StartTime = new DateTime(2025, 6, day, 14, 0, 0),
                    EndTime = new DateTime(2025, 6, day, 18, 0, 0),
                    Therapist = therapist2,
                    Practice = practice2
                });
            }

            db.Workshifts.AddRange(workshifts);
        }
        
        db.SaveChanges();
    }
}
