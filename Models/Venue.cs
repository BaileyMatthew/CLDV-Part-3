using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventBooking.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int Capacity { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Event Type")]
        public int EventTypeId { get; set; }

        [ForeignKey("EventTypeId")]
        public EventType? EventType { get; set; }

        [Display(Name = "Available?")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Available From")]
        [DataType(DataType.Date)]
        public DateTime? AvailableFromDate { get; set; }

        [Display(Name = "Available To")]
        [DataType(DataType.Date)]
        public DateTime? AvailableToDate { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; } 
    }
}
