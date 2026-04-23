using System.Collections.ObjectModel;

namespace ExpenseTracker.Models;

public class ProjectItem
{
    public string Id { get; set; } = "";
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
    public string Status { get; set; } = "";
    public int ArrayIndex { get; set; } = 0;  // Track position in projects array
    public ObservableCollection<ExpenseInput> Expenses { get; set; } = new();  // Store expenses for this project
}
