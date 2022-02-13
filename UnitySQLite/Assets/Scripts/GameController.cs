using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {


        Debug.Log("Entrei aqui");
        var w = GamesCodeDataSource.Instance.WeaponDAO.GetWeapon(1);
        
        Debug.Log(w);
        Debug.Log(w.Name);
        Debug.Log($"Terminei!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
