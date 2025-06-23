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
        public DbSet<EventEase.Models.EventType> EventType { get; set; } = default!; 


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


            //add seeded data for the event type - this options will show up when user selects types of event 
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeID = 1, EventTypeName = "Wedding"},
                   new EventType { EventTypeID = 2, EventTypeName = "Baby Shower" },
                      new EventType { EventTypeID = 3, EventTypeName = "Birthday" },
                         new EventType { EventTypeID = 4, EventTypeName = "Conference" },
                            new EventType { EventTypeID = 5, EventTypeName = "Reunion" },
                               new EventType { EventTypeID = 6, EventTypeName = "Graduation" }

                ); 


        }






    }
}
