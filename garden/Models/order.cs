using System.Text.Json.Serialization;

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public int TotalPrice { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public User? User { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}