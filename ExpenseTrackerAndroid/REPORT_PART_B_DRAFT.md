# Part B Report Draft (Android + MAUI)

Student: [Your Name]
Module: [Module Code]
Date: [Submission Date]

This draft is based on the current implementation in both apps:
- Android native app (ExpenseTrackerCW)
- .NET MAUI cross-platform app (ExpenseTracker)

## Section 1 - Feature Checklist (Concise)

Important note: This checklist requires you to honestly evaluate your app against the coursework specification. The statuses below reflect our current code state.

| Feature | Status | Your Comments |
|---|---|---|
| **Functionality A**<br>Enter details of a project and overall design of the app (10%) | Fully completed ☑<br>Partially completed ☐<br>Having bugs/Not working ☐<br>Not implemented ☐ | The Android app provides a comprehensive form for project creation. All required fields are validated before submission. |
| **Functionality B**<br>Store, view and delete project details or reset the database (10%) | Fully completed ☑<br>Partially completed ☐<br>Having bugs/Not working ☐<br>Not implemented ☐ | SQLite database stores all projects. The user can view a list of projects, edit or delete them, and reset the entire database. |
| **Functionality C**<br>Manage expense (15%) | Fully completed ☑<br>Partially completed ☐<br>Having bugs/Not working ☐<br>Not implemented ☐ | Expenses can be added to specific projects with all required category dropdowns and text fields. |
| **Functionality D**<br>Search (10%) | Fully completed ☑<br>Partially completed ☐<br>Having bugs/Not working ☐<br>Not implemented ☐ | Advanced search is implemented to find matching projects based on name, description, date, status, and owner. |
| **Functionality E**<br>Upload details to a cloud-based web service (15%) | Fully completed ☑<br>Partially completed ☐<br>Having bugs/Not working ☐<br>Not implemented ☐ | The Android app checks for a network connection and uploads local SQLite data to Firebase Realtime Database. |
| **Functionality F**<br>Add additional features to the Android app (5%) | Fully completed ☐<br>Partially completed ☐<br>Having bugs/Not working ☐<br>Not implemented ☑ | No extra features (such as camera or location) were implemented in the Android app. |
| **Functionality G**<br>Create a hybrid app using .NET MAUI (10%) | Fully completed ☐<br>Partially completed ☑<br>Having bugs/Not working ☐<br>Not implemented ☐ | The MAUI app reads data from Firebase, displays projects, and can filter by name. Adding expenses to the cloud works. |
| **Functionality H**<br>Implement setting favourite projects in hybrid app (5%) | Fully completed ☐<br>Partially completed ☐<br>Having bugs/Not working ☐<br>Not implemented ☑ | Favourite projects functionality is not fully implemented in the MAUI app UI yet. |

## Section 2 - Reflection on Development (approx. 350 words)

Developing the two versions of the app was a useful comparison between platform-specific and cross-platform development approaches. The Android app was built in Java using Activities, XML layouts, and a local SQLite database. This gave strong control over data persistence and user flow because each screen had a direct lifecycle and predictable behavior. The MAUI app used XAML with ViewModels and service classes, which encouraged cleaner separation of concerns and made UI-binding faster to build once the project structure was stable.

One major lesson learned was that architecture choices directly affect development speed later in the project. In Android, writing database schema and CRUD operations early made the rest of the implementation straightforward, especially search, edit, and delete features. In MAUI, creating ViewModels and navigation routes gave a good foundation, but because persistence depended on remote Firebase calls, debugging became harder when network/API behavior changed. This showed that a clean architecture is important, but reliability also depends on choosing stable data access patterns.

What went well was feature coverage in core workflows. Both apps support project and expense data entry with structured fields, and both support practical categorization of expenses. The Android app also delivered useful administrative features such as advanced filtering, reset database, and upload of all records to Firebase. The MAUI app achieved a consistent user interface with less repeated code and can target multiple platforms from one codebase, which is a strong outcome for cross-platform goals.

