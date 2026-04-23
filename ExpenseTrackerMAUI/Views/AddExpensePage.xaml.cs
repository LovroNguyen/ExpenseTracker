using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Views;

public partial class AddExpensePage : ContentPage
{
    private readonly AddExpenseViewModel _viewModel;

    public AddExpensePage(AddExpenseViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Get the project from app state
        if (AppState.CurrentProject != null)
        {
            _viewModel.SetProject(AppState.CurrentProject);
        }
    }
}
