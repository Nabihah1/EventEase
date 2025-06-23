using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using Humanizer.Localisation;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseContext _context;

        public BookingsController(EventEaseContext context)
        {
            _context = context;

        }                

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            if (_context.Booking == null)
            {
                return Problem("Entity set 'EventEase.Context.Booking'  is null.");
            }

            var booking = await _context.Booking
           .Include(b => b.Event)
           .Include(b => b.Venue)
           .ToListAsync();

           

            return View(booking);
                        
        }


        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }


        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventDescription");
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueLocation");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,EventID,VenueID,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventDescription", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueLocation", booking.VenueID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventDescription", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueLocation", booking.VenueID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,EventID,VenueID,BookingDate")] Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventID"] = new SelectList(_context.Event, "EventID", "EventDescription", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueLocation", booking.VenueID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //find the booking by its ID
            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Delete the booking
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            // After the booking is deleted, check if there are any remaining bookings for the venue or event 
            var venueCheck = booking.Venue;
            var eventCheck = booking.Event;

            // Delete the venue if no more bookings are associated
            bool venueHasOtherBookings = await _context.Booking
                .AnyAsync(b => b.VenueID == venueCheck.VenueID);

            if (!venueHasOtherBookings)
            {
                _context.Venue.Remove(venueCheck);
                await _context.SaveChangesAsync();
            }

            // Delete the event if no more bookings are associated
            bool eventHasOtherBookings = await _context.Booking
                .AnyAsync(b => b.EventID == eventCheck.EventID);

            if (!eventHasOtherBookings)
            {
                _context.Event.Remove(eventCheck);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingID == id);
        }
    }
}
