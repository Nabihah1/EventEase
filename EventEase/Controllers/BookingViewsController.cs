using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class BookingViewsController : Controller
    {
        private readonly EventEaseContext _context;

        public BookingViewsController(EventEaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {

            // Include Venue name 
            var bookings = _context.Booking
                .Include(b => b.Event)
                .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            //filter by event name or bookingID 
            if (!String.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                b.Event != null && b.Event.EventName.Contains(searchString) ||
                  b.BookingID.ToString().Contains(searchString));
            }



            // Go to BookingViewModel
            var bookingViewModels = await bookings
                .Select(b => new BookingViewModel
                {//display the following information 
                    BookingID = b.BookingID,
                    EventName = b.Event.EventName,
                    EventDescription = b.Event.EventDescription,
                    StartEventDate = b.Event.StartEventDate,
                    EndEventDate = b.Event.EndEventDate,
                    StartTime = b.Event.StartTime,
                    EndTime = b.Event.EndTime,
                    VenueName = b.Venue.VenueName,
                    VenueLocation = b.Venue.VenueLocation,
                    EventTypeName = b.Event.EventType.EventTypeName
                




                }).ToListAsync();


            return View(bookingViewModels);
        }



        public async Task<IActionResult> Search(int? eventTypeID, int? VenueID, DateOnly? filterStartDate, DateOnly? filterEndDate)
        {
            var venuesQuery = _context.Venue.AsQueryable();

            // Filter by event type (only venues that hosted that event type before, optional)
            if (eventTypeID.HasValue)
            {
                venuesQuery = venuesQuery.Where(v => _context.Event.Any(e => e.EventTypeID == eventTypeID && e.VenueID == v.VenueID));
            }

            // Filter by specific venue (optional)
            if (VenueID.HasValue)
            {
                venuesQuery = venuesQuery.Where(v => v.VenueID == VenueID);
                 //venuesQuery = _context.Venue.AsQueryable();
            }

            // Filter by availability (no overlap with booked events)
            if (filterStartDate.HasValue && filterEndDate.HasValue)
            {
                venuesQuery = venuesQuery.Where(v => !_context.Event.Any(e =>
                    e.VenueID == v.VenueID &&
                    (
                        (filterStartDate >= e.StartEventDate && filterStartDate < e.EndEventDate) ||
                        (filterEndDate > e.StartEventDate && filterEndDate <= e.EndEventDate) ||
                        (filterStartDate <= e.StartEventDate && filterEndDate >= e.EndEventDate)
                    )));

                if (filterStartDate > filterEndDate)
                {
                    ModelState.AddModelError("", "Start date must be before end date.");
                }
            }

            var viewModel = new BookingViewModel
            {
                EventTypeID = eventTypeID,
                VenueID = VenueID,
                FilterStartDate = filterStartDate ?? default,
                FilterEndDate = filterEndDate ?? default,
                AvailableVenues = await venuesQuery.ToListAsync(),
                EventTypes = new SelectList(_context.EventType.ToList(), "EventTypeID", "EventTypeName"),
                Venues = new SelectList(_context.Venue.ToList(), "VenueID", "VenueName")
            };

            return View(viewModel);
        }



    }
}
