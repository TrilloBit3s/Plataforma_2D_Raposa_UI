using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    Player controller;

    public BoxCollider2D plataformGround;

    [HideInInspector]
    public bool onLadder = false;

    public float climbSpeed;
    public float exitHop = 3f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        controller = GetComponent<Player>();    
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("ladder"))
        {
            if(Input.GetAxisRaw("Vertical") !=0)
            {
                rb.velocity = new Vector2(rb.velocity.x, Input.GetAxisRaw("Vertical") * climbSpeed);
                rb.gravityScale = 0;//para a fisica nao afetar a gravidade do player
                onLadder = true;
                plataformGround.enabled = false;// desabilitar a plataforma quando passar por baixo ou por cima
                controller.usingLadder = onLadder;
            }
            else if(Input.GetAxisRaw("Vertical") == 0 && onLadder)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            anim.SetBool("onLadder", onLadder);
            anim.SetFloat("speedY", Mathf.Abs(Input.GetAxisRaw("Vertical")));
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("ladder") && onLadder)
        {
            rb.gravityScale = 2;
            onLadder = false;
            controller.usingLadder = onLadder;
            plataformGround.enabled = true;

            anim.SetBool("onLadder", onLadder);

           if(!controller.isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, exitHop);
        }    
    }
}