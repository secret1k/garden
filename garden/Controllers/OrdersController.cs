using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationContext _context;

    public OrdersController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .ToListAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
        var existingProducts = await _context.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToListAsync();
        if (existingProducts.Count != productIds.Count)
            return BadRequest("some products do not exist");
        if (order.TotalPrice == 0)
        {
            order.TotalPrice = order.Items.Sum(i => i.Quantity * i.Price);
        }
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
    }

    [HttpPost("{id}/items")]
    public async Task<IActionResult> AddOrderItem(int id, OrderItem orderItem)
    {
        var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();

        var product = await _context.Products.FindAsync(orderItem.ProductId);
        if (product == null)
            return BadRequest("product does not exist");

        order.Items.Add(orderItem);
        order.TotalPrice += orderItem.Quantity * orderItem.Price;

        await _context.SaveChangesAsync();

        return Ok(order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, Order order)
    {
        if (id != order.OrderId)
            return BadRequest("badrequest");

        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Orders.AnyAsync(o => o.OrderId == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpPut("{orderId}/items/{itemId}")]
    public async Task<IActionResult> UpdateOrderItem(int orderId, int itemId, OrderItem updatedItem)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return NotFound();

        var item = order.Items.FirstOrDefault(i => i.OrderItemId == itemId);
        if (item == null)
            return NotFound();

        item.Quantity = updatedItem.Quantity;
        item.Price = updatedItem.Price;

        order.TotalPrice = order.Items.Sum(i => i.Quantity * i.Price);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            return NotFound();

        _context.OrderItems.RemoveRange(order.Items);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{orderId}/items/{itemId}")]
    public async Task<IActionResult> DeleteOrderItem(int orderId, int itemId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            return NotFound();

        var item = order.Items.FirstOrDefault(i => i.OrderItemId == itemId);
        if (item == null)
            return NotFound();

        order.Items.Remove(item);
        order.TotalPrice -= item.Quantity * item.Price;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}