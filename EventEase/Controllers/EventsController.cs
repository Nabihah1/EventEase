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
            var events = await _context.Event
                .Include(e => e.EventType) // Includes EventType
                .ToListAsync();

            return View(events);
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
            // Populate dropdowns for Event Type and Venue
            ViewBag.EventTypes = new SelectList(_context.EventType, "EventTypeID", "EventTypeName");
            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName");
            // Return a new Event model so @model isn't null in the view
            return View(new Event());



        }
        

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,EventName,StartEventDate,EndEventDate,StartTime,EndTime,EventDescription,EventImage,VenueID,EventTypeID")] Event @event,IFormFile image)
        {
            

            if (@event == null)
            {
                ModelState.AddModelError("", "Event cannot be null");
                ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName");
                ViewBag.EventTypes = new SelectList(_context.EventType, "EventTypeID", "EventTypeName");
                return View(new Event());
            }




            if (ModelState.IsValid)
            {
                // Check for conflicting bookings- prevents double booking 
                var conflictingBooking = await _context.Booking
                    .Include(b => b.Event)
                    .AnyAsync(b => b.VenueID == @event.VenueID
                    && b.Event != null
                    && b.Event.StartEventDate == @event.StartEventDate
                    && b.Event.EndEventDate == @event.EndEventDate
                    && (@event.StartTime < b.Event.EndTime)
                    && (b.Event.StartTime < @event.EndTime));

                if (conflictingBooking)
                {                
                    ModelState.AddModelError("", "This venue is already booked for the selected date and time.\nPlease select another venue or a different time.");
                  // Re-populate ViewBag (drop downs) for return view
                    ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName", @event.VenueID);
                    ViewBag.EventTypes = new SelectList(_context.EventType, "EventTypeID", "EventTypeName", @event.EventTypeID);

                    return View(@event ?? new Event()); // prevent saving
                }

              
           
                if (image is { Length: > 0 })
                {
                    var url = await _blobService.UploadFileAsync(image.OpenReadStream(),
                                                          Path.GetRandomFileName() + Path.GetExtension(image.FileName),
                                                          image.ContentType);
                    @event.EventImage = url;
                } else
                {
                    ModelState.AddModelError("", " Please Upload an image.");
                    // Re-populate ViewBag (drop downs) for return vie
                    ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName", @event.VenueID);
                    ViewBag.EventTypes = new SelectList(_context.EventType, "EventTypeID", "EventTypeName", @event.EventTypeID);
                    return View(@event ?? new Event());
                }

               
                //save the event 
                _context.Add(@event);
                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Index));
            }


            // Validation failed: reload dropdowns
            ViewBag.EventTypes = new SelectList(_context.EventType, "EventTypeID", "EventTypeName");
            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View(@event ?? new Event());
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
            ViewBag.EventTypes = new SelectList(_context.EventType, "EventTypeID", "EventTypeName");
            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,EventName,StartEventDate,EndEventDate,StartTime,EndTime,EventDescription,EventImage,VenueID,EventTypeID")] Event @event, IFormFile? image)
        {
            if (id != @event.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing event from DB 
                    var existingEvent = await _context.Event.AsNoTracking().FirstOrDefaultAsync(e => e.EventID == id);

                    if (existingEvent == null)
                        return NotFound();

                    if (image != null && image.Length > 0)
                    {
                        // Delete old blob if exists
                        if (!string.IsNullOrEmpty(existingEvent.EventImage))
                        {
                            var oldBlobName = Path.GetFileName(new Uri(existingEvent.EventImage).LocalPath);
                            await _blobService.DeleteFileAsync(oldBlobName);
                        }

                        // Upload new image
                        var newFileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName);
                        var newUrl = await _blobService.UploadFileAsync(image.OpenReadStream(), newFileName, image.ContentType);
                        @event.EventImage = newUrl;
                    }
                    else
                    {
                        // Keep old image URL if no new image uploaded
                        @event.EventImage = existingEvent.EventImage;
                    }


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

            // Re-populate dropdowns if validation fails
            ViewBag.EventTypes = new SelectList(_context.EventType, "EventTypeID", "EventTypeName");
            ViewBag.Venues = new SelectList(_context.Venue, "VenueID", "VenueName");
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
            bool hasBooking = await _context.Booking.AnyAsync(b => b.EventID == id);
  
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
                // Delete blob image if exists
                if (!string.IsNullOrEmpty(eventToActuallyDelete.EventImage))
                {
                    var blobName = Path.GetFileName(new Uri(eventToActuallyDelete.EventImage).LocalPath);
                    await _blobService.DeleteFileAsync(blobName);
                }


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
