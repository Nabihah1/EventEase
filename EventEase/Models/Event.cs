﻿using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        [Display(Name = "Event ID")]
        [DataType(DataType.Text)]
        public int EventID { get; set; }

        //foreign key
        [Required]
        //[Display(Name = "Venue ID")]
        //[DataType(DataType.Text)]
        public int? VenueID { get; set; }



        [Display(Name = "Name of Event")]
        [DataType(DataType.Text)]
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string? EventName { get; set; }



        [Display(Name = "Start Date of Event")]
        [DataType(DataType.Date)]
        [Required]
        public DateOnly StartEventDate { get; set; }


        [Display(Name = "End Date of Event")]
        [DataType(DataType.Date)]
        [Required]
        public DateOnly EndEventDate { get; set; }




        [Display(Name = "Start of Event")]
        [DataType(DataType.Time)]
        [Required]
        public TimeOnly StartTime { get; set; }



        [Display(Name = "End of Event")]
        [DataType(DataType.Time)]
        [Required]
        public TimeOnly EndTime { get; set; }



        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string? EventDescription { get; set; }


        [Display(Name = "Image")]
        public string? EventImage { get; set; }

        //add foreign key for event type 
        [Display(Name = "Event Type ID")]
        [DataType(DataType.Text)]
        public int EventTypeID { get; set; }

       
        //navigation properties
        public List<Booking>? Bookings { get; set; }
        public Venue? Venue { get; set; }
        public EventType? EventType { get; set; }
    }
}
