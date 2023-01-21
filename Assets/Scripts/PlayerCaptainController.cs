using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCaptainController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float attackSpeed = 1;
    int wayX;
    float attackSpeedCooldown;
    Transform sprite;
    Animator animator;
    private void Start()
    {
        sprite = transform.GetComponent<Unit>().GetSpriteTransform().parent;
         animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        { 
            wayX = -1;
            sprite.Rotate(0, 180f, 0, Space.Self);
        }

        if (Input.GetKeyDown(KeyCode.D))
        { wayX = 1; }


        if (Input.GetMouseButtonDown(0) && attackSpeedCooldown <= Time.time)
        {
            Attack();
            attackSpeedCooldown = attackSpeed + Time.time;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("IsRunning", true);
            MoveFoward();
        }
        else
            animator.SetBool("IsRunning", false);


    }



    void Attack()
    {
        string name = "SwordAttack";
        //if (animator.HasState(0, Animator.StringToHash(name)))
            GetComponent<Animator>().Play(name);
    }

    void MoveFoward()
    {
        transform.Translate(new Vector2(moveSpeed * wayX * Time.deltaTime, 0));

        //string name = "Run";
        //if (animator.HasState(0, Animator.StringToHash(name)))
        //    GetComponent<Animator>().Play(name);
    }
}
