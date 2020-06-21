using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kolokwium.Models
{
    public class EventsDbContext : DbContext
    {
        public EventsDbContext()
        {

        }

        public EventsDbContext(DbContextOptions<EventsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Artist> Artist { get; set; }
        public DbSet<ArtistEvent> ArtistEvent { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventOrganiser> EventOrganiser { get; set; }
        public DbSet<Organiser> Organiser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=Kolokwium;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(e => e.IdArtist);
                entity.Property(p => p.IdArtist).ValueGeneratedOnAdd();

                entity.Property(e => e.Nickname)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<ArtistEvent>(entity =>
            {
                entity.HasKey(e => new { e.IdEvent, e.IdArtist });

                entity.Property(e => e.PerformanceDate).IsRequired();

                entity.HasOne(ae => ae.Artist)
                    .WithMany(a => a.ArtistEvents)
                    .HasForeignKey(ae => ae.IdArtist)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(ae => ae.Event)
                    .WithMany(e => e.ArtistEvents)
                    .HasForeignKey(ae => ae.IdEvent)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Event>(entity => {
                entity.HasKey(e => e.IdEvent);
                entity.Property(p => p.IdEvent).ValueGeneratedOnAdd();

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.StartDate).IsRequired();

                entity.Property(p => p.EndDate).IsRequired();
            });

            modelBuilder.Entity<EventOrganiser>(entity => {
                entity.HasKey(e => new { e.IdEvent, e.IdOrganiser });

                entity.HasOne(eo => eo.Event)
                    .WithMany(e => e.EventOrganisers)
                    .HasForeignKey(eo => eo.IdEvent)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(eo => eo.Organiser)
                    .WithMany(o => o.EventOrganisers)
                    .HasForeignKey(eo => eo.IdOrganiser)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Organiser>(entity => {
                entity.HasKey(e => e.IdOrganizer);
                entity.Property(e => e.IdOrganizer).ValueGeneratedOnAdd();

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(30);
            });
        }
    }
}
