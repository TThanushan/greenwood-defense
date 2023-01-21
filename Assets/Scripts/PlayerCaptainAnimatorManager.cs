using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCaptainAnimatorManager : MonoBehaviour
{
    Animator animator;
    Unit unit;
    Vector3 startPos;
    Transform unitSpriteTransform;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        startPos = unitSpriteTransform.localPosition;
    }


    private void PlayAttackAnimation()
    {
        animator.Play("Attack");
    }


}
