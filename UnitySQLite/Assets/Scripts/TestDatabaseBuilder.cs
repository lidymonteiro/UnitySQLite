using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;

public class TestDatabaseBuilder : MonoBehaviour
{

    private void Awake()
    {
        using (var con = new SqliteConnection("Data source = TesteDB.db"))
        {
            con.Open();
            Debug.Log("Conexão de teste com banco de dados realizada com sucesso!");
        }
    }


}
