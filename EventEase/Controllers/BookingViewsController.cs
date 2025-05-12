using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
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
                    BookingDate = b.BookingDate,
                    EventName = b.Event.EventName,
                    EventDescription = b.Event.EventDescription,
                    EventDate = b.Event.EventDate,
                    StartTime = b.Event.StartTime,
                    EndTime = b.Event.EndTime,
                    VenueName = b.Venue.VenueName,
                    VenueLocation = b.Venue.VenueLocation,


                }).ToListAsync();


            return View(bookingViewModels);
        }
    }
}
