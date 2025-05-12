namespace EventEase.Models
{
    public class BookingViewModel
    {

        public int BookingID { get; set; }
        public DateTime BookingDate { get; set; }


        // Event details
        public string? EventName { get; set; }
        public string? EventDescription { get; set; }
        public DateOnly EventDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }


        // Venue details
        public string? VenueName { get; set; }
        public string? VenueLocation { get; set; }

        


    }
}
