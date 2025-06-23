using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBooking.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        public DateTime EventDate { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Venue selection is required")]
        [ForeignKey("Venue")]
        public int VenueId { get; set; }

        public Venue? Venue { get; set; }
    }
}
