using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float WalkSpeed;
    public float TurnSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        var movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (movement.x != 0 || movement.z != 0)
        {
            transform.Translate(movement * WalkSpeed * Time.deltaTime);
        }
        
    }
}
