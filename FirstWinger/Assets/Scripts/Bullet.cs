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
    const float LifeTime = 15.0f;

    OwnerSide ownerSide = OwnerSide.Player;

    [SerializeField]
    Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    float Speed = 0.0f;

    bool NeedMove = false;

    float FiredTime;
    bool Hited = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ProcessDisapperCondition())
            return;

        UpdateMove();
    }

    void UpdateMove()
    {
        if (!NeedMove)
            return;


        Vector3 moveVector = MoveDirection.normalized * Speed * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;

    }

    public void Fire(OwnerSide FireOwner, Vector3 firePosition, Vector3 direction, float speed)
    {
        ownerSide = FireOwner;
        transform.position = firePosition;
        MoveDirection = direction;
        Speed = speed;

        NeedMove = true;
        FiredTime = Time.time;
    }

    Vector3 AdjustMove(Vector3 moveVector)
    {
        RaycastHit hitInfo;

        if (Physics.Linecast(transform.position, transform.position + moveVector, out hitInfo))
        {
            moveVector = hitInfo.point - transform.position;
            OnBulletCollision(hitInfo.collider);
        }

        return moveVector;

    }


    void OnBulletCollision(Collider collider)
    {
        if (Hited)
            return;

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        Hited = true;
        NeedMove = false;

        if (ownerSide == OwnerSide.Player)
        {
            Enemy enemy = collider.GetComponentInParent<Enemy>();
        }
        else
        {
            Player player = collider.GetComponentInParent<Player>();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        OnBulletCollision(other);
    }

    bool ProcessDisapperCondition()
    {
        if (transform.position.x > 15.0f || transform.position.x < -15.0f
            || transform.position.y > 15.0f || transform.position.y < -15.0f)
        {
            Disapper();
            return true;
        }
        else if(Time.time - FiredTime > LifeTime)
        {
            Disapper();
            return true;
        }


        return false;
    }

    void Disapper()
    {
        Destroy(gameObject);
    }
}
