using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly ApplicationContext _context;

    public OrderController(ApplicationContext context)
    {
        _context = context;
    }

    //[HttpGet]
    //public async Task<IActionResult> GetOrders()
    //{
    //    var orders = await _context.Orders
    //        .Include(o => o.User)
    //        .ToListAsync();
    //    return Ok(orders);
    //}

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetOrder(int id)
    //{
    //    var order = await _context.Orders
    //        .Include(o => o.User)
    //        .FirstOrDefaultAsync(o => o.OrderId == id);

    //    if (order == null)
    //        return NotFound();

    //    return Ok(order);
    //}

    //[HttpPost]
    //public async Task<IActionResult> CreateOrder([FromBody] Order order)
    //{
    //    _context.Orders.Add(order);
    //    await _context.SaveChangesAsync();
    //    return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
    //}

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
    {
        if (id != order.OrderId)
            return BadRequest();

        _context.Entry(order).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}