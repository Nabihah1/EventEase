using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.Services;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventEaseContext _context;
        private readonly IBlobService _blobService; 

        public EventsController(EventEaseContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService ;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Event.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            // Pass the list of venues to the view
            ViewBag.Venues = _context.Venue.ToList();
            return View();

        }
        

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,EventName,EventDate,StartTime,EndTime,EventDescription,EventImage")] Event @event,IFormFile image, int venueID)
        {
 
            if (ModelState.IsValid)
            {

                // Check for conflicting bookings- prevents double booking 
                var conflictingBooking = await _context.Booking
                    .Include(b => b.Event)
                    .Where(b => b.VenueID == venueID && b.Event.EventDate == @event.EventDate)
                    .AnyAsync(b =>
                        (@event.StartTime < b.Event.EndTime) && (b.Event.StartTime < @event.EndTime)
                    );

                if (conflictingBooking)
                {
                    ViewBag.Venues = _context.Venue.ToList(); // Re-populate ViewBag for return view
                    ModelState.AddModelError("", "This venue is already booked for the selected date and time.\nPlease select another venue or a different time.");
                    return View(@event); // prevent saving
                }

              
           
                if (image is { Length: > 0 })
                {
                    var url = await _blobService.UploadFileAsync(image.OpenReadStream(),
                                                          Path.GetRandomFileName() + Path.GetExtension(image.FileName),
                                                          image.ContentType);
                    @event.EventImage = url;
                } else
                {
                    ModelState.AddModelError("", "Upload an image.");
                    ViewBag.Venues = _context.Venue.ToList();
                    return View(@event);
                }

                @event.VenueID = venueID; // Link event to selected venue

                //save the event 
                _context.Add(@event);
                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Venues = _context.Venue.ToList(); // Needed on error

            return View(@event);
            }
        

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,EventName,EventDate,StartTime,EndTime,EventDescription,EventImage")] Event @event)
        {
            if (id != @event.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.EventID == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //check if any booking exists for an event
            bool hasBooking = _context.Event.Any(b => b.EventID == id);
  
            if (hasBooking)
            {
                //retrieve the EventID to re-display the Delete view with error 
                var eventToDelete = await _context.Event.FindAsync(id);
                ModelState.AddModelError("", "This event cannot be deleted.\nThere is an existing booking record for this event!");
             return View(eventToDelete);
            }

            var eventToActuallyDelete = await _context.Event.FindAsync(id);
            if (eventToActuallyDelete != null)
            {
                _context.Event.Remove(eventToActuallyDelete);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));

        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventID == id);
        }
    }
}
