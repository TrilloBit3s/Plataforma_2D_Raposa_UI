using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoPlayerJoystick : MonoBehaviour
{
    private float horizontalMove = 0f;  
   // private float verticalMove = 0f;  
    public Joystick joystick;
    public float runSpeedHorizontal = 2f;
    public float runSpeed = 1.25f;
    public float jumpSpeed = 10f;
    public float doubleJumpSpeed = 4f;
    private bool canDoubleJump;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    Rigidbody2D rb2D;

    public static bool isGrounded;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        animator.SetFloat("speedX", Mathf.Abs(horizontalMove));
        animator.SetBool("isGround", isGrounded);
        animator.SetFloat("speedY", rb2D.velocity.y);

        if(horizontalMove > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if(horizontalMove < 0)
        {
            spriteRenderer.flipX = true;
        }

        FixedUpdate();
    }

    void FixedUpdate() 
    {
        horizontalMove =  joystick.Horizontal * runSpeedHorizontal;    
        transform.position += new Vector3(horizontalMove, 0, 0) * Time.deltaTime * runSpeed;  
    }

    public void Jump()
    {
        if(isGrounded== true)
        {
            canDoubleJump = true;
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpSpeed);
        }
        else
        {
            
            if(canDoubleJump)
            {
              //  animator.SetBool("DoubleJump", true);
                rb2D.velocity = new Vector2(rb2D.velocity.x, doubleJumpSpeed);
                canDoubleJump = false;
            }
            
        }
    }

    //saber se esta no chão
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        isGrounded = true; 
        Debug.Log("esta no chão");  
    }
    //saber se saiu do chão
    private void OnTriggerExit2D(Collider2D other) 
    {
        isGrounded = false;    
        Debug.Log("saiu do chão");  
    }
}
