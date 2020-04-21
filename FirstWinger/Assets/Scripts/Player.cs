using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Vector3 MoveVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        Vector3 move = MoveVector;

        transform.position += MoveVector;
    }

    void ProcessInput(Vector3 moveDirection)
    {
        MoveVector = moveDirection * 1.0f;
    }
}
