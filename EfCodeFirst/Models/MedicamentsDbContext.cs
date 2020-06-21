using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCodeFirst.Models
{
    public class MedicamentsDbContext : DbContext
    {
        public MedicamentsDbContext()
        {
        }

        public MedicamentsDbContext(DbContextOptions<MedicamentsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Doctor> Doctor { get; set; }
        public virtual DbSet<Medicament> Medicament { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<Prescription> Prescription { get; set; }
        public virtual DbSet<PrescriptionMedicament> PrescriptionMedicament { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.IdDoctor);
                entity.Property(p => p.IdDoctor).ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                // Seed doctors table
                var doctors = new[]
                {
                    new Doctor { IdDoctor=1, FirstName="Albert", LastName="Sum", Email="as@gmail.com" },
                    new Doctor { IdDoctor=2, FirstName="Adam", LastName="Brown", Email="ab@gmail.com" },
                    new Doctor { IdDoctor=3, FirstName="Catherine", LastName="Smith", Email="as@gmail.com" },
                };

                entity.HasData(doctors);
            });

            modelBuilder.Entity<Medicament>(entity =>
            {
                entity.HasKey(e => e.IdMedicament);
                entity.Property(e => e.IdMedicament).ValueGeneratedOnAdd();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);

                // Seed medicaments table
                var medicaments = new[]
                {
                    new Medicament { IdMedicament=1, Name="Xanax", Description="Short-acting benzodiazepine", Type="Pills" },
                    new Medicament { IdMedicament=2, Name="Adderall", Description="Drug containing salts of amphetamine", Type="Pills" },
                    new Medicament { IdMedicament=3, Name="Vicodin", Description="Effective pain killer", Type="Pills" },
                };

                entity.HasData(medicaments);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.IdPatient);
                entity.Property(e => e.IdPatient).ValueGeneratedOnAdd();

                entity.Property(e => e.Birthdate)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                // Seed patients table
                var patients = new[]
                {
                    new Patient { IdPatient=1, FirstName="Martin", LastName="Gooseman", Birthdate=DateTime.Now },
                    new Patient { IdPatient=2, FirstName="Luke", LastName="Krause", Birthdate=DateTime.Now },
                    new Patient { IdPatient=3, FirstName="Kyron", LastName="Harwood", Birthdate=DateTime.Now },
                };

                entity.HasData(patients);
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasKey(e => e.IdPrescription);
                entity.Property(e => e.IdPrescription).ValueGeneratedOnAdd();

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(e => e.DueDate)
                    .IsRequired()
                    .HasColumnType("date");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.IdDoctor)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.IdPatient)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                // Seed prescriptions table
                var prescriptions = new[]
                {
                    new Prescription { IdPrescription=1, IdDoctor=1, IdPatient=1, Date=DateTime.Now, DueDate=DateTime.Now.AddDays(1) },
                    new Prescription { IdPrescription=2, IdDoctor=2, IdPatient=2, Date=DateTime.Now, DueDate=DateTime.Now.AddDays(1) },
                    new Prescription { IdPrescription=3, IdDoctor=3, IdPatient=3, Date=DateTime.Now, DueDate=DateTime.Now.AddDays(1) }
                };

                entity.HasData(prescriptions);
            });

            modelBuilder.Entity<PrescriptionMedicament>(entity =>
            {
                entity.HasKey(e => new { e.IdMedicament, e.IdPrescription });

                entity.Property(e => e.Details)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Medicament)
                    .WithMany(p => p.PrescriptionMedicaments)
                    .HasForeignKey(d => d.IdMedicament)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Prescription)
                    .WithMany(p => p.PrescriptionMedicaments)
                    .HasForeignKey(d => d.IdPrescription)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                // Seed Prescription_Medicament table
                var prescriptionMedicaments = new[]
                {
                    new PrescriptionMedicament { IdPrescription=1, IdMedicament=1, Details="Lorem ipsum dolor", Dose=15 },
                    new PrescriptionMedicament { IdPrescription=1, IdMedicament=3, Details="Lorem ipsum dolor", Dose=10 },
                    new PrescriptionMedicament { IdPrescription=2, IdMedicament=2, Details="Lorem ipsum dolor", Dose=8 },
                    new PrescriptionMedicament { IdPrescription=2, IdMedicament=3, Details="Lorem ipsum dolor", Dose=6 },
                    new PrescriptionMedicament { IdPrescription=3, IdMedicament=1, Details="Lorem ipsum dolor", Dose=3 },
                    new PrescriptionMedicament { IdPrescription=3, IdMedicament=2, Details="Lorem ipsum dolor", Dose=1 },
                };

                entity.HasData(prescriptionMedicaments);
            });
        }
    }
}
