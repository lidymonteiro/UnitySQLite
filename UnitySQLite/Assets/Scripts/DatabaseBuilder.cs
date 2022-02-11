using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Data.Sqlite;
using System.IO;
using System;

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

        //CreateDatabaseFileIfNotExists();
        CopyDatabaseFileIfNotExists();

        try
        {
            //CreateTableWeapon();
            //CreateTableCharacter();
            //InsertDataWeapon("Sword", 10, 25.89d);
            //InsertDataCharacter("Kaz", 2, 1, 3, 10, 1);
            //InsertDataCharacter("Kaz", 2, 1, 3, 10, 1);
            //Debug.Log(GetCharacter(1));
            //Debug.Log(DeleteCharacter(1));
            //Debug.Log("UPDATE CHARACTER: " + UpdateCharacter(1, "Lid Monteiro", 400, 75, 6, 7, 1));
            InsertDataCharacter("Mariana", 2, 1, 3, 10, 1);
        }
        catch (Exception e)
        {
            Debug.LogError($"Database not created! {e.Message}");
        }
    }

    #region Create database

    private void CopyDatabaseFileIfNotExists()
    {
        this.databasePath = Path.Combine(Application.persistentDataPath, this.databaseName);

        if (File.Exists(this.databasePath))
            return;

        var originDatabasePath = string.Empty;
        var isAndroid = false;

#if UNITY_EDITOR || UNITY_WP8 || UNITY_WINRT

        originDatabasePath = Path.Combine(Application.streamingAssetsPath, this.databaseName);

#elif UNITY_STANDALONE_OSX

        originDatabasePath = Path.Combine(Application.dataPath, "/Resources/Data/StreamingAssets/", this.databaseName);

#elif UNITY_IOS

        originDatabasePath = Path.Combine(Application.dataPath, "Raw", this.databaseName);


#elif UNITY_ANDROID
        isAndroid = true;
        originDatabasePath = "jar:file://" + Application.dataPath + "!/assets" + this.databaseName;
        StartCoroutine(GetInternalFileAndroid(originDatabasePath));
        
        
#endif
        if(!isAndroid)
            File.Copy(originDatabasePath, this.databasePath);


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
   
    private IEnumerator GetInternalFileAndroid(string path)
    {
        var request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();

        if(request.isHttpError || request.isNetworkError)
        {
            Debug.LogError($"Error reading android file: {request.error}");
        }
        else
        {
            File.WriteAllBytes(this.databasePath, request.downloadHandler.data);
            Debug.Log("File copied!");
        }
    }

    #endregion

    protected void CreateTableCharacter()
    {
        var commandText = 
           "CREATE TABLE Character " +
           "(" +
           "   Id INTEGER PRIMARY KEY, " +
           "   Name TEXT NOT NULL, " +
           "   Attack INTEGER NOT NULL, " +
           "   Defense INTEGER NOT NULL, " +
           "   Agility INTEGER NOT NULL, " +
           "   Health INTEGER NOT NULL, " +
           "   WeaponId INTEGER," +
           "   FOREIGN KEY (WeaponId) REFERENCES Weapon(Id) ON UPDATE CASCADE ON DELETE RESTRICT" +
           ");";

        using (var connection = Connection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                Debug.Log("Create table Character...");
            }
        }
    }


    protected void CreateTableWeapon()
    {
        var commandText =
            "CREATE TABLE Weapon" +
            "(" +
            "   Id INTEGER PRIMARY KEY, " +
            "   Name TEXT NOT NULL, " +
            "   Attack INTEGER NOT NULL, " +
            "   Price REAL NOT NULL" +
            "); ";

        using (var connection = Connection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
                Debug.Log("Create table Weapon...");
            }
        }
    }

    
    protected void InsertDataWeapon(string name, int attack, double price)
    {
        var commandText = "INSERT INTO Weapon(Name, Attack, Price) VALUES (@name, @attack, @price);";

        using (var connection = Connection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@attack", attack);
                command.Parameters.AddWithValue("@price", price);

                var result = command.ExecuteNonQuery();
                Debug.Log($"INSERT WEAPON: {result.ToString()}");
            }
        }
    }

    
    protected void InsertDataCharacter(string name, int attack, int defense, int agility, int health, int weaponId)
    {
        var commandText = "INSERT INTO Character(Name, Attack, Defense, Agility, Health, WeaponId) VALUES (@name, @attack, @defense, @agility, @health, @weaponId);";

        using (var connection = Connection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@attack", attack);
                command.Parameters.AddWithValue("@defense", defense);
                command.Parameters.AddWithValue("@agility", agility);
                command.Parameters.AddWithValue("@health", health);
                command.Parameters.AddWithValue("@weaponId", weaponId);

                var result = command.ExecuteNonQueryWithFK();
                Debug.Log($"INSERT CHARACTER: {result.ToString()}");
            }
        }
    }


    protected string GetCharacter(int id)  
    {
        var commandText = "SELECT * FROM Character WHERE Id = @id;";
        var result = "None";

        using (var connection = Connection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                command.Parameters.AddWithValue("@id", id);

                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    result = $"Id: {reader["Id"]}, " +
                        $"Name: {reader.GetString(1)}, " +
                        $"Attack: {reader["Attack"]}, " +
                        $"Defense: {reader["Defense"]}, " +
                        $"Agility: {reader["Agility"]}, " +
                        $"Health: {reader["Health"]}, " +
                        $"Weapon Id: {reader["WeaponId"]}";
                }
                Debug.Log($"SELECT CHARACTER...");
                return result;
            }
        }   
    }


    protected int DeleteCharacter(int id)
    {
        var commandText = "DELETE FROM Character WHERE Id = @id;";

        using (var connection = Connection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.AddWithValue("@id", id);
                Debug.Log($"DELETE CHARACTER...");
                return command.ExecuteNonQuery();
            }
        }
    }


    protected int UpdateCharacter(int id, string name, int attack, int defense, int agility, int health, int weaponId)
    {
        var commandText =
            "UPDATE Character SET " +
            "Name = @name, " +
            "Attack = @attack, " +
            "Defense = @defense, " +
            "Agility = @agility, " +
            "Health = @health, " +
            "WeaponId = @weaponId " +
            "WHERE Id = @id;";

        using (var connection = Connection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@attack", attack);
                command.Parameters.AddWithValue("@defense", defense);
                command.Parameters.AddWithValue("@agility", agility);
                command.Parameters.AddWithValue("@health", health);
                command.Parameters.AddWithValue("@weaponId", weaponId);

                return command.ExecuteNonQuery();
               
            }
        }
    }


}
