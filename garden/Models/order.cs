public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public int TotalPrice { get; set; }
    public string Status { get; set; } = "";
    public DateTime Date { get; set; }
    public User? User { get; set; }
}