using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBooking.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        // Foreign key reference to Event
        [ForeignKey("Event")]
        public int EventId { get; set; }
        public Event? Event { get; set; }

        // Foreign key reference to Venue
        [ForeignKey("Venue")]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; }

        // Booking date
        public DateTime BookingDate { get; set; }
    }
}
