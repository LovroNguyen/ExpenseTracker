using ExpenseTracker.Models;

namespace ExpenseTracker;

public static class AppState
{
    public static ProjectItem? CurrentProject { get; set; }
}
