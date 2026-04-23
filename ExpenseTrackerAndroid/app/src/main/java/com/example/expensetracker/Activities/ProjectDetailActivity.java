package com.example.expensetracker.Activities;

import android.content.Intent;
import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import com.example.expensetracker.Database.DatabaseHelper;
import com.example.expensetracker.Models.Expense;
import com.example.expensetracker.Models.Project;
import com.example.expensetracker.R;

import java.util.ArrayList;
import java.util.List;

public class ProjectDetailActivity extends AppCompatActivity {
    private DatabaseHelper db;
    private long projectId;
    private final List<Expense> expenses = new ArrayList<>();
    private ArrayAdapter<String> adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_project_detail);
        db = new DatabaseHelper(this);

        projectId = getIntent().getLongExtra("project_id", -1);
        TextView tvProject = findViewById(R.id.tvProjectDetail);
        Button btnEdit = findViewById(R.id.btnEditProject);
        Button btnDelete = findViewById(R.id.btnDeleteProject);
        Button btnAddExpense = findViewById(R.id.btnAddExpense);
        ListView lvExpenses = findViewById(R.id.listExpenses);

        adapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, new ArrayList<>());
        lvExpenses.setAdapter(adapter);

        btnEdit.setOnClickListener(v -> {
            Intent i = new Intent(this, ProjectFormActivity.class);
            i.putExtra("project_id", projectId);
            startActivity(i);
        });
        btnDelete.setOnClickListener(v -> confirmDeleteProject());
        btnAddExpense.setOnClickListener(v -> {
            Intent i = new Intent(this, ExpenseFormActivity.class);
            i.putExtra("project_id", projectId);
            startActivity(i);
        });
        lvExpenses.setOnItemClickListener((parent, view, position, id) -> {
            Intent i = new Intent(this, ExpenseFormActivity.class);
            i.putExtra("project_id", projectId);
            i.putExtra("expense_id", expenses.get(position).id);
            startActivity(i);
        });

        Project p = db.getProject(projectId);
        if (p != null) {
            tvProject.setText(p.code + " - " + p.name + "\n" + p.description + "\nOwner: " + p.owner +
                    "\nStatus: " + p.status + "\nBudget: " + p.budget);
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        loadExpenses();
    }

    private void loadExpenses() {
        expenses.clear();
        expenses.addAll(db.getExpensesByProject(projectId));
        List<String> labels = new ArrayList<>();
        for (Expense e : expenses) {
            labels.add(e.expenseCode + " | " + e.expenseType + " | " + e.amount + " " + e.currency + " | " + e.paymentStatus);
        }
        adapter.clear();
        adapter.addAll(labels);
        adapter.notifyDataSetChanged();
    }

    private void confirmDeleteProject() {
        new AlertDialog.Builder(this)
                .setTitle("Delete project?")
                .setMessage("This also removes all expenses under this project.")
                .setPositiveButton("Delete", (d, w) -> {
                    db.deleteProject(projectId);
                    finish();
                })
                .setNegativeButton("Cancel", null)
                .show();
    }
}