What could be improved is mainly in robustness and security. Some configuration values are hardcoded, and authentication is not yet integrated, which is risky for real deployment. Sync behavior is also limited because Android upload is one-way and MAUI has no offline caching strategy. If I repeated this project, I would plan security and synchronization from the first week, define a shared data contract between both apps, and include test scenarios for failure cases such as no network, invalid payloads, and partial writes.

I also learned that reporting and testing should be continuous rather than left until the end. Keeping short development notes after each implementation task made it easier to explain design decisions and justify trade-offs in this report.

## Section 3 - Evaluation of the Apps (700-1000 words)

This evaluation compares the Android native app and the MAUI cross-platform app against four key dimensions: human-computer interaction, security, ability to run across different screen sizes, and readiness for live deployment. The two apps solve similar business tasks, but they make different technical trade-offs.

From a human-computer interaction perspective, the Android app is practical and task-oriented. The main screen presents direct action buttons (add project, search, upload, reset), and project rows are readable because they combine code, name, status, and budget in one line. This reduces navigation depth for administrative users. Form screens provide clear required fields and basic validation, so the user is guided to complete mandatory data before saving. The use of spinners for controlled vocabularies (expense type, payment method, payment status) reduces typing errors and improves data consistency. Confirmation dialogs for destructive actions (delete, reset) are also good interaction choices because they reduce accidental loss.

However, there are usability limits in the Android app. The forms are long and rely on many text inputs in a vertical scroll, which can be tiring and error-prone on smaller devices. Date entry is free text instead of a date picker, so format mistakes are likely. Search is powerful, but all filter fields are optional text boxes rather than structured pickers for status and date ranges. This makes the feature flexible but less guided. The UI is functional rather than polished, with limited visual hierarchy and no progressive disclosure for advanced actions.

The MAUI app offers a cleaner and more modern structure in terms of code organization and cross-platform UI consistency. MVVM-style bindings reduce boilerplate and make it easier to reason about state changes in a single place. The project list filtering is responsive, and navigation to detail/add pages is straightforward. The Add Expense page uses pickers and entries similarly to Android, maintaining familiar interaction patterns. For users, this consistency is positive, especially if the same app is used across Android, iOS, and desktop.

On the other hand, MAUI currently implements fewer management operations from a user perspective. Core read/add paths are present, but edit and delete workflows are limited compared with Android. This means MAUI supports data capture but not full correction lifecycle. In real usage, users often need to fix mistakes after submission, so this gap affects usability and operational quality.

Security is currently the weakest area across both apps. In the current state, sensitive backend configuration values are present in client-side code, and there is no integrated authentication or role-based authorization. For a coursework prototype this may be acceptable, but for production this is a major risk. Without authenticated user identity, there is no guarantee that only authorized users can read or modify data. Android local storage is unencrypted SQLite by default, so device-level compromise could expose records. Network calls should also enforce stronger trust boundaries through token-based access and server-side validation.

A secure redesign would include several concrete improvements. First, move API keys and environment settings out of source code into secure build-time configuration, and use different keys for development and production. Second, enforce Firebase security rules so read/write operations require authenticated users and constrained data paths. Third, add input sanitization and field-length limits in both apps and backend rules. Fourth, encrypt sensitive local data on device where required, and store auth/session tokens in secure storage mechanisms rather than plain preferences.

Regarding ability to run across a range of screen sizes, the MAUI app has an architectural advantage because it targets multiple platforms from one project and uses adaptable layout containers with scrollable forms. This provides a baseline level of portability. The Android app also uses flexible widths and weights in key places, so it can run on many phone sizes. Nevertheless, both apps can improve adaptive behavior significantly. Neither implementation currently shows dedicated layouts for tablets, landscape optimization, or dynamic UI rearrangement for larger widths. Long forms remain single-column and could be improved by two-column grouping on tablets. Font scaling and accessibility settings should also be tested explicitly to prevent clipping.

