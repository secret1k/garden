using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public CategoriesController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new
            {
                c.CategoryId,
                c.Name,
                c.Img,
                c.ParentCategoryId,
                Subcategories = c.Subcategories.Select(sc => new
                {
                    sc.CategoryId,
                    sc.Name,
                    sc.Img
                }).ToList()
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound("Category not found");
        }

        return category;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] Category category)
    {
        var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
        if (existingCategory == null)
        {
            return NotFound("Category not found");
        }

        existingCategory.Name = category.Name;
        existingCategory.Img = category.Img;
        existingCategory.ParentCategoryId = category.ParentCategoryId;

        await _context.SaveChangesAsync();

        return Ok(existingCategory);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound("Category not found");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok($"Category with id {id} was deleted");
    }

}