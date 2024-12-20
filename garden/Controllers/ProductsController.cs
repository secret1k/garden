using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationContext _context;

    public ProductsController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Select(p => new
            {
                p.ProductId,
                p.Name,
                p.Description,
                p.Price,
                p.Img,
                p.CategoryId,
                Category = new
                {
                    p.Category.CategoryId,
                    p.Category.Name,
                    p.Category.Img
                }
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound("product not found");
        }

        return product;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(product);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.ProductId);
        if (existingProduct == null)
        {
            return NotFound("Product not found");
        }

        existingProduct.Name = product.Name;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Img = product.Img;

        await _context.SaveChangesAsync();

        return Ok(existingProduct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound("Product not found");
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok($"Product with id {id} was deleted");
    }

}