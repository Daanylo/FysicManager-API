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
        public DbSet<Workshift> Workshifts { get; set; }
        public DbSet<AccessLog> AccessLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DateTime properties to be stored as UTC
            modelBuilder.Entity<Appointment>()
                .Property(e => e.Time)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<Workshift>()
                .Property(e => e.StartTime)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<Workshift>()
                .Property(e => e.EndTime)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<AccessLog>()
                .Property(e => e.Timestamp)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<Patient>()
                .Property(e => e.DateOfBirth)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        }
    }
}
