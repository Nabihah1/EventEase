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
    public class VenuesController : Controller
    {
        private readonly EventEaseContext _context;
        private readonly IBlobService _blobService;

        public VenuesController(EventEaseContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venue.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueID,VenueName,VenueLocation,VenueCapacity,VenueImage")] Venue venue, IFormFile image)
        {

            if (ModelState.IsValid)
            {

                if (image is { Length: > 0 })
                {
                    var url = await _blobService.UploadFileAsync(image.OpenReadStream(),
                                                          Path.GetRandomFileName() + Path.GetExtension(image.FileName),
                                                          image.ContentType);
                    venue.VenueImage = url;
                }
                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueID,VenueName,VenueLocation,VenueCapacity,VenueImage")] Venue venue, IFormFile? image)
        {
            if (id != venue.VenueID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current venue from DB to check old image
                    var existingVenue = await _context.Venue.AsNoTracking().FirstOrDefaultAsync(v => v.VenueID == venue.VenueID);

                    if (image != null && image.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingVenue?.VenueImage))
                        {
                            var oldBlobName = Path.GetFileName(new Uri(existingVenue.VenueImage).LocalPath);
                            await _blobService.DeleteFileAsync(oldBlobName);
                        }

                        var newFileName = Path.GetRandomFileName() + Path.GetExtension(image.FileName);
                        var newUrl = await _blobService.UploadFileAsync(image.OpenReadStream(), newFileName, image.ContentType);
                        venue.VenueImage = newUrl;
                    }
                    else
                    {
                        // Keep existing image URL if no new image uploaded
                        venue.VenueImage = existingVenue?.VenueImage;
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID))
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
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venue
                .FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //check if any booking exists for an venue
            bool hasBooking = await _context.Booking.AnyAsync(b => b.VenueID == id);

            if (hasBooking)
            {
                //retrieve the VenueID to re-display the Delete view with error 
                var venueToDelete = await _context.Venue.FindAsync(id);
                ModelState.AddModelError("", "This venue cannot be deleted.\nThere is an existing booking record for this event!");
                return View(venueToDelete);
            }

            var venueToActuallyDelete = await _context.Venue.FindAsync(id);
            if (venueToActuallyDelete != null)

            {
                // Delete the blob image first (if exists)
                if (!string.IsNullOrEmpty(venueToActuallyDelete.VenueImage))
                {
                    var blobName = Path.GetFileName(new Uri(venueToActuallyDelete.VenueImage).LocalPath);
                    await _blobService.DeleteFileAsync(blobName);
                }

                _context.Venue.Remove(venueToActuallyDelete);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.VenueID == id);
        }
    }
}
