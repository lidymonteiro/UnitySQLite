using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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

        //CreateDatabaseFileIfNotExists();
        CopyDatabaseFileIfNotExists();
    }

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
}