For live deployment readiness, both apps need further engineering beyond feature completion. Operational concerns include release signing, versioning strategy, crash reporting, analytics, and CI/CD pipelines for repeatable builds. Error handling is currently user-visible but basic (toasts, status labels, debug output). Production systems require structured logging and clear recovery paths when API calls fail. Data synchronization design also needs maturity: Android currently performs bulk upload rather than conflict-aware incremental sync, and MAUI does not yet implement robust offline-first behavior. These gaps could lead to data conflicts or temporary data unavailability in poor network conditions.

Overall, the Android app is stronger in full local CRUD functionality and operational control, while the MAUI app is stronger in cross-platform code reuse and architectural separation. As a pair, they demonstrate good progress toward the coursework aims, but they are still prototype-level in security, resilience, and deployment hardening. The most valuable next improvements are: adding secure authentication and access rules, implementing edit/delete parity in MAUI, introducing offline caching plus conflict-aware sync, and improving adaptive UX for tablets and accessibility settings.

## Section 4 - Screenshots and Captions

Insert screenshots that demonstrate each implemented feature. Suggested structure:

1. Android home screen with project list and main actions.
   Caption: Project listing with add/search/upload/reset actions.
2. Android project form screen.
   Caption: Project creation with validation-ready required fields.
3. Android project detail screen with expense list.
   Caption: Project overview, expense navigation, edit/delete controls.
4. Android expense form screen.
   Caption: Expense entry using category/payment status controls.
5. Android search screen.
   Caption: Multi-criteria search by keyword/date/status/owner/code.
6. Android delete confirmation dialog.
   Caption: Safe destructive action flow for record deletion.
7. MAUI project list page.
   Caption: Project retrieval and name-based filtering in cross-platform UI.
8. MAUI project details page.
   Caption: Selected project metadata and expense list display.
9. MAUI add expense page.
   Caption: Expense submission form with picker-based controlled values.
10. MAUI success/error feedback state.
    Caption: Save feedback and navigation behavior after expense submission.

## Section 5 - Code Listing (Handwritten Files Only)

This section should show only handwritten source files that demonstrate architecture and implemented features.

Code listing policy (for both logbook and coursework):
- Use copy-paste from your real source files.
- Include only main and important code sections.
- Do not include layout XML or resource files.
- Include only important parts of configuration files (for example, permission settings in AndroidManifest.xml).
- Use the file name as a heading before each code listing.
- Add a short explanation (3-4 sentences) for each A4 page of source code.

Exclude from listings:
- Any generated or build output: build/, bin/, obj/, .gradle/, intermediates/, outputs/, tmp/
- Environment-specific machine files that do not explain your logic.

### Appendix A - Android Native App (Java/XML)

Core logic files (recommended):
- app/src/main/java/com/example/expensetracker/Database/DatabaseHelper.java
- app/src/main/java/com/example/expensetracker/UploadTask.java
- app/src/main/java/com/example/expensetracker/Models/Project.java
- app/src/main/java/com/example/expensetracker/Models/Expense.java

Important flow/controller files (recommended if they contain business logic):
- app/src/main/java/com/example/expensetracker/Activities/MainActivity.java
- app/src/main/java/com/example/expensetracker/Activities/ProjectDetailActivity.java
- app/src/main/java/com/example/expensetracker/Activities/ProjectFormActivity.java
- app/src/main/java/com/example/expensetracker/Activities/ExpenseFormActivity.java
- app/src/main/java/com/example/expensetracker/Activities/SearchActivity.java

Important configuration excerpt only (optional):
- app/src/main/AndroidManifest.xml (permissions and key activity declarations only)

### Appendix B - MAUI Cross-Platform App (C#/XAML)

Core MAUI logic/infrastructure files (recommended):
- MauiProgram.cs
- App.xaml.cs
- AppState.cs

Services and models:
- Services/FirebaseService.cs
- Services/FirebaseConfig.cs
- Models/ProjectItem.cs
- Models/ExpenseInput.cs

