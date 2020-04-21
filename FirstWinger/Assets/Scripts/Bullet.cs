using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OwnerSide : int
{
    Player = 0,
    Enemy
}

public class Bullet : MonoBehaviour
{

    OwnerSide ownerSide = OwnerSide.Player;

    [SerializeField]
    Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    float Speed = 0.0f;

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

        Vector3 moveVector = MoveDirection.normalized * Speed *Time.deltaTime;

        transform.position += moveVector;
    }

    public void Fire(OwnerSide fireOwner, Vector3 firePosition, Vector3 direction, float speed)
    {
        ownerSide = fireOwner;
        transform.position = firePosition;
        MoveDirection = direction;
        Speed = speed;

    }

}
