using Microsoft.EntityFrameworkCore;
using FysicManagerAPI.Models;

namespace FysicManagerAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Practice> Practices { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Therapist> Therapists { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
    }
}
