using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Data
{
    public class EventEaseContext : DbContext
    {
        public EventEaseContext (DbContextOptions<EventEaseContext> options)
            : base(options)
        {
        }

        public DbSet<EventEase.Models.Event> Event { get; set; } = default!;
        public DbSet<EventEase.Models.Venue> Venue { get; set; } = default!;
        public DbSet<EventEase.Models.Booking> Booking { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent cascade delete errors- configure foreign key between event and venue 
            //with no cascade on delete

            modelBuilder.Entity<Event>()
               .HasOne(e => e.Venue)  // The Event has one Venue
        .WithMany(v => v.Events) // A Venue can have many Events
        .HasForeignKey(e => e.VenueID)  // Foreign key on Event
        .OnDelete(DeleteBehavior.Restrict); // Specify no cascade on delete
        }






    }
}
