using System.ComponentModel.DataAnnotations;

namespace EventBooking.Models
{
    public class EventType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Event Type")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