ViewModels:
- ViewModels/ProjectsViewModel.cs
- ViewModels/ProjectDetailsViewModel.cs
- ViewModels/AddExpenseViewModel.cs

Important controller/navigation files (include if they contain logic):
- AppShell.xaml.cs
- Views/ProjectsPage.xaml.cs
- Views/ProjectDetailsPage.xaml.cs
- Views/AddExpensePage.xaml.cs

Optional supporting file (if your lecturer expects project-level setup evidence):
- ExpenseTracker.csproj

Section 5 file tree (all files listed above):
\begin{verbatim}
Section5CodeListings/
├── AndroidApp_ExpenseTrackerCW/
│   └── app/
│       └── src/
│           └── main/
│               ├── AndroidManifest.xml
│               └── java/
│                   └── com/example/expensetracker/
│                       ├── UploadTask.java
│                       ├── Activities/
│                       │   ├── MainActivity.java
│                       │   ├── ProjectDetailActivity.java
│                       │   ├── ProjectFormActivity.java
│                       │   ├── ExpenseFormActivity.java
│                       │   └── SearchActivity.java
│                       ├── Database/
│                       │   └── DatabaseHelper.java
│                       └── Models/
│                           ├── Project.java
│                           └── Expense.java
└── MauiApp_ExpenseTracker/
    ├── ExpenseTracker.csproj
    ├── MauiProgram.cs
    ├── App.xaml.cs
    ├── AppState.cs
    ├── AppShell.xaml.cs
    ├── Services/
    │   ├── FirebaseService.cs
    │   └── FirebaseConfig.cs
    ├── Models/
    │   ├── ProjectItem.cs
    │   └── ExpenseInput.cs
    ├── ViewModels/
    │   ├── ProjectsViewModel.cs
    │   ├── ProjectDetailsViewModel.cs
    │   └── AddExpenseViewModel.cs
    └── Views/
        ├── ProjectsPage.xaml.cs
        ├── ProjectDetailsPage.xaml.cs
        └── AddExpensePage.xaml.cs
\end{verbatim}

### Appendix Code Snippets (Copy-Paste Ready)

#### DatabaseHelper.java
```java
@Override
public void onCreate(SQLiteDatabase db) {
   db.execSQL("CREATE TABLE " + T_PROJECT + " (" +
         "id INTEGER PRIMARY KEY AUTOINCREMENT," +
         "code TEXT NOT NULL," +
         "name TEXT NOT NULL," +
         "description TEXT NOT NULL," +
         "start_date TEXT NOT NULL," +
         "end_date TEXT NOT NULL," +
         "owner TEXT NOT NULL," +
         "status TEXT NOT NULL," +
         "budget REAL NOT NULL," +
         "special_requirements TEXT," +
         "client_info TEXT," +
         "notes TEXT)");

   db.execSQL("CREATE TABLE " + T_EXPENSE + " (" +
         "id INTEGER PRIMARY KEY AUTOINCREMENT," +
         "project_id INTEGER NOT NULL," +
         "expense_code TEXT NOT NULL," +
         "expense_date TEXT NOT NULL," +
         "amount REAL NOT NULL," +
         "currency TEXT NOT NULL," +
         "expense_type TEXT NOT NULL," +
         "payment_method TEXT NOT NULL," +
         "claimant TEXT NOT NULL," +
         "payment_status TEXT NOT NULL," +
         "description TEXT," +
         "location TEXT," +
         "FOREIGN KEY(project_id) REFERENCES projects(id) ON DELETE CASCADE)");
}

public long upsertProject(Project p) {
   SQLiteDatabase db = getWritableDatabase();
   ContentValues cv = new ContentValues();
   cv.put("code", p.code);
   cv.put("name", p.name);
   cv.put("description", p.description);
   cv.put("start_date", p.startDate);
   cv.put("end_date", p.endDate);
   cv.put("owner", p.owner);
   cv.put("status", p.status);
   cv.put("budget", p.budget);
   cv.put("special_requirements", p.specialRequirements);
   cv.put("client_info", p.clientInfo);
   cv.put("notes", p.notes);

   if (p.id > 0) {
      db.update(T_PROJECT, cv, "id=?", new String[]{String.valueOf(p.id)});
      return p.id;
   }
   return db.insert(T_PROJECT, null, cv);
}
```
Explanation: This is the main persistence layer in Android. It defines both tables and enforces project-expense linkage with a foreign key and cascade delete. The upsert method handles both create and update in one flow, simplifying Activity logic. This file is direct evidence for CRUD and local persistence implementation.

