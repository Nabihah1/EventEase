using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Booking
    {


        [Key]

        public int BookingID { get; set; }



        [Display(Name = "Event ID")]
        [DataType(DataType.Text)]
        public int EventID { get; set; }



        [Display(Name = "Venue ID")]
        [DataType(DataType.Text)]
        public int VenueID { get; set; }



        [Display(Name = "Date of Booking")]
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime BookingDate { get; set; }



        //navigation properties 
        public Event? Event { get; set; }
        public Venue? Venue { get; set; }

    }
}
