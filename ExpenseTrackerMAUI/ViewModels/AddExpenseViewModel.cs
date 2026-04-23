using ExpenseTracker.Models;
using ExpenseTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ExpenseTracker.ViewModels;

public class AddExpenseViewModel : INotifyPropertyChanged
{
    private readonly FirebaseService _firebaseService;

    private string _projectId = "";
    private int _projectArrayIndex = 0;
    private string _projectName = "";
    private string _expenseCode = "";
    private string _date = DateTime.Now.ToString("yyyy-MM-dd");
    private string _amount = "";
    private string _currency = "USD";
    private string _expenseType = "Travel";
    private string _paymentMethod = "Cash";
    private string _claimant = "";
    private string _paymentStatus = "Pending";
    private string _description = "";
    private string _location = "";
    private bool _isBusy = false;
    private string _statusMessage = "";

    public ObservableCollection<string> ExpenseTypes { get; } = new()
    {
        "Travel",
        "Equipment",
        "Materials",
        "Services",
        "Software/Licenses",
        "Labour costs",
        "Utilities",
        "Miscellaneous"
    };

    public ObservableCollection<string> PaymentMethods { get; } = new()
    {
        "Cash",
        "Credit Card",
        "Bank Transfer",
        "Cheque"
    };

    public ObservableCollection<string> PaymentStatuses { get; } = new()
    {
        "Paid",
        "Pending",
        "Reimbursed"
    };

    public string ProjectId
    {
        get => _projectId;
        set { _projectId = value; OnPropertyChanged(nameof(ProjectId)); }
    }

    public string ProjectName
    {
        get => _projectName;
        set { _projectName = value; OnPropertyChanged(nameof(ProjectName)); }
    }

    public string ExpenseCode
    {
        get => _expenseCode;
        set { _expenseCode = value; OnPropertyChanged(nameof(ExpenseCode)); }
    }

    public string Date
    {
        get => _date;
        set { _date = value; OnPropertyChanged(nameof(Date)); }
    }

    public string Amount
    {
        get => _amount;
        set { _amount = value; OnPropertyChanged(nameof(Amount)); }
    }

    public string Currency
    {
        get => _currency;
        set { _currency = value; OnPropertyChanged(nameof(Currency)); }
    }

    public string ExpenseType
    {
        get => _expenseType;
        set { _expenseType = value; OnPropertyChanged(nameof(ExpenseType)); }
    }

    public string PaymentMethod
    {
        get => _paymentMethod;
        set { _paymentMethod = value; OnPropertyChanged(nameof(PaymentMethod)); }
    }

    public string Claimant
    {
        get => _claimant;
        set { _claimant = value; OnPropertyChanged(nameof(Claimant)); }
    }

    public string PaymentStatus
    {
        get => _paymentStatus;
        set { _paymentStatus = value; OnPropertyChanged(nameof(PaymentStatus)); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(nameof(Description)); }
    }

    public string LocationProperty
    {
        get => _location;
        set { _location = value; OnPropertyChanged(nameof(LocationProperty)); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
    }

    public ICommand SaveExpenseCommand { get; }

    public AddExpenseViewModel(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
        SaveExpenseCommand = new Command(SaveExpense);
    }

    public void SetProject(ProjectItem project)
    {
        if (project != null)
        {
            ProjectId = project.Id;
            _projectArrayIndex = project.ArrayIndex;
            ProjectName = project.Name;
            System.Diagnostics.Debug.WriteLine($"Project set: {project.Name} (ID: {project.Id}, ArrayIndex: {project.ArrayIndex})");
        }
    }

    private async void SaveExpense()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(Amount))
        {
            StatusMessage = "Please enter an amount";
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Saving expense...";

            var expense = new ExpenseInput
            {
                ExpenseCode = ExpenseCode,
                Date = Date,
                Amount = Amount,
                Currency = Currency,
                ExpenseType = ExpenseType,
                PaymentMethod = PaymentMethod,
                Claimant = Claimant,
                PaymentStatus = PaymentStatus,
                Description = Description,
                Location = LocationProperty
            };

            await _firebaseService.AddExpenseToRealtimeDatabaseAsync(_projectArrayIndex.ToString(), expense);

            StatusMessage = "Expense saved successfully!";
            Debug.WriteLine("Expense saved to Firebase Realtime Database");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            Debug.WriteLine($"Error saving expense: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save expense: {ex.Message}", "OK");
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

