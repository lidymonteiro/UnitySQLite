using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapNeedle : MonoBehaviour
{
    public Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        this.weapon = GamesCodeDataSource.Instance.WeaponDAO.GetWeapon(3);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("Atacou o personagem!");
            print(this.weapon.Attack);
            other.GetComponent<CharacterController>().TakeDamage(this.weapon.Attack);
        }
    }

    
}
