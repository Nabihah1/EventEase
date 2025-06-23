using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventType
    {
        [Display(Name = "Event Type ID")]
        [DataType(DataType.Text)]
        public int EventTypeID { get; set; }


        [Display(Name = "Type of Event")]
        [DataType(DataType.Text)]
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string? EventTypeName { get; set; }


        //navigation property 
        //the event type table will link to the event table 
        public List <Event>? Events { get; set; }


    }
}
