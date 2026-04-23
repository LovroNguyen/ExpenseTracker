using ExpenseTracker.Models;
using ExpenseTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ExpenseTracker.ViewModels;

public class ProjectDetailsViewModel : INotifyPropertyChanged
{
    private readonly FirebaseService _firebaseService;
    private ProjectItem? _currentProject;

    private string _projectName = "";
    private string _projectCode = "";
    private string _projectStartDate = "";
    private string _projectStatus = "";

    public ObservableCollection<ExpenseInput> Expenses { get; } = new();

    public string ProjectName
    {
        get => _projectName;
        set { _projectName = value; OnPropertyChanged(nameof(ProjectName)); }
    }

    public string ProjectCode
    {
        get => _projectCode;
        set { _projectCode = value; OnPropertyChanged(nameof(ProjectCode)); }
    }

    public string ProjectStartDate
    {
        get => _projectStartDate;
        set { _projectStartDate = value; OnPropertyChanged(nameof(ProjectStartDate)); }
    }

    public string ProjectStatus
    {
        get => _projectStatus;
        set { _projectStatus = value; OnPropertyChanged(nameof(ProjectStatus)); }
    }

    public ICommand AddExpenseCommand { get; }

    public ProjectDetailsViewModel(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
        AddExpenseCommand = new Command(OpenAddExpense);
    }

    public void SetProject(ProjectItem project)
    {
        if (project != null)
        {
            _currentProject = project;
            ProjectName = project.Name;
            ProjectCode = project.Code;
            ProjectStartDate = project.StartDate;
            ProjectStatus = project.Status;
            Debug.WriteLine($"📦 ProjectDetailsViewModel: Project set to {project.Name} (ArrayIndex: {project.ArrayIndex})");
        }
    }

    public async Task LoadExpensesAsync()
    {
        try
        {
            if (_currentProject == null)
            {
                Debug.WriteLine("❌ LoadExpensesAsync: project is NULL");
                return;
            }

            Debug.WriteLine($"🔄 Loading expenses for project: {_currentProject.Name}");
            Expenses.Clear();

            var expenses = await _firebaseService.GetExpensesFromRealtimeDatabaseAsync(_currentProject.ArrayIndex.ToString());

            Debug.WriteLine($"✅ Retrieved {expenses.Count} expenses");

            foreach (var expense in expenses)
            {
                Expenses.Add(expense);
                Debug.WriteLine($"  ➕ Added expense: {expense.ExpenseCode} - Amount: {expense.Amount}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error loading expenses: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private async void OpenAddExpense()
    {
        if (_currentProject == null)
        {
            Debug.WriteLine("❌ OpenAddExpense: project is NULL");
            return;
        }

        try
        {
            Debug.WriteLine($"🚀 Opening Add Expense for project: {_currentProject.Name}");

            // Store the project in app state for AddExpensePage
            AppState.CurrentProject = _currentProject;
            Debug.WriteLine($"✅ AppState.CurrentProject set to: {AppState.CurrentProject?.Name}");

            Debug.WriteLine("🔄 Navigating to AddExpensePage...");
            await Shell.Current.GoToAsync("///AddExpensePage");
            Debug.WriteLine("✅ Navigation complete");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
