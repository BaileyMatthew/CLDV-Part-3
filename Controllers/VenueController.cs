using Azure.Storage.Blobs;
using EventBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public VenueController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string searchString, int? eventTypeId, bool? isAvailable, DateTime? fromDate, DateTime? toDate)
        {
            ViewBag.EventTypes = new SelectList(_context.EventType, "Id", "Name");
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentEventType = eventTypeId;
            ViewBag.CurrentAvailability = isAvailable;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            var venues = _context.Venue.Include(v => v.EventType).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                venues = venues.Where(v => v.VenueName.Contains(searchString) || v.Location.Contains(searchString));

            if (eventTypeId.HasValue)
                venues = venues.Where(v => v.EventTypeId == eventTypeId.Value);

            if (isAvailable.HasValue)
                venues = venues.Where(v => v.IsAvailable == isAvailable.Value);

            if (fromDate.HasValue)
                venues = venues.Where(v => v.AvailableFromDate == null || v.AvailableFromDate <= fromDate);

            if (toDate.HasValue)
                venues = venues.Where(v => v.AvailableToDate == null || v.AvailableToDate >= toDate);

            return View(await venues.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["EventTypeId"] = new SelectList(_context.EventType, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string connectionString = _configuration.GetConnectionString("AzureBlobStorage");
                    string containerName = "venue-images";
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    var blobServiceClient = new BlobServiceClient(connectionString);
                    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    await containerClient.CreateIfNotExistsAsync();
                    var blobClient = containerClient.GetBlobClient(fileName);

                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, overwrite: true);
                    }

                    venue.ImageUrl = blobClient.Uri.ToString();
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventTypeId"] = new SelectList(_context.EventType, "Id", "Name", venue.EventTypeId);
            return View(venue);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.Include(v => v.EventType).FirstOrDefaultAsync(v => v.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();

            ViewData["EventTypeId"] = new SelectList(_context.EventType, "Id", "Name", venue.EventTypeId);
            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venue.Any(v => v.VenueId == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["EventTypeId"] = new SelectList(_context.EventType, "Id", "Name", venue.EventTypeId);
            return View(venue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venue.Include(v => v.EventType).FirstOrDefaultAsync(v => v.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue != null)
            {
                _context.Venue.Remove(venue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
