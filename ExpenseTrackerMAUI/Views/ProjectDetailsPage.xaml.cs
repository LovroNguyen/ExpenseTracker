using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Views;

public partial class ProjectDetailsPage : ContentPage
{
    private readonly ProjectDetailsViewModel _viewModel;

    public ProjectDetailsPage(ProjectDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Get the project from app state and set it in the view model
        if (AppState.CurrentProject != null)
        {
            _viewModel.SetProject(AppState.CurrentProject);
            await _viewModel.LoadExpensesAsync();
        }
    }
}