#### UploadTask.java
```java
public void uploadAll() {
   if (!isOnline()) {
      Toast.makeText(context, "No network available. Connect to internet first.", Toast.LENGTH_LONG).show();
      return;
   }

   ExecutorService executor = Executors.newSingleThreadExecutor();
   Handler handler = new Handler(Looper.getMainLooper());
   executor.execute(() -> {
      try {
         JSONObject payload = buildPayload();
         int code = putJson(payload.toString());
         handler.post(() -> Toast.makeText(context, "Upload finished. HTTP " + code, Toast.LENGTH_LONG).show());
      } catch (Exception e) {
         handler.post(() -> Toast.makeText(context, "Upload failed: " + e.getMessage(), Toast.LENGTH_LONG).show());
      }
   });
}

private int putJson(String json) throws Exception {
   URL url = new URL(UPLOAD_URL);
   HttpURLConnection conn = (HttpURLConnection) url.openConnection();
   conn.setRequestMethod("PUT");
   conn.setRequestProperty("Content-Type", "application/json");
   conn.setDoOutput(true);
   byte[] out = json.getBytes(StandardCharsets.UTF_8);
   try (OutputStream os = conn.getOutputStream()) {
      os.write(out);
   }
   return conn.getResponseCode();
}
```
Explanation: This class performs cloud upload from local SQLite to Firebase Realtime Database. It checks connectivity first, then runs network work on a background thread to keep UI responsive. The payload is uploaded using HTTP PUT to a .json endpoint, matching Firebase REST API requirements. This snippet supports your cloud sync functionality claim.

#### MainActivity.java
```java
btnAdd.setOnClickListener(v -> startActivity(new Intent(this, ProjectFormActivity.class)));
btnSearch.setOnClickListener(v -> startActivity(new Intent(this, SearchActivity.class)));
btnUpload.setOnClickListener(v -> new UploadTask(this, db).uploadAll());
btnReset.setOnClickListener(v -> confirmReset());

private void loadProjects() {
   projects.clear();
   projects.addAll(db.getAllProjects());
   List<String> labels = new ArrayList<>();
   for (Project p : projects) {
      labels.add(p.code + " | " + p.name + " | " + p.status + " | Budget: " + p.budget);
   }
   adapter.clear();
   adapter.addAll(labels);
   adapter.notifyDataSetChanged();
}
```
Explanation: This Activity coordinates the top-level user flow in the Android app. It routes users to create, search, upload, and reset actions while also showing project summaries. The load method binds database results into list labels for quick project overview. This is good evidence for project management and navigation flow.

#### SearchActivity.java
```java
btnRun.setOnClickListener(v -> {
   results.clear();
   results.addAll(db.searchProjects(
         etKeyword.getText().toString(),
         etDate.getText().toString(),
         etStatus.getText().toString(),
         etOwner.getText().toString(),
         etCode.getText().toString()
   ));

   List<String> labels = new ArrayList<>();
   for (Project p : results) {
      labels.add(p.code + " | " + p.name + " | " + p.status + " | " + p.startDate);
   }
   adapter.clear();
   adapter.addAll(labels);
   adapter.notifyDataSetChanged();
});
```
Explanation: This Activity implements multi-criteria search over projects. User inputs are passed to the SQL search helper, then rendered into a result list. It demonstrates practical filtering by keyword, date, status, owner, and code. This directly supports the search/filter feature in your checklist.

