using ExpenseTracker.Models;
using ExpenseTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ExpenseTracker.ViewModels;

public partial class ProjectsViewModel : BindableObject
{
    private readonly FirebaseService _firebaseService;
    private string _searchText = "";
    private string _debugInfo = "";

    public ObservableCollection<ProjectItem> Projects { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set 
        { 
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            OnPropertyChanged(nameof(FilteredProjects));
        }
    }

    public string DebugInfo
    {
        get => _debugInfo;
        set
        {
            _debugInfo = value;
            OnPropertyChanged(nameof(DebugInfo));
        }
    }

    public IEnumerable<ProjectItem> FilteredProjects
    {
        get => string.IsNullOrWhiteSpace(SearchText)
            ? Projects
            : Projects.Where(p => p.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
    }

    public ICommand LoadCommand { get; }
    public ICommand AddExpenseCommand { get; }

    public ProjectsViewModel(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
        LoadCommand = new Command(LoadProjects);
        AddExpenseCommand = new Command<ProjectItem>(OpenAddExpense);
    }

    private async void LoadProjects()
    {
        await LoadAsync();
    }

    private async void OpenAddExpense(ProjectItem project)
    {
        if (project == null) 
        {
            Debug.WriteLine("❌ OpenAddExpense: project is NULL");
            return;
        }

        try
        {
            Debug.WriteLine($"🚀 OpenAddExpense called for project: {project.Name}");

            // Store the project in a static property for easy access
            AppState.CurrentProject = project;
            Debug.WriteLine($"✅ AppState.CurrentProject set to: {AppState.CurrentProject?.Name}");

            Debug.WriteLine("🔄 Navigating to AddExpensePage...");
            await Shell.Current.GoToAsync("///AddExpensePage");
            Debug.WriteLine("✅ Navigation complete");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error navigating to Add Expense: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    public async Task LoadAsync()
    {
        try
        {
            DebugInfo = "Loading projects from Firebase...";
            Projects.Clear();
            Debug.WriteLine("Starting to load projects...");

            var projects = await _firebaseService.GetProjectsFromRealtimeDatabaseAsync();

            DebugInfo = $"Loaded {projects.Count} projects";
            Debug.WriteLine($"Projects loaded: {projects.Count}");

            foreach (var project in projects)
            {
                Projects.Add(project);
                Debug.WriteLine($"  ➕ Added project: {project.Name} (ID: {project.Id})");
            }

            if (projects.Count == 0)
            {
                DebugInfo = "No projects found. Check Firebase connection.";
            }
        }
        catch (Exception ex)
        {
            DebugInfo = $"Error: {ex.Message}";
            Debug.WriteLine($"Error loading projects: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
