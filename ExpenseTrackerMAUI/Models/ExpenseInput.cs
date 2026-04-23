namespace ExpenseTracker.Models;

public class ExpenseInput
{
    public string ExpenseCode { get; set; } = "";
    public string Date { get; set; } = "";
    public string Amount { get; set; } = "";
    public string Currency { get; set; } = "";
    public string ExpenseType { get; set; } = "Travel";
    public string PaymentMethod { get; set; } = "Cash";
    public string Claimant { get; set; } = "";
    public string PaymentStatus { get; set; } = "Pending";
    public string Description { get; set; } = "";
    public string Location { get; set; } = "";
}
