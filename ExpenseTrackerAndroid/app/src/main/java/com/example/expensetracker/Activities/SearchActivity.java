package com.example.expensetracker.Activities;

import android.content.Intent;
import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;

import androidx.appcompat.app.AppCompatActivity;

import com.example.expensetracker.Database.DatabaseHelper;
import com.example.expensetracker.Models.Project;
import com.example.expensetracker.R;

import java.util.ArrayList;
import java.util.List;

public class SearchActivity extends AppCompatActivity {
    private DatabaseHelper db;
    private final List<Project> results = new ArrayList<>();
    private ArrayAdapter<String> adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_search);
        db = new DatabaseHelper(this);

        EditText etKeyword = findViewById(R.id.etSearchKeyword);
        EditText etDate = findViewById(R.id.etSearchDate);
        EditText etStatus = findViewById(R.id.etSearchStatus);
        EditText etOwner = findViewById(R.id.etSearchOwner);
        EditText etCode = findViewById(R.id.etSearchCode);
        Button btnRun = findViewById(R.id.btnRunSearch);
        ListView list = findViewById(R.id.listSearchResults);

        adapter = new ArrayAdapter<>(this, android.R.layout.simple_list_item_1, new ArrayList<>());
        list.setAdapter(adapter);

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

        list.setOnItemClickListener((parent, view, position, id) -> {
            Intent i = new Intent(this, ProjectDetailActivity.class);
            i.putExtra("project_id", results.get(position).id);
            startActivity(i);
        });
    }
}
