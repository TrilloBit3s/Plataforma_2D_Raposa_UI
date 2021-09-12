using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float speed;
    Rigidbody2D rb;
    Transform groundCheck;
    
    bool facingRight = false;
    bool noChao = false;
    
    public Transform headPoint;//colisor na cabeça para saber quando o player pisou em cima
    public float jumpForce = 3000;//700

    public BoxCollider2D boxCollider2D;
    //public BoxCollider2D boxCollider2D2;

    AudioSource audioS;

	void Start () 
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("EnemyGroundCheck");
        audioS = gameObject.GetComponent<AudioSource>();
	}
	
	void Update () 
    {
        noChao = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("ground"));
        if (!noChao)
            speed *= -1;
	}

    void FixedUpdate()
    {
        rb.velocity = new Vector2(speed, rb.velocity.y);

        if(speed > 0 && !facingRight)
        {
            Flip();
        }

        else if(speed < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    bool playerDestroyed = false;
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            audioS.Play();
            float height = col.contacts[0].point.y - headPoint.position.y;
           //Debug.Log(height);//se for Negativo Destroi o Player, se for positivo Destroi o Inimigo
            if (height > 0 && !playerDestroyed)
            {
                //col.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 4, ForceMode2D.Impulse);
                speed = 0;
                boxCollider2D.enabled = false;
                //boxCollider2D2.enabled = false;
                rb.bodyType = RigidbodyType2D.Kinematic;
                //Debug.Log("matou o Inimigo");

                col.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));

                Destroy(gameObject, 0.25f);
            }/*else
            {
                playerDestroyed = true;
                //GameController.instance.ShowGameOver();
                Destroy(col.gameObject);
                //Debug.Log("matou o jogador");
            }*/
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
         if (col.gameObject.CompareTag("Player"))
        {
            audioS.Play();
            BoxCollider2D[] boxes = gameObject.GetComponents<BoxCollider2D>();
            foreach(BoxCollider2D box in boxes)
            {
                box.enabled = false;
            }

            col.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));

            speed = 0;
            transform.Rotate(new Vector3(0, 0, -180));
            Destroy(gameObject, 3);
        }
    }
}