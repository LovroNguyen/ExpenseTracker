package com.example.expensetracker.Database;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import com.example.expensetracker.Models.Expense;
import com.example.expensetracker.Models.Project;

import java.util.ArrayList;
import java.util.List;

public class DatabaseHelper extends SQLiteOpenHelper {
    private static final String DB_NAME = "expense_tracker.db";
    private static final int DB_VERSION = 1;

    public static final String T_PROJECT = "projects";
    public static final String T_EXPENSE = "expenses";

    public DatabaseHelper(Context context) {
        super(context, DB_NAME, null, DB_VERSION);
    }

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

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        db.execSQL("DROP TABLE IF EXISTS " + T_EXPENSE);
        db.execSQL("DROP TABLE IF EXISTS " + T_PROJECT);
        onCreate(db);
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

    public List<Project> getAllProjects() {
        return searchProjects("", "", "", "", "");
    }

    public Project getProject(long id) {
        SQLiteDatabase db = getReadableDatabase();
        Cursor c = db.rawQuery("SELECT * FROM " + T_PROJECT + " WHERE id=?", new String[]{String.valueOf(id)});
        if (c.moveToFirst()) {
            Project p = mapProject(c);
            c.close();
            return p;
        }
        c.close();
        return null;
    }

    public void deleteProject(long id) {
        SQLiteDatabase db = getWritableDatabase();
        db.delete(T_EXPENSE, "project_id=?", new String[]{String.valueOf(id)});
        db.delete(T_PROJECT, "id=?", new String[]{String.valueOf(id)});
    }

    public void resetDatabase() {
        SQLiteDatabase db = getWritableDatabase();
        db.delete(T_EXPENSE, null, null);
        db.delete(T_PROJECT, null, null);
    }

    public List<Project> searchProjects(String nameOrDesc, String date, String status, String owner, String code) {
        SQLiteDatabase db = getReadableDatabase();
        StringBuilder sql = new StringBuilder("SELECT * FROM ").append(T_PROJECT).append(" WHERE 1=1");
        List<String> args = new ArrayList<>();

        if (!nameOrDesc.trim().isEmpty()) {
            sql.append(" AND (name LIKE ? OR description LIKE ?)");
            String v = "%" + nameOrDesc.trim() + "%";
            args.add(v);
            args.add(v);
        }
        if (!date.trim().isEmpty()) {
            sql.append(" AND (start_date LIKE ? OR end_date LIKE ?)");
            String v = "%" + date.trim() + "%";
            args.add(v);
            args.add(v);
        }
        if (!status.trim().isEmpty()) {
            sql.append(" AND status LIKE ?");
            args.add("%" + status.trim() + "%");
        }
        if (!owner.trim().isEmpty()) {
            sql.append(" AND owner LIKE ?");
            args.add("%" + owner.trim() + "%");
        }
        if (!code.trim().isEmpty()) {
            sql.append(" AND code LIKE ?");
            args.add("%" + code.trim() + "%");
        }
        sql.append(" ORDER BY id DESC");

        Cursor c = db.rawQuery(sql.toString(), args.toArray(new String[0]));
        List<Project> list = new ArrayList<>();
        while (c.moveToNext()) {
            list.add(mapProject(c));
        }
        c.close();
        return list;
    }

    public long upsertExpense(Expense e) {
        SQLiteDatabase db = getWritableDatabase();
        ContentValues cv = new ContentValues();
        cv.put("project_id", e.projectId);
        cv.put("expense_code", e.expenseCode);
        cv.put("expense_date", e.date);
        cv.put("amount", e.amount);
        cv.put("currency", e.currency);
        cv.put("expense_type", e.expenseType);
        cv.put("payment_method", e.paymentMethod);
        cv.put("claimant", e.claimant);
        cv.put("payment_status", e.paymentStatus);
        cv.put("description", e.description);
        cv.put("location", e.location);

        if (e.id > 0) {
            db.update(T_EXPENSE, cv, "id=?", new String[]{String.valueOf(e.id)});
            return e.id;
        }
        return db.insert(T_EXPENSE, null, cv);
    }

    public List<Expense> getExpensesByProject(long projectId) {
        SQLiteDatabase db = getReadableDatabase();
        Cursor c = db.rawQuery("SELECT * FROM " + T_EXPENSE + " WHERE project_id=? ORDER BY id DESC",
                new String[]{String.valueOf(projectId)});
        List<Expense> list = new ArrayList<>();
        while (c.moveToNext()) {
            list.add(mapExpense(c));
        }
        c.close();
        return list;
    }

    public Expense getExpense(long id) {
        SQLiteDatabase db = getReadableDatabase();
        Cursor c = db.rawQuery("SELECT * FROM " + T_EXPENSE + " WHERE id=?", new String[]{String.valueOf(id)});
        if (c.moveToFirst()) {
            Expense e = mapExpense(c);
            c.close();
            return e;
        }
        c.close();
        return null;
    }

    public void deleteExpense(long id) {
        SQLiteDatabase db = getWritableDatabase();
        db.delete(T_EXPENSE, "id=?", new String[]{String.valueOf(id)});
    }

    private Project mapProject(Cursor c) {
        Project p = new Project();
        p.id = c.getLong(c.getColumnIndexOrThrow("id"));
        p.code = c.getString(c.getColumnIndexOrThrow("code"));
        p.name = c.getString(c.getColumnIndexOrThrow("name"));
        p.description = c.getString(c.getColumnIndexOrThrow("description"));
        p.startDate = c.getString(c.getColumnIndexOrThrow("start_date"));
        p.endDate = c.getString(c.getColumnIndexOrThrow("end_date"));
        p.owner = c.getString(c.getColumnIndexOrThrow("owner"));
        p.status = c.getString(c.getColumnIndexOrThrow("status"));
        p.budget = c.getDouble(c.getColumnIndexOrThrow("budget"));
        p.specialRequirements = c.getString(c.getColumnIndexOrThrow("special_requirements"));
        p.clientInfo = c.getString(c.getColumnIndexOrThrow("client_info"));
        p.notes = c.getString(c.getColumnIndexOrThrow("notes"));
        return p;
    }

    private Expense mapExpense(Cursor c) {
        Expense e = new Expense();
        e.id = c.getLong(c.getColumnIndexOrThrow("id"));
        e.projectId = c.getLong(c.getColumnIndexOrThrow("project_id"));
        e.expenseCode = c.getString(c.getColumnIndexOrThrow("expense_code"));
        e.date = c.getString(c.getColumnIndexOrThrow("expense_date"));
        e.amount = c.getDouble(c.getColumnIndexOrThrow("amount"));
        e.currency = c.getString(c.getColumnIndexOrThrow("currency"));
        e.expenseType = c.getString(c.getColumnIndexOrThrow("expense_type"));
        e.paymentMethod = c.getString(c.getColumnIndexOrThrow("payment_method"));
        e.claimant = c.getString(c.getColumnIndexOrThrow("claimant"));
        e.paymentStatus = c.getString(c.getColumnIndexOrThrow("payment_status"));
        e.description = c.getString(c.getColumnIndexOrThrow("description"));
        e.location = c.getString(c.getColumnIndexOrThrow("location"));
        return e;
    }
}
