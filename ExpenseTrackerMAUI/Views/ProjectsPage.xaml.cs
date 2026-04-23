using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;
using System.Diagnostics;

namespace ExpenseTracker.Views;

public partial class ProjectsPage : ContentPage
{
    private readonly ProjectsViewModel _viewModel;

    public ProjectsPage(ProjectsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnViewProjectDetails(object sender, EventArgs e)
    {
        Debug.WriteLine("🔍 View Details button clicked");

        var button = (Button)sender;
        var project = (ProjectItem)button.CommandParameter;

        if (project == null)
        {
            Debug.WriteLine("❌ Project is NULL!");
            return;
        }

        Debug.WriteLine($"📦 Navigating to details for project: {project.Name}");

        // Store the project in app state
        AppState.CurrentProject = project;

        // Navigate to ProjectDetailsPage
        await Shell.Current.GoToAsync("///ProjectDetailsPage");
    }
}

