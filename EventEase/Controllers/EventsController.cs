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
        public IActionResult Create(Event AnotherEvent, Venue AnotherVenue)
        {
            // Check if another event is already booked at the same venue and time
            bool isVenueBooked = _context.Event.Any(e =>
                e.EventDate == AnotherEvent.EventDate &&
                e.StartTime < AnotherEvent.StartTime &&
                e.EndTime > AnotherEvent.EndTime);

            bool isVenueBooked1 = _context.Venue.Any(v =>
            v.VenueID == AnotherVenue.VenueID);

            if (isVenueBooked && isVenueBooked1)
            {
                ModelState.AddModelError("", "This venue is already booked at the selected date and time.\nPlease select another venue or time.");
               return View(AnotherEvent);
            }
            return View(AnotherEvent);
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,EventName,EventDate,StartTime,EndTime,EventDescription,EventImage")] Event @event,IFormFile image)
        {
            if (ModelState.IsValid)
            {

                if (image is { Length: > 0 })
                {
                    var url = await _blobService.UploadFileAsync(image.OpenReadStream(),
                                                          Path.GetRandomFileName() + Path.GetExtension(image.FileName),
                                                          image.ContentType);
                    @event.EventImage = url;
                }
                _context.Add(@event);
                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Index));
            }
               
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
            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventID == id);
        }
    }
}