#### ProjectFormActivity.java
```java
private void validateAndConfirm() {
   if (isEmpty(etCode, "Please enter project code")) return;
   if (isEmpty(etName, "Please enter project name")) return;
   if (isEmpty(etDescription, "Please enter project description")) return;
   if (isEmpty(etStartDate, "Please enter start date")) return;
   if (isEmpty(etEndDate, "Please enter end date")) return;
   if (isEmpty(etOwner, "Please enter project manager/owner")) return;
   if (isEmpty(etBudget, "Please enter project budget")) return;

   double budget;
   try {
      budget = Double.parseDouble(etBudget.getText().toString().trim());
   } catch (NumberFormatException ex) {
      etBudget.setError("Budget must be a number");
      etBudget.requestFocus();
      return;
   }

   Project p = new Project();
   p.id = projectId;
   p.code = etCode.getText().toString().trim();
   p.name = etName.getText().toString().trim();
   p.status = spStatus.getSelectedItem().toString();
   p.budget = budget;

   db.upsertProject(p);
}
```
Explanation: This form handles validation and save for project records. Required fields are checked before save, and numeric parsing prevents invalid budget values. The same method supports create and edit by preserving projectId when present. This snippet is key evidence for validation and edit workflow.

#### ExpenseFormActivity.java
```java
private void validateAndSave() {
   if (isEmpty(etCode, "Please enter expense ID")) return;
   if (isEmpty(etDate, "Please enter date of expense")) return;
   if (isEmpty(etAmount, "Please enter amount")) return;
   if (isEmpty(etCurrency, "Please enter currency")) return;
   if (isEmpty(etClaimant, "Please enter claimant")) return;

   double amount;
   try {
      amount = Double.parseDouble(etAmount.getText().toString().trim());
   } catch (NumberFormatException ex) {
      etAmount.setError("Amount must be numeric");
      etAmount.requestFocus();
      return;
   }

   Expense e = new Expense();
   e.id = expenseId;
   e.projectId = projectId;
   e.amount = amount;
   e.paymentStatus = spStatus.getSelectedItem().toString();
   db.upsertExpense(e);
}
```
Explanation: This file captures expense data entry and update behavior. It validates required fields and enforces numeric amount before persisting. The expense is linked to a project through projectId, preserving project-level data grouping. This supports expense CRUD and data quality controls.

#### AndroidManifest.xml (important excerpt only)
```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<activity android:name=".Activities.ExpenseFormActivity" />
<activity android:name=".Activities.ProjectDetailActivity" />
<activity android:name=".Activities.ProjectFormActivity" />
<activity android:name=".Activities.SearchActivity" />
<activity
   android:name=".Activities.MainActivity"
   android:exported="true">
   <intent-filter>
      <action android:name="android.intent.action.MAIN" />
      <category android:name="android.intent.category.LAUNCHER" />
   </intent-filter>
</activity>
```
Explanation: This configuration snippet shows key runtime permissions and app entry point declarations. Internet and network-state permissions are required for upload/sync behavior. Activity registration confirms the implemented screens in your workflow. Only critical config is shown, matching your lecturer instruction.

#### MauiProgram.cs
```csharp
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<FirebaseConfig>();
builder.Services.AddSingleton<FirebaseService>();

builder.Services.AddSingleton<ProjectsViewModel>();
builder.Services.AddTransient<ProjectDetailsViewModel>();
builder.Services.AddTransient<AddExpenseViewModel>();

builder.Services.AddSingleton<ProjectsPage>();
builder.Services.AddTransient<ProjectDetailsPage>();
builder.Services.AddTransient<AddExpensePage>();
```
Explanation: This is the MAUI dependency-injection setup. Shared services are registered as singletons, while detail/edit screens use transient lifetimes. This arrangement separates infrastructure, view models, and pages in a maintainable structure. It is strong evidence of application architecture and bootstrapping.

