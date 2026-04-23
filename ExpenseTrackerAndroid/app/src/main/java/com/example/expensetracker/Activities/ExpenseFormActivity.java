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
import com.example.expensetracker.Models.Expense;
import com.example.expensetracker.R;

public class ExpenseFormActivity extends AppCompatActivity {
    private DatabaseHelper db;
    private long projectId;
    private long expenseId;
    private EditText etCode, etDate, etAmount, etCurrency, etClaimant, etDescription, etLocation;
    private Spinner spType, spMethod, spStatus;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_expense_form);
        db = new DatabaseHelper(this);

        projectId = getIntent().getLongExtra("project_id", -1);
        expenseId = getIntent().getLongExtra("expense_id", -1);
        bind();
        setupSpinners();

        if (expenseId > 0) {
            loadExpense(expenseId);
        }

        Button btnSave = findViewById(R.id.btnSaveExpense);
        Button btnDelete = findViewById(R.id.btnDeleteExpense);

        btnSave.setOnClickListener(v -> validateAndSave());
        btnDelete.setOnClickListener(v -> {
            if (expenseId <= 0) {
                Toast.makeText(this, "Nothing to delete", Toast.LENGTH_SHORT).show();
                return;
            }
            new AlertDialog.Builder(this)
                    .setTitle("Delete expense?")
                    .setPositiveButton("Delete", (d, w) -> {
                        db.deleteExpense(expenseId);
                        finish();
                    })
                    .setNegativeButton("Cancel", null)
                    .show();
        });
    }

    private void bind() {
        etCode = findViewById(R.id.etExpenseCode);
        etDate = findViewById(R.id.etExpenseDate);
        etAmount = findViewById(R.id.etAmount);
        etCurrency = findViewById(R.id.etCurrency);
        etClaimant = findViewById(R.id.etClaimant);
        etDescription = findViewById(R.id.etExpenseDescription);
        etLocation = findViewById(R.id.etLocation);
        spType = findViewById(R.id.spExpenseType);
        spMethod = findViewById(R.id.spPaymentMethod);
        spStatus = findViewById(R.id.spPaymentStatus);
    }

    private void setupSpinners() {
        spType.setAdapter(new ArrayAdapter<>(this, android.R.layout.simple_spinner_dropdown_item,
                new String[]{"Travel", "Equipment", "Materials", "Services", "Software/Licenses",
                        "Labour costs", "Utilities", "Miscellaneous"}));
        spMethod.setAdapter(new ArrayAdapter<>(this, android.R.layout.simple_spinner_dropdown_item,
                new String[]{"Cash", "Credit Card", "Bank Transfer", "Cheque"}));
        spStatus.setAdapter(new ArrayAdapter<>(this, android.R.layout.simple_spinner_dropdown_item,
                new String[]{"Paid", "Pending", "Reimbursed"}));
    }

    private void loadExpense(long id) {
        Expense e = db.getExpense(id);
        if (e == null) return;
        etCode.setText(e.expenseCode);
        etDate.setText(e.date);
        etAmount.setText(String.valueOf(e.amount));
        etCurrency.setText(e.currency);
        etClaimant.setText(e.claimant);
        etDescription.setText(e.description);
        etLocation.setText(e.location);
        setSpinner(spType, e.expenseType);
        setSpinner(spMethod, e.paymentMethod);
        setSpinner(spStatus, e.paymentStatus);
    }

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
        e.expenseCode = etCode.getText().toString().trim();
        e.date = etDate.getText().toString().trim();
        e.amount = amount;
        e.currency = etCurrency.getText().toString().trim();
        e.expenseType = spType.getSelectedItem().toString();
        e.paymentMethod = spMethod.getSelectedItem().toString();
        e.claimant = etClaimant.getText().toString().trim();
        e.paymentStatus = spStatus.getSelectedItem().toString();
        e.description = etDescription.getText().toString().trim();
        e.location = etLocation.getText().toString().trim();

        db.upsertExpense(e);
        Toast.makeText(this, "Expense saved", Toast.LENGTH_SHORT).show();
        finish();
    }

    private boolean isEmpty(EditText et, String message) {
        if (TextUtils.isEmpty(et.getText().toString().trim())) {
            et.setError(message);
            et.requestFocus();
            return true;
        }
        return false;
    }

    private void setSpinner(Spinner sp, String value) {
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
