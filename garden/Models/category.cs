public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = "";
    public string Img { get; set; } = "";
    public int? ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }
    public List<Category> Subcategories { get; set; } = new();
}
