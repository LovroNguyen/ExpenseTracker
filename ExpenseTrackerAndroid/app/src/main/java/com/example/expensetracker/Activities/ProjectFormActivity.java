package com.example.expensetracker.Activities;

import android.os.Bundle;
import android.text.TextUtils;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import com.example.expensetracker.Database.DatabaseHelper;
import com.example.expensetracker.Models.Project;
import com.example.expensetracker.R;

public class ProjectFormActivity extends AppCompatActivity {
    private DatabaseHelper db;
    private long projectId = -1;
    private EditText etCode, etName, etDescription, etStartDate, etEndDate, etOwner, etBudget, etSpecial, etClient, etNotes;
    private Spinner spStatus;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_project_form);
        db = new DatabaseHelper(this);
        bindViews();

        spStatus.setAdapter(new ArrayAdapter<>(this, android.R.layout.simple_spinner_dropdown_item,
                new String[]{"Active", "Completed", "On Hold"}));

        projectId = getIntent().getLongExtra("project_id", -1);
        if (projectId > 0) {
            loadProject(projectId);
        }

        Button btnSave = findViewById(R.id.btnSaveProject);
        btnSave.setOnClickListener(v -> validateAndConfirm());
    }

    private void bindViews() {
        etCode = findViewById(R.id.etProjectCode);
        etName = findViewById(R.id.etProjectName);
        etDescription = findViewById(R.id.etDescription);
        etStartDate = findViewById(R.id.etStartDate);
        etEndDate = findViewById(R.id.etEndDate);
        etOwner = findViewById(R.id.etOwner);
        etBudget = findViewById(R.id.etBudget);
        etSpecial = findViewById(R.id.etSpecialRequirements);
        etClient = findViewById(R.id.etClientInfo);
        etNotes = findViewById(R.id.etNotes);
        spStatus = findViewById(R.id.spStatus);
    }

    private void loadProject(long id) {
        Project p = db.getProject(id);
        if (p == null) return;
        etCode.setText(p.code);
        etName.setText(p.name);
        etDescription.setText(p.description);
        etStartDate.setText(p.startDate);
        etEndDate.setText(p.endDate);
        etOwner.setText(p.owner);
        etBudget.setText(String.valueOf(p.budget));
        etSpecial.setText(p.specialRequirements);
        etClient.setText(p.clientInfo);
        etNotes.setText(p.notes);
        setSpinnerSelection(spStatus, p.status);
    }

    private void validateAndConfirm() {
        if (isEmpty(etCode, "Please enter project code")) return;
        if (isEmpty(etName, "Please enter project name")) return;
        if (isEmpty(etDescription, "Please enter project description")) return;
        if (isEmpty(etStartDate, "Please enter start date")) return;
        if (isEmpty(etEndDate, "Please enter end date")) return;
        if (isEmpty(etOwner, "Please enter project manager/owner")) return;
        if (isEmpty(etBudget, "Please enter project budget")) return;

        String budgetText = etBudget.getText().toString().trim();
        double budget;
        try {
            budget = Double.parseDouble(budgetText);
        } catch (NumberFormatException ex) {
            etBudget.setError("Budget must be a number");
            etBudget.requestFocus();
            return;
        }

        Project p = new Project();
        p.id = projectId;
        p.code = etCode.getText().toString().trim();
        p.name = etName.getText().toString().trim();
        p.description = etDescription.getText().toString().trim();
        p.startDate = etStartDate.getText().toString().trim();
        p.endDate = etEndDate.getText().toString().trim();
        p.owner = etOwner.getText().toString().trim();
        p.status = spStatus.getSelectedItem().toString();
        p.budget = budget;
        p.specialRequirements = etSpecial.getText().toString().trim();
        p.clientInfo = etClient.getText().toString().trim();
        p.notes = etNotes.getText().toString().trim();

        String confirm = "Code: " + p.code + "\nName: " + p.name + "\nDescription: " + p.description +
                "\nDates: " + p.startDate + " to " + p.endDate + "\nOwner: " + p.owner +
                "\nStatus: " + p.status + "\nBudget: " + p.budget;

        new AlertDialog.Builder(this)
                .setTitle("Confirm project details")
                .setMessage(confirm)
                .setPositiveButton("Save", (d, w) -> {
                    db.upsertProject(p);
                    Toast.makeText(this, "Project saved", Toast.LENGTH_SHORT).show();
                    finish();
                })
                .setNegativeButton("Edit", null)
                .show();
    }

    private boolean isEmpty(EditText et, String message) {
        if (TextUtils.isEmpty(et.getText().toString().trim())) {
            et.setError(message);
            et.requestFocus();
            return true;
        }
        return false;
    }

    private void setSpinnerSelection(Spinner sp, String value) {
        ArrayAdapter<?> adapter = (ArrayAdapter<?>) sp.getAdapter();
        if (adapter == null) return;
        for (int i = 0; i < adapter.getCount(); i++) {
            if (value != null && value.equals(adapter.getItem(i))) {
                sp.setSelection(i);
                return;
            }
        }
    }
}
