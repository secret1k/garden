public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public int TotalPrice { get; set; }
    public string Status { get; set; } = "";
    public string Date { get; set; } = "";
}