using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class BookingViewModel
    {

        public int BookingID { get; set; }
        ////public DateTime BookingDate { get; set; }


        //// Event details 
        public string? EventName { get; set; }
      
        public string? EventDescription { get; set; }
        public DateOnly StartEventDate { get; set; }
        public DateOnly EndEventDate { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }


        // Venue details
        public string? VenueName { get; set; }
        public string? VenueLocation { get; set; }

        public string? EventTypeName { get; set; }


        //////Event Type details
        ////public SelectList? EventTypeID { get; set; }
        ////public SelectList? EventTypeName { get; set; }


        //// Filtering inputs
        ////to store the selected event type category 
        //public int? EventTypeID { get; set; }  
        //// to store the selected venue 
        //public int? VenueID { get; set; }




        // Filtering inputs
        [Display(Name = "Event Type")]
        public int? EventTypeID { get; set; }

        [Display(Name = "Venue")]
        public int? VenueID { get; set; }

        [Display(Name = "Start Date")]
        public DateOnly? FilterStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateOnly? FilterEndDate { get; set; }

        // Dropdowns
        //select list:  displays a drop-down menu 
        public SelectList? EventTypes { get; set; }
        public SelectList? Venues { get; set; }

        // Output: available venues based on filter
        public List<Venue>? AvailableVenues { get; set; }

      
       
       
    }
}
