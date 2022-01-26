using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

public class DatabaseBuilder : MonoBehaviour
{

    public string databaseName;
    protected string databasePath;
    protected SqliteConnection Connection => new SqliteConnection($"Data source = {this.databasePath};");


    private void Awake()
    {
        if (string.IsNullOrEmpty(this.databaseName))
        {
            Debug.LogError("Database name is empty!");
            return;
        }

        CreateDatabaseFileIfNotExists();
    }
    
    private void CreateDatabaseFileIfNotExists()
    {
        this.databasePath = Path.Combine(Application.persistentDataPath, this.databaseName);
        if (!File.Exists(this.databasePath))
        {
            SqliteConnection.CreateFile(this.databasePath);
            Debug.Log($"Database path: {this.databasePath}");
        }
    }
   
}
