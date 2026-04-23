package com.example.expensetracker.Activities;

import android.content.Intent;
import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import com.example.expensetracker.Database.DatabaseHelper;
import com.example.expensetracker.Models.Project;
import com.example.expensetracker.R;
import com.example.expensetracker.UploadTask;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity {
    private DatabaseHelper db;
    private final List<Project> projects = new ArrayList<>();
    private ArrayAdapter<String> adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        db = new DatabaseHelper(this);
        ListView listProjects = findViewById(R.id.listProjects);
        Button btnAdd = findViewById(R.id.btnAddProject);
        Button btnSearch = findViewById(R.id.btnSearch);
        Button btnUpload = findViewById(R.id.btnUploadAll);
        Button btnReset = findViewById(R.id.btnResetDb);

        adapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, new ArrayList<>());
        listProjects.setAdapter(adapter);

        btnAdd.setOnClickListener(v -> startActivity(new Intent(this, ProjectFormActivity.class)));
        btnSearch.setOnClickListener(v -> startActivity(new Intent(this, SearchActivity.class)));
        btnUpload.setOnClickListener(v -> new UploadTask(this, db).uploadAll());
        btnReset.setOnClickListener(v -> confirmReset());

        listProjects.setOnItemClickListener((parent, view, position, id) -> {
            Intent intent = new Intent(this, ProjectDetailActivity.class);
            intent.putExtra("project_id", projects.get(position).id);
            startActivity(intent);
        });
    }

    @Override
    protected void onResume() {
        super.onResume();
        loadProjects();
    }

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

    private void confirmReset() {
        new AlertDialog.Builder(this)
                .setTitle("Reset database?")
                .setMessage("This will remove all projects and expenses.")
                .setPositiveButton("Yes", (dialog, which) -> {
                    db.resetDatabase();
                    loadProjects();
                    Toast.makeText(this, "Database reset complete", Toast.LENGTH_SHORT).show();
                })
                .setNegativeButton("No", null)
                .show();
    }
}
