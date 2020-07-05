using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KolokwiumPopr.Models
{
    public class FirefightersDbContext : DbContext
    {
        public FirefightersDbContext()
        {

        }

        public FirefightersDbContext(DbContextOptions<FirefightersDbContext> options)
            : base(options)
        {
        }

        public DbSet<Firefighter> Firefighter { get; set; }
        public DbSet<FirefighterAction> FirefighterAction { get; set; }
        public DbSet<Action> Action { get; set; }
        public DbSet<FiretruckAction> FiretruckAction { get; set; }
        public DbSet<Firetruck> Firetruck { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=KolokwiumPopr;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Firefighter>(entity =>
            {
                entity.HasKey(e => e.IdFirefighter);
                entity.Property(p => p.IdFirefighter).ValueGeneratedOnAdd();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<FirefighterAction>(entity =>
            {
                entity.HasKey(e => new { e.IdFirefighter, e.IdAction });

                entity.HasOne(e => e.Firefighter)
                    .WithMany(e => e.FirefighterActions)
                    .HasForeignKey(e => e.IdFirefighter)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.Action)
                    .WithMany(e => e.FirefighterActions)
                    .HasForeignKey(e => e.IdAction)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Action>(entity => {
                entity.HasKey(e => e.IdAction);
                entity.Property(p => p.IdAction).ValueGeneratedOnAdd();

                entity.Property(p => p.StartDate).IsRequired();

                entity.Property(p => p.NeedSpecialEquipment).IsRequired();
                entity.Property(p => p.NeedSpecialEquipment).HasColumnType("bit");
            });

            modelBuilder.Entity<FiretruckAction>(entity => {
                entity.HasKey(e => e.IdFiretruckAction);

                entity.HasOne(e => e.Action)
                    .WithMany(e => e.FiretruckActions)
                    .HasForeignKey(e => e.IdAction)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(e => e.Firetruck)
                    .WithMany(e => e.FiretruckActions)
                    .HasForeignKey(e => e.IdFireTruck)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.Property(p => p.AssignmentDate).IsRequired();

            });

            modelBuilder.Entity<Firetruck>(entity => {
                entity.HasKey(e => e.IdFireTruck);
                entity.Property(e => e.IdFireTruck).ValueGeneratedOnAdd();

                entity.Property(p => p.OperationalNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(p => p.SpecialEquipment).IsRequired();
                entity.Property(p => p.SpecialEquipment).HasColumnType("bit");
            });
        }
    }
}
