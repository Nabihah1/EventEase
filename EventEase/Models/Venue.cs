using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]


        [Display(Name = "Venue ID")]
        [DataType(DataType.Text)]
        public int VenueID { get; set; }



        [Display(Name = "Name of Venue")]
        [DataType(DataType.Text)]
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string? VenueName { get; set; }



        [Display(Name = "Location of Venue")]
        [DataType(DataType.MultilineText)]
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string? VenueLocation { get; set; }



        [Display(Name = "Maximum Capacity")]
        [DataType(DataType.Text)]
        [Range(1, 1000)]
        [Required]
        public int VenueCapacity { get; set; }


        [Display(Name = "Image")]
        public string? VenueImage { get; set; }



        //navigation properties 
        public List<Booking>? Bookings { get; set; }


    }
}
