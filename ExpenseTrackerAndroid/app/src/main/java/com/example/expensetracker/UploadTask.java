package com.example.expensetracker;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkCapabilities;
import android.os.Handler;
import android.os.Looper;
import android.widget.Toast;

import com.example.expensetracker.Database.DatabaseHelper;
import com.example.expensetracker.Models.Expense;
import com.example.expensetracker.Models.Project;

import org.json.JSONArray;
import org.json.JSONObject;

import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class UploadTask {
    // Realtime Database REST API requires a non-root path ending with .json.
    private static final String UPLOAD_URL = "https://expense-tracker-3263c-default-rtdb.asia-southeast1.firebasedatabase.app/mobileSync/latest.json";
    private final Context context;
    private final DatabaseHelper db;

    public UploadTask(Context context, DatabaseHelper db) {
        this.context = context;
        this.db = db;
    }

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

    private JSONObject buildPayload() throws Exception {
        JSONObject root = new JSONObject();
        JSONArray projectsJson = new JSONArray();

        List<Project> projects = db.getAllProjects();
        for (Project p : projects) {
            JSONObject pJson = new JSONObject();
            pJson.put("id", p.id);
            pJson.put("code", p.code);
            pJson.put("name", p.name);
            pJson.put("description", p.description);
            pJson.put("startDate", p.startDate);
            pJson.put("endDate", p.endDate);
            pJson.put("owner", p.owner);
            pJson.put("status", p.status);
            pJson.put("budget", p.budget);
            pJson.put("specialRequirements", p.specialRequirements);
            pJson.put("clientInfo", p.clientInfo);
            pJson.put("notes", p.notes);

            JSONArray expensesJson = new JSONArray();
            for (Expense e : db.getExpensesByProject(p.id)) {
                JSONObject eJson = new JSONObject();
                eJson.put("id", e.id);
                eJson.put("expenseCode", e.expenseCode);
                eJson.put("date", e.date);
                eJson.put("amount", e.amount);
                eJson.put("currency", e.currency);
                eJson.put("expenseType", e.expenseType);
                eJson.put("paymentMethod", e.paymentMethod);
                eJson.put("claimant", e.claimant);
                eJson.put("paymentStatus", e.paymentStatus);
                eJson.put("description", e.description);
                eJson.put("location", e.location);
                expensesJson.put(eJson);
            }
            pJson.put("expenses", expensesJson);
            projectsJson.put(pJson);
        }
        root.put("projects", projectsJson);
        return root;
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

    private boolean isOnline() {
        ConnectivityManager cm = (ConnectivityManager) context.getSystemService(Context.CONNECTIVITY_SERVICE);
        if (cm == null) return false;
        NetworkCapabilities capabilities = cm.getNetworkCapabilities(cm.getActiveNetwork());
        return capabilities != null && (
                capabilities.hasTransport(NetworkCapabilities.TRANSPORT_WIFI) ||
                        capabilities.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR) ||
                        capabilities.hasTransport(NetworkCapabilities.TRANSPORT_ETHERNET)
        );
    }
}