#### FirebaseService.cs
```csharp
public async Task<List<ProjectItem>> GetProjectsFromRealtimeDatabaseAsync()
{
   var data = await GetRealtimeDatabaseAsync("mobileSync/latest.json");
   var projects = new List<ProjectItem>();

   if (data.ValueKind == JsonValueKind.Object &&
      data.TryGetProperty("projects", out var projectsArray) &&
      projectsArray.ValueKind == JsonValueKind.Array)
   {
      int arrayIndex = 0;
      foreach (var projectElement in projectsArray.EnumerateArray())
      {
         projects.Add(new ProjectItem
         {
            Id = GetJsonString(projectElement, "id"),
            Name = GetJsonString(projectElement, "name"),
            Status = GetJsonString(projectElement, "status"),
            ArrayIndex = arrayIndex
         });
         arrayIndex++;
      }
   }
   return projects;
}
```
Explanation: This method maps Firebase JSON into strongly typed project objects used by the UI. It handles array traversal and stores ArrayIndex for later expense operations. The service centralizes remote data access and keeps networking out of pages. This is a core evidence file for MAUI persistence and sync logic.

#### ProjectsViewModel.cs
```csharp
public IEnumerable<ProjectItem> FilteredProjects =>
   string.IsNullOrWhiteSpace(SearchText)
      ? Projects
      : Projects.Where(p => p.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);

public async Task LoadAsync()
{
   Projects.Clear();
   var projects = await _firebaseService.GetProjectsFromRealtimeDatabaseAsync();
   foreach (var project in projects)
   {
      Projects.Add(project);
   }
}
```
Explanation: This ViewModel controls list loading and client-side filtering behavior. It binds the project collection to the UI and updates the filtered view dynamically using SearchText. Data retrieval is delegated to FirebaseService, preserving separation of concerns. This supports your MAUI search and listing features.

#### AddExpenseViewModel.cs
```csharp
private async void SaveExpense()
{
   if (IsBusy || string.IsNullOrWhiteSpace(Amount))
   {
      StatusMessage = "Please enter an amount";
      return;
   }

   IsBusy = true;
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
   MainThread.BeginInvokeOnMainThread(async () => await Shell.Current.GoToAsync(".."));
   IsBusy = false;
}
```
Explanation: This method implements MAUI-side expense submission with validation, async save, status feedback, and navigation. It blocks duplicate submissions via IsBusy and ensures required amount input. The payload is sent through FirebaseService, keeping network logic outside the page. This demonstrates core expense entry workflow in the cross-platform app.

#### ProjectsPage.xaml.cs
```csharp
protected override async void OnAppearing()
{
   base.OnAppearing();
   await _viewModel.LoadAsync();
}

private async void OnViewProjectDetails(object sender, EventArgs e)
{
   var button = (Button)sender;
   var project = (ProjectItem)button.CommandParameter;
   if (project == null) return;

   AppState.CurrentProject = project;
   await Shell.Current.GoToAsync("///ProjectDetailsPage");
}
```
Explanation: This page code-behind connects page lifecycle and navigation to the ViewModel flow. It loads project data when the page appears, then stores selected project context before navigation. AppState acts as a lightweight state handoff between pages. This snippet is useful evidence for MAUI interaction flow without showing XAML layout code.

Suggested write-up template per file:
- Heading: FileName.ext
- Code: Paste only the important methods/blocks used to implement the feature.
- Explanation paragraph (3-4 sentences): describe what the file does, which feature(s) it supports, and why the shown code is important.

Presentation tip:
- Prioritize backend/logic files.
- Include Activities and MAUI code-behind only when they contain important workflow logic.
- Do not include layout/resource files.
- Keep each listing short and focused (selected methods plus brief explanation).

---

## Final Checklist Before Submission

- Replace bracket placeholders (name, module, date).
- Update Section 1 statuses if your newest code differs.
- Insert real screenshots in Section 4.
- Add actual code listings in appendices.
- Run spelling and grammar check.
- Ensure total word count in Sections 2 and 3 remains within requirements.
