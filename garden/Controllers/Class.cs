using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationContext _context;

    public ReviewsController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.User)
            .ToListAsync();
        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.Product)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.ReviewId == id);

        if (review == null)
            return NotFound();

        return Ok(review);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview(Review review)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _context.Products.AnyAsync(p => p.ProductId == review.ProductId) ||
            !await _context.Users.AnyAsync(u => u.UserId == review.UserId))
        {
            return BadRequest("wrong ProductId or UserId");
        }

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, Review review)
    {
        if (id != review.ReviewId)
            return BadRequest("badrequest");

        if (!await _context.Reviews.AnyAsync(r => r.ReviewId == id))
            return NotFound();

        _context.Entry(review).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Reviews.AnyAsync(r => r.ReviewId == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound();

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
