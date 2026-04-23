using System.Text;
using System.Text.Json;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public class FirebaseService
{
    private readonly HttpClient _httpClient;
    private readonly FirebaseConfig _config;

    public FirebaseService(HttpClient httpClient, FirebaseConfig config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_config.ProjectId) && !string.IsNullOrWhiteSpace(_config.WebApiKey);

    public async Task<List<ProjectItem>> GetProjectsAsync()
    {
        if (!IsConfigured) return new List<ProjectItem>();
        var url = $"https://firestore.googleapis.com/v1/projects/{_config.ProjectId}/databases/(default)/documents/projects?key={_config.WebApiKey}";
        var json = await _httpClient.GetStringAsync(url);
        using var doc = JsonDocument.Parse(json);
        var list = new List<ProjectItem>();
        if (!doc.RootElement.TryGetProperty("documents", out var documents)) return list;

        foreach (var d in documents.EnumerateArray())
        {
            var fields = d.GetProperty("fields");
            list.Add(new ProjectItem
            {
                Id = GetString(fields, "id"),
                Code = GetString(fields, "code"),
                Name = GetString(fields, "name"),
                Description = GetString(fields, "description"),
                StartDate = GetString(fields, "startDate"),
                EndDate = GetString(fields, "endDate"),
                Status = GetString(fields, "status")
            });
        }

        return list;
    }

    public async Task<List<ExpenseInput>> GetFavoritesAsync()
    {
        if (!IsConfigured) return new List<ExpenseInput>();
        var url = $"https://firestore.googleapis.com/v1/projects/{_config.ProjectId}/databases/(default)/documents/favorites?key={_config.WebApiKey}";
        try
        {
            var json = await _httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);
            var list = new List<ExpenseInput>();
            if (!doc.RootElement.TryGetProperty("documents", out var documents)) return list;

            foreach (var d in documents.EnumerateArray())
            {
                var fields = d.GetProperty("fields");
                list.Add(new ExpenseInput
                {
                    ExpenseCode = GetString(fields, "expenseCode"),
                    Date = GetString(fields, "date"),
                    Amount = GetString(fields, "amount"),
                    Currency = GetString(fields, "currency"),
                    ExpenseType = GetString(fields, "expenseType"),
                    PaymentMethod = GetString(fields, "paymentMethod"),
                    Claimant = GetString(fields, "claimant"),
                    PaymentStatus = GetString(fields, "paymentStatus"),
                    Description = GetString(fields, "description"),
                    Location = GetString(fields, "location")
                });
            }
            return list;
        }
        catch
        {
            return new List<ExpenseInput>();
        }
    }

    public async Task<JsonElement> GetRealtimeDatabaseAsync(string path = "mobileSync/latest.json")
    {
        try
        {
            var url = $"https://expense-tracker-3263c-default-rtdb.asia-southeast1.firebasedatabase.app/{path}";
            var json = await _httpClient.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching realtime database: {ex.Message}");
            using var emptyDoc = JsonDocument.Parse("{}");
            return emptyDoc.RootElement.Clone();
        }
    }

    public async Task<List<ProjectItem>> GetProjectsFromRealtimeDatabaseAsync()
    {
        try
        {
            var data = await GetRealtimeDatabaseAsync("mobileSync/latest.json");
            var projects = new List<ProjectItem>();

            if (data.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                // Try to get the "projects" array
                if (data.TryGetProperty("projects", out var projectsArray) && 
                    projectsArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    int arrayIndex = 0;
                    foreach (var projectElement in projectsArray.EnumerateArray())
                    {
                        var project = new ProjectItem
                        {
                            Id = GetJsonString(projectElement, "id"),
                            Code = GetJsonString(projectElement, "code"),
                            Name = GetJsonString(projectElement, "name"),
                            Description = GetJsonString(projectElement, "description"),
                            StartDate = GetJsonString(projectElement, "startDate"),
                            EndDate = GetJsonString(projectElement, "endDate"),
                            Status = GetJsonString(projectElement, "status"),
                            ArrayIndex = arrayIndex  // Set the array index
                        };
                        projects.Add(project);
                        System.Diagnostics.Debug.WriteLine($"Parsed project: {project.Name} at index {arrayIndex}");
                        arrayIndex++;
                    }
                }
                else
                {
                    // Fallback: Try as an object with keyed projects
                    int arrayIndex = 0;
                    foreach (var projectProp in data.EnumerateObject())
                    {
                        var project = new ProjectItem
                        {
                            Id = GetJsonString(projectProp.Value, "id"),
                            Code = GetJsonString(projectProp.Value, "code"),
                            Name = GetJsonString(projectProp.Value, "name"),
                            Description = GetJsonString(projectProp.Value, "description"),
                            StartDate = GetJsonString(projectProp.Value, "startDate"),
                            EndDate = GetJsonString(projectProp.Value, "endDate"),
                            Status = GetJsonString(projectProp.Value, "status"),
                            ArrayIndex = arrayIndex  // Set the array index
                        };
                        projects.Add(project);
                        arrayIndex++;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Total projects retrieved: {projects.Count}");
            return projects;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching projects from realtime database: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return new List<ProjectItem>();
        }
    }

    public async Task<List<ExpenseInput>> GetExpensesFromRealtimeDatabaseAsync(string projectIndex = "0")
    {
        try
        {
            var data = await GetRealtimeDatabaseAsync("mobileSync/latest.json");
            var expenses = new List<ExpenseInput>();

            if (data.ValueKind == System.Text.Json.JsonValueKind.Object &&
                data.TryGetProperty("projects", out var projectsArray) &&
                projectsArray.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                var projectIdx = int.Parse(projectIndex);
                var projectsEnumerator = projectsArray.EnumerateArray().ElementAtOrDefault(projectIdx);

                if (projectsEnumerator.ValueKind == System.Text.Json.JsonValueKind.Object &&
                    projectsEnumerator.TryGetProperty("expenses", out var expensesArray) &&
                    expensesArray.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var expenseElement in expensesArray.EnumerateArray())
                    {
                        var expense = new ExpenseInput
                        {
                            ExpenseCode = GetJsonString(expenseElement, "expenseCode"),
                            Date = GetJsonString(expenseElement, "date"),
                            Amount = GetJsonString(expenseElement, "amount"),
                            Currency = GetJsonString(expenseElement, "currency"),
                            ExpenseType = GetJsonString(expenseElement, "expenseType"),
                            PaymentMethod = GetJsonString(expenseElement, "paymentMethod"),
                            Claimant = GetJsonString(expenseElement, "claimant"),
                            PaymentStatus = GetJsonString(expenseElement, "paymentStatus"),
                            Description = GetJsonString(expenseElement, "description"),
                            Location = GetJsonString(expenseElement, "location")
                        };
                        expenses.Add(expense);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Expenses retrieved for project {projectIndex}: {expenses.Count}");
            return expenses;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching expenses from realtime database: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return new List<ExpenseInput>();
        }
    }

    public async Task AddExpenseToRealtimeDatabaseAsync(string projectId, ExpenseInput expense)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Starting to add expense to project {projectId}");

            // Construct the URL to add to the expenses array
            var url = "https://expense-tracker-3263c-default-rtdb.asia-southeast1.firebasedatabase.app/mobileSync/latest/projects/" + projectId + "/expenses.json";
            System.Diagnostics.Debug.WriteLine($"Target URL: {url}");

            // Get current expenses
            string currentExpensesJson = "[]";
            try
            {
                currentExpensesJson = await _httpClient.GetStringAsync(url);
                System.Diagnostics.Debug.WriteLine($"Current expenses count: {currentExpensesJson.Length} bytes");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Could not get current expenses (may be first one): {ex.Message}");
            }

            var currentExpensesArray = JsonDocument.Parse(currentExpensesJson).RootElement;
            var updatedExpenses = new List<object>();

            if (currentExpensesArray.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                try
                {
                    // Convert current expenses to a list of dynamic objects
                    foreach (var item in currentExpensesArray.EnumerateArray())
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in item.EnumerateObject())
                        {
                            dict[prop.Name] = prop.Value.ValueKind switch
                            {
                                System.Text.Json.JsonValueKind.String => prop.Value.GetString(),
                                System.Text.Json.JsonValueKind.Number => prop.Value.GetDouble(),
                                _ => prop.Value.ToString()
                            };
                        }
                        updatedExpenses.Add(dict);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error parsing existing expenses: {ex.Message}");
                }
            }

            // Create new expense object
            var newExpense = new
            {
                amount = int.TryParse(expense.Amount, out var amt) ? amt : 0,
                claimant = expense.Claimant ?? "",
                currency = expense.Currency ?? "USD",
                date = expense.Date ?? DateTime.Now.ToString("yyyy-MM-dd"),
                description = expense.Description ?? "",
                expenseCode = expense.ExpenseCode ?? "",
                expenseType = expense.ExpenseType ?? "",
                id = updatedExpenses.Count + 1,
                location = expense.Location ?? "",
                paymentMethod = expense.PaymentMethod ?? "Cash",
                paymentStatus = expense.PaymentStatus ?? "Pending"
            };

            updatedExpenses.Add(newExpense);
            System.Diagnostics.Debug.WriteLine($"New expense object created with ID: {updatedExpenses.Count}");

            // Upload updated expenses
            var jsonContent = JsonSerializer.Serialize(updatedExpenses, new JsonSerializerOptions { WriteIndented = true });
            System.Diagnostics.Debug.WriteLine($"Uploading {updatedExpenses.Count} expenses...");

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);

            System.Diagnostics.Debug.WriteLine($"Firebase response status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Firebase error response: {errorContent}");
                throw new Exception($"Firebase error: {response.StatusCode} - {errorContent}");
            }

            System.Diagnostics.Debug.WriteLine($"✅ Expense added successfully to project {projectId}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error adding expense to realtime database: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task AddExpenseToProjectAsync(string projectId, ExpenseInput expense)
    {
        if (!IsConfigured) return;
        var getUrl = $"https://firestore.googleapis.com/v1/projects/{_config.ProjectId}/databases/(default)/documents/projects/{projectId}?key={_config.WebApiKey}";
        var current = await _httpClient.GetStringAsync(getUrl);
        using var currentDoc = JsonDocument.Parse(current);

        var existing = new List<Dictionary<string, string>>();
        if (TryGetExpenseArray(currentDoc.RootElement, out var parsed))
        {
            existing = parsed;
        }
        existing.Add(new Dictionary<string, string>
        {
            ["expenseCode"] = expense.ExpenseCode,
            ["date"] = expense.Date,
            ["amount"] = expense.Amount,
            ["currency"] = expense.Currency,
            ["expenseType"] = expense.ExpenseType,
            ["paymentMethod"] = expense.PaymentMethod,
            ["claimant"] = expense.Claimant,
            ["paymentStatus"] = expense.PaymentStatus,
            ["description"] = expense.Description,
            ["location"] = expense.Location
        });

        var patchBody = BuildExpensePatch(existing);
        var patchUrl = $"https://firestore.googleapis.com/v1/projects/{_config.ProjectId}/databases/(default)/documents/projects/{projectId}?key={_config.WebApiKey}&updateMask.fieldPaths=expenses";
        using var content = new StringContent(patchBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync(patchUrl, content);
        response.EnsureSuccessStatusCode();
    }

    private static string GetString(JsonElement fields, string key)
    {
        if (!fields.TryGetProperty(key, out var property)) return "";
        if (property.TryGetProperty("stringValue", out var sv)) return sv.GetString() ?? "";
        if (property.TryGetProperty("doubleValue", out var dv)) return dv.GetDouble().ToString("0.##");
        return "";
    }

    private static string GetJsonString(JsonElement element, string key)
    {
        if (!element.TryGetProperty(key, out var property)) return "";

        return property.ValueKind switch
        {
            System.Text.Json.JsonValueKind.String => property.GetString() ?? "",
            System.Text.Json.JsonValueKind.Number => property.GetDouble().ToString("0.##"),
            _ => ""
        };
    }

    private static bool TryGetExpenseArray(JsonElement root, out List<Dictionary<string, string>> list)
    {
        list = new List<Dictionary<string, string>>();
        if (!root.TryGetProperty("fields", out var fields)) return false;
        if (!fields.TryGetProperty("expenses", out var expenses)) return false;
        if (!expenses.TryGetProperty("arrayValue", out var arrayValue)) return false;
        if (!arrayValue.TryGetProperty("values", out var values)) return false;

        foreach (var value in values.EnumerateArray())
        {
            if (!value.TryGetProperty("mapValue", out var mv)) continue;
            if (!mv.TryGetProperty("fields", out var ef)) continue;
            var item = new Dictionary<string, string>();
            foreach (var f in ef.EnumerateObject())
            {
                if (f.Value.TryGetProperty("stringValue", out var sv)) item[f.Name] = sv.GetString() ?? "";
                if (f.Value.TryGetProperty("doubleValue", out var dv)) item[f.Name] = dv.GetDouble().ToString("0.##");
            }
            list.Add(item);
        }
        return true;
    }

    public async Task<string> TestFirebaseConnectionAsync()
    {
        try
        {
            var url = "https://expense-tracker-3263c-default-rtdb.asia-southeast1.firebasedatabase.app/mobileSync/latest.json";
            System.Diagnostics.Debug.WriteLine($"Testing connection to: {url}");

            var response = await _httpClient.GetAsync(url);
            System.Diagnostics.Debug.WriteLine($"HTTP Status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Raw JSON response length: {json.Length}");
                System.Diagnostics.Debug.WriteLine($"Raw JSON: {json.Substring(0, Math.Min(500, json.Length))}");
                return json;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error response: {error}");
                return $"Error: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Connection test failed: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return $"Exception: {ex.Message}";
        }
    }

    private static string BuildExpensePatch(List<Dictionary<string, string>> expenses)
    {
        var values = expenses.Select(e => new
        {
            mapValue = new
            {
                fields = e.ToDictionary(
                    kv => kv.Key,
                    kv => (object)new Dictionary<string, string> { ["stringValue"] = kv.Value })
            }
        });

        var payload = new
        {
            fields = new
            {
                expenses = new
                {
                    arrayValue = new
                    {
                        values = values
                    }
                }
            }
        };
        return JsonSerializer.Serialize(payload);
    }
}
